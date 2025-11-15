namespace PrettyScreenSHOT.Helpers
{
    public static class DebugHelper
    {
        public static void ShowMessage(string title, string message)
        {
#if DEBUG
            System.Windows.MessageBox.Show(message, title);
#endif
        }

        public static void LogDebug(string message)
        {
#if DEBUG
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var logMessage = $"[{timestamp}] {message}";
            System.Diagnostics.Debug.WriteLine(logMessage);
            
            // Równie¿ wypisz do pliku w DEBUG
            try
            {
                var logPath = System.IO.Path.Combine(
                    System.IO.Path.GetTempPath(), 
                    "PrettyScreenSHOT_Debug.log");
                System.IO.File.AppendAllText(logPath, logMessage + Environment.NewLine);
            }
            catch { }
#endif
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void LogInfo(string category, string message)
        {
            LogDebug($"[{category}] {message}");
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void LogError(string category, string message, Exception? ex = null)
        {
            var fullMessage = $"[ERROR] [{category}] {message}";
            if (ex != null)
                fullMessage += $" - {ex.Message}";
            LogDebug(fullMessage);
        }
    }
}