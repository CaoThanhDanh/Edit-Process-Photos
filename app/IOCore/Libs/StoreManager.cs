using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Services.Store;
using WinRT.Interop;

namespace IOCore.Libs
{
    public class StoreManager
    {
#if DEBUG
        public enum License
        {
            Normal,
            Trial,
            Premium
        }

        public static License Force;
#endif

        public enum PurchaseType
        {
            NEW,
            RESTORE
        }

        public interface IInAppPurchase
        {
            Task UpdateLicenseStatus();
        }

        public class ProductItem
        {
            public string StoreId { get; set; }
            public TextBlock TitleTextBlock { get; set; }
            public TextBlock PriceTextBlock { get; set; }
            public Control PurchaseButton { get; set; }

            public ProductItem(string storeId, TextBlock titleTextBlock, TextBlock priceTextBlock, Control purchaseButton)
            {
                StoreId = storeId;
                TitleTextBlock = titleTextBlock;
                PriceTextBlock = priceTextBlock;
                PurchaseButton = purchaseButton;

                if (PurchaseButton != null)
                    PurchaseButton.IsEnabled = false;
            }

            public async void Init(Status status)
            {
                if (status.IsTrial)
                {
                    if (PurchaseButton != null && !PurchaseButton.IsEnabled)
                    {
                        var productResult = await Inst.GetContext().GetStoreProductForCurrentAppAsync();

                        if (productResult.ExtendedError == null)
                        {
                            if (TitleTextBlock != null)
                                TitleTextBlock.Text = productResult.Product.Title;
                            if (PriceTextBlock != null)
                                PriceTextBlock.Text = productResult.Product.Price.FormattedPrice;
                            if (PurchaseButton != null)
                                PurchaseButton.IsEnabled = true;
                        }
                    }
                }
            }
        }

        public class Status
        {
            public bool IsPurchased { get; set; }
            public bool IsExpired { get; set; }

            public bool IsPurchasedExpired => IsPurchased && IsExpired;
            public bool IsPremium => IsPurchased && !IsExpired;
            public bool IsTrial => !IsPremium;

            public Status(bool isPurchased, bool isExpired)
            {
                IsPurchased = isPurchased;
                IsExpired = isExpired;
            }
        }

        private StoreManager()
        {
        }

        private static readonly Lazy<StoreManager> lazy = new(() => new());
        public static StoreManager Inst => lazy.Value;

        //

        private StoreContext _context;
        public StoreContext GetContext()
        {
            if (_context == null)
            {
                _context = StoreContext.GetDefault();
                InitializeWithWindow.Initialize(_context, IOWindow.Inst.HandleIntPtr);
            }

            return _context;
        }

        public async Task<Status> GetProductLicenseStatus(bool useLoading)
        {
#if DEBUG
            if (Force == License.Trial)
                return new(false, false);
            else if (Force == License.Premium)
                return new(true, false);
#endif

            //if (useLoading) MainWindow.Inst.Loading = true;
            var license = await GetContext()?.GetAppLicenseAsync();
            //if (useLoading) MainWindow.Inst.Loading = false;

            return new(license.IsActive && !license.IsTrial, false);
        }

        public async Task<StoreProductResult> GetCurrentProduct(bool useLoading)
        {
            //if (useLoading) MainWindow.Inst.Loading = true;
            var storeProductResult = await GetContext()?.GetStoreProductForCurrentAppAsync();
            //if (useLoading) MainWindow.Inst.Loading = false;

            return storeProductResult;
        }

        public async Task<StoreProductQueryResult> GetProductsByStoreIds(bool useLoading, IEnumerable<string> storeIds, Action<StoreProductQueryResult> action = null)
        {
            //if (useLoading) MainWindow.Inst.Loading = true;
            var storeProductResults = await GetContext()?.GetStoreProductsAsync(new List<string> { "Application" }, storeIds);
            //if (useLoading) MainWindow.Inst.Loading = false;

            action?.Invoke(storeProductResults);

            return storeProductResults;
        }

        private async Task<bool> PurchaseProduct()
        {
            var productResult = await GetContext().GetStoreProductForCurrentAppAsync();

            if (productResult.ExtendedError != null)
            {
                //MainWindow.Inst.ShowMessageTeachingTip(null, "Error", productResult.ExtendedError.Message);
                return false;
            }

            var result = await productResult.Product.RequestPurchaseAsync();

            if (result.ExtendedError != null)
            {
                //MainWindow.Inst.ShowMessageTeachingTip(null, "Purchase failed", result.ExtendedError.Message);
                return false;
            }

            var errorMessage = result.ExtendedError != null ? result.ExtendedError.Message : string.Empty;

            var status = false;

            switch (result.Status)
            {
                //case StorePurchaseStatus.AlreadyPurchased:
                //    status = true;
                //    break;
                //case StorePurchaseStatus.Succeeded:
                //    status = true;
                //    MainWindow.Inst.ShowMessageTeachingTip(null, "Thank you for purchasing", $"{Package.Current.DisplayName} Premium Forever!");
                //    break;
                //case StorePurchaseStatus.NotPurchased:
                //    MainWindow.Inst.ShowMessageTeachingTip(null, "The purchase did not complete, it may have been canceled", errorMessage);
                //    break;
                //case StorePurchaseStatus.NetworkError:
                //    MainWindow.Inst.ShowMessageTeachingTip(null, "Product was not purchased due to a Network Error", errorMessage);
                //    break;
                //case StorePurchaseStatus.ServerError:
                //    MainWindow.Inst.ShowMessageTeachingTip(null, "Product was not purchased due to a Server Error", errorMessage);
                //    break;
                //default:
                //    MainWindow.Inst.ShowMessageTeachingTip(null, "Product was not purchased due to an Unknown Error", errorMessage);
                //    break;
            }

            return status;
        }

