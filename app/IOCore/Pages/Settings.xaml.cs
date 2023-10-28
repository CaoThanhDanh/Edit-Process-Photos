using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using Windows.ApplicationModel.Resources;
using Microsoft.UI.Xaml;
using Windows.Globalization;
using Microsoft.Windows.AppLifecycle;
using IOCore.Libs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IOCore.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Page, INotifyPropertyChanged
    {
        public static Settings Inst { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private static readonly ResourceLoader _resourceLoader = ResourceLoader.GetForViewIndependentUse();

        private ContentDialog _dialog;

        private readonly ThemeManager.Theme _currentTheme;
        private readonly LanguageManager.Culture _currentLanguage;

        public Settings(ContentDialog dialog)
        {
            InitializeComponent();
            Inst = this;
            DataContext = this;

            _dialog = dialog;

            ThemeComboBox.SelectionChanged -= ThemeOrLanguageComboBox_SelectionChanged;
            LanguageComboBox.SelectionChanged -= ThemeOrLanguageComboBox_SelectionChanged;

            foreach (var i in Enum.GetValues(typeof(ThemeManager.Theme)))
                ThemeComboBox.Items.Add(new ComboBoxItem { Tag = i, Content = _resourceLoader.GetString(ThemeManager.THEMES[(ThemeManager.Theme)i].Text) });

            _currentTheme = ThemeManager.LoadThemeSettingAutoFallback();
            ThemeComboBox.SelectedIndex = (int)_currentTheme;

            foreach (var i in Enum.GetValues(typeof(LanguageManager.Culture)))
                LanguageComboBox.Items.Add(new ComboBoxItem { Tag = i, Content = _resourceLoader.GetString(LanguageManager.LANGUAGES[(LanguageManager.Culture)i].Text) });

            _currentLanguage = LanguageManager.LoadCultureSettingAutoFallback();
            LanguageComboBox.SelectedIndex = (int)_currentLanguage;

            ThemeComboBox.SelectionChanged += ThemeOrLanguageComboBox_SelectionChanged;
            LanguageComboBox.SelectionChanged += ThemeOrLanguageComboBox_SelectionChanged;
        }

        private void ThemeOrLanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTheme = (ThemeComboBox.SelectedItem as FrameworkElement).Tag as ThemeManager.Theme?;
            var selectedLanguage = (LanguageComboBox.SelectedItem as FrameworkElement).Tag as LanguageManager.Culture?;

            ApplyButton.IsEnabled = CancelButton.IsEnabled = _currentTheme != selectedTheme || _currentLanguage != selectedLanguage;
        }

        private void ApplyThemeOrLanguageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Control control) return;

            if (control == ApplyButton)
            {
                var selectedTheme = (ThemeComboBox.SelectedItem as FrameworkElement).Tag as ThemeManager.Theme?;
                ThemeManager.SaveThemeSettingAutoFallback(selectedTheme);

                var selectedLanguage = (LanguageComboBox.SelectedItem as FrameworkElement).Tag as LanguageManager.Culture?;
                var language = LanguageManager.SaveCultureSettingAutoFallback(selectedLanguage);
                ApplicationLanguages.PrimaryLanguageOverride = LanguageManager.LANGUAGES[language].Value as string;

                AppInstance.Restart(string.Empty);
            }
            else if (control == CancelButton)
            {
                ThemeComboBox.SelectedIndex = (int)_currentTheme;
                LanguageComboBox.SelectedIndex = (int)_currentLanguage;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _dialog.Hide();
        }
    }
}