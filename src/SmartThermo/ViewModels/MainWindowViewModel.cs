using Prism.Mvvm;
using SmartThermo.Models;
using System;
using System.Collections.ObjectModel;

namespace SmartThermo.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        #region Field

        private ObservableCollection<TimeEther> _sensorsEtherItems;

        #endregion

        #region Property

        public ObservableCollection<TimeEther> SensorsEtherItems
        {
            get => _sensorsEtherItems;
            set => SetProperty(ref _sensorsEtherItems, value);
        }

        #endregion

        public MainWindowViewModel()
        {
            var random = new Random();

            SensorsEtherItems = new ObservableCollection<TimeEther>();
            for (var i = 0; i < 42; i++)
                SensorsEtherItems.Add(new TimeEther { Id = i + 1, Time = random.Next(0, 60) });
        }

        #region Method


        #endregion
    }
}
