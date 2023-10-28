using System;
using System.Linq;
using System.Globalization;
using Windows.Globalization;
using System.Collections.Generic;
using IOCore.Types;

namespace IOCore.Libs
{
    public class LanguageManager
    {
        private static readonly string SCOPE = nameof(LanguageManager);

        public enum Culture
        {
            UsingSystemSetting,
            EnUs,
            JaJp,
            ViVn
        }

        public static readonly Dictionary<Culture, SwitchRecord> LANGUAGES = new()
        {
            { Culture.UsingSystemSetting,   new(string.Empty,   "Settings_LanguageUsingSystemSetting",    true) },
            { Culture.EnUs,                 new("en-US",        "Settings_LanguageEnUs",                  true) },
            { Culture.JaJp,                 new("ja-JP",        "Settings_LanguageJaJp",                  false) },
            { Culture.ViVn,                 new("vi-VN",        "Settings_LanguageViVn",                  false) },
        };

        public static void Init()
        {
            var language = LoadCultureSettingAutoFallback();
            ApplicationLanguages.PrimaryLanguageOverride = LANGUAGES[language].Value as string;
        }

        public static Culture VerifyCultureAutoFallback(Culture? culture)
        {
            if (culture == null || culture == Culture.UsingSystemSetting)
            {
                var systemLanguageMap = LANGUAGES.FirstOrDefault(i => (i.Value.Value as string).Equals(CultureInfo.CurrentCulture.Name, StringComparison.InvariantCultureIgnoreCase));

                if (systemLanguageMap.Value == null)
                    systemLanguageMap = LANGUAGES.FirstOrDefault(i => !string.IsNullOrWhiteSpace(i.Value.Value as string) && (i.Value.Value as string)[..2].Equals(CultureInfo.CurrentCulture.Name[..2], StringComparison.CurrentCultureIgnoreCase));

                if (systemLanguageMap.Value == null)
                    culture = Culture.EnUs;
                else
                    culture = systemLanguageMap.Key;
            }

            return culture.Value;
        }

        public static Culture VerifyCultureAutoFallback(string cultureText)
        {
            if (!Enum.TryParse(cultureText, out Culture culture))
                culture = Culture.UsingSystemSetting;
            return VerifyCultureAutoFallback(culture);
        }

        public static Culture LoadCultureSettingAutoFallback()
        {
            var culture = LocalStorage.GetValueOrDefault(SCOPE, Culture.UsingSystemSetting);
            LocalStorage.Set(SCOPE, culture);
            return culture;
        }

        public static Culture SaveCultureSettingAutoFallback(string cultureText)
        {
            var culture = VerifyCultureAutoFallback(cultureText);
            LocalStorage.Set(SCOPE, culture);
            return culture;
        }

        public static Culture SaveCultureSettingAutoFallback(Culture culture)
        {
            culture = VerifyCultureAutoFallback(culture);
            LocalStorage.Set(SCOPE, culture);
            return culture;
        }

        public static Culture SaveCultureSettingAutoFallback(Culture? culture)
        {
            culture = VerifyCultureAutoFallback(culture);
            LocalStorage.Set(SCOPE, culture);
            return culture.Value;
        }
    }
}