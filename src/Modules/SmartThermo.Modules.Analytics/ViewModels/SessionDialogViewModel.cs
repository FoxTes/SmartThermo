using Prism.Commands;
using Prism.Services.Dialogs;
using SmartThermo.Core.Mvvm;

namespace SmartThermo.Modules.Analytics.ViewModels
{
    public class SessionDialogViewModel : DialogViewModelBase
    {
        public DelegateCommand CloseDialogCommand { get; }

        public SessionDialogViewModel()
        {
            CloseDialogCommand = new DelegateCommand(CloseDialog);
        }

        private void CloseDialog()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.None));
        }
    }
}
