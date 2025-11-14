namespace PrettyScreenSHOT
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
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
#endif
        }
    }
}