using SmartThermo.Core.Extensions;
using SmartThermo.Modules.Dialog.SettingsSensor.Enums;
using SmartThermo.Modules.Dialog.SettingsSensor.Helpers;
using SmartThermo.Modules.Dialog.SettingsSensor.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartThermo.Modules.Dialog.SettingsSensor.Services
{
    public class SensorTuner
    {
        #region Const

        private const int IntervalDelay = 2000;
        private const int CountByteReceive = 64;

        #endregion

        #region Field

        private readonly SerialPort _serialPort;

        #endregion

        #region Property

        public SettingPortSensor SettingPortSensor { get; set; }

        #endregion

        #region Constructor

        public SensorTuner()
        {
            _serialPort = new SerialPort();
        }

        #endregion
        
        #region Method

        public async Task<string> ExecuteCommand(Command command)
        {
            OpenPort();

            if (command.Address != 0x03 && command.Address != 0x13)
            {
                switch (command.TypeCommand)
                {
                    case TypeCommand.Write:
                        await WriteCommand(command, new CancellationToken());
                        break;
                    case TypeCommand.Read:
                        return await ReadCommand(command, new CancellationToken());
                }
            }
            else
            {
                switch (command.TypeCommand)
                {
                    case TypeCommand.Write:
                        await WriteArrayToDevice(command);
                        break;
                    case TypeCommand.Read:
                        await ReadArrayToDevice(command);
                        break;
                }
            }
            return "";
        }

        public void ClearUpPort()
        {
            _serialPort.Close();
        }

        private void OpenPort()
        {
            _serialPort.PortName = SettingPortSensor.NamePort;
            _serialPort.BaudRate = SettingPortSensor.BaudRate;
            _serialPort.StopBits = SettingPortSensor.StopBits;
            _serialPort.Open();
        }

        private async Task WriteCommand(Command command, CancellationToken cancellationToken)
        {
            var byteWrite = new List<byte> { 0x55, command.Address };

            byteWrite.AddRange(command.Bytes);
            byteWrite.Add(byteWrite.Skip(1).CrcCalc());

            await WriteDataAsync(byteWrite, cancellationToken);
        }

        private async Task<string> ReadCommand(Command command, CancellationToken cancellationToken)
        {
            var byteWrite = new List<byte> { 0x55, command.Address, command.Address };
            return await ReadDataAsync(command, byteWrite, cancellationToken);
        }

        public async Task WakeUpCommand(CancellationToken cancellationToken)
        {
            OpenPort();

            while (true)
            {
                await Task.Delay(100, cancellationToken);

                if (_serialPort.BytesToRead != 4)
                {
                    _serialPort.DiscardOutBuffer();
                    _serialPort.DiscardInBuffer();
                    continue;
                }

                var byteRead = new byte[4];
                _serialPort.Read(byteRead, 0, 4);

                var crc = byteRead.Take(byteRead.Length - 1).Skip(1).CrcCalc();
                if (crc != byteRead[^1])
                    continue;

                await ReadCommand(new Command
                {
                    Name = "Чтение номера канала",
                    TypeCommand = TypeCommand.Read,
                    Address = 0x05,
                    Bytes = new List<byte> { 0x00 },
                    TypeValue = typeof(byte)
                }, cancellationToken);

                return;
            }
        }

        private async Task WriteArrayToDevice(Command command)
        {
            var excelProvider = ExcelProvider.ExtractDataFromFile();
            var byteWrite = new List<byte> { 0x55, command.Address };

            foreach (var value in excelProvider.Select(item => (ushort)(Convert.ToSingle(item) / 0.001d)))
                byteWrite.AddRange(BitConverter.GetBytes(value));
            byteWrite.Add(byteWrite.Skip(1).CrcCalc());

            await WriteDataAsync(byteWrite, new CancellationToken());
        }

        private async Task ReadArrayToDevice(Command command)
        {
            var byteWrite = new List<byte> { 0x55, command.Address, command.Address };

            _serialPort.DiscardOutBuffer();
            _serialPort.DiscardInBuffer();
            _serialPort.Write(byteWrite.ToArray(), 0, byteWrite.Count);

            await Task.Delay(IntervalDelay);

            var byteReadCount = byteWrite.Count + CountByteReceive;
            if (_serialPort.BytesToRead != byteReadCount)
                throw new Exception($"Ошибка ответа датчика. Кол-во принятых байт {_serialPort.BytesToRead}.");

            var byteRead = new byte[byteReadCount];
            _serialPort.Read(byteRead, 0, byteReadCount);

            var crc = byteRead.Take(byteRead.Length - 1).Skip(1).CrcCalc();
            if (crc != byteRead[^1])
                throw new Exception("Ошибка ответа датчика. Неверный CRC.");

            var outArray = new List<string>();
            for (var i = 2; i < (byteRead.Length - 1); i += 2)
            {
                var value = BitConverter.ToUInt16(byteRead, i) * 0.001d;
                outArray.Add(value.ToString(CultureInfo.InvariantCulture));
            }

            ExcelProvider.UploadDataToFile(outArray);
        }

        private async Task WriteDataAsync(List<byte> byteWrite, CancellationToken cancellationToken)
        {
            _serialPort.DiscardOutBuffer();
            _serialPort.DiscardInBuffer();
            _serialPort.Write(byteWrite.ToArray(), 0, byteWrite.Count);

            await Task.Delay(IntervalDelay, cancellationToken);

            var byteToRead = _serialPort.BytesToRead;
            if (byteToRead != byteWrite.Count)
                throw new Exception($"Ошибка ответа датчика. Кол-во принятых байт {byteToRead}.");

            var byteRead = new byte[byteWrite.Count];
            _serialPort.Read(byteRead, 0, byteWrite.Count);

            byteRead[1] -= 0x40;
            byteRead[^1] -= 0x40;

            if (!byteRead.SequenceEqual(byteWrite))
                throw new Exception("Ошибка ответа датчика. Неверный CRC.");
        }

        private async Task<string> ReadDataAsync(Command command, List<byte> byteWrite, CancellationToken cancellationToken)
        {
            _serialPort.DiscardOutBuffer();
            _serialPort.DiscardInBuffer();
            _serialPort.Write(byteWrite.ToArray(), 0, byteWrite.Count);

            await Task.Delay(IntervalDelay, cancellationToken);

            var byteReadCount = byteWrite.Count + command.Bytes.Count;
            var byteToRead = _serialPort.BytesToRead;

            if (byteToRead != byteReadCount)
                throw new Exception($"Ошибка ответа датчика. Кол-во принятых байт {byteToRead}.");

            var byteRead = new byte[byteReadCount];
            _serialPort.Read(byteRead, 0, byteReadCount);

            var crc = byteRead.Take(byteRead.Length - 1).Skip(1).CrcCalc();
            if (crc != byteRead[^1])
                throw new Exception("Ошибка ответа датчика. Неверный CRC.");

            var result = string.Empty;
            if (command.TypeValue == typeof(byte))
                result = (byteRead.Skip(2).FirstOrDefault() * command.Coefficient).ToString();
            else if (command.TypeValue == typeof(ushort))
                result = (BitConverter.ToUInt16(byteRead, 2) * command.Coefficient).ToString();

            return result;
        }

        #endregion
    }
}
