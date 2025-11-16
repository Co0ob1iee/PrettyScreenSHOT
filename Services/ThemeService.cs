using System;
using Wpf.Ui.Appearance;
using PrettyScreenSHOT.Helpers;

namespace PrettyScreenSHOT.Services
{
    /// <summary>
    /// Service for managing application themes using WPF UI.
    /// Replaces the legacy ThemeManager class.
    /// </summary>
    public class ThemeService
    {
        private static ThemeService? _instance;
        public static ThemeService Instance => _instance ??= new ThemeService();

        private ThemeService() { }

        /// <summary>
        /// Sets the theme using string name (for backward compatibility).
        /// </summary>
        /// <param name="themeName">Theme name: "Light", "Dark", "System", or "Neumorphic"</param>
        public void SetTheme(string themeName)
        {
            ApplicationTheme appTheme = themeName?.ToLower() switch
            {
                "light" => ApplicationTheme.Light,
                "dark" => ApplicationTheme.Dark,
                "neumorphic" => ApplicationTheme.Light, // Map Neumorphic to Light
                "system" => ApplicationTheme.Unknown,    // Auto-detect system theme
                _ => ApplicationTheme.Dark
            };

            ApplyTheme(appTheme);
        }

        /// <summary>
        /// Applies the system theme (auto-detect from Windows settings).
        /// </summary>
        public void ApplySystemTheme()
        {
            ApplicationThemeManager.ApplySystemTheme();
            DebugHelper.LogInfo("ThemeService", "Applied system theme");
        }

        /// <summary>
        /// Gets the current theme name.
        /// </summary>
        /// <returns>Theme name as string</returns>
        public string GetCurrentTheme()
        {
            var current = ApplicationThemeManager.GetAppTheme();
            return current switch
            {
                ApplicationTheme.Light => "Light",
                ApplicationTheme.Dark => "Dark",
                _ => "System"
            };
        }

        /// <summary>
        /// Gets the current WPF UI ApplicationTheme.
        /// </summary>
        /// <returns>ApplicationTheme enum value</returns>
        public ApplicationTheme GetCurrentWpfUiTheme()
        {
            return ApplicationThemeManager.GetAppTheme();
        }

        /// <summary>
        /// Applies the specified theme.
        /// </summary>
        /// <param name="theme">ApplicationTheme to apply</param>
        private void ApplyTheme(ApplicationTheme theme)
        {
            if (theme == ApplicationTheme.Unknown)
            {
                ApplySystemTheme();
            }
            else
            {
                // Apply theme - WindowBackdropType may not be available in this WPF UI version
                ApplicationThemeManager.Apply(theme);

                DebugHelper.LogInfo("ThemeService", $"Applied theme: {theme}");
            }
        }

        /// <summary>
        /// Legacy method for backward compatibility.
        /// In WPF UI, themes are applied globally, so this does nothing.
        /// </summary>
        /// <param name="window">Window to apply theme to (ignored)</param>
        [Obsolete("ApplyTheme(window) is no longer needed. WPF UI applies themes globally.")]
        public void ApplyTheme(System.Windows.Window window)
        {
            // No-op: WPF UI applies themes globally
            // This method exists only for backward compatibility during migration
        }
    }
}
