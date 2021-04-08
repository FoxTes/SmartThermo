using Prism.Services.Dialogs;
using System;
using System.Windows;

namespace SmartThermo.Dialogs.Views
{
    /// <summary>
    /// Interaction logic for NotificationWindowCloseButton.xaml
    /// </summary>
    public partial class NotificationWindowCloseButton : Window, IDialogWindow
    {
        public IDialogResult Result { get; set; }

        public NotificationWindowCloseButton()
        {
            InitializeComponent();
        }

        private void Window_OnContentRendered(object sender, EventArgs e)
        {
            InvalidateMeasure();
        }
    }
}
