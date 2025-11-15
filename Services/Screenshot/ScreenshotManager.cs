using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.IO;
using System.Linq;
using PrettyScreenSHOT.Services.Settings;

namespace PrettyScreenSHOT.Services.Screenshot
{
    public class ScreenshotManager
    {
        private static readonly ScreenshotManager instance = new();
        public static ScreenshotManager Instance => instance;

        public ObservableCollection<ScreenshotItem> History { get; } = new();
        public BitmapSource? LastCapturedBitmap { get; set; }
        private string historyDirectory;

        private ScreenshotManager()
        {
            UpdateHistoryDirectory();
            LoadHistory();
        }

        private void UpdateHistoryDirectory()
        {
            historyDirectory = SettingsManager.Instance.SavePath;
            if (!Directory.Exists(historyDirectory))
            {
                Directory.CreateDirectory(historyDirectory);
            }
        }

        public void AddScreenshot(BitmapSource bitmap)
        {
            UpdateHistoryDirectory();
            
            // Optymalizacja wydajności - zmniejsz rozmiar jeśli potrzeba
            var optimizedBitmap = bitmap;
            if (SettingsManager.Instance.MaxImageWidth > 0 || SettingsManager.Instance.MaxImageHeight > 0)
            {
                optimizedBitmap = PerformanceOptimizer.OptimizeForSave(
                    bitmap, 
                    SettingsManager.Instance.MaxImageWidth, 
                    SettingsManager.Instance.MaxImageHeight);
            }

            // Bezpieczeństwo - usuń metadane jeśli włączone
            if (SettingsManager.Instance.RemoveMetadata)
            {
                optimizedBitmap = SecurityManager.RemoveMetadata(optimizedBitmap);
            }

            // Bezpieczeństwo - dodaj znak wodny jeśli włączony
            if (SettingsManager.Instance.EnableWatermark && !string.IsNullOrEmpty(SettingsManager.Instance.WatermarkText))
            {
                optimizedBitmap = SecurityManager.AddWatermark(
                    optimizedBitmap, 
                    SettingsManager.Instance.WatermarkText);
            }
            
            var timestamp = DateTime.Now;
            var format = SettingsManager.Instance.ImageFormat.ToLower();
            var extension = format switch
            {
                "jpg" or "jpeg" => "jpg",
                "bmp" => "bmp",
                _ => "png"
            };
            
            var filename = $"Screenshot_{timestamp:yyyy-MM-dd_HH-mm-ss-fff}.{extension}";
            var filepath = Path.Combine(historyDirectory, filename);

            SaveBitmapToFile(optimizedBitmap, filepath, format);

            // Szyfrowanie jeśli włączone
            if (SettingsManager.Instance.EnableEncryption)
            {
                var encryptedPath = filepath + ".encrypted";
                SecurityManager.EncryptScreenshot(filepath, encryptedPath);
                File.Delete(filepath); // Usuń niezaszyfrowaną wersję
                filepath = encryptedPath;
                filename = Path.GetFileName(encryptedPath);
            }

            var item = new ScreenshotItem
            {
                Timestamp = timestamp,
                Filename = filename,
                FilePath = filepath,
                Thumbnail = CreateThumbnail(bitmap)
            };

            LastCapturedBitmap = bitmap;

            History.Insert(0, item);
            DebugHelper.LogDebug($"Screenshot added: {filename}");
        }

