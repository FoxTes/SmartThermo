using Prism.Mvvm;
using ScottPlot;
using System.Collections.Generic;
using System.Timers;

namespace SmartThermo.Modules.DataViewer.ViewModels
{
    public class DataViewerWindowViewModel : BindableBase
    {
        private readonly List<double> _data = new List<double>(1000);
        
        private WpfPlot _chartTests;

        public WpfPlot ChartTest
        {
            get { return _chartTests; ; }
            set { SetProperty(ref _chartTests, value); }
        }

        public DataViewerWindowViewModel()
        {
            var timer = new Timer {Interval = 1000, AutoReset = true, Enabled = true};
            timer.Elapsed += OnTimerElapsed; 
            
            ChartTest = new WpfPlot();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _data.Add(5d);
            ChartTest.Plot.AddSignal(_data.ToArray());
        }
    }
}
