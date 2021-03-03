using Prism.Regions;
using SmartThermo.Core.Mvvm;
using SmartThermo.Services.DeviceConnector;
using System.Collections.Generic;
using System.Timers;
using SmartThermo.Services.Notifications;

namespace SmartThermo.Modules.DataViewer.ViewModels
{
    public class DataViewerWindowViewModel : RegionViewModelBase
    {
        private readonly List<double> _data = new List<double>(1000);
        
        public DataViewerWindowViewModel(IRegionManager regionManager, IDeviceConnector deviceConnector, INotifications notifications) 
            : base(regionManager, deviceConnector, notifications)
        {
            var timer = new Timer {Interval = 1000, AutoReset = true, Enabled = true};
            timer.Elapsed += OnTimerElapsed; 
            
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _data.Add(5d);
        }
    }
}
