using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using QRCoder;
using RestSharp;
using Windows.ApplicationModel;
using Windows.Management.Deployment;
using Windows.Services.Store;
using WinRT.Interop;

namespace IOCore.Libs
{
    public class AppItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public class Platform
        {
            public string AppId { get; set; }
            public string StoreId { get; set; }

            public string ReferenceId { get; set; }
            public bool IsInReview { get; set; }
        }

        public string _id { get; set; }
        public string Name { get; set; }
        public string Headline { get; set; }
        public string Description { get; set; }
        public Platform Windows { get; set; }

        public bool IsFeatured { get; set; }

        public int Order { get; set; }

        //

        public string AppId => Windows?.AppId;
        public string StoreId => Windows?.StoreId;

        public string StoreUrl => $"ms-windows-store://pdp/?productid={Windows.StoreId}&cid={Meta.APP_SLUG}";

        //

        private StoreProduct _storeProduct;
        public StoreProduct StoreProduct 
        {
            get => _storeProduct;
            set
            {
                _storeProduct = value;

                PropertyChanged?.Invoke(this, new(nameof(StoreProduct)));

                PropertyChanged?.Invoke(this, new(nameof(Icon)));
                PropertyChanged?.Invoke(this, new(nameof(Price)));
                PropertyChanged?.Invoke(this, new(nameof(SalePrice)));
                PropertyChanged?.Invoke(this, new(nameof(IsOnSale)));
            }
        }

        public string Icon => _storeProduct?.Images[0]?.Uri?.OriginalString;
        public string Price => _storeProduct?.Price?.FormattedBasePrice;
        public string SalePrice => _storeProduct?.Price?.FormattedPrice;
        public bool IsOnSale => _storeProduct?.Price?.IsOnSale ?? false;

        public BitmapImage StoreQrCodeImage { get; private set; }
        public void UseQrCodeImage(bool raise)
        {
            if (StoreQrCodeImage != null) return;

            var qrCodeData = InAppPromotion.QrCodeGenerator.CreateQrCode(StoreUrl, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(4);

            StoreQrCodeImage = Utils.ConvertBitmapToBitmapImage(qrCodeImage);
            if (raise) PropertyChanged?.Invoke(this, new(nameof(StoreQrCodeImage)));
        }

        public AppItem() { }
    }

    public class InAppPromotion
    {
        public static readonly QRCodeGenerator QrCodeGenerator = new();

        private readonly List<AppItem> _appItems = new();

        public AppItem CurrentAppItem { get; private set; } = null;
        public AppItem ReferenceAppItem { get; private set; } = null;

        public List<AppItem> FeaturedAppItems { get; private set; } = new();
        public List<AppItem> StandardAppItems { get; private set; } = new();

        public List<Package> InstalledPackages { get; private set; } = null;

        public object Locked = new();

        private InAppPromotion()
        {
        }

        private static readonly Lazy<InAppPromotion> lazy = new(() => new());
        public static InAppPromotion Inst => lazy.Value;

        public void LoadAppItemsAsync(Action action)
        {
            _ = Task.Run(() =>
            {
                lock (Locked)
                {
                    if (_appItems.Count == 0)
                    {
                        try
                        {
                            InstalledPackages ??= new PackageManager().FindPackagesForUser(string.Empty).
                                Where(package => Meta.IO_PUBLISHERS.Any(publisher => publisher.Value == package.Id.Publisher)).ToList();

                            var appItems = new RestClient(Meta.URL_IO_API_APPS).Get<List<AppItem>>(new());

                            appItems.ForEach(i =>
                            {
                                if (i.AppId == Package.Current.Id.Name || !InstalledPackages.Any(p => p.Id.Name == i.AppId))
                                {
                                    _appItems.Add(i);

                                    if (i.AppId == Package.Current.Id.Name)
                                    {
                                        CurrentAppItem = i;
                                        var index = appItems.FindIndex(a => a._id == CurrentAppItem.Windows.ReferenceId);
                                        if (index != -1) ReferenceAppItem = appItems[index];
                                    }
                                    else if (i.IsFeatured)
                                        FeaturedAppItems.Add(i);
                                    else
                                        StandardAppItems.Add(i);
                                }
                            });

                            if (FeaturedAppItems.Count >= 2)
                                FeaturedAppItems.Sort((i1, i2) => i2.Order.CompareTo(i1.Order));

                            StandardAppItems.Shuffle();

                            var context = StoreContext.GetDefault();
                            InitializeWithWindow.Initialize(context, IOWindow.Inst.HandleIntPtr);
                            var storeProducts = context.GetStoreProductsAsync(new List<string> { "Application" }, _appItems.Select(i => i.StoreId)).GetAwaiter().GetResult();

                            foreach (var i in _appItems)
                                if (i.StoreId != null && storeProducts.Products.Any(p => p.Key == i.StoreId))
                                    i.StoreProduct = storeProducts.Products[i.StoreId];
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }
                }

                action?.Invoke();
            });
        }

        public void GetAppItemByNameAsync(string name, Action<AppItem> action)
        {
            LoadAppItemsAsync(() =>
            {
                if (_appItems.Count > 0)
                    action?.Invoke(_appItems.FirstOrDefault(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
            });
        }

        public void GetAppItemByAppIdAsync(string appId, Action<AppItem> action)
        {
            LoadAppItemsAsync(() =>
            {
                if (_appItems.Count > 0)
                    action?.Invoke(_appItems.FirstOrDefault(i => i.AppId == appId));
            });
        }

        public void GetRandomAppItemAsync(Action<AppItem> action)
        {
            LoadAppItemsAsync(() =>
            {
                var index = 0;
                for (var i = 0; i < 3; i++)
                {
                    index = Utils.Random.Next(_appItems.Count);
                    if (CurrentAppItem == null) break;
                    else if (CurrentAppItem.AppId != _appItems[index].AppId) break;
                }

                action?.Invoke(_appItems.ElementAtOrDefault(index));
            });
        }
    }
}