using System;
using System.Collections.Generic;
using System.ComponentModel;
using ImageMagick;
using ImageMagick.Formats;

namespace IOApp.Configs
{
    internal class AppTypes
    {
        public static readonly Dictionary<string, string> DOCS = new()
        {
            { "INTERLACED", "https://www.iostream.vn/io/difference-between-interlaced-image-and-non-interlaced-image-kpysN" },
            { "PROGRESSIVE", "https://www.iostream.vn/io/difference-between-baseline-jpeg-and-progressive-jpeg-x1gVy5" },
            { "INTERLACED_PROGRESSIVE", "https://www.iostream.vn/io/interlaced-image-and-progressive-image-eyxtl1" },
            { "IMAGE_FIT_TYPE", "https://www.iostream.vn/io/image-fit-types-to-fit-an-image-to-the-container-rUZZZ1" }
        };

        public enum ResizeOption
        {
            SameAsInput,
            FixedWidth,
            FixedHeight,
            MaxWidth,
            MaxHeight,
            MinWidth,
            MinHeight,
            Percentage,
            Contain,
            Cover,
            Fill,
        }

        public static readonly Dictionary<ResizeOption, string> RESIZE_OPTIONS = new()
        {
            { ResizeOption.SameAsInput, "Types_SameAsInput" },
            { ResizeOption.FixedWidth, "Types_FixedWidth" },
            { ResizeOption.FixedHeight, "Types_FixedHeight" },
            { ResizeOption.MaxWidth, "Types_MaxWidth" },
            { ResizeOption.MaxHeight, "Types_MaxHeight" },
            { ResizeOption.MinWidth, "Types_MinWidth" },
            { ResizeOption.MinHeight, "Types_MinHeight" },
            { ResizeOption.Percentage, "Types_Percentage" },
            { ResizeOption.Contain, "Types_Contain" },
            { ResizeOption.Cover, "Types_Cover" },
            { ResizeOption.Fill, "Types_Fill" },
        };

        public enum IcoFitType
        {
            Contain,
            Cover,
            Fill
        }

        public static readonly Dictionary<IcoFitType, string> ICO_FIT_TYPES = new()
        {
            { IcoFitType.Contain, "Types_Contain" },
            { IcoFitType.Cover, "Types_Cover" },
            { IcoFitType.Fill, "Types_Fill" }
        };

        public enum CompressionType
        {
            None,
            Lossless,
            Lossy
        }

        public static readonly Dictionary<CompressionType, string> COMPRESSION_TYPES = new()
        {
            { CompressionType.None, "None" },
            { CompressionType.Lossless, "Types_Lossless" },
            { CompressionType.Lossy, "Types_Lossy" }
        };

		//

        public enum WhiteBalance
        {
            UseCamera,
            Auto,
            None
        }

        public static readonly Dictionary<WhiteBalance, string> WHITE_BALANCES = new()
        {
            { WhiteBalance.UseCamera, "Types_UseCamera" },
            { WhiteBalance.Auto, "Auto" },
            { WhiteBalance.None, "None" }
        };

        //

        public static readonly Dictionary<WebPImageHint, string> WEBP_IMAGE_HINTS = new()
        {
            { WebPImageHint.Default, "Default" },
            { WebPImageHint.Picture, "Types_Picture" },
            { WebPImageHint.Photo, "Types_Photo" },
            { WebPImageHint.Graph, "Types_Graph" }
        };

        public static readonly Dictionary<DitherMethod, string> DITHER_METHODS = new()
        {
            { DitherMethod.No, "None" },
            { DitherMethod.FloydSteinberg, "Types_FloydSteinberg" },
            { DitherMethod.Riemersma, "Types_Riemersma" }
        };

		public static readonly Dictionary<WebPPreprocessing, string> WEBP_PREPROCESSING_TYPES = new()
        {
            { WebPPreprocessing.None, "None" },
            { WebPPreprocessing.SegmentSmooth, "Types_SegmentSmooth" },
            { WebPPreprocessing.PseudoRandom, "Types_PseudoRandom" },
        };
	
		public static readonly Dictionary<WebPFilterType, string> WEBP_FILTER_TYPES = new()
        {
            { WebPFilterType.Simple, "Types_Simple" },
            { WebPFilterType.Strong, "Types_Strong" }
		};

        public class FrameIcon : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public static readonly int[] SIZES = { 16, 24, 32, 48, 64, 96, 128, 192, 256 };
            public static readonly int MAX_SIZE = 256;

            public int Size;

            private bool _isChecked = true;
            public bool IsChecked { get => _isChecked; set { _isChecked = value; PropertyChanged?.Invoke(this, new(nameof(IsChecked))); } }

            private bool _isEnabled = true;
            public bool IsEnabled { get => _isEnabled; set { _isEnabled = value; PropertyChanged?.Invoke(this, new(nameof(IsEnabled))); } }

            public string DimensionText => $"{Size}x{Size}";
        }

        public class Metadata : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public static readonly Tuple<string, string>[] METADATAS =
            {
                new("Info_MetadataExif", "Types_RemoveExifMetadata"),
                new("Info_MetadataXmp", "Types_RemoveXmpMetadata"),
                new("Info_MetadataIptc", "Types_RemoveIptcMetadata")
            };

            public string Text { get; set; }
            public string Tag { get; set; }

            private bool _isChecked = false;
            public bool IsChecked { get => _isChecked; set { _isChecked = value; PropertyChanged?.Invoke(this, new(nameof(IsChecked))); } }

            private bool _isEnabled = false;
            public bool IsEnabled { get => _isEnabled; set { _isEnabled = value; PropertyChanged?.Invoke(this, new(nameof(IsEnabled))); } }
        }
    }
}