using System;
using System.Threading.Tasks;

namespace IOCore.Libs
{
    public class AskToDo
    {
        private readonly static string SCOPE = nameof(AskToDo);

        public static long Latest
        {
            get => LocalStorage.GetValueOrDefault($"{SCOPE}-{nameof(Latest)}", 0L);
            private set { LocalStorage.Set($"{SCOPE}-{nameof(Latest)}", value); }
        }

        public static int HitCount
        {
            get => LocalStorage.GetValueOrDefault($"{SCOPE}-{nameof(HitCount)}", 0);
            private set { LocalStorage.Set($"{SCOPE}-{nameof(HitCount)}", value); }
        }

        public static long RequestedCount
        {
            get => LocalStorage.GetValueOrDefault($"{SCOPE}-{nameof(RequestedCount)}", -1);
            set { LocalStorage.Set($"{SCOPE}-{nameof(RequestedCount)}", value); }
        }

        public static async Task<bool> Request(Action action, bool useTimeTest, long timeTest, bool useHitCountTest = false, int hitCount = 1)
        {
            if (hitCount <= 0) throw new ArithmeticException($"hitCount must be greater than 0");
#if DEBUG
            return false;
#endif
            RequestedCount = RequestedCount < 0 ? 1 : RequestedCount + 1;

            var current = DateTimeOffset.Now.ToUnixTimeSeconds();

            var test = true;

            if (useTimeTest)
            {
                if (current - Latest < timeTest) test = false;
                Latest = current;
            }

            if (useHitCountTest)
            {
                HitCount++;
                if (HitCount % hitCount > 0) test = false;
            }

            if (!test) return false;

            action?.Invoke();
            return true;
        }
    }
}