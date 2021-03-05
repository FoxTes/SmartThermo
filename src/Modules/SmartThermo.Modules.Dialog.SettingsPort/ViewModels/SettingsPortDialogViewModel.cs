using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace SmartThermo.Modules.Dialog.SettingsPort.ViewModels
{
    public class SettingsPortDialogViewModel : BindableBase, IDialogAware
    {
        private string _message;

        public event Action<IDialogResult> RequestClose;

        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public string Title => "привет";

        public SettingsPortDialogViewModel()
        {
            Message = "View A from your Prism Module";
        }

       

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            
        }

        public bool CanCloseDialog()
        {
            return true;
        }
    }
}
