using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using PrettyScreenSHOT.Helpers;
using PrettyScreenSHOT.Models;
using PrettyScreenSHOT.Services.Settings;
using PrettyScreenSHOT.Services.Update;
using Wpf.Ui.Controls;

namespace PrettyScreenSHOT.Views.Windows
{
    public partial class UpdateNotificationWindow : FluentWindow
    {
        private UpdateInfo? updateInfo;
        private UpdateDownloader? downloader;
        private bool isDownloading = false;

        public UpdateNotificationWindow(UpdateInfo updateInfo)
        {
            InitializeComponent();
            this.updateInfo = updateInfo;
            LoadUpdateInfo();
        }

        private void LoadUpdateInfo()
        {
            if (updateInfo == null) return;

            VersionText.Text = $"Version {updateInfo.Version} is now available";
            ReleaseNotesText.Text = updateInfo.ReleaseNotes;
            
            if (updateInfo.FileSize > 0)
            {
                FileSizeText.Text = FormatFileSize(updateInfo.FileSize);
            }
            
            if (updateInfo.ReleaseDate != DateTime.MinValue)
            {
                PublishedText.Text = updateInfo.ReleaseDate.ToString("yyyy-MM-dd HH:mm");
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

        private async void UpdateNowButton_Click(object sender, RoutedEventArgs e)
        {
            if (updateInfo == null || isDownloading) return;

            try
            {
                isDownloading = true;
                UpdateNowButton.IsEnabled = false;
                UpdateNowButton.Content = "Downloading...";

                // Pokaż okno postępu
                var progressWindow = new UpdateProgressWindow();
                progressWindow.Show();

                downloader = new UpdateDownloader();
                var progress = new Progress<double>(percent =>
                {
                    progressWindow.UpdateProgress(percent);
                });

                // Pobierz aktualizację
                var installerPath = await downloader.DownloadUpdateAsync(
                    updateInfo.DownloadUrl, updateInfo.FileName, progress);

                progressWindow.Close();

                // Zainstaluj
                var isMsix = installerPath.EndsWith(".msix", StringComparison.OrdinalIgnoreCase);

                if (isMsix)
                {
                    // MSIX installation
                    InstallMsixAndRestart(installerPath);
                    return;
                }
                else
                {
                    // EXE installation
                    var installer = new UpdateInstaller();
                    installer.InstallUpdate(installerPath, restartAfterInstall: true);
                }

            }
            catch (Exception ex)
            {
                DebugHelper.LogError("UpdateNotification", "Error during update", ex);
                System.Windows.MessageBox.Show(
                    $"Error: {ex.Message}",
                    "Update Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
                
                UpdateNowButton.IsEnabled = true;
                UpdateNowButton.Content = "Update Now";
            }
            finally
            {
                isDownloading = false;
            }
        }

        private void InstallMsixAndRestart(string msixPath)
        {
            try
            {
                var installer = new UpdateInstaller();
                installer.InstallUpdate(msixPath, restartAfterInstall: true);
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("UpdateNotification", "MSIX install error", ex);
                System.Windows.MessageBox.Show(
                    "Failed to install MSIX package.",
                    "Install Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }

        private void LaterButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SkipVersionButton_Click(object sender, RoutedEventArgs e)
        {
            if (updateInfo != null)
            {
                SettingsManager.Instance.SkippedVersion = updateInfo.Version;
                DebugHelper.LogInfo("UpdateNotification", $"Skipped version: {updateInfo.Version}");
            }
            this.Close();
        }

        private void ViewOnGitHubButton_Click(object sender, RoutedEventArgs e)
        {
            // Otwórz stronę release na GitHub
            var url = $"https://github.com/yourusername/PrettyScreenSHOT/releases/tag/v{updateInfo?.Version}"; // TODO: Zmień na prawdziwe repo
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    }
}

