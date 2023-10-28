using System.Collections.Generic;
using System.Linq;

namespace IOCore.Libs
{
    public class FileUtils
    {
        public enum Type
        {
            Unknown,
            Image,
            Video,
            Audio,
            Document,
        }

        public static readonly Dictionary<Type, string[]> EXTENSIONS = new()
        {
            { Type.Image, new[] {
                ".avif", ".eps", ".heic", ".heif", ".psb", ".psd", ".qoi", ".svg", ".svgz", ".icb", ".tga", ".vda", ".vst", ".tif", ".tiff", ".webp", ".xbm", ".xpm", ".bmp", ".rle", ".dib", ".gif", ".ico", ".jpg", ".jpeg", ".jpe", ".png", ".pbm", ".pgm", ".ppm", ".pnm", ".pfm", ".pam", ".pcx", ".wbm", ".wbmp",
                ".arw", ".srf", ".sr2", ".cr2", ".cr3", ".crw", ".dcr", ".kdc", ".k25", ".dng", ".erf", ".mef", ".nef", ".nrw", ".orf", ".pef", ".raf", ".raw", ".rw2"
            }},
            { Type.Video, new[] {
                ".3gp", ".3g2", ".asf", ".avi", ".flv", ".m4v", ".mkv", ".mov", ".mp4", ".m4a", ".m4p", ".m4b", ".m4r", ".mpeg", ".mpg", ".mxf", ".ogg", ".ogv", ".oga", ".ogx", ".ogm", ".spx", ".opus", ".ogv", ".swf", ".ts", ".tsv", ".tsa", ".m2t", ".vob", ".webm", ".wm", ".wmv"
            }},
            { Type.Audio, new[] {
                ".aac", ".ac3", ".aiff", ".aifc", ".aif", ".amr", ".au", ".caf", ".dff", ".dsf", ".flac", ".m4a", ".m4b", ".m4p", ".m4r", ".mp2", ".mp3", ".ogg", ".oga", ".opus", ".spx", ".opus", ".tta", ".voc", ".wav", ".weba", ".wma", ".wv"
            }},
            { Type.Document, new[] { 
                ".doc", ".docx", ".docm", ".ppt", ".pptx", ".pptm", ".xls", ".xlsx", ".xlsm", ".pdf", ".txt", ".text", ".rtf", ".tex", ".odt", ".ods", ".odp", ".pages", ".key", ".epub", ".mobi", ".azw", ".azw3", ".fb2"
            }},
        };

        public static bool Is(string extension, Type type)
        {
            return EXTENSIONS[type].Contains(extension);
        }

        public static Type GetType(string extension)
        {
            foreach (var i in EXTENSIONS)
                if (i.Value.Contains(extension))
                    return i.Key;

            return Type.Unknown;
        }
    }
}