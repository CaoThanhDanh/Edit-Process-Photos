using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel;
using RestSharp;
using IOCore.Libs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IOCore.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Support : Page, INotifyPropertyChanged
    {
        public static Support Inst { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private static readonly ResourceLoader _resourceLoader = ResourceLoader.GetForViewIndependentUse();

        private ContentDialog _dialog;

        private string _appName;
        public string AppName { get => _appName; set { _appName = value; PropertyChanged?.Invoke(this, new(nameof(AppName))); } }

        private string _supportEmail;
        public string SupportEmail { get => _supportEmail; set { _supportEmail = value; PropertyChanged?.Invoke(this, new(nameof(SupportEmail))); } }

        public Support(ContentDialog dialog)
        {
            InitializeComponent();
            Inst = this;
            DataContext = this;

            _dialog = dialog;

            AppName = Package.Current.DisplayName;

            SupportEmail = Meta.IO_EMAIL;
        }

        public void ClearAll()
        {
            EmailTextBox.Text = string.Empty;
            DescriptionTextBox.Text = string.Empty;
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            var obj = new
            {
                name = Package.Current.DisplayName,
                phone = "N/A",
                email = EmailTextBox.Text.Replace(" ", "").ToLowerInvariant(),
                message = DescriptionTextBox.Text.Trim(' '),
                source = "app",
                domain = Package.Current.PublisherDisplayName,
                platform = "windows",
                extra = "Support",
                utcOffset = (int)TimeZoneInfo.Local.BaseUtcOffset.TotalSeconds
            };

            if (string.IsNullOrWhiteSpace(obj.email) || string.IsNullOrWhiteSpace(obj.message))
            {
                IOWindow.Inst.ShowMessageTeachingTip(null, _resourceLoader.GetString("Support_RequireAllInfo"));
                return;
            }

            if (!Utils.IsValidEmail(obj.email))
            {
                IOWindow.Inst.ShowMessageTeachingTip(null, _resourceLoader.GetString("Support_InvalidEmailAddress"));
                return;
            }

            try
            {
                SendButton.IsEnabled = false;

                var client = new RestClient(Meta.URL_IO_API_CONTACT);
                var request = new RestRequest();
                request.AddBody(obj);

                var resp = await client.PostAsync(request);

                if (resp.IsSuccessStatusCode)
                {
                    IOWindow.Inst.ShowMessageTeachingTip(null, _resourceLoader.GetString("Support_SendSuccess"), _resourceLoader.GetString("Support_SendSuccessDescription"));
                    ClearAll();
                    _dialog.Hide();
                }
                else throw new();
            }
            catch
            {
                IOWindow.Inst.ShowMessageTeachingTip(null, _resourceLoader.GetString("Support_SendFailure"), string.Format(_resourceLoader.GetString("Support_SendFailureDescription"), Meta.IO_EMAIL));
            }
            finally
            {
                SendButton.IsEnabled = true;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _dialog.Hide();
        }
    }
}