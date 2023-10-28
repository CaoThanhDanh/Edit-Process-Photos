using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;

namespace IOCore.Controls
{
    public class ButtonTypes
    {
        private static readonly double DESIGNED_ICON_SIZE = 16.0;
        private static readonly double DESIGNED_TEXT_SIZE = 14.0;
        private static readonly double DESIGNED_SPACING = 10.0;
        private static readonly double DESIGNED_CORNER_RADIUS = 4.0;
        
        public enum SizeOption
        {
            XS = 16,
            SM = 24,
            MD = 32,
            LG = 36,
            XL = 40,
            XXL = 48,
            Designed = MD
        }

        public enum CornerOption
        {
            Regular,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
            Left,
            Top,
            Right,
            Bottom,
            MainDiagonal,
            MinorDiagonal,
            None,
            Default = Regular
        }

        public enum VariantOption
        {
            Regular,
            Text,
            Outline,
            Default = Regular                
        }

        public static double GetSize(double size, SizeOption sizeOption, double constantScale)
        {  
            return size / (double)SizeOption.Designed * (double)sizeOption * constantScale;
        }

        public static double GetIconSize(SizeOption sizeOption, double constantScale)
        {
            return GetSize(DESIGNED_ICON_SIZE, sizeOption, constantScale);
        }

        public static double GetTextSize(SizeOption sizeOption, double constantScale)
        {
            return GetSize(DESIGNED_TEXT_SIZE, sizeOption, constantScale);
        }

        public static double GetSpacing(SizeOption sizeOption, double constantScale)
        {
            return GetSize(DESIGNED_SPACING, sizeOption, constantScale);
        }

        public static CornerRadius GetCornerRadius(SizeOption sizeOption, CornerOption cornerOption, double constantScale)
        {
            var radius = Math.Clamp(GetSize(DESIGNED_CORNER_RADIUS, sizeOption, constantScale), 1.0, 4.0);

            if (cornerOption == CornerOption.Default || cornerOption == CornerOption.Regular) return new(radius);
            else if (cornerOption == CornerOption.TopLeft) return new(radius, 0, 0, 0);
            else if (cornerOption == CornerOption.TopRight) return new(0, radius, 0, 0);
            else if (cornerOption == CornerOption.BottomLeft) return new(0, 0, radius, 0);
            else if (cornerOption == CornerOption.BottomRight) return new(0, 0, 0, radius);
            else if (cornerOption == CornerOption.Left) return new(radius, 0, 0, radius);
            else if (cornerOption == CornerOption.Top) return new(radius, radius, 0, 0);
            else if (cornerOption == CornerOption.Right) return new(0, radius, radius, 0);
            else if (cornerOption == CornerOption.Bottom) return new(0, 0, radius, radius);
            else if (cornerOption == CornerOption.MainDiagonal) return new(radius, 0, radius, 0);
            else if (cornerOption == CornerOption.MinorDiagonal) return new(0, radius, 0, radius);
            else return new(0);
        }

        public static Thickness GetBorderThickness(VariantOption option)
        {
            return option == VariantOption.Text ? new(0) : new(1);
        }

        public static Brush GetBackground(VariantOption option, Brush defaultBackground)
        {
            if (option == VariantOption.Default || option == VariantOption.Regular) return defaultBackground;
            else if (option == VariantOption.Text) return new SolidColorBrush(Colors.Transparent);
            else return new SolidColorBrush(Colors.Transparent);
        }

    }
}
