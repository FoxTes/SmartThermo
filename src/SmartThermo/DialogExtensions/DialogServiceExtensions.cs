using Prism.Services.Dialogs;
using System;

namespace SmartThermo.DialogExtensions
{
    public static class DialogServiceExtensions
    {
        public static void ShowNotification(this IDialogService dialogService, string message, Action<IDialogResult> callBack)
        {
            dialogService.ShowDialog("SettingsPortDialog", new DialogParameters($"message={message}"), callBack);
        }
    }
}
