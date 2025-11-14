using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PrettyScreenSHOT
{
    public partial class ScreenshotEditorWindow : Window
    {
        private BitmapSource originalBitmap;
        private DrawingVisual drawingVisual;
        private EditTool currentTool = EditTool.None;
        private Point startPoint;
        private Point currentPoint;
        private bool isDrawing = false;
        private Color currentColor = Colors.Red;
        private List<DrawingAction> drawingHistory = new();

        public enum EditTool
        {
            None,
            Text,
            Marker,
            Blur,
            Rectangle,
            Arrow
        }

        private class DrawingAction
        {
            public EditTool Tool { get; set; }
            public Point Start { get; set; }
            public Point End { get; set; }
            public Color Color { get; set; }
            public double Size { get; set; }
        }

        public ScreenshotEditorWindow(BitmapSource bitmap)
        {
            InitializeComponent();
            originalBitmap = bitmap;
            drawingVisual = new DrawingVisual();
            
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Topmost = true;
            this.ResizeMode = ResizeMode.CanResize;
            
            SetupEditor();
        }

        private void SetupEditor()
        {
            EditorCanvas.Width = originalBitmap.PixelWidth;
            EditorCanvas.Height = originalBitmap.PixelHeight;
            RedrawCanvas();
        }

        private void RedrawCanvas()
        {
            var drawingContext = drawingVisual.RenderOpen();
            var brush = new ImageBrush(originalBitmap);
            drawingContext.DrawRectangle(brush, null, new Rect(0, 0, originalBitmap.PixelWidth, originalBitmap.PixelHeight));
            
            // Narysuj wszystkie wczeœniejsze akcje
            foreach (var action in drawingHistory)
            {
                DrawAction(drawingContext, action);
            }
            
            drawingContext.Close();

            var renderTargetBitmap = new RenderTargetBitmap(
                originalBitmap.PixelWidth,
                originalBitmap.PixelHeight,
                96, 96,
                PixelFormats.Pbgra32);
            renderTargetBitmap.Render(drawingVisual);

            EditorCanvas.Background = new ImageBrush(renderTargetBitmap);
        }

        private void DrawAction(DrawingContext drawingContext, DrawingAction action)
        {
            var pen = new Pen(new SolidColorBrush(action.Color), action.Size)
            {
                StartLineCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round,
                LineJoin = PenLineJoin.Round
            };
            var rect = new Rect(action.Start, action.End);

            switch (action.Tool)
            {
                case EditTool.Marker:
                    drawingContext.DrawLine(pen, action.Start, action.End);
                    break;
                case EditTool.Rectangle:
                    drawingContext.DrawRectangle(null, pen, rect);
                    break;
                case EditTool.Blur:
                    // Blur przez zamazanie - stworz ciemny prostokat
                    drawingContext.DrawRectangle(
                        new SolidColorBrush(Color.FromArgb(200, 50, 50, 50)), 
                        null, 
                        rect);
                    break;
                case EditTool.Arrow:
                    DrawArrow(drawingContext, action.Start, action.End, pen);
                    break;
            }
        }

        private void DrawArrow(DrawingContext drawingContext, Point start, Point end, Pen pen)
        {
            drawingContext.DrawLine(pen, start, end);
            
            var angle = System.Math.Atan2(end.Y - start.Y, end.X - start.X);
            var arrowLength = 15;
            
            var point1 = new Point(
                end.X - arrowLength * System.Math.Cos(angle - System.Math.PI / 6),
                end.Y - arrowLength * System.Math.Sin(angle - System.Math.PI / 6)
            );
            var point2 = new Point(
                end.X - arrowLength * System.Math.Cos(angle + System.Math.PI / 6),
                end.Y - arrowLength * System.Math.Sin(angle + System.Math.PI / 6)
            );

            drawingContext.DrawLine(pen, end, point1);
            drawingContext.DrawLine(pen, end, point2);
        }

        private void OnToolButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                currentTool = btn.Tag switch
                {
                    "marker" => EditTool.Marker,
                    "text" => EditTool.Text,
                    "blur" => EditTool.Blur,
                    "rectangle" => EditTool.Rectangle,
                    "arrow" => EditTool.Arrow,
                    _ => EditTool.None
                };
                DebugHelper.LogDebug($"Narzedzie: {currentTool}");
                
                // Highlight active button
                if (sender is Button button)
                {
                    button.BorderThickness = new Thickness(2);
                    button.BorderBrush = new SolidColorBrush(Colors.Yellow);
                }
            }
        }

        private void OnColorPickerClick(object sender, RoutedEventArgs e)
        {
            var colorDialog = new System.Windows.Forms.ColorDialog();
            colorDialog.Color = System.Drawing.Color.FromArgb(currentColor.R, currentColor.G, currentColor.B);
            
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                currentColor = Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
                ColorButton.Background = new SolidColorBrush(currentColor);
                DebugHelper.LogDebug($"Kolor zmieniony: RGB({currentColor.R},{currentColor.G},{currentColor.B})");
            }
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            var renderTargetBitmap = new RenderTargetBitmap(
                originalBitmap.PixelWidth,
                originalBitmap.PixelHeight,
                96, 96,
                PixelFormats.Pbgra32);
            renderTargetBitmap.Render(drawingVisual);

            ScreenshotManager.Instance.AddScreenshot(renderTargetBitmap);
            DebugHelper.ShowMessage("Sukces", "Screenshot zapisany!");
            this.Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnClearClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Czy na pewno chcesz wyczyœciæ wszystkie zmiany?", "Potwierdzenie", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ClearAll();
            }
        }

        private void OnUndoClick(object sender, RoutedEventArgs e)
        {
            Undo();
        }

        private void OnEditorCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (currentTool == EditTool.None) return;
            
            isDrawing = true;
            startPoint = e.GetPosition(EditorCanvas);
            EditorCanvas.CaptureMouse();
        }

        private void OnEditorCanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing || currentTool == EditTool.None) return;
            
            currentPoint = e.GetPosition(EditorCanvas);
            // Live preview (nie zapisujemy jeszcze)
        }

        private void OnEditorCanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!isDrawing || currentTool == EditTool.None) return;
            
            isDrawing = false;
            EditorCanvas.ReleaseMouseCapture();
            var endPoint = e.GetPosition(EditorCanvas);
            
            if (currentTool != EditTool.None && startPoint != endPoint)
            {
                drawingHistory.Add(new DrawingAction
                {
                    Tool = currentTool,
                    Start = startPoint,
                    End = endPoint,
                    Color = currentColor,
                    Size = SizeSlider.Value
                });
                
                RedrawCanvas();
            }
        }

        // Undo - usuñ ostatni¹ akcjê
        public void Undo()
        {
            if (drawingHistory.Count > 0)
            {
                drawingHistory.RemoveAt(drawingHistory.Count - 1);
                RedrawCanvas();
                DebugHelper.LogDebug("Undo - ostatnia akcja usuniêta");
            }
        }

        // Clear all
        public void ClearAll()
        {
            drawingHistory.Clear();
            RedrawCanvas();
            DebugHelper.LogDebug("Wszystkie zmiany usuniête");
        }
    }
}
