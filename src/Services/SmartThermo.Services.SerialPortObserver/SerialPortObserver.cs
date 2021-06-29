using System;
using System.IO.Ports;
using System.Linq;
using System.Management;
using SmartThermo.Services.Configuration;
using SmartThermo.Services.SerialPortObserver.Enums;
using SmartThermo.Services.SerialPortObserver.Models;

namespace SmartThermo.Services.SerialPortObserver
{
    /// <inheritdoc />
    public class SerialPortObserver : ISerialPortObserver
    {
        private string[] _serialPorts;
        private ManagementEventWatcher _arrival;
        private ManagementEventWatcher _removal;

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        /// <param name="configuration">Конфигурация.</param>
        public SerialPortObserver(IConfiguration configuration)
        {
            _serialPorts = GetAvailableSerialPorts();
            MonitoringDeviceChanges();

            if (configuration.IsAutoConnect)
                Start();
        }

        /// <inheritdoc />
        public event EventHandler<SerialPortChangedArgs> SerialPortChanged;

        /// <inheritdoc />
        public void Start()
        {
            _arrival.Start();
            _removal.Start();
        }

        /// <inheritdoc />
        public void Stop()
        {
            _arrival.Stop();
            _removal.Stop();
        }

        private void MonitoringDeviceChanges()
        {
            try
            {
                var deviceArrivalQuery
                    = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2");
                var deviceRemovalQuery
                    = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 3");

                _arrival = new ManagementEventWatcher(deviceArrivalQuery);
                _removal = new ManagementEventWatcher(deviceRemovalQuery);

                _arrival.EventArrived += (o, args) =>
                    RaisePortsChangedIfNecessary(NotifySerialPortChangedAction.Add);
                _removal.EventArrived += (sender, eventArgs) =>
                    RaisePortsChangedIfNecessary(NotifySerialPortChangedAction.Remove);
            }
            catch (ManagementException)
            {
            }
        }

        private void RaisePortsChangedIfNecessary(NotifySerialPortChangedAction notifySerialPortChangedAction)
        {
            lock (_serialPorts)
            {
                var availableSerialPorts = GetAvailableSerialPorts();
                if (_serialPorts.SequenceEqual(availableSerialPorts))
                    return;

                _serialPorts = availableSerialPorts;
                SerialPortChanged?.Invoke(null, new SerialPortChangedArgs(notifySerialPortChangedAction, _serialPorts));
            }
        }

        private string[] GetAvailableSerialPorts() => SerialPort.GetPortNames();
    }
}