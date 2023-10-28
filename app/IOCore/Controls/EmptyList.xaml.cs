using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using Windows.ApplicationModel.Resources;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IOCore.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class EmptyList : UserControl
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new(propertyName));

        protected static readonly ResourceLoader _resourceLoader = ResourceLoader.GetForViewIndependentUse();

        private static readonly Type _type = typeof(EmptyList);

        public EmptyList() : base()
        {
            InitializeComponent();
        }

        public string Icon { get => (string)GetValue(_iconProperty); set => SetValue(_iconProperty, value); }
        private static readonly DependencyProperty _iconProperty = DependencyProperty.Register(nameof(Icon), typeof(string), _type, new("\uEA37",
            (DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as dynamic).OnPropertyChanged(nameof(Icon))));

        public string Text { get => (string)GetValue(_textProperty); set => SetValue(_textProperty, value); }
        private static readonly DependencyProperty _textProperty = DependencyProperty.Register(nameof(Text), typeof(string), _type, new(_resourceLoader.GetString("NothingInList"),
            (DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as dynamic).OnPropertyChanged(nameof(Text))));

        public new Visibility Visibility { get => (Visibility)GetValue(_visibilityProperty); set => SetValue(_visibilityProperty, value); }
        private static readonly DependencyProperty _visibilityProperty = DependencyProperty.Register(nameof(Visibility), typeof(Visibility), _type, new(Visibility.Visible,
            (DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as UIElement).Visibility = (Visibility)e.NewValue));
    }
}