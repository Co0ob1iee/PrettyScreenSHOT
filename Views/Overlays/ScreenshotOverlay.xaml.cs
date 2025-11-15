using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;
using Point = System.Windows.Point;

namespace PrettyScreenSHOT.Views.Overlays
{
    public partial class ScreenshotOverlay : Window
    {
        private Point startPoint;
        private Point endPoint;
        private bool isSelecting = false;

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public ScreenshotOverlay()
        {
            InitializeComponent();
            SetupMultiMonitor();
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
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isSelecting)
            {
                endPoint = e.GetPosition(this);
                InvalidateVisual();
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            isSelecting = false;
            endPoint = e.GetPosition(this);
            CaptureScreenshot();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            // Black mask over entire virtual screen
            drawingContext.DrawRectangle(new SolidColorBrush(System.Windows.Media.Color.FromArgb(100, 0, 0, 0)), null, 
                new Rect(0, 0, this.Width, this.Height));

            if (isSelecting)
            {
                // Bright selection rectangle
                var rect = new Rect(startPoint, endPoint);
                drawingContext.DrawRectangle(null, new System.Windows.Media.Pen(new SolidColorBrush(System.Windows.Media.Colors.Cyan), 3), rect);
            }
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
                // Hide overlay before taking screenshot
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
                        TrayIconManager.Instance.ShowNotification(
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
