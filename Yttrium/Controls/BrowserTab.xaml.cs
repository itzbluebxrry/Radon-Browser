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
using CommunityToolkit.Mvvm;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.ComponentModel;
using System.Web;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Project_Radon.Controls
{
    public sealed partial class BrowserTab : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public event EventHandler<string> NewTabRequested = delegate { };
        internal void Set<T>(ref T obj, T value, string name = null)
        {
            obj = value;
            InvokePropertyChanged(name);
        }

        public void InvokePropertyChanged(string name = null)
        {
            this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }
        string OriginalUserAgent;
        string GoogleSignInUserAgent;

        private bool _IsLoading;
        public bool IsLoading
        {
            get => _IsLoading;
            set => Set(ref _IsLoading,value);
        }
        public bool CanGoBack => WebBrowser.CanGoBack;
        public bool CanGoFoward => WebBrowser.CanGoForward;
        public string SourceUri => IsCoreInitialized ? WebBrowser.Source.AbsoluteUri : "";
        public string Favicon => IsCoreInitialized ? ("https://www.google.com/s2/favicons?sz=48&domain_url=" + WebBrowser.Source.AbsoluteUri) : null;
        public string Title => IsCoreInitialized ? (WebBrowser.CoreWebView2.DocumentTitle ?? WebBrowser.Source.AbsoluteUri) : "Loading..";
        public bool IsCoreInitialized { get; private set; }
        public BrowserTab()
        {
            this.InitializeComponent();
            WebBrowser.CoreWebView2Initialized += delegate
            {
                IsCoreInitialized = true;
                OriginalUserAgent = WebBrowser.CoreWebView2.Settings.UserAgent;
                GoogleSignInUserAgent = OriginalUserAgent.Substring(0, OriginalUserAgent.IndexOf("Edg/"))
                .Replace("Mozilla/5.0", "Mozilla/4.0");
                WebBrowser.CoreWebView2.Settings.UserAgent = GoogleSignInUserAgent;
                WebBrowser.CoreWebView2.NewWindowRequested += delegate (Microsoft.Web.WebView2.Core.CoreWebView2 s, Microsoft.Web.WebView2.Core.CoreWebView2NewWindowRequestedEventArgs e)
                {
                    e.Handled = true;
                    NewTabRequested.Invoke(s, e.Uri);
                };
            };

        }


        private void WebBrowser_NavigationCompleted(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            WebBrowser.Focus(FocusState.Pointer);
            WebBrowser.Focus(FocusState.Keyboard);
            IsLoading = false;
        }
        public void Close() => WebBrowser.Close();
        private void WebBrowser_NavigationStarting(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {
            IsLoading = true;
        }
        public async Task SearchOrGoto(string SearchBarText)
        {
            //Won't work as we expected
            Regex UrlMatch = new Regex(@"(?i)(http(s)?:\/\/)?(\w{2,25}\.)+\w{3}([a-z0-9\-?=$-_.+!*()]+)(?i)", RegexOptions.Singleline);
            var v = UrlMatch.Match(SearchBarText).Value;
            v.ToString();
            await WebBrowser.EnsureCoreWebView2Async();
            WebBrowser.Visibility = Visibility.Visible;
            if (UrlMatch.IsMatch(SearchBarText))
                WebBrowser.Source = Uri.TryCreate(SearchBarText,UriKind.Absolute,out var r) ? r : new Uri("https://www.google.com/search?q=" + HttpUtility.UrlEncode(SearchBarText));
            else
                WebBrowser.Source = new Uri("https://www.google.com/search?q=" + HttpUtility.UrlEncode(SearchBarText));
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
        public async Task OpenDownloadsDialog()
        {
            await WebBrowser.EnsureCoreWebView2Async();
            WebBrowser.CoreWebView2.DefaultDownloadDialogCornerAlignment = Microsoft.Web.WebView2.Core.CoreWebView2DefaultDownloadDialogCornerAlignment.TopRight;
            WebBrowser.CoreWebView2.OpenDefaultDownloadDialog();
        }
        public void BackButtonSender()
        { 
            if (WebBrowser.CanGoBack)
            {
                WebBrowser.GoBack();
            }
            WebBrowser.Visibility = Visibility.Visible;
        }
        public void FowardButtonSender()
        {
            if (WebBrowser.CanGoForward)
            {
                WebBrowser.GoForward();
            }
            WebBrowser.Visibility = Visibility.Visible;
        }
        public void Reload()
        {
            WebBrowser.Reload();
        }
        public void Stop()
        {
            WebBrowser.CoreWebView2.Stop();
        }

    }
}
