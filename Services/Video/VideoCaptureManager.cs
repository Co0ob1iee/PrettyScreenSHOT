using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using ImageMagick;

namespace PrettyScreenSHOT.Services.Video
{
    /// <summary>
    /// Manager do nagrywania ekranu do GIF/MP4
    /// </summary>
    public class VideoCaptureManager : IDisposable
    {
        private bool isRecording = false;
        private CancellationTokenSource? cancellationTokenSource;
        private List<Bitmap> frames = new();
        private int frameRate = 10; // FPS dla GIF
        private Rectangle captureArea;
        private string outputPath = "";
        private int framesCaptured = 0;

        public bool IsRecording => isRecording;
        public int FramesCount => framesCaptured;
        public int FrameRate 
        { 
            get => frameRate; 
            set => frameRate = Math.Clamp(value, 1, 30); 
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int cx, int cy);

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int x, int y, int cx, int cy, IntPtr hdcSrc, int x1, int y1, uint rop);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// Rozpoczyna nagrywanie ekranu
        /// </summary>
        public void StartRecording(Rectangle area, int fps = 10)
        {
            if (isRecording)
                throw new InvalidOperationException("Recording is already in progress");

            captureArea = area;
            frameRate = fps;
            frames.Clear();
            framesCaptured = 0;
            isRecording = true;
            cancellationTokenSource = new CancellationTokenSource();

            DebugHelper.LogInfo("VideoCapture", $"Recording started: {area.Width}x{area.Height} @ {fps} FPS");
        }

        /// <summary>
        /// Zatrzymuje nagrywanie i zapisuje do pliku
        /// </summary>
        public async Task<string> StopRecordingAsync(string outputPath, VideoFormat format = VideoFormat.GIF)
        {
            if (!isRecording)
                throw new InvalidOperationException("No recording in progress");

            isRecording = false;
            cancellationTokenSource?.Cancel();

            this.outputPath = outputPath;

            DebugHelper.LogInfo("VideoCapture", $"Stopping recording: {frames.Count} frames");

            string resultPath = format switch
            {
                VideoFormat.GIF => await SaveAsGifAsync(outputPath),
                VideoFormat.MP4 => await SaveAsMp4Async(outputPath),
                _ => throw new NotSupportedException($"Format {format} is not supported")
            };

            // Zwolnij pamięć
            foreach (var frame in frames)
            {
                frame?.Dispose();
            }
            frames.Clear();
            framesCaptured = 0;

            return resultPath;
        }

        /// <summary>
        /// Nagrywa jedną klatkę
        /// </summary>
        public void CaptureFrame()
        {
            if (!isRecording) return;

            try
            {
                var frame = CaptureScreenRegion(captureArea);
                if (frame != null)
                {
                    lock (frames)
                    {
                        frames.Add(frame);
                        framesCaptured++;
                    }
                }
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("VideoCapture", "Error capturing frame", ex);
            }
        }

        /// <summary>
        /// Rozpoczyna automatyczne nagrywanie klatek
        /// </summary>
        public async Task RecordFramesAsync()
        {
            if (!isRecording || cancellationTokenSource == null)
                return;

            int delayMs = 1000 / frameRate;

            try
            {
                while (isRecording && !cancellationTokenSource.Token.IsCancellationRequested)
                {
                    CaptureFrame();
                    await Task.Delay(delayMs, cancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // Normalne zakończenie
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("VideoCapture", "Error in recording loop", ex);
            }
        }

        private Bitmap CaptureScreenRegion(Rectangle area)
        {
            IntPtr screenDC = GetDC(IntPtr.Zero);
            IntPtr memDC = CreateCompatibleDC(screenDC);
            IntPtr memBitmap = CreateCompatibleBitmap(screenDC, area.Width, area.Height);
            IntPtr oldBitmap = SelectObject(memDC, memBitmap);

            BitBlt(memDC, 0, 0, area.Width, area.Height, screenDC, area.X, area.Y, 0x00CC0020);

            Bitmap bitmap = new Bitmap(area.Width, area.Height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(bitmap);
            IntPtr hdc = graphics.GetHdc();
            BitBlt(hdc, 0, 0, area.Width, area.Height, memDC, 0, 0, 0x00CC0020);
            graphics.ReleaseHdc(hdc);
            graphics.Dispose();

            SelectObject(memDC, oldBitmap);
            DeleteDC(memDC);
            ReleaseDC(IntPtr.Zero, screenDC);
            DeleteObject(memBitmap);

            return bitmap;
        }

        private async Task<string> SaveAsGifAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                if (frames.Count == 0)
                    throw new InvalidOperationException("No frames to save");

                DebugHelper.LogInfo("VideoCapture", $"Saving {frames.Count} frames as animated GIF using Magick.NET");

                try
                {
                    using (var collection = new MagickImageCollection())
                    {
                        int delay = 100 / frameRate; // Delay w setnych sekundy (100 = 1 sekunda)

                        foreach (var frame in frames)
                        {
                            using (var ms = new MemoryStream())
                            {
                                frame.Save(ms, ImageFormat.Png);
                                ms.Position = 0;

                                var magickImage = new MagickImage(ms);
                                magickImage.AnimationDelay = (uint)delay;
                                collection.Add(magickImage);
                            }
                        }

                        // Ustaw opcje animacji
                        collection[0].AnimationIterations = 0; // 0 = nieskończona pętla
                        
                        // Zapisz jako animowany GIF
                        var settings = new QuantizeSettings
                        {
                            Colors = 256
                        };
                        collection.Quantize(settings);
                        collection.Optimize();

                        collection.Write(filePath);
                    }

                    DebugHelper.LogInfo("VideoCapture", $"GIF saved successfully: {filePath}");
                    return filePath;
                }
                catch (Exception ex)
                {
                    DebugHelper.LogError("VideoCapture", "Error saving GIF with Magick.NET", ex);
                    throw;
                }
            });
        }

        private async Task<string> SaveAsMp4Async(string filePath)
        {
            return await Task.Run(() =>
            {
                if (frames.Count == 0)
                    throw new InvalidOperationException("No frames to save");

                // Sprawdź czy FFmpeg jest dostępny
                string ffmpegPath = GetFFmpegPath();
                if (string.IsNullOrEmpty(ffmpegPath) || !File.Exists(ffmpegPath))
                {
                    throw new FileNotFoundException(
                        "FFmpeg not found. Please install FFmpeg and configure path in settings.\n" +
                        "Download from: https://ffmpeg.org/download.html");
                }

                DebugHelper.LogInfo("VideoCapture", $"Saving {frames.Count} frames as MP4 using FFmpeg");

                try
                {
                    // Zapisz klatki jako tymczasowe pliki PNG
                    string tempDir = Path.Combine(Path.GetTempPath(), $"VideoCapture_{Guid.NewGuid()}");
                    Directory.CreateDirectory(tempDir);

                    try
                    {
                        // Zapisz wszystkie klatki
                        for (int i = 0; i < frames.Count; i++)
                        {
                            string framePath = Path.Combine(tempDir, $"frame_{i:D6}.png");
                            frames[i].Save(framePath, ImageFormat.Png);
                        }

                        // Wywołaj FFmpeg
                        string framePattern = Path.Combine(tempDir, "frame_%06d.png");
                        string ffmpegArgs = $"-y -framerate {frameRate} -i \"{framePattern}\" " +
                                          $"-c:v libx264 -pix_fmt yuv420p -crf 23 \"{filePath}\"";

                        var processInfo = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = ffmpegPath,
                            Arguments = ffmpegArgs,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true
                        };

                        using (var process = System.Diagnostics.Process.Start(processInfo))
                        {
                            if (process == null)
                                throw new Exception("Failed to start FFmpeg process");

                            process.WaitForExit();

                            if (process.ExitCode != 0)
                            {
                                string error = process.StandardError.ReadToEnd();
                                throw new Exception($"FFmpeg error: {error}");
                            }
                        }

                        DebugHelper.LogInfo("VideoCapture", $"MP4 saved successfully: {filePath}");
                        return filePath;
                    }
                    finally
                    {
                        // Usuń tymczasowe pliki
                        try
                        {
                            Directory.Delete(tempDir, true);
                        }
                        catch { }
                    }
                }
                catch (Exception ex)
                {
                    DebugHelper.LogError("VideoCapture", "Error saving MP4 with FFmpeg", ex);
                    throw;
                }
            });
        }

        private string GetFFmpegPath()
        {
            // Sprawdź ustawienia
            var settings = SettingsManager.Instance;
            if (!string.IsNullOrEmpty(settings.FFmpegPath) && File.Exists(settings.FFmpegPath))
            {
                return settings.FFmpegPath;
            }

            // Sprawdź standardowe lokalizacje
            var commonPaths = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "ffmpeg", "bin", "ffmpeg.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "ffmpeg", "bin", "ffmpeg.exe"),
                "ffmpeg.exe" // W PATH
            };

            foreach (var path in commonPaths)
            {
                if (File.Exists(path))
                    return path;
            }

            return "";
        }

        public void Dispose()
        {
            if (isRecording)
            {
                cancellationTokenSource?.Cancel();
            }

            cancellationTokenSource?.Dispose();

            foreach (var frame in frames)
            {
                frame?.Dispose();
            }
            frames.Clear();
        }

        public enum VideoFormat
        {
            GIF,
            MP4
        }
    }
}
