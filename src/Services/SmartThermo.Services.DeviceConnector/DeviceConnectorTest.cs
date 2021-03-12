using SmartThermo.Services.DeviceConnector.BitExtensions;
using SmartThermo.Services.DeviceConnector.Enums;
using SmartThermo.Services.DeviceConnector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SmartThermo.Services.DeviceConnector
{
    public class DeviceConnectorTest : IDeviceConnector
    {
        #region Event

        public event EventHandler<StatusConnect> StatusConnectChanged;
        public event EventHandler<List<SensorInfoEventArgs>> RegistersRequested;
        public event EventHandler<SettingDeviceEventArgs> SettingDeviceChanged;

        #endregion

        #region Field

        private readonly Random _random;
        private readonly Timer _timer;

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

            StatusConnect = StatusConnect.Disconnected;
        }

        #endregion

        #region Method

        private void OnTimer(object o)
        {
            var data = new ushort[36];
            for (var i = 0; i < 36; i++) 
                data[i] = 0xAC00;

            var result = data.Select((x , index)=> new SensorInfoEventArgs
            {
                Id = index + 1,
                Temperature = (byte)_random.Next(40,60),
                TimeLastBroadcast = (byte)((data[index] & 0b0011_1111_0000_0000) >> 8),
                IsEmergencyDescent = data[index].IsBitSet(14),
                IsAir = data[index].IsBitSet(15)
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
            _timer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3));
        }

        private void StopTimer()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public Task<SettingDeviceEventArgs> GetSettingDevice()
        {
            //TODO: Написать заглушку.
            throw new NotImplementedException();
        }

        public Task SetSettingDevice(SettingDeviceEventArgs settingDevice)
        {
            //TODO: Написать заглушку.
            throw new NotImplementedException();
        }

        #endregion
    }
}
