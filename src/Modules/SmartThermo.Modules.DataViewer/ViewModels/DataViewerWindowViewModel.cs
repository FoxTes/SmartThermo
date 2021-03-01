using Prism.Mvvm;
using ScottPlot;
using System;
using System.Collections.Generic;

namespace SmartThermo.Modules.DataViewer.ViewModels
{
    public class DataViewerWindowViewModel : BindableBase
    {
        private List<WpfPlot> _chartItems;

        public List<WpfPlot> ChartItems
        {
            get { return _chartItems; }
            set { SetProperty(ref _chartItems, value); }
        }

        private WpfPlot _chartTests;

        public WpfPlot ChartTest
        {
            get { return _chartTests; ; }
            set { SetProperty(ref _chartTests, value); }
        }

        public DataViewerWindowViewModel()
        {
            ChartTest = new WpfPlot();

            int pointCount = 51;
            double[] x = DataGen.Consecutive(pointCount);
            double[] sin = DataGen.Sin(pointCount);
            double[] cos = DataGen.Cos(pointCount);

            ChartTest.plt.PlotScatter(x, sin);
            ChartTest.plt.PlotScatter(x, cos);

            ChartTest.plt.PlotHLine(y: .85, draggable: false, dragLimitLower: -1, dragLimitUpper: +1);
            ChartTest.plt.PlotVLine(x: 23, draggable: false, dragLimitLower: 0, dragLimitUpper: 50);

            ChartTest.plt.Grid(lineStyle: LineStyle.Dot);

        }
    }
}
