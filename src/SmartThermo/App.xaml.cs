using Prism.Ioc;
using Prism.Modularity;
using SmartThermo.Modules.DataViewer;
using SmartThermo.Views;
using System.Windows;

namespace SmartThermo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.RegisterSingleton<ISerialPortService, SerialPortService>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<DataViewerModule>();
        }
    }
}