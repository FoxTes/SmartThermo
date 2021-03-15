using LiveCharts;
using LiveCharts.Configurations;
using SmartThermo.Core.Mvvm;
using SmartThermo.Modules.DataViewer.Models;
using SmartThermo.Modules.Dialog.SettingsDevice.Enums;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.DeviceConnector.BitExtensions;
using SmartThermo.Services.DeviceConnector.Enums;
using SmartThermo.Services.DeviceConnector.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;

namespace SmartThermo.Modules.DataViewer.ViewModels
{
    public class DataViewerWindowViewModel : ViewModelBase
    {
        #region Field

        private static readonly object Lock = new object();

        private readonly IDeviceConnector _deviceConnector;
        private ObservableCollection<LimitRelay> _limitRelayItems = new ObservableCollection<LimitRelay>();
        private ObservableCollection<SensorsEther> _sensorsEtherItems = new ObservableCollection<SensorsEther>();

        private double _axisMax;
        private double _axisMin;

        #endregion

        #region Property

        public ObservableCollection<LimitRelay> LimitRelayItems
        {
            get => _limitRelayItems;
            set => SetProperty(ref _limitRelayItems, value);
        }

        public ObservableCollection<SensorsEther> SensorsEtherItems
        {
            get => _sensorsEtherItems;
            set => SetProperty(ref _sensorsEtherItems, value);
        }

        public List<ChartValues<MeasureData>> ChartValues { get; set; }

        public Func<double, string> DateTimeFormatter { get; set; }

        public double AxisStep { get; set; }

        public double AxisUnit { get; set; }

        public double AxisMax
        {
            get => _axisMax;
            set => SetProperty(ref _axisMax, value);
        }

        public double AxisMin
        {
            get => _axisMin;
            set => SetProperty(ref _axisMin, value);
        }

        #endregion

        #region Constructor

        public DataViewerWindowViewModel(IDeviceConnector deviceConnector)
        {
            _deviceConnector = deviceConnector;
            _deviceConnector.RegistersRequested += DeviceConnector_RegistersRequested;
            _deviceConnector.SettingDeviceChanged += DeviceConnector_SettingDeviceChanged;
            _deviceConnector.StatusConnectChanged += DeviceConnector_StatusConnectChanged;

            InitCharts();
            BindingOperations.EnableCollectionSynchronization(SensorsEtherItems, Lock);     
        }

        #endregion

        #region Method

        private void DeviceConnector_StatusConnectChanged(object sender, StatusConnect e)
        {
            if (e == StatusConnect.Disconnected)
                SensorsEtherItems.Clear();
        }

        private void DeviceConnector_SettingDeviceChanged(object sender, SettingDeviceEventArgs e)
        {
            SetRelayLimits();
        }

        private void DeviceConnector_RegistersRequested(object sender, List<SensorInfoEventArgs> e)
        {
            var data = e.Where(x => x.IsAir)
                        .Select(x => new SensorsEther
                        {
                            Id = x.Id,
                            Time = x.TimeLastBroadcast
                        })
                        .ToList();

            SensorsEtherItems.Clear();
            SensorsEtherItems.AddRange(data);

            var now = DateTime.Now;
            for (var i = 0; i < 6; i++)
                ChartValues[i].Add(new MeasureData
                {
                    DateTime = now,
                    Value = e[i].Temperature
                });

            SetAxisLimits(now);
            foreach (var item in ChartValues.Where(item => item.Count > 30))
                item.RemoveAt(0);
        }

        private void InitCharts()
        {
            var mapper = Mappers.Xy<MeasureData>()
                .X(model => model.DateTime.Ticks)
                .Y(model => model.Value);
            Charting.For<MeasureData>(mapper);

            ChartValues = new List<ChartValues<MeasureData>>();
            for (var i = 0; i < 6; i++)
            {
                ChartValues.Add(new ChartValues<MeasureData>());
                LimitRelayItems.Add(new LimitRelay());
            }

            DateTimeFormatter = value => new DateTime((long)value).ToString("mm:ss");
            AxisStep = TimeSpan.FromSeconds(10).Ticks;
            AxisUnit = TimeSpan.TicksPerSecond;
            SetAxisLimits(DateTime.Now);
            SetRelayLimits();
        }

        private void SetRelayLimits()
        {
            var data = _deviceConnector.SettingDevice;

            SetDataFromSetting(data.BindingSensorRelay1);
            SetDataFromSetting(data.BindingSensorRelay2);
            SetDataFromSetting(data.BindingSensorRelay3);

            RaisePropertyChanged(nameof(LimitRelayItems));
        }

        private void SetDataFromSetting(ushort data)
        {
            var mode = (BindingRelay)((data & 0b0000_0011_0000_0000) >> 8);
            switch (mode)
            {
                case BindingRelay.TemperatureThreshold1:
                {
                    for (var i = 0; i < LimitRelayItems.Count; i++)
                    {
                        LimitRelayItems[i].TemperatureThreshold1 = (byte) (data.IsBitSet(i) ? _deviceConnector.SettingDevice.TemperatureThreshold[0] : 0);
                        LimitRelayItems[i].HysteresisThreshold1 = (byte) (_deviceConnector.SettingDevice.TemperatureHysteresis &
                                                                          0b0000_0000_1111_1111);
                    }
                    break;
                }
                case BindingRelay.TemperatureThreshold2:
                {
                    for (var i = 0; i < LimitRelayItems.Count; i++)
                    {
                        LimitRelayItems[i].TemperatureThreshold2 = (byte) (data.IsBitSet(i) ? _deviceConnector.SettingDevice.TemperatureThreshold[1] : 0);
                        LimitRelayItems[i].HysteresisThreshold2 = (byte) (_deviceConnector.SettingDevice.TemperatureHysteresis >> 8);
                    }
                    break;
                }
                case BindingRelay.NotUsed:
                    break;
                case BindingRelay.СommunicationChanelFailure:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetAxisLimits(DateTime now)
        {
            AxisMax = now.Ticks + TimeSpan.FromSeconds(1).Ticks;
            AxisMin = now.Ticks - TimeSpan.FromSeconds(59).Ticks;
        }

        #endregion
    }
}
