using SmartThermo.Services.DeviceConnector.Enums;
using SmartThermo.Services.DeviceConnector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SmartThermo.Services.DeviceConnector.BitExtensions;

namespace SmartThermo.Services.DeviceConnector
{
    public class DeviceConnectorTest : IDeviceConnector
    {
        #region Event

        public event EventHandler<StatusConnect> StatusConnectChanged;
        public event EventHandler<List<SensorInfoEventArgs>> RegistersRequested;

        #endregion

        #region Field

        private readonly Random _random;
        private readonly Timer _timer;

        #endregion

        #region Property

        public StatusConnect StatusConnect { get; private set; }
        public SettingPortDevice SettingPortPort { get; set; }

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
                data[i] = (ushort) (i + 30);

            var result = data.Select((x , index)=> new SensorInfoEventArgs
            {
                Id = index + 1,
                Temperature = (byte)data[index],
                TimeLastBroadcast = (byte)((data[index] & 0b0011_1111_0000_0000) >> 8),
                IsEmergencyDescent = data[index].IsBitSet(14),
                IsAir = data[index].IsBitSet(15)
            }).ToList();
            RegistersRequested?.Invoke(this, result);
        }

        public Task Open()
        {
            StartTimer();

            StatusConnect = StatusConnect.Connected;
            StatusConnectChanged?.Invoke(this, StatusConnect);

            return Task.CompletedTask;
        }

        public void Close(bool notification = true)
        {
            StopTimer();

            if (!notification)
                return;
            StatusConnect = StatusConnect.Disconnected;
            StatusConnectChanged?.Invoke(this, StatusConnect);
        }

        public async Task<List<LimitTriggerEventArgs>> GetLimitTriggerDevice()
        {
            await Task.Delay(250);

            return Enumerable.Range(0, 6)
                .Select(_ => new LimitTriggerEventArgs
                {
                    UpperValue = _random.Next(40, 60),
                    LowerValue = _random.Next(10, 30)
                }).ToList();
        }

        public Task SetLimitTriggerDevice(List<LimitTriggerEventArgs> limitTriggers)
        {
            throw new NotImplementedException();
        }

        private void StartTimer()
        {
            _timer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3));
        }

        private void StopTimer()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public Task<SettingDevice> GetSettingDevice()
        {
            throw new NotImplementedException();
        }

        public Task SetSettingDevice(SettingDevice settingDevice)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
