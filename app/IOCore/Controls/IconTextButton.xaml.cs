using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using System;
using System.ComponentModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IOCore.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IconTextButton : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new(propertyName));

        private static readonly Type _type = typeof(IconTextButton);
        
        private readonly Brush _defaultBackground;

        public IconTextButton()
        {
            InitializeComponent();
            _defaultBackground = XButton.Background;
        }

        public string Icon { get => (string)GetValue(_iconProperty); set => SetValue(_iconProperty, value); }
        private static readonly DependencyProperty _iconProperty = DependencyProperty.Register(nameof(Icon), typeof(string), _type, new(null, (DependencyObject d,
            DependencyPropertyChangedEventArgs e) => (d as dynamic).OnPropertyChanged(nameof(Icon))));

        public string Text { get => (string)GetValue(_textProperty); set => SetValue(_textProperty, value); }
        private static readonly DependencyProperty _textProperty = DependencyProperty.Register(nameof(Text), typeof(string), _type, new(string.Empty, (DependencyObject d,
            DependencyPropertyChangedEventArgs e) => (d as dynamic).OnPropertyChanged(nameof(Text))));

        public ButtonTypes.SizeOption Size { get => (ButtonTypes.SizeOption)GetValue(_sizeProperty); set => SetValue(_sizeProperty, value); }
        private static readonly DependencyProperty _sizeProperty = DependencyProperty.Register(nameof(Size), typeof(ButtonTypes.SizeOption), _type, new(ButtonTypes.SizeOption.Designed));

        public double ConstantScale { get => (double)GetValue(_constantScaleProperty); set => SetValue(_constantScaleProperty, value); }
        private static readonly DependencyProperty _constantScaleProperty = DependencyProperty.Register(nameof(ConstantScale), typeof(double), _type, new(1.0));

        public ButtonTypes.VariantOption Variant { get => (ButtonTypes.VariantOption)GetValue(_variantProperty); set => SetValue(_variantProperty, value); }
        private static readonly DependencyProperty _variantProperty = DependencyProperty.Register(nameof(Variant), typeof(bool), _type, new(ButtonTypes.VariantOption.Default));

        public ButtonTypes.CornerOption Corner { get => (ButtonTypes.CornerOption)GetValue(_cornerProperty); set => SetValue(_cornerProperty, value); }
        private static readonly DependencyProperty _cornerProperty = DependencyProperty.Register(nameof(Corner), typeof(ButtonTypes.CornerOption), _type, new(ButtonTypes.CornerOption.Default));

        public FlyoutBase Flyout { get => (FlyoutBase)GetValue(_flyoutProperty); set => SetValue(_flyoutProperty, value); }
        private static readonly DependencyProperty _flyoutProperty = DependencyProperty.Register(nameof(Flyout), typeof(FlyoutBase), _type, new(default));

        public new Visibility Visibility { get => (Visibility)GetValue(_visibilityProperty); set => SetValue(_visibilityProperty, value); }
        private static readonly DependencyProperty _visibilityProperty = DependencyProperty.Register(nameof(Visibility), typeof(Visibility), _type, new(Visibility.Visible,
            (DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as UIElement).Visibility = (Visibility)e.NewValue));

        //

        public double ButtonHeight => ButtonTypes.GetSize((double)ButtonTypes.SizeOption.Designed, Size, ConstantScale);

        public Visibility IconVisibility => string.IsNullOrWhiteSpace(Icon) ? Visibility.Collapsed : Visibility.Visible;
        public double IconBoxWidth => ButtonHeight / 4 * 3;
        public double IconSize => ButtonTypes.GetIconSize(Size, ConstantScale);

        public Thickness TextPadding => new(ButtonHeight / 3, 0, ButtonHeight / 3, 0);
        public double TextSize => ButtonTypes.GetTextSize(Size, ConstantScale);

        public Thickness ButtonBorderThickness => ButtonTypes.GetBorderThickness(Variant);
        public Brush ButtonBackground => ButtonTypes.GetBackground(Variant, _defaultBackground);

        public CornerRadius ButtonCornerRadius => ButtonTypes.GetCornerRadius(Size, Corner, ConstantScale);

        //

        public event RoutedEventHandler Click;

        private void XButton_Click(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, e);
        }
    }
}
