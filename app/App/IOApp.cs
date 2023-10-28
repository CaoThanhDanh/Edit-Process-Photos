using Microsoft.UI.Xaml;
using IOCore;
using IOCore.Libs;
using ImageMagick;

namespace IOApp
{
    internal class IOApp
    {
        internal static void Init(Application app)
        {
            Meta.APP_SLUG = "universal-media-player";
            Meta.IO_APP_ID = "64cc790a7b745464ef9e96e5";
            Meta.MS_STORE_ID = "9PJ0RRWR8DNR";

            Meta.APP_INIT_SIZE = new(1200, 740);

            AskForRate.TimeTest = 2 * 24 * 3600;

            LanguageManager.LANGUAGES[LanguageManager.Culture.EnUs].IsOn = true;
            LanguageManager.LANGUAGES[LanguageManager.Culture.JaJp].IsOn = true;
            LanguageManager.LANGUAGES[LanguageManager.Culture.ViVn].IsOn = true;

            ThemeManager.THEMES[ThemeManager.Theme.Dark].IsOn = true;
            ThemeManager.THEMES[ThemeManager.Theme.Light].IsOn = true;

            ThemeManager.Init(app);
            LanguageManager.Init();

#if DEBUG
            StoreManager.Force = StoreManager.License.Normal;
#endif
        }

        internal static void InitExt()
        {
            ResourceLimits.LimitMemory(new Percentage(90));
        } 
    }
}