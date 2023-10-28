using ImageMagick;
using Microsoft.UI.Xaml.Media.Imaging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IOCore.Libs
{
    public class ImageMagickUtils
    {
        public class AppIcon
        {
            private static MagickImage _iconCache;

            public static BitmapImage Load(int width, int height, string fileName = "icon.svg", bool useCache = true)
            {
                const int CACHE_SIZE = 96;

                MagickImage image;

                var filePath = Path.Combine(Utils.GetAssetsFolderPath(), fileName);

                var size = GetResolution(filePath);
                if (size == null) return null;

                var density = Utils.ComputeNeededDensityForSvg(size.Value.Width, size.Value.Height, CACHE_SIZE, CACHE_SIZE);

                if (useCache)
                {
                    if (_iconCache == null)
                    {
                        _iconCache = new MagickImage(filePath, new MagickReadSettings() { BackgroundColor = MagickColors.Transparent, Density = new(density) });
                        _iconCache.Resize(CACHE_SIZE, CACHE_SIZE);
                    }

                    image = _iconCache.Clone() as MagickImage;

                    if (_iconCache.Width != width || _iconCache.Height != height)
                        image.Resize(width, height);
                }
                else
                {
                    image = new MagickImage(filePath, new MagickReadSettings() { BackgroundColor = MagickColors.Transparent, Density = new(density) });
                    image.Resize(width, height);
                }

                var memoryStream = new MemoryStream();
                var bitmapImage = new BitmapImage();

                image.Write(memoryStream, MagickFormat.Bmp);
                memoryStream.Position = 0;
                image.Dispose();

                bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
                memoryStream.Dispose();

                return bitmapImage;
            }
        }

        public enum FormatFamily
        {
            AI,
            EPS,
            SVG,
            ICO,
            TGA,
        }

        public static readonly Dictionary<FormatFamily, List<MagickFormat>> FORMAT_FAMILIES = new()
        {
            { FormatFamily.AI,   new() { MagickFormat.Ai } },
            { FormatFamily.EPS,  new() { MagickFormat.Epi, MagickFormat.Eps, MagickFormat.Eps2, MagickFormat.Eps3, MagickFormat.Epsf, MagickFormat.Epsi, MagickFormat.Ept, MagickFormat.Ept2, MagickFormat.Ept3 } },
            { FormatFamily.SVG,  new() { MagickFormat.Svg, MagickFormat.Svgz } },
            { FormatFamily.ICO,  new() { MagickFormat.Ico } },
            { FormatFamily.TGA,  new() { MagickFormat.Icb, MagickFormat.Tga, MagickFormat.Vda, MagickFormat.Vst } },
        };

        public static bool IsAnyFormatFamilies(MagickFormat magickFormat, params FormatFamily[] formats)
        {
            return formats.Any(i =>
            {
                try
                {
                    return FORMAT_FAMILIES[i].Contains(magickFormat);
                }
                catch
                {
                    return false;
                }
            });
        }

        public static bool IsVectorFamily(MagickFormat magickFormat)
        {
            return IsAnyFormatFamilies(magickFormat, FormatFamily.AI, FormatFamily.EPS, FormatFamily.SVG);
        }

        public static MagickImage GetMagickImageMeta(string filePath)
        {
            try
            {
                var imageMeta = new MagickImage();
                imageMeta.Ping(filePath);
                AutoOrient(imageMeta);

                if (imageMeta.Format == MagickFormat.Unknown)
                    throw new();

                return imageMeta;
            }
            catch
            {
                var extension = Path.GetExtension(filePath).ToLowerInvariant();

                if (extension == ".dng")
                {
                    try
                    {
                        var imageMeta = new MagickImage();
                        imageMeta.Ping(filePath, new MagickReadSettings() { Format = MagickFormat.Jpeg });

                        if (imageMeta.Format == MagickFormat.Unknown)
                            throw new();

                        return imageMeta;
                    }
                    catch 
                    {
                        throw new MissingLibException("dng");
                    }
                }
                else if (extension == ".eps")
                    throw new MissingLibException("eps");

                throw new MissingLibException(string.Empty);
            }
        }

        public static int GetFrameCount(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();

            if (extension == ".dng" || extension == ".eps")
            {
                try
                {
                    using var imageMeta = GetMagickImageMeta(filePath);
                    return 1;
                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                using var imageMetas = new MagickImageCollection();
                imageMetas.Ping(filePath);
                return imageMetas.Count;
            }
        }

        public static Size? GetResolution(string filePath)
        {
            try
            {
                using var metaImage = GetMagickImageMeta(filePath);
                AutoOrient(metaImage);
                return new(metaImage.Width, metaImage.Height);
            }
            catch
            {
                return null;
            }
        }

        public static void RemoveExifMetadata(MagickImage image)
        {
            var exifProfile = image.GetExifProfile();
            if (exifProfile != null) image.RemoveProfile(exifProfile);
        }

        public static void RemoveXmpMetadata(MagickImage image)
        {
            var xmpProfile = image.GetXmpProfile();
            if (xmpProfile != null) image.RemoveProfile(xmpProfile);
        }

        public static void RemoveIptcMetadata(MagickImage image)
        {
            var iptcProfile = image.GetIptcProfile();
            if (iptcProfile != null) image.RemoveProfile(iptcProfile);
        }

        public static void RemoveAllMetadata(MagickImage image)
        {
            var exifProfile = image.GetExifProfile();
            var iptcProfile = image.GetIptcProfile();
            var xmpProfile = image.GetXmpProfile();

            if (exifProfile != null) image.RemoveProfile(exifProfile);
            if (iptcProfile != null) image.RemoveProfile(iptcProfile);
            if (xmpProfile != null) image.RemoveProfile(xmpProfile);
        }

        public static void RemoveAllMetadata(ref MagickImageCollection images)
        {
            foreach (var i in images)
            {
                var exifProfile = i.GetExifProfile();
                var iptcProfile = i.GetIptcProfile();
                var xmpProfile = i.GetXmpProfile();

                if (exifProfile != null) i.RemoveProfile(exifProfile);
                if (iptcProfile != null) i.RemoveProfile(iptcProfile);
                if (xmpProfile != null) i.RemoveProfile(xmpProfile);
            }
        }

        public static void Convert(ref MagickImage image, MagickFormat format)
        {
            if (image == null) return;
            if (image.Format == format) return;

            using var memoryStream = new MemoryStream();
            image.Write(memoryStream, MagickFormat.Bmp);
            memoryStream.Position = 0;

            image.Dispose();
            image = new MagickImage(memoryStream, MagickFormat.Bmp);
            memoryStream.Dispose();
        }

        public static void Normalize(ref MagickImage image)
        {
            if (IsAnyFormatFamilies(image.FormatInfo.Format, FormatFamily.TGA))
            {
                if (image.ChannelCount == 4)
                {
                    var memoryStream = new MemoryStream();
                    image.Write(memoryStream, MagickFormat.Bmp3);
                    memoryStream.Position = 0;

                    image.Dispose();
                    image = new MagickImage(memoryStream, MagickFormat.Bmp3);
                    memoryStream.Dispose();
                }
            }

            image.AutoOrient();
        }

        public static void AutoOrient(IMagickImage image)
        {
            image.AutoOrient();
        }

        public static BitmapImage LoadBitmapImage(string filePath)
        {
            var image = new MagickImage(filePath, new MagickReadSettings()
            {
                BackgroundColor = MagickColors.Transparent,
                ColorSpace = ColorSpace.sRGB
            });

            var memoryStream = new MemoryStream();
            var bitmapImage = new BitmapImage();

            image.Write(memoryStream, MagickFormat.Bmp);
            memoryStream.Position = 0;
            image.Dispose();

            bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
            memoryStream.Dispose();

            return bitmapImage;
        }

        public static BitmapImage LoadBitmapImage(string filePath, int width, int height)
        {
            var size = GetResolution(filePath);
            if (size == null) return null;

            var density = Utils.ComputeNeededDensityForSvg(size.Value.Width, size.Value.Height, width, height);

            var image = new MagickImage(filePath, new MagickReadSettings() { BackgroundColor = MagickColors.Transparent, Density = new(density) });
            image.Resize(width, height);

            var memoryStream = new MemoryStream();
            var bitmapImage = new BitmapImage();

            image.Write(memoryStream, MagickFormat.Bmp);
            memoryStream.Position = 0;
            image.Dispose();

            bitmapImage.SetSource(memoryStream.AsRandomAccessStream());
            memoryStream.Dispose();

            return bitmapImage;
        }

        public static void LoadImageAsync(string url, int? width, int? height, Action<MemoryStream> actionOnSuccess)
        {
            var memoryStream = new MemoryStream();

            IProgress<bool> progress = new Progress<bool>(success =>
            {
                if (success)
                {
                    actionOnSuccess?.Invoke(memoryStream);
                    memoryStream.Dispose();
                }
            });

            _ = Task.Run(() =>
            {
                try
                {
                    using var client = new RestClient(url);
                    var bytes = client.DownloadData(new("#", Method.Get));

                    memoryStream = new(bytes)
                    {
                        Position = 0
                    };

                    var image = new MagickImage(memoryStream, new MagickReadSettings()
                    {
                        BackgroundColor = MagickColors.Transparent,
                        ColorSpace = ColorSpace.sRGB
                    });

                    if (width != null && height != null)
                        image.Resize(width.Value, height.Value);

                    image.Write(memoryStream, MagickFormat.Bmp);
                    memoryStream.Position = 0;
                    image.Dispose();

                    progress.Report(true);
                }
                catch (Exception)
                {
                    progress.Report(false);
                }
            });
        }
    }
}
