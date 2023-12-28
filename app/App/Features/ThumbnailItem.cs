using ImageMagick;
using ImageMagick.Formats;
using Microsoft.UI.Xaml.Media.Imaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using IOCore.Libs;
using IOCore;
using System.Drawing;
using OpenCvSharp;

namespace IOApp.Features
{
    internal class ThumbnailItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private static readonly ResourceLoader _resourceLoader = ResourceLoader.GetForViewIndependentUse();

        public enum StatusType
        {
            Ready,
            ProcessInQueue,
            ProcessStart,
            Processing,
            Processed,
            ProcessFailed,
            ProcessPaused,
            ProcessStopped
        };

        public static readonly Dictionary<StatusType, string> STATUSES = new()
		{
            { StatusType.Ready, "Item_StatusReady" },
            { StatusType.ProcessInQueue, "Item_StatusProcessInQueue"},
            { StatusType.ProcessStart, "Item_StatusProcessStart"},
            { StatusType.Processing, "Item_StatusProcessing"},
            { StatusType.Processed, "Item_StatusProcessed"},
            { StatusType.ProcessFailed, "Item_StatusProcessFailed"},
            { StatusType.ProcessPaused, "Item_StatusProcessPaused"},
            { StatusType.ProcessStopped, "Item_StatusProcessStopped"}
        };

        [JsonIgnore]
        public ImageInfo InputInfo { get; set; }

        private int _position;
        [JsonIgnore]
        public int Position { get => _position; set { _position = value; PropertyChanged?.Invoke(this, new(nameof(Position))); } }

        public string InputFilePath { get => InputInfo.Path; set { InputInfo = new(value); PropertyChanged?.Invoke(this, new(nameof(InputFilePath))); } }

        [JsonIgnore]
        public BitmapImage Thumbnail { get; private set; }

        public BitmapImage OutputThumbnail { get; set; }

        public Mat SourceImage { get; set; }
        public Mat InpaintedImage { get; set; }
        public Mat Mask { get; set; }

        [JsonIgnore]
        public string CacheImagePath { get; private set; }

        private int _cacheImageLevel = 0;

        [JsonIgnore]
        public string TmpFilterPath { get; private set; }

        [JsonIgnore]
        public int ThumbnailWidth { get => 160; }
        [JsonIgnore]
        public int ThumbnailHeight { get => Utils.Round((double)InputInfo.Height.GetValueOrDefault(160) / InputInfo.Width.GetValueOrDefault(160) * ThumbnailWidth); }

        [JsonIgnore]
        public string InputMimeType => InputInfo.MimeType;
        [JsonIgnore]
        public long InputSize => InputInfo.FileSize;
        [JsonIgnore]
        public string InputDimensionText => InputInfo.DimensionText;
        [JsonIgnore]
        public int InputFrameCount => InputInfo.FrameCount ?? 0;

        private StatusType _status;
        [JsonIgnore]
        public StatusType Status
        {
            get => _status;
            set
            {
                _status = value;
                PropertyChanged?.Invoke(this, new(nameof(Status)));
                PropertyChanged?.Invoke(this, new(nameof(StatusText)));
            }
        }

        [JsonIgnore]
        public string StatusText => _resourceLoader.GetString(STATUSES[_status]);

        private bool _isEnabled;
        [JsonIgnore]
        public bool IsEnabled { get => _isEnabled; set { _isEnabled = value; PropertyChanged?.Invoke(this, new(nameof(IsEnabled))); } }

        public ThumbnailItem()
        {
            _isEnabled = true;

            Status = StatusType.Ready;
        }

        public void LoadBasicInfoIfNotExist()
        {
            if (InputInfo.FileInfo != null) return;

            try
            {
                InputInfo.LoadFormatAndDimentionInfo();
                InputInfo.LoadInfo();
                LoadThumbnailIfNotExist();
            }
            catch
            {
                throw;
            }
        }

        private void LoadThumbnailIfNotExist(int maxWidth = 160)
        {
            if (Thumbnail != null) return;

            var memoryStream = new MemoryStream();

            IProgress<bool> progress = new Progress<bool>(result =>
            {
                if (result)
                {
                    try
                    {
                        Thumbnail = new BitmapImage();
                        Thumbnail.SetSource(memoryStream.AsRandomAccessStream());
                        memoryStream.Dispose();

                        PropertyChanged?.Invoke(this, new(nameof(Thumbnail)));
                    }
                    catch { }
                } 
            });

            _ = Task.Run(() =>
            {
                try
                {
                    var maxHeight = Utils.Round((double)InputInfo.Height.GetValueOrDefault(1) / InputInfo.Width.GetValueOrDefault(1) * maxWidth);

                    if (InputInfo.IsRaw)
                    {
                        var imageMeta = new MagickImage();
                        imageMeta.Settings.SetDefines(new DngReadDefines { ReadThumbnail = true });
                        imageMeta.Ping(InputInfo.Path);

                        var profile = imageMeta.GetProfile("dng:thumbnail");
                        MagickImage image = null;

                        try
                        {
                            image = new MagickImage(profile?.GetData());
                        }
                        catch
                        {
                            image = new MagickImage(InputInfo.Path, new MagickReadSettings()
                            {
                                ColorSpace = ColorSpace.sRGB,
                                Defines = new DngReadDefines()
                                {
                                    ReadThumbnail = false,
                                    DisableAutoBrightness = true,
                                    UseCameraWhitebalance = true,
                                    UseAutoWhitebalance = false
                                },
                            });
                        }

                        ImageMagickUtils.AutoOrient(image);
                        image.Thumbnail(maxWidth, maxHeight);

                        image.Write(memoryStream, MagickFormat.Bmp);
                        memoryStream.Position = 0;

                        image.Dispose();
                    }
                    else
                    {
                        var readSettings = new MagickReadSettings()
                        {
                            Format = InputInfo.Format,
                            BackgroundColor = MagickColors.Transparent,
                            ColorSpace = ColorSpace.sRGB
                        };

                        if (ImageMagickUtils.IsVectorFamily(InputInfo.Format))
                            readSettings.Density = new(Utils.ComputeNeededDensityForSvg(InputInfo.Width.GetValueOrDefault(1), InputInfo.Height.GetValueOrDefault(1), maxWidth, maxHeight));

                        var image = new MagickImage(InputInfo.Path, readSettings);

                        ImageMagickUtils.AutoOrient(image);
                        image.Thumbnail(maxWidth, maxHeight);

                        image.Write(memoryStream, MagickFormat.Bmp);
                        memoryStream.Position = 0;

                        image.Dispose();
                    }

                    progress.Report(true);
                }
                catch
                {
                    progress.Report(false);
                }
            });
        }

