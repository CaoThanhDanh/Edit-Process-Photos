using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.Storage.Pickers;
using Windows.ApplicationModel.Resources;
using ImageMagick;
using Windows.Win32;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using IOApp.Features;
using IOApp.Configs;
using IOCore.Libs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IOApp.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    sealed partial class SaveSettings : Page, INotifyPropertyChanged
    {
        public static SaveSettings Inst { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private static readonly ResourceLoader _resourceLoader = ResourceLoader.GetForViewIndependentUse();
        private Manage.ModeType _mode => Main.Inst.Mode;

        public ContentDialog _dialog;

        private Main.StatusType _status;
        public Main.StatusType Status { get => _status; set { _status = value; PropertyChanged?.Invoke(this, new(nameof(Status))); } }

        private ThumbnailItem _item;
        public ThumbnailItem Item
        {
            get => _item; 
            set 
            { 
                _item = value;

                WidthTextBox.TextChanging -= SizeTextBox_TextChanging;
                HeightTextBox.TextChanging -= SizeTextBox_TextChanging;

                WidthTextBox.Text = Item.InputInfo.Width.ToString();
                HeightTextBox.Text = Item.InputInfo.Height.ToString();

                WidthTextBox.TextChanging += SizeTextBox_TextChanging;
                HeightTextBox.TextChanging += SizeTextBox_TextChanging;

                ThumbnailImage.Source = Item.OutputThumbnail;

                PropertyChanged?.Invoke(this, new(nameof(Item))); 
            }
        }

        private FileSavePicker _outputFilePicker;

        public SaveSettings(ContentDialog dialog)
        {
            InitializeComponent();
            Inst = this;
            DataContext = this;

            _dialog = dialog;
        }

        #region WIDTH_HEIGHT_CONFIGS

        private void SizeTextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(i => !char.IsDigit(i));
        }

        private void SizeTextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            if (sender.Text == string.Empty) return;

            var width = 0;
            var height = 0;

            WidthTextBox.TextChanging -= SizeTextBox_TextChanging;
            HeightTextBox.TextChanging -= SizeTextBox_TextChanging;

            if (sender == WidthTextBox)
            {
                try
                {
                    width = Math.Max(int.Parse(sender.Text), 0);
                }
                catch (FormatException)
                {
                    sender.Text = width.ToString();
                }

                if (sender.Text.StartsWith("0"))
                    WidthTextBox.Text = width.ToString();

                HeightTextBox.Text = Math.Max(Utils.Round((double)Item.InputInfo.Height.GetValueOrDefault(1) / Item.InputInfo.Width.GetValueOrDefault(1) * width), 0).ToString();
            }
            else if (sender == HeightTextBox)
            {
                try
                {
                    height = Math.Max(int.Parse(sender.Text), 0);
                }
                catch (FormatException)
                {
                    HeightTextBox.Text = height.ToString();
                }

                if (sender.Text.StartsWith("0"))
                    HeightTextBox.Text = height.ToString();

                WidthTextBox.Text = Math.Max(Utils.Round((double)Item.InputInfo.Width.GetValueOrDefault(1) / Item.InputInfo.Height.GetValueOrDefault(1) * height), 0).ToString();
            }

            WidthTextBox.TextChanging += SizeTextBox_TextChanging;
            HeightTextBox.TextChanging += SizeTextBox_TextChanging;
        }

        private void SizeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox.Text == string.Empty || textBox.Text == "0")
            {
                WidthTextBox.Text = "1";
                HeightTextBox.Text = "1";
            }
        }

        #endregion

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_outputFilePicker == null)
            {
                _outputFilePicker = new() { SuggestedStartLocation = PickerLocationId.PicturesLibrary, SuggestedFileName = "Image" };
                WinRT.Interop.InitializeWithWindow.Initialize(_outputFilePicker, PInvoke.GetActiveWindow());

                _outputFilePicker.FileTypeChoices.Clear();
                foreach (var i in Profile.OUTPUT_IMAGE_FORMATS)
                    if (i.Value.IsSupported)
                        _outputFilePicker.FileTypeChoices.Add(i.Value.Name, new List<string>() { i.Value.Extension });
            }

            _outputFilePicker.SuggestedFileName = Path.GetFileNameWithoutExtension(Item.InputFilePath);

            var storageFile = await _outputFilePicker.PickSaveFileAsync();
            if (storageFile == null) return;

            Status = Main.StatusType.Processing;

            var width = Utils.GetValueOrDefault(WidthTextBox.Text, 1);
            var height = Utils.GetValueOrDefault(HeightTextBox.Text, 1);

            IProgress<Main.StatusType> progress = new Progress<Main.StatusType>((Main.StatusType status) =>
            {
                Utils.RevealInFileExplorer(storageFile.Path);
                Status = status;
                _dialog.Hide();

                _ = AskForRate.Request(true, AskForRate.TimeTest, true, 2);
            });

            if (_mode == Manage.ModeType.Inpaint)
            {
                _item.LoadImageCacheIfNotExist(false, new Progress<int>((int cacheImageLevel) =>
                {
                    _ = Task.Run(() =>
                    {
                        try
                        {
                            // INPAINT 
                            Cv2.Resize(_item.Mask, _item.Mask, _item.SourceImage.Size());
                            _item.InpaintedImage = new Mat();
                            Cv2.Inpaint(_item.SourceImage, _item.Mask, _item.InpaintedImage, 10, InpaintMethod.Telea);
                            var bitmap = BitmapConverter.ToBitmap(_item.InpaintedImage);

                            // BITMAP TO MAGICK IMAGE
                            MemoryStream memoryStream = new MemoryStream();
                            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                            memoryStream.Position = 0;
                            var image = new MagickImage(memoryStream);

                            // SAVE WITH CONFIG
                            ImageMagickUtils.AutoOrient(image);
                            image.Quality = 100;
                            image.Resize(width, height);
                            image.Write(storageFile.Path);
                            image.Dispose();
                            bitmap.Dispose();

                            progress.Report(Main.StatusType.Processed);
                        }
                        catch
                        {
                            progress.Report(Main.StatusType.ProcessFailed);
                        }
                    });
                }));
            }
            else if (_mode == Manage.ModeType.Filter) 
            {
                _ = Task.Run(() =>
                {
                    try
                    {
                        MagickImage image = new MagickImage(_item.TmpFilterPath);
                        MagickImage copiedImage = (MagickImage)image.Clone();
                        copiedImage.Write(storageFile.Path);
                        image.Dispose();
                        copiedImage.Dispose();

                        progress.Report(Main.StatusType.Processed);

                    }
                    catch 
                    {
                        progress.Report(Main.StatusType.ProcessFailed);
                    }
                });
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _dialog.Hide();
        }
    }
}