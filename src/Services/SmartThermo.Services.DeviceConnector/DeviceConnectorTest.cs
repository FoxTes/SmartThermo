using SmartThermo.Services.DeviceConnector.Enums;
using SmartThermo.Services.DeviceConnector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace SmartThermo.Services.DeviceConnector
{
    public class DeviceConnectorTest : IDeviceConnector
    {
        #region Event

        public event EventHandler<StatusConnect> StatusConnectChanged;
        public event EventHandler<List<SensorInfo>> RegistersRequested;

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
            _timer = new Timer { Interval = 3000, AutoReset = true };
            _timer.Elapsed += OnTimerElapsed;

            StatusConnect = StatusConnect.Disconnected;
        }

        #endregion

        #region Method

        private void OnTimerElapsed(object o, ElapsedEventArgs elapsedEventArgs)
        {
            var data = Enumerable.Range(0, 36)
                .Select(_ => new SensorInfo()
                {
                    Temperature = (byte)_random.Next(0, 155),
                    TimeLastBroadcast = (byte)_random.Next(0, 63),
                    IsEmergencyDescent = _random.Next(10) > 5,
                    IsAir = _random.Next(10) > 5,
                })
                .ToList();
            RegistersRequested?.Invoke(this, data);
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

            StatusConnect = StatusConnect.Disconnected;
            StatusConnectChanged?.Invoke(this, StatusConnect);
        }

        public async Task<List<LimitTrigger>> GetLimitTriggerDevice()
        {
            await Task.Delay(250);

            return Enumerable.Range(0, 6)
                .Select(_ => new LimitTrigger
                {
                    UpperValue = _random.Next(40, 60),
                    LowerValue = _random.Next(10, 30)
                }).ToList();
        }

        public Task SetLimitTriggerDevice(List<LimitTrigger> limitTriggers)
        {
            throw new NotImplementedException();
        }

        private void StartTimer()
        {
            _timer.Enabled = true;
        }

        private void StopTimer()
        {
            _timer.Enabled = false;
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
