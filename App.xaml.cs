using System.Windows;

namespace PrettyScreenSHOT
{
    public partial class App : Application
    {
        private TrayIconManager? trayIconManager;

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

            DebugHelper.LogDebug("Application started - press PRTSCN for screenshot");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            trayIconManager?.Dispose();
            ScreenshotHelper.RemoveKeyboardHook();
            base.OnExit(e);
        }
    }
}