        public void LoadImageCacheIfNotExist(bool useRawLevel1, IProgress<int> progress)
        {
            _ = Task.Run(() =>
            {
                try
                {
                    Directory.CreateDirectory(Meta.TEMP_DIR);

                    if (InputInfo.IsRaw)
                    {
                        if (CacheImagePath == null || !Utils.IsExistFileOrDirectory(CacheImagePath))
                        {
                            MagickImage image = null;

                            var filePath = Path.Join(Meta.TEMP_DIR, Guid.NewGuid().ToString() + ".bmp");

                            if (useRawLevel1)
                            {
                                var metaImage = new MagickImage();
                                metaImage.Settings.SetDefines(new DngReadDefines { ReadThumbnail = true });
                                metaImage.Ping(InputInfo.Path);

                                var profile = metaImage.GetProfile("dng:thumbnail");

                                try
                                {
                                    image = new MagickImage(profile?.GetData());
                                    ImageMagickUtils.AutoOrient(image);

                                    image.Write(filePath, MagickFormat.Bmp);
                                    image.Dispose();

                                    CacheImagePath = filePath;
                                    _cacheImageLevel = 1;

                                    progress.Report(_cacheImageLevel);
                                }
                                catch { }
                            }

                            image = new MagickImage(InputInfo.Path, new MagickReadSettings()
                            {
                                ColorSpace = ColorSpace.sRGB,
                                Defines = new DngReadDefines()
                                {
                                    ReadThumbnail = false,
                                    DisableAutoBrightness = true,
                                    UseCameraWhitebalance = true,
                                    UseAutoWhitebalance = false
                                },
                            });

                            ImageMagickUtils.AutoOrient(image);

                            image.Write(filePath, MagickFormat.Bmp);
                            image.Dispose();

                            CacheImagePath = filePath;
                            _cacheImageLevel = 2;
                        }

                        progress.Report(_cacheImageLevel);
                    }
                    else
                    {
                        if (CacheImagePath == null || !Utils.IsExistFileOrDirectory(CacheImagePath))
                        {
                            MagickImage image;

                            var readSettings = new MagickReadSettings()
                            {
                                Format = InputInfo.Format,
                                BackgroundColor = MagickColors.Transparent,
                                ColorSpace = ColorSpace.sRGB
                            };

                            if (ImageMagickUtils.IsVectorFamily(InputInfo.Format))
                                readSettings.Density = new(Utils.ComputeNeededDensityForSvg(InputInfo.Width.GetValueOrDefault(1), InputInfo.Height.GetValueOrDefault(1), 2048, 2048));

                            image = new MagickImage(InputFilePath, readSettings);
                            ImageMagickUtils.AutoOrient(image);

                            var filePath = Path.Join(Meta.TEMP_DIR, Guid.NewGuid().ToString() + ".bmp");

                            image.Write(filePath, MagickFormat.Bmp);
                            image.Dispose();

                            CacheImagePath = filePath;
                            _cacheImageLevel = 2;
                        }

                        progress.Report(_cacheImageLevel);
                    }
                }
                catch 
                {
                    progress.Report(_cacheImageLevel);
                }
            });
        }

        public void GenerateFilterFileIfNotExist(IProgress<bool> progress)
        {
            _ = Task.Run(() =>
            {
                try
                {
                    Directory.CreateDirectory(Meta.FILTER_DIR);

                    if (TmpFilterPath == null || !Utils.IsExistFileOrDirectory(TmpFilterPath))
                    {
                        MagickImage image;

                        var readSettings = new MagickReadSettings()
                        {
                            Format = InputInfo.Format,
                            BackgroundColor = MagickColors.Transparent,
                            ColorSpace = ColorSpace.sRGB
                        };

                        if (ImageMagickUtils.IsVectorFamily(InputInfo.Format))
                            readSettings.Density = new(Utils.ComputeNeededDensityForSvg(InputInfo.Width.GetValueOrDefault(1), InputInfo.Height.GetValueOrDefault(1), 2048, 2048));

                        image = new MagickImage(InputFilePath, readSettings);
                        ImageMagickUtils.AutoOrient(image);

                        var filePath = Path.Join(Meta.FILTER_DIR, Guid.NewGuid().ToString() + ".bmp");

                        image.Write(filePath, MagickFormat.Bmp);
                        image.Dispose();

                        TmpFilterPath = filePath;
                    }

                    progress.Report(true);
                }
                catch
                {
                    progress.Report(false);
                }
            });
        }

        public void Dispose()
        {
            Thumbnail = null;
            GC.Collect();
        }
    }
}