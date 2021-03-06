﻿using Prism.Commands;
using Prism.Services.Dialogs;
using SmartThermo.Core.Mvvm;
using SmartThermo.DataAccess.Sqlite;
using SmartThermo.Modules.Analytics.Models;
using SmartThermo.Services.Notifications;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SmartThermo.Core.Extensions;

namespace SmartThermo.Modules.Analytics.Dialogs.ViewModels
{
    public class SessionDialogViewModel : DialogViewModelBase
    {
        #region Field

        private readonly INotifications _notifications;
        private ObservableCollection<SessionInfo> _sessionItems = new ObservableCollection<SessionInfo>();
        private bool _checkCurrentSession;
        private int _sessionItemsSelected;

        #endregion

        #region Property

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

        #endregion
        
        #region Constructor

        public SessionDialogViewModel(INotifications notifications)
        {
            _notifications = notifications;

            GetSessionInfo().Await(() => { },
                exception => _notifications.ShowWarning("Не удалось прочитать данные о сессиях"));

            SelectCommand = new DelegateCommand(SelectExecute);
            DeleteSelectCommand = new DelegateCommand(DeleteExecute);
            DeleteAllCommand = new DelegateCommand(DeleteAllExecute);
            CancelCommand = new DelegateCommand(CancelExecute);
        }
        
        #endregion

        #region Method
        
        public override void OnDialogOpened(IDialogParameters parameters) 
            => CheckCurrentSession = parameters.GetValue<bool>("CheckCurrentSession");

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
                { "SessionItemSelected", CheckCurrentSession ? 0 : SessionItems[_sessionItemsSelected].Id }
            };
            RaiseRequestClose(new DialogResult(ButtonResult.OK, parameters));
        }

        private async void DeleteExecute()
        {
            if (CheckSessionItemsForZero()) 
                return;
            
            var sessionDeleteTask = Task.Run(() =>
            {
                using var context = new Context();
                var session = context.Sessions.First(x => x.Id == _sessionItems[_sessionItemsSelected].Id);
                context.Remove(session);
                context.SaveChanges();
            });
            await Task.WhenAll(sessionDeleteTask);
            
            _notifications.ShowSuccess($"Запись от {_sessionItems[_sessionItemsSelected].DateCreate} была удалена.");
            await GetSessionInfo();
        }
        
        private async void DeleteAllExecute()
        {
            if (CheckSessionItemsForZero()) 
                return;

            var sessionDeleteTask = Task.Run(() =>
            {
                using var context = new Context();
                var sessions = context.Sessions
                    .OrderByDescending(x => x.DateCreate)
                    .Skip(1)
                    .ToList();
                context.RemoveRange(sessions);
                context.SaveChanges();
            });
            await Task.WhenAll(sessionDeleteTask);

            _notifications.ShowSuccess("Все записи были успешно удалены.");
            await GetSessionInfo();        
        }
        
        private bool CheckSessionItemsForZero()
        {
            if (SessionItems.Count != 0) 
                return false;

            _notifications.ShowInformation("Отсутствуют записи для удаления.");
            return true;
        }

        private void CancelExecute() => RaiseRequestClose(new DialogResult(ButtonResult.Cancel));

        private async Task GetSessionInfo()
        {
            var sessionInfoTask = Task.Run(() =>
            {
                using var context = new Context();
                return context.Sessions
                    .OrderByDescending(x => x.DateCreate)
                    .Select(x => new SessionInfo
                    {
                        Id = x.Id,
                        DateCreate = x.DateCreate,
                        CountRecord = context.SensorInformations
                            .Count(y => y.SensorGroup.SessionId == x.Id 
                                     && y.SensorGroup.Name == "Первая группа")
                    })
                    .Skip(1)
                    .ToList();
            });
            await Task.WhenAll(sessionInfoTask);

            SessionItems.Clear();
            SessionItems.AddRange(sessionInfoTask.Result);
            SessionItemSelected = 0;
        }
        
        #endregion
    }
}
