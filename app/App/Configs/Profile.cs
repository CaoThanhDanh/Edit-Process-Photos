using ImageMagick;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using IOCore.Libs;
using IOApp.Features;

namespace IOApp.Configs
{
    internal class Profile
    {
        public enum InputFormatFamily
        {
            Webp,
            Bmp,
            Jpg,
            Png,
        }

        public static readonly Dictionary<InputFormatFamily, ImageFormat> INPUT_IMAGE_FORMATS = new()
        {
            { InputFormatFamily.Webp, new(true, string.Empty, false, new[] { ".webp" },                 new[] { MagickFormat.WebP }) },
            { InputFormatFamily.Bmp,  new(true, string.Empty, false, new[] { ".bmp", ".rle", ".dib" },  new[] { MagickFormat.Bmp, MagickFormat.Rle, MagickFormat.Dib }) },
            { InputFormatFamily.Jpg,  new(true, string.Empty, false, new[] { ".jpg", ".jpeg", ".jpe" }, new[] { MagickFormat.Jpg, MagickFormat.Jpeg, MagickFormat.Jpe, MagickFormat.Mat }) },
            { InputFormatFamily.Png,  new(true, string.Empty, false, new[] { ".png" },                  new[] { MagickFormat.Png }) },
        };

        //

        public enum OutputFormatFamily
        {
            Png,
            Png8,
            Jpg,
            Bmp,
            Webp,
        }

        public static readonly Dictionary<OutputFormatFamily, ImageFormat> OUTPUT_IMAGE_FORMATS = new()
        {
            { OutputFormatFamily.Png,  new(true,  "PNG",      false, new[] { ".png" },  new[] { MagickFormat.Png }) },
            { OutputFormatFamily.Png8, new(true,  "PNG-8",    false, new[] { ".png" },  new[] { MagickFormat.Png8 }) },
            { OutputFormatFamily.Jpg,  new(true,  "JPG/JPEG", false, new[] { ".jpg" },  new[] { MagickFormat.Jpg }) },
            { OutputFormatFamily.Bmp,  new(true,  "BMP",      false, new[] { ".bmp" },  new[] { MagickFormat.Bmp }) },
            { OutputFormatFamily.Webp, new(true,  "WEBP",     false, new[] { ".webp" }, new[] { MagickFormat.WebP }) },
        };

        //

        public static readonly string[] INPUT_EXTENSIONS;
        public static readonly MagickFormat[] INPUT_FORMATS;

        public static readonly string[] OUTPUT_EXTENSIONS;
        public static readonly MagickFormat[] OUTPUT_FORMATS;

        static Profile()
        {
            INPUT_EXTENSIONS = INPUT_IMAGE_FORMATS.Where(i => i.Value.IsSupported).Select(i => i.Value.Extensions).SelectMany(i => i).Distinct().ToArray();
            INPUT_FORMATS = INPUT_IMAGE_FORMATS.Where(i => i.Value.IsSupported).Select(i => i.Value.Formats).SelectMany(i => i).Distinct().ToArray();

            OUTPUT_EXTENSIONS = OUTPUT_IMAGE_FORMATS.Where(i => i.Value.IsSupported).Select(i => i.Value.Extensions).SelectMany(i => i).Distinct().ToArray();
            OUTPUT_FORMATS = OUTPUT_IMAGE_FORMATS.Where(i => i.Value.IsSupported).Select(i => i.Value.Formats).SelectMany(i => i).Distinct().ToArray();
        }

        //

        public static string GetInputExtensionsTextByGroupFamily()
        {
            const int LENGTH = 20;

            var extensions = INPUT_EXTENSIONS.ToArray();

            List<string> extTexts = new();
            while (extensions.Length > 0)
            {
                extTexts.Add(string.Join(", ", extensions.Skip(0).Take(LENGTH).ToArray()));
                extensions = extensions.Skip(LENGTH).ToArray();
            }

            return string.Join("\n", extTexts);
        }

        public static Size GetCacheImageLimitSize(int width, int height)
        {
            return width > height ? new Size(1536, 1536) : new Size(1536, 1536);
        }

        public static bool IsAcceptedInputFormat(string filePath)
        {
            try
            {
                var imageMeta = ImageMagickUtils.GetMagickImageMeta(filePath);
                return IsAcceptedInputFormat(imageMeta.Format);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsAcceptedInputFormat(MagickFormat? format)
        {
            if (format == null) return false;
            return INPUT_FORMATS.Contains(format.Value);
        }

        public static InputFormatFamily? GetInputFormatFamilyKeyFromFormat(MagickFormat magickFormat)
        {
            foreach (var i in INPUT_IMAGE_FORMATS)
                if (i.Value.Formats.Contains(magickFormat))
                    return i.Key;

            return null;
        }

        public static ImageFormat GetInputFormatFamilyValueFromFormat(MagickFormat magickFormat)
        {
            foreach (var i in INPUT_IMAGE_FORMATS)
                if (i.Value.Formats.Contains(magickFormat))
                    return i.Value;

            return null;
        }

        public static bool IsRawInputFormat(MagickFormat format)
        {
            return GetInputFormatFamilyValueFromFormat(format).IsRaw;
        }

        public static OutputFormatFamily? GetOutputFormatFamilyKeyFromFormat(MagickFormat magickFormat)
        {
            foreach (var i in OUTPUT_IMAGE_FORMATS)
                if (i.Value.Formats.Contains(magickFormat))
                    return i.Key;

            return null;
        }

        public static ImageFormat GetOutputFormatFamilyValueFromFormat(MagickFormat magickFormat)
        {
            foreach (var i in OUTPUT_IMAGE_FORMATS)
                if (i.Value.Formats.Contains(magickFormat))
                    return i.Value;

            return null;
        }
    }
}