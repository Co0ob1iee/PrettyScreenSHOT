using System.Windows;
using System.Windows.Forms;
using System.Drawing;

namespace PrettyScreenSHOT
{
    public class TrayIconManager : IDisposable
    {
        private NotifyIcon? notifyIcon;
        private ContextMenuStrip? contextMenu;

        public void Initialize()
        {
            CreateTrayIcon();
            SetupKeyboardHook();
        }

        private void CreateTrayIcon()
        {
            contextMenu = new ContextMenuStrip();
            
            var historyItem = new ToolStripMenuItem("Historia screenshotow", null, ShowHistory);
            var exitItem = new ToolStripMenuItem("Wyjscie", null, ExitApplication);
            
            contextMenu.Items.Add(historyItem);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(exitItem);

            // Spróbuj za³adowaæ .ico
            Icon? icon = TryLoadIcon();
            
            notifyIcon = new NotifyIcon
            {
                Icon = icon ?? SystemIcons.Application,
                Visible = true,
                Text = "PrettyScreenSHOT - Nacisnij PRTSCN",
                ContextMenuStrip = contextMenu
            };

            notifyIcon.MouseDoubleClick += (s, e) => ShowHistory(null, null);
        }

        private Icon? TryLoadIcon()
        {
            try
            {
                var exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                var exeDir = System.IO.Path.GetDirectoryName(exePath);
                
                var icoPath = System.IO.Path.Combine(exeDir ?? "", "app.ico");
                if (System.IO.File.Exists(icoPath))
                    return new Icon(icoPath);

                // Szukaj w alternatywnych lokalizacjach
                var altPaths = new[]
                {
                    System.IO.Path.Combine(exeDir ?? "", "bin", "Debug", "app.ico"),
                    System.IO.Path.Combine(exeDir ?? "", "bin", "Release", "app.ico"),
                    System.IO.Path.Combine(exeDir ?? "", "..", "..", "app.ico"),
                };

                foreach (var path in altPaths)
                {
                    if (System.IO.File.Exists(path))
                        return new Icon(path);
                }
            }
            catch (Exception ex)
            {
                DebugHelper.LogDebug($"Blad wczytywania ikony: {ex.Message}");
            }

            return null;
        }

        private void SetupKeyboardHook()
        {
            ScreenshotHelper.SetupKeyboardHook(() =>
            {
                DebugHelper.LogDebug("PRTSCN nacisniety");
                var overlayWindow = new ScreenshotOverlay();
                overlayWindow.Show();
            });
        }

        private void ShowHistory(object? sender, EventArgs? e)
        {
            var historyWindow = new ScreenshotHistoryWindow();
            historyWindow.Show();
        }

        private void ExitApplication(object? sender, EventArgs? e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void Dispose()
        {
            notifyIcon?.Dispose();
            contextMenu?.Dispose();
        }
    }
}