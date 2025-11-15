using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PrettyScreenSHOT.Services.Cloud
{
    /// <summary>
    /// Provider uploadu do Cloudinary (popularny serwis CDN)
    /// </summary>
    public class CloudinaryUploadProvider : ICloudUploadProvider
    {
        private const string CloudinaryApiUrl = "https://api.cloudinary.com/v1_1/{0}/image/upload";
        private const string CloudNameHeader = "cloud_name";
        private const string ApiKeyHeader = "api_key";
        private const string ApiSecretHeader = "api_secret";

        public string ProviderName => "Cloudinary";
        public bool RequiresApiKey => true;

        public async Task<CloudUploadResult> UploadAsync(byte[] imageBytes, string filename, string? apiKey = null)
        {
            var result = new CloudUploadResult
            {
                ProviderName = ProviderName
            };

            try
            {
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    result.ErrorMessage = "Cloudinary credentials required. Format: cloud_name|api_key|api_secret";
                    return result;
                }

                // Cloudinary wymaga: cloud_name|api_key|api_secret
                var parts = apiKey.Split('|');
                if (parts.Length != 3)
                {
                    result.ErrorMessage = "Invalid Cloudinary credentials format. Expected: cloud_name|api_key|api_secret";
                    return result;
                }

                var cloudName = parts[0].Trim();
                var apiKeyValue = parts[1].Trim();
                var apiSecret = parts[2].Trim();

                if (string.IsNullOrWhiteSpace(cloudName) || string.IsNullOrWhiteSpace(apiKeyValue) || string.IsNullOrWhiteSpace(apiSecret))
                {
                    result.ErrorMessage = "All Cloudinary credentials are required.";
                    return result;
                }

                var apiUrl = string.Format(CloudinaryApiUrl, cloudName);

                // Cloudinary używa multipart/form-data z plikiem
                using var client = new HttpClient();
                
                var content = new MultipartFormDataContent();
                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                content.Add(imageContent, "file", filename);
                content.Add(new StringContent(apiKeyValue), "api_key");
                content.Add(new StringContent(apiSecret), "api_secret");

                DebugHelper.LogInfo("CloudUpload", $"Uploading to Cloudinary: {filename} ({imageBytes.Length} bytes)");

                var response = await client.PostAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonDoc = JsonDocument.Parse(responseContent);
                    
                    result.Success = true;
                    result.Url = jsonDoc.RootElement.TryGetProperty("secure_url", out var secureUrl) 
                        ? secureUrl.GetString() 
                        : jsonDoc.RootElement.TryGetProperty("url", out var url) 
                            ? url.GetString() 
                            : null;

                    DebugHelper.LogInfo("CloudUpload", $"Upload successful: {result.Url}");
                }
                else
                {
                    result.ErrorMessage = $"Cloudinary API error: {response.StatusCode} - {responseContent}";
                    DebugHelper.LogError("CloudUpload", result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = $"Upload failed: {ex.Message}";
                DebugHelper.LogError("CloudUpload", "Cloudinary upload exception", ex);
            }

            return result;
        }
    }
}

