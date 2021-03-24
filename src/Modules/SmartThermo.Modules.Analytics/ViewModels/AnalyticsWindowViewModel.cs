using Microsoft.EntityFrameworkCore;
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
            _plot.Style(ScottPlot.Style.Gray1);
        }

        private async void InitChartValueAsync()
        {
            await GetIdGroupsSensorAsync();
            await GetSensorDataAsync();
        }

        private async Task GetIdGroupsSensorAsync()
        {
            await using var context = new Context();

            var sessionId = await context.Sessions
                .MaxAsync(p => p.Id);

            DateCreateSession = (await context.Sessions
                .Where(x => x.Id == sessionId)
                .Select(x => x.DateCreate)
                .FirstOrDefaultAsync())
                .Round(TimeSpan.FromSeconds(1))
                .ToString();

            var result = await context.GroupSensors
                .Where(x => x.SessionId == sessionId)
                .Select(x => x.Id)
                .ToListAsync();
            _groupSensorId.AddRange(result);
        }

        private async Task GetSensorDataAsync()
        {
            ShowLoadIndicator = Visibility.Visible;

            await using var context = new Context();
            var countRecord = await context.SensorInformations
                .Where(x => x.SensorGroupId == _groupSensorId[_sensorGroupSelected])
                .CountAsync().ConfigureAwait(false);
            if (countRecord < 2)
            {
                Notifications.ShowInformation("Нет данных для анализа.");
                return;
            }

            var taskLoad = context.SensorInformations
                .Where(x => x.SensorGroupId == _groupSensorId[_sensorGroupSelected])
                .ToListAsync();
            await Task.WhenAll(taskLoad, Task.Delay(1000));
            var result = taskLoad.Result;

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
