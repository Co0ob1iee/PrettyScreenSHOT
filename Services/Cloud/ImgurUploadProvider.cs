using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PrettyScreenSHOT.Helpers;

namespace PrettyScreenSHOT.Services.Cloud
{
    /// <summary>
    /// Provider uploadu do Imgur (bezpłatny, popularny serwis)
    /// </summary>
    public class ImgurUploadProvider : ICloudUploadProvider
    {
        private const string ImgurApiUrl = "https://api.imgur.com/3/image";
        private const string ClientIdHeader = "Client-ID";

        public string ProviderName => "Imgur";
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
                    result.ErrorMessage = "Imgur API key is required. Please configure it in settings.";
                    return result;
                }

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add(ClientIdHeader, apiKey);

                var base64Image = Convert.ToBase64String(imageBytes);
                
                // Imgur API wymaga JSON z base64 obrazem
                var jsonContent = new
                {
                    image = base64Image,
                    name = filename,
                    type = "base64"
                };
                
                var json = JsonSerializer.Serialize(jsonContent);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                DebugHelper.LogInfo("CloudUpload", $"Uploading to Imgur: {filename} ({imageBytes.Length} bytes)");

                var response = await client.PostAsync(ImgurApiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonDoc = JsonDocument.Parse(responseContent);
                    var data = jsonDoc.RootElement.GetProperty("data");

                    result.Success = true;
                    result.Url = data.TryGetProperty("link", out var link) ? link.GetString() : null;
                    result.DeleteUrl = data.TryGetProperty("deletehash", out var deleteHash) 
                        ? $"https://imgur.com/delete/{deleteHash.GetString()}" 
                        : null;

                    DebugHelper.LogInfo("CloudUpload", $"Upload successful: {result.Url}");
                }
                else
                {
                    result.ErrorMessage = $"Imgur API error: {response.StatusCode} - {responseContent}";
                    DebugHelper.LogError("CloudUpload", result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = $"Upload failed: {ex.Message}";
                DebugHelper.LogError("CloudUpload", "Imgur upload exception", ex);
            }

            return result;
        }
    }
}

