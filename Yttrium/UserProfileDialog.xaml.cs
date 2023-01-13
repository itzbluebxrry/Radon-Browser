using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Yttrium
{
    public sealed partial class UserProfileDialog : ContentDialog
    {
        public UserProfileDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {

        }

        private void pfpchanged_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pfppreview.ProfilePicture = new BitmapImage(new Uri(string.Join("ms-appx:///accountpictures/", pfpchanged.SelectedValue, ".png")));


        }

        private void updateprofile_Click(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["username"] = username_box.Text;
            Username_Display.Text = username_box.Text;
        }
    }
}
