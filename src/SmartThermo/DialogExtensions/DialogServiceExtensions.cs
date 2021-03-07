using Prism.Services.Dialogs;
using System;

namespace SmartThermo.DialogExtensions
{
    public static class DialogServiceExtensions
    {
        public static void ShowNotification(this IDialogService dialogService, string name, Action<IDialogResult> callBack)
        {
            dialogService.ShowDialog(name, new DialogParameters(), callBack);
        }
    }
}
