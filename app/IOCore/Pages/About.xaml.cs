using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel;
using CommunityToolkit.WinUI.Helpers;
using IOCore.Libs;
using IOCore;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IOCore.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class About : Page, INotifyPropertyChanged
    {
        public static About Inst { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private static readonly ResourceLoader _resourceLoader = ResourceLoader.GetForViewIndependentUse();

        public class Item
        {
            public int Row { get; set; }

            public int Column { get; set; }

            public string Name { get; set; }
            public string Icon { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Link { get; set; }

            public Item(
                int row, int column,
                string name, string icon, string title, string description,
                string link)
            {
                Row = row;
                Column = column;
                Name = name;
                Icon = icon;
                Title = title;
                Description = description;
                Link = link;
            }
        }

        private ContentDialog _dialog;

        private string _appName;
        public string AppName { get => _appName; set { _appName = value; PropertyChanged?.Invoke(this, new(nameof(AppName))); } }

        private string _version;
        public string Version { get => _version; set { _version = value; PropertyChanged?.Invoke(this, new(nameof(Version))); } }

        private string _credit;
        public string Credit { get => _credit; set { _credit = value; PropertyChanged?.Invoke(this, new(nameof(Credit))); } }

        private Uri _policyUri;
        public Uri PolicyUri { get => _policyUri; set { _policyUri = value; PropertyChanged?.Invoke(this, new(nameof(PolicyUri))); } }

        private readonly ObservableCollection<Item> _items = new()
        {
            new (1, 0,
                "leaveAnIdea",
                "\uE913",
                _resourceLoader.GetString("About_ShareValuableIdeas"),
                _resourceLoader.GetString("About_ShareValuableIdeasDescription"),
                Meta.URL_APP_STORE_REVIEW),
            new (0, 1,
                "publisher",
                "\uE8F9",
                _resourceLoader.GetString("About_DiscoverMoreApps"),
                _resourceLoader.GetString("About_DiscoverMoreAppsDescription"),
                Meta.URL_APP_STORE_PUBLISHER),
            new (0, 1,
                "twitter",
                "\uE910",
                _resourceLoader.GetString("About_ConnectWithUsOnTwitter"),
                _resourceLoader.GetString("About_ConnectWithUsOnTwitterDescription"),
                Meta.URL_TWITTER),
            new (1, 1,
                "ioStream",
                "\uE774",
                string.Format(_resourceLoader.GetString("About_OurWebsite"), Package.Current.DisplayName),
                _resourceLoader.GetString("About_OurWebsiteDescription"),
                Meta.URL_IO_APP)
        };

        public ObservableCollection<Item> Items => _items;

        public About(ContentDialog dialog)
        {
            InitializeComponent();
            Inst = this;
            DataContext = this;

            _dialog = dialog;

            AppIconImage.Source = ImageMagickUtils.AppIcon.Load(96, 96);
            AppIconImage.Width = AppIconImage.Height = 96;

            AppName = Package.Current.DisplayName;
            Version = $"v{Package.Current.Id.Version.ToFormattedString(3)}";
            PolicyUri = new(Meta.URL_IO_PRIVACY);

            Credit = string.Format(_resourceLoader.GetString("About_Credit"), DateTime.Now.Year, Package.Current.PublisherDisplayName);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement).DataContext is not Item item) return;
            _ = Launcher.LaunchUriAsync(new(item.Link));
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _dialog.Hide();
        }
    }
}
