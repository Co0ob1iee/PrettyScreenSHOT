namespace PrettyScreenSHOT.Helpers
{
    /// <summary>
    /// Debug helper class for logging and debugging.
    /// All methods are compiled only in DEBUG builds using Conditional attribute.
    /// In RELEASE builds, these method calls are completely removed by the compiler.
    /// </summary>
    public static class DebugHelper
    {
        /// <summary>
        /// Shows a message box (DEBUG only).
        /// </summary>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void ShowMessage(string title, string message)
        {
            MessageBoxHelper.Show(message, title);
        }

        /// <summary>
        /// Logs a debug message to Debug output and file (DEBUG only).
        /// </summary>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void LogDebug(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var logMessage = $"[{timestamp}] {message}";
            System.Diagnostics.Debug.WriteLine(logMessage);

            // Also write to file in DEBUG builds
            try
            {
                var logPath = System.IO.Path.Combine(
                    System.IO.Path.GetTempPath(),
                    "PrettyScreenSHOT_Debug.log");
                System.IO.File.AppendAllText(logPath, logMessage + Environment.NewLine);
            }
            catch
            {
                // Ignore file logging errors
            }
        }

        /// <summary>
        /// Logs an info message with category (DEBUG only).
        /// </summary>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void LogInfo(string category, string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var logMessage = $"[{timestamp}] [INFO] [{category}] {message}";
            System.Diagnostics.Debug.WriteLine(logMessage);
        }

        /// <summary>
        /// Logs an error message with optional exception (DEBUG only).
        /// </summary>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void LogError(string category, string message, Exception? ex = null)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var fullMessage = $"[{timestamp}] [ERROR] [{category}] {message}";
            if (ex != null)
                fullMessage += $"\n    Exception: {ex.GetType().Name}: {ex.Message}\n    StackTrace: {ex.StackTrace}";
            System.Diagnostics.Debug.WriteLine(fullMessage);
        }
    }
}