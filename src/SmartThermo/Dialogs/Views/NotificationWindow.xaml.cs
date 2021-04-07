using Prism.Services.Dialogs;
using System;
using System.Windows;

namespace SmartThermo.Dialogs.Views
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window, IDialogWindow
    {
        public IDialogResult Result { get; set; }

        public NotificationWindow()
        {
            InitializeComponent();
        }

        private void Window_OnContentRendered(object sender, EventArgs e)
        {
            InvalidateMeasure();
        }
    }
}
