using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using SmartThermo.Core.Mvvm;
using SmartThermo.Modules.DataViewer.Models;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.DeviceConnector.Enums;
using SmartThermo.Services.DeviceConnector.Models;

namespace SmartThermo.Modules.DataViewer.ViewModels
{
    public class DataViewerWindowViewModel : ViewModelBase
    {
        private static readonly object Lock = new object();
        private ObservableCollection<SensorsEther> _sensorsEtherItems = new ObservableCollection<SensorsEther>();

        public ObservableCollection<SensorsEther> SensorsEtherItems
        {
            get => _sensorsEtherItems;
            set => SetProperty(ref _sensorsEtherItems, value);
        }

        public DataViewerWindowViewModel(IDeviceConnector deviceConnector)
        {
            deviceConnector.RegistersRequested += DeviceConnector_RegistersRequested;
            deviceConnector.StatusConnectChanged += DeviceConnector_StatusConnectChanged;

            BindingOperations.EnableCollectionSynchronization(SensorsEtherItems, Lock);
        }

        private void DeviceConnector_StatusConnectChanged(object sender, StatusConnect e)
        {
            if (e == StatusConnect.Disconnected)
                SensorsEtherItems.Clear();
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
        }
    }
}
