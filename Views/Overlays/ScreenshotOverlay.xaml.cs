using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Runtime.InteropServices;
using PrettyScreenSHOT.Helpers;
using PrettyScreenSHOT.Services;
using PrettyScreenSHOT.Services.Screenshot;
using PrettyScreenSHOT.Services.Settings;
using Point = System.Windows.Point;

namespace PrettyScreenSHOT.Views.Overlays
{
    public partial class ScreenshotOverlay : Window
    {
        private Point startPoint;
        private Point endPoint;
        private bool isSelecting = false;
        private bool helpVisible = false;
        private Color maskColor = Color.FromArgb(100, 0, 0, 0); // Configurable mask color
        private byte maskOpacity = 100; // Configurable opacity (0-255)

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public ScreenshotOverlay()
        {
            InitializeComponent();
            SetupMultiMonitor();
            InitializeOverlay();
        }

        private void InitializeOverlay()
        {
            // Load overlay settings from SettingsManager if available
            // For now, use defaults
            this.KeyDown += OnOverlayKeyDown;

            // Show help panel with fade-in animation
            ShowHelpPanelWithAnimation();

            // Hide help after 3 seconds
            var hideTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            hideTimer.Tick += (s, e) =>
            {
                HideHelpPanelWithAnimation();
                hideTimer.Stop();
            };
            hideTimer.Start();
        }

        private void ShowHelpPanelWithAnimation()
        {
            var animation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            HelpPanel.BeginAnimation(OpacityProperty, animation);
            helpVisible = true;
        }

        private void HideHelpPanelWithAnimation()
        {
            var animation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            HelpPanel.BeginAnimation(OpacityProperty, animation);
            helpVisible = false;
        }

        private void OnOverlayKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    // Cancel selection
                    this.Hide();
                    DebugHelper.LogInfo("Overlay", "Selection cancelled by user (ESC)");
                    break;

                case Key.Enter:
                    // Confirm selection
                    if (isSelecting)
                    {
                        CaptureScreenshot();
                    }
                    break;

                case Key.F1:
                    // Toggle help
                    if (helpVisible)
                        HideHelpPanelWithAnimation();
                    else
                        ShowHelpPanelWithAnimation();
                    break;

                case Key.Space:
                    // Capture full screen
                    CaptureFullScreen();
                    break;
            }
        }

        private void CaptureFullScreen()
        {
            try
            {
                this.Hide();
                System.Threading.Thread.Sleep(100);

                var virtualBounds = ScreenshotHelper.GetVirtualScreenBounds();
                var bitmapSource = ScreenshotHelper.CaptureScreenRegion(
                    virtualBounds.Left,
                    virtualBounds.Top,
                    virtualBounds.Width,
                    virtualBounds.Height);

                if (SettingsManager.Instance.CopyToClipboard)
                {
                    CopyBitmapToClipboard(bitmapSource);
                }

                ScreenshotManager.Instance.LastCapturedBitmap = bitmapSource;

                if (TrayIconManager.Instance != null && SettingsManager.Instance.ShowNotifications)
                {
                    ToastNotificationManager.Instance.ShowSuccess(
                        LocalizationHelper.GetString("Notification_Title"),
                        "Przechwycono pełny ekran");
                }

                DebugHelper.LogInfo("Overlay", "Full screen captured");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("Overlay", "Error capturing full screen", ex);
            }
        }

        private void OnCaptureClick(object sender, RoutedEventArgs e)
        {
            CaptureScreenshot();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            this.Hide();
            DebugHelper.LogInfo("Overlay", "Selection cancelled by user");
        }

        private void SetupMultiMonitor()
        {
            // Get virtual screen bounds covering all monitors
            var virtualBounds = ScreenshotHelper.GetVirtualScreenBounds();
            
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;
            this.Background = System.Windows.Media.Brushes.Transparent;
            this.Topmost = true;
            this.ShowInTaskbar = false;
            this.ResizeMode = ResizeMode.NoResize;
            
            // Set window to cover all monitors
            this.Left = virtualBounds.Left;
            this.Top = virtualBounds.Top;
            this.Width = virtualBounds.Width;
            this.Height = virtualBounds.Height;
            
            DebugHelper.LogInfo("Overlay", $"Multi-monitor overlay: {virtualBounds.Width}x{virtualBounds.Height} at ({virtualBounds.Left}, {virtualBounds.Top})");
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            startPoint = e.GetPosition(this);
            isSelecting = true;
            InfoPanel.Visibility = Visibility.Visible;
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isSelecting)
            {
                endPoint = e.GetPosition(this);

                // Update info panel with selection dimensions
                int width = (int)Math.Abs(endPoint.X - startPoint.X);
                int height = (int)Math.Abs(endPoint.Y - startPoint.Y);
                InfoText.Text = $"{width} x {height} px";

                // Show quick toolbar if selection is large enough
                if (width > 20 && height > 20)
                {
                    QuickToolbar.Visibility = Visibility.Visible;
                }
                else
                {
                    QuickToolbar.Visibility = Visibility.Collapsed;
                }

                InvalidateVisual();
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (!isSelecting) return;

            endPoint = e.GetPosition(this);

            // Check if selection is valid
            int width = (int)Math.Abs(endPoint.X - startPoint.X);
            int height = (int)Math.Abs(endPoint.Y - startPoint.Y);

            if (width > 5 && height > 5)
            {
                // Show quick toolbar for confirmation
                QuickToolbar.Visibility = Visibility.Visible;
                // Don't auto-capture, wait for user confirmation
            }
            else
            {
                // Too small, cancel
                isSelecting = false;
                InfoPanel.Visibility = Visibility.Collapsed;
                QuickToolbar.Visibility = Visibility.Collapsed;
                InvalidateVisual();
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            // Draw mask over entire virtual screen with configurable color/opacity
            var maskBrush = new SolidColorBrush(maskColor);
            drawingContext.DrawRectangle(maskBrush, null, new Rect(0, 0, this.Width, this.Height));

            if (isSelecting)
            {
                var rect = new Rect(startPoint, endPoint);

                // Clear the selected area (no mask)
                drawingContext.DrawRectangle(Brushes.Transparent, null, rect);

                // Draw modern selection border
                var borderPen = new Pen(new SolidColorBrush(Color.FromRgb(0, 120, 212)), 2); // Blue border
                drawingContext.DrawRectangle(null, borderPen, rect);

                // Draw corner handles
                DrawCornerHandles(drawingContext, rect);

                // Draw crosshair guides (optional, subtle)
                DrawCrosshairGuides(drawingContext, rect);
            }
        }

        private void DrawCornerHandles(DrawingContext drawingContext, Rect rect)
        {
            double handleSize = 8;
            double handleThickness = 20;
            var handleBrush = new SolidColorBrush(Color.FromRgb(0, 120, 212));
            var handlePen = new Pen(handleBrush, 3);

            // Top-left
            drawingContext.DrawLine(handlePen, new Point(rect.Left, rect.Top), new Point(rect.Left + handleThickness, rect.Top));
            drawingContext.DrawLine(handlePen, new Point(rect.Left, rect.Top), new Point(rect.Left, rect.Top + handleThickness));

            // Top-right
            drawingContext.DrawLine(handlePen, new Point(rect.Right, rect.Top), new Point(rect.Right - handleThickness, rect.Top));
            drawingContext.DrawLine(handlePen, new Point(rect.Right, rect.Top), new Point(rect.Right, rect.Top + handleThickness));

            // Bottom-left
            drawingContext.DrawLine(handlePen, new Point(rect.Left, rect.Bottom), new Point(rect.Left + handleThickness, rect.Bottom));
            drawingContext.DrawLine(handlePen, new Point(rect.Left, rect.Bottom), new Point(rect.Left, rect.Bottom - handleThickness));

            // Bottom-right
            drawingContext.DrawLine(handlePen, new Point(rect.Right, rect.Bottom), new Point(rect.Right - handleThickness, rect.Bottom));
            drawingContext.DrawLine(handlePen, new Point(rect.Right, rect.Bottom), new Point(rect.Right, rect.Bottom - handleThickness));
        }

        private void DrawCrosshairGuides(DrawingContext drawingContext, Rect rect)
        {
            var guidePen = new Pen(new SolidColorBrush(Color.FromArgb(60, 255, 255, 255)), 1)
            {
                DashStyle = DashStyles.Dash
            };

            // Horizontal center line
            double centerY = (rect.Top + rect.Bottom) / 2;
            drawingContext.DrawLine(guidePen, new Point(0, centerY), new Point(this.Width, centerY));

            // Vertical center line
            double centerX = (rect.Left + rect.Right) / 2;
            drawingContext.DrawLine(guidePen, new Point(centerX, 0), new Point(centerX, this.Height));
        }

        private void CaptureScreenshot()
        {
            // Convert window-relative coordinates to screen coordinates
            var windowPoint = new System.Drawing.Point((int)this.Left, (int)this.Top);
            int x = (int)(Math.Min(startPoint.X, endPoint.X) + windowPoint.X);
            int y = (int)(Math.Min(startPoint.Y, endPoint.Y) + windowPoint.Y);
            int width = (int)Math.Abs(endPoint.X - startPoint.X);
            int height = (int)Math.Abs(endPoint.Y - startPoint.Y);

            DebugHelper.LogInfo("Overlay", $"Capture started - Area: ({x}, {y}) {width}x{height}px");

            if (width <= 0 || height <= 0)
            {
                DebugHelper.LogError("Overlay", "Invalid area - width or height <= 0");
                return;
            }

            try
            {
                // Hide UI elements and overlay before taking screenshot
                InfoPanel.Visibility = Visibility.Collapsed;
                QuickToolbar.Visibility = Visibility.Collapsed;
                HelpPanel.Opacity = 0;

                this.Hide();
                this.IsEnabled = false;
                DebugHelper.LogInfo("Overlay", "Overlay hidden");

                // Give system time to render
                System.Threading.Thread.Sleep(100);

                // Capture screenshot without overlay
                System.Windows.Media.Imaging.BitmapSource? bitmapSource = null;
                try
                {
                    bitmapSource = ScreenshotHelper.CaptureScreenRegion(x, y, width, height);
                    DebugHelper.LogInfo("Overlay", "Screenshot captured");

                    if (SettingsManager.Instance.CopyToClipboard)
                    {
                        CopyBitmapToClipboard(bitmapSource);
                        DebugHelper.LogInfo("Overlay", "Screenshot copied to clipboard");
                    }

                    DebugHelper.LogDebug($"Screenshot captured: {width}x{height}px");

                    // Save screenshot
                    ScreenshotManager.Instance.LastCapturedBitmap = bitmapSource;
                    DebugHelper.LogInfo("Overlay", "Screenshot saved to manager");

                    // Show notification
                    if (TrayIconManager.Instance != null && SettingsManager.Instance.ShowNotifications)
                    {
                        ToastNotificationManager.Instance.ShowSuccess(
                            LocalizationHelper.GetString("Notification_Title"),
                            LocalizationHelper.GetString("Notification_ScreenshotSaved"));
                        DebugHelper.LogInfo("Overlay", "Notification shown");
                    }
                }
                finally
                {
                    // BitmapSource will be disposed by GC, but we can help by clearing reference if needed
                    // Note: BitmapSource implements IDisposable but Freeze() makes it immutable and safe
                }
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("Overlay", "Error during capture", ex);
            }
            finally
            {
                try
                {
                    // Reset state
                    isSelecting = false;
                    InfoPanel.Visibility = Visibility.Collapsed;
                    QuickToolbar.Visibility = Visibility.Collapsed;

                    this.Hide();
                    DebugHelper.LogInfo("Overlay", "Capture completed - overlay hidden");
                }
                catch { }
            }
        }

        private void CopyBitmapToClipboard(System.Windows.Media.Imaging.BitmapSource bitmap)
        {
            var dataObject = new System.Windows.DataObject();
            dataObject.SetData(System.Windows.DataFormats.Bitmap, bitmap);
            System.Windows.Clipboard.SetDataObject(dataObject, true);
        }
    }
}
