using System;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace SmartThermo.Modules.Dialog.SettingsDevice.Extensions
{
    public class ObservableCollectionExtension<T> : ObservableCollection<T> where T : BindableBase
    {
        public ObservableCollectionExtension()
        {
            CollectionChanged += ObservableCollectionEx_CollectionChanged;
        }

        private void ObservableCollectionEx_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                {
                    foreach (T item in e.OldItems) 
                        item.PropertyChanged -= EntityViewModelPropertyChanged;
                    break;
                }
                case NotifyCollectionChangedAction.Add:
                {
                    foreach (T item in e.NewItems) 
                        item.PropertyChanged += EntityViewModelPropertyChanged;
                    break;
                }
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void EntityViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(args);
        }
    }
}
