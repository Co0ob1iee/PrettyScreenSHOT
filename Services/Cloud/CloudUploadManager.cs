using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PrettyScreenSHOT
{
    /// <summary>
    /// Manager do zarządzania uploadami do chmury
    /// </summary>
    public class CloudUploadManager
    {
        private static readonly CloudUploadManager instance = new();
        public static CloudUploadManager Instance => instance;

        private readonly Dictionary<string, ICloudUploadProvider> providers = new();
        private string? currentProviderName;

        private CloudUploadManager()
        {
            // Rejestruj dostępne providery
            RegisterProvider(new ImgurUploadProvider());
            RegisterProvider(new CloudinaryUploadProvider());
            RegisterProvider(new S3UploadProvider());
            RegisterProvider(new CustomServerUploadProvider());
        }

        public void RegisterProvider(ICloudUploadProvider provider)
        {
            providers[provider.ProviderName] = provider;
            DebugHelper.LogInfo("CloudUpload", $"Registered provider: {provider.ProviderName}");
        }

        public IEnumerable<string> GetAvailableProviders()
        {
            return providers.Keys;
        }

        public ICloudUploadProvider? GetProvider(string providerName)
        {
            return providers.TryGetValue(providerName, out var provider) ? provider : null;
        }

        public string? CurrentProvider
        {
            get => currentProviderName;
            set
            {
                if (value != null && !providers.ContainsKey(value))
                {
                    DebugHelper.LogError("CloudUpload", $"Provider not found: {value}");
                    return;
                }
                currentProviderName = value;
                try
                {
                    // Aktualizuj ustawienia tylko jeśli SettingsManager jest już zainicjalizowany
                    if (SettingsManager.Instance != null)
                    {
                        SettingsManager.Instance.CloudProvider = value ?? "";
                    }
                }
                catch
                {
                    // Ignoruj błędy podczas inicjalizacji
                }
            }
        }

        /// <summary>
        /// Uploaduje BitmapSource do chmury
        /// </summary>
        public async Task<CloudUploadResult> UploadScreenshotAsync(BitmapSource bitmap, string filename)
        {
            if (string.IsNullOrEmpty(currentProviderName))
            {
                return new CloudUploadResult
                {
                    Success = false,
                    ErrorMessage = "No cloud provider selected. Please configure in settings."
                };
            }

            var provider = GetProvider(currentProviderName);
            if (provider == null)
            {
                return new CloudUploadResult
                {
                    Success = false,
                    ErrorMessage = $"Provider '{currentProviderName}' not found."
                };
            }

            try
            {
                // Konwertuj BitmapSource do byte array (PNG)
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));

                byte[] imageBytes;
                using (var memoryStream = new MemoryStream())
                {
                    encoder.Save(memoryStream);
                    imageBytes = memoryStream.ToArray();
                }

                var apiKey = SettingsManager.Instance.CloudApiKey;
                if (provider.RequiresApiKey && string.IsNullOrWhiteSpace(apiKey))
                {
                    return new CloudUploadResult
                    {
                        Success = false,
                        ErrorMessage = $"API key required for {provider.ProviderName}. Please configure in settings."
                    };
                }

                return await provider.UploadAsync(imageBytes, filename, apiKey);
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("CloudUpload", "Error converting bitmap to bytes", ex);
                return new CloudUploadResult
                {
                    Success = false,
                    ErrorMessage = $"Error preparing image: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Uploaduje plik z dysku do chmury
        /// </summary>
        public async Task<CloudUploadResult> UploadFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new CloudUploadResult
                {
                    Success = false,
                    ErrorMessage = "File not found."
                };
            }

            try
            {
                var imageBytes = await File.ReadAllBytesAsync(filePath);
                var filename = Path.GetFileName(filePath);

                if (string.IsNullOrEmpty(currentProviderName))
                {
                    return new CloudUploadResult
                    {
                        Success = false,
                        ErrorMessage = "No cloud provider selected."
                    };
                }

                var provider = GetProvider(currentProviderName);
                if (provider == null)
                {
                    return new CloudUploadResult
                    {
                        Success = false,
                        ErrorMessage = $"Provider '{currentProviderName}' not found."
                    };
                }

                var apiKey = SettingsManager.Instance.CloudApiKey;
                if (provider.RequiresApiKey && string.IsNullOrWhiteSpace(apiKey))
                {
                    return new CloudUploadResult
                    {
                        Success = false,
                        ErrorMessage = $"API key required for {provider.ProviderName}."
                    };
                }

                return await provider.UploadAsync(imageBytes, filename, apiKey);
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("CloudUpload", "Error reading file", ex);
                return new CloudUploadResult
                {
                    Success = false,
                    ErrorMessage = $"Error reading file: {ex.Message}"
                };
            }
        }
    }
}

