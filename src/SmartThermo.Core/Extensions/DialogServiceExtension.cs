using System;
using Prism.Services.Dialogs;

namespace SmartThermo.Core.Extensions
{
    /// <summary>
    /// Предоставляет методы для работы с сервисом диалоговых окон.
    /// </summary>
    public static class DialogServiceExtension
    {
        /// <summary>
        /// Показывает диалоговое окно.
        /// </summary>
        /// <param name="dialogService">Сервис диалоговых окон.</param>
        /// <param name="name">Имя открываемого окна.</param>
        /// <param name="callBack">Действие, при закрытии окна.</param>
        /// <param name="dialogParameters">Параметры диалогового окна.</param>
        /// <param name="windowName">Название внешнего окна.</param>
        public static void ShowNotification(
            this IDialogService dialogService,
            string name,
            Action<IDialogResult> callBack,
            DialogParameters dialogParameters = null,
            string windowName = null)
        {
            if (windowName == null)
                dialogService.ShowDialog(name, dialogParameters, callBack);
            else
                dialogService.ShowDialog(name, dialogParameters, callBack, windowName);
        }
    }
}
