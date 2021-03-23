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
using SmartThermo.DataAccess.Sqlite.Models;

namespace SmartThermo.Modules.Analytics.ViewModels
{
    public class AnalyticsWindowViewModel : RegionViewModelBase
    {
        #region Field

        private readonly List<int> _groupSensorId = new List<int>();
        private Plot _plot;

        #endregion
        
        #region Property

        public WpfPlot PlotControl { get; set; } 
        
        public DelegateCommand GetSensorDataCommand { get; }

        #endregion
        
        #region Constuctor

        public AnalyticsWindowViewModel(IRegionManager regionManager, IDeviceConnector deviceConnector,
            INotifications notifications, IDialogService dialogService)
            : base(regionManager, deviceConnector, notifications, dialogService)
        {
            InitChart();
            InitChartValueAsync();

            GetSensorDataCommand = new DelegateCommand(GetSensorDataExecute);
        }
        
        private void InitChart()
        {
            PlotControl = new WpfPlot();
            
            _plot = PlotControl.Plot;
            _plot.Style(Style.Gray1);
            _plot.SetAxisLimits(yMin: 0, yMax: 165);
        }

        private async void InitChartValueAsync()
        {
            await GetIdGroupsSensorAsync();
            
            var result = await GetSensorDataAsync();
            if (result.Count == 0) 
                    Notifications.ShowInformation("Нет данных для анализа.");
            
            _plot.AddSignal(result.Select(x => (double)x.Value1).ToArray());
            _plot.AddSignal(result.Select(x => (double)x.Value2).ToArray());
            _plot.AddSignal(result.Select(x => (double)x.Value3).ToArray());
            _plot.AddSignal(result.Select(x => (double)x.Value4).ToArray());
            _plot.AddSignal(result.Select(x => (double)x.Value5).ToArray());
            _plot.AddSignal(result.Select(x => (double)x.Value6).ToArray());
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

        private async Task<List<SensorInformation>> GetSensorDataAsync()
        {
            await using var context = new Context();
            return await context.SensorInformations
                .Where(x => x.SensorGroupId == _groupSensorId[0])
                .ToListAsync();
        }

        private async void GetSensorDataExecute()
        {
            var result = await GetSensorDataAsync();
            if (result.Count == 0) 
                Notifications.ShowInformation("Нет данных для анализа.");
            
            _plot.AddSignal(result.Select(x => (double)x.Value1).ToArray());
            _plot.AddSignal(result.Select(x => (double)x.Value2).ToArray());
            _plot.AddSignal(result.Select(x => (double)x.Value3).ToArray());
            _plot.AddSignal(result.Select(x => (double)x.Value4).ToArray());
            _plot.AddSignal(result.Select(x => (double)x.Value5).ToArray());
            _plot.AddSignal(result.Select(x => (double)x.Value6).ToArray());
        }

        #endregion
    }
}
