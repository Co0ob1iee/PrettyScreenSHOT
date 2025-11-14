using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PrettyScreenSHOT
{
    public partial class ScreenshotEditorWindow : Window, IDisposable
    {
        private BitmapSource originalBitmap;
        private DrawingVisual drawingVisual;
        private EditTool currentTool = EditTool.None;
        private Point startPoint;
        private Point currentPoint;
        private bool isDrawing = false;
        private Color currentColor = Colors.Red;
        private List<DrawingAction> drawingHistory = new();
        private string textInput = "";
        private int textFontSize = 24;
        private RenderTargetBitmap? currentCanvasBitmap;
        private ImageBrush? currentCanvasBrush;
        private bool disposed = false;

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
            public string? TextContent { get; set; }
        }

        public ScreenshotEditorWindow(BitmapSource bitmap)
        {
            DebugHelper.LogInfo("Editor", "Editor window initializing");
            InitializeComponent();
            originalBitmap = bitmap;
            drawingVisual = new DrawingVisual();
            
            DebugHelper.LogInfo("Editor", $"Original bitmap: {bitmap.PixelWidth}x{bitmap.PixelHeight}px");
            
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Topmost = true;
            this.ResizeMode = ResizeMode.CanResize;
            
            // Załaduj lokalizowane teksty
            LoadLocalizedStrings();
            
            // Zastosuj theme
            ThemeManager.Instance.ApplyTheme(this);
            
            // Zarejestruj skróty klawiszowe
            KeyboardShortcutsManager.Instance.RegisterWindowShortcuts(this);
            
            this.Closed += (s, e) => KeyboardShortcutsManager.Instance.UnregisterWindowShortcuts(this);
            
            SetupEditor();
            DebugHelper.LogInfo("Editor", "Editor window initialized");
        }

        private void LoadLocalizedStrings()
        {
            Title = LocalizationHelper.GetString("Editor_Title");
        }

        private void SetupEditor()
        {
            DebugHelper.LogInfo("Editor", "Setting up canvas");
            EditorCanvas.Width = originalBitmap.PixelWidth;
            EditorCanvas.Height = originalBitmap.PixelHeight;
            RedrawCanvas();
        }

        private void RedrawCanvas()
        {
            var drawingContext = drawingVisual.RenderOpen();
            var brush = new ImageBrush(originalBitmap);
            drawingContext.DrawRectangle(brush, null, new Rect(0, 0, originalBitmap.PixelWidth, originalBitmap.PixelHeight));
            
            // Narysuj wszystkie wcze�niejsze akcje
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
                    DrawBlur(drawingContext, rect, action.Size);
                    break;
                case EditTool.Arrow:
                    DrawArrow(drawingContext, action.Start, action.End, pen);
                    break;
                case EditTool.Text:
                    DrawText(drawingContext, action.Start, action.TextContent ?? "", action.Color, action.Size);
                    break;
            }
        }

        private void DrawBlur(DrawingContext drawingContext, Rect rect, double blurStrength)
        {
            // Prawdziwy blur - rozmycie rzeczywistych pikseli ze screenshotu
            // Dzi�ki temu ukrywamy dane ale pozostaje widoczna og�lna struktura
            
            int blurRadius = (int)Math.Max(2, blurStrength);
            
            // Renderuj oryginalny obraz w czasowy bitmap
            var tempBitmap = new RenderTargetBitmap(
                (int)rect.Width,
                (int)rect.Height,
                96, 96,
                PixelFormats.Pbgra32);
            
            var visual = new DrawingVisual();
            using (var dc = visual.RenderOpen())
            {
                dc.DrawRectangle(
                    new ImageBrush(originalBitmap) 
                    { 
                        Viewbox = rect,
                        ViewboxUnits = BrushMappingMode.Absolute
                    }, 
                    null, 
                    new Rect(0, 0, rect.Width, rect.Height));
            }
            
            tempBitmap.Render(visual);
            
            // Wczytaj piksele
            int width = (int)rect.Width;
            int height = (int)rect.Height;
            int stride = width * 4;
            byte[] pixels = new byte[height * stride];
            tempBitmap.CopyPixels(pixels, stride, 0);
            
            // Zastosuj blur Gaussian
            byte[] blurredPixels = ApplyGaussianBlur(pixels, width, height, blurRadius);
            
            // Narysuj rozmyty obraz
            var blurredBitmap = BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, blurredPixels, stride);
            drawingContext.DrawImage(blurredBitmap, rect);
            
            DebugHelper.LogInfo("Blur", $"Gaussian blur applied - radius: {blurRadius}, area: {rect.Width}x{rect.Height}");
        }

        private byte[] ApplyGaussianBlur(byte[] pixels, int width, int height, int radius)
        {
            byte[] result = new byte[pixels.Length];
            Array.Copy(pixels, result, pixels.Length);
            
            // Kernel Gaussian'a
            double[] kernel = CreateGaussianKernel(radius);
            
            // Aplikuj blur
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double r = 0, g = 0, b = 0, a = 0;
                    double kernelSum = 0;
                    
                    for (int ky = -radius; ky <= radius; ky++)
                    {
                        for (int kx = -radius; kx <= radius; kx++)
                        {
                            int sx = Math.Clamp(x + kx, 0, width - 1);
                            int sy = Math.Clamp(y + ky, 0, height - 1);
                            
                            int pixelIndex = (sy * width + sx) * 4;
                            double kernelValue = kernel[Math.Abs(kx) * kernel.Length / (radius + 1)] *
                                               kernel[Math.Abs(ky) * kernel.Length / (radius + 1)];
							
                            b += pixels[pixelIndex] * kernelValue;
                            g += pixels[pixelIndex + 1] * kernelValue;
                            r += pixels[pixelIndex + 2] * kernelValue;
                            a += pixels[pixelIndex + 3] * kernelValue;
                            kernelSum += kernelValue;
                        }
                    }
                    
                    if (kernelSum > 0)
                    {
                        int outIndex = (y * width + x) * 4;
                        result[outIndex] = (byte)Math.Clamp(b / kernelSum, 0, 255);
                        result[outIndex + 1] = (byte)Math.Clamp(g / kernelSum, 0, 255);
                        result[outIndex + 2] = (byte)Math.Clamp(r / kernelSum, 0, 255);
                        result[outIndex + 3] = (byte)Math.Clamp(a / kernelSum, 0, 255);
                    }
                }
            }
            
            return result;
        }

        private double[] CreateGaussianKernel(int radius)
        {
            int size = radius * 2 + 1;
            double[] kernel = new double[size];
            double sigma = radius / 3.0;
            double sum = 0;
            
            for (int i = 0; i < size; i++)
            {
                int x = i - radius;
                kernel[i] = Math.Exp(-(x * x) / (2 * sigma * sigma));
                sum += kernel[i];
            }
            
            // Normalizuj
            for (int i = 0; i < size; i++)
            {
                kernel[i] /= sum;
            }
            
            return kernel;
        }

        private void DrawText(DrawingContext drawingContext, Point position, string text, Color color, double fontSize)
        {
            if (string.IsNullOrEmpty(text)) return;

            var typeface = new Typeface("Arial");
            var formattedText = new FormattedText(
                text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                fontSize,
                new SolidColorBrush(color),
                1.0);

            drawingContext.DrawText(formattedText, position);
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
                var newTool = btn.Tag switch
                {
                    "marker" => EditTool.Marker,
                    "text" => EditTool.Text,
                    "blur" => EditTool.Blur,
                    "rectangle" => EditTool.Rectangle,
                    "arrow" => EditTool.Arrow,
                    _ => EditTool.None
                };

                // Je�li to TEXT, otw�rz dialog
                if (newTool == EditTool.Text)
                {
                    var inputWindow = new TextInputWindow();
                    if (inputWindow.ShowDialog() == true)
                    {
                        textInput = inputWindow.InputText;
                        textFontSize = inputWindow.FontSize;
                        currentTool = newTool;
                        DebugHelper.LogDebug($"Tekst: {textInput}, Rozmiar: {textFontSize}");
                    }
                }
                else
                {
                    currentTool = newTool;
                    DebugHelper.LogDebug($"Narzedzie: {currentTool}");
                }
                
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

        private async void OnUploadClick(object sender, RoutedEventArgs e)
        {
            DebugHelper.LogInfo("Editor", "Upload button clicked");
            
            try
            {
                // Sprawdź czy provider jest skonfigurowany
                var provider = SettingsManager.Instance.CloudProvider;
                if (string.IsNullOrWhiteSpace(provider))
                {
                    MessageBox.Show(
                        LocalizationHelper.GetString("Editor_CloudNotConfigured"),
                        LocalizationHelper.GetString("Editor_CloudNotConfiguredTitle"),
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                // Renderuj obraz z edycjami
                var renderTargetBitmap = new RenderTargetBitmap(
                    originalBitmap.PixelWidth,
                    originalBitmap.PixelHeight,
                    96, 96,
                    PixelFormats.Pbgra32);
                renderTargetBitmap.Render(drawingVisual);

                DebugHelper.LogInfo("Editor", $"Screenshot rendered for upload: {renderTargetBitmap.PixelWidth}x{renderTargetBitmap.PixelHeight}px");

                // Wyłącz przycisk podczas uploadu
                if (sender is Button btn)
                {
                    btn.IsEnabled = false;
                    btn.Content = new TextBlock { Text = LocalizationHelper.GetString("Editor_Uploading"), FontSize = 12, FontWeight = FontWeights.Bold };
                }

                var filename = $"Screenshot_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
                var result = await CloudUploadManager.Instance.UploadScreenshotAsync(renderTargetBitmap, filename);

                // Przywróć przycisk
                if (sender is Button button)
                {
                    button.IsEnabled = true;
                    button.Content = new TextBlock { Text = LocalizationHelper.GetString("Editor_Upload"), FontSize = 12, FontWeight = FontWeights.Bold };
                }

                if (result.Success && !string.IsNullOrEmpty(result.Url))
                {
                    // Skopiuj URL do schowka
                    Clipboard.SetText(result.Url);
                    
                    var message = string.Format(LocalizationHelper.GetString("Editor_UploadSuccessMessage"), "\n", result.Url);
                    MessageBox.Show(message, LocalizationHelper.GetString("Editor_UploadSuccess"), MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    DebugHelper.LogInfo("Editor", $"Upload successful: {result.Url}");
                }
                else
                {
                    var errorMsg = result.ErrorMessage ?? LocalizationHelper.GetString("Editor_Error");
                    var message = string.Format(LocalizationHelper.GetString("Editor_UploadErrorMessage"), "\n", errorMsg);
                    MessageBox.Show(message, LocalizationHelper.GetString("Editor_UploadError"), MessageBoxButton.OK, MessageBoxImage.Error);
                    DebugHelper.LogError("Editor", $"Upload failed: {errorMsg}");
                }
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("Editor", "Error during upload", ex);
                var message = string.Format(LocalizationHelper.GetString("Editor_ErrorWithMessage"), ex.Message);
                MessageBox.Show(message, LocalizationHelper.GetString("Editor_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            DebugHelper.LogInfo("Editor", "Save button clicked");
            try
            {
                var renderTargetBitmap = new RenderTargetBitmap(
                    originalBitmap.PixelWidth,
                    originalBitmap.PixelHeight,
                    96, 96,
                    PixelFormats.Pbgra32);
                renderTargetBitmap.Render(drawingVisual);

                DebugHelper.LogInfo("Editor", $"Screenshot rendered: {renderTargetBitmap.PixelWidth}x{renderTargetBitmap.PixelHeight}px");

                // Pokaż dialog do zarządzania tagami/kategoriami
                var saveDialog = new SaveScreenshotDialog();
                saveDialog.Owner = this;
                
                if (saveDialog.ShowDialog() == true)
                {
                    // Zapisz screenshot z metadanymi
                    var item = ScreenshotManager.Instance.AddScreenshotWithMetadata(
                        renderTargetBitmap,
                        saveDialog.Category,
                        saveDialog.Tags,
                        saveDialog.Notes);
                    
                    DebugHelper.LogInfo("Editor", "Screenshot added to manager with metadata");
                    
                    DebugHelper.ShowMessage(
                        LocalizationHelper.GetString("Editor_Success"),
                        LocalizationHelper.GetString("Editor_ScreenshotSaved"));
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("Editor", "Error saving screenshot", ex);
                DebugHelper.ShowMessage("B��d", $"B��d: {ex.Message}");
            }
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnClearClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Czy na pewno chcesz wyczy�ci� wszystkie zmiany?", "Potwierdzenie", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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
        }

        private void OnEditorCanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!isDrawing || currentTool == EditTool.None) return;
            
            isDrawing = false;
            EditorCanvas.ReleaseMouseCapture();
            var endPoint = e.GetPosition(EditorCanvas);
            
            if (currentTool != EditTool.None && (startPoint != endPoint || currentTool == EditTool.Text))
            {
                drawingHistory.Add(new DrawingAction
                {
                    Tool = currentTool,
                    Start = startPoint,
                    End = endPoint,
                    Color = currentColor,
                    Size = currentTool == EditTool.Text ? textFontSize : SizeSlider.Value,
                    TextContent = currentTool == EditTool.Text ? textInput : null
                });
                
                RedrawCanvas();
                currentTool = EditTool.None; // Reset po tek�cie
            }
        }

        private void SizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SizeLabel != null)
            {
                SizeLabel.Text = string.Format(LocalizationHelper.GetString("Editor_SizeLabel"), (int)e.NewValue);
            }
        }

        public void Undo()
        {
            if (drawingHistory.Count > 0)
            {
                drawingHistory.RemoveAt(drawingHistory.Count - 1);
                RedrawCanvas();
                DebugHelper.LogDebug("Undo - ostatnia akcja usuni�ta");
            }
        }

        public void ClearAll()
        {
            drawingHistory.Clear();
            RedrawCanvas();
            DebugHelper.LogDebug("Wszystkie zmiany usuni�te");
        }

        public void SaveScreenshot()
        {
            OnSaveClick(this, new RoutedEventArgs());
        }

        protected override void OnClosed(EventArgs e)
        {
            Dispose();
            base.OnClosed(e);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    if (currentCanvasBrush != null)
                    {
                        currentCanvasBrush = null;
                    }
                    if (currentCanvasBitmap != null)
                    {
                        currentCanvasBitmap = null;
                    }
                    if (drawingVisual != null)
                    {
                        drawingVisual = null;
                    }
                    if (originalBitmap != null && !originalBitmap.IsFrozen)
                    {
                        // Only dispose if not frozen (frozen bitmaps are immutable and safe)
                        originalBitmap = null;
                    }
                    drawingHistory?.Clear();
                }
                disposed = true;
            }
        }
    }
}
