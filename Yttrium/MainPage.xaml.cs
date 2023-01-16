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
using Yttrium;

namespace Yttrium_browser
{
    public sealed partial class MainPage : Page
    {
        string OriginalUserAgent;
        string GoogleSignInUserAgent;

        public MainPage()
        {
            this.InitializeComponent();
            //creates settings file on app first launch
            SettingsData settings = new SettingsData();
            settings.CreateSettingsFile();

            RefreshButton.IsEnabled = false;

            //google login fix
            WebBrowser.CoreWebView2Initialized += delegate
            {
                OriginalUserAgent = WebBrowser.CoreWebView2.Settings.UserAgent;
                GoogleSignInUserAgent = OriginalUserAgent.Substring(0, OriginalUserAgent.IndexOf("Edg/"))
                .Replace("Mozilla/5.0", "Mozilla/4.0");
            };

            //Navigation Buttons on initialize
            if (!WebBrowser.CanGoForward)
            {
                ForwardButton.Visibility = Visibility.Collapsed;
            }

            if (!WebBrowser.CanGoBack)
            {
                BackButton.IsEnabled = false;
            }
        }


        //back navigation
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (WebBrowser.CanGoBack)
            {
                WebBrowser.GoBack();
            }
            else
            {
                BackButton.IsEnabled = false;
            }
            WebBrowser.Visibility = Visibility.Visible;
        }

        //forward navigation
        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {

            if (WebBrowser.CanGoForward)
            {
                WebBrowser.GoForward();
            }
            else
            {
                ForwardButton.Visibility = Visibility.Collapsed;
                
            }
            WebBrowser.Visibility = Visibility.Visible;

        }

        //refresh 
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowser.Reload();
        }

        //navigation completed
        private async void WebBrowser_NavigationCompleted(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {

            if (SearchBar.FocusState == FocusState.Unfocused)
            {
                SearchBar.Text = WebBrowser.Source.AbsoluteUri;
                WebBrowser.Focus(FocusState.Pointer);
                WebBrowser.Focus(FocusState.Keyboard);
            }

            WebBrowser.Visibility = Visibility.Visible;

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
                WebBrowser.CoreWebView2.Settings.IsStatusBarEnabled = false;
                Uri icoURI = new Uri("https://www.google.com/s2/favicons?sz=48&domain_url=" + WebBrowser.Source);
                faviconicon.UriSource = icoURI;
                faviconicon.ShowAsMonochrome = false;

                RefreshButton.Visibility = Visibility.Visible;
                StopRefreshButton.Visibility = Visibility.Collapsed;
                if (!loadingbar.ShowError == true)
                {
                    loadingbar.IsIndeterminate = false;
                }
                
                //history
                DataTransfer datatransfer = new DataTransfer();
                if (!string.IsNullOrEmpty(SearchBar.Text))
                {
                    datatransfer.SaveSearchTerm(SearchBar.Text, WebBrowser.CoreWebView2.DocumentTitle, WebBrowser.Source.AbsoluteUri);
                }
            }
            catch (Exception ExLoader)
            {
                ErrorDialog dialog = new ErrorDialog(ExLoader.ToString());
                await dialog.ShowAsync();
            }


            if (WebBrowser.Source.AbsoluteUri.Contains("https"))
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

        //if enter is pressed, it searches text in SearchBar or goes to web page
        private async void SearchBar_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            
            if (e.Key == Windows.System.VirtualKey.Enter && WebBrowser != null && WebBrowser.CoreWebView2 != null)
            {
                WebBrowser.Visibility = Visibility.Visible;
                if (SearchBar.Text.Contains("."))
                    if (SearchBar.Text.Contains(':')) 
                    {
                        WebBrowser.Source = new Uri(SearchBar.Text);
                    }
                    else 
                    {
                        WebBrowser.Source = new Uri("https://" + SearchBar.Text);
                    }
                else if (SearchBar.Text.Contains(":"))
                {
                    try
                    {
                        WebBrowser.Source = new Uri(SearchBar.Text);
                    }
                    catch (Exception KeyDownArg)
                    {
                        ErrorDialog dialog = new ErrorDialog(KeyDownArg.ToString());
                        await dialog.ShowAsync();
                    }
                }

                else
                {
                    Search();
                }
            }

            if (e.Key == Windows.System.VirtualKey.Escape)
            {
                SearchBar.Text = WebBrowser.Source.AbsoluteUri;
                WebBrowser.Focus(FocusState.Pointer);
                WebBrowser.Focus(FocusState.Keyboard);
            }
        }

        //if clicked on SearchBar, the text will be selected
        private void SearchBar_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchBar.SelectAll();
        }

        //method for search engine + updates link text in SearchBar
        private void Search()
        {
            WebBrowser.Source = new Uri("https://www.google.com/search?q=" + SearchBar.Text);
        }

        //home button redirects to homepage
        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowser.Visibility = Visibility.Collapsed;
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
            RefreshButton.Visibility = Visibility.Collapsed;
            StopRefreshButton.Visibility = Visibility.Visible;
            WebBrowser.Focus(FocusState.Pointer);
            WebBrowser.Focus(FocusState.Keyboard);
            RefreshButton.IsEnabled = true;
            loadingbar.IsIndeterminate = true;
            var isGoogleLogin = new Uri(args.Uri).Host.Contains("accounts.google.com");
            WebBrowser.CoreWebView2.Settings.UserAgent = isGoogleLogin ? GoogleSignInUserAgent : OriginalUserAgent;
        }

        //stops refreshing if clicked on progressbar
        private async void StopRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowser.CoreWebView2.Stop();
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

        //add new tab
        private void Tabs_AddTabButtonClick(TabView sender, object args)
        {
            sender.TabItems.Add(CreateNewTab(sender.TabItems.Count));
        }

        private TabViewItem CreateNewTab(int index)
        {
            TabViewItem newItem = new TabViewItem();

            newItem.Header = $"New Tab {index}";
            newItem.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Document };

            Frame frame = new Frame();

            frame.Navigate(typeof(HomePage));

            newItem.Content = frame;
            return newItem;
        }
        private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            if (sender.TabItems.Count <= 1)
                Tabs_AddTabButtonClick(sender, args);

            sender.TabItems.Remove(args.Tab);
        }

        //opens about app dialog
        private async void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuButton.Flyout.Hide();
            ContentDialog aboutdialog = new AboutDialog();
            var result = await aboutdialog.ShowAsync();
        }

        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MenuButton.Flyout.Hide();
        }

        private void printbutton_Click(object sender, RoutedEventArgs e)
        {
            MenuButton.Flyout.Hide();
        }

        private void TabView_Loaded(object sender, RoutedEventArgs e)
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
                tabs_grid.Height = new GridLength(40);
            }
            else
            {
                if (view.TryEnterFullScreenMode())
                {
                    ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
                    tabs_grid.Height = new GridLength(0);
                }
            }
        }
    }
}