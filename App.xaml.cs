using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Wpf.Ui.Appearance;
using PrettyScreenSHOT.Helpers;
using PrettyScreenSHOT.Services;
using PrettyScreenSHOT.Services.Update;
using PrettyScreenSHOT.Services.Cloud;
using PrettyScreenSHOT.Services.Settings;
using PrettyScreenSHOT.Services.Screenshot;
using PrettyScreenSHOT.Views.Windows;

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

            // Initialize WPF UI Theme Manager (replaces ThemeManager)
            InitializeWpfUiTheme();

            // Initialize tray icon
            trayIconManager = new TrayIconManager();
            trayIconManager.Initialize();

            // Initialize auto-update
            InitializeAutoUpdate();

            DebugHelper.LogDebug("Application started - press PRTSCN for screenshot");
        }

        private void InitializeWpfUiTheme()
        {
            var themeName = SettingsManager.Instance.Theme;

            // Map legacy theme names to WPF UI themes
            ApplicationTheme appTheme = themeName?.ToLower() switch
            {
                "light" => ApplicationTheme.Light,
                "dark" => ApplicationTheme.Dark,
                "neumorphic" => ApplicationTheme.Light, // Map Neumorphic to Light
                "system" => ApplicationTheme.Unknown,    // Auto-detect system theme
                _ => ApplicationTheme.Dark
            };

            // Apply theme globally
            if (appTheme == ApplicationTheme.Unknown)
            {
                ApplicationThemeManager.ApplySystemTheme();
            }
            else
            {
                // Apply theme - WindowBackdropType may not be available in this WPF UI version
                ApplicationThemeManager.Apply(appTheme);
            }

            DebugHelper.LogInfo("App", $"WPF UI Theme initialized: {themeName} -> {appTheme}");
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
