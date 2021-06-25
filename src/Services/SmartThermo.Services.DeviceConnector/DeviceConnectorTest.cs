using SmartThermo.Services.DeviceConnector.Enums;
using SmartThermo.Services.DeviceConnector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SmartThermo.Core.Extensions;

namespace SmartThermo.Services.DeviceConnector
{
    public class DeviceConnectorTest : IDeviceConnector
    {
        #region Event

        public event EventHandler<StatusConnect> StatusConnectChanged;
        public event EventHandler<List<SensorInfoEventArgs>> RegistersRequested;  
        public event EventHandler<SettingDeviceEventArgs> SettingDeviceChanged;

        #endregion

        #region Const

        private const int TimeStart = 1;
        private const int TimePeriod = 3;
        private const int CountRegister = 36;

        #endregion

        #region Field

        private readonly Random _random;
        private readonly Timer _timer;
        private readonly ushort[] _sendData;

        #endregion

        #region Property

        public StatusConnect StatusConnect { get; private set; }

        public SettingPortDevice SettingPortPort { get; set; }

        public SettingDeviceEventArgs SettingDevice { get; private set; }

        #endregion

        #region Constructor

        public DeviceConnectorTest()
        {
            _random = new Random();
            _timer = new Timer(OnTimer, 0, Timeout.Infinite, Timeout.Infinite);
            _sendData = new ushort[CountRegister];

            StatusConnect = StatusConnect.Disconnected;
        }

        #endregion

        #region Method

        private void OnTimer(object o)
        {
            for (var i = 0; i < CountRegister; i++)
                _sendData[i] = (ushort)(0xAC00 + _random.Next(16128, 32256));

            var result = _sendData.Select((x, index) => new SensorInfoEventArgs
            {
                Id = index,
                Number = (index / 6 + 1) * 10 + index + 1 - 6 * (index / 6),
                Temperature = index switch
                {
                    1 => (byte)(_random.Next(25, 35) + Math.Sin(index * 0.03d) * 20),
                    2 => (byte)(_random.Next(50, 59) + Math.Cos(index * 0.01d) * 20),
                    3 => (byte)(_random.Next(42, 49) + Math.Sin(index * 0.1d) * 20),
                    4 => (byte)(_random.Next(86, 89) + Math.Cos(index * 1d) * 20),
                    5 => (byte)(_random.Next(110, 119) + Math.Cos(index * 2d) * 20),
                    6 => (byte)(_random.Next(145, 159) + Math.Sin(index * 0.05d) * 20),
                    _ => (byte)(_random.Next(5, 9) + Math.Cos(index * 0.03d) * 20),
                },
                TimeLastBroadcast = (byte) ((_sendData[index] & 0b0011_1111_0000_0000) >> 8),
                IsEmergencyDescent = _sendData[index].IsBitSet(14),
                IsAir = _random.Next(0, 10) < 9
            }).ToList();
            RegistersRequested?.Invoke(this, result);
        }

        public async Task Open()
        {
            await GetSettingDevice();
            StartTimer();

            StatusConnect = StatusConnect.Connected;
            StatusConnectChanged?.Invoke(this, StatusConnect);
        }

        public void Close(bool notification = true)
        {
            StopTimer();

            if (!notification)
                return;
            StatusConnect = StatusConnect.Disconnected;
            StatusConnectChanged?.Invoke(this, StatusConnect);
        }

        private void StartTimer()
        {
            _timer.Change(TimeSpan.FromSeconds(TimeStart), TimeSpan.FromSeconds(TimePeriod));
        }

        private void StopTimer()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public async Task GetSettingDevice()
        {
            await Task.Delay(500);
            SettingDevice = new SettingDeviceEventArgs()
            {
                AddressDevice = 3,
                Speed = 0,
                Parity = 1,
                NumberChanelId = 1029,
                TemperatureThreshold = new List<ushort> { 50, 30 },
                TemperatureHysteresis = 1285,
                DelaySignalRelays = 257,
                BindingSensorRelay1 = 513,
                BindingSensorRelay2 = 769,
                BindingSensorRelay3 = 0,
                StatusAlarmRelay = 7
            };
        }

        public async Task SetSettingDevice(SettingDeviceEventArgs settingDevice)
        {
            SettingDevice = settingDevice;
            await Task.Delay(500);
            SettingDeviceChanged?.Invoke(this, settingDevice);
        }

        #endregion
    }
}
