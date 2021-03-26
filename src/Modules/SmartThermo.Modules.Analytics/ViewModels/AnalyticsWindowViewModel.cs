using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
using ScottPlot;
using SmartThermo.Core.Extensions;
using SmartThermo.Core.Models;
using SmartThermo.Core.Mvvm;
using SmartThermo.DataAccess.Sqlite;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SmartThermo.Modules.Analytics.ViewModels
{
    public class AnalyticsWindowViewModel : RegionViewModelBase
    {
        #region Field

        private readonly List<int> _groupSensorId = new List<int>();

        private WpfPlot _plotControl;
        private Plot _plot;
        private int _sensorGroupSelected;
        private ObservableCollection<ItemDescriptor<bool>> _groupCheckItems = new ObservableCollection<ItemDescriptor<bool>>();
        private string _dateCreateSession;
        private Visibility _showLoadIndicator = Visibility.Hidden;
        private int _currentSessionId;
        private bool _isLoadCurrentSession = true;

        #endregion

        #region Property

        public WpfPlot PlotControl
        {
            get => _plotControl;
            set => SetProperty(ref _plotControl, value);
        }

        public List<ItemDescriptor<int>> SensorGroups { get; }

        public int SensorGroupSelected
        {
            get => _sensorGroupSelected;
            set => SetProperty(ref _sensorGroupSelected, value);
        }

        public ObservableCollection<ItemDescriptor<bool>> GroupCheckItems
        {
            get => _groupCheckItems;
            set => SetProperty(ref _groupCheckItems, value);
        }

        public string DateCreateSession
        {
            get => _dateCreateSession;
            set => SetProperty(ref _dateCreateSession, value);
        }

        public Visibility ShowLoadIndicator
        {
            get => _showLoadIndicator;
            set => SetProperty(ref _showLoadIndicator, value);
        }

        public DelegateCommand SelectSessionCommand { get; }

        public DelegateCommand GetSensorDataCommand { get; }

        #endregion

        #region Constuctor

        public AnalyticsWindowViewModel(IRegionManager regionManager, IDeviceConnector deviceConnector,
            INotifications notifications, IDialogService dialogService)
            : base(regionManager, deviceConnector, notifications, dialogService)
        {
            SensorGroups = Enumerable.Range(0, 6)
                .Select(x => new ItemDescriptor<int>($"Группа №{x + 1}", x))
                .ToList();
            SensorGroupSelected = 0;

            GroupCheckItems.AddRange(Enumerable.Range(0, 6)
                .Select(x => new ItemDescriptor<bool>($"Датчик №{x + 1}", true))
                .ToList());

            InitChart();
            InitChartValueAsync();

            SelectSessionCommand = new DelegateCommand(SelectSessionExecute);
            GetSensorDataCommand = new DelegateCommand(GetSensorDataExecute);
        }

        #endregion

        #region Method

        private void InitChart()
        {
            PlotControl = new WpfPlot();

            _plot = PlotControl.Plot;
            _plot.Style(
                dataBackground: Color.FromArgb(31, 31, 31), 
                figureBackground: Color.FromArgb(31, 31, 31),
                grid: Color.FromArgb(121, 121, 121), 
                tick: Color.FromArgb(170, 170, 170));
            _plot.XAxis.TickLabelStyle(fontSize: 12);
            _plot.YAxis.TickLabelStyle(fontSize: 12);
        }

        private async void InitChartValueAsync()
        {
            await GetIdGroupsSensorAsync();
            await GetSensorDataAsync();
        }

        private async Task GetIdGroupsSensorAsync()
        {
            var idTask = Task.Run(() =>
            {
                using var context = new Context();
                return context.Sessions
                    .Max(p => p.Id);
            });
            await Task.WhenAll(idTask);
            var sessionId = idTask.Result;

            var dataCreateTask = Task.Run(() =>
            {
                using var context = new Context();
                return context.Sessions
                    .Where(x => x.Id == sessionId)
                    .Select(x => x.DateCreate)
                    .FirstOrDefault();
            });
            await Task.WhenAll(dataCreateTask);
            DateCreateSession = dataCreateTask.Result
                .Round(TimeSpan.FromSeconds(1))
                .ToString(CultureInfo.InvariantCulture);

            var groupIdTask = Task.Run(() =>
            {
                using var context = new Context();
                return context.GroupSensors
                    .Where(x => x.SessionId == sessionId)
                    .Select(x => x.Id)
                    .ToList();
            });
            await Task.WhenAll(groupIdTask);

            _groupSensorId.Clear();
            _groupSensorId.AddRange(groupIdTask.Result);
        }

        private async Task GetSensorDataAsync()
        {
            _plot.Clear();

            var task = Task.Run(() =>
            {
                using var context = new Context();
                return context.SensorInformations
                    .Count(x => x.SensorGroupId == _groupSensorId[_sensorGroupSelected]);
            });
            await Task.WhenAll(task);
            var countItem = task.Result;

            if (countItem < 2)
            {
                Notifications.ShowInformation("Нет данных для анализа.");
                return;
            }
            ShowLoadIndicator = Visibility.Visible;

            var getItemsTask = Task.Run(() =>
            {
                using var context = new Context();
                return context.SensorInformations
                    .Where(x => x.SensorGroupId == _groupSensorId[_sensorGroupSelected])
                    .ToList();
            });
            await Task.WhenAll(getItemsTask, Task.Delay(1000));
            var result = getItemsTask.Result;

            if (GroupCheckItems[0].Value)
                _plot.AddSignal(result.Select(x => (double)x.Value1).ToArray(), color: Color.FromArgb(0x00, 0x3f, 0x5c));
            if (GroupCheckItems[1].Value)
                _plot.AddSignal(result.Select(x => (double)x.Value2).ToArray(), color: Color.FromArgb(0x44, 0x4e, 0x86));
            if (GroupCheckItems[2].Value)
                _plot.AddSignal(result.Select(x => (double)x.Value3).ToArray(), color: Color.FromArgb(0x95, 0x51, 0x96));
            if (GroupCheckItems[3].Value)
                _plot.AddSignal(result.Select(x => (double)x.Value4).ToArray(), color: Color.FromArgb(0xdd, 0x51, 0x82));
            if (GroupCheckItems[4].Value)
                _plot.AddSignal(result.Select(x => (double)x.Value5).ToArray(), color: Color.FromArgb(0xff, 0x6e, 0x54));
            if (GroupCheckItems[5].Value)
                _plot.AddSignal(result.Select(x => (double)x.Value6).ToArray(), color: Color.FromArgb(0xff, 0xa6, 0x00));

            _plot.AxisAutoX();
            _plot.SetAxisLimitsY(0, 165);
            PlotControl.Render();

            ShowLoadIndicator = Visibility.Hidden;
        }

        private void SelectSessionExecute()
        {
            var parameters = new DialogParameters
            {
                { "CheckCurrentSession", _isLoadCurrentSession },
                { "SessionItemSelected", _currentSessionId }
            };

            DialogService.ShowNotification("SessionDialog", async r =>
            {
                if (r.Result == ButtonResult.Cancel)
                    Notifications.ShowInformation("Операция прервана пользователем.");
                else if (r.Result == ButtonResult.OK)
                {
                    var isLoadCurrentSession = r.Parameters.GetValue<bool>("CheckCurrentSession");
                    _currentSessionId = r.Parameters.GetValue<int>("SessionItemSelected");

                    if (isLoadCurrentSession)
                    {
                        if (_isLoadCurrentSession)
                            return;

                        await GetIdGroupsSensorAsync();
                        await GetSensorDataAsync();

                        Notifications.ShowSuccess($"Загружена текущая сессия.");
                        _isLoadCurrentSession = true;
                    }
                    else
                    {
                        var dataCreateTask = Task.Run(() =>
                        {
                            using var context = new Context();
                            return context.Sessions
                                .Where(x => x.Id == _currentSessionId)
                                .Select(x => x.DateCreate)
                                .FirstOrDefault();
                        });
                        await Task.WhenAll(dataCreateTask);
                        DateCreateSession = dataCreateTask.Result
                            .Round(TimeSpan.FromSeconds(1))
                            .ToString(CultureInfo.InvariantCulture);

                        var groupIdTask = Task.Run(() =>
                        {
                            using var context = new Context();
                            return context.GroupSensors
                                .Where(x => x.SessionId == _currentSessionId)
                                .Select(x => x.Id)
                                .ToList();
                        });
                        await Task.WhenAll(groupIdTask);

                        _groupSensorId.Clear();
                        _groupSensorId.AddRange(groupIdTask.Result);

                        await GetSensorDataAsync();
                        Notifications.ShowSuccess($"Загружена сессия от {DateCreateSession}.");
                        _isLoadCurrentSession = false;
                    }
                }
            }, parameters);
        }

        private async void GetSensorDataExecute() => await GetSensorDataAsync();

        #endregion
    }
}
