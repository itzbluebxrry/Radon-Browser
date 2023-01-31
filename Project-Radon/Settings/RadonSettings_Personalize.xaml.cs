using System;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;





// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Project_Radon.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RadonSettings_Personalize : Page
    {
        public RadonSettings_Personalize()
        {
            InitializeComponent();
        }

        private async void TeachingTip_ActionButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
        {
            // Attempt restart, with arguments.
            AppRestartFailureReason result =
                await CoreApplication.RequestRestartAsync("-fastInit -level 1 -foo");
        }

        private void TeachingTip_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Appthemecombobox_Loaded(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            String value = localSettings.Values["apptheme"] as string;

            if (value == null)
            {
                Apptheme_box.SelectedIndex = 0;
            }
            else if (value == "1")
            {
                Apptheme_box.SelectedIndex = 1;
            }

        }

        private void Appthemecombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            // Save a setting locally on the device
            localSettings.Values["apptheme"] = Apptheme_box.SelectedIndex;


        }
    }
}
