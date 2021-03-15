using Prism.Mvvm;

namespace SmartThermo.Modules.Dialog.SettingsDevice.Models
{
    public class GroupInfo : BindableBase
    {
        private bool _enable;

        public string Name { get; set; }

        public bool Enable
        {
            get => _enable;
            set => SetProperty(ref _enable, value);
        }
    }
}
