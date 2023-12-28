using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using Windows.System;
using IOApp.Pages;
using IOCore.Libs;
using IOCore;
using IOApp.Configs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IOApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    internal sealed partial class MainWindow : IOWindow
    {
        public static new MainWindow Inst { get; private set; }

        public override XamlRoot RootXamlRoot => Root.XamlRoot;

        public Visibility PromotionAppVisibility { get; private set; } = Visibility.Collapsed;
        public RangeObservableCollection<AppItem> PromotionAppItems { get; private set; } = new();

        public MainWindow() : base()
        {
            InitializeComponent();
            Root.DataContext = this;
            Inst = this;
            Init();

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);

            Utils.DeleteFileOrDirectory(Meta.TEMP_DIR);
            Utils.DeleteFileOrDirectory(Meta.FILTER_DIR);

            Status = StatusType.Ready;
        }

        private void Root_Loaded(object sender, RoutedEventArgs e)
        {
            IProgress<AppItem> progress = new Progress<AppItem>(i =>
            {
                if (i != null) PromotionAppItems.Add(i);
                if (InAppPromotion.Inst.ReferenceAppItem != null) PromotionAppItems.Add(InAppPromotion.Inst.ReferenceAppItem);
                if (PromotionAppItems.Count > 0)
                {
                    PromotionAppVisibility = Visibility.Visible;
                    OnPropertyChanged(nameof(PromotionAppVisibility));
                }
            });
            InAppPromotion.Inst.GetRandomAppItemAsync(i => { progress.Report(i); });
        }

        public async void ShowMissingLibDialog()
        {
            var dialog = new ContentDialog() { XamlRoot = Root.XamlRoot };
            object content = new MissingLib(dialog);
            dialog.Content = content;
            _ = await dialog.ShowAsync();
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.Tag is not string tag) return;

            if (tag == "AddFiles")
                Main.Inst?.OpenInputPicker();
            else
                PerformStandardMenuAction(tag);
        }

        //

        private void TeachingTip_ActionButtonClick(TeachingTip sender, object args)
        {
            var action = sender.ActionButtonCommandParameter as Action;
            action?.Invoke();

            sender.IsOpen = false;
        }

        private void TeachingTip_CloseButtonClick(TeachingTip sender, object args)
        {
            var action = sender.CloseButtonCommandParameter as Action;
            action?.Invoke();

            sender.IsOpen = false;
        }

        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            SetCurrentNavigationViewItem(PageNavigationView.MenuItems[0] as NavigationViewItem);
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            SetCurrentNavigationViewItem(args.SelectedItemContainer as NavigationViewItem);
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Minimize();
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            Maximize();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            Utils.DeleteFileOrDirectory(Meta.TEMP_DIR);
        }

        //

        public override void ShowInfoTeachingTip(object sender, string title = null, string subTitle = null)
        {
            if ((sender as FrameworkElement)?.Tag is not string tag) return;

            InfoTeachingTip.Target = sender as FrameworkElement;
            InfoTeachingTip.Title = title;
            InfoTeachingTip.Subtitle = subTitle;

            if (AppTypes.DOCS.Any(i => i.Key == tag))
            {
                InfoTeachingTip.ActionButtonContent = _resourceLoader.GetString("LearnMore");
                InfoTeachingTip.ActionButtonCommandParameter = () => { _ = Launcher.LaunchUriAsync(new(AppTypes.DOCS[tag])); };
            }
            else
                InfoTeachingTip.ActionButtonContent = null;

            InfoTeachingTip.PreferredPlacement = TeachingTipPlacementMode.Auto;
            InfoTeachingTip.IsOpen = true;
        }

        public override void ShowMessageTeachingTip(object sender, string title = null, string subTitle = null, Action action = null)
        {
            MessageTeachingTip.Target = sender as FrameworkElement;

            MessageTeachingTip.Title = title;
            MessageTeachingTip.Subtitle = subTitle;

            MessageTeachingTip.ActionButtonContent = _resourceLoader.GetString("Okay");
            MessageTeachingTip.ActionButtonCommandParameter = action;

            MessageTeachingTip.PreferredPlacement = TeachingTipPlacementMode.Auto;
            MessageTeachingTip.IsOpen = true;
        }

        public override void ShowConfirmTeachingTip(object sender, string title = null, string subTitle = null, Action confirmAction = null, Action closeAction = null)
        {
            ConfirmTeachingTip.Target = sender as FrameworkElement;

            ConfirmTeachingTip.Title = title;
            ConfirmTeachingTip.Subtitle = subTitle;

            ConfirmTeachingTip.ActionButtonCommandParameter = confirmAction;
            ConfirmTeachingTip.CloseButtonCommandParameter = closeAction;

            ConfirmTeachingTip.PreferredPlacement = TeachingTipPlacementMode.Auto;
            ConfirmTeachingTip.IsOpen = true;
        }

        public override void StatusBarLoading(bool visible)
        {
            ProcessingProgressRing.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            ProcessingProgressRing.IsActive = visible;

            Utils.SetThreadExecutionState(visible, false);
        }

        public override void StatusBarSetText(string text)
        {
            StatusTextBlock.Text = text;
        }

        public override void SetCurrentNavigationViewItem(NavigationViewItem item, object parameter = null)
        {
            if (item?.Tag is not string tag) return;

            PageNavigationView.SelectionChanged -= NavigationView_SelectionChanged;

            _ = ContentFrame.Navigate(Type.GetType(tag), parameter);
            PageNavigationView.Header = null;
            PageNavigationView.SelectedItem = item;

            PageNavigationView.SelectionChanged += NavigationView_SelectionChanged;
        }

        public override void SetCurrentNavigationViewItem(string tag, object parameter = null)
        {
            for (int i = 0; i < PageNavigationView.MenuItems.Count; i++)
            {
                if ((PageNavigationView.MenuItems[i] as FrameworkElement).Tag as string == tag)
                {
                    SetCurrentNavigationViewItem(PageNavigationView.MenuItems[i] as NavigationViewItem, parameter);
                    return;
                }
            }

            for (int i = 0; i < PageNavigationView.FooterMenuItems.Count; i++)
            {
                if ((PageNavigationView.FooterMenuItems[i] as FrameworkElement).Tag as string == tag)
                {
                    SetCurrentNavigationViewItem(PageNavigationView.FooterMenuItems[i] as NavigationViewItem, parameter);
                    return;
                }
            }
        }

        public override bool TryGoBack()
        {
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack();
                return true;
            }
            return false;
        }

        public override void EnableNavigationViewItems(bool isEnabled)
        {
            for (int i = 0; i < PageNavigationView.MenuItems.Count; i++)
                (PageNavigationView.MenuItems[i] as Control).IsEnabled = isEnabled;

            for (int i = 0; i < PageNavigationView.FooterMenuItems.Count; i++)
                (PageNavigationView.FooterMenuItems[i] as Control).IsEnabled = isEnabled;
        }

        public override void VisibleNavigationPane(bool isVisible)
        {
            PageNavigationView.IsPaneVisible = isVisible;
        }
    }
}
