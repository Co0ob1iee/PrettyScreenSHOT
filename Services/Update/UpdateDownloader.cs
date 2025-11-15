using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PrettyScreenSHOT.Services.Update
{
    /// <summary>
    /// Klasa do pobierania aktualizacji
    /// </summary>
    public class UpdateDownloader : IDisposable
    {
        private readonly HttpClient httpClient;
        private CancellationTokenSource? cancellationTokenSource;

        public UpdateDownloader()
        {
            httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMinutes(30); // Długi timeout dla dużych plików
        }

        /// <summary>
        /// Pobiera plik aktualizacji z podanego URL
        /// </summary>
        public async Task<string> DownloadUpdateAsync(string url, string fileName, IProgress<double>? progress = null)
        {
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            try
            {
                DebugHelper.LogInfo("UpdateDownloader", $"Downloading update from: {url}");

                // Utwórz folder temp jeśli nie istnieje
                string tempFolder = Path.Combine(Path.GetTempPath(), "PrettyScreenSHOT_Updates");
                Directory.CreateDirectory(tempFolder);

                string filePath = Path.Combine(tempFolder, fileName);

                // Jeśli plik już istnieje, usuń go
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                using (var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token))
                {
                    response.EnsureSuccessStatusCode();

                    var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                    var canReportProgress = totalBytes != -1 && progress != null;

                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    using (var contentStream = await response.Content.ReadAsStreamAsync(token))
                    {
                        var totalBytesRead = 0L;
                        var buffer = new byte[8192];
                        var isMoreToRead = true;

                        do
                        {
                            var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, token);
                            if (bytesRead == 0)
                            {
                                isMoreToRead = false;
                            }
                            else
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesRead, token);
                                totalBytesRead += bytesRead;

                                if (canReportProgress)
                                {
                                    var progressPercentage = (double)totalBytesRead / totalBytes * 100;
                                    progress.Report(progressPercentage);
                                }
                            }
                        } while (isMoreToRead);
                    }
                }

                DebugHelper.LogInfo("UpdateDownloader", $"Update downloaded successfully: {filePath}");
                return filePath;
            }
            catch (OperationCanceledException)
            {
                DebugHelper.LogInfo("UpdateDownloader", "Download cancelled by user");
                throw;
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("UpdateDownloader", "Error downloading update", ex);
                throw;
            }
        }

        /// <summary>
        /// Anuluje pobieranie
        /// </summary>
        public void CancelDownload()
        {
            cancellationTokenSource?.Cancel();
        }

        public void Dispose()
        {
            cancellationTokenSource?.Dispose();
            httpClient?.Dispose();
        }
    }
}
