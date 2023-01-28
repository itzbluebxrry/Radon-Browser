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
using CommunityToolkit.Mvvm;
using Windows.Networking.NetworkOperators;
using System.ServiceModel.Channels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SymbolIconSource = Microsoft.UI.Xaml.Controls.SymbolIconSource;
using Project_Radon.Settings;

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
            this.InitializeComponent();
            CurrentTabs.Add(new BrowserTabViewItem());
            CurrentTabs[0].Tab.PropertyChanged += SelectedTabPropertyChanged;


        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentTabs[BrowserTabs.SelectedIndex].Tab.BackButtonSender();
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentTabs[BrowserTabs.SelectedIndex].Tab.FowardButtonSender();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentTabs[BrowserTabs.SelectedIndex].Tab.Reload();
        }

        //TODO: On window size changed, if navbuttonbar.Width is less than 140, hide the standard nav buttons and show the overflow buttons.

        //navigation completed
        private async void WebBrowser_NavigationCompleted(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            /*if (SearchBar.FocusState == FocusState.Unfocused)
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
            }*/
            
            //website load status
            try
            {
                Uri icoURI = new Uri("https://www.google.com/s2/favicons?sz=48&domain_url=" + SearchValue);
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
            if(e.Key == VirtualKey.Enter && !string.IsNullOrEmpty(SearchBar.Text))
            await CurrentTabs[BrowserTabs.SelectedIndex].Tab.SearchOrGoto(SearchBar.Text);
            if (e.Key == VirtualKey.Escape)
            {   
                //TODO: Pressing ESC will set the SearchBar.Text to WebView2 source (ESC will cancel URL changes)
                SearchBar.Text = CurrentTabs[BrowserTabs.SelectedIndex].Tab.SourceUri;
                
                //TODO: WebView2 will steal the focus for keyboard and pointer
            }

        }

        //TODO: Create a looping function which will update favicon and tab title with 500 - 1000ms interval (What most browser does)


        private void SearchBar_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchBar.SelectAll();
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
                loadingbar.Visibility = CurrentTabs[BrowserTabs.SelectedIndex].Tab.IsLoading ? Visibility.Visible : Visibility.Collapsed;
                StopRefreshButton.Visibility = CurrentTabs[BrowserTabs.SelectedIndex].Tab.IsLoading ? Visibility.Visible : Visibility.Collapsed;
                RefreshButton.Visibility = !CurrentTabs[BrowserTabs.SelectedIndex].Tab.IsLoading ? Visibility.Visible : Visibility.Collapsed;
                BackButton.IsEnabled = CurrentTabs[BrowserTabs.SelectedIndex].Tab.CanGoBack ? IsEnabled : false;
                ForwardButton.Visibility = CurrentTabs[BrowserTabs.SelectedIndex].Tab.CanGoFoward ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        private void NewTabRequested(object s,string e)
        {
            var b = new BrowserTabViewItem();
            CurrentTabs.Add(b);
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
        private void BrowserTabs_AddTabButtonClick(TabView sender, object args)
        {
            CurrentTabs.Add(new BrowserTabViewItem());
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
            MenuButton.Flyout.Hide();
        }
        private void TabView_Loaded(object sender, RoutedEventArgs e)
        {
            MenuButton.Flyout.Hide();
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
            MenuButton.Flyout.Hide();
            ContentDialog aboutdialog = new AboutDialog();
            var result = await aboutdialog.ShowAsync();
        }
        private void printbutton_Click(object sender, RoutedEventArgs e)
        {
            MenuButton.Flyout.Hide();
        }
        private void downloadbutton_Click(object sender, RoutedEventArgs e)
        {
            MenuButton.Flyout.Hide();

            if (BrowserTabs.SelectedIndex >= 0)
                _ = CurrentTabs[BrowserTabs.SelectedIndex].Tab.OpenDownloadsDialog();
            
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
                view.TryEnterFullScreenMode();
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

        private void SettingsPageButton_Click(object sender, RoutedEventArgs e)
        {
            var t = new BrowserTabViewItem()
            {
                CustomContentType = typeof(RadonSettings),
                ShowCustomContent = true,
                CustomHeader = "Radon Settings",
                CustomIcon = new SymbolIconSource() { Symbol = Symbol.Setting}
            };
            t.Tab.Close();
            CurrentTabs.Add(t);
            BrowserTabs.SelectedIndex = CurrentTabs.Count - 1;
        }
        #endregion
    }
}