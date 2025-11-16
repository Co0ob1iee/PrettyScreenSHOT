using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using PrettyScreenSHOT.Helpers;
using PrettyScreenSHOT.Services;
using PrettyScreenSHOT.Services.Cloud;
using PrettyScreenSHOT.Services.Screenshot;
using Wpf.Ui.Controls;

namespace PrettyScreenSHOT.Views.Windows
{
    public partial class UploadHistoryWindow : FluentWindow
    {
        private readonly ObservableCollection<UploadItem> uploadItems = new();

        public class UploadItem
        {
            public int Index { get; set; }
            public string Url { get; set; } = "";
            public string FileName { get; set; } = "";
            public string Provider { get; set; } = "";
            public string Status { get; set; } = "";
            public Brush StatusColor { get; set; } = Brushes.Gray;
            public string? FilePath { get; set; }
            public DateTime UploadDate { get; set; }
        }

        public UploadHistoryWindow()
        {
            InitializeComponent();
            LoadUploadHistory();
        }

        private void LoadUploadHistory()
        {
            uploadItems.Clear();

            // Get screenshots that have been uploaded
            var screenshots = ScreenshotManager.Instance.GetAllScreenshots()
                .Where(s => !string.IsNullOrEmpty(s.CloudUrl))
                .OrderByDescending(s => s.CaptureTime)
                .Take(100) // Limit to last 100 uploads
                .ToList();

            if (screenshots.Count == 0)
            {
                EmptyState.Visibility = Visibility.Visible;
                return;
            }

            EmptyState.Visibility = Visibility.Collapsed;

            int index = 1;
            foreach (var screenshot in screenshots)
            {
                var item = new UploadItem
                {
                    Index = index++,
                    Url = screenshot.CloudUrl ?? "",
                    FileName = System.IO.Path.GetFileName(screenshot.FilePath),
                    Provider = screenshot.CloudProvider ?? "Unknown",
                    Status = "Sukces",
                    StatusColor = new SolidColorBrush(Color.FromRgb(16, 185, 129)), // Green
                    FilePath = screenshot.FilePath,
                    UploadDate = screenshot.CaptureTime
                };

                uploadItems.Add(item);
            }

            UploadItemsControl.ItemsSource = uploadItems;
        }

        private void OnUrlClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBlock textBlock && textBlock.DataContext is UploadItem item)
            {
                if (!string.IsNullOrEmpty(item.Url))
                {
                    System.Windows.Clipboard.SetText(item.Url);
                    ToastNotificationManager.Instance.ShowInfo("URL skopiowany", "URL został skopiowany do schowka");
                }
            }
        }

        private void OnCopyUrlClick(object sender, RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.Button button && button.Tag is UploadItem item)
            {
                if (!string.IsNullOrEmpty(item.Url))
                {
                    System.Windows.Clipboard.SetText(item.Url);
                    ToastNotificationManager.Instance.ShowSuccess("Skopiowano", "URL został skopiowany do schowka");
                }
            }
        }

        private async void OnReuploadClick(object sender, RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.Button button && button.Tag is UploadItem item)
            {
                if (string.IsNullOrEmpty(item.FilePath) || !System.IO.File.Exists(item.FilePath))
                {
                    ToastNotificationManager.Instance.ShowError("Błąd", "Plik źródłowy nie został znaleziony");
                    return;
                }

                try
                {
                    button.IsEnabled = false;
                    ToastNotificationManager.Instance.ShowInfo("Przesyłanie...", "Przesyłanie zdjęcia do chmury");

                    // Load bitmap from file
                    var bitmap = new System.Windows.Media.Imaging.BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(item.FilePath);
                    bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    var result = await CloudUploadManager.Instance.UploadScreenshotAsync(
                        bitmap,
                        System.IO.Path.GetFileName(item.FilePath));

                    button.IsEnabled = true;

                    if (result.Success && !string.IsNullOrEmpty(result.Url))
                    {
                        System.Windows.Clipboard.SetText(result.Url);
                        ToastNotificationManager.Instance.ShowSuccess(
                            "Upload zakończony",
                            $"Zdjęcie przesłane ponownie. URL skopiowany do schowka.");

                        // Update item
                        item.Url = result.Url;
                        item.Provider = result.ProviderName ?? "Unknown";
                        item.Status = "Sukces";
                        item.StatusColor = new SolidColorBrush(Color.FromRgb(16, 185, 129));

                        // Refresh view
                        LoadUploadHistory();
                    }
                    else
                    {
                        ToastNotificationManager.Instance.ShowError(
                            "Błąd uploadowania",
                            result.ErrorMessage ?? "Nieznany błąd");
                    }
                }
                catch (Exception ex)
                {
                    button.IsEnabled = true;
                    DebugHelper.LogError("UploadHistory", "Error re-uploading", ex);
                    ToastNotificationManager.Instance.ShowError("Błąd", $"Błąd: {ex.Message}");
                }
            }
        }

        private void OnOpenInBrowserClick(object sender, RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.Button button && button.Tag is UploadItem item)
            {
                if (!string.IsNullOrEmpty(item.Url))
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = item.Url,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.LogError("UploadHistory", "Error opening URL", ex);
                        ToastNotificationManager.Instance.ShowError("Błąd", "Nie można otworzyć URL w przeglądarce");
                    }
                }
            }
        }
    }
}
