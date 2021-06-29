using Prism.Regions;
using SmartThermo.Core.Mvvm;
using SmartThermo.Services.Configuration;
using SmartThermo.Services.SerialPortObserver;

namespace SmartThermo.Modules.SettingsApplication.ViewModels
{
    /// <summary>
    /// Модель представления настроек приложения.
    /// </summary>
    public class SettingsApplicationWindowViewModel : RegionViewModelBase
    {
        private readonly IConfiguration _configuration;
        private readonly ISerialPortObserver _serialPortObserver;
        private bool _isLoadView = true;

        private int _timeBeforeWarning;
        private int _timeBeforeOffline;
        private bool _isWriteToDatabase;
        private bool _isAutoConnect;

        /// <inheritdoc />
        public SettingsApplicationWindowViewModel(
            IConfiguration configuration,
            ISerialPortObserver serialPortObserver)
        {
            _configuration = configuration;
            _serialPortObserver = serialPortObserver;
        }

        /// <summary>
        /// Время, после которого датчик переходит в режим ожидания.
        /// </summary>
        public int TimeBeforeWarning
        {
            get => _timeBeforeWarning;
            set
            {
                SetProperty(ref _timeBeforeWarning, value);
                SaveSetting();
            }
        }

        /// <summary>
        /// Время, после которого связь с датчиком потеряна.
        /// </summary>
        public int TimeBeforeOffline
        {
            get => _timeBeforeOffline;
            set
            {
                SetProperty(ref _timeBeforeOffline, value);
                SaveSetting();
            }
        }

        /// <summary>
        /// Автоподключение к прибору.
        /// </summary>
        public bool IsAutoConnect
        {
            get => _isAutoConnect;
            set
            {
                SetProperty(ref _isAutoConnect, value);
                ChangeStatusObserver(value);
                SaveSetting();
            }
        }

        /// <summary>
        /// Вести запись полученных данных.
        /// </summary>
        public bool IsWriteToDatabase
        {
            get => _isWriteToDatabase;
            set
            {
                SetProperty(ref _isWriteToDatabase, value);
                SaveSetting();
            }
        }

        /// <inheritdoc />
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            _isLoadView = true;

            TimeBeforeWarning = _configuration.TimeBeforeWarning;
            TimeBeforeOffline = _configuration.TimeBeforeOffline;
            IsAutoConnect = _configuration.IsAutoConnect;
            IsWriteToDatabase = _configuration.IsWriteToDatabase;

            _isLoadView = false;
        }

        private void ChangeStatusObserver(bool status)
        {
            if (status)
                _serialPortObserver.Start();
            else
                _serialPortObserver.Stop();
        }
        
        private void SaveSetting()
        {
            if (_isLoadView)
                return;

            _configuration.TimeBeforeWarning = _timeBeforeWarning;
            _configuration.TimeBeforeOffline = _timeBeforeOffline;
            _configuration.IsAutoConnect = _isAutoConnect;
            _configuration.IsWriteToDatabase = _isWriteToDatabase;
            _configuration.SaveChangedAsync();
        }
    }
}