        public ScreenshotItem AddScreenshotWithMetadata(BitmapSource bitmap, string category, List<string> tags, string? notes)
        {
            UpdateHistoryDirectory();
            
            // Optymalizacja wydajności - zmniejsz rozmiar jeśli potrzeba
            var optimizedBitmap = bitmap;
            if (SettingsManager.Instance.MaxImageWidth > 0 || SettingsManager.Instance.MaxImageHeight > 0)
            {
                optimizedBitmap = PerformanceOptimizer.OptimizeForSave(
                    bitmap, 
                    SettingsManager.Instance.MaxImageWidth, 
                    SettingsManager.Instance.MaxImageHeight);
            }

            // Bezpieczeństwo - usuń metadane jeśli włączone
            if (SettingsManager.Instance.RemoveMetadata)
            {
                optimizedBitmap = SecurityManager.RemoveMetadata(optimizedBitmap);
            }

            // Bezpieczeństwo - dodaj znak wodny jeśli włączony
            if (SettingsManager.Instance.EnableWatermark && !string.IsNullOrEmpty(SettingsManager.Instance.WatermarkText))
            {
                optimizedBitmap = SecurityManager.AddWatermark(
                    optimizedBitmap, 
                    SettingsManager.Instance.WatermarkText);
            }
            
            var timestamp = DateTime.Now;
            var format = SettingsManager.Instance.ImageFormat.ToLower();
            var extension = format switch
            {
                "jpg" or "jpeg" => "jpg",
                "bmp" => "bmp",
                _ => "png"
            };
            
            var filename = $"Screenshot_{timestamp:yyyy-MM-dd_HH-mm-ss-fff}.{extension}";
            var filepath = Path.Combine(historyDirectory, filename);

            SaveBitmapToFile(optimizedBitmap, filepath, format);

            // Szyfrowanie jeśli włączone
            if (SettingsManager.Instance.EnableEncryption)
            {
                var encryptedPath = filepath + ".encrypted";
                SecurityManager.EncryptScreenshot(filepath, encryptedPath);
                File.Delete(filepath); // Usuń niezaszyfrowaną wersję
                filepath = encryptedPath;
                filename = Path.GetFileName(encryptedPath);
            }

            var item = new ScreenshotItem
            {
                Timestamp = timestamp,
                Filename = filename,
                FilePath = filepath,
                Thumbnail = CreateThumbnail(bitmap),
                Category = category ?? "",
                Tags = tags ?? new List<string>(),
                Notes = notes
            };

            LastCapturedBitmap = bitmap;

            History.Insert(0, item);
            DebugHelper.LogDebug($"Screenshot added with metadata: {filename}");
            
            // Automatyczny upload jeśli włączony
            if (SettingsManager.Instance.AutoUpload)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var result = await CloudUploadManager.Instance.UploadScreenshotAsync(bitmap, filename);
                        if (result.Success && !string.IsNullOrEmpty(result.Url))
                        {
                            // Aktualizuj item w głównym wątku
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                item.CloudUrl = result.Url;
                                item.CloudProvider = result.ProviderName;
                            });

                            DebugHelper.LogInfo("ScreenshotManager", $"Auto-upload successful: {result.Url}");
                            
                            if (TrayIconManager.Instance != null && SettingsManager.Instance.ShowNotifications)
                            {
                                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                                {
                                    TrayIconManager.Instance.ShowNotification(
                                        "Upload Complete",
                                        $"Screenshot uploaded to {result.ProviderName}. URL copied to clipboard.");
                                });
                                System.Windows.Clipboard.SetText(result.Url);
                            }
                        }
                        else
                        {
                            DebugHelper.LogError("ScreenshotManager", $"Auto-upload failed: {result.ErrorMessage}");
                        }
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.LogError("ScreenshotManager", "Auto-upload exception", ex);
                    }
                });
            }
            
            return item;
        }

        private void SaveBitmapToFile(BitmapSource bitmap, string filepath, string format)
        {
            BitmapEncoder encoder = format switch
            {
                "jpg" or "jpeg" => new JpegBitmapEncoder 
                { 
                    QualityLevel = SettingsManager.Instance.ImageQuality 
                },
                "bmp" => new BmpBitmapEncoder(),
                _ => new PngBitmapEncoder()
            };

            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (var fileStream = new FileStream(filepath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        private BitmapSource CreateThumbnail(BitmapSource source)
        {
            // Użyj PerformanceOptimizer dla lepszej wydajności
            string cacheKey = source.GetHashCode().ToString();
            return PerformanceOptimizer.GetOrCreateThumbnail(source, cacheKey);
        }

        private void LoadHistory()
        {
            UpdateHistoryDirectory();
            
            if (!Directory.Exists(historyDirectory))
                return;

            var files = Directory.GetFiles(historyDirectory, "Screenshot_*.*")
                .Where(f => f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                           f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                           f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                           f.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
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
                catch (Exception ex)
                {
                    DebugHelper.LogError("ScreenshotManager", $"Failed to load screenshot: {file}", ex);
                }
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
                DebugHelper.LogError("ScreenshotManager", "Failed to delete screenshot", ex);
            }
        }
    }

    public class ScreenshotItem
    {
        public DateTime Timestamp { get; set; }
        public string Filename { get; set; } = "";
        public string FilePath { get; set; } = "";
        public BitmapSource? Thumbnail { get; set; }
        public string? CloudUrl { get; set; }
        public string? CloudProvider { get; set; }
        public string Category { get; set; } = "";
        public List<string> Tags { get; set; } = new();
        public string? Notes { get; set; }
    }
}
