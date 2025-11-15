using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PrettyScreenSHOT
{
    /// <summary>
    /// Klasa do sprawdzania dostępności aktualizacji z GitHub Releases
    /// </summary>
    public class UpdateChecker
    {
        private const string GitHubApiBaseUrl = "https://api.github.com/repos";
        
        // TODO: Zmień te wartości na swoje dane GitHub
        private const string RepositoryOwner = "yourusername"; 
        private const string RepositoryName = "PrettyScreenSHOT";
        
        private readonly HttpClient httpClient;
        private readonly string currentVersion;

        public UpdateChecker()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "PrettyScreenSHOT-UpdateChecker");
            httpClient.Timeout = TimeSpan.FromSeconds(10);
            
            // Pobierz aktualną wersję z AssemblyInfo
            currentVersion = GetCurrentVersion();
        }

        /// <summary>
        /// Sprawdza czy dostępna jest nowa wersja
        /// </summary>
        public async Task<UpdateInfo?> CheckForUpdatesAsync(string? channel = null)
        {
            try
            {
                string apiUrl = $"{GitHubApiBaseUrl}/{RepositoryOwner}/{RepositoryName}/releases/latest";
                
                DebugHelper.LogInfo("UpdateChecker", $"Checking for updates: {apiUrl}");

                var response = await httpClient.GetStringAsync(apiUrl);
                var release = JsonSerializer.Deserialize<GitHubRelease>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (release == null)
                {
                    DebugHelper.LogError("UpdateChecker", "Failed to parse GitHub API response");
                    return null;
                }

                // Sprawdź czy to wersja prerelease (jeśli nie chcemy beta)
                if (channel == "stable" && release.Prerelease)
                {
                    DebugHelper.LogInfo("UpdateChecker", "Latest release is prerelease, skipping (stable channel)");
                    return null;
                }

                // Wyciągnij wersję z tagu (usuń 'v' jeśli jest)
                string latestVersion = release.TagName.TrimStart('v', 'V');
                
                // Porównaj wersje
                if (!IsNewerVersion(currentVersion, latestVersion))
                {
                    DebugHelper.LogInfo("UpdateChecker", $"Current version ({currentVersion}) is up to date");
                    return null;
                }

                // Znajdź odpowiedni plik instalatora
                var installerAsset = FindInstallerAsset(release.Assets);
                if (installerAsset == null)
                {
                    DebugHelper.LogError("UpdateChecker", "No installer asset found in release");
                    return null;
                }

                var updateInfo = new UpdateInfo
                {
                    Version = latestVersion,
                    DownloadUrl = installerAsset.BrowserDownloadUrl,
                    ReleaseNotes = release.Body ?? "",
                    ReleaseDate = release.PublishedAt,
                    FileSize = installerAsset.Size,
                    FileName = installerAsset.Name,
                    IsPrerelease = release.Prerelease,
                    ReleaseUrl = release.HtmlUrl
                };

                DebugHelper.LogInfo("UpdateChecker", $"Update available: {latestVersion}");
                return updateInfo;
            }
            catch (HttpRequestException ex)
            {
                DebugHelper.LogError("UpdateChecker", "Network error while checking for updates", ex);
                return null;
            }
            catch (TaskCanceledException ex)
            {
                DebugHelper.LogError("UpdateChecker", "Timeout while checking for updates", ex);
                return null;
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("UpdateChecker", "Error checking for updates", ex);
                return null;
            }
        }

        /// <summary>
        /// Porównuje wersje i zwraca true jeśli newVersion jest nowsza
        /// </summary>
        private bool IsNewerVersion(string currentVersion, string newVersion)
        {
            try
            {
                var current = Version.Parse(currentVersion);
                var newer = Version.Parse(newVersion);
                return newer > current;
            }
            catch
            {
                // Jeśli parsowanie się nie powiodło, użyj porównania stringów
                return string.Compare(newVersion, currentVersion, StringComparison.OrdinalIgnoreCase) > 0;
            }
        }

        /// <summary>
        /// Znajduje odpowiedni plik instalatora w assets
        /// </summary>
        private GitHubAsset? FindInstallerAsset(GitHubAsset[] assets)
        {
            // Priorytet: MSIX > EXE
            foreach (var asset in assets)
            {
                if (asset.Name.EndsWith(".msix", StringComparison.OrdinalIgnoreCase))
                {
                    return asset;
                }
            }

            foreach (var asset in assets)
            {
                if (asset.Name.Contains("Setup") && asset.Name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    return asset;
                }
            }

            return null;
        }

        /// <summary>
        /// Pobiera aktualną wersję aplikacji
        /// </summary>
        private string GetCurrentVersion()
        {
            try
            {
                var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                return $"{version.Major}.{version.Minor}.{version.Build}";
            }
            catch
            {
                return "1.0.0"; // Fallback
            }
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }

        #region GitHub API Models

        private class GitHubRelease
        {
            [JsonPropertyName("tag_name")]
            public string TagName { get; set; } = "";

            [JsonPropertyName("name")]
            public string Name { get; set; } = "";

            [JsonPropertyName("body")]
            public string? Body { get; set; }

            [JsonPropertyName("published_at")]
            public DateTime PublishedAt { get; set; }

            [JsonPropertyName("prerelease")]
            public bool Prerelease { get; set; }

            [JsonPropertyName("html_url")]
            public string HtmlUrl { get; set; } = "";

            [JsonPropertyName("assets")]
            public GitHubAsset[] Assets { get; set; } = Array.Empty<GitHubAsset>();
        }

        private class GitHubAsset
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = "";

            [JsonPropertyName("browser_download_url")]
            public string BrowserDownloadUrl { get; set; } = "";

            [JsonPropertyName("size")]
            public long Size { get; set; }
        }

        #endregion
    }
}
