﻿using LiveCharts;
using LiveCharts.Configurations;
using Prism.Commands;
using Prism.Regions;
using SmartThermo.Core.Extensions;
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using SmartThermo.Core.Enums;
using SmartThermo.Services.Configuration;

namespace SmartThermo.Modules.DataViewer.ViewModels.Represent
{
    public class LoadDataViewerWindowViewModel : ViewModelBase, IRegionMemberLifetime
    {
        #region Const

        private const double MaxAxisYValue = 165d;
        private const double MinAxisYValue = 0d;

        #endregion

        #region Field

        private static readonly object Lock = new object();

        private readonly IConfiguration _configuration;
        private readonly IDeviceConnector _deviceConnector;
        private readonly List<int> _groupSensorId = new List<int>();
        private ObservableCollection<LimitRelay> _limitRelayItems = new ObservableCollection<LimitRelay>();
        private ObservableCollection<SensorsEther> _sensorsEtherItems = new ObservableCollection<SensorsEther>();

        private double _axisMax;
        private double _axisMin;
        private List<double> _axisYMax = new List<double>();
        private List<double> _axisYMin = new List<double>();
        private List<bool> _selectMode = new List<bool>();
        private List<SensorsData> _sensorsData = new List<SensorsData>();

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

        public List<double> AxisYMax
        {
            get => _axisYMax;
            set => SetProperty(ref _axisYMax, value);
        }

        public List<double> AxisYMin
        {
            get => _axisYMin;
            set => SetProperty(ref _axisYMin, value);
        }

        public List<bool> SelectMode
        {
            get => _selectMode;
            set => SetProperty(ref _selectMode, value);
        }

        public List<SensorsData> SensorsData
        {
            get => _sensorsData;
            set => SetProperty(ref _sensorsData, value);
        }

        public DelegateCommand<int?> ChangeSelectModeCommand { get; }

        public DelegateCommand<int?> ResetScalingChartCommand { get; }

        public bool KeepAlive => true;

        #endregion

        #region Constructor

        public LoadDataViewerWindowViewModel(
            IConfiguration configuration, 
            IDeviceConnector deviceConnector, 
            INotifications notifications)
        {
            _configuration = configuration;
            _deviceConnector = deviceConnector;
            _deviceConnector.SettingDeviceChanged += DeviceConnector_SettingDeviceChanged;
            _deviceConnector.StatusConnectChanged += DeviceConnector_StatusConnectChanged;

            InitCharts();
            GetIdGroupsSensorAsync();
            BindingOperations.EnableCollectionSynchronization(SensorsEtherItems, Lock);

            ResetScalingChartCommand =
                new DelegateCommand<int?>(i =>
                {
                    if (i == null)
                        return;

                    notifications.ShowInformation($"Сброшен масштаб графика №{i}.");
                    AxisYMax[(int)i - 1] = MaxAxisYValue;
                    AxisYMin[(int)i - 1] = MinAxisYValue;
                    RaisePropertyChanged(nameof(AxisYMax));
                    RaisePropertyChanged(nameof(AxisYMin));
                });
            ChangeSelectModeCommand =
                new DelegateCommand<int?>(i =>
                {
                    if (i == null)
                        return;

                    SelectMode[(int)i - 1] = !SelectMode[(int)i - 1];
                    RaisePropertyChanged(nameof(SelectMode));

                    using var context = new Context();
                    var selectMode = context.SelectModes
                        .First(x => x.Id == i);
                    selectMode.Stage = _selectMode[(int)i - 1];
                    context.SaveChanges();
                });
        }

