using ToastNotifications;
using ToastNotifications.Core;

namespace SmartThermo.Services.Notifications
{
    public interface INotifications
    {
        public void SetNewInstanceNotifier(Notifier notifier);

        /// <summary>
        /// Показывает уведомление со статусом информации.
        /// </summary>
        /// <param name="message">Текст уведомления.</param>
        void ShowInformation(string message);
        
        void ShowInformation(string message, MessageOptions opts);
        
        /// <summary>
        /// Показывает уведомление с успешным статусом.
        /// </summary>
        /// <param name="message">Текст уведомления.</param>
        void ShowSuccess(string message);
        
        void ShowSuccess(string message, MessageOptions opts);
        
        /// <summary>
        /// Показывает уведомление со статусом предупреждения.
        /// </summary>
        /// <param name="message">Текст уведомления.</param>
        void ShowWarning(string message);
        
        void ShowWarning(string message, MessageOptions opts);
        
        /// <summary>
        /// Показывает уведомление со статусом ошибки.
        /// </summary>
        /// <param name="message">Текст уведомления.</param>
        void ShowError(string message);
        
        void ShowError(string message, MessageOptions opts);
        
        /// <summary>
        /// Удаляет выбранное уведомление.
        /// </summary>
        /// <param name="msg">Уведомление, подлежащие удалению.</param>
        void ClearMessages(string msg);
        
        /// <summary>
        /// Очищает все открытые уведомления.
        /// </summary>
        void ClearAll();
    }
}