using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PrettyScreenSHOT
{
    /// <summary>
    /// Główny manager do zarządzania aktualizacjami
    /// </summary>
    public class UpdateManager : IDisposable
    {
        private readonly UpdateChecker updateChecker;
        private readonly UpdateDownloader updateDownloader;
        private readonly UpdateInstaller updateInstaller;
        private System.Threading.Timer? checkTimer;
        private bool isChecking = false;

        public event EventHandler<UpdateInfo>? UpdateAvailable;
        public event EventHandler<double>? DownloadProgress;

        public UpdateManager()
        {
            updateChecker = new UpdateChecker();
            updateDownloader = new UpdateDownloader();
            updateInstaller = new UpdateInstaller();
        }

        /// <summary>
        /// Inicjalizuje manager i rozpoczyna okresowe sprawdzanie
        /// </summary>
        public void Initialize()
        {
            var settings = SettingsManager.Instance;

            if (!settings.EnableAutoUpdate)
            {
                DebugHelper.LogInfo("UpdateManager", "Auto-update is disabled");
                return;
            }

            // Sprawdź przy starcie jeśli włączone
            if (settings.CheckForUpdatesOnStartup)
            {
                Task.Run(async () => await CheckForUpdatesAsync());
            }

            // Ustaw timer do okresowego sprawdzania
            int intervalHours = settings.UpdateCheckIntervalHours;
            if (intervalHours > 0)
            {
                var interval = TimeSpan.FromHours(intervalHours);
                // Use System.Threading.Timer and a callback that starts the async check
                checkTimer = new System.Threading.Timer(state => { _ = CheckForUpdatesAsync(); }, null, interval, interval);
                DebugHelper.LogInfo("UpdateManager", $"Update check timer set to {intervalHours} hours");
            }
        }

        /// <summary>
        /// Sprawdza dostępność aktualizacji
        /// </summary>
        public async Task<UpdateInfo?> CheckForUpdatesAsync(bool showNotification = true)
        {
            if (isChecking)
            {
                DebugHelper.LogInfo("UpdateManager", "Update check already in progress");
                return null;
            }

            isChecking = true;
            try
            {
                var settings = SettingsManager.Instance;
                var channel = settings.UpdateChannel ?? "stable";

                DebugHelper.LogInfo("UpdateManager", "Checking for updates...");
                var updateInfo = await updateChecker.CheckForUpdatesAsync(channel);

                if (updateInfo != null)
                {
                    // Sprawdź czy użytkownik nie pominął tej wersji
                    if (updateInfo.Version == settings.SkippedVersion)
                    {
                        DebugHelper.LogInfo("UpdateManager", $"Update {updateInfo.Version} was skipped by user");
                        return null;
                    }

                    // Sprawdź czy nie sprawdzaliśmy zbyt niedawno
                    if (settings.LastUpdateCheck > DateTime.Now.AddHours(-1))
                    {
                        DebugHelper.LogInfo("UpdateManager", "Update check was performed recently, skipping");
                        return null;
                    }

                    settings.LastUpdateCheck = DateTime.Now;

                    if (showNotification && settings.ShowUpdateNotifications)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            UpdateAvailable?.Invoke(this, updateInfo);
                        });
                    }

                    return updateInfo;
                }

                DebugHelper.LogInfo("UpdateManager", "No updates available");
                return null;
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("UpdateManager", "Error checking for updates", ex);
                return null;
            }
            finally
            {
                isChecking = false;
            }
        }

        /// <summary>
        /// Pobiera i instaluje aktualizację
        /// </summary>
        public async Task DownloadAndInstallAsync(UpdateInfo updateInfo, IProgress<double>? progress = null)
        {
            try
            {
                DebugHelper.LogInfo("UpdateManager", $"Downloading update: {updateInfo.Version}");

                // Pobierz aktualizację
                string installerPath = await updateDownloader.DownloadUpdateAsync(
                    updateInfo.DownloadUrl,
                    updateInfo.FileName,
                    progress);

                DebugHelper.LogInfo("UpdateManager", "Update downloaded, starting installation");

                // Zainstaluj
                updateInstaller.InstallUpdate(installerPath, restartAfterInstall: true);
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("UpdateManager", "Error downloading/installing update", ex);
                throw;
            }
        }

        /// <summary>
        /// Pomija aktualizację (zapamięta wersję)
        /// </summary>
        public void SkipUpdate(string version)
        {
            var settings = SettingsManager.Instance;
            settings.SkippedVersion = version;
            DebugHelper.LogInfo("UpdateManager", $"Update {version} skipped by user");
        }

        public void Dispose()
        {
            checkTimer?.Dispose();
            updateChecker?.Dispose();
            updateDownloader?.Dispose();
        }
    }
}
