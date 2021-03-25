using Prism.Commands;
using Prism.Services.Dialogs;
using SmartThermo.Core.Mvvm;
using SmartThermo.DataAccess.Sqlite;
using SmartThermo.Modules.Analytics.Models;
using SmartThermo.Services.Notifications;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SmartThermo.Modules.Analytics.Dialogs.ViewModels
{
    public class SessionDialogViewModel : DialogViewModelBase
    {
        private readonly INotifications _notifications;
        private ObservableCollection<SessionInfo> _sessionItems = new ObservableCollection<SessionInfo>();
        private bool _checkCurrentSession = true;
        private int _sessionItemsSelected;

        public bool CheckCurrentSession
        {
            get => _checkCurrentSession;
            set => SetProperty(ref _checkCurrentSession, value);
        }

        public ObservableCollection<SessionInfo> SessionItems
        {
            get => _sessionItems;
            set => SetProperty(ref _sessionItems, value);
        }

        public int SessionItemSelected
        {
            get => _sessionItemsSelected;
            set => SetProperty(ref _sessionItemsSelected, value);
        }

        public DelegateCommand SelectCommand { get; }

        public DelegateCommand DeleteSelectCommand { get; }

        public DelegateCommand DeleteAllCommand { get; }

        public DelegateCommand CancelCommand { get; }

        public SessionDialogViewModel(INotifications notifications)
        {
            _notifications = notifications;

            GetSessionInfo();

            SelectCommand = new DelegateCommand(SelectExecute);
            DeleteSelectCommand = new DelegateCommand(DeleteExecute);
            DeleteAllCommand = new DelegateCommand(DeleteAllExecute);
            CancelCommand = new DelegateCommand(CancelExecute);
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            CheckCurrentSession = parameters.GetValue<bool>("CheckCurrentSession");
        }

        private void SelectExecute()
        {
            if (SessionItems.Count == 0 && !CheckCurrentSession)
            {
                _notifications.ShowInformation("Не выбрана сессия для чтения.");
                return;
            }

            var parameters = new DialogParameters
            {
                { "CheckCurrentSession", CheckCurrentSession },
                { "SessionItemSelected", SessionItems[_sessionItemsSelected].Id }
            };
            RaiseRequestClose(new DialogResult(ButtonResult.OK, parameters));
        }

        private void DeleteExecute()
        {
            // TODO: Проверить сначала количество.
            _notifications.ShowInformation("Запись от xx^xx^xx была удалена.");
        }

        private void DeleteAllExecute()
        {
            // TODO: Проверить сначала количество.
            _notifications.ShowInformation("Все записи были успешно удалены.");
        }

        private void CancelExecute()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
        }

        private async void GetSessionInfo()
        {
            var sesionInfoTask = Task.Run(() =>
            {
                using var context = new Context();
                return context.Sessions
                    .OrderByDescending(x => x.DateCreate)
                    .Select(x => new SessionInfo
                    {
                        Id = x.Id,
                        DateCreate = x.DateCreate
                    })
                    .Skip(1)
                    .ToList();
            });
            await Task.WhenAll(sesionInfoTask);

            SessionItems.Clear();
            SessionItems.AddRange(sesionInfoTask.Result);
            SessionItemSelected = 0;
        }
    }
}
