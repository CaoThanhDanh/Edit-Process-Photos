using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using System;
using System.ComponentModel;
using Windows.System;
using Windows.ApplicationModel.Resources;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IOApp.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MissingLib : Page, INotifyPropertyChanged
    {
        public static MissingLib Inst { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private static readonly ResourceLoader _resourceLoader = ResourceLoader.GetForViewIndependentUse();

        private ContentDialog _dialog;

        public MissingLib(ContentDialog dialog)
        {
            InitializeComponent();
            Inst = this;
            DataContext = this;

            _dialog = dialog;
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement).Tag is not string tag) return;

            if (tag == "Download")
            {
                _ = Launcher.LaunchUriAsync(new(
                        IntPtr.Size == 4 ?
                        "https://github.com/ArtifexSoftware/ghostpdl-downloads/releases/download/gs10011/gs10011w32.exe" :
                        "https://github.com/ArtifexSoftware/ghostpdl-downloads/releases/download/gs10011/gs10011w64.exe")
                    );
            }
            else if (tag == "Restart")
                AppInstance.Restart(string.Empty);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _dialog.Hide();
        }
    }
}