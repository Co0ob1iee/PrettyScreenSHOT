using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PrettyScreenSHOT
{
    /// <summary>
    /// Provider uploadu do AWS S3
    /// </summary>
    public class S3UploadProvider : ICloudUploadProvider
    {
        public string ProviderName => "AWS S3";
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
                    result.ErrorMessage = "AWS S3 credentials required. Format: access_key|secret_key|bucket_name|region|endpoint_url";
                    return result;
                }

                // AWS S3 wymaga: access_key|secret_key|bucket_name|region|endpoint_url
                var parts = apiKey.Split('|');
                if (parts.Length < 3)
                {
                    result.ErrorMessage = "Invalid AWS S3 credentials format. Expected: access_key|secret_key|bucket_name|region|endpoint_url";
                    return result;
                }

                var accessKey = parts[0].Trim();
                var secretKey = parts[1].Trim();
                var bucketName = parts[2].Trim();
                var region = parts.Length > 3 ? parts[3].Trim() : "us-east-1";
                var endpointUrl = parts.Length > 4 ? parts[4].Trim() : "";

                if (string.IsNullOrWhiteSpace(accessKey) || string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(bucketName))
                {
                    result.ErrorMessage = "AWS S3 access key, secret key, and bucket name are required.";
                    return result;
                }

                // Dla uproszczenia, użyjemy prostego PUT request z podpisem
                // W produkcji warto użyć AWS SDK
                var objectKey = $"screenshots/{DateTime.UtcNow:yyyy/MM/dd}/{filename}";
                var url = string.IsNullOrEmpty(endpointUrl)
                    ? $"https://{bucketName}.s3.{region}.amazonaws.com/{objectKey}"
                    : $"{endpointUrl}/{bucketName}/{objectKey}";

                // Uproszczona implementacja - w produkcji użyj AWS SDK
                result.ErrorMessage = "AWS S3 upload requires AWS SDK. Please install AWSSDK.S3 NuGet package for full support.";
                DebugHelper.LogError("CloudUpload", "AWS S3 upload not fully implemented - requires AWS SDK");

                // TODO: Implementacja z AWS SDK
                // using var s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.GetBySystemName(region));
                // var request = new PutObjectRequest
                // {
                //     BucketName = bucketName,
                //     Key = objectKey,
                //     InputStream = new MemoryStream(imageBytes),
                //     ContentType = "image/png"
                // };
                // await s3Client.PutObjectAsync(request);
                // result.Success = true;
                // result.Url = url;

                return result;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = $"Upload failed: {ex.Message}";
                DebugHelper.LogError("CloudUpload", "S3 upload exception", ex);
            }

            return result;
        }
    }
}

