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
            Avif,
            Eps,
            Heic,
            Psd,
            Qoi,
            Svg,
            Tga,
            Tiff,
            Webp,
            Xpm,

            Bmp,
            Gif,
            Ico,
            Jpg,
            Png,

            Pbm,
            Pcx,
            Wbmp,

            //

            Arw,
            Cr2,
            Dcr,
            Dng,
            Erf,
            Mef,
            Nef,
            Orf,
            Pef,
            Raf,
            Raw,
            Rw2,
        }

        public static readonly Dictionary<InputFormatFamily, ImageFormat> INPUT_IMAGE_FORMATS = new()
        {
            //{ InputFormatFamily.Avif, new(true, string.Empty, false, new[] { ".avif" },                 new[] { MagickFormat.Avif }) },
            //{ InputFormatFamily.Eps,  new(true, string.Empty, false, new[] { ".eps" },                  new[] { MagickFormat.Epi, MagickFormat.Eps, MagickFormat.Eps2, MagickFormat.Eps3, MagickFormat.Epsf, MagickFormat.Epsi, MagickFormat.Ept, MagickFormat.Ept2, MagickFormat.Ept3 }) },
            //{ InputFormatFamily.Heic, new(true, string.Empty, false, new[] { ".heic", ".heif" },        new[] { MagickFormat.Heic, MagickFormat.Heif }) },
            //{ InputFormatFamily.Psd,  new(true, string.Empty, false, new[] { ".psb", ".psd" },          new[] { MagickFormat.Psb, MagickFormat.Psd }) },
            //{ InputFormatFamily.Qoi,  new(true, string.Empty, false, new[] { ".qoi" },                  new[] { MagickFormat.Qoi }) },
            //{ InputFormatFamily.Svg,  new(true, string.Empty, false, new[] { ".svg", ".svgz" },         new[] { MagickFormat.Svg, MagickFormat.Svgz }) },
            //{ InputFormatFamily.Tga,  new(true, string.Empty, false, new[] { ".icb", ".tga", ".vda", ".vst" }, new[] { MagickFormat.Icb, MagickFormat.Tga, MagickFormat.Vda, MagickFormat.Vst }) },
            //{ InputFormatFamily.Tiff, new(true, string.Empty, false, new[] { ".tif", ".tiff" },         new[] { MagickFormat.Tif, MagickFormat.Tiff, MagickFormat.Tiff64, MagickFormat.Ptif }) },
            { InputFormatFamily.Webp, new(true, string.Empty, false, new[] { ".webp" },                 new[] { MagickFormat.WebP }) },
            //{ InputFormatFamily.Xpm,  new(true, string.Empty, false, new[] { ".xbm", ".xpm" },          new[] { MagickFormat.Xbm, MagickFormat.Xpm }) },

            { InputFormatFamily.Bmp,  new(true, string.Empty, false, new[] { ".bmp", ".rle", ".dib" },  new[] { MagickFormat.Bmp, MagickFormat.Rle, MagickFormat.Dib }) },
            //{ InputFormatFamily.Gif,  new(true, string.Empty, false, new[] { ".gif" },                  new[] { MagickFormat.Gif, MagickFormat.Gif87 }) },
            //{ InputFormatFamily.Ico,  new(true, string.Empty, false, new[] { ".ico" },                  new[] { MagickFormat.Ico }) },
            { InputFormatFamily.Jpg,  new(true, string.Empty, false, new[] { ".jpg", ".jpeg", ".jpe" }, new[] { MagickFormat.Jpg, MagickFormat.Jpeg, MagickFormat.Jpe, MagickFormat.Mat }) },
            { InputFormatFamily.Png,  new(true, string.Empty, false, new[] { ".png" },                  new[] { MagickFormat.Png }) },

            //{ InputFormatFamily.Pbm,  new(true, string.Empty, false, new[] { ".pbm", ".pgm", ".ppm", ".pnm", ".pfm", ".pam" }, new[] { MagickFormat.Pbm, MagickFormat.Pgm, MagickFormat.Ppm, MagickFormat.Pnm, MagickFormat.Pfm, MagickFormat.Pam }) },
            //{ InputFormatFamily.Pcx,  new(true, string.Empty, false, new[] { ".pcx" },                  new[] { MagickFormat.Pcx }) },
            //{ InputFormatFamily.Wbmp, new(true, string.Empty, false, new[] { ".wbm", ".wbmp" },         new[] { MagickFormat.Wbmp }) },

			//

            //{ InputFormatFamily.Arw, new(true, string.Empty, true, new[] { ".arw", ".srf", ".sr2" },    new[] { MagickFormat.Arw, MagickFormat.Srf, MagickFormat.Sr2 }) },
            //{ InputFormatFamily.Cr2, new(true, string.Empty, true, new[] { ".cr2", ".cr3", ".crw" },    new[] { MagickFormat.Cr2, MagickFormat.Cr3, MagickFormat.Crw }) },
            //{ InputFormatFamily.Dcr, new(true, string.Empty, true, new[] { ".dcr", ".kdc", ".k25" },    new[] { MagickFormat.Dcr, MagickFormat.Kdc, MagickFormat.K25 }) },
            //{ InputFormatFamily.Dng, new(true, string.Empty, true, new[] { ".dng" },                    new[] { MagickFormat.Dng }) },
            //{ InputFormatFamily.Erf, new(true, string.Empty, true, new[] { ".erf" },                    new[] { MagickFormat.Erf }) },
            //{ InputFormatFamily.Mef, new(true, string.Empty, true, new[] { ".mef" },                    new[] { MagickFormat.Mef }) },
            //{ InputFormatFamily.Nef, new(true, string.Empty, true, new[] { ".nef", ".nrw" },            new[] { MagickFormat.Nef, MagickFormat.Nrw }) },
            //{ InputFormatFamily.Orf, new(true, string.Empty, true, new[] { ".orf" },                    new[] { MagickFormat.Orf }) },
            //{ InputFormatFamily.Pef, new(true, string.Empty, true, new[] { ".pef" },                    new[] { MagickFormat.Pef }) },
            //{ InputFormatFamily.Raf, new(true, string.Empty, true, new[] { ".raf" },                    new[] { MagickFormat.Raf }) },
            //{ InputFormatFamily.Raw, new(true, string.Empty, true, new[] { ".raw" },                    new[] { MagickFormat.Raw }) },
            //{ InputFormatFamily.Rw2, new(true, string.Empty, true, new[] { ".rw2" },                    new[] { MagickFormat.Rw2 }) },
        };

        //

        public enum OutputFormatFamily
        {
            Png,
            Png8,
            Jpg,
            Gif,
            Bmp,
            Ico,
            Webp,

            Pbm,
            Pcx,
            Wbmp,
        }

        public static readonly Dictionary<OutputFormatFamily, ImageFormat> OUTPUT_IMAGE_FORMATS = new()
        {
            { OutputFormatFamily.Png,  new(true,  "PNG",      false, new[] { ".png" },  new[] { MagickFormat.Png }) },
            { OutputFormatFamily.Png8, new(true,  "PNG-8",    false, new[] { ".png" },  new[] { MagickFormat.Png8 }) },
            { OutputFormatFamily.Jpg,  new(true,  "JPG/JPEG", false, new[] { ".jpg" },  new[] { MagickFormat.Jpg }) },
            { OutputFormatFamily.Gif,  new(true,  "GIF",      false, new[] { ".gif" },  new[] { MagickFormat.Gif }) },
            { OutputFormatFamily.Bmp,  new(true,  "BMP",      false, new[] { ".bmp" },  new[] { MagickFormat.Bmp }) },
            { OutputFormatFamily.Ico,  new(true,  "ICO",      false, new[] { ".ico" },  new[] { MagickFormat.Ico }) },
            { OutputFormatFamily.Webp, new(true,  "WEBP",     false, new[] { ".webp" }, new[] { MagickFormat.WebP }) },

            { OutputFormatFamily.Pbm,  new(true,  "PBM",      false, new[] { ".pbm" },  new[] { MagickFormat.Pbm }) },
            { OutputFormatFamily.Pcx,  new(true,  "PCX",      false, new[] { ".pcx" },  new[] { MagickFormat.Pcx }) },
            { OutputFormatFamily.Wbmp, new(true,  "WBMP",     false, new[] { ".wbmp" }, new[] { MagickFormat.Wbmp }) },
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