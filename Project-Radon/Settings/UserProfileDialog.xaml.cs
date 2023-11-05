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
            InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            String username = localSettings.Values["username"] as string;
            Username_Display.Text = username;
        }

        private void pfpchanged_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pfppreview.ProfilePicture = new BitmapImage(new Uri(string.Join("", new string[] { "ms-appx:///accountpictures/", (pfpchanged.SelectedItem as ComboBoxItem).Content.ToString(), ".png" })));


        }

        private void updateprofile_Click(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["username"] = username_box.Text;
            Username_Display.Text = username_box.Text;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void debug_Click(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["username"] = null;
            Username_Display.Text = username_box.Text;
        }

        private void KeyboardAccelerator_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            ApplicationData.Current.LocalSettings.Values["username"] = null;
            Username_Display.Text = username_box.Text;
        }
    }
}
