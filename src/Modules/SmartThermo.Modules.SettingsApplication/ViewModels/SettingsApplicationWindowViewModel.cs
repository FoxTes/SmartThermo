using Prism.Regions;
using SmartThermo.Core.Mvvm;
using SmartThermo.Services.Configuration;

namespace SmartThermo.Modules.SettingsApplication.ViewModels
{
    /// <summary>
    /// Модель представления настроек приложения.
    /// </summary>
    public class SettingsApplicationWindowViewModel : RegionViewModelBase
    {
        private readonly IConfiguration _configuration;

        private int _timeBeforeWarning;
        private int _timeBeforeOffline;
        private bool _isWriteToDatabase;

        /// <inheritdoc />
        public SettingsApplicationWindowViewModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Время, после которого датчик переходит в режим ожидания.
        /// </summary>
        public int TimeBeforeWarning
        {
            get => _timeBeforeWarning;
            set => SetProperty(ref _timeBeforeWarning, value);
        }

        /// <summary>
        /// Время, после которого связь с датчиком потеряна.
        /// </summary>
        public int TimeBeforeOffline
        {
            get => _timeBeforeOffline;
            set => SetProperty(ref _timeBeforeOffline, value);
        }

        /// <summary>
        /// Вести запись полученных данных.
        /// </summary>
        public bool IsWriteToDatabase
        {
            get => _isWriteToDatabase;
            set => SetProperty(ref _isWriteToDatabase, value);
        }

        /// <inheritdoc />
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            TimeBeforeWarning = _configuration.TimeBeforeWarning;
            TimeBeforeOffline = _configuration.TimeBeforeOffline;
            IsWriteToDatabase = _configuration.IsWriteToDatabase;
        }

        /// <inheritdoc />
        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _configuration.TimeBeforeWarning = _timeBeforeWarning;
            _configuration.TimeBeforeOffline = _timeBeforeOffline;
            _configuration.IsWriteToDatabase = _isWriteToDatabase;
            _configuration.SaveChangedAsync();
        }
    }
}
