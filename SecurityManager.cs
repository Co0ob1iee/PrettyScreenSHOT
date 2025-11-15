using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace PrettyScreenSHOT
{
    /// <summary>
    /// Manager do bezpieczeństwa - szyfrowanie i watermarking
    /// </summary>
    public static class SecurityManager
    {
        private const int KeySize = 32; // 256 bits dla AES-256
        private const int SaltSize = 16; // 128 bits dla salt
        private const int Iterations = 100000; // PBKDF2 iterations

        /// <summary>
        /// Generuje losowy klucz szyfrowania
        /// </summary>
        public static byte[] GenerateRandomKey()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] key = new byte[KeySize];
                rng.GetBytes(key);
                return key;
            }
        }

        /// <summary>
        /// Generuje losowy salt
        /// </summary>
        public static byte[] GenerateSalt()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] salt = new byte[SaltSize];
                rng.GetBytes(salt);
                return salt;
            }
        }

        /// <summary>
        /// Derive key from password using PBKDF2
        /// </summary>
        public static byte[] DeriveKeyFromPassword(string password, byte[] salt)
        {
            return Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize);
        }

        /// <summary>
        /// Szyfruje plik screenshotu z losowym kluczem lub hasłem
        /// </summary>
        public static void EncryptScreenshot(string filePath, string outputPath, string? password = null)
        {
            try
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                byte[] key;
                byte[] salt = GenerateSalt();
                byte[] iv = new byte[16]; // AES block size

                if (password != null)
                {
                    // Użyj PBKDF2 do wygenerowania klucza z hasła
                    key = DeriveKeyFromPassword(password, salt);
                }
                else
                {
                    // Generuj losowy klucz
                    key = GenerateRandomKey();
                }

                // Generuj losowy IV
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(iv);
                }

                using (var aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var encryptor = aes.CreateEncryptor())
                    using (var fileStream = new FileStream(outputPath, FileMode.Create))
                    {
                        // Zapisz salt, IV, a potem zaszyfrowane dane
                        fileStream.Write(salt, 0, salt.Length);
                        fileStream.Write(iv, 0, iv.Length);

                        using (var cryptoStream = new CryptoStream(fileStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(fileBytes, 0, fileBytes.Length);
                        }
                    }
                }

                DebugHelper.LogInfo("Security", $"Screenshot encrypted: {outputPath}");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("Security", "Error encrypting screenshot", ex);
                throw;
            }
        }

        /// <summary>
        /// Deszyfruje plik screenshotu
        /// </summary>
        public static void DecryptScreenshot(string encryptedFilePath, string outputPath, string? password = null)
        {
            try
            {
                using (var fileStream = new FileStream(encryptedFilePath, FileMode.Open))
                {
                    // Odczytaj salt i IV
                    byte[] salt = new byte[SaltSize];
                    byte[] iv = new byte[16];
                    int saltRead = fileStream.Read(salt, 0, salt.Length);
                    int ivRead = fileStream.Read(iv, 0, iv.Length);
                    
                    if (saltRead != SaltSize || ivRead != 16)
                    {
                        throw new InvalidDataException("Invalid encrypted file format");
                    }

                    byte[] key;
                    if (password != null)
                    {
                        // Użyj PBKDF2 do wygenerowania klucza z hasła
                        key = DeriveKeyFromPassword(password, salt);
                    }
                    else
                    {
                        throw new ArgumentException("Password is required for decryption");
                    }

                    using (var aes = Aes.Create())
                    {
                        aes.Key = key;
                        aes.IV = iv;
                        aes.Mode = CipherMode.CBC;
                        aes.Padding = PaddingMode.PKCS7;

                        using (var decryptor = aes.CreateDecryptor())
                        using (var cryptoStream = new CryptoStream(fileStream, decryptor, CryptoStreamMode.Read))
                        using (var outputStream = new FileStream(outputPath, FileMode.Create))
                        {
                            cryptoStream.CopyTo(outputStream);
                        }
                    }
                }

                DebugHelper.LogInfo("Security", $"Screenshot decrypted: {outputPath}");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("Security", "Error decrypting screenshot", ex);
                throw;
            }
        }

        /// <summary>
        /// Dodaje znak wodny do obrazu
        /// </summary>
        public static BitmapSource AddWatermark(BitmapSource source, string watermarkText, 
            WatermarkPosition position = WatermarkPosition.BottomRight, 
            double opacity = 0.5)
        {
            try
            {
                var drawingVisual = new DrawingVisual();
                using (var drawingContext = drawingVisual.RenderOpen())
                {
                    // Narysuj oryginalny obraz
                    drawingContext.DrawImage(source, new Rect(0, 0, source.PixelWidth, source.PixelHeight));

                    // Dodaj znak wodny
                    var watermarkBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(
                        (byte)(255 * opacity), 255, 255, 255));

                    var typeface = new Typeface("Arial");
                    var formattedText = new FormattedText(
                        watermarkText,
                        System.Globalization.CultureInfo.CurrentCulture,
                        System.Windows.FlowDirection.LeftToRight,
                        typeface,
                        24,
                        watermarkBrush,
                        1.0);

                    // Oblicz pozycję
                    System.Windows.Point watermarkPosition = CalculateWatermarkPosition(
                        source.PixelWidth, source.PixelHeight, 
                        formattedText.Width, formattedText.Height, position);

                    drawingContext.DrawText(formattedText, watermarkPosition);
                }

                var renderTarget = new RenderTargetBitmap(
                    source.PixelWidth, source.PixelHeight, 96, 96, PixelFormats.Pbgra32);
                renderTarget.Render(drawingVisual);
                renderTarget.Freeze();

                return renderTarget;
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("Security", "Error adding watermark", ex);
                return source; // Zwróć oryginał w przypadku błędu
            }
        }

        /// <summary>
        /// Dodaje znak wodny obrazowy
        /// </summary>
        public static BitmapSource AddImageWatermark(BitmapSource source, BitmapSource watermarkImage,
            WatermarkPosition position = WatermarkPosition.BottomRight,
            double opacity = 0.5, double scale = 0.2)
        {
            try
            {
                var drawingVisual = new DrawingVisual();
                using (var drawingContext = drawingVisual.RenderOpen())
                {
                    // Narysuj oryginalny obraz
                    drawingContext.DrawImage(source, new Rect(0, 0, source.PixelWidth, source.PixelHeight));

                    // Skaluj znak wodny
                    double watermarkWidth = source.PixelWidth * scale;
                    double watermarkHeight = watermarkImage.PixelHeight * (watermarkWidth / watermarkImage.PixelWidth);

                    // Oblicz pozycję
                    System.Windows.Point watermarkPosition = CalculateWatermarkPosition(
                        source.PixelWidth, source.PixelHeight,
                        watermarkWidth, watermarkHeight, position);

                    // Utwórz brush z przezroczystością
                    var watermarkBrush = new ImageBrush(watermarkImage)
                    {
                        Opacity = opacity
                    };

                    drawingContext.DrawRectangle(watermarkBrush, null,
                        new Rect(watermarkPosition.X, watermarkPosition.Y, watermarkWidth, watermarkHeight));
                }

                var renderTarget = new RenderTargetBitmap(
                    source.PixelWidth, source.PixelHeight, 96, 96, PixelFormats.Pbgra32);
                renderTarget.Render(drawingVisual);
                renderTarget.Freeze();

                return renderTarget;
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("Security", "Error adding image watermark", ex);
                return source;
            }
        }

        /// <summary>
        /// Usuwa metadane EXIF z obrazu
        /// </summary>
        public static BitmapSource RemoveMetadata(BitmapSource source)
        {
            try
            {
                // Utwórz nowy BitmapSource bez metadanych
                var stride = source.PixelWidth * ((source.Format.BitsPerPixel + 7) / 8);
                byte[] pixels = new byte[stride * source.PixelHeight];
                source.CopyPixels(pixels, stride, 0);

                var cleanBitmap = BitmapSource.Create(
                    source.PixelWidth,
                    source.PixelHeight,
                    source.DpiX,
                    source.DpiY,
                    source.Format,
                    null,
                    pixels,
                    stride);

                cleanBitmap.Freeze();
                return cleanBitmap;
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("Security", "Error removing metadata", ex);
                return source;
            }
        }

        private static System.Windows.Point CalculateWatermarkPosition(int imageWidth, int imageHeight,
            double watermarkWidth, double watermarkHeight, WatermarkPosition position)
        {
            const int margin = 10;

            return position switch
            {
                WatermarkPosition.TopLeft => new System.Windows.Point(margin, margin),
                WatermarkPosition.TopRight => new System.Windows.Point(imageWidth - watermarkWidth - margin, margin),
                WatermarkPosition.BottomLeft => new System.Windows.Point(margin, imageHeight - watermarkHeight - margin),
                WatermarkPosition.BottomRight => new System.Windows.Point(imageWidth - watermarkWidth - margin, imageHeight - watermarkHeight - margin),
                WatermarkPosition.Center => new System.Windows.Point((imageWidth - watermarkWidth) / 2, (imageHeight - watermarkHeight) / 2),
                _ => new System.Windows.Point(imageWidth - watermarkWidth - margin, imageHeight - watermarkHeight - margin)
            };
        }

        public enum WatermarkPosition
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
            Center
        }
    }
}
