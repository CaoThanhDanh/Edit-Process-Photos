using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using System;
using System.IO;

namespace IOCore.Libs
{
    public class DataGridRowToIndexConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value is not DataGridRow v) return -1;
            return v.GetIndex();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }

    public class EnumComparisonToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value == null) return false;
            if (parameter is not string p) return false;

            return Array.IndexOf(p.Split('|'), Enum.GetName(value.GetType(), value)) >= 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }

    public class EnumComparisonToBoolInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value == null) return false;
            if (parameter is not string p) return false;

            return Array.IndexOf(p.Split('|'), Enum.GetName(value.GetType(), value)) < 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }

    public class EnumComparisonToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value == null) return Visibility.Collapsed;
            if (parameter is not string p) return Visibility.Collapsed;

            return Array.IndexOf(p.Split('|'), Enum.GetName(value.GetType(), value)) >= 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }

    public class EnumComparisonToVisibleInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value == null) return Visibility.Visible;
            if (parameter is not string p) return Visibility.Visible;

            return Array.IndexOf(p.Split('|'), Enum.GetName(value.GetType(), value)) >= 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }

    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            return Enum.GetName(value.GetType(), value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }

    public class FileSizeToSizeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value is not long v) return string.Empty;
            return Utils.GetReadableByteSizeText(v);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }

    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value is not ListViewItemPresenter v) return 0;
            if (parameter is not int p) p = 0;

            var item = VisualTreeHelper.GetParent(v) as ListViewItem;
            return ItemsControl.ItemsControlFromItemContainer(item).IndexFromContainer(item) + p;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }

    public class IndexMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value is not int v) return 0;
            if (parameter is not string p) return v;
            return v + Utils.GetValueOrDefault(p, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value == null) return null;
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value is not bool v) return Visibility.Collapsed;
            return v ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }

    public class IsEqualIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value is not int v || parameter is not int p) return false;
            return v == p;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }

    public class IsNotEqualIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value is not int v || parameter is not int p) return false;
            return v != p;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }

    public class IsNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            var isNull = value == null;

            if (parameter is string p && p == "invert")
                isNull = !isNull;

            return isNull;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }

    public class IsNullOrEmptyStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value is not string v) return false;

            var isNullOrEmptyString = string.IsNullOrEmpty(v);

            if (parameter is string p && p == "invert")
                isNullOrEmptyString = !isNullOrEmptyString;

            return isNullOrEmptyString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }

    public class IsNullOrEmptyArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            var invert = parameter is string p && p == "invert";

            var isNullOrEmpty = false;

            if (value is not Array v)
                isNullOrEmpty = true;
            else if (v.Length == 0)
                isNullOrEmpty = true;

            if (invert)
                isNullOrEmpty = !isNullOrEmpty;

            return isNullOrEmpty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }

    public class ObjectToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value is not string v) return string.Empty;
            if (parameter is not string p) return v;
            return TextHelper.Transform(v, p);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PosfixTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            var text = string.Empty;
            if (value is string v) text += v;
            if (parameter is string p) text += p;

            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }

    public class PremiumHelpConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value is not bool v) v = false;
            if (parameter is not string p) p = string.Empty;
            return p == "symbol" ?
                v ? Symbol.Help : Symbol.SolidStar :
                v ? null : "Premium";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TimeSpanToTimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value is not TimeSpan v) return "--:--:--";
            return $"{v.Hours:D2}:{v.Minutes:D2}:{v.Seconds:D2}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }

    public class TrimStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value is not string v) return string.Empty;
            if (parameter is not string p) return v;

            var length = Utils.GetValueOrDefault(p, 0);
            return v.Length > length ? $"{v[..length]}..." : v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }

    public class TrimWordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value is not string v) return string.Empty;
            if (parameter is not string p) return v;

            return Utils.TruncateAtWord(v, Utils.GetValueOrDefault(p, v.Length));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }

    public class UseOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value is not bool v) v = true;
            if (parameter is not string p) p = "1.0";
            return v ? Utils.GetValueOrDefault(p, 1.0) : 1.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }

    public class FilePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value is not string v || parameter is not string p) return string.Empty;

            if (p == "extension")
                return Path.GetExtension(v);
            else if (p == "fileNameWithoutExtension")
                return Path.GetFileNameWithoutExtension(v);
            else if (p == "fileName")
            {
                string ret = Path.GetFileName(v);

                if (string.IsNullOrEmpty(ret))
                    ret = v;

                return ret;
            }
            else if (p == "directoryName")
                return Path.GetDirectoryName(v);

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }
    public class TicksToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture) => new TimeSpan((long)value).ToString(@"hh\:mm\:ss");
        public object ConvertBack(object value, Type targetType, object parameter, string culture) => throw new NotImplementedException();
    }

    public class TicksToSecondsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture) => (long)value / 10000000.0;
        public object ConvertBack(object value, Type targetType, object parameter, string culture) => (long)((double)value * 10000000);
    }
}