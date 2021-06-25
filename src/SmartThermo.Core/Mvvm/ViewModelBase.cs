using Prism.Mvvm;
using Prism.Navigation;

namespace SmartThermo.Core.Mvvm
{
    /// <summary>
    /// Базовая ViewModel.
    /// </summary>
    public abstract class ViewModelBase : BindableBase, IDestructible
    {
        /// <inheritdoc />
        public virtual void Destroy()
        {
        }
    }
}
