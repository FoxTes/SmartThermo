using System.Windows;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime.Clear;
using ToastNotifications.Messages;

namespace SmartThermo.Services.Notifications
{
    public class Notifications : INotifications
    {
        private Notifier _notifier;
        
        public Notifications(Notifier notifier)
        {
            _notifier = notifier;
            _notifier.ClearMessages(new ClearAll());
        }

        public void SetNewInstanceNotifier(Notifier notifier)
        {
            _notifier?.ClearMessages(new ClearAll());
            _notifier = notifier;
        }

        public void ShowInformation(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _notifier.ShowInformation(message);
            });
        }

        public void ShowInformation(string message, MessageOptions opts)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _notifier.ShowInformation(message, opts);
            });
        }

        public void ShowSuccess(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _notifier.ShowSuccess(message);
            });
        }

        public void ShowSuccess(string message, MessageOptions opts)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _notifier.ShowSuccess(message, opts);
            });
        }

        public void ShowWarning(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _notifier.ShowWarning(message);
            });
        }
        
        public void ShowWarning(string message, MessageOptions opts)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _notifier.ShowWarning(message, opts);
            });
        }

        public void ShowError(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _notifier.ShowError(message);
            });
        }

        public void ShowError(string message, MessageOptions opts)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _notifier.ShowError(message, opts);
            });
        }
        
        public void ClearMessages(string msg)
        {
            _notifier.ClearMessages(new ClearByMessage(msg));
        }
        
        public void ClearAll()
        {
            _notifier.ClearMessages(new ClearAll());
        }
    }
}
