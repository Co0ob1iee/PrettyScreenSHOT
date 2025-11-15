using System.Globalization;
using System.Resources;
using System.Windows;

namespace PrettyScreenSHOT
{
    public static class LocalizationHelper
    {
        private static ResourceManager? resourceManager;
        private static CultureInfo currentCulture = CultureInfo.CurrentCulture;

        public static void Initialize()
        {
            try
            {
                resourceManager = new ResourceManager("PrettyScreenSHOT.Properties.Resources", typeof(LocalizationHelper).Assembly);
                
                // Ustaw domyślny język z ustawień
                var settings = SettingsManager.Instance;
                var languageCode = settings.Language;
                
                switch (languageCode.ToLower())
                {
                    case "pl":
                        currentCulture = new CultureInfo("pl-PL");
                        break;
                    case "de":
                        currentCulture = new CultureInfo("de-DE");
                        break;
                    case "zh":
                    case "cn":
                        currentCulture = new CultureInfo("zh-CN");
                        break;
                    case "fr":
                        currentCulture = new CultureInfo("fr-FR");
                        break;
                    default:
                        currentCulture = new CultureInfo("en-US");
                        break;
                }
                
                CultureInfo.DefaultThreadCurrentCulture = currentCulture;
                CultureInfo.DefaultThreadCurrentUICulture = currentCulture;
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("Localization", "Failed to initialize localization", ex);
                currentCulture = CultureInfo.CurrentCulture;
            }
        }

        public static string GetString(string key)
        {
            try
            {
                if (resourceManager == null)
                    return key;

                var value = resourceManager.GetString(key, currentCulture);
                return value ?? key;
            }
            catch
            {
                return key;
            }
        }

        public static void SetLanguage(string languageCode)
        {
            switch (languageCode.ToLower())
            {
                case "pl":
                    currentCulture = new CultureInfo("pl-PL");
                    break;
                case "de":
                    currentCulture = new CultureInfo("de-DE");
                    break;
                case "zh":
                case "cn":
                    currentCulture = new CultureInfo("zh-CN");
                    break;
                case "fr":
                    currentCulture = new CultureInfo("fr-FR");
                    break;
                default:
                    currentCulture = new CultureInfo("en-US");
                    break;
            }
            
            CultureInfo.DefaultThreadCurrentCulture = currentCulture;
            CultureInfo.DefaultThreadCurrentUICulture = currentCulture;
        }

        public static string CurrentLanguage => currentCulture.TwoLetterISOLanguageName;
    }
}

