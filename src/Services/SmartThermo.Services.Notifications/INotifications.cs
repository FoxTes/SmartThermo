using ToastNotifications.Core;

namespace SmartThermo.Services.Notifications
{
    public interface INotifications
    {
        void OnUnloaded();
        
        void ShowInformation(string message);
        
        void ShowInformation(string message, MessageOptions opts);
        
        void ShowSuccess(string message);
        
        void ShowSuccess(string message, MessageOptions opts);
        
        void ClearMessages(string msg);
        
        void ShowWarning(string message, MessageOptions opts);
        
        void ShowError(string message);
        
        void ShowError(string message, MessageOptions opts);
        
        void ClearAll();
    }
}