using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using Windows.ApplicationModel.Resources;
using System;
using System.Text;
using System.Web;

namespace IOCore.Libs
{
    public class TextHelper
    {
        public enum Case
        {
            Normal,
            SentenceCase,
            LowerCase,
            UpperCase,
            Capitalize,
            ToggleCase
        }

        public static string Transform(string text, Case useCase)
        {
            if (useCase == Case.LowerCase)
                return text.ToLowerInvariant();
            else if (useCase == Case.UpperCase)
                return text.ToUpperInvariant();

            return text;
        }

        public static string Transform(string text, string useCaseText)
        {
            if (Enum.TryParse(useCaseText, out Case useCase))
                return Transform(text, useCase);
            return text;
        }
    }

    internal static class TextBlockHelper
    {
        public static TextHelper.Case GetUseCase(DependencyObject obj)
        {
            return (TextHelper.Case)obj.GetValue(UseCaseProperty);
        }

        public static void SetUseCase(DependencyObject obj, TextHelper.Case value)
        {
            obj.SetValue(UseCaseProperty, value);
        }

        public static readonly DependencyProperty UseCaseProperty =
            DependencyProperty.RegisterAttached("UseCase", typeof(TextHelper.Case), typeof(TextBlock), new(TextHelper.Case.Normal, (sender, args) =>
            {
                var textBlock = sender as TextBlock;
                textBlock.RegisterPropertyChangedCallback(TextBlock.TextProperty, (obj, e) =>
                {
                    var useCase = (TextHelper.Case)obj.GetValue(UseCaseProperty);
                    textBlock.Text = TextHelper.Transform(textBlock.Text, useCase);
                });
            }
        ));
    }

    // https://stackoverflow.com/questions/69884508/localize-strings-in-xaml-ui-in-uwp
    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    public class SRE : MarkupExtension // String Resource Extension
    {
        private static readonly ResourceLoader _resourceLoader = ResourceLoader.GetForViewIndependentUse();

        public string K { get; set; } = string.Empty;

        public TextHelper.Case UseCase { get; set; } = TextHelper.Case.Normal;
        public string Prefix { get; set; } = string.Empty;
        public string Postfix { get; set; } = string.Empty;
        
        public SRE()
        {
        }

        protected override object ProvideValue()
        {
            StringBuilder sb = new();

            Prefix = HttpUtility.UrlDecode(Prefix);
            if (!string.IsNullOrWhiteSpace(Prefix))
                sb.Append(Prefix);

            sb.Append(TextHelper.Transform(_resourceLoader.GetString(K), UseCase));

            Postfix = HttpUtility.UrlDecode(Postfix);
            if (!string.IsNullOrWhiteSpace(Postfix))
                sb.Append(Postfix);

            return sb.ToString();
        }
    }
}
