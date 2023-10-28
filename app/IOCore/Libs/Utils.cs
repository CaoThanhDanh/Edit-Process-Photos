using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel;
using System.Threading;
using Windows.Graphics;
using Windows.ApplicationModel.Activation;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.System.Power;
using Microsoft.UI.Xaml.Controls;

namespace IOCore.Libs
{
    public static class Utils
    {
        public static readonly Random Random = new();

        public static class Shutdown
        {
            public enum Types
            {
                Restart,
                Shutdown,
                CancelShutdown
            }

            public static void Run(Types type)
            {
                string argument = null;

                if (type == Types.Restart)
                    argument = "/r";
                else if (type == Types.Shutdown)
                    argument = "/s";
                else if (type == Types.CancelShutdown)
                    argument = "/a";

                if (!string.IsNullOrWhiteSpace(argument))
                    Process.Start("ShutDown", argument);
            }
        }

        public class Debouncer : IDisposable
        {
            private CancellationTokenSource _cancellationTokenSource;

            public Debouncer() { }

            public void Debounce(Action action, int miliseconds)
            {
                Dispose();

                _cancellationTokenSource = new();

                Task.Delay(miliseconds).ContinueWith(task => action?.Invoke(), _cancellationTokenSource.Token);
            }

            public void Dispose()
            {
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                }
            }

            ~Debouncer()
            {
                Dispose();
            }
        }

        public static string GetAssetsFolderPath()
        {
            var exeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var exeFolderPath = Path.GetDirectoryName(exeFilePath);
            return Path.Combine(exeFolderPath, "Assets");
        }

        public static string[] GetReadableByteSize(long size)
        {
            var prefix = string.Empty;

            if (size < 0)
            {
                prefix = "-";
                size = -size;
            }

            string[] sizes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            var len = (double)size;
            var order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                len /= 1024;
                order++;
            }

