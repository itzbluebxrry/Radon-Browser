using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Project_Radon.Helpers;
using Project_Radon.Controls;
using Yttrium;
using Windows.Networking.NetworkOperators;

namespace Yttrium_browser
{
    public sealed partial class MainPage : Page
    {
        BrowserTab browserTab;
        string OriginalUserAgent;
        string GoogleSignInUserAgent;
        public static string SearchValue;

        public MainPage()
        {
            this.InitializeComponent();
            SettingsData settings = new SettingsData();
            settings.CreateSettingsFile();
            RefreshButton.IsEnabled = false;
            browserTab = new BrowserTab();
            ForwardButton.Visibility = Visibility.Collapsed;
            BackButton.IsEnabled = false;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            browserTab.BackButtonSender();
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            browserTab.FowardButtonSender();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            browserTab.Reload();
        }

        //navigation completed
        private async void WebBrowser_NavigationCompleted(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            if (SearchBar.FocusState == FocusState.Unfocused)
            {
                SearchBar.Text = WebBrowser.Source.AbsoluteUri;
            }

            if (WebBrowser.CanGoForward)
            {
                ForwardButton.Visibility = Visibility.Visible;
            }
            else
            {
                ForwardButton.Visibility = Visibility.Collapsed;
            }

            if (WebBrowser.CanGoBack)
            {
                BackButton.IsEnabled = true;
            }
            else
            {
                BackButton.IsEnabled = false;
            }
            
            //website load status
            try
            {
                Uri icoURI = new Uri("https://www.google.com/s2/favicons?sz=48&domain_url=" + WebBrowser.Source);
                faviconicon.UriSource = icoURI;
                faviconicon.ShowAsMonochrome = false;

                RefreshButton.Visibility = Visibility.Visible;
                StopRefreshButton.Visibility = Visibility.Collapsed;
                if (!loadingbar.ShowError == true)
                {
                    loadingbar.IsIndeterminate = false;
                } 
            }
            catch (Exception ExLoader)
            {
                ErrorDialog dialog = new ErrorDialog(ExLoader.ToString());
                await dialog.ShowAsync();
            }


            if (SearchValue.Contains("https"))
            {
                //change icon to lock
                SSLIcon.FontFamily = new FontFamily("Segoe Fluent Icons");
                SSLIcon.Glyph = "\xe705";

                //change icon to lock
                SSLFlyoutIcon.FontFamily = new FontFamily("Segoe Fluent Icons");
                SSLFlyoutIcon.Glyph = "\xe705";
                SSLFlyoutHeader.Text = "Your connection is secured";
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
        }

        private async void SearchBar_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            SearchValue = SearchBar.Text;
            browserTab.KeyDownSender(e, SearchValue);
        }

        private void SearchBar_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchBar.SelectAll();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            browserTab.VisibilityService(true);
            SearchBar.Text = string.Empty;
        }

        //opens settings page
        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }


        //handles progressring and refresh behavior
        private void WebBrowser_NavigationStarting(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {
            browserTab.WebBrowser_NavigationStarting(sender, args);
            RefreshButton.Visibility = Visibility.Collapsed;
            StopRefreshButton.Visibility = Visibility.Visible;
            RefreshButton.IsEnabled = true;
            loadingbar.IsIndeterminate = true;
        }

        //stops refreshing if clicked on progressbar
        private async void StopRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            browserTab.Stop();
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
        private void BrowserTabs_AddTabButtonClick(TabView sender, object args)
        {
            sender.TabItems.Add(CreateNewTab(sender.TabItems.Count));
        }

        private TabViewItem CreateNewTab(int index)
        {
            TabViewItem newItem = new TabViewItem();

            newItem.Header = $"New Tab";
            newItem.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Document };

            Frame frame = new Frame();

            frame.Navigate(typeof(BrowserTab));

            newItem.Content = frame;
            return newItem;
        }
        private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            if (sender.TabItems.Count <= 1)
                BrowserTabs_AddTabButtonClick(sender, args);

            sender.TabItems.Remove(args.Tab);
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MenuButton.Flyout.Hide();
        }
        private void TabView_Loaded(object sender, RoutedEventArgs e)
        {
            MenuButton.Flyout.Hide();
        }
        #endregion

        #region Flyout Handlers
        //opens about app dialog
        private async void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuButton.Flyout.Hide();
            ContentDialog aboutdialog = new AboutDialog();
            var result = await aboutdialog.ShowAsync();
        }
        private void printbutton_Click(object sender, RoutedEventArgs e)
        {
            MenuButton.Flyout.Hide();
        }
        private async void downloadbutton_Click(object sender, RoutedEventArgs e)
        {
            MenuButton.Flyout.Hide();
            await Launcher.LaunchUriAsync(new Uri("https://google.com"));
        }

        private async void editprofile_Click(object sender, RoutedEventArgs e)
        {
            MenuButton.Flyout.Hide();
            await new UserProfileDialog().ShowAsync();
        }

        private void fullscreenbutton_Click(object sender, RoutedEventArgs e)
        {
            MenuButton.Flyout.Hide();
            var view = ApplicationView.GetForCurrentView();
            if (view.IsFullScreenMode)
            {
                view.ExitFullScreenMode();
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
            }
            else
            {
                if (view.TryEnterFullScreenMode())
                {
                    ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
                }
            }
        }
        #endregion
    }
}