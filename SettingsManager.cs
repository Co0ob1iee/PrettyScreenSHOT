using System.IO;
using System.Text.Json;
using PrettyScreenSHOT.Services;

namespace PrettyScreenSHOT
{
    public class SettingsManager
    {
        private static readonly SettingsManager instance = new();
        public static SettingsManager Instance => instance;

        private readonly string settingsPath;
        private AppSettings settings;

        public string Language
        {
            get => settings.Language;
            set
            {
                settings.Language = value;
                SaveSettings();
                LocalizationHelper.SetLanguage(value);
            }
        }

        public string SavePath
        {
            get => settings.SavePath;
            set
            {
                settings.SavePath = value;
                SaveSettings();
            }
        }

        public string Hotkey
        {
            get => settings.Hotkey;
            set
            {
                settings.Hotkey = value;
                SaveSettings();
            }
        }

        public string ImageFormat
        {
            get => settings.ImageFormat;
            set
            {
                settings.ImageFormat = value;
                SaveSettings();
            }
        }

        public int ImageQuality
        {
            get => settings.ImageQuality;
            set
            {
                settings.ImageQuality = value;
                SaveSettings();
            }
        }

        public bool AutoSave
        {
            get => settings.AutoSave;
            set
            {
                settings.AutoSave = value;
                SaveSettings();
            }
        }

        public bool CopyToClipboard
        {
            get => settings.CopyToClipboard;
            set
            {
                settings.CopyToClipboard = value;
                SaveSettings();
            }
        }

        public bool ShowNotifications
        {
            get => settings.ShowNotifications;
            set
            {
                settings.ShowNotifications = value;
                SaveSettings();
            }
        }

        public bool AutoUpload
        {
            get => settings.AutoUpload;
            set
            {
                settings.AutoUpload = value;
                SaveSettings();
            }
        }

        public string CloudProvider
        {
            get => settings.CloudProvider;
            set
            {
                settings.CloudProvider = value;
                SaveSettings();
                try
                {
                    CloudUploadManager.Instance.CurrentProvider = string.IsNullOrWhiteSpace(value) ? null : value;
                }
                catch
                {
                    // Ignoruj błędy podczas inicjalizacji
                }
            }
        }

        public string CloudApiKey
        {
            get => settings.CloudApiKey;
            set
            {
                settings.CloudApiKey = value;
                SaveSettings();
            }
        }

        public string Theme
        {
            get => settings.Theme;
            set
            {
                settings.Theme = value;
                SaveSettings();
                // Apply theme using new ThemeService
                ThemeService.Instance.SetTheme(value);
            }
        }

        // === SPRINT 3: ADVANCED CAPTURE SETTINGS ===

        public bool PrivacyMode
        {
            get => settings.PrivacyMode;
            set
            {
                settings.PrivacyMode = value;
                SaveSettings();
            }
        }

        public bool CaptureCursor
        {
            get => settings.CaptureCursor;
            set
            {
                settings.CaptureCursor = value;
                SaveSettings();
            }
        }

        public int TimedCaptureDelay
        {
            get => settings.TimedCaptureDelay;
            set
            {
                settings.TimedCaptureDelay = value;
                SaveSettings();
            }
        }

        private SettingsManager()
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PrettyScreenSHOT");
            
            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);

            settingsPath = Path.Combine(appDataPath, "settings.json");
            LoadSettings();
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(settingsPath))
                {
                    var json = File.ReadAllText(settingsPath);
                    settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
                else
                {
                    settings = new AppSettings();
                }
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("Settings", "Failed to load settings", ex);
                settings = new AppSettings();
            }

            // Walidacja i poprawki
            if (string.IsNullOrEmpty(settings.SavePath) || !Directory.Exists(settings.SavePath))
            {
                settings.SavePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                    "PrettyScreenSHOT");
            }

            if (string.IsNullOrEmpty(settings.Language))
            {
                settings.Language = "en";
            }

            if (string.IsNullOrEmpty(settings.Hotkey))
            {
                settings.Hotkey = "PRTSCN";
            }

            if (string.IsNullOrEmpty(settings.ImageFormat))
            {
                settings.ImageFormat = "PNG";
            }

            // Zastosuj ustawienia
            LocalizationHelper.SetLanguage(settings.Language);
        }

        public void SaveSettings()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(settingsPath, json);
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("Settings", "Failed to save settings", ex);
            }
        }

        public void ResetToDefaults()
        {
            settings = new AppSettings();
            SaveSettings();
            LoadSettings();
        }

        // Security settings
        public bool EnableEncryption
        {
            get => settings.EnableEncryption;
            set { settings.EnableEncryption = value; SaveSettings(); }
        }

        public bool EnableWatermark
        {
            get => settings.EnableWatermark;
            set { settings.EnableWatermark = value; SaveSettings(); }
        }

        public string WatermarkText
        {
            get => settings.WatermarkText;
            set { settings.WatermarkText = value; SaveSettings(); }
        }

        public bool RemoveMetadata
        {
            get => settings.RemoveMetadata;
            set { settings.RemoveMetadata = value; SaveSettings(); }
        }

        // Performance settings
        public bool EnableCache
        {
            get => settings.EnableCache;
            set { settings.EnableCache = value; SaveSettings(); }
        }

        public int CacheSize
        {
            get => settings.CacheSize;
            set { settings.CacheSize = value; SaveSettings(); }
        }

        public int MaxImageWidth
        {
            get => settings.MaxImageWidth;
            set { settings.MaxImageWidth = value; SaveSettings(); }
        }

        public int MaxImageHeight
        {
            get => settings.MaxImageHeight;
            set { settings.MaxImageHeight = value; SaveSettings(); }
        }

        // Video capture settings
        public bool EnableVideoCapture
        {
            get => settings.EnableVideoCapture;
            set { settings.EnableVideoCapture = value; SaveSettings(); }
        }

        public int VideoFrameRate
        {
            get => settings.VideoFrameRate;
            set { settings.VideoFrameRate = value; SaveSettings(); }
        }

        public string VideoFormat
        {
            get => settings.VideoFormat;
            set { settings.VideoFormat = value; SaveSettings(); }
        }

        // Scroll capture settings
        public bool EnableScrollCapture
        {
            get => settings.EnableScrollCapture;
            set { settings.EnableScrollCapture = value; SaveSettings(); }
        }

        public int MaxScrolls
        {
            get => settings.MaxScrolls;
            set { settings.MaxScrolls = value; SaveSettings(); }
        }

        public string FFmpegPath
        {
            get => settings.FFmpegPath;
            set { settings.FFmpegPath = value; SaveSettings(); }
        }

        // Auto-update settings
        public bool EnableAutoUpdate
        {
            get => settings.EnableAutoUpdate;
            set { settings.EnableAutoUpdate = value; SaveSettings(); }
        }

        public int UpdateCheckIntervalHours
        {
            get => settings.UpdateCheckIntervalHours;
            set { settings.UpdateCheckIntervalHours = value; SaveSettings(); }
        }

        public bool CheckForUpdatesOnStartup
        {
            get => settings.CheckForUpdatesOnStartup;
            set { settings.CheckForUpdatesOnStartup = value; SaveSettings(); }
        }

        public bool ShowUpdateNotifications
        {
            get => settings.ShowUpdateNotifications;
            set { settings.ShowUpdateNotifications = value; SaveSettings(); }
        }

        public string UpdateChannel
        {
            get => settings.UpdateChannel;
            set { settings.UpdateChannel = value; SaveSettings(); }
        }

        public DateTime LastUpdateCheck
        {
            get => settings.LastUpdateCheck;
            set { settings.LastUpdateCheck = value; SaveSettings(); }
        }

        public string SkippedVersion
        {
            get => settings.SkippedVersion;
            set { settings.SkippedVersion = value; SaveSettings(); }
        }

        // Legacy compatibility
        public bool AutoUpdateEnabled
        {
            get => settings.EnableAutoUpdate;
            set { settings.EnableAutoUpdate = value; SaveSettings(); }
        }
        
        public bool AutoUpdateCheckOnStartup
        {
            get => settings.CheckForUpdatesOnStartup;
            set { settings.CheckForUpdatesOnStartup = value; SaveSettings(); }
        }

        public bool AutoInstallUpdates
        {
            get => settings.AutoInstallUpdates;
            set { settings.AutoInstallUpdates = value; SaveSettings(); }
        }

        public void SaveSkippedVersion(string version)
        {
            settings.SkippedVersion = version;
            SaveSettings();
        }

        public bool IsVersionSkipped(string version)
        {
            return settings.SkippedVersion == version;
        }

        private class AppSettings
        {
            public string Language { get; set; } = "en";
            public string SavePath { get; set; } = "";
            public string Hotkey { get; set; } = "PRTSCN";
            public string ImageFormat { get; set; } = "PNG";
            public int ImageQuality { get; set; } = 90;
            public bool AutoSave { get; set; } = true;
            public bool CopyToClipboard { get; set; } = true;
            public bool ShowNotifications { get; set; } = true;
            public bool AutoUpload { get; set; } = false;
            public string CloudProvider { get; set; } = "";
            public string CloudApiKey { get; set; } = "";
            public string Theme { get; set; } = "Dark";
            
            // Security
            public bool EnableEncryption { get; set; } = false;
            public bool EnableWatermark { get; set; } = false;
            public string WatermarkText { get; set; } = "PrettyScreenSHOT";
            public bool RemoveMetadata { get; set; } = false;
            
            // Performance
            public bool EnableCache { get; set; } = true;
            public int CacheSize { get; set; } = 50;
            public int MaxImageWidth { get; set; } = 0;
            public int MaxImageHeight { get; set; } = 0;
            
            // Video
            public bool EnableVideoCapture { get; set; } = false;
            public int VideoFrameRate { get; set; } = 10;
            public string VideoFormat { get; set; } = "GIF";
            
            // Scroll
            public bool EnableScrollCapture { get; set; } = false;
            public int MaxScrolls { get; set; } = 20;
            
            // FFmpeg
            public string FFmpegPath { get; set; } = "";
            
            // Auto-update
            public bool EnableAutoUpdate { get; set; } = true;
            public bool CheckForUpdatesOnStartup { get; set; } = true;
            public int UpdateCheckIntervalHours { get; set; } = 24;
            public bool ShowUpdateNotifications { get; set; } = true;
            public string UpdateChannel { get; set; } = "stable"; // stable, beta, alpha
            public DateTime LastUpdateCheck { get; set; } = DateTime.MinValue;
            public string SkippedVersion { get; set; } = "";
            public bool AutoInstallUpdates { get; set; } = false;

            // Sprint 3: Advanced Capture
            public bool PrivacyMode { get; set; } = false;
            public bool CaptureCursor { get; set; } = false;
            public int TimedCaptureDelay { get; set; } = 0; // 0 = disabled, 3/5/10 = seconds

            // Legacy compatibility (properties, not fields)
            // AutoUpdateEnabled i AutoUpdateCheckOnStartup są już zdefiniowane jako properties powyżej
        }
    }
}

