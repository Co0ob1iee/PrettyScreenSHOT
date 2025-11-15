using System.Windows;
using Wpf.Ui.Controls;

namespace PrettyScreenSHOT.Views.Windows
{
    public partial class UpdateProgressWindow : FluentWindow
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

