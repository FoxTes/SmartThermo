using Prism.Commands;
using Prism.Mvvm;
using System;

namespace SmartThermo.Modules.Analytics.ViewModels
{
    public class AnalyticsWindowViewModel : BindableBase
    {
        public DelegateCommand CancelCommand { get; }

        public AnalyticsWindowViewModel()
        {
            CancelCommand = new DelegateCommand(SetSessionExecute);
        }

        private void SetSessionExecute()
        {
            //throw new NotImplementedException();
        }
    }
}
