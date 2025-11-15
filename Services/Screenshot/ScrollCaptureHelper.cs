using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace PrettyScreenSHOT
{
    /// <summary>
    /// Helper do przechwytywania długich stron (scroll capture)
    /// </summary>
    public static class ScrollCaptureHelper
    {
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(POINT point);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        private const uint WM_VSCROLL = 0x0115;
        private const uint WM_HSCROLL = 0x0114;
        private const int SB_LINEDOWN = 1;
        private const int SB_LINEUP = 0;
        private const int SB_PAGEDOWN = 3;
        private const int SB_PAGEUP = 2;

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        /// <summary>
        /// Przechwytuje długą stronę przez automatyczne przewijanie
        /// </summary>
        public static async Task<BitmapSource> CaptureScrollableAreaAsync(
            Rectangle initialArea, 
            ScrollDirection direction = ScrollDirection.Vertical,
            int maxScrolls = 20)
        {
            DebugHelper.LogInfo("ScrollCapture", $"Starting scroll capture: {initialArea.Width}x{initialArea.Height}, direction: {direction}");

            var screenshots = new List<BitmapSource>();
            var currentArea = initialArea;

            try
            {
                // Przechwyć początkowy obszar
                var firstScreenshot = ScreenshotHelper.CaptureScreenRegion(
                    currentArea.X, currentArea.Y, currentArea.Width, currentArea.Height);
                screenshots.Add(firstScreenshot);

                // Znajdź okno do przewijania
                var windowHandle = GetWindowAtPoint(new System.Drawing.Point(
                    currentArea.X + currentArea.Width / 2,
                    currentArea.Y + currentArea.Height / 2));

                if (windowHandle == IntPtr.Zero)
                {
                    DebugHelper.LogError("ScrollCapture", "Could not find window to scroll");
                    return firstScreenshot; // Zwróć tylko pierwszy screenshot
                }

                // Przewijaj i przechwytuj
                BitmapSource? previousScreenshot = firstScreenshot;
                for (int i = 0; i < maxScrolls; i++)
                {
                    // Przewiń
                    bool scrolled = ScrollWindow(windowHandle, direction);
                    
                    if (!scrolled)
                    {
                        DebugHelper.LogInfo("ScrollCapture", $"No more content to scroll after {i} scrolls");
                        break;
                    }

                    // Poczekaj na render
                    await Task.Delay(500); // Zwiększony delay dla lepszej stabilności

                    // Przechwyć następny obszar
                    var screenshot = ScreenshotHelper.CaptureScreenRegion(
                        currentArea.X, currentArea.Y, currentArea.Width, currentArea.Height);
                    
                    // Ulepszone porównanie - sprawdź czy zawartość się zmieniła
                    double similarity = CalculateSimilarity(previousScreenshot, screenshot);
                    
                    if (similarity > 0.98) // 98% podobieństwa = prawdopodobnie ten sam obraz
                    {
                        DebugHelper.LogInfo("ScrollCapture", $"Reached end of scrollable content (similarity: {similarity:P2})");
                        break;
                    }

                    screenshots.Add(screenshot);
                    previousScreenshot = screenshot;
                }

                // Połącz wszystkie screenshoty
                return CombineScreenshots(screenshots, direction);
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("ScrollCapture", "Error during scroll capture", ex);
                return screenshots.Count > 0 ? screenshots[0] : 
                    ScreenshotHelper.CaptureScreenRegion(
                        initialArea.X, initialArea.Y, initialArea.Width, initialArea.Height);
            }
        }

        private static IntPtr GetWindowAtPoint(System.Drawing.Point point)
        {
            POINT pt = new POINT { X = point.X, Y = point.Y };
            return WindowFromPoint(pt);
        }

        private static bool ScrollWindow(IntPtr hWnd, ScrollDirection direction)
        {
            if (hWnd == IntPtr.Zero)
                return false;

            uint scrollMsg = direction == ScrollDirection.Vertical ? WM_VSCROLL : WM_HSCROLL;
            IntPtr scrollParam = new IntPtr(SB_PAGEDOWN); // Przewiń o jedną stronę w dół

            SendMessage(hWnd, scrollMsg, scrollParam, IntPtr.Zero);
            return true;
        }

        /// <summary>
        /// Ulepszone porównanie screenshotów używając histogramów i porównania pikseli
        /// </summary>
        private static double CalculateSimilarity(BitmapSource img1, BitmapSource img2)
        {
            if (img1.PixelWidth != img2.PixelWidth || img1.PixelHeight != img2.PixelHeight)
                return 0.0;

            try
            {
                // Metoda 1: Porównanie histogramów kolorów
                double histogramSimilarity = CompareHistograms(img1, img2);
                
                // Metoda 2: Porównanie próbek pikseli
                double pixelSimilarity = ComparePixelSamples(img1, img2);
                
                // Metoda 3: Porównanie struktury (edge detection - uproszczone)
                double structureSimilarity = CompareStructure(img1, img2);
                
                // Średnia ważona wszystkich metod
                double finalSimilarity = (histogramSimilarity * 0.4) + 
                                       (pixelSimilarity * 0.4) + 
                                       (structureSimilarity * 0.2);
                
                return finalSimilarity;
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("ScrollCapture", "Error calculating similarity", ex);
                return 0.0;
            }
        }

        /// <summary>
        /// Porównuje histogramy kolorów dwóch obrazów
        /// </summary>
        private static double CompareHistograms(BitmapSource img1, BitmapSource img2)
        {
            int[] hist1 = CalculateHistogram(img1);
            int[] hist2 = CalculateHistogram(img2);

            // Oblicz korelację histogramów
            double correlation = 0.0;
            double sum1 = 0, sum2 = 0, sum1Sq = 0, sum2Sq = 0, sumProd = 0;
            int n = hist1.Length;

            for (int i = 0; i < n; i++)
            {
                sum1 += hist1[i];
                sum2 += hist2[i];
                sum1Sq += hist1[i] * hist1[i];
                sum2Sq += hist2[i] * hist2[i];
                sumProd += hist1[i] * hist2[i];
            }

            double num = sumProd - (sum1 * sum2 / n);
            double den = Math.Sqrt((sum1Sq - sum1 * sum1 / n) * (sum2Sq - sum2 * sum2 / n));

            if (den != 0)
            {
                correlation = num / den;
            }

            // Normalizuj do zakresu 0-1
            return (correlation + 1) / 2;
        }

        /// <summary>
        /// Oblicza histogram kolorów (uproszczony - używa jasności)
        /// </summary>
        private static int[] CalculateHistogram(BitmapSource image)
        {
            int[] histogram = new int[256];
            int stride = image.PixelWidth * ((image.Format.BitsPerPixel + 7) / 8);
            byte[] pixels = new byte[stride * image.PixelHeight];
            image.CopyPixels(pixels, stride, 0);

            int bytesPerPixel = image.Format.BitsPerPixel / 8;
            for (int i = 0; i < pixels.Length; i += bytesPerPixel)
            {
                // Oblicz jasność (uproszczone)
                byte r = pixels[i + 2];
                byte g = pixels[i + 1];
                byte b = pixels[i];
                int brightness = (int)(0.299 * r + 0.587 * g + 0.114 * b);
                histogram[brightness]++;
            }

            return histogram;
        }

        /// <summary>
        /// Porównuje próbki pikseli z różnych obszarów obrazu
        /// </summary>
        private static double ComparePixelSamples(BitmapSource img1, BitmapSource img2)
        {
            int samplePoints = 100; // Zwiększona liczba próbek
            int matches = 0;
            int threshold = 10; // Próg różnicy kolorów

            var random = new Random(42); // Stały seed dla powtarzalności

            int stride1 = img1.PixelWidth * ((img1.Format.BitsPerPixel + 7) / 8);
            int stride2 = img2.PixelWidth * ((img2.Format.BitsPerPixel + 7) / 8);
            byte[] pixels1 = new byte[stride1 * img1.PixelHeight];
            byte[] pixels2 = new byte[stride2 * img2.PixelHeight];
            img1.CopyPixels(pixels1, stride1, 0);
            img2.CopyPixels(pixels2, stride2, 0);

            int bytesPerPixel = img1.Format.BitsPerPixel / 8;

            for (int i = 0; i < samplePoints; i++)
            {
                int x = random.Next(0, img1.PixelWidth);
                int y = random.Next(0, img1.PixelHeight);

                int index1 = (y * stride1) + (x * bytesPerPixel);
                int index2 = (y * stride2) + (x * bytesPerPixel);

                // Porównaj RGB
                int diffR = Math.Abs(pixels1[index1 + 2] - pixels2[index2 + 2]);
                int diffG = Math.Abs(pixels1[index1 + 1] - pixels2[index2 + 1]);
                int diffB = Math.Abs(pixels1[index1] - pixels2[index2]);

                if (diffR <= threshold && diffG <= threshold && diffB <= threshold)
                {
                    matches++;
                }
            }

            return (double)matches / samplePoints;
        }

        /// <summary>
        /// Porównuje strukturę obrazu (uproszczone wykrywanie krawędzi)
        /// </summary>
        private static double CompareStructure(BitmapSource img1, BitmapSource img2)
        {
            // Uproszczone porównanie struktury - porównaj gradienty
            // W rzeczywistości można użyć bardziej zaawansowanych metod jak SSIM
            
            int sampleRegions = 20;
            int regionSize = 50;
            int matches = 0;

            var random = new Random(42);

            for (int i = 0; i < sampleRegions; i++)
            {
                int x = random.Next(0, Math.Max(0, img1.PixelWidth - regionSize));
                int y = random.Next(0, Math.Max(0, img1.PixelHeight - regionSize));

                // Porównaj średnią jasność regionu
                double avg1 = CalculateRegionBrightness(img1, x, y, regionSize);
                double avg2 = CalculateRegionBrightness(img2, x, y, regionSize);

                if (Math.Abs(avg1 - avg2) < 5) // Próg różnicy
                {
                    matches++;
                }
            }

            return (double)matches / sampleRegions;
        }

        private static double CalculateRegionBrightness(BitmapSource image, int startX, int startY, int size)
        {
            int stride = image.PixelWidth * ((image.Format.BitsPerPixel + 7) / 8);
            byte[] pixels = new byte[stride * image.PixelHeight];
            image.CopyPixels(pixels, stride, 0);

            int bytesPerPixel = image.Format.BitsPerPixel / 8;
            long totalBrightness = 0;
            int pixelCount = 0;

            for (int y = startY; y < Math.Min(startY + size, image.PixelHeight); y++)
            {
                for (int x = startX; x < Math.Min(startX + size, image.PixelWidth); x++)
                {
                    int index = (y * stride) + (x * bytesPerPixel);
                    byte r = pixels[index + 2];
                    byte g = pixels[index + 1];
                    byte b = pixels[index];
                    int brightness = (int)(0.299 * r + 0.587 * g + 0.114 * b);
                    totalBrightness += brightness;
                    pixelCount++;
                }
            }

            return pixelCount > 0 ? (double)totalBrightness / pixelCount : 0;
        }

        private static BitmapSource CombineScreenshots(List<BitmapSource> screenshots, ScrollDirection direction)
        {
            if (screenshots.Count == 0)
                throw new ArgumentException("No screenshots to combine");

            if (screenshots.Count == 1)
                return screenshots[0];

            int totalWidth = screenshots[0].PixelWidth;
            int totalHeight = screenshots[0].PixelHeight;

            if (direction == ScrollDirection.Vertical)
            {
                // Połącz pionowo
                totalHeight = screenshots[0].PixelHeight * screenshots.Count;
            }
            else
            {
                // Połącz poziomo
                totalWidth = screenshots[0].PixelWidth * screenshots.Count;
            }

            var renderTarget = new RenderTargetBitmap(
                totalWidth, totalHeight, 96, 96, PixelFormats.Pbgra32);

            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                int offset = 0;
                foreach (var screenshot in screenshots)
                {
                    if (direction == ScrollDirection.Vertical)
                    {
                        drawingContext.DrawImage(screenshot, 
                            new Rect(0, offset, screenshot.PixelWidth, screenshot.PixelHeight));
                        offset += screenshot.PixelHeight;
                    }
                    else
                    {
                        drawingContext.DrawImage(screenshot, 
                            new Rect(offset, 0, screenshot.PixelWidth, screenshot.PixelHeight));
                        offset += screenshot.PixelWidth;
                    }
                }
            }

            renderTarget.Render(drawingVisual);
            renderTarget.Freeze();

            return renderTarget;
        }

        public enum ScrollDirection
        {
            Vertical,
            Horizontal
        }
    }
}
