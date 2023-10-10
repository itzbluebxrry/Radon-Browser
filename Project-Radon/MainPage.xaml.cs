using Microsoft.UI.Xaml.Controls;
using Project_Radon.Controls;
using Project_Radon.Helpers;
using Project_Radon.Settings;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Yttrium;
using Windows.ApplicationModel.Core;
using SymbolIconSource = Microsoft.UI.Xaml.Controls.SymbolIconSource;
using Windows.UI.Core;
using Windows.UI;
using Microsoft.Web.WebView2.Core;
using Windows.UI.Xaml.Media.Imaging;
using System.Numerics;
using Windows.UI.Core.Preview;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.WindowManagement;
using System.ServiceModel.Channels;
using Windows.UI.Popups;

namespace Yttrium_browser
{
    public sealed partial class MainPage : Page
    {
        string OriginalUserAgent;
        string GoogleSignInUserAgent;
        public static string SearchValue;
        private readonly ObservableCollection<BrowserTabViewItem> CurrentTabs = new ObservableCollection<BrowserTabViewItem>();
        public MainPage()
        {
            InitializeComponent();
            CurrentTabs.Add(new BrowserTabViewItem());
            CurrentTabs[0].Tab.PropertyChanged += SelectedTabPropertyChanged;

            Window.Current.CoreWindow.Activated += CoreWindow_Activated;

            // TitleBar customizations



            profileCheck();

            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            //load theme settings
            String colorthemevalue = localSettings.Values["appcolortheme"] as string;
            appthemebackground.Source = new BitmapImage(new Uri(string.Join("", new string[] { "ms-appx:///wallpapers/"+ colorthemevalue + ".png" })));
            fullscreentopbarbackground.ImageSource = new BitmapImage(new Uri(string.Join("", new string[] { "ms-appx:///wallpapers/" + colorthemevalue + ".png" })));

            //load Inline Mode settings
            String inlineMode = localSettings.Values["inlineMode"] as string;
            if (inlineMode == "True")
            {
                compactuibar.Visibility = Visibility.Visible;
                DefaultBarUI.Height = new Windows.UI.Xaml.GridLength(0);

                BrowserTabs.TabWidthMode = TabViewWidthMode.Compact;
                compacttitlebar_rightpadding.Visibility = Visibility.Visible;
            }
            else
            {
                compactuibar.Visibility = Visibility.Collapsed;
                DefaultBarUI.Height = new Windows.UI.Xaml.GridLength(40);

                BrowserTabs.TabWidthMode = TabViewWidthMode.Equal;

                compacttitlebar_rightpadding.Visibility = Visibility.Collapsed;
            }

            //load titlebar settings
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            String systemTitleBar = localSettings.Values["systemTitleBar"] as string;

            if (systemTitleBar == "True")
            {
                coreTitleBar.ExtendViewIntoTitleBar = false;

                var titleBar = ApplicationView.GetForCurrentView().TitleBar;

                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.BackgroundColor = Colors.Transparent;

                localSettings.Values["systemTitleBar"] = "True";
            }

            else
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;

                coreTitleBar.ExtendViewIntoTitleBar = true;

                localSettings.Values["systemTitleBar"] = "False";
            }

            // check user registration

        }

