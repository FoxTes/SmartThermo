using LiveCharts;
using LiveCharts.Configurations;
using Prism.Commands;
using SmartThermo.Core.Mvvm;
using SmartThermo.DataAccess.Sqlite;
using SmartThermo.DataAccess.Sqlite.Models;
using SmartThermo.Modules.DataViewer.Models;
using SmartThermo.Modules.Dialog.SettingsDevice.Enums;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.DeviceConnector.Enums;
using SmartThermo.Services.DeviceConnector.Models;
using SmartThermo.Services.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Prism.Regions;
using SmartThermo.Core.Extensions;

namespace SmartThermo.Modules.DataViewer.ViewModels
{
    public class DataViewerWindowViewModel : ViewModelBase, IRegionMemberLifetime
    {
        #region Field

        private static readonly object Lock = new object();

        private readonly IDeviceConnector _deviceConnector;
        private readonly List<int> _groupSensorId = new List<int>();
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

        public Func<double, string> XFormatter { get; set; }

        public Func<double, string> YFormatter { get; set; }

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

        public DelegateCommand<int?> ResetScalingChartCommand { get; }

        public bool KeepAlive => true;

        #endregion

        #region Constructor

        public DataViewerWindowViewModel(IDeviceConnector deviceConnector, INotifications notifications)
        {
            _deviceConnector = deviceConnector;
            _deviceConnector.RegistersRequested += DeviceConnector_RegistersRequested;
            _deviceConnector.SettingDeviceChanged += DeviceConnector_SettingDeviceChanged;
            _deviceConnector.StatusConnectChanged += DeviceConnector_StatusConnectChanged;

            InitCharts();
            GetIdGroupsSensorAsync();
            BindingOperations.EnableCollectionSynchronization(SensorsEtherItems, Lock);

            ResetScalingChartCommand =
                new DelegateCommand<int?>(i => notifications.ShowInformation($"Сброшен масштаб графика №{i}."));
        }

        private async void GetIdGroupsSensorAsync()
        {
            await using var context = new Context();
            var result = context.GroupSensors
                .Where(x => x.SessionId == context.Sessions.Max(p => p.Id))
                .Select(x => x.Id)
                .ToList();
            _groupSensorId.AddRange(result);
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

        private void DeviceConnector_RegistersRequested(object sender, List<SensorInfoEventArgs> sensorData)
        {
            var data = sensorData.Where(x => x.IsAir)
                .Select(x => new SensorsEther
                {
                    Id = x.Number,
                    Time = x.TimeLastBroadcast
                })
                .ToList();

            SensorsEtherItems.Clear();
            SensorsEtherItems.AddRange(data);

            var now = DateTime.Now.Round(TimeSpan.FromSeconds(1));


            foreach (var item in ChartValues.Where(item => item.Count > 30))
                item.RemoveAt(0);

            SaveDataToDatabaseAsync(now, sensorData);
        }

        private async void SaveDataToDatabaseAsync(DateTime time, IReadOnlyList<SensorInfoEventArgs> sensorData)
        {
            await using var context = new Context();
            var result = Enumerable.Range(0, 6)
                .Select((x, index) => new SensorInformation
                {
                    Value1 = sensorData[0 + index * 6].Temperature,
                    Value2 = sensorData[1 + index * 6].Temperature,
                    Value3 = sensorData[2 + index * 6].Temperature,
                    Value4 = sensorData[3 + index * 6].Temperature,
                    Value5 = sensorData[4 + index * 6].Temperature,
                    Value6 = sensorData[5 + index * 6].Temperature,
                    DataTime = time,
                    SensorGroupId = _groupSensorId[index]
                }).ToList();

            await context.SensorInformations.AddRangeAsync(result);
            await context.SaveChangesAsync();
        }

        private void InitCharts()
        {
            var mapper = Mappers.Xy<MeasureData>()
                .X(model => model.DateTime.Ticks)
                .Y(model => model.Value);
            Charting.For<MeasureData>(mapper);

            ChartValues = new List<ChartValues<MeasureData>>();
            ChartValues.AddRange(Enumerable.Range(0, 36)
                .Select(x => new ChartValues<MeasureData>())
                .ToList());
            LimitRelayItems.AddRange(Enumerable.Range(0, 6)
                .Select(x => new LimitRelay())
                .ToList());

            XFormatter = value => new DateTime((long)value).ToString("mm:ss");
            YFormatter = value => Math.Round(value, 1).ToString(CultureInfo.InvariantCulture);
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
                        LimitRelayItems[i].HysteresisThreshold1 = data.IsBitSet(i)
                            ? (_deviceConnector.SettingDevice.TemperatureHysteresis & 0b0000_0000_1111_1111) * 2d
                            : 0;

                        LimitRelayItems[i].TemperatureThreshold1 = data.IsBitSet(i)
                            ? _deviceConnector.SettingDevice.TemperatureThreshold[0] - LimitRelayItems[i].HysteresisThreshold1 / 2d
                            : 0;
                    }
                    break;
                }
                case BindingRelay.TemperatureThreshold2:
                {
                    for (var i = 0; i < LimitRelayItems.Count; i++)
                    {
                        LimitRelayItems[i].HysteresisThreshold2 = data.IsBitSet(i)
                            ? ((_deviceConnector.SettingDevice.TemperatureHysteresis & 0b1111_1111_0000_0000) >> 8) * 2d
                            : 0;

                        LimitRelayItems[i].TemperatureThreshold2 = data.IsBitSet(i)
                            ? _deviceConnector.SettingDevice.TemperatureThreshold[1] - LimitRelayItems[i].HysteresisThreshold2 / 2d
                            : 0;
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
            AxisMin = now.Ticks - TimeSpan.FromSeconds(60).Ticks;
        }

        #endregion
    }
}
