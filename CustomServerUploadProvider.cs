using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PrettyScreenSHOT
{
    /// <summary>
    /// Provider uploadu do własnego serwera (custom endpoint)
    /// </summary>
    public class CustomServerUploadProvider : ICloudUploadProvider
    {
        public string ProviderName => "Custom Server";
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
                    result.ErrorMessage = "Custom server URL required. Format: endpoint_url|api_key (optional)";
                    return result;
                }

                // Format: endpoint_url|api_key (opcjonalnie)
                var parts = apiKey.Split('|');
                var endpointUrl = parts[0].Trim();
                var authKey = parts.Length > 1 ? parts[1].Trim() : "";

                if (string.IsNullOrWhiteSpace(endpointUrl))
                {
                    result.ErrorMessage = "Endpoint URL is required.";
                    return result;
                }

                // Sprawdź czy URL jest poprawny
                if (!Uri.TryCreate(endpointUrl, UriKind.Absolute, out var uri))
                {
                    result.ErrorMessage = "Invalid endpoint URL format.";
                    return result;
                }

                using var client = new HttpClient();
                
                // Dodaj API key do nagłówków jeśli podano
                if (!string.IsNullOrWhiteSpace(authKey))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {authKey}");
                    // Alternatywnie można użyć:
                    // client.DefaultRequestHeaders.Add("X-API-Key", authKey);
                }

                // Użyj multipart/form-data dla większości serwerów
                var content = new MultipartFormDataContent();
                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                content.Add(imageContent, "file", filename);
                content.Add(new StringContent(filename), "filename");

                DebugHelper.LogInfo("CloudUpload", $"Uploading to custom server: {endpointUrl} ({imageBytes.Length} bytes)");

                var response = await client.PostAsync(endpointUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Próbuj parsować JSON odpowiedź
                    try
                    {
                        var jsonDoc = System.Text.Json.JsonDocument.Parse(responseContent);
                        
                        // Szukaj typowych pól w odpowiedzi
                        if (jsonDoc.RootElement.TryGetProperty("url", out var urlProp))
                            result.Url = urlProp.GetString();
                        else if (jsonDoc.RootElement.TryGetProperty("link", out var linkProp))
                            result.Url = linkProp.GetString();
                        else if (jsonDoc.RootElement.TryGetProperty("data", out var dataProp) && 
                                 dataProp.TryGetProperty("url", out var dataUrlProp))
                            result.Url = dataUrlProp.GetString();
                        else
                            result.Url = endpointUrl; // Fallback do endpoint URL
                    }
                    catch
                    {
                        // Jeśli nie JSON, użyj odpowiedzi jako URL lub endpoint
                        result.Url = responseContent.Trim().StartsWith("http") 
                            ? responseContent.Trim() 
                            : endpointUrl;
                    }

                    result.Success = true;
                    DebugHelper.LogInfo("CloudUpload", $"Upload successful: {result.Url}");
                }
                else
                {
                    result.ErrorMessage = $"Server error: {response.StatusCode} - {responseContent}";
                    DebugHelper.LogError("CloudUpload", result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = $"Upload failed: {ex.Message}";
                DebugHelper.LogError("CloudUpload", "Custom server upload exception", ex);
            }

            return result;
        }
    }
}

