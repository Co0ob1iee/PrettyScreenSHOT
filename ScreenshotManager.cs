using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.IO;
using System.Linq;

namespace PrettyScreenSHOT
{
    public class ScreenshotManager
    {
        private static readonly ScreenshotManager instance = new();
        public static ScreenshotManager Instance => instance;

        public ObservableCollection<ScreenshotItem> History { get; } = new();
        private readonly string historyDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            "PrettyScreenSHOT");

        private ScreenshotManager()
        {
            if (!Directory.Exists(historyDirectory))
                Directory.CreateDirectory(historyDirectory);

            LoadHistory();
        }

        public void AddScreenshot(BitmapSource bitmap)
        {
            var timestamp = DateTime.Now;
            var filename = $"Screenshot_{timestamp:yyyy-MM-dd_HH-mm-ss-fff}.png";
            var filepath = Path.Combine(historyDirectory, filename);

            SaveBitmapToFile(bitmap, filepath);

            var item = new ScreenshotItem
            {
                Timestamp = timestamp,
                Filename = filename,
                FilePath = filepath,
                Thumbnail = CreateThumbnail(bitmap)
            };

            History.Insert(0, item);
            DebugHelper.LogDebug($"Screenshot dodany: {filename}");
        }

        private void SaveBitmapToFile(BitmapSource bitmap, string filepath)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (var fileStream = new FileStream(filepath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        private BitmapSource CreateThumbnail(BitmapSource source)
        {
            const int thumbSize = 80;
            var scaleTransform = new System.Windows.Media.ScaleTransform(
                thumbSize / source.PixelWidth,
                thumbSize / source.PixelHeight);

            var transformedBitmap = new TransformedBitmap(source, scaleTransform);
            transformedBitmap.Freeze();
            return transformedBitmap;
        }

        private void LoadHistory()
        {
            if (!Directory.Exists(historyDirectory))
                return;

            var files = Directory.GetFiles(historyDirectory, "Screenshot_*.png")
                .OrderByDescending(f => File.GetLastWriteTime(f))
                .Take(20);

            foreach (var file in files)
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(file);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    var item = new ScreenshotItem
                    {
                        Timestamp = File.GetLastWriteTime(file),
                        Filename = Path.GetFileName(file),
                        FilePath = file,
                        Thumbnail = CreateThumbnail(bitmap)
                    };

                    History.Add(item);
                }
                catch { }
            }
        }

        public void DeleteScreenshot(ScreenshotItem item)
        {
            try
            {
                if (File.Exists(item.FilePath))
                    File.Delete(item.FilePath);

                History.Remove(item);
            }
            catch (Exception ex)
            {
                DebugHelper.LogDebug($"B³¹d usuwania: {ex.Message}");
            }
        }
    }

    public class ScreenshotItem
    {
        public DateTime Timestamp { get; set; }
        public string Filename { get; set; } = "";
        public string FilePath { get; set; } = "";
        public BitmapSource? Thumbnail { get; set; }
    }
}
