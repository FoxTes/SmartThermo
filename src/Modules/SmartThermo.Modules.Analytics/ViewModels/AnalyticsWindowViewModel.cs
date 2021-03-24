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

            GetSensorDataCommand = new DelegateCommand(GetSensorDataExecute);
        }

        private void InitChart()
        {
            PlotControl = new WpfPlot();

            _plot = PlotControl.Plot;
            _plot.Style(dataBackground:Color.FromArgb(31,31,31),
                figureBackground:Color.FromArgb(31, 31, 31), 
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
            _groupSensorId.AddRange(groupIdTask.Result);
        }

        private async Task GetSensorDataAsync()
        {
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

            InitChart();
            if (GroupCheckItems[0].Value)
                _plot.AddSignal(result.Select(x => (double)x.Value1).ToArray());
            if (GroupCheckItems[1].Value)
                _plot.AddSignal(result.Select(x => (double)x.Value2).ToArray());
            if (GroupCheckItems[2].Value)
                _plot.AddSignal(result.Select(x => (double)x.Value3).ToArray());
            if (GroupCheckItems[3].Value)
                _plot.AddSignal(result.Select(x => (double)x.Value4).ToArray());
            if (GroupCheckItems[4].Value)
                _plot.AddSignal(result.Select(x => (double)x.Value5).ToArray());
            if (GroupCheckItems[5].Value)
                _plot.AddSignal(result.Select(x => (double)x.Value6).ToArray());

            //                Stroke = "#003f5c"
            //                Stroke = "#444e86"
            //                Stroke = "#955196"
            //                Stroke = "#dd5182"
            //                Stroke = "#ff6e54"
            //                Stroke = "#ffa600"

            _plot.AxisAutoX();
            _plot.SetAxisLimitsY(0, 165);
            _plot.Render();

            ShowLoadIndicator = Visibility.Hidden;
        }

        private async void GetSensorDataExecute()
        {
            await GetSensorDataAsync();
        }

        #endregion
    }
}
