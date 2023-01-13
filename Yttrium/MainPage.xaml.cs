using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Yttrium;



// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Yttrium_browser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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
            if (WebBrowser.CanGoForward)
            {

            }
            else
            {
                ForwardButton.Visibility = Visibility.Collapsed;
            }

            if (WebBrowser.CanGoBack)
            {
                
            }
            else
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
                BackButton.IsEnabled= false;
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
                ForwardButton.Visibility =Visibility.Collapsed;
                
            }
            WebBrowser.Visibility = Visibility.Visible;

        }

        //refresh 
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowser.Reload();
            

        }

        //navigation completed
        private void WebBrowser_NavigationCompleted(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {

            if (SearchBar.FocusState == FocusState.Unfocused)
            {
                SearchBar.Text = WebBrowser.Source.AbsoluteUri;
                WebBrowser.Focus(FocusState.Pointer);
                WebBrowser.Focus(FocusState.Keyboard);
            }

            WebBrowser.Visibility = Visibility.Visible;

            if (WebBrowser.Source.AbsoluteUri.Contains("start.zorin"))
            {
                SearchBar.Text = "Search the web or enter a URL";
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
                WebBrowser.CoreWebView2.Settings.IsStatusBarEnabled = false;
                Uri icoURI = new Uri("https://www.google.com/s2/favicons?sz=64&domain_url=" + WebBrowser.Source);
                // FaviconIcon.Source = new Windows.UI.Xaml.Media.ImageSource() == icoURI;
                // favicon. = WebBrowser.CoreWebView2.DocumentTitle.ToString();
                
                

                RefreshButton.Visibility = Visibility.Visible;
                StopRefreshButton.Visibility = Visibility.Collapsed;
                if (loadingbar.ShowError == true)
                {
                    //do nothing
                }
                else
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
            catch
            {

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
                SSLFlyoutStatus.Text = "This site has a valid SSL certificate with secure connections.";
                SSLFlyoutStatus2.Text = "Your personal informations will be securely sent to the site and cannot be intercepted or seen by others.";

                //ToolTip tooltip = new ToolTip
                //{
                //    Content = "This website has a SSL certificate"
                //};
                //ToolTipService.SetToolTip(SSLButton, tooltip);

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
                SSLFlyoutStatus2.Text = "Informations will not be securely sent to this site and likely to be intercepted and seen by others.";

            }


            //            await WebBrowser.EnsureCoreWebView2Async();
            //            await WebBrowser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(@"
            //document.addEventListener('DOMContentLoaded', function() {
            //  const style = document.createElement('style');
            //  style.textContent = '/* width */ \
            //::-webkit-scrollbar { \
            //  width: 20px !important; \
            //} \
            // \
            //::-webkit-scrollbar-track { \
            //  background: red !important; \
            //}';
            //  document.head.append(style);
            //}, false);");
        }

        //if enter is pressed, it searches text in SearchBar or goes to web page
        private void SearchBar_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            
            if (e.Key == Windows.System.VirtualKey.Enter && WebBrowser != null && WebBrowser.CoreWebView2 != null)
            {
                WebBrowser.Visibility = Visibility.Visible;
                if (SearchBar.Text.Contains("."))

                        WebBrowser.Source = new Uri("https://" + SearchBar.Text);

                else if (SearchBar.Text.Contains(":"))
                {
                    WebBrowser.Source = new Uri(SearchBar.Text);
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
            //if (SearchBar.Text.Contains("https://") || SearchBar.Text.Contains("http://"))
            //{
            //    WebBrowser.Source = new Uri(SearchBar.Text);
            //}
            //else
            //{
            //    WebBrowser.Source = new Uri("https://www.google.com/search?q=" + SearchBar.Text);
            //}
            //string link = "https://" + SearchBar.Text;
            //WebBrowser.CoreWebView2.Navigate(link);

            WebBrowser.Source = new Uri("https://www.google.com/search?q=" + SearchBar.Text);
            //SearchBar.Text = newTab.Content == new HomePage() ? "Home page" : WebBrowser.Source.AbsoluteUri;

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
            //WebView2 webView = new WebView2();
            //await webView.EnsureCoreWebView2Async();
            //webView.CoreWebView2.Navigate("https://google.com");
            //newTab.Content = new HomePage();
            //sender.TabItems.Add(new TabViewItem() { Content = newTab });
            //sender.SelectedItem = newTab ;
            //SearchBar.Text = newTab.Header.ToString();
            
        }

        //close tab
        private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            if (sender.TabItems.Count <= 1)
                Tabs_AddTabButtonClick(sender, args);

            sender.TabItems.Remove(args.Tab);
        }
        //opens about app dialog
        private async void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog aboutdialog = new AboutDialog();

            var result = await aboutdialog.ShowAsync();

        }


        private void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

    }
}

