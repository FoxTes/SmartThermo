using System;
using Prism.Services.Dialogs;

namespace SmartThermo.Core.Extensions
{
    public static class DialogServiceExtension
    {
        public static void ShowNotification(this IDialogService dialogService, string name, Action<IDialogResult> callBack)
        {
            dialogService.ShowDialog(name, new DialogParameters(), callBack);
        }
    }
}
