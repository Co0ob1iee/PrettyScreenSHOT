using System.Windows;
using System.Windows.Controls;

namespace PrettyScreenSHOT
{
    public partial class ScreenshotHistoryWindow : Window
    {
        public ScreenshotHistoryWindow()
        {
            InitializeComponent();
            HistoryListBox.ItemsSource = ScreenshotManager.Instance.History;
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ScreenshotItem item)
            {
                ScreenshotManager.Instance.DeleteScreenshot(item);
            }
        }
    }
}
