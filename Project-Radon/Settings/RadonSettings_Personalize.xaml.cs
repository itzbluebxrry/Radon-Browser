using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;





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
            this.InitializeComponent();
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