        public async Task<List<StoreProduct>> GetAddOns(string[] storeIds, bool useLoading)
        {
            //if (useLoading) MainWindow.Inst.Loading = true;
            var queryResult = await GetContext()?.GetStoreProductsAsync(new[] { "Durable" }, storeIds);
            //if (useLoading) MainWindow.Inst.Loading = false;

            if (queryResult.ExtendedError != null)
                return null;

            var addOns = new List<StoreProduct>();

            foreach (var i in queryResult.Products)
                addOns.Add(i.Value);

            return addOns;
        }

        public async Task<Status> GetAddOnLicenseStatus(string[] addOnStoreIds, bool useLoading)
        {
#if DEBUG
            if (Force == License.Trial)
                return new(false, false);
            else if (Force == License.Premium)
                return new(true, false);
#endif

            //if (useLoading) MainWindow.Inst.Loading = true;
            var license = await GetContext().GetAppLicenseAsync();
            //if (useLoading) MainWindow.Inst.Loading = false;

            if (license == null)
            {
                //MainWindow.Inst.ShowMessageTeachingTip(null, "An error occurred while retrieving the license");
                return new(false, false);
            }

            var isPurchased = false;

            foreach (var item in license.AddOnLicenses)
            {              
                foreach (var storeId in addOnStoreIds)
                {
                    if (item.Value.SkuStoreId.StartsWith(storeId))
                    {
                        isPurchased = true;

                        if ((item.Value.ExpirationDate - DateTime.Now).TotalDays >= 0)
                            return new(true, false);
                    }
                }
            }

            if (isPurchased)
                return new(true, true);

            return new(false, false);
        }

        private async Task<bool> PurchaseAddOn(string storeId)
        {
            var result = await GetContext().RequestPurchaseAsync(storeId);

            string errorMessage = result.ExtendedError != null ? result.ExtendedError.Message : string.Empty;

            var status = false;

            switch (result.Status)
            {
                case StorePurchaseStatus.AlreadyPurchased:
                    status = true;
                    IOWindow.Inst.ShowMessageTeachingTip(null, "You have already purchased the product");
                    break;
                case StorePurchaseStatus.Succeeded:
                    status = true;
                    IOWindow.Inst.ShowMessageTeachingTip(null, "Thank you for purchasing");
                    break;
                case StorePurchaseStatus.NotPurchased:
                    IOWindow.Inst.ShowMessageTeachingTip(null, "The purchase did not complete, it may have been canceled", errorMessage);
                    break;
                case StorePurchaseStatus.NetworkError:
                    IOWindow.Inst.ShowMessageTeachingTip(null, "The purchase was unsuccessful due to a network error", errorMessage);
                    break;
                case StorePurchaseStatus.ServerError:
                    IOWindow.Inst.ShowMessageTeachingTip(null, "The purchase was unsuccessful due to a server error", errorMessage);
                    break;
                default:
                    IOWindow.Inst.ShowMessageTeachingTip(null, "The purchase was unsuccessful due to an unknown error", errorMessage);
                    break;
            }

            return status;
        }

        //

        public async void PurchaseOrRestoreProduct(PurchaseType purchaseType, Action success = null)
        {
            if (purchaseType == PurchaseType.NEW)
            {
                if (await PurchaseProduct()) success?.Invoke();
            }
            else if (purchaseType == PurchaseType.RESTORE)
            {
                var status = await GetProductLicenseStatus(true);
                if (status.IsPremium) success?.Invoke();
                //else MainWindow.Inst.ShowMessageTeachingTip(null, "Restore failed");
            }
        }

        public async void PurchaseOrRestoreAddOn(PurchaseType purchaseType, string[] addOnStoreIds, Action success = null)
        {
            if (purchaseType == PurchaseType.NEW)
            {
                if (await PurchaseAddOn(addOnStoreIds[0])) success?.Invoke();
            }
            else if (purchaseType == PurchaseType.RESTORE)
            {
                var status = await GetAddOnLicenseStatus(addOnStoreIds, true);
                if (status.IsPremium) success?.Invoke();
                //else MainWindow.Inst.ShowMessageTeachingTip(null, "Restore failed");
            }
        }
    }
}
