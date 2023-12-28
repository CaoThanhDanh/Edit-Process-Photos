using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Windows.ApplicationModel;

namespace IOCore
{
    public class Meta
    {
        public static string APP_SLUG;
        public static string IO_APP_ID;
        public static string MS_STORE_ID;
        public static string[] MS_STORE_ADD_ON_IDS;

        public static Size APP_INIT_SIZE;

        public static string URL_IO_APP => $"https://www.iostream.vn/{APP_SLUG}";
        public static string URL_IO_API_APP => $"{URL_IO_API_BASE}/apps/@{APP_SLUG}";
        public static string URL_WEB_STORE => $"https://apps.microsoft.com/store/detail/{MS_STORE_ID}";
        public static string URL_APP_STORE => $"ms-windows-store://pdp/?productid={MS_STORE_ID}";
        public static string URL_APP_STORE_REVIEW => $"ms-windows-store://review/?productid={MS_STORE_ID}";

        public static readonly string IO_EMAIL = "developer@iostream.vn";

        public static readonly Dictionary<string, string> IO_PUBLISHERS = new()
        {
            {"IO Stream", "CN=A9D6F582-B836-4F76-80AA-966F7B6D2DAF" },
            {"IO Lab", "CN=868B4F3C-E052-4393-A023-73B1FAB8378A" },
            {"IO Vision", "CN=F20DD56F-CC1B-4FA6-B0F1-136620615BC7" },
            {"Cypher Stream", "CN=4BB51AAB-3AE9-488B-8E47-3FBFF9AFA3F1" },
            {"IO Stream Co., Ltd", "CN=671AC663-A793-4A03-B1E1-5D1E63B6097E" }
        };

        //

        public static readonly string URL_IO_PRIVACY = "https://www.iostream.vn/io/io-apps-privacy-policy-D13wF2";

        public static readonly string URL_IO = "https://www.iostream.vn";

        public static readonly string URL_IO_API_BASE = "https://api.iostream.vn";
        public static readonly string URL_IO_API_APPS = $"{URL_IO_API_BASE}/apps/iap?windows=true";
        public static readonly string URL_IO_API_CONTACT = $"{URL_IO_API_BASE}/contact";

        public static readonly string URL_IO_CANCEL_SUBSCRIPTION = "https://www.iostream.vn/io/how-to-cancel-microsoft-subscription-R1PpQC";
        public static readonly string URL_APP_STORE_PUBLISHER = $"ms-windows-store://publisher/?name={Package.Current.PublisherDisplayName}";

        //

        public static readonly string EXTERNAL_DIR = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Externals");
        public static readonly string APP_DIR = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Package.Current.PublisherDisplayName, Package.Current.DisplayName);
        public static readonly string TEMP_DIR = Path.Combine(Path.GetTempPath(), Package.Current.PublisherDisplayName, Package.Current.DisplayName);
        public static readonly string FILTER_DIR = Path.Combine(Path.GetTempPath(), "FILTER_DIR");

        //

        public static readonly string URL_TWITTER = "https://twitter.com/iostream_vn";
        public static readonly string DONATION_LINK = "https://ko-fi.com/iostream_vn";
    }
}