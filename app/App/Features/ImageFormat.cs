using ImageMagick;
using System.Linq;

namespace IOApp.Features
{
    internal class ImageFormat
    {
        public string Name { get; private set; }
        public bool IsRaw { get; private set; }
        public string[] Extensions { get; private set; }
        public MagickFormat[] Formats { get; private set; }
        public bool IsSupported { get; private set; }

        public string Extension => Extensions.FirstOrDefault();
        public MagickFormat Format => Formats.FirstOrDefault();

        public ImageFormat(bool isSupported, string name, bool isRaw, string[] extensions, MagickFormat[] formats)
        {
            IsSupported = isSupported;
            Name = name;
            IsRaw = isRaw;
            Extensions = extensions;
            Formats = formats;
        }
    }
}