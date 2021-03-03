using Prism.Commands;
using Prism.Regions;
using SmartThermo.Core.Mvvm;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.DeviceConnector.Enums;

namespace SmartThermo.ViewModels
{
    public class MainWindowViewModel : RegionViewModelBase
    {
        #region Field
        
        private StatusConnect _statusConnect = StatusConnect.Disconnected;
        
        private string _labelButton = "Подключить прибор";
        
        #endregion

        #region Property
        
        public string LabelButton
        {
            get => _labelButton;
            set => SetProperty(ref _labelButton, value);
        }
        
        public DelegateCommand ChangeConnectDeviceCommand { get; }
        
        #endregion

        #region Constructor

        public MainWindowViewModel(IRegionManager regionManager, IDeviceConnector deviceConnector) 
            : base(regionManager, deviceConnector)
        {
            DeviceConnector.StatusConnectChanged += (_, connect) =>
            {
                _statusConnect = connect;
                LabelButton = _statusConnect == StatusConnect.Connected ? "Отключить прибор" : "Подключить прибор";
            };
            
            ChangeConnectDeviceCommand = new DelegateCommand(ChangeConnectDeviceExecute);
        }
        
        #endregion

        #region Method

        private void ChangeConnectDeviceExecute()
        {
            if (_statusConnect == StatusConnect.Connected)
                DeviceConnector.Close();
            else
                DeviceConnector.Open();
        }
        
        #endregion
    }
}