            return new string[] { prefix, string.Format("{0:0.##}", len), sizes[order] };
        }

        public static string GetReadableByteSizeText(long size)
        {
            var sizes = GetReadableByteSize(size);
            return $"{sizes[0]}{sizes[1]} {sizes[2]}";
        }

        public static long? GetFileSizeFromFilePath(string filePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                return null;
            else if (File.Exists(filePath))
                return (new FileInfo(filePath)).Length;

            return null;
        }

        public static bool IsExistFileOrDirectory(string path)
        {
            return Directory.Exists(path) || File.Exists(path);
        }

        public static void CreateDirectoryIfNotExist(string path)
        {
            if (!Directory.Exists(path))
                _ = Directory.CreateDirectory(path);
        }

        public static void DeleteFileOrDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;

            var isFile = IsFilePath(path);
            if (isFile != null)
            {
                try
                {
                    if (isFile.Value) File.Delete(path);
                    else Directory.Delete(path, true);
                }
                catch { }
            }
        }

        public static string TruncateAtWord(string text, int length)
        {
            if (text == null || text.Length < length) return text;

            var nextSpace = text.LastIndexOf(" ", length);
            return string.Format("{0}...", text[..((nextSpace > 0) ? nextSpace : length)].Trim());
        }

        public static void SetWindowSize(IntPtr hWnd, int width, int height)
        {
            var HWND = new HWND(hWnd);
            var scalingFactor = PInvoke.GetDpiForWindow(HWND) / 96.0;

            width = Round(width * scalingFactor);
            height = Round(height * scalingFactor);

            PInvoke.SetWindowPos(HWND, new(IntPtr.Zero), 0, 0, width, height, SET_WINDOW_POS_FLAGS.SWP_NOMOVE);
        }

        //

        public static string NextAvailableFolderName(string path)
        {
            const string NUMBER_PATTERN = " ({0})";

            if (!Directory.Exists(path))
                return path;

            return GetNextFolderName(path + NUMBER_PATTERN);
        }

        private static string GetNextFolderName(string pattern)
        {
            var tmp = string.Format(pattern, 1);
            if (tmp == pattern)
                throw new ArgumentException("The pattern must include an index place-holder", nameof(pattern));

            if (!Directory.Exists(tmp)) return tmp;

            int min = 1, max = 2; // min is inclusive, max is exclusive/untested

            while (Directory.Exists(string.Format(pattern, max)))
            {
                min = max;
                max *= 2;
            }

            while (max != min + 1)
            {
                var pivot = (max + min) / 2;
                if (Directory.Exists(string.Format(pattern, pivot)))
                    min = pivot;
                else
                    max = pivot;
            }

            return string.Format(pattern, max);
        }

        public static string NextAvailableFileName(string path)
        {
            const string NUMBER_PATTERN = " ({0})";

            if (!File.Exists(path))
                return path;

            if (Path.HasExtension(path))
                return GetNextFileName(path.Insert(path.LastIndexOf(Path.GetExtension(path)), NUMBER_PATTERN));

            return GetNextFileName(path + NUMBER_PATTERN);
        }

        private static string GetNextFileName(string pattern)
        {
            var tmp = string.Format(pattern, 1);
            if (tmp == pattern)
                throw new ArgumentException("The pattern must include an index place-holder", nameof(pattern));

            if (!File.Exists(tmp)) return tmp;

            int min = 1, max = 2; // min is inclusive, max is exclusive/untested

            while (File.Exists(string.Format(pattern, max)))
            {
                min = max;
                max *= 2;
            }

            while (max != min + 1)
            {
                var pivot = (max + min) / 2;
                if (File.Exists(string.Format(pattern, pivot)))
                    min = pivot;
                else
                    max = pivot;
            }

            return string.Format(pattern, max);
        }

        //

        public static string NextAvailableFolderNameAdvanced(string path)
        {
            const string NUMBER_PATTERN = " ({0})";

            if (!Directory.Exists(path) && !File.Exists(path))
                return path;

            return GetNextFolderNameAdvanced(path + NUMBER_PATTERN);
        }

        private static string GetNextFolderNameAdvanced(string pattern)
        {
            var tmp = string.Format(pattern, 1);
            if (tmp == pattern)
                throw new ArgumentException("The pattern must include an index place-holder", nameof(pattern));

            if (!Directory.Exists(tmp) && !File.Exists(tmp))
                return tmp;

            int min = 1, max = 2; // min is inclusive, max is exclusive/untested

            while (Directory.Exists(string.Format(pattern, max)) ||
                   File.Exists(string.Format(pattern, max)))
            {
                min = max;
                max *= 2;
            }

            while (max != min + 1)
            {
                var pivot = (max + min) / 2;
                if (Directory.Exists(string.Format(pattern, pivot)) ||
                    File.Exists(string.Format(pattern, pivot)))
                    min = pivot;
                else
                    max = pivot;
            }

            return string.Format(pattern, max);
        }

        //

        public static string NextAvailableFileNameAdvanced(string path)
        {
            const string NUMBER_PATTERN = " ({0})";

            if (!File.Exists(path) && !Directory.Exists(path))
                return path;

            if (Path.HasExtension(path))
                return GetNextFileNameAdvanced(path.Insert(path.LastIndexOf(Path.GetExtension(path)), NUMBER_PATTERN));

            return GetNextFileNameAdvanced(path + NUMBER_PATTERN);
        }

        private static string GetNextFileNameAdvanced(string pattern)
        {
            var tmp = string.Format(pattern, 1);
            if (tmp == pattern)
                throw new ArgumentException("The pattern must include an index place-holder", nameof(pattern));

            if (!File.Exists(tmp) && !Directory.Exists(tmp))
                return tmp;

            int min = 1, max = 2; // min is inclusive, max is exclusive/untested

            while (File.Exists(string.Format(pattern, max)) ||
                   Directory.Exists(string.Format(pattern, max)))
            {
                min = max;
                max *= 2;
            }

            while (max != min + 1)
            {
                var pivot = (max + min) / 2;

                if (File.Exists(string.Format(pattern, pivot)) ||
                    Directory.Exists(string.Format(pattern, pivot)))
                    min = pivot;
                else
                    max = pivot;
            }

            return string.Format(pattern, max);
        }

        public static bool? IsFilePath(string path)
        {
            if (!IsExistFileOrDirectory(path)) return null;
            return !File.GetAttributes(path).HasFlag(FileAttributes.Directory);
        }

        public static bool RevealInFileExplorer(string path)
        {
            if (!IsExistFileOrDirectory(path)) return false;
            Process.Start("explorer.exe", $"/select, \"{path}\"");
            return true;
        }

        public static bool OpenEntryWithDefaultApp(string entry)
        {
            var process = new Process()
            {
                StartInfo = new()
                {
                    FileName = "explorer.exe",
                    Arguments = entry,
                    WindowStyle = ProcessWindowStyle.Minimized
                }
            };

            return process.Start();
        }

        public static bool OpenFileWithDefaultApp(string filePath)
        {
            if (!IsExistFileOrDirectory(filePath)) return false;
            return OpenEntryWithDefaultApp(filePath);
        }

        public static bool RunFile(string path, string arguments)
        {
            if (!IsExistFileOrDirectory(path)) return false;

            var process = new Process()
            {
                StartInfo = new()
                {
                    FileName = path,
                    WindowStyle = ProcessWindowStyle.Minimized
                }
            };

            if (!string.IsNullOrWhiteSpace(arguments))
                process.StartInfo.Arguments = arguments;

            return process.Start();
        }

        //

        public static bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult);
        }

        //

        public static float GetContainScaleRatio(float width, float height, float maxWidth, float maxHeight)
        {
            if (width > maxWidth || height > maxHeight)
            {
                if (width / maxWidth > height / maxHeight)
                    return width / maxWidth;
                return height / maxHeight;
            }

            return 1.0f;
        }

        public static int GetContainScaleRatio(int width, int height, int maxWidth, int maxHeight)
        {
            var ratio = GetContainScaleRatio(width, height, maxWidth, maxHeight);
            return Round(ratio);
        }

        //

        public static SizeF GetMaxContainSize(float width, float height, float containerWidth, float containerHeight)
        {
            var size = new SizeF(width, height);

            if (width > containerWidth || height > containerHeight)
            {
                if (width / containerWidth > height / containerHeight)
                {
                    var ratio = containerWidth / width;

                    size.Width = containerWidth;
                    size.Height = height * ratio;
                }
                else
                {
                    var ratio = containerHeight / height;

                    size.Width = width * ratio;
                    size.Height = containerHeight;
                }
            }

            return size;
        }

        public static Size GetMaxContainSize(int width, int height, int containerWidth, int containerHeight)
        {
            SizeF size = GetMaxContainSize(width, (float)height, containerWidth, containerHeight);
            return new(Round(size.Width), Round(size.Height));
        }

        //

        public static SizeInt32 GetContainSize(double width, double height, double containerWidth, double containerHeight)
        {
            var imageRatio = height / width;
            var containerRatio = containerHeight / containerWidth;

            return imageRatio > containerRatio ?
                new SizeInt32 { Width = Round(width * containerHeight / height), Height = Round(containerHeight) } :
                new SizeInt32 { Width = Round(containerWidth), Height = Round(height * containerWidth / width) };
        }

        public static SizeF GetContainSize(float width, float height, float containerWidth, float containerHeight)
        {
            var sizeD = GetContainSize(width, height, containerWidth, (double)containerHeight);
            return new((float)sizeD.Width, (float)sizeD.Height);
        }


        public static Size GetContainSize(int width, int height, int containerWidth, int containerHeight)
        {
            var sizeD = GetContainSize(width, height, containerWidth, (double)containerHeight);
            return new(Round(sizeD.Width), Round(sizeD.Height));
        }

        //

        public static SizeF GetCoverSize(float width, float height, float containerWidth, float containerHeight)
        {
            var imageRatio = height / width;
            var containerRatio = containerHeight / containerWidth;

            return imageRatio < containerRatio ?
                new SizeF(width * containerHeight / height, containerHeight) :
                new SizeF(containerWidth, height * containerWidth / width);
        }

        public static Size GetCoverSize(int width, int height, int containerWidth, int containerHeight)
        {
            var size = GetCoverSize(width, height, containerWidth, (float)containerHeight);
            return new(Round(size.Width), Round(size.Height));
        }

        public static int Round(double value)
        {
            return (int)(value + 0.5);
        }

        public static Size GetPrimaryMonitorResolution()
        {
            return new Size(
                PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXVIRTUALSCREEN),
                PInvoke.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CYVIRTUALSCREEN)
                );
        }

        public static BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            var bitmapImage = new BitmapImage();
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);
                stream.Position = 0;
                bitmapImage.SetSource(stream.AsRandomAccessStream());
            }

            return bitmapImage;
        }

        public static BitmapImage LoadBitmapImageFromFile(string filePath)
        {
            using var bitmap = new Bitmap(filePath, true);
            return ConvertBitmapToBitmapImage(bitmap);

        }

        public static async Task<InMemoryRandomAccessStream> ConvertWriteableBitmapSourceToStream(WriteableBitmap source)
        {
            var stream = new InMemoryRandomAccessStream();

            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);

            var pixelStream = source.PixelBuffer.AsStream();
            var pixels = new byte[pixelStream.Length];

            await pixelStream.ReadAsync(pixels);

            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)source.PixelWidth, (uint)source.PixelHeight, 96.0, 96.0, pixels);
            await encoder.FlushAsync();

            return stream;
        }

        public static int ComputeNeededDensityForSvg(int svgWidth, int svgHeight, int width, int height)
        {
            var scaleWidth = (float)width / svgWidth;
            var scaleHeight = (float)height / svgHeight;

            return scaleWidth > scaleHeight ? 72 * ((int)scaleWidth + 1) : 72 * ((int)scaleHeight + 1);
        }

        public static int MinMax(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        public static int GetAdjustedValue(int value, int valueMax, int adjustedMin, int adjustedMax)
        {
            return MinMax(Round((double)adjustedMax * value / valueMax), adjustedMin, adjustedMax);
        }

        public static string GetNumberInText(string text)
        {
            if (text == null) return string.Empty;
            return new string(text.Where(c => char.IsDigit(c)).ToArray());
        }

        public static T GetValueOrDefault<T>(string text, T value)
        {
            try { return (T)Convert.ChangeType(text, typeof(T)); }
            catch { return value; }
        }

        public static string GetValidFileName(string arbitraryText)
        {
            var invalidFileNameChars = Path.GetInvalidFileNameChars();

            foreach (char c in invalidFileNameChars)
                arbitraryText = arbitraryText.Replace(c, '_');
            return arbitraryText;
        }

        public static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child); // get parent item

            if (parentObject == null) return null; // reached the end of the tree

            T parent = parentObject as T; // check if the parent matches the type we’re looking for

            if (parent != null)
                return parent;
            else
                return FindVisualParent<T>(parentObject); // use recursion to proceed with next level
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                var childType = child as T;

                if (childType != null)
                    yield return (T)child;

                foreach (var other in FindVisualChildren<T>(child))
                    yield return other;
            }
        }

        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null; // confirm parent and childName are valid

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                var childObject = VisualTreeHelper.GetChild(parent, i);

                T child = childObject as T;

                if (child == null) // if the child is not of the request child type child
                {
                    foundChild = FindChild<T>(childObject, childName); // recursively drill down the tree

                    if (foundChild != null) // if the child is found, break so we do not overwrite the found child
                        break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = childObject as FrameworkElement;

                    if (frameworkElement != null && frameworkElement.Name == childName) // if the child's name is set for search
                    {
                        foundChild = (T)childObject; // if the child's name is of the request name
                        break;
                    }
                }
                else
                {
                    foundChild = (T)childObject; // child element found
                    break;
                }
            }

            return foundChild;
        }

        public static T FindChildByNameAndTag<T>(DependencyObject parent, string childName, string childTag) where T : DependencyObject
        {
            if (parent == null) return null; // confirm parent and childName are valid

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                var childObject = VisualTreeHelper.GetChild(parent, i);

                T child = childObject as T;

                if (child == null) // if the child is not of the request child type child
                {
                    foundChild = FindChildByNameAndTag<T>(childObject, childName, childTag); // recursively drill down the tree

                    if (foundChild != null) // if the child is found, break so we do not overwrite the found child
                        break;
                }
                else if (!string.IsNullOrEmpty(childName) && !string.IsNullOrEmpty(childTag))
                {
                    var fe = childObject as FrameworkElement;

                    if (fe != null && fe.Name == childName && (string)fe.Tag == childTag) // if the child's name is set for search
                    {
                        foundChild = (T)childObject; // if the child's name is of the request name
                        break;
                    }
                }
                else
                {
                    foundChild = (T)childObject; // child element found
                    break;
                }
            }

            return foundChild;
        }

        public static object GetProperty(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }

        public static bool Any<T>(T value, params T[] values)
        {
            return values.Any(v => Comparer<T>.Default.Compare(v, value) == 0);
        }

        public static bool Every<T>(T value, params T[] values)
        {
            return !values.Any(v => Comparer<T>.Default.Compare(v, value) != 0);
        }

        public static bool Are<T>(IEnumerable<T> values1, IEnumerable<T> values2)
        {
            return values1.Count() == values2.Count() && values1.All(value => values2.Contains(value));
        }

        public static bool IsSame(double v1, double v2)
        {
            return Math.Abs(v1 - v2) <= Math.Abs(v1 * .00001);
        }

        public static string Serialize<T>(T t)
        {
            using var sw = new StringWriter();
            using var xw = XmlWriter.Create(sw);
            new XmlSerializer(typeof(T)).Serialize(xw, t);
            return sw.GetStringBuilder().ToString();
        }

        public static T Deserialize<T>(string s_xml)
        {
            using var xw = XmlReader.Create(new StringReader(s_xml));
            return (T)new XmlSerializer(typeof(T)).Deserialize(xw);
        }

        public static byte[] GetJsonByteArrayFromObject(object obj)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        }

        public static void RemoveLatestBytesFromFile(string path, long size)
        {
            using var fs = new FileInfo(path).Open(FileMode.Open);
            fs.SetLength(Math.Max(0, fs.Length - size));
        }

        public static bool IsValidEmail(string email)
        {
            var m = Regex.Match(email, @"@.+\.", RegexOptions.IgnoreCase);
            return m.Success;
        }

        public static void SetTextToClipboard(string text)
        {
            var dataPackage = new DataPackage
            {
                RequestedOperation = DataPackageOperation.Copy
            };

            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);
        }

        public static async Task<string> GetTextFromClipboard()
        {
            var dataPackageView = Clipboard.GetContent();

            if (dataPackageView.Contains(StandardDataFormats.Text))
                return await dataPackageView.GetTextAsync();

            return string.Empty;
        }

        public static int RandomInRange(int min, int max)
        {
            return Random.Next(max + 1 - min) + min;
        }

        public static bool IsStartupTask()
        {
            var activatedEventArgs = AppInstance.GetActivatedEventArgs();
            if (activatedEventArgs == null) return false;

            return AppInstance.GetActivatedEventArgs().Kind == ActivationKind.StartupTask;
        }

        public static async Task<bool?> IsRunOnStartupState(StartupTask startupTask = null)
        {
            if (startupTask == null)
                startupTask = await StartupTask.GetAsync(Package.Current.Id.Name);

            if (startupTask.State == StartupTaskState.Enabled)
                return true;
            else if (startupTask.State == StartupTaskState.Disabled)
                return false;
            else if (startupTask.State == StartupTaskState.DisabledByUser)
                return null;

            return null;
        }

        public static async Task<bool?> SetRunOnStartupState(bool isEnabled, StartupTask startupTask = null)
        {
            if (startupTask == null)
                startupTask = await StartupTask.GetAsync(Package.Current.Id.Name);

            var state = await IsRunOnStartupState(startupTask);
            if (state == null) return null;
            if (state.Value == isEnabled) return isEnabled;

            if (startupTask.State == StartupTaskState.Enabled)
            {
                if (!isEnabled)
                {
                    startupTask.Disable();
                    return false;
                }
            }
            else if (startupTask.State == StartupTaskState.Disabled)
            {
                if (isEnabled)
                {
                    await startupTask.RequestEnableAsync();
                    return true;
                }
            }
            else if (startupTask.State == StartupTaskState.DisabledByUser)
            {
                if (isEnabled)
                {
                    IOWindow.Inst.ShowMessageTeachingTip(null, "Unable to change state of startup task via the application", "Enable via Startup tab on Task Manager (Ctrl+Shift+Esc)");
                    return null;
                }
            }

            IOWindow.Inst.ShowMessageTeachingTip(null, "Unable to change state of startup task");
            return null;
        }

        public static void SetThreadExecutionState(bool keepPcAwake, bool keepScreenOn)
        {
            var states = EXECUTION_STATE.ES_CONTINUOUS;

            if (keepPcAwake)
                states |= EXECUTION_STATE.ES_SYSTEM_REQUIRED;

            if (keepScreenOn)
                states |= EXECUTION_STATE.ES_DISPLAY_REQUIRED;

            PInvoke.SetThreadExecutionState(states);
        }

        public static T ChangeType<T>(object value)
        {
            var t = typeof(T);

            try
            {
                if (t.IsEnum)
                {
                    var res = (T)Enum.Parse(typeof(T), value as string);
                    if (!Enum.IsDefined(typeof(T), res))
                        return default;
                    return res;
                }
                else
                {
                    if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        if (value == null)
                            return default;

                        t = Nullable.GetUnderlyingType(t);
                    }

                    return (T)Convert.ChangeType(value, t);
                }
            }
            catch
            {
                return default;
            }
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            var count = list.Count;
            while (count > 1)
            {
                count--;
                var index = Random.Next(count + 1);
                (list[count], list[index]) = (list[index], list[count]);
            }
        }

        public static MemoryStream LoadMemoryStreamFromFile(string filePath)
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            
            var memoryStream = new MemoryStream();
            fileStream.CopyTo(memoryStream);
            return memoryStream;
        }

        public static void SaveMemoryStreamToFile(string filePath, MemoryStream memoryStream)
        {
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            memoryStream.CopyTo(fileStream);
        }

        public static void LoseFocus(object sender)
        {
            if (sender is not Control control) return;

            var isTabStop = control.IsTabStop;
            var isEnabled = control.IsEnabled;

            control.IsTabStop = false;
            control.IsEnabled = false;

            control.IsTabStop = isTabStop;
            control.IsEnabled = isEnabled;
        }

        public static Color GetBWForegroundColor(Color backgroundColor)
        {
            return ((backgroundColor.R * 0.2126 / 255 + backgroundColor.G * 0.7152 / 255 + backgroundColor.B * 0.0722 / 255) < 0.5) ?
                Color.FromArgb(backgroundColor.A, 255, 255, 255) :
                Color.FromArgb(backgroundColor.A, 0, 0, 0);
        }
    }
}
