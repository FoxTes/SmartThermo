using System;
using System.Windows;
using Prism.Services.Dialogs;

namespace SmartThermo.Dialogs.Views
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window, IDialogWindow
    {
        /// <inheritdoc />
        public NotificationWindow()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        public IDialogResult Result { get; set; }

        private void Window_OnContentRendered(object sender, EventArgs e)
        {
            InvalidateMeasure();
        }
    }
}
