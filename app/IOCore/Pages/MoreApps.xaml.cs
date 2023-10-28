using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.System;
using System.ComponentModel;
using System.Linq;
using Microsoft.UI.Xaml.Input;
using IOCore.Libs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IOCore.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MoreApps : Page, INotifyPropertyChanged
    {
        public static MoreApps Inst { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private ContentDialog _dialog;

        public RangeObservableCollection<AppItem> FeaturedAppItems { get; private set; } = new();

        private int _appItemOffset = 0;
        private readonly DispatcherTimer _appTimer = new() { Interval = TimeSpan.FromSeconds(1) };
        private int _progressValue = 0;
        public int ProgressValue { get => _progressValue; set { _progressValue = value; PropertyChanged?.Invoke(this, new(nameof(ProgressValue))); } }
        public RangeObservableCollection<AppItem> StandardAppItems { get; private set; } = new();

        public bool AppLoaded => FeaturedAppItems.Any();

        public MoreApps(ContentDialog dialog)
        {
            InitializeComponent();
            Inst = this;
            DataContext = this;

            _dialog = dialog;
            
            _appTimer.Tick += (sender, args) =>
            {
                ProgressValue++;
                if (ProgressValue > 100) ProgressValue = 0;

                if (ProgressValue == 0)
                    RefreshAppItems();
            };
        }

        public void TryLoad()
        {
            IProgress<bool> progress = new Progress<bool>(_ =>
            {
                if (InAppPromotion.Inst.FeaturedAppItems.Count > 0)
                {
                    FeaturedAppItems.ReplaceRange(InAppPromotion.Inst.FeaturedAppItems.Take(2));
                    PropertyChanged?.Invoke(this, new(nameof(AppLoaded)));
                }

                if (InAppPromotion.Inst.StandardAppItems.Count > 0)
                {
                    if (StandardAppItems.Count == 0)
                        RefreshAppItems();

                    _appTimer.Start();
                }
            });

            InAppPromotion.Inst.LoadAppItemsAsync(() => { progress.Report(true); });
        }

        private void RefreshAppItems()
        {
            StandardAppItems.ReplaceRange(
                InAppPromotion.Inst.StandardAppItems.Skip(_appItemOffset).Take(4));

            _appItemOffset += 4;
            if (_appItemOffset >= InAppPromotion.Inst.StandardAppItems.Count) _appItemOffset = 0;
        }

        private void AppItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((sender as FrameworkElement).DataContext is not AppItem item) return;
            _ = Launcher.LaunchUriAsync(new(item.StoreUrl));
        }

        private void AllApps_Click(object sender, RoutedEventArgs e)
        {
            _ = Launcher.LaunchUriAsync(new(Meta.URL_APP_STORE_PUBLISHER));
        }

        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            (sender as Grid).Opacity = 0.8;
        }

        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            (sender as Grid).Opacity = 1.0;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _appTimer.Stop();
            _dialog.Hide();
        }
    }
}
