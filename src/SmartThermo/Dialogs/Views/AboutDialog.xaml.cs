using ModernWpf.Controls;

namespace SmartThermo.Dialogs.Views
{
    /// <summary>
    /// Interaction logic for AboutDialog.xaml
    /// </summary>
    public partial class AboutDialog
    {
        /// <inheritdoc />
        public AboutDialog()
        {
            InitializeComponent();
        }

        private void OnCloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();
            deferral.Complete();
        }
    }
}
