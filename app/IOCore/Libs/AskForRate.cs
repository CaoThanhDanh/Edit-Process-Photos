using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Services.Store;
using IOCore.Pages;

namespace IOCore.Libs
{
    public class AskForRate
    {
        private readonly static string SCOPE = nameof(AskForRate);

        public static long TimeTest = 2 * 24 * 3600;

        public static bool IsRated
        {
            get => LocalStorage.GetValueOrDefault($"{SCOPE}-{nameof(IsRated)}", false);
            private set { LocalStorage.Set($"{SCOPE}-{nameof(IsRated)}", value); }
        }

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

        public static async Task<bool> Request(bool useTimeTest, long timeTest, bool useHitCountTest = false, int hitCount = 1)
        {
            if (hitCount <= 0) throw new ArithmeticException($"hitCount must be greater than 0");
#if DEBUG
            return false;
#endif
            RequestedCount = RequestedCount < 0 ? 1 : RequestedCount + 1;

            if (IsRated) return false;

            var test = true;

            if (useTimeTest)
            {
                var current = DateTimeOffset.Now.ToUnixTimeSeconds();
                if (current - Latest < timeTest) test = false;
                Latest = current;
            }

            if (useHitCountTest)
            {
                HitCount++;
                if (HitCount % hitCount > 0) test = false;
            }

            if (!test) return false;

            var result = await StoreManager.Inst.GetContext().RequestRateAndReviewAppAsync();

            switch (result.Status)
            {
                case StoreRateAndReviewStatus.Succeeded:
                    IOWindow.Inst.ShowMessageTeachingTip(null, "Thank you for your rating!");
                    IsRated = true;
                    break;
                case StoreRateAndReviewStatus.CanceledByUser:
                    break;
                case StoreRateAndReviewStatus.NetworkError:
                    break;
                case StoreRateAndReviewStatus.Error:
                default:
                    break;
            }

            return true;
        }

        public static async void TryShowRateToUnlockIfAny(XamlRoot xamlRoot, Action purchaseAction, Action restoreAction)
        {
#if DEBUG
            return;
#endif
            if (IsRated) return;

            try
            {
                var client = new RestClient(Meta.URL_IO_API_APP);
                var appItem = await client.GetAsync<AppItem>(new());

                if (!(appItem?.Windows?.IsInReview ?? true))
                {
                    var contentDialog = new ContentDialog() { XamlRoot = xamlRoot };
                    object content = new RateToUnlock(contentDialog, purchaseAction, restoreAction);
                    contentDialog.Content = content;
                    _ = await contentDialog.ShowAsync();
                }
            }
            catch { }
        }
    }

    public class AskForRateAdvanced
    {
        public class Record
        {
            public string Key;

            public long Latest;
            public int HitCount;
            public long RequestedCount;
        }

        private readonly static string SCOPE = nameof(AskForRateAdvanced);

        public static Dictionary<string, Record> Records
        {
            get
            {
                var recordArrayStr = LocalStorage.GetValueOrDefault<string>($"{SCOPE}-{nameof(Records)}", null);
                if (recordArrayStr == null)
                    return new Dictionary<string, Record>();

                var recordArray = JsonConvert.DeserializeObject<string[]>(recordArrayStr);

                Dictionary<string, Record> records = new();

                if (recordArray == null) return records;

                foreach (var i in recordArray)
                {
                    var record = JsonConvert.DeserializeObject<Record>(i);
                    records.Add(record.Key, record);
                }

                return records;
            }

            private set 
            {
                if (value != null)
                {
                    var str = JsonConvert.SerializeObject(value.Select(i => JsonConvert.SerializeObject(i.Value)).ToArray());
                    LocalStorage.Set($"{SCOPE}-{nameof(Records)}", str);
                }
            }
        }

        public static bool IsRated
        {
            get => LocalStorage.GetValueOrDefault($"{SCOPE}-{nameof(IsRated)}", false);
            private set { LocalStorage.Set($"{SCOPE}-{nameof(IsRated)}", value); }
        }

        public static async Task<bool> Request(string key, bool useTimeTest, long timeTest, bool useHitCountTest = false, int hitCount = 1)
        {
            var tempRecords = Records;

            Record record;

            if (tempRecords.ContainsKey(key))
                record = tempRecords[key];
            else
            {
                record = new()
                {
                    Key = key
                };
                tempRecords.Add(key, record);

                Records = tempRecords;
                record = tempRecords[key];
            }

            if (hitCount <= 0) throw new ArithmeticException($"hitCount must be greater than 0");

            record.RequestedCount = record.RequestedCount < 0 ? 1 : record.RequestedCount + 1;

            Records = tempRecords;

            if (IsRated) return false;

            var test = true;

            if (useTimeTest)
            {
                var current = DateTimeOffset.Now.ToUnixTimeSeconds();
                if (current - record.Latest < timeTest) test = false;
                record.Latest = current;

                Records = tempRecords;
            }

            if (useHitCountTest)
            {
                record.HitCount++;
                if (record.HitCount % hitCount > 0) test = false;

                Records = tempRecords;
            }

            if (!test) return false;

#if DEBUG
            IOWindow.Inst.ShowMessageTeachingTip(null, "Ask for rate", "Debug mode only");
            IsRated = true;
            return true;
#endif
            var result = await StoreManager.Inst.GetContext().RequestRateAndReviewAppAsync();

            switch (result.Status)
            {
                case StoreRateAndReviewStatus.Succeeded:
                    IOWindow.Inst.ShowMessageTeachingTip(null, "Thank you for your rating!");
                    IsRated = true;
                    break;
                case StoreRateAndReviewStatus.CanceledByUser:
                    break;
                case StoreRateAndReviewStatus.NetworkError:
                    break;
                case StoreRateAndReviewStatus.Error:
                default:
                    break;
            }

            return true;
        }

        public static async void TryShowRateToUnlockIfAny(XamlRoot xamlRoot, Action purchaseAction, Action restoreAction)
        {
#if DEBUG
            return;
#endif
            if (IsRated) return;

            try
            {
                var client = new RestClient(Meta.URL_IO_API_APP);
                var appItem = await client.GetAsync<AppItem>(new());

                if (!(appItem?.Windows?.IsInReview ?? true))
                {
                    var contentDialog = new ContentDialog() { XamlRoot = xamlRoot };
                    object content = new RateToUnlock(contentDialog, purchaseAction, restoreAction);
                    contentDialog.Content = content;
                    _ = await contentDialog.ShowAsync();
                }
            }
            catch { }
        }
    }
}