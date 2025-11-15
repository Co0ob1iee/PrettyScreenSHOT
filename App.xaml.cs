using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PrettyScreenSHOT
{
    public partial class App : System.Windows.Application
    {
        private TrayIconManager? trayIconManager;
        private UpdateManager? updateManager;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize localization
            LocalizationHelper.Initialize();

            // Initialize cloud upload manager
            var cloudManager = CloudUploadManager.Instance;
            if (!string.IsNullOrWhiteSpace(SettingsManager.Instance.CloudProvider))
            {
                cloudManager.CurrentProvider = SettingsManager.Instance.CloudProvider;
            }

            // Initialize theme manager
            var themeName = SettingsManager.Instance.Theme;
            if (System.Enum.TryParse<Theme>(themeName, true, out var theme))
            {
                ThemeManager.Instance.SetTheme(theme);
            }

            // Initialize tray icon
            trayIconManager = new TrayIconManager();
            trayIconManager.Initialize();

            // Initialize auto-update
            InitializeAutoUpdate();

            DebugHelper.LogDebug("Application started - press PRTSCN for screenshot");
        }

        private void InitializeAutoUpdate()
        {
            if (!SettingsManager.Instance.EnableAutoUpdate)
            {
                DebugHelper.LogInfo("App", "Auto-update is disabled");
                return;
            }

            // Utwórz UpdateManager
            updateManager = new UpdateManager();
            
            // Subskrybuj zdarzenie UpdateAvailable
            updateManager.UpdateAvailable += (sender, updateInfo) =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    var updateWindow = new UpdateWindow(updateInfo, updateManager);
                    updateWindow.Show();
                });
            };

            // Inicjalizuj (rozpocznie okresowe sprawdzanie)
            updateManager.Initialize();
            
            DebugHelper.LogInfo("App", "Auto-update initialized");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            trayIconManager?.Dispose();
            updateManager?.Dispose();
            ScreenshotHelper.RemoveKeyboardHook();
            base.OnExit(e);
        }
    }
}
