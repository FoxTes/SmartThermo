using System;
using System.Windows;
using Prism.Services.Dialogs;

namespace SmartThermo.Dialogs.Views
{
    /// <summary>
    /// Interaction logic for NotificationWindowCloseButton.xaml
    /// </summary>
    public partial class NotificationWindowCloseButton : Window, IDialogWindow
    {
        /// <inheritdoc />
        public NotificationWindowCloseButton()
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
