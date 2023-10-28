using System;

namespace IOCore.Libs
{
    public class TimeUtils
    {
        public static ulong GetCurrentUnixTimestamp()
        {
            return GetUnixTimestamp(DateTime.Now);
        }

        public static ulong GetUnixTimestamp(DateTime time)
        {
            var zero = new DateTime(1970, 1, 1);
            var span = time.Subtract(zero);

            return (ulong)span.TotalSeconds;
        }
    }
}
