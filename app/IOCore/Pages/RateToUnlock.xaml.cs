using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using Microsoft.Windows.AppLifecycle;
using IOCore.Libs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IOCore.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    sealed partial class RateToUnlock : Page, INotifyPropertyChanged
    {
        public static RateToUnlock Inst { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private ContentDialog _dialog;
        private Action _purchaseAction;
        private Action _restoreAction;

        public RateToUnlock(ContentDialog dialog, Action purchaseAction, Action restoreAction)
        {
            InitializeComponent();
            Inst = this;
            DataContext = this;

            _dialog = dialog;

            _purchaseAction = purchaseAction;
            _restoreAction = restoreAction;
        }

        private async void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement).Tag is not string tag) return;

            if (tag == "RateToUnlock")
            {
                if (await AskForRate.Request(true, AskForRate.TimeTest))
                    AppInstance.Restart(string.Empty);
            }
            else if (tag == "PurchaseNow")
                _purchaseAction?.Invoke();
            else if (tag == "RestorePurchase")
                _restoreAction?.Invoke();
        }
    }
}
