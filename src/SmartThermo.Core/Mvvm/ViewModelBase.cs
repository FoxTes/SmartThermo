using Prism.Mvvm;
using Prism.Navigation;

namespace SmartThermo.Core.Mvvm
{
    public abstract class ViewModelBase : BindableBase, IDestructible
    {
        public virtual void Destroy()
        {

        }
    }
}
