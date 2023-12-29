using ImageMagick;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using IOCore.Libs;
using IOApp.Features;

namespace IOApp.Configs
{
    internal class Manage
    {
        public enum ModeType
        {
            Inpaint,
            Filter,
            OCR
        }

        public enum StatusType
        {
            Ready,
            Loading,
            Loaded,
            LoadFailed,
            Processing,
            Processed,
            ProcessFailed,
        }

        public static readonly Dictionary<StatusType, string> STATUSES = new()
        {
            { StatusType.Ready, "StatusReady" },
            { StatusType.Loading, "StatusLoading" },
            { StatusType.Loaded, "StatusLoaded" },
            { StatusType.LoadFailed, "StatusLoadFailed" },
            { StatusType.Processing, "StatusProcessing" },
            { StatusType.Processed, "StatusProcessed" },
            { StatusType.ProcessFailed, "StatusProcessFailed" },
        };

        public enum ShapeType
        {
            Polyline,
            Rectangle,
            Ellipse
        }

        public enum FilterType
        {
            _1977,
            Aden,
            Brannan,
            Brooklyn,
            Clarendon,
            Earlybird,
            Gingham,
            Hudson,
            Inkwell,
            Kelvin,
            Lark,
            Lofi,
            Maven,
            Mayfair,
            Moon,
            Nashville,
            Perpetua,
            Reyes,
            Rise,
            Slumber,
            Stinson,
            Toaster,
            Valencia,
            Walden,
            Willow,
            Xpro2,
        }

        public enum LangType
        {
            English,
            Vietnamese,
            Chinese
        }
        static Manage()
        {
        }
    }
}