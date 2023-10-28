using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using Windows.System;
using Microsoft.UI.Xaml.Media.Imaging;
using System.ComponentModel;
using IOCore.Libs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IOCore.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class PromotionApp : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new(propertyName));
        
        private static readonly Type _type = typeof(PromotionApp);

        public PromotionApp()
        {
            InitializeComponent();
        }

        public AppItem AppItem { get => (AppItem)GetValue(_appItemProperty); set => SetValue(_appItemProperty, value); }
        public static readonly DependencyProperty _appItemProperty = DependencyProperty.Register(nameof(AppItem), typeof(AppItem), _type, new(null));

        public string AppName => AppItem.Name;
        public string AppDescription => AppItem.Description;

        public BitmapImage AppIcon => !string.IsNullOrWhiteSpace(AppItem.Icon) ? new(new(AppItem.Icon)) : null;
        public Visibility AppIconVisibility => string.IsNullOrWhiteSpace(AppItem.Icon) ? Visibility.Visible : Visibility.Collapsed;
    
        public bool AppIsOnSale => AppItem.IsOnSale;
        public string AppPrice => AppItem.Price;
        public string AppSalePrice => AppItem.SalePrice;

        public new Visibility Visibility { get => (Visibility)GetValue(_visibilityProperty); set => SetValue(_visibilityProperty, value); }
        private static readonly DependencyProperty _visibilityProperty = DependencyProperty.Register(nameof(Visibility), typeof(Visibility), _type, new(Visibility.Visible,
            (DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as UIElement).Visibility = (Visibility)e.NewValue));

        //

        private void Item_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            (sender as FrameworkElement).Opacity = 0.8;
        }

        private void Item_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            (sender as FrameworkElement).Opacity = 1.0;
        }

        private void Item_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((sender as FrameworkElement).DataContext is not AppItem item) return;
            _ = Launcher.LaunchUriAsync(new(item.StoreUrl));
        }
    }
}
