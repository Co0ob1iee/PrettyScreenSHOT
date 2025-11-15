using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Wpf.Ui.Controls;

namespace PrettyScreenSHOT
{
    public partial class UpdateWindow : FluentWindow
    {
        private UpdateInfo? updateInfo;
        private UpdateManager? updateManager;

        public UpdateWindow(UpdateInfo info, UpdateManager manager)
        {
            InitializeComponent();
            updateInfo = info;
            updateManager = manager;
            
            LoadUpdateInfo();
        }

        private void LoadUpdateInfo()
        {
            if (updateInfo == null) return;

            // Pobierz aktualną wersję
            try
            {
                var currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                CurrentVersionText.Text = $"{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}";
            }
            catch
            {
                CurrentVersionText.Text = "Unknown";
            }

            NewVersionText.Text = updateInfo.Version;
            ReleaseDateText.Text = updateInfo.ReleaseDate.ToString("yyyy-MM-dd");
            
            // Formatuj rozmiar pliku
            string fileSize = FormatFileSize(updateInfo.FileSize);
            FileSizeText.Text = fileSize;

            // Release notes
            if (!string.IsNullOrEmpty(updateInfo.ReleaseNotes))
            {
                ReleaseNotesText.Text = updateInfo.ReleaseNotes;
            }
            else
            {
                ReleaseNotesText.Text = "No release notes available.";
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (updateInfo == null || updateManager == null) return;

            try
            {
                // Ukryj przyciski, pokaż progress
                DownloadButton.IsEnabled = false;
                SkipButton.IsEnabled = false;
                LaterButton.IsEnabled = false;
                ProgressPanel.Visibility = Visibility.Visible;

                // Pobierz aktualizację z progressem
                var progress = new Progress<double>(percent =>
                {
                    DownloadProgressBar.Value = percent;
                    ProgressText.Text = $"{percent:F1}%";
                });

                await updateManager.DownloadAndInstallAsync(updateInfo, progress);

                // Jeśli dotarliśmy tutaj, instalacja się rozpoczęła
                // Aplikacja zostanie zamknięta przez UpdateInstaller
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("UpdateWindow", "Error downloading update", ex);
                
                System.Windows.MessageBox.Show(
                    $"Error downloading update:\n\n{ex.Message}\n\nPlease try downloading manually from GitHub.",
                    "Download Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                // Przywróć UI
                DownloadButton.IsEnabled = true;
                SkipButton.IsEnabled = true;
                LaterButton.IsEnabled = true;
                ProgressPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            if (updateInfo == null || updateManager == null) return;

            updateManager.SkipUpdate(updateInfo.Version);
            this.Close();
        }

        private void LaterButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
