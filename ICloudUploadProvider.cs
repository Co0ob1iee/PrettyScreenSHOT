using System.Threading.Tasks;

namespace PrettyScreenSHOT
{
    /// <summary>
    /// Interfejs dla providerów uploadu do chmury
    /// </summary>
    public interface ICloudUploadProvider
    {
        /// <summary>
        /// Nazwa providera (np. "Imgur", "Cloudinary")
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// Czy wymaga konfiguracji API key
        /// </summary>
        bool RequiresApiKey { get; }

        /// <summary>
        /// Uploaduje obraz do chmury
        /// </summary>
        /// <param name="imageBytes">Bajty obrazu (PNG)</param>
        /// <param name="filename">Nazwa pliku</param>
        /// <param name="apiKey">Opcjonalny API key</param>
        /// <returns>Wynik uploadu z URL lub błędem</returns>
        Task<CloudUploadResult> UploadAsync(byte[] imageBytes, string filename, string? apiKey = null);
    }

    /// <summary>
    /// Wynik uploadu do chmury
    /// </summary>
    public class CloudUploadResult
    {
        public bool Success { get; set; }
        public string? Url { get; set; }
        public string? DeleteUrl { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ProviderName { get; set; }
    }
}

