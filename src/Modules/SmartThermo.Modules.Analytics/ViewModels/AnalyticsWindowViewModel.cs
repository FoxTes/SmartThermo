using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
using SmartThermo.Core.Mvvm;
using SmartThermo.DataAccess.Sqlite;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.Notifications;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScottPlot;
using SmartThermo.Core.Models;

namespace SmartThermo.Modules.Analytics.ViewModels
{
    public class AnalyticsWindowViewModel : RegionViewModelBase
    {
        #region Field

        private readonly List<int> _groupSensorId = new List<int>();
        private WpfPlot _plotControl;
        private Plot _plot;

        private int _sensorGroupSelected;

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

        public DelegateCommand GetSensorDataCommand { get; }

        #endregion
        
        #region Constuctor

        public AnalyticsWindowViewModel(IRegionManager regionManager, IDeviceConnector deviceConnector,
            INotifications notifications, IDialogService dialogService)
            : base(regionManager, deviceConnector, notifications, dialogService)
        {
            InitChart();
            InitChartValueAsync();

            SensorGroups = new List<ItemDescriptor<int>>()
            {
                new ItemDescriptor<int>("Группа №1", 0),
                new ItemDescriptor<int>("Группа №2", 1),
                new ItemDescriptor<int>("Группа №3", 2),
                new ItemDescriptor<int>("Группа №4", 3),
                new ItemDescriptor<int>("Группа №5", 4),
                new ItemDescriptor<int>("Группа №6", 5),
            };
            SensorGroupSelected = 0;

            GetSensorDataCommand = new DelegateCommand(GetSensorDataExecute);
        }
        
        private void InitChart()
        {
            PlotControl = new WpfPlot();
            
            _plot = PlotControl.Plot;
            _plot.Style(Style.Gray1);
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
            var result = await context.GroupSensors
                .Where(x => x.SessionId == sessionId)
                .Select(x => x.Id)
                .ToListAsync();
            
            _groupSensorId.AddRange(result);
        }

        private async Task GetSensorDataAsync()
        {
            await using var context = new Context();
            var result =  await context.SensorInformations
                .Where(x => x.SensorGroupId == _groupSensorId[_sensorGroupSelected])
                .ToListAsync();

            if (result.Count < 2)
            {
                Notifications.ShowInformation("Нет данных для анализа.");
                return;
            }

            InitChart();
            _plot.AddSignal(result.Select(x => (double)x.Value1).ToArray());
            _plot.AddSignal(result.Select(x => (double)x.Value2).ToArray());
            _plot.AddSignal(result.Select(x => (double)x.Value3).ToArray());
            _plot.AddSignal(result.Select(x => (double)x.Value4).ToArray());
            _plot.AddSignal(result.Select(x => (double)x.Value5).ToArray());
            _plot.AddSignal(result.Select(x => (double)x.Value6).ToArray());
            _plot.AxisAutoX();
            _plot.SetAxisLimitsY(0,165);
            _plot.Render();
        }

        private async void GetSensorDataExecute()
        {
            await GetSensorDataAsync();
        }

        #endregion
    }
}
