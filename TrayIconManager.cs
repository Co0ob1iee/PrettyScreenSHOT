using System.Windows;
using System.Windows.Forms;
using System.Drawing;

namespace PrettyScreenSHOT
{
    public class TrayIconManager : IDisposable
    {
        private NotifyIcon? notifyIcon;
        private ContextMenuStrip? contextMenu;
        private static TrayIconManager? instance;

        public static TrayIconManager? Instance { get; private set; }

        public void Initialize()
        {
            instance = this;
            Instance = this;
            CreateTrayIcon();
            SetupKeyboardHook();
        }

        private void CreateTrayIcon()
        {
            contextMenu = new ContextMenuStrip();
            
            var editItem = new ToolStripMenuItem(LocalizationHelper.GetString("Menu_EditLastScreenshot"), null, EditLastScreenshot);
            var historyItem = new ToolStripMenuItem(LocalizationHelper.GetString("Menu_History"), null, ShowHistory);
            var scrollCaptureItem = new ToolStripMenuItem("Scroll Capture", null, StartScrollCapture);
            var videoCaptureItem = new ToolStripMenuItem("Video Capture", null, StartVideoCapture);
            var settingsItem = new ToolStripMenuItem(LocalizationHelper.GetString("Menu_Settings"), null, ShowSettings);
            var exitItem = new ToolStripMenuItem(LocalizationHelper.GetString("Menu_Exit"), null, ExitApplication);
            
            contextMenu.Items.Add(editItem);
            contextMenu.Items.Add(historyItem);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(scrollCaptureItem);
            contextMenu.Items.Add(videoCaptureItem);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(settingsItem);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(exitItem);

            // Spr�buj za�adowa� .ico
            Icon? icon = TryLoadIcon();
            
            notifyIcon = new NotifyIcon
            {
                Icon = icon ?? SystemIcons.Application,
                Visible = true,
                Text = LocalizationHelper.GetString("Tray_Tooltip"),
                ContextMenuStrip = contextMenu
            };

            notifyIcon.MouseDoubleClick += (s, e) => ShowHistory(null, null);
            notifyIcon.BalloonTipClicked += (s, e) => EditLastScreenshot(null, null);
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

        public void ShowNotification(string title, string message)
        {
            if (notifyIcon != null)
            {
                notifyIcon.ShowBalloonTip(5000, title, message, ToolTipIcon.Info);
                DebugHelper.LogDebug($"Notyfikacja: {title} - {message}");
            }
        }

        private void EditLastScreenshot(object? sender, EventArgs? e)
        {
            try
            {
                DebugHelper.LogInfo("TrayIcon", "EditLastScreenshot clicked");
                
                var bitmap = ScreenshotManager.Instance?.LastCapturedBitmap;
                if (bitmap == null)
                {
                    DebugHelper.LogError("TrayIcon", "LastCapturedBitmap is null");
                    return;
                }

                DebugHelper.LogInfo("TrayIcon", $"Opening editor with bitmap: {bitmap.PixelWidth}x{bitmap.PixelHeight}");
                
                var editorWindow = new ScreenshotEditorWindow(bitmap);
                editorWindow.Owner = null;
                editorWindow.Show();
                editorWindow.Activate();
                
                DebugHelper.LogInfo("TrayIcon", "Editor opened successfully");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("TrayIcon", "Error opening editor", ex);
            }
        }

        private void ShowHistory(object? sender, EventArgs? e)
        {
            var historyWindow = new ScreenshotHistoryWindow();
            historyWindow.Show();
        }

        private void ShowSettings(object? sender, EventArgs? e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
            
            // Refresh menu after language change
            CreateTrayIcon();
        }

        private void StartScrollCapture(object? sender, EventArgs? e)
        {
            try
            {
                var overlayWindow = new ScreenshotOverlay();
                overlayWindow.Show();
                
                // Po zaznaczeniu obszaru, uruchom scroll capture
                overlayWindow.Closed += async (s, args) =>
                {
                    // Ta funkcjonalność wymaga dodatkowej implementacji w ScreenshotOverlay
                    DebugHelper.LogInfo("TrayIcon", "Scroll capture requested");
                };
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("TrayIcon", "Error starting scroll capture", ex);
            }
        }

        private void StartVideoCapture(object? sender, EventArgs? e)
        {
            try
            {
                // Pokaż overlay do wyboru obszaru
                var overlayWindow = new ScreenshotOverlay();
                Rectangle? selectedArea = null;
                
                overlayWindow.Closed += (s, args) =>
                {
                    if (selectedArea.HasValue)
                    {
                        // Otwórz okno kontroli video capture
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            var videoWindow = new VideoCaptureWindow();
                            videoWindow.SetCaptureArea(selectedArea.Value);
                            videoWindow.Show();
                        });
                    }
                };
                
                // Przechwyć zaznaczony obszar (wymaga modyfikacji ScreenshotOverlay)
                // Na razie użyj prostego podejścia - pokaż okno z możliwością ręcznego wyboru
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    var videoWindow = new VideoCaptureWindow();
                    // Domyślny obszar - cały ekran
                    var screenBounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
                    videoWindow.SetCaptureArea(screenBounds);
                    videoWindow.Show();
                });
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("TrayIcon", "Error starting video capture", ex);
            }
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