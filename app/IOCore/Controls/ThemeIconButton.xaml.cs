using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.ComponentModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IOCore.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class ThemeIconButton : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new(propertyName));

        private static readonly Type _type = typeof(IconButton);

        public ThemeIconButton()
        {
            InitializeComponent();
        }

        public string Icon { get => (string)GetValue(_iconProperty); set => SetValue(_iconProperty, value); }
        private static readonly DependencyProperty _iconProperty = DependencyProperty.Register(nameof(Icon), typeof(string), _type, new(null, (DependencyObject d,
            DependencyPropertyChangedEventArgs e) => (d as dynamic).OnPropertyChanged(nameof(Icon))));

        public ButtonTypes.SizeOption Size { get => (ButtonTypes.SizeOption)GetValue(_sizeProperty); set => SetValue(_sizeProperty, value); }
        private static readonly DependencyProperty _sizeProperty = DependencyProperty.Register(nameof(Size), typeof(ButtonTypes.SizeOption), _type, new(ButtonTypes.SizeOption.Designed));

        public double ConstantScale { get => (double)GetValue(_constantScaleProperty); set => SetValue(_constantScaleProperty, value); }
        private static readonly DependencyProperty _constantScaleProperty = DependencyProperty.Register(nameof(ConstantScale), typeof(double), _type, new(1.0));

        public ButtonTypes.CornerOption Corner { get => (ButtonTypes.CornerOption)GetValue(_cornerProperty); set => SetValue(_cornerProperty, value); }
        private static readonly DependencyProperty _cornerProperty = DependencyProperty.Register(nameof(Corner), typeof(ButtonTypes.CornerOption), _type, new(ButtonTypes.CornerOption.Default));

        public FlyoutBase Flyout { get => (FlyoutBase)GetValue(_flyoutProperty); set => SetValue(_flyoutProperty, value); }
        private static readonly DependencyProperty _flyoutProperty = DependencyProperty.Register(nameof(Flyout), typeof(FlyoutBase), _type, new(default));

        public bool IsSquare { get => (bool)GetValue(_isSquareProperty); set => SetValue(_isSquareProperty, value); }
        private static readonly DependencyProperty _isSquareProperty = DependencyProperty.Register(nameof(IsSquare), typeof(bool), _type, new(false));

        public new Visibility Visibility { get => (Visibility)GetValue(_visibilityProperty); set => SetValue(_visibilityProperty, value); }
        private static readonly DependencyProperty _visibilityProperty = DependencyProperty.Register(nameof(Visibility), typeof(Visibility), _type, new(Visibility.Visible,
            (DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as UIElement).Visibility = (Visibility)e.NewValue));

        //

        public double ButtonWidth => ButtonTypes.GetSize(IsSquare ? (double)ButtonTypes.SizeOption.Designed : 48.0, Size, ConstantScale);
        public double ButtonHeight => ButtonTypes.GetSize((double)ButtonTypes.SizeOption.Designed, Size, ConstantScale);

        public double IconSize => ButtonTypes.GetIconSize(Size, ConstantScale);

        public CornerRadius ButtonCornerRadius => ButtonTypes.GetCornerRadius(Size, Corner, ConstantScale);

        //

        public event RoutedEventHandler Click;

        private void XButton_Click(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, e);
        }
    }
}
