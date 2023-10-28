using Microsoft.UI.Xaml.Controls;
using System.IO;
using IOCore.Libs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IOCore.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LicenseInformation : Page
    {
        public static LicenseInformation Inst { get; private set; }

        private ContentDialog _dialog;

        public LicenseInformation(ContentDialog dialog)
        {
            InitializeComponent();
            Inst = this;
            DataContext = this;

            _dialog = dialog;

            LicenseTextBlock.Text = File.ReadAllText(Path.Combine(Utils.GetAssetsFolderPath(), "license.txt"));
        }

        private void CloseButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            _dialog.Hide();
        }
    }
}