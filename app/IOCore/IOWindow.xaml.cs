using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Resources;
using WinRT.Interop;
using Windows.ApplicationModel;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using System.IO;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using IOCore.Libs;
using Windows.ApplicationModel.DataTransfer;
using IOCore.Pages;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IOCore
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public abstract class IOWindow : Window, INotifyPropertyChanged
    {
        public static IOWindow Inst { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new(propertyName));

        protected static readonly ResourceLoader _resourceLoader = ResourceLoader.GetForViewIndependentUse();

        public enum StatusType
        {
            Init,
            Ready,
        }

        protected StatusType _status;
        public StatusType Status { get => _status; set { _status = value; OnPropertyChanged(nameof(Status)); } }

        public bool IsMaximize { get; set; }

        protected BitmapImage _icon;
        public BitmapImage Icon { get => _icon; set { _icon = value; OnPropertyChanged(nameof(Icon)); } }

        protected string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(nameof(Name)); } }

        protected BitmapImage _loadingIcon;
        public BitmapImage LoadingIcon { get => _loadingIcon; set { _loadingIcon = value; OnPropertyChanged(nameof(LoadingIcon)); } }

        protected bool _loading;
        public bool Loading { get => _loading; set { _loading = value; OnPropertyChanged(nameof(Loading)); } }

        protected bool _isFullScreen;
        public bool IsFullScreen { get => _isFullScreen; set { _isFullScreen = value; OnPropertyChanged(nameof(IsFullScreen)); } }

        public IOWindow()
        {
            Inst = this;
        }

        public void Init()
        {
            HandleIntPtr = WindowNative.GetWindowHandle(this);

            _windowId = Win32Interop.GetWindowIdFromWindow(HandleIntPtr);
            _appWindow = AppWindow.GetFromWindowId(_windowId);
            _appWindow.Changed += AppWindow_Changed;

            AppWindow.SetIcon(Path.Combine(Utils.GetAssetsFolderPath(), "icon.ico"));

            var text = Marshal.StringToCoTaskMemUni(Package.Current.DisplayName);
            PInvoke.SendMessage(new(HandleIntPtr), PInvoke.WM_SETTEXT, new(0), text);
            Marshal.FreeCoTaskMem(text);

            //

            LoadingIcon = ImageMagickUtils.AppIcon.Load(96, 96);

            Name = Package.Current.DisplayName;
            Icon = ImageMagickUtils.AppIcon.Load(20, 20);

            Utils.SetWindowSize(HandleIntPtr, Meta.APP_INIT_SIZE.Width, Meta.APP_INIT_SIZE.Height);
            SubClassing();
            IsMaximize = false;
        }

        #region Abstract

        public abstract XamlRoot RootXamlRoot { get; }

        public abstract void StatusBarLoading(bool visible);
        public abstract void StatusBarSetText(string text);

        public abstract void ShowInfoTeachingTip(object sender, string title = null, string subTitle = null);
        public abstract void ShowMessageTeachingTip(object sender, string title = null, string subTitle = null, Action action = null);
        public abstract void ShowConfirmTeachingTip(object sender, string title = null, string subTitle = null, Action confirmAction = null, Action closeAction = null);
        
        public abstract void SetCurrentNavigationViewItem(NavigationViewItem item, object parameter = null);
        public abstract void SetCurrentNavigationViewItem(string tag, object parameter = null);
        
        public abstract bool TryGoBack();

        public abstract void EnableNavigationViewItems(bool isEnabled);
        public abstract void VisibleNavigationPane(bool isVisible);

        #endregion

        public void Minimize() => PInvoke.ShowWindow(new(HandleIntPtr), SHOW_WINDOW_CMD.SW_MINIMIZE);

        public void Maximize()
        {
            HWND hWND = new(HandleIntPtr);

            var style = (int)WINDOW_LONG_PTR_INDEX.GWL_STYLE & (int)WINDOW_STYLE.WS_MAXIMIZE;
            if (IOInvoke.GetWindowLongPtr(HandleIntPtr, style) != IntPtr.Zero)
                PInvoke.ShowWindow(hWND, SHOW_WINDOW_CMD.SW_RESTORE);
            else
                PInvoke.ShowWindow(hWND, SHOW_WINDOW_CMD.SW_MAXIMIZE);

            IsMaximize = !IsMaximize;
        }

        private void AppWindow_Changed(AppWindow sender, AppWindowChangedEventArgs args)
        {
            if (args.DidPresenterChange && AppWindowTitleBar.IsCustomizationSupported())
                IsFullScreen = sender.Presenter.Kind == AppWindowPresenterKind.FullScreen;
        }

        public void ToggleFullScreenMode()
        {
            if (_appWindow.Presenter.Kind == AppWindowPresenterKind.FullScreen)
                _appWindow.SetPresenter(AppWindowPresenterKind.Default);
            else
                _appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
        }

        public double GetScalingFactor(bool system)
        {
            return system ? PInvoke.GetDpiForSystem() / 96.0 : PInvoke.GetDpiForWindow(new(HandleIntPtr)) / 96.0;
        }

        public async void PerformStandardMenuAction(string tag)
        {
            if (tag == "Share")
            {
                var hWnd = WindowNative.GetWindowHandle(this);
                var interop = DataTransferManager.As<IDataTransferManagerInterop>();
                var transferManager = DataTransferManager.FromAbi(interop.GetForWindow(hWnd, Guid.Parse("a5caee9b-8708-49d1-8d36-67d25a8da00c")));

                transferManager.DataRequested += (DataTransferManager _, DataRequestedEventArgs args) =>
                {
                    args.Request.Data = new DataPackage();
                    args.Request.Data.Properties.Title = Package.Current.DisplayName;
                    args.Request.Data.SetWebLink(new(Meta.URL_WEB_STORE));
                };

                interop.ShowShareUIForWindow(hWnd);
            }    
            else if (tag == "RateUs")
                _ = Launcher.LaunchUriAsync(new(Meta.URL_APP_STORE_REVIEW));
            else if (tag == "Exit")
                Application.Current.Exit();
            else
            {
                var dialog = new ContentDialog() { XamlRoot = RootXamlRoot };
                object content = null;

                if (tag == "Support")
                    content = new Support(dialog);
                else if (tag == "Settings")
                    content = new Settings(dialog);
                else if (tag == "LicenseInformation")
                    content = new LicenseInformation(dialog);
                else if (tag == "About")
                    content = new About(dialog);
                else if (tag == "MoreApps")
                {
                    var moreApps = new MoreApps(dialog);
                    moreApps.TryLoad();
                    content = moreApps;
                }

                dialog.Content = content;
                _ = await dialog.ShowAsync();
            }
        }

        //

        [ComImport, Guid("3A3DCD6C-3EAB-43DC-BCDE-45671CE800C8")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        protected interface IDataTransferManagerInterop
        {
            IntPtr GetForWindow([In] IntPtr appWindow, [In] ref Guid riid);
            void ShowShareUIForWindow(IntPtr appWindow);
        }

        //

        public IntPtr HandleIntPtr { get; private set; }

        public WindowId _windowId;
        protected AppWindow _appWindow;

        private WNDPROC _newWinProc;
        private IntPtr _oldWinProc = IntPtr.Zero;

        protected void SubClassing()
        {
            _newWinProc = new(NewWindowProc);
            _oldWinProc = IOInvoke.SetWindowLongPtr(HandleIntPtr, (int)WINDOW_LONG_PTR_INDEX.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(_newWinProc));
        }

        private LRESULT NewWindowProc(HWND HWND, uint msg, WPARAM wParam, LPARAM lParam)
        {
            switch (msg)
            {
                case PInvoke.WM_GETMINMAXINFO:
                    var scalingFactor = PInvoke.GetDpiForWindow(HWND) / 96.0;

                    var minMaxInfo = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                    minMaxInfo.ptMinTrackSize.X = Utils.Round(Meta.APP_INIT_SIZE.Width * scalingFactor);
                    minMaxInfo.ptMinTrackSize.Y = Utils.Round(Meta.APP_INIT_SIZE.Height * scalingFactor);
                    Marshal.StructureToPtr(minMaxInfo, lParam, true);
                    break;
            }

            return PInvoke.CallWindowProc(Marshal.GetDelegateForFunctionPointer<WNDPROC>(_oldWinProc), HWND, msg, new(wParam), new(lParam));
        }
    }
}
