using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;

namespace PrettyScreenSHOT
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
            this.WindowState = WindowState.Maximized;
            this.Left = 0;
            this.Top = 0;
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = SystemParameters.PrimaryScreenHeight;
            this.Topmost = true;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            startPoint = e.GetPosition(this);
            isSelecting = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
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
            this.Close();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            // Czarna maska za ca³ym ekranem
            drawingContext.DrawRectangle(new SolidColorBrush(Color.FromArgb(100, 0, 0, 0)), null, 
                new Rect(0, 0, this.Width, this.Height));

            if (isSelecting)
            {
                // Jasniejszy prostok¹t zaznaczenia
                var rect = new Rect(startPoint, endPoint);
                drawingContext.DrawRectangle(null, new Pen(new SolidColorBrush(Colors.Cyan), 3), rect);
                
                // Wyczyszczenie zaznaczonego obszaru (przejrzystoœæ)
                drawingContext.PushOpacity(0.3);
                drawingContext.DrawRectangle(new SolidColorBrush(Colors.White), null, rect);
                drawingContext.Pop();
            }
        }

        private void CaptureScreenshot()
        {
            int x = (int)Math.Min(startPoint.X, endPoint.X);
            int y = (int)Math.Min(startPoint.Y, endPoint.Y);
            int width = (int)Math.Abs(endPoint.X - startPoint.X);
            int height = (int)Math.Abs(endPoint.Y - startPoint.Y);

            if (width <= 0 || height <= 0)
                return;

            try
            {
                var bitmapSource = ScreenshotHelper.CaptureScreenRegion(x, y, width, height);
                CopyBitmapToClipboard(bitmapSource);
                
                DebugHelper.LogDebug($"Screenshot skopiowany: {width}x{height}px");
                
                // Otworz edytor BEZ messageboxa
                var editorWindow = new ScreenshotEditorWindow(bitmapSource);
                editorWindow.Owner = null;
                editorWindow.Show();
                editorWindow.Activate();
                
                DebugHelper.LogDebug("Edytor otwarty");
            }
            catch (Exception ex)
            {
                DebugHelper.LogDebug($"Blad: {ex.Message}");
                DebugHelper.ShowMessage("Blad", $"Blad podczas przechwytywania screenshotu: {ex.Message}");
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