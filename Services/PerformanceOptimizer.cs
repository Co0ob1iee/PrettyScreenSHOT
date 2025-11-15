using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace PrettyScreenSHOT.Services
{
    /// <summary>
    /// Klasa do optymalizacji wydajności - caching, lazy loading, memory management
    /// </summary>
    public static class PerformanceOptimizer
    {
        private static readonly Dictionary<string, BitmapSource> thumbnailCache = new();
        private static readonly Dictionary<string, DateTime> cacheTimestamps = new();
        private static readonly object cacheLock = new object();
        
        private const int MaxCacheSize = 50; // Maksymalna liczba cache'owanych miniatur
        private const int CacheExpirationMinutes = 30; // Czas wygaśnięcia cache (minuty)
        private const int ThumbnailSize = 80; // Rozmiar miniatury

        /// <summary>
        /// Tworzy lub pobiera z cache miniaturę
        /// </summary>
        public static BitmapSource GetOrCreateThumbnail(BitmapSource source, string cacheKey)
        {
            if (string.IsNullOrEmpty(cacheKey))
                cacheKey = Guid.NewGuid().ToString();

            lock (cacheLock)
            {
                // Sprawdź cache
                if (thumbnailCache.TryGetValue(cacheKey, out var cachedThumbnail))
                {
                    if (cacheTimestamps.TryGetValue(cacheKey, out var timestamp))
                    {
                        if (DateTime.Now - timestamp < TimeSpan.FromMinutes(CacheExpirationMinutes))
                        {
                            return cachedThumbnail;
                        }
                        else
                        {
                            // Cache wygasł - usuń
                            thumbnailCache.Remove(cacheKey);
                            cacheTimestamps.Remove(cacheKey);
                        }
                    }
                }

                // Utwórz nową miniaturę
                var thumbnail = CreateThumbnailOptimized(source);
                
                // Dodaj do cache
                AddToCache(cacheKey, thumbnail);
                
                return thumbnail;
            }
        }

        private static BitmapSource CreateThumbnailOptimized(BitmapSource source)
        {
            // Użyj mniejszego rozmiaru dla lepszej wydajności
            var scale = Math.Min(ThumbnailSize / (double)source.PixelWidth, 
                                ThumbnailSize / (double)source.PixelHeight);
            
            var scaleTransform = new System.Windows.Media.ScaleTransform(scale, scale);
            var transformedBitmap = new TransformedBitmap(source, scaleTransform);
            transformedBitmap.Freeze();
            
            return transformedBitmap;
        }

        private static void AddToCache(string key, BitmapSource thumbnail)
        {
            // Jeśli cache jest pełny, usuń najstarsze wpisy
            if (thumbnailCache.Count >= MaxCacheSize)
            {
                CleanupOldCacheEntries();
            }

            thumbnailCache[key] = thumbnail;
            cacheTimestamps[key] = DateTime.Now;
        }

        private static void CleanupOldCacheEntries()
        {
            // Usuń najstarsze wpisy (FIFO)
            var sortedEntries = new List<KeyValuePair<string, DateTime>>(cacheTimestamps);
            sortedEntries.Sort((a, b) => a.Value.CompareTo(b.Value));

            int entriesToRemove = thumbnailCache.Count - MaxCacheSize + 1;
            for (int i = 0; i < entriesToRemove && i < sortedEntries.Count; i++)
            {
                var key = sortedEntries[i].Key;
                thumbnailCache.Remove(key);
                cacheTimestamps.Remove(key);
            }
        }

        /// <summary>
        /// Czyści cały cache
        /// </summary>
        public static void ClearCache()
        {
            lock (cacheLock)
            {
                thumbnailCache.Clear();
                cacheTimestamps.Clear();
                GC.Collect(); // Wymuś garbage collection
            }
        }

        /// <summary>
        /// Czyści wygasłe wpisy z cache
        /// </summary>
        public static void CleanupExpiredCache()
        {
            lock (cacheLock)
            {
                var expiredKeys = new List<string>();
                var expirationTime = DateTime.Now - TimeSpan.FromMinutes(CacheExpirationMinutes);

                foreach (var entry in cacheTimestamps)
                {
                    if (entry.Value < expirationTime)
                    {
                        expiredKeys.Add(entry.Key);
                    }
                }

                foreach (var key in expiredKeys)
                {
                    thumbnailCache.Remove(key);
                    cacheTimestamps.Remove(key);
                }
            }
        }

        /// <summary>
        /// Lazy loading dla historii - ładuje miniatury w tle
        /// </summary>
        public static async Task<BitmapSource> LoadThumbnailAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(filePath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.DecodePixelWidth = ThumbnailSize; // Załaduj od razu w mniejszym rozmiarze
                    bitmap.EndInit();
                    bitmap.Freeze();
                    
                    return bitmap;
                }
                catch (Exception ex)
                {
                    DebugHelper.LogError("PerformanceOptimizer", $"Failed to load thumbnail: {filePath}", ex);
                    return null;
                }
            });
        }

        /// <summary>
        /// Optymalizuje BitmapSource przed zapisem
        /// </summary>
        public static BitmapSource OptimizeForSave(BitmapSource source, int maxWidth = 0, int maxHeight = 0)
        {
            if (source == null) return source;

            // Jeśli nie ma limitów, zwróć oryginał
            if (maxWidth <= 0 && maxHeight <= 0)
                return source;

            int targetWidth = source.PixelWidth;
            int targetHeight = source.PixelHeight;

            // Oblicz nowe wymiary zachowując proporcje
            if (maxWidth > 0 && targetWidth > maxWidth)
            {
                double ratio = (double)maxWidth / targetWidth;
                targetWidth = maxWidth;
                targetHeight = (int)(targetHeight * ratio);
            }

            if (maxHeight > 0 && targetHeight > maxHeight)
            {
                double ratio = (double)maxHeight / targetHeight;
                targetHeight = maxHeight;
                targetWidth = (int)(targetWidth * ratio);
            }

            // Jeśli wymiary się nie zmieniły, zwróć oryginał
            if (targetWidth == source.PixelWidth && targetHeight == source.PixelHeight)
                return source;

            // Skaluj obraz
            var scaleTransform = new System.Windows.Media.ScaleTransform(
                (double)targetWidth / source.PixelWidth,
                (double)targetHeight / source.PixelHeight);

            var scaledBitmap = new TransformedBitmap(source, scaleTransform);
            scaledBitmap.Freeze();

            return scaledBitmap;
        }

        /// <summary>
        /// Pobiera statystyki cache
        /// </summary>
        public static CacheStatistics GetCacheStatistics()
        {
            lock (cacheLock)
            {
                return new CacheStatistics
                {
                    CachedItems = thumbnailCache.Count,
                    MaxSize = MaxCacheSize,
                    ExpirationMinutes = CacheExpirationMinutes
                };
            }
        }

        public class CacheStatistics
        {
            public int CachedItems { get; set; }
            public int MaxSize { get; set; }
            public int ExpirationMinutes { get; set; }
        }
    }
}

