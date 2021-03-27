using System;
using Prism.Services.Dialogs;

namespace SmartThermo.Core.Extensions
{
    public static class DialogServiceExtension
    {
        public static void ShowNotification(this IDialogService dialogService, string name,
            Action<IDialogResult> callBack, DialogParameters dialogParameters = null)
        {
            dialogService.ShowDialog(name, dialogParameters, callBack);
        }
    }
}
