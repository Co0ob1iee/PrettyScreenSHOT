using System.Windows;

namespace PrettyScreenSHOT
{
    public partial class UpdateProgressWindow : Window
    {
        public UpdateProgressWindow()
        {
            InitializeComponent();
        }

        public void UpdateProgress(double percent)
        {
            ProgressBar.Value = percent;
            ProgressText.Text = $"{percent:F1}%";
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Anulowanie będzie obsłużone przez UpdateNotificationWindow
            this.Close();
        }
    }
}

