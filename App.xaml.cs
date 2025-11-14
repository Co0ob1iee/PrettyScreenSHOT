using System.Windows;

namespace PrettyScreenSHOT
{
    public partial class App : Application
    {
        private TrayIconManager? trayIconManager;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Inicjalizuj tray icon od razu
            trayIconManager = new TrayIconManager();
            trayIconManager.Initialize();

            DebugHelper.LogDebug("Aplikacja uruchomiona - nacisnij PRTSCN dla screenshotu");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            trayIconManager?.Dispose();
            ScreenshotHelper.RemoveKeyboardHook();
            base.OnExit(e);
        }
    }
}
