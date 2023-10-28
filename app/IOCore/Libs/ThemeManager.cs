using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using IOCore.Types;

namespace IOCore.Libs
{
    public class ThemeManager
    {
        private readonly static string SCOPE = nameof(ThemeManager);

        public enum Theme
        {
            UsingSystemSetting,
            Dark,
            Light
        }

        public static readonly Dictionary<Theme, SwitchRecord> THEMES = new()
        {
            { Theme.UsingSystemSetting, new(Theme.UsingSystemSetting,   "Settings_ThemeUsingSystemSetting", true) },
            { Theme.Dark,               new(Theme.Dark,                 "Settings_ThemeDark",               true) },
            { Theme.Light,              new(Theme.Light,                "Settings_ThemeLight",              true) },
        };


        public static void Init(Application app)
        {
            var theme = LocalStorage.GetValueOrDefault(SCOPE, Theme.Dark);
            LocalStorage.Set(SCOPE, theme);

            app.RequestedTheme = theme == Theme.UsingSystemSetting ? app.RequestedTheme :
                theme == Theme.Dark ? ApplicationTheme.Dark : ApplicationTheme.Light;
        }

        public static bool IsDarkMode()
        {
            return Application.Current.RequestedTheme == ApplicationTheme.Dark;
        }

        public static Theme VerifyThemeAutoFallback(Theme? theme)
        {
            return theme == null ? Theme.UsingSystemSetting : theme.Value;
        }

        public static Theme VerifyThemeAutoFallback(string themeText)
        {
            if (themeText == null || !Enum.TryParse(themeText, out Theme theme))
                theme = Theme.UsingSystemSetting;
            return VerifyThemeAutoFallback(theme);
        }

        public static Theme LoadThemeSettingAutoFallback()
        {
            var theme = LocalStorage.GetValueOrDefault(SCOPE, Theme.UsingSystemSetting);
            LocalStorage.Set(SCOPE, theme);
            return theme;
        }

        public static Theme SaveThemeSettingAutoFallback(string themeText)
        {
            var theme = VerifyThemeAutoFallback(themeText);
            LocalStorage.Set(SCOPE, theme);
            return theme;
        }

        public static Theme SaveThemeSettingAutoFallback(Theme theme)
        {
            theme = VerifyThemeAutoFallback(theme);
            LocalStorage.Set(SCOPE, theme);
            return theme;
        }

        public static Theme SaveThemeSettingAutoFallback(Theme? theme)
        {
            theme = VerifyThemeAutoFallback(theme);
            LocalStorage.Set(SCOPE, theme);
            return theme.Value;
        }
    }
}