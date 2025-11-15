using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Point = System.Windows.Point;
using Color = System.Windows.Media.Color;
using Pen = System.Windows.Media.Pen;
using Forms = System.Windows.Forms; // alias to avoid ambiguity with System.Windows
using WindowsFontStyle = System.Windows.FontStyle;
using WindowsFontWeight = System.Windows.FontWeight;
using WindowsFontFamily = System.Windows.Media.FontFamily;

namespace PrettyScreenSHOT
{
    public partial class ScreenshotEditorWindow : Window, IDisposable
    {
        private BitmapSource? originalBitmap;
        private DrawingVisual? drawingVisual;
        private EditTool currentTool = EditTool.None;
        private Point startPoint;
        private Point currentPoint;
        private bool isDrawing = false;
        private Color currentColor = Colors.Red;
        private List<DrawingAction> drawingHistory = new();
        private RenderTargetBitmap? currentCanvasBitmap;
        private ImageBrush? currentCanvasBrush;
        private bool disposed = false;
        private System.Windows.Controls.Button? activeToolButton = null;

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

            // Dodatkowe właściwości formatowania tekstu
            public WindowsFontWeight FontWeight { get; set; } = FontWeights.Normal;
            public WindowsFontStyle FontStyle { get; set; } = FontStyles.Normal;
            public TextDecorationCollection TextDecorations { get; set; } = new TextDecorationCollection();
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

            // WPF UI applies themes globally - no need to apply per-window

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
            if (originalBitmap == null) return;
            EditorCanvas.Width = originalBitmap.PixelWidth;
            EditorCanvas.Height = originalBitmap.PixelHeight;
            RedrawCanvas();
        }

        private void RedrawCanvas()
        {
            if (drawingVisual == null || originalBitmap == null) return;
            var drawingContext = drawingVisual.RenderOpen();
            var brush = new ImageBrush(originalBitmap);
            drawingContext.DrawRectangle(brush, null, new Rect(0, 0, originalBitmap.PixelWidth, originalBitmap.PixelHeight));
            
            // Narysuj wszystkie wcześniejsze akcje
            foreach (var action in drawingHistory)
            {
                DrawAction(drawingContext, action);
            }
            
            drawingContext.Close();

            if (originalBitmap == null || drawingVisual == null) return;
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
                    if (rect.Width > 0 && rect.Height > 0)
                    {
                        DrawBlur(drawingContext, rect, action.Size);
                    }
                    else
                    {
                        // Dla bardzo małych obszarów, narysuj prostokąt
                        drawingContext.DrawRectangle(null, pen, rect);
                    }
                    break;
                case EditTool.Arrow:
                    DrawArrow(drawingContext, action.Start, action.End, pen);
                    break;
                case EditTool.Text:
                    DrawTextInRect(drawingContext, new Rect(action.Start, action.End), action.TextContent ?? "",
                                 action.Color, action.Size, action.FontWeight, action.FontStyle, action.TextDecorations);
                    break;
            }
        }

        private void DrawBlur(DrawingContext drawingContext, Rect rect, double blurStrength)
        {
            // Prawdziwy blur - rozmycie rzeczywistych pikseli ze screenshotu
            // Dzięki temu ukrywamy dane ale pozostaje widoczna ogólna struktura

            // Sprawdź czy prostokąt ma prawidłowe wymiary
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                DebugHelper.LogInfo("DrawBlur", $"Invalid rect dimensions: Width={rect.Width}, Height={rect.Height}");
                return;
            }

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
                            double kernelValueX = kernel[kx + radius];
                            double kernelValueY = kernel[ky + radius];
                            double kernelValue = kernelValueX * kernelValueY;
							
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
                System.Windows.FlowDirection.LeftToRight,
                typeface,
                fontSize,
                new SolidColorBrush(color),
                1.0);

            drawingContext.DrawText(formattedText, position);
        }

        private void DrawTextInRect(DrawingContext drawingContext, Rect textRect, string text, Color color, double fontSize,
                                   WindowsFontWeight fontWeight = default, WindowsFontStyle fontStyle = default,
                                   TextDecorationCollection textDecorations = null)
        {
            if (string.IsNullOrEmpty(text) || textRect.Width <= 0 || textRect.Height <= 0) return;

            // Użyj domyślnych wartości jeśli nie zostały przekazane
            fontWeight = fontWeight == FontWeights.Normal ? FontWeights.Normal : fontWeight;
            fontStyle = fontStyle == FontStyles.Normal ? FontStyles.Normal : fontStyle;
            textDecorations = textDecorations ?? new TextDecorationCollection();

            var typeface = new Typeface(new WindowsFontFamily("Arial"), fontStyle, fontWeight, FontStretches.Normal);

            // Dostosuj rozmiar czcionki do prostokąta jeśli jest za duża
            var adjustedFontSize = Math.Min(fontSize, textRect.Height * 0.9);

            var formattedText = new FormattedText(
                text,
                System.Globalization.CultureInfo.CurrentCulture,
                System.Windows.FlowDirection.LeftToRight,
                typeface,
                adjustedFontSize,
                new SolidColorBrush(color),
                1.0);

            // Dodaj dekoracje tekstu
            formattedText.SetTextDecorations(textDecorations);

            // Wyśrodkuj tekst w prostokącie
            var textWidth = formattedText.Width;
            var textHeight = formattedText.Height;

            // Jeśli tekst jest za szeroki, zmniejsz czcionkę proporcjonalnie
            if (textWidth > textRect.Width)
            {
                adjustedFontSize = adjustedFontSize * (textRect.Width / textWidth);
                formattedText.SetFontSize(adjustedFontSize);
                textWidth = formattedText.Width;
                textHeight = formattedText.Height;
            }

            var textX = textRect.X + (textRect.Width - textWidth) / 2;
            var textY = textRect.Y + (textRect.Height - textHeight) / 2;

            drawingContext.DrawText(formattedText, new Point(textX, textY));
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
            if (sender is System.Windows.Controls.Button btn)
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

                // Jeśli to TEXT, przygotuj do zaznaczania obszaru
                if (newTool == EditTool.Text)
                {
                    DebugHelper.LogDebug("Narzędzie Text wybrane - zaznacz obszar na screenshot");
                    currentTool = newTool;
                    UpdateActiveToolButton(btn);
                    DebugHelper.LogDebug("Zaznacz prostokąt dla tekstu na screenshot");
                }
                else
                {
                    currentTool = newTool;
                    UpdateActiveToolButton(btn);
                    DebugHelper.LogDebug($"Narzędzie: {currentTool}");
                }
            }
        }

        private void UpdateActiveToolButton(System.Windows.Controls.Button? newActiveButton)
        {
            // Resetuj poprzedni aktywny przycisk
            if (activeToolButton != null)
            {
                activeToolButton.BorderThickness = new Thickness(0);
                activeToolButton.BorderBrush = new SolidColorBrush(Colors.Transparent);
                activeToolButton.Background = new SolidColorBrush(Color.FromRgb(0x3F, 0x3F, 0x3F));
            }

            // Ustaw nowy aktywny przycisk
            activeToolButton = newActiveButton;
            if (activeToolButton != null)
            {
                activeToolButton.BorderThickness = new Thickness(2);
                activeToolButton.BorderBrush = new SolidColorBrush(Colors.Yellow);
                activeToolButton.Background = new SolidColorBrush(Color.FromRgb(0x5F, 0x5F, 0x5F));
            }
        }

        private void OnColorPickerClick(object sender, RoutedEventArgs e)
        {
            var colorDialog = new Forms.ColorDialog();
            colorDialog.Color = System.Drawing.Color.FromArgb(currentColor.R, currentColor.G, currentColor.B);

            if (colorDialog.ShowDialog() == Forms.DialogResult.OK)
            {
                currentColor = System.Windows.Media.Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
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
                    System.Windows.MessageBox.Show(
                        LocalizationHelper.GetString("Editor_CloudNotConfigured"),
                        LocalizationHelper.GetString("Editor_CloudNotConfiguredTitle"),
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                // Renderuj obraz z edycjami
                if (originalBitmap == null || drawingVisual == null) return;
                var renderTargetBitmap = new RenderTargetBitmap(
                    originalBitmap.PixelWidth,
                    originalBitmap.PixelHeight,
                    96, 96,
                    PixelFormats.Pbgra32);
                renderTargetBitmap.Render(drawingVisual);

                DebugHelper.LogInfo("Editor", $"Screenshot rendered for upload: {renderTargetBitmap.PixelWidth}x{renderTargetBitmap.PixelHeight}px");

                // Wyłącz przycisk podczas uploadu
                if (sender is System.Windows.Controls.Button btn)
                {
                    btn.IsEnabled = false;
                    btn.Content = new TextBlock { Text = LocalizationHelper.GetString("Editor_Uploading"), FontSize = 12, FontWeight = FontWeights.Bold };
                }

                var filename = $"Screenshot_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
                var result = await CloudUploadManager.Instance.UploadScreenshotAsync(renderTargetBitmap, filename);

                // Przywróć przycisk
                if (sender is System.Windows.Controls.Button button)
                {
                    button.IsEnabled = true;
                    button.Content = new TextBlock { Text = LocalizationHelper.GetString("Editor_Upload"), FontSize = 12, FontWeight = FontWeights.Bold };
                }

                if (result.Success && !string.IsNullOrEmpty(result.Url))
                {
                    // Skopiuj URL do schowka
                    System.Windows.Clipboard.SetText(result.Url);
                    
                    var message = string.Format(LocalizationHelper.GetString("Editor_UploadSuccessMessage"), "\n", result.Url);
                    System.Windows.MessageBox.Show(message, LocalizationHelper.GetString("Editor_UploadSuccess"), MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    DebugHelper.LogInfo("Editor", $"Upload successful: {result.Url}");
                }
                else
                {
                    var errorMsg = result.ErrorMessage ?? LocalizationHelper.GetString("Editor_Error");
                    var message = string.Format(LocalizationHelper.GetString("Editor_UploadErrorMessage"), "\n", errorMsg);
                    System.Windows.MessageBox.Show(message, LocalizationHelper.GetString("Editor_UploadError"), MessageBoxButton.OK, MessageBoxImage.Error);
                    DebugHelper.LogError("Editor", $"Upload failed: {errorMsg}");
                }
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("Editor", "Error during upload", ex);
                var message = string.Format(LocalizationHelper.GetString("Editor_ErrorWithMessage"), ex.Message);
                System.Windows.MessageBox.Show(message, LocalizationHelper.GetString("Editor_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            DebugHelper.LogInfo("Editor", "Save button clicked");
            try
            {
                if (originalBitmap == null || drawingVisual == null) return;
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
                DebugHelper.ShowMessage("Błąd", $"Błąd: {ex.Message}");
            }
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnClearClick(object sender, RoutedEventArgs e)
        {
            if (System.Windows.MessageBox.Show("Czy na pewno chcesz wyczyścić wszystkie zmiany?", "Potwierdzenie", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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

        private void OnEditorCanvasMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!isDrawing || currentTool == EditTool.None) return;

            currentPoint = e.GetPosition(EditorCanvas);

            // Dodaj wizualny feedback podczas rysowania
            RedrawCanvasWithPreview();
        }

        private void RedrawCanvasWithPreview()
        {
            if (drawingVisual == null || originalBitmap == null) return;

            var drawingContext = drawingVisual.RenderOpen();
            var brush = new ImageBrush(originalBitmap);
            drawingContext.DrawRectangle(brush, null, new Rect(0, 0, originalBitmap.PixelWidth, originalBitmap.PixelHeight));

            // Narysuj wszystkie wcześniejsze akcje
            foreach (var action in drawingHistory)
            {
                DrawAction(drawingContext, action);
            }

            // Narysuj podgląd aktualnej akcji
            if (isDrawing && currentTool != EditTool.None && startPoint != currentPoint)
            {
                DrawPreviewAction(drawingContext);
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

        private void DrawPreviewAction(DrawingContext drawingContext)
        {
            var pen = new Pen(new SolidColorBrush(currentColor), SizeSlider.Value)
            {
                StartLineCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round,
                LineJoin = PenLineJoin.Round
            };

            var rect = new Rect(startPoint, currentPoint);

            switch (currentTool)
            {
                case EditTool.Marker:
                    drawingContext.DrawLine(pen, startPoint, currentPoint);
                    break;
                case EditTool.Rectangle:
                    drawingContext.DrawRectangle(null, pen, rect);
                    break;
                case EditTool.Blur:
                    // Sprawdź czy prostokąt ma prawidłowe wymiary przed wywołaniem DrawBlur
                    if (rect.Width > 0 && rect.Height > 0)
                    {
                        DrawBlur(drawingContext, rect, SizeSlider.Value);
                    }
                    else
                    {
                        // Dla bardzo małych obszarów, narysuj prostokąt jako wskazówkę
                        drawingContext.DrawRectangle(null, pen, rect);
                    }
                    break;
                case EditTool.Arrow:
                    DrawArrow(drawingContext, startPoint, currentPoint, pen);
                    break;
                case EditTool.Text:
                    // Dla tekstu pokaż zaznaczony prostokąt
                    drawingContext.DrawRectangle(new SolidColorBrush(Color.FromArgb(64, currentColor.R, currentColor.G, currentColor.B)), new Pen(new SolidColorBrush(currentColor), 1), rect);
                    // Dodaj wskazówkę tekstową
                    var centerX = (startPoint.X + currentPoint.X) / 2;
                    var centerY = (startPoint.Y + currentPoint.Y) / 2;
                    var hintText = new FormattedText(
                        "TEXT",
                        System.Globalization.CultureInfo.CurrentCulture,
                        System.Windows.FlowDirection.LeftToRight,
                        new Typeface("Arial"),
                        12,
                        new SolidColorBrush(currentColor),
                        1.0);
                    drawingContext.DrawText(hintText, new Point(centerX - 15, centerY - 8));
                    break;
            }
        }

        private void OnEditorCanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!isDrawing || currentTool == EditTool.None) return;
            
            isDrawing = false;
            EditorCanvas.ReleaseMouseCapture();
            var endPoint = e.GetPosition(EditorCanvas);
            
            if (currentTool != EditTool.None && (startPoint != endPoint || currentTool == EditTool.Text))
            {
                // Dla narzędzi rysujących obszary (jak Blur), sprawdź czy obszar ma prawidłowe wymiary
                if (currentTool == EditTool.Blur)
                {
                    var rect = new Rect(startPoint, endPoint);
                    if (rect.Width <= 0 || rect.Height <= 0)
                    {
                        DebugHelper.LogInfo("Editor", "Blur action skipped - invalid dimensions");
                        return;
                    }
                }

                // Dla TEXT - najpierw zaznacz obszar, potem otwórz dialog
                if (currentTool == EditTool.Text)
                {
                    var textRect = new Rect(startPoint, endPoint);
                    if (textRect.Width > 10 && textRect.Height > 10) // Minimalny rozmiar dla tekstu
                    {
                        DebugHelper.LogDebug("Otwieram okno wprowadzania tekstu po zaznaczeniu obszaru");
                        var inputWindow = new TextInputWindow();

                        // Automatycznie dostosuj rozmiar czcionki do wielkości prostokąta
                        var suggestedFontSize = Math.Max(12, Math.Min(72, (int)(textRect.Height * 0.8)));
                        inputWindow.SetSuggestedFontSize(suggestedFontSize);

                        var result = inputWindow.ShowDialog();
                        DebugHelper.LogDebug($"TextInputWindow.ShowDialog() zwrócił: {result}");

                        if (result == true)
                        {
                            var userText = inputWindow.InputText;
                            var userFontSize = inputWindow.FontSize;
                            DebugHelper.LogDebug($"Tekst wprowadzony: '{userText}', Rozmiar: {userFontSize}");

                            // Teraz dodaj akcję do historii z wprowadzonym tekstem
                            drawingHistory.Add(new DrawingAction
                            {
                                Tool = currentTool,
                                Start = startPoint,
                                End = endPoint,
                                Color = inputWindow.TextColor,
                                Size = userFontSize,
                                TextContent = userText,
                                FontWeight = inputWindow.FontWeight,
                                FontStyle = inputWindow.FontStyle,
                                TextDecorations = inputWindow.TextDecorations
                            });

                            RedrawCanvas();
                        }

                        // Reset narzędzia po zakończeniu
                        currentTool = EditTool.None;
                        UpdateActiveToolButton(null);
                    }
                    else
                    {
                        DebugHelper.LogDebug("Zaznaczony obszar dla tekstu jest za mały");
                    }
                    return;
                }

                drawingHistory.Add(new DrawingAction
                {
                    Tool = currentTool,
                    Start = startPoint,
                    End = endPoint,
                    Color = currentColor,
                    Size = SizeSlider.Value,
                    TextContent = null
                });

                RedrawCanvas();

                // Reset tylko dla narzędzi jednorazowych
                if (currentTool == EditTool.Text)
                {
                    currentTool = EditTool.None;
                }
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
                DebugHelper.LogDebug("Undo - ostatnia akcja usunięta");
            }
        }

        public void ClearAll()
        {
            drawingHistory.Clear();
            RedrawCanvas();
                DebugHelper.LogDebug("Wszystkie zmiany usunięte");
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

        private void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
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
