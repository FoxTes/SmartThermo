using System;
using Prism.Services.Dialogs;

namespace SmartThermo.Core.Mvvm
{
    /// <summary>
    /// Базовая ViewModel для диалоговых окон.
    /// </summary>
    public class DialogViewModelBase : RegionViewModelBase, IDialogAware
    {
        private string _iconSource;
        private string _title = string.Empty;

        /// <inheritdoc/>
        public event Action<IDialogResult> RequestClose;

        /// <summary>
        /// Иконка диалогового окна.
        /// </summary>
        public string IconSource
        {
            get => _iconSource;
            set => SetProperty(ref _iconSource, value);
        }

        /// <inheritdoc/>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <inheritdoc/>
        public virtual bool CanCloseDialog() => true;

        /// <inheritdoc/>
        public virtual void OnDialogClosed()
        {
        }

        /// <inheritdoc/>
        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
        }

        /// <summary>
        /// Метод вызывающий закрытие диалогового окна.
        /// </summary>
        /// <param name="dialogResult">Диалоговое окно.</param>
        protected void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }
    }
}
