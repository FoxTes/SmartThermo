using System;
using Prism.Services.Dialogs;

namespace SmartThermo.Core.Extensions
{
    public static class DialogServiceExtension
    {
        public static void ShowNotification(this IDialogService dialogService, string name,
            Action<IDialogResult> callBack, DialogParameters dialogParameters = null, string windowName = null)
        {
            if (windowName == null)
                dialogService.ShowDialog(name, dialogParameters, callBack);
            else
                dialogService.ShowDialog(name, dialogParameters, callBack, windowName);
        }
    }
}
