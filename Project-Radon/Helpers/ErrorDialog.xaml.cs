using Windows.UI.Xaml.Controls;
using Yttrium_browser;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Project_Radon.Helpers
{
    public sealed partial class ErrorDialog : ContentDialog
    {
        public ErrorDialog(string ExceptionHook)
        {
            InitializeComponent();
            ExceptionText.Text = "Exception: " + ExceptionHook;
        }

        private void ContentDialog_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            App.Current.Exit();
        }
    }
}