        private void profileCheck()
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            // profile check mechanisms
            string username = localSettings.Values["username"] as string;
            if (username == null)
            {
                //this.Frame.Navigate(typeof(oobe1), null);
            }
        }
        private async void OnCloseRequest(object sender, SystemNavigationCloseRequestedPreviewEventArgs e)
        {
            await new ConfirmExitDialog().ShowAsync();
        }

        private void CoreWindow_Activated(CoreWindow sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == CoreWindowActivationState.Deactivated)
            {
                appthemebackground.Opacity = 0.2;
            }

            else
            {
                appthemebackground.Opacity = 0.4;
            }
        }
        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {
            backbutton_icon.Translation = new Vector3(-12, 0, 0);
            await Task.Delay(150);
            backbutton_icon.Translation = new Vector3(0, 0, 0);
            
            CurrentTabs[BrowserTabs.SelectedIndex].Tab.BackButtonSender();
            ElementSoundPlayer.Play(ElementSoundKind.MovePrevious);

        }

        private async void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            forwardbutton_icon.Translation = new Vector3(12, 0, 0);
            await Task.Delay(150);
            forwardbutton_icon.Translation = new Vector3(0, 0, 0);
            CurrentTabs[BrowserTabs.SelectedIndex].Tab.FowardButtonSender();
            ElementSoundPlayer.Play(ElementSoundKind.MoveNext);

        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentTabs[BrowserTabs.SelectedIndex].Tab.Reload();
            ElementSoundPlayer.Play(ElementSoundKind.GoBack);

        }

        //TODO: On window size changed, if navbuttonbar.Width is less than 140, hide the standard nav buttons and show the overflow buttons.

        //navigation completed
        private async void WebBrowser_NavigationCompleted(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {

            if (CurrentTabs[BrowserTabs.SelectedIndex].Tab.SourceUri.Contains("https"))
            {
                //change icon to lock
                SSLIcon.FontFamily = new FontFamily("Segoe Fluent Icons");
                SSLIcon.Glyph = "\xe705";

                //change icon to lock
                SSLFlyoutIcon.FontFamily = new FontFamily("Segoe Fluent Icons");
                SSLFlyoutIcon.Glyph = "\xe705";
                SSLFlyoutHeader.Text = "Your connection is secured.";
                SSLFlyoutStatus.Text = "This site has a valid SSL certificate.";
                SSLFlyoutStatus2.Text = "Your data will be securely sent to the site and will not be intercepted or seen by others.";
            }
            else
            {
                //change icon to warning
                SSLIcon.FontFamily = new FontFamily("Segoe Fluent Icons");
                SSLIcon.Glyph = "\xe783";

                //change icon to lock
                SSLFlyoutIcon.FontFamily = new FontFamily("Segoe Fluent Icons");
                SSLFlyoutIcon.Glyph = "\xe783";
                SSLFlyoutHeader.Text = "This site seems dangerous";
                SSLFlyoutStatus.Text = "This site does not have a valid SSL certificate.";
                SSLFlyoutStatus2.Text = "You data may be at risk of being stolen or intercepted. Be careful on this website.";
            }

            if (SearchBar.FocusState == FocusState.Unfocused)
            {
                
            }

            if (SearchBar.Text.Equals("radon://radon-ntp/"))
            {
                SearchBar.Text = string.Empty;
            }


            //website load status
            try
            {
                Uri icoURI = new Uri("http://www.google.com/s2/favicons?domain=" + SearchValue);
                faviconicon.UriSource = icoURI;
                faviconicon.ShowAsMonochrome = false;

                RefreshButton.Visibility = Visibility.Visible;
                StopRefreshButton.Visibility = Visibility.Collapsed;
                if (!loadingbar.ShowError == true)
                {
                    loadingbar.IsIndeterminate = false;
                    compactloadingbar.IsIndeterminate = false;
                }
            }
            catch (Exception ExLoader)
            {
                ErrorDialog dialog = new ErrorDialog(ExLoader.ToString());
                await dialog.ShowAsync();
            }


            
        }

        private async void SearchBar_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter && !string.IsNullOrEmpty(SearchBar.Text))
            {
                if (SearchBar.Text.Contains("://settings"))
                {
                    settings_click_helper();
                }
                else
                {
                    await CurrentTabs[BrowserTabs.SelectedIndex].Tab.SearchOrGoto(SearchBar.Text);
                }
            }
              
            if (e.Key == VirtualKey.Escape)
            {
                //TODO: Pressing ESC will set the SearchBar.Text to WebView2 source (ESC will cancel URL changes)
                SearchBar.Text = CurrentTabs[BrowserTabs.SelectedIndex].Tab.SourceUri;

                //TODO: WebView2 will steal the focus for keyboard and pointer
                BrowserTabs.Focus(FocusState.Keyboard);
            }

            // ======This line is used to quickly renavigate to MainPage========

            // this.Frame.Navigate(typeof(MainPage), null, new EntranceNavigationTransitionInfo());

        }


        private void SearchBar_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchBar.SelectAll();
            RefreshButton.Visibility = Visibility.Collapsed;
        }

        private void SearchBar_LostFocus(object sender, RoutedEventArgs e)
        {
            RefreshButton.Visibility = Visibility.Visible;
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBar.Text = string.Empty;
        }


        //handles progressring and refresh behavior
        private void WebBrowser_NavigationStarting(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {
            RefreshButton.Visibility = Visibility.Collapsed;
            StopRefreshButton.Visibility = Visibility.Visible;
            RefreshButton.IsEnabled = true;
            loadingbar.IsIndeterminate = true;
            BrowserTabs.Focus(FocusState.Keyboard);
        }

        //stops refreshing if clicked on progressbar
        private async void StopRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentTabs[BrowserTabs.SelectedIndex].Tab.Stop();
            loadingbar.ShowError = true;
            await Task.Delay(2000);
            loadingbar.ShowError = false;
            loadingbar.IsIndeterminate = false;
        }

        //titlebar
        private void DragArea_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SetTitleBar(sender as Border);
        }

        #region Tabs
        private void SelectedTabPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (BrowserTabs.SelectedIndex >= 0)
            {
                SearchBar.Text = CurrentTabs[BrowserTabs.SelectedIndex].Tab.SourceUri;
                if (SearchBar.Text.Contains("://settings"))
                    SearchBar.IsEnabled = false;
                loadingbar.Visibility = CurrentTabs[BrowserTabs.SelectedIndex].Tab.IsLoading ? Visibility.Visible : Visibility.Collapsed;
                StopRefreshButton.Visibility = CurrentTabs[BrowserTabs.SelectedIndex].Tab.IsLoading ? Visibility.Visible : Visibility.Collapsed;
                RefreshButton.Visibility = !CurrentTabs[BrowserTabs.SelectedIndex].Tab.IsLoading ? Visibility.Visible : Visibility.Collapsed;
                BackButton.IsEnabled = CurrentTabs[BrowserTabs.SelectedIndex].Tab.CanGoBack ? IsEnabled : false;
                ForwardButton.IsEnabled = CurrentTabs[BrowserTabs.SelectedIndex].Tab.CanGoFoward ? IsEnabled : false;
            }
        }
        private async void NewTabRequested(object s, string e)
        {
            var b = new BrowserTabViewItem();
            CurrentTabs.Add(b);
            await Task.Delay(50);
            BrowserTabs.SelectedIndex = CurrentTabs.Count - 1;
            _ = b.Tab.GoTo(e);


        }
        private void BrowserTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var r = e.RemovedItems.Select(x => (BrowserTabViewItem)x).ToList();
            r.ForEach(x =>
            {
                x.Tab.PropertyChanged -= SelectedTabPropertyChanged;
                x.Tab.NewTabRequested -= NewTabRequested;
            });
            var s = e.AddedItems.Select(x => (BrowserTabViewItem)x).ToList();
            s.ForEach(x =>
            {
                x.Tab.PropertyChanged += SelectedTabPropertyChanged;
                x.Tab.NewTabRequested += NewTabRequested;
            });
            SelectedTabPropertyChanged(null, null);
        }
        private async void BrowserTabs_AddTabButtonClick(TabView sender, object args)
        {
            CurrentTabs.Add(new BrowserTabViewItem());
            await Task.Delay(25);
            BrowserTabs.SelectedIndex = CurrentTabs.Count - 1;
        }

        private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            if (sender.TabItems.Count <= 1)
                BrowserTabs_AddTabButtonClick(sender, args);

            sender.TabItems.Remove(args.Tab);
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MenuButton.Flyout.Hide();
        }
        private void TabView_Loaded(object sender, RoutedEventArgs e)
        {
            //MenuButton.Flyout.Hide();
        }
        private void BrowserTabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            if (CurrentTabs.Count == 1)
                Application.Current.Exit();

            (args.Item as BrowserTabViewItem).Tab.PropertyChanged -= SelectedTabPropertyChanged;
            (args.Item as BrowserTabViewItem).Tab.Close();
            CurrentTabs.Remove(args.Item as BrowserTabViewItem);
        }
        private void NewTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            BrowserTabs_AddTabButtonClick(null, null);
            args.Handled = true;
        }

        private void CloseSelectedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            if (CurrentTabs.Count == 1)
                Application.Current.Exit();

            CurrentTabs[BrowserTabs.SelectedIndex].Tab.PropertyChanged -= SelectedTabPropertyChanged;
            CurrentTabs[BrowserTabs.SelectedIndex].Tab.Close();
            CurrentTabs.RemoveAt(BrowserTabs.SelectedIndex);
            args.Handled = true;
        }

        private void NavigateToNumberedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {

            int tabToSelect = 0;

            switch (sender.Key)
            {
                case VirtualKey.Number1:
                    tabToSelect = 0;
                    break;
                case VirtualKey.Number2:
                    tabToSelect = 1;
                    break;
                case VirtualKey.Number3:
                    tabToSelect = 2;
                    break;
                case VirtualKey.Number4:
                    tabToSelect = 3;
                    break;
                case VirtualKey.Number5:
                    tabToSelect = 4;
                    break;
                case VirtualKey.Number6:
                    tabToSelect = 5;
                    break;
                case VirtualKey.Number7:
                    tabToSelect = 6;
                    break;
                case VirtualKey.Number8:
                    tabToSelect = 7;
                    break;
                case VirtualKey.Number9:
                    // Select the last tab
                    tabToSelect = CurrentTabs.Count - 1;
                    break;
                case VirtualKey.LeftButton:
                    tabToSelect = CurrentTabs.Count == 1 ? 1 : BrowserTabs.SelectedIndex - 1;
                    break;
                case VirtualKey.RightButton:
                    tabToSelect = BrowserTabs.SelectedIndex + 1 == CurrentTabs.Count ? BrowserTabs.SelectedIndex : BrowserTabs.SelectedIndex + 1;
                    break;
            }

            // Only select the tab if it is in the list
            if (tabToSelect < CurrentTabs.Count)
            {
                BrowserTabs.SelectedIndex = tabToSelect;
            }

            args.Handled = true;
        }
        #endregion

        #region Flyout Handlers
        //opens about app dialog
        private async void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //MenuButton.Flyout.Hide();
            ContentDialog aboutdialog = new AboutDialog();
            var result = await aboutdialog.ShowAsync();
        }
        private void printbutton_Click(object sender, RoutedEventArgs e)
        {
            //MenuButton.Flyout.Hide();
        }
        private async void downloadbutton_Click(object sender, RoutedEventArgs e)
        {
            //MenuButton.Flyout.Hide();
            controlCenterButton.Flyout.Hide();
            await new Downloads_Dialog().ShowAsync();

            //if (BrowserTabs.SelectedIndex >= 0)
            //    _ = CurrentTabs[BrowserTabs.SelectedIndex].Tab.OpenDownloadsDialog();

        }

        private async void editprofile_Click(object sender, RoutedEventArgs e)
        {
            //MenuButton.Flyout.Hide();
            controlCenterButton.Flyout.Hide();
            await new UserProfileDialog().ShowAsync();
        }

        private void fullscreenbutton_Click(object sender, RoutedEventArgs e)
        {
            //MenuButton.Flyout.Hide();
            var view = ApplicationView.GetForCurrentView();
            if (view.IsFullScreenMode)
            {
                view.TryEnterFullScreenMode();
                view.ExitFullScreenMode();
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
                fullscreenbutton_icon.Glyph = "\uE740";
                BrowserTabs.Margin = new Windows.UI.Xaml.Thickness(0, 0, 0, 0);
                DefaultBarUI.Height = new Windows.UI.Xaml.GridLength(40);
                fullscreentopbar.Visibility = Visibility.Collapsed;
            }
            else
            {
                view.TryEnterFullScreenMode();
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
                fullscreenbutton_icon.Glyph = "\uE73F";
                BrowserTabs.Margin = new Windows.UI.Xaml.Thickness(0, -40, 0, 0);
                DefaultBarUI.Height = new Windows.UI.Xaml.GridLength(0);
                fullscreentopbar.Visibility = Visibility.Visible;

            }
        }

        private void SettingsPageButton_Click(object sender, RoutedEventArgs e)
        {
            settings_click_helper();
        }

        private async void settings_click_helper()
        {
            var t = new BrowserTabViewItem()
            {
                CustomContentType = typeof(RadonSettings),
                ShowCustomContent = true,
                CustomHeader = "Radon Settings",
                CustomIcon = new SymbolIconSource() { Symbol = Symbol.Setting }
            };
            t.Tab.Close();
            CurrentTabs.Add(t);
            await Task.Delay(50);
            BrowserTabs.SelectedIndex = CurrentTabs.Count - 1;

        }
        #endregion

        private void BaseGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {



        }

        private async void CompactSearchBar_KeyDown(object sender, KeyRoutedEventArgs e)
        {

            if (e.Key == VirtualKey.Enter && !string.IsNullOrEmpty(SearchBar.Text))
            {
                SearchBar.Text = CompactSearchBar.Text;
                await CurrentTabs[BrowserTabs.SelectedIndex].Tab.SearchOrGoto(SearchBar.Text);
            }
            if (e.Key == VirtualKey.Escape)
            {
                //TODO: Pressing ESC will set the SearchBar.Text to WebView2 source (ESC will cancel URL changes)
                CompactSearchBar.Text = CurrentTabs[BrowserTabs.SelectedIndex].Tab.SourceUri;

                //TODO: WebView2 will steal the focus for keyboard and pointer
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void addtab_button_Click(object sender, RoutedEventArgs e)
        {
            var b = new BrowserTabViewItem();
            CurrentTabs.Add(b);
            await Task.Delay(50);
            BrowserTabs.SelectedIndex = CurrentTabs.Count - 1;
            //addtabtip.IsOpen = true;
            controlCenter.IsOpen = false;

        }

        private void tabactions_devtools_Click(object sender, RoutedEventArgs e)
        {
            if (!CurrentTabs[BrowserTabs.SelectedIndex].ShowCustomContent)
               _ = CurrentTabs[BrowserTabs.SelectedIndex].Tab.OpenDevTools();
        }

        private void tabaction_inline_Click(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (compactuibar.Visibility == Visibility.Collapsed)
            {
                compactuibar.Visibility = Visibility.Visible;
                DefaultBarUI.Height = new Windows.UI.Xaml.GridLength(0);

                BrowserTabs.TabWidthMode = TabViewWidthMode.Compact;
                compacttitlebar_rightpadding.Visibility = Visibility.Visible;

                localSettings.Values["inlineMode"] = "True";
            }

            else
            {
                compactuibar.Visibility = Visibility.Collapsed;
                DefaultBarUI.Height = new Windows.UI.Xaml.GridLength(40);

                BrowserTabs.TabWidthMode = TabViewWidthMode.Equal;

                compacttitlebar_rightpadding.Visibility = Visibility.Collapsed;

                localSettings.Values["inlineMode"] = "False";
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            addtabtip.IsOpen = false;
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ZoomSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {

        }

        private void tabaction_titlebar_Click(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            if (coreTitleBar.ExtendViewIntoTitleBar == true)
            {
                coreTitleBar.ExtendViewIntoTitleBar = false;

                var titleBar = ApplicationView.GetForCurrentView().TitleBar;

                titleBar.ButtonBackgroundColor = null;
                titleBar.ButtonInactiveBackgroundColor = null;
                titleBar.BackgroundColor = null;

                localSettings.Values["systemTitleBar"] = "True";
            }

            else
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                
                coreTitleBar.ExtendViewIntoTitleBar = true;
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.BackgroundColor = null;

                localSettings.Values["systemTitleBar"] = "False";
            }
        }

        private void ChangeThemeButton_Click(object sender, RoutedEventArgs e)
        {
            //MenuButton.Flyout.Hide();
            ThemePopup.IsOpen = true;
            
        }

        private void ThemePickerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["appcolortheme"] = (ThemePickerComboBox.SelectedItem as ComboBoxItem).Content.ToString();


            appthemebackground.Source = new BitmapImage(new Uri(string.Join("", new string[] { "ms-appx:///wallpapers/", (ThemePickerComboBox.SelectedItem as ComboBoxItem).Content.ToString(), ".png" })));

        }

        private void ThemePopupDoneButton_Click(object sender, RoutedEventArgs e)
        {
            ThemePopup.IsOpen = false;
        }

        private void RefreshButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            
        }

        private void controlCenterToggleButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void controlCenter_Closed(object sender, object e)
        {
        }

        private void controlCenter_sound_Click(object sender, RoutedEventArgs e)
        {

        }

        private void controlcenter_sidebar_Click(object sender, RoutedEventArgs e)
        {

        }

        // Forgot to name the ToggleButton, btw it's the toggle for profileCenter
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
                profileCenter.IsOpen = true;
        }

        private void profileCenter_Closed(object sender, object e)
        {
            
        }

        private void profileCenter_Opened(object sender, object e)
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            String localValue = localSettings.Values["username"] as string;

            if (localValue != null)
            {
                profileCenter_UsernameHeader.Text = localValue;
            }
            else {
               // this.Frame.Navigate(typeof(oobe1), null, new EntranceNavigationTransitionInfo());
            }
        }

        private void fullscreentopbar_Click(object sender, RoutedEventArgs e)
        {
            fullscreentopbar_flyout.IsOpen = true;
        }

        private void newWindow_Click(object sender, RoutedEventArgs e)
        {
            
            Frame newFrame = new Frame();
            newFrame.Navigate(typeof(MainPage));
            Window.Current.Content = newFrame;
            Window.Current.Activate();
        }
    }
}