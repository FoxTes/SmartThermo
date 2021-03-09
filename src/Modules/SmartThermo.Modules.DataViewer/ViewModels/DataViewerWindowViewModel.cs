using System.Collections.ObjectModel;
using System.Linq;
using Prism.Common;
using Prism.Regions;
using Prism.Services.Dialogs;
using SmartThermo.Core.Mvvm;
using SmartThermo.Modules.DataViewer.Models;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.Notifications;

namespace SmartThermo.Modules.DataViewer.ViewModels
{
    public class DataViewerWindowViewModel : RegionViewModelBase
    {
        private ObservableCollection<SensorsEther> _sensorsEtherItems = new ObservableCollection<SensorsEther>();

        public ObservableCollection<SensorsEther> SensorsEtherItems
        {
            get => _sensorsEtherItems;
            set => SetProperty(ref _sensorsEtherItems, value);
        }

        public DataViewerWindowViewModel(IRegionManager regionManager, IDeviceConnector deviceConnector, 
            INotifications notifications, IDialogService dialogService) 
            : base(regionManager, deviceConnector, notifications, dialogService)
        {
            deviceConnector.RegistersRequested += (o, list) =>
            {
                var data = list.Select(x => new SensorsEther
                {
                    Time = x.TimeLastBroadcast
                }).ToList();

                SensorsEtherItems.Clear();
                SensorsEtherItems.AddRange(data);
            };
        }
    }
}
