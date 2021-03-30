using LiveCharts;
using LiveCharts.Wpf;
using System.ComponentModel;

namespace SmartThermo.Modules.DataViewer.Views.Charts
{
    /// <summary>
    /// Interaction logic for CustomersTooltip.xaml
    /// </summary>
    public partial class CustomersTooltip : IChartTooltip
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private TooltipData _data;

        public CustomersTooltip()
        {
            InitializeComponent();
            DataContext = this;
        }

        public TooltipData Data
        {
            get { return _data; }
            set
            {
                _data = value;
                OnPropertyChanged("Data");
            }
        }

        public TooltipSelectionMode? SelectionMode { get; set; }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
