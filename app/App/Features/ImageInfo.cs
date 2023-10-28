using System.IO;
using ImageMagick;
using IOApp.Configs;
using IOCore.Libs;

namespace IOApp.Features
{
    internal class ImageInfo
    {
        public string Path { get; set; }

        public FileInfo FileInfo;

        public MagickFormat Format { get; set; }
        public bool IsRaw { get; private set; }
        public string MimeType { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int? FrameCount { get; set; }

        //

        public string FileName => FileInfo?.Name ?? string.Empty;
        public string FileExtension => FileInfo?.Extension ?? string.Empty;
        public long FileSize => FileInfo?.Length ?? 0;
        public string DimensionText => Width != null && Height != null ? $"{Width}x{Height}" : string.Empty;

        //

        public ImageInfo(string path = null, bool doLoadInfo = false)
        {
            Path = path;

            Format = MagickFormat.Null;
            MimeType = null;

            Width = null;
            Height = null;

            FrameCount = null;

            if (doLoadInfo)
                LoadInfo();
        }

        public void LoadInfo()
        {
            try
            {
                FileInfo = new(Path);
            }
            catch
            {
            }
        }

        public void LoadFormatAndDimentionInfo()
        {
            try
            {
                var imageMeta = ImageMagickUtils.GetMagickImageMeta(Path);

                Format = imageMeta.FormatInfo.Format;
                MimeType = imageMeta.FormatInfo.MimeType;

                Width = imageMeta.Width;
                Height = imageMeta.Height;

                IsRaw = Profile.IsRawInputFormat(Format);
            }
            catch { }
        }

        public void LoadFrameCount()
        {
            FrameCount = IsRaw ? 1 : ImageMagickUtils.GetFrameCount(Path);
        }
    }
}