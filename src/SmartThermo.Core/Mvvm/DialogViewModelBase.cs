using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace SmartThermo.Core.Mvvm
{
    public class DialogViewModelBase : BindableBase, IDialogAware
    {
        public event Action<IDialogResult> RequestClose;

        private string _iconSource;
        private string _title = string.Empty;

        public string IconSource
        {
            get => _iconSource;
            set => SetProperty(ref _iconSource, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        protected void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public virtual bool CanCloseDialog() => true;

        public virtual void OnDialogClosed()
        {

        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {

        }
    }
}