        private async void GetIdGroupsSensorAsync()
        {
            var groupIdTask = Task.Run(() =>
            {
                using var context = new Context();
                var sessionId = context.Sessions
                    .Max(p => p.Id);
                return context.GroupSensors
                    .Where(x => x.SessionId == sessionId)
                    .Select(x => x.Id)
                    .ToList();
            });
            await Task.WhenAll(groupIdTask); 
            _groupSensorId.AddRange(groupIdTask.Result); 

            _deviceConnector.RegistersRequested += DeviceConnector_RegistersRequested;
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
            var sensorEtherItems = sensorData.Where(x => x.IsAir)
                .Select(x => new SensorsEther
                {
                    Id = x.Number,
                    Time = x.TimeLastBroadcast
                })
                .ToList();
            SensorsEtherItems.Clear();
            SensorsEtherItems.AddRange(sensorEtherItems);

            _sensorsData.Clear();
            _sensorsData.AddRange(sensorData
                .Select(x => new SensorsData
                { 
                    Temperature = x.IsAir ? (int?)x.Temperature : null, 
                    StatusSensor = x.IsAir ? GetStatusSensor(x.TimeLastBroadcast) : StatusSensor.Offline, 
                })
                .ToList());
            RaisePropertyChanged(nameof(SensorsData));

            var now = DateTime.Now.Round(TimeSpan.FromSeconds(1));
            Application.Current?.Dispatcher?.InvokeAsync(() =>
            {
                for (var i = 0; i < 36; i++)
                {
                    var sensorItem = sensorData[i];
                    ChartValues[i].Add(new MeasureData
                    {
                        DateTime = now,
                        Value =  sensorItem.IsAir ? sensorItem.Temperature : double.NaN
                    });
                }
            }, DispatcherPriority.Background);

            SetAxisXLimits(now);
            foreach (var item in ChartValues.Where(item => item.Count > 25))
                item.RemoveAt(0);

            if (_configuration.IsWriteToDatabase)
                SaveDataToDatabaseAsync(now, sensorData);
        }
        
        private StatusSensor GetStatusSensor(int time)
        {
            return time switch
            {
                var n when n < _configuration.TimeBeforeWarning => StatusSensor.Online,
                var n when n >= _configuration.TimeBeforeWarning => StatusSensor.Wait,
                var n when n >= _configuration.TimeBeforeOffline => StatusSensor.Offline, 
                _ => throw new ArgumentOutOfRangeException(nameof(time), time, null)
            };
        }

        private async void SaveDataToDatabaseAsync(DateTime time, IReadOnlyList<SensorInfoEventArgs> sensorData)
        {
            var saveDataTask = Task.Run(() =>
            {
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

                using var context = new Context();
                context.SensorInformations.AddRange(result);
                return context.SaveChanges();
            });
            await Task.WhenAll(saveDataTask);
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
            SensorsData.AddRange(Enumerable.Range(0,36)
                .Select(x => new SensorsData()
                { 
                    Temperature = default , 
                    StatusSensor = StatusSensor.Offline
                })
                .ToList());
            LimitRelayItems.AddRange(Enumerable.Range(0, 6)
                .Select(x => new LimitRelay())
                .ToList());
            GetSelectMode();

            XFormatter = value => new DateTime((long)value).ToString("mm:ss");
            YFormatter = value => Math.Round(value, 1).ToString(CultureInfo.InvariantCulture);
            AxisStep = TimeSpan.FromSeconds(10).Ticks;
            AxisUnit = TimeSpan.TicksPerSecond;
            SetAxisXLimits(DateTime.Now);
            SetAxisYLimits();
            SetRelayLimits();
        }
        
        private void GetSelectMode()
        {
            using var context = new Context();
            SelectMode.AddRange(context.SelectModes
                .Select(x => x.Stage)
                .ToList());
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
                                ? _deviceConnector.SettingDevice.TemperatureThreshold[0] -
                                  LimitRelayItems[i].HysteresisThreshold1 / 2d
                                : 0;
                        }
                        break;
                    }
                case BindingRelay.TemperatureThreshold2:
                    {
                        for (var i = 0; i < LimitRelayItems.Count; i++)
                        {
                            LimitRelayItems[i].HysteresisThreshold2 = data.IsBitSet(i)
                                ? ((_deviceConnector.SettingDevice.TemperatureHysteresis & 0b1111_1111_0000_0000) >>
                                   8) * 2d
                                : 0;

                            LimitRelayItems[i].TemperatureThreshold2 = data.IsBitSet(i)
                                ? _deviceConnector.SettingDevice.TemperatureThreshold[1] -
                                  LimitRelayItems[i].HysteresisThreshold2 / 2d
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

        private void SetAxisXLimits(DateTime now)
        {
            AxisMax = now.Ticks + TimeSpan.FromSeconds(1).Ticks;
            AxisMin = now.Ticks - TimeSpan.FromSeconds(60).Ticks;
        }

        private void SetAxisYLimits()
        {
            AxisYMin.AddRange(Enumerable.Range(0, 6).Select(x => MinAxisYValue).ToList());
            AxisYMax.AddRange(Enumerable.Range(0, 6).Select(x => MaxAxisYValue).ToList());
        }

        #endregion
    }
}
