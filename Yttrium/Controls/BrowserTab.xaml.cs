using Project_Radon.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Project_Radon.Controls
{
    public sealed partial class BrowserTab : Page
    {
        string OriginalUserAgent;
        string GoogleSignInUserAgent;

        public BrowserTab()
        {
            this.InitializeComponent();

            WebBrowser.CoreWebView2Initialized += delegate
            {
                OriginalUserAgent = WebBrowser.CoreWebView2.Settings.UserAgent;
                GoogleSignInUserAgent = OriginalUserAgent.Substring(0, OriginalUserAgent.IndexOf("Edg/"))
                .Replace("Mozilla/5.0", "Mozilla/4.0");
            };

        }

        public void WebBrowser_NavigationCompleted(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args, string SearchText)
        {
            WebBrowser.Focus(FocusState.Pointer);
            WebBrowser.Focus(FocusState.Keyboard);
            bool isGoogleLogin = SearchText.Contains("accounts.google.com");
            WebBrowser.CoreWebView2.Settings.UserAgent = isGoogleLogin ? GoogleSignInUserAgent : OriginalUserAgent;
        }

        public void WebBrowser_NavigationStarting(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {

        }

        public async void KeyDownSender(KeyRoutedEventArgs e, string SearchBarText)
        {
            if (e.Key == Windows.System.VirtualKey.Enter && WebBrowser != null && WebBrowser.CoreWebView2 != null)
            {
                WebBrowser.Visibility = Visibility.Visible;
                if (SearchBarText.Contains("."))
                    if (SearchBarText.Contains(':'))
                    {
                        WebBrowser.Source = new Uri(SearchBarText);
                    }
                    else
                    {
                        WebBrowser.Source = new Uri("https://" + SearchBarText);
                    }
                else if (SearchBarText.Contains(":"))
                {
                    try
                    {
                        WebBrowser.Source = new Uri(SearchBarText);
                    }
                    catch (Exception KeyDownArg)
                    {
                        ErrorDialog dialog = new ErrorDialog(KeyDownArg.ToString());
                        await dialog.ShowAsync();
                    }
                }

                else
                {
                    Search(SearchBarText);
                }
            }

            if (e.Key == Windows.System.VirtualKey.Escape)
            {
                SearchBarText = WebBrowser.Source.AbsoluteUri;
                WebBrowser.Focus(FocusState.Pointer);
                WebBrowser.Focus(FocusState.Keyboard);
            }
        }
        private void Search(string BarTextQuery)
        {
            WebBrowser.Source = new Uri("https://www.google.com/search?q=" + BarTextQuery);
        }

        public void VisibilityService(bool hidden)
        {
            if (hidden)
            {
                WebBrowser.Visibility = Visibility.Collapsed;
            }
            else
            {
                WebBrowser.Visibility = Visibility.Visible;
            }
        }

        public void BackButtonSender()
        {

        }
    }
}
