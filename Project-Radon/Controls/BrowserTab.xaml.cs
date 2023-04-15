using Fluent.Icons;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

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
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }
        string OriginalUserAgent;
        string GoogleSignInUserAgent;

        private bool _IsLoading;
        public bool IsLoading
        {
            get => _IsLoading;
            set => Set(ref _IsLoading, value);
        }
        public bool CanGoBack => WebBrowser.CanGoBack;
        public bool CanGoFoward => WebBrowser.CanGoForward;
        public string SourceUri => IsCoreInitialized ? (WebBrowser.Source.AbsoluteUri.ToLower().Contains("edge://") ? "radon://" + WebBrowser.Source.AbsoluteUri.Remove(0, 7) : WebBrowser.Source.AbsoluteUri) : "";
        public string Favicon => IsCoreInitialized && !IsLoading ? ("https://icons.duckduckgo.com/ip3/" + WebBrowser.Source.AbsoluteUri+".ico") : "https://raw.githubusercontent.com/microsoft/fluentui-system-icons/main/assets/Document/SVG/ic_fluent_document_48_regular.svg";
        public string Title => IsCoreInitialized && !IsLoading ? (WebBrowser.CoreWebView2.DocumentTitle ?? WebBrowser.Source.AbsoluteUri) : "Loading";
        public bool IsCoreInitialized { get; private set; }
        public BrowserTab()
        {
            InitializeComponent();

            WebBrowser.Source = new Uri("about:radon-ntp");
            
            //Windows.System.Launcher.LaunchFolderAsync(ApplicationData.Current.LocalFolder);
            WebBrowser.CoreWebView2Initialized += delegate
            {
                IsCoreInitialized = true;
                OriginalUserAgent = WebBrowser.CoreWebView2.Settings.UserAgent;
                GoogleSignInUserAgent = OriginalUserAgent.Substring(0, OriginalUserAgent.IndexOf("Edg/"))
                .Replace("Mozilla/5.0", "Mozilla/4.0");
                WebBrowser.CoreWebView2.Settings.UserAgent = GoogleSignInUserAgent;
                WebBrowser.CoreWebView2.DocumentTitleChanged += (_, e) => InvokePropertyChanged();
                WebBrowser.CoreWebView2.SourceChanged+= (_, e) => InvokePropertyChanged();
                WebBrowser.CoreWebView2.ContextMenuRequested += async (s, e) =>
                {
                    IList<CoreWebView2ContextMenuItem> menuList = e.MenuItems;
                    if (e.ContextMenuTarget.HasLinkUri)
                    {
                        CoreWebView2ContextMenuItem newItem = WebBrowser.CoreWebView2.Environment.CreateContextMenuItem("Open link in new tab", null, CoreWebView2ContextMenuItemKind.Command);
                        newItem.CustomItemSelected += (CoreWebView2ContextMenuItem sender, object args) =>
                        {
                            NewTabRequested.Invoke(s, e.ContextMenuTarget.LinkUri);
                        };
                        for (int index = 0; index < menuList.Count; index++)
                        {
                            if (menuList[index].Name == "openLinkInNewWindow")
                            {
                                menuList.RemoveAt(index);
                                break;
                            }
                        }
                    }
                };
            };

            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            String username = localSettings.Values["username"] as string;
            ToolTip toolTip = new ToolTip
            {
                Content = username
            };
            ToolTipService.SetToolTip(profileCenterToggle, toolTip);
        }


        private void WebBrowser_NavigationCompleted(Microsoft.UI.Xaml.Controls.WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            ntpSearchBar.Text = string.Empty;
            IsLoading = false;
            if (WebBrowser.Source.Equals("edge://radon-ntp/"))
            {
                WebBrowser.Visibility = Visibility.Collapsed;
                ntpGrid.Visibility = Visibility.Visible;
                ntpbackgroundbrush.ImageSource = new BitmapImage(new Uri("https://bing.biturl.top/?resolution=1366&format=image&index=random&mkt=en-US"));

            }
            else
            {
                WebBrowser.Visibility = Visibility.Visible;
                ntpGrid.Visibility = Visibility.Collapsed;
            }
        }
        public void Close() => WebBrowser.Close();
        private void WebBrowser_NavigationStarting(Microsoft.UI.Xaml.Controls.WebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {
            IsLoading = true;
            WebBrowser.Focus(FocusState.Pointer);
            WebBrowser.Focus(FocusState.Keyboard);

            
        }
        public async Task GoTo(string url)
        {
            Regex UrlMatch = new Regex("^(http(s)?:\\/\\/.)?(www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{2,256}\\.[a-z]{2,6}\\b([-a-zA-Z0-9@:%_\\+.~#?&//=]*)$", RegexOptions.Singleline);
            await WebBrowser.EnsureCoreWebView2Async();
            if (url.ToLower().StartsWith("radon://"))
            {
                WebBrowser.Source = new Uri("edge://" + url.Remove(0, 7));
            }
            else if (UrlMatch.IsMatch(url) || url.StartsWith("https://") || url.StartsWith("http://") || url.StartsWith("edge://"))
            {
                WebBrowser.Source = new Uri(!Uri.TryCreate(url, UriKind.Absolute, out var r) || !r.IsAbsoluteUri ? "https://" + url : url);
            }
        }
        public async Task SearchOrGoto(string SearchBarText)
        {
            Regex UrlMatch = new Regex("^(http(s)?:\\/\\/.)?(www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{2,256}\\.[a-z]{2,6}\\b([-a-zA-Z0-9@:%_\\+.~#?&//=]*)$", RegexOptions.Singleline);
            await WebBrowser.EnsureCoreWebView2Async();
            if (SearchBarText.ToLower().StartsWith("radon://"))
            {
                WebBrowser.Source = new Uri("edge://" + SearchBarText.Remove(0, 7));
            }
            else if (UrlMatch.IsMatch(SearchBarText) || SearchBarText.StartsWith("https://") || SearchBarText.StartsWith("http://") || SearchBarText.StartsWith("edge://"))
            {
                WebBrowser.Source = new Uri(!Uri.TryCreate(SearchBarText, UriKind.Absolute, out var r) || !r.IsAbsoluteUri ? "https://" + SearchBarText : SearchBarText);
            }
            else
            {
                WebBrowser.Source = new Uri("https://www.google.com/search?q=" + HttpUtility.UrlEncode(SearchBarText));
            }
        }

        public async Task OpenDownloadsDialog()
        {
            await WebBrowser.EnsureCoreWebView2Async();
            WebBrowser.CoreWebView2.DefaultDownloadDialogCornerAlignment = Microsoft.Web.WebView2.Core.CoreWebView2DefaultDownloadDialogCornerAlignment.TopRight;
            WebBrowser.CoreWebView2.OpenDefaultDownloadDialog();
        }
        public async Task OpenDevTools()
        {
            await WebBrowser.EnsureCoreWebView2Async();
            WebBrowser.CoreWebView2.OpenDevToolsWindow();
        }
        public void BackButtonSender()
        {
            if (WebBrowser.CanGoBack)
            {
                WebBrowser.GoBack();
            }
        }
        public void FowardButtonSender()
        {
            if (WebBrowser.CanGoForward)
            {
                WebBrowser.GoForward();
            }
        }
        public void Reload()
        {
            WebBrowser.Reload();
        }
        public void Stop()
        {
            WebBrowser.CoreWebView2.Stop();
        }
        public async Task ExecuteScriptAsyc(string script)
        {
            await WebBrowser.EnsureCoreWebView2Async();
            await WebBrowser.CoreWebView2.ExecuteScriptAsync(script);
        }

        private void ntpSearchBar_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter && !string.IsNullOrEmpty(ntpSearchBar.Text))
            {
                WebBrowser.Source = new Uri("about:blank");
                ntpGrid.Visibility = Visibility.Collapsed;
                WebBrowser.Source = new Uri("https://www.google.com/search?q=" + HttpUtility.UrlEncode(ntpSearchBar.Text));
            }
        }

        private void ntpSearchBar_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {

        }
    }
}
