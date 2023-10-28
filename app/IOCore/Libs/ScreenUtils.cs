using System.Drawing;
using Windows.Win32;
using Windows.Win32.Graphics.Gdi;

namespace IO.Core.Libs
{
    internal class ScreenUtils
    {
        public static MONITORINFO MonitorInfoFromPoint(Point pt, MONITOR_FROM_FLAGS flags)
        {
            var monitor = PInvoke.MonitorFromPoint(pt, flags);
            MONITORINFO info = new();
            PInvoke.GetMonitorInfo(monitor, ref info);
            return info;
        }
    }
}