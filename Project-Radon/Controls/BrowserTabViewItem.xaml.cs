﻿using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using IconSource = Microsoft.UI.Xaml.Controls.IconSource;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Project_Radon.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowserTabViewItem : TabViewItem, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        internal void Set<T>(ref T obj, T value, string name = null)
        {
            obj = value;
            InvokePropertyChanged(name);
        }

        public void InvokePropertyChanged(string name = null)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public BrowserTab Tab { get; private set; } = new BrowserTab();

        private Type _CustomContentType;
        public Type CustomContentType
        {
            get => _CustomContentType;
            set => Set(ref _CustomContentType, value);
        }

        private bool _ShowCustomContent;
        public bool ShowCustomContent
        {
            get => _ShowCustomContent;
            set => Set(ref _ShowCustomContent, value);
        }

        private string _CustomHeader;
        public string CustomHeader
        {
            get => _CustomHeader;
            set => Set(ref _CustomHeader, value);
        }

        private IconSource _CustomIcon;
        public IconSource CustomIcon
        {
            get => _CustomIcon;
            set => Set(ref _CustomIcon, value);
        }
        private object TabContent => ShowCustomContent && CustomContentType != null ? Activator.CreateInstance(CustomContentType) : Tab;
        private object TabHeader => CustomHeader ?? Tab.Title;

        private object TabSourceUri => Tab.SourceUri.ToString();


        public BrowserTabViewItem()
        {
            InitializeComponent();
            Tab.PropertyChanged += Tab_PropertyChanged;
            PropertyChanged += Tab_PropertyChanged;
        }

        private void Tab_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            VisualStateManager.GoToState(this, Tab.IsLoading ? "Loading" : "NotLoading", false);
            IconSource = CustomIcon ?? new ImageIconSource() { ImageSource = new BitmapImage(new Uri(Tab.Favicon)) };
            PropertyChanged -= Tab_PropertyChanged;
            InvokePropertyChanged();
            PropertyChanged += Tab_PropertyChanged;
        }
    }
}
