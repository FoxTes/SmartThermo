using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Prism.Mvvm;

namespace SmartThermo.Core.Extensions
{
    /// <summary>
    /// Расширение для ObservableCollection, позволяющие отслеживать изменения внутри коллекции.
    /// </summary>
    /// <typeparam name="T">Коллекция.</typeparam>
    public sealed class ObservableCollectionExtension<T> : ObservableCollection<T>
        where T : BindableBase
    {
        /// <inheritdoc />
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
