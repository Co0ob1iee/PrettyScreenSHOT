using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;

namespace PrettyScreenSHOT
{
    public partial class VideoCaptureWindow : Window
    {
        private VideoCaptureManager? videoManager;
        private Rectangle captureArea;
        private DispatcherTimer? recordingTimer;
        private DateTime recordingStartTime;
        private Task? recordingTask;

        public VideoCaptureWindow()
        {
            InitializeComponent();
            videoManager = new VideoCaptureManager();

            // Zastosuj theme
            ThemeManager.Instance.ApplyTheme(this);

            // Załaduj lokalizowane teksty
            LoadLocalizedStrings();
        }

        private void LoadLocalizedStrings()
        {
            Title = LocalizationHelper.GetString("VideoCapture_Title");
            if (StatusText != null)
                StatusText.Text = LocalizationHelper.GetString("VideoCapture_ReadyToRecord");
        }

        public void SetCaptureArea(Rectangle area)
        {
            captureArea = area;
            AreaText.Text = $"{area.Width}x{area.Height} at ({area.X}, {area.Y})";
        }

        private void FrameRateSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int fps = (int)e.NewValue;
            FrameRateLabel.Text = string.Format(LocalizationHelper.GetString("VideoCapture_FrameRateLabel"), fps);
            if (videoManager != null)
            {
                videoManager.FrameRate = fps;
            }
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (videoManager == null)
                {
                    videoManager = new VideoCaptureManager();
                }

                int fps = (int)FrameRateSlider.Value;
                videoManager.StartRecording(captureArea, fps);

                // Rozpocznij nagrywanie klatek w tle
                recordingTask = videoManager.RecordFramesAsync();

                // Rozpocznij timer
                recordingStartTime = DateTime.Now;
                recordingTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(100)
                };
                recordingTimer.Tick += RecordingTimer_Tick;
                recordingTimer.Start();

                // Aktualizuj UI
                StartButton.IsEnabled = false;
                StopButton.IsEnabled = true;
                StatusText.Text = "Recording...";
                StatusText.Foreground = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(244, 67, 54)); // Red
                RecordingInfoGrid.Visibility = Visibility.Visible;
                FramesInfoGrid.Visibility = Visibility.Visible;

                DebugHelper.LogInfo("VideoCapture", "Recording started");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("VideoCapture", "Error starting recording", ex);
                System.Windows.MessageBox.Show(
                    string.Format(LocalizationHelper.GetString("VideoCapture_ErrorStarting"), ex.Message),
                    LocalizationHelper.GetString("Editor_Error"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void RecordingTimer_Tick(object? sender, EventArgs e)
        {
            if (videoManager != null && videoManager.IsRecording)
            {
                var elapsed = DateTime.Now - recordingStartTime;
                TimeText.Text = $"{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";
                
                // Aktualizuj liczbę klatek
                FramesText.Text = videoManager.FramesCount.ToString();
            }
        }

        private async void StopButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (videoManager == null || !videoManager.IsRecording)
                    return;

                // Zatrzymaj timer
                recordingTimer?.Stop();

                // Wybierz format
                var format = FormatComboBox.SelectedIndex == 0 
                    ? VideoCaptureManager.VideoFormat.GIF 
                    : VideoCaptureManager.VideoFormat.MP4;

                // Wybierz ścieżkę zapisu
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = format == VideoCaptureManager.VideoFormat.GIF 
                        ? "GIF files (*.gif)|*.gif" 
                        : "MP4 files (*.mp4)|*.mp4",
                    FileName = $"VideoCapture_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    StatusText.Text = LocalizationHelper.GetString("VideoCapture_Saving");
                    StopButton.IsEnabled = false;

                    string outputPath = await videoManager.StopRecordingAsync(saveDialog.FileName, format);

                    StatusText.Text = LocalizationHelper.GetString("VideoCapture_RecordingSaved");
                    StatusText.Foreground = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(76, 175, 80)); // Green

                    System.Windows.MessageBox.Show(
                        string.Format(LocalizationHelper.GetString("VideoCapture_VideoSavedMessage"), "\n", outputPath),
                        LocalizationHelper.GetString("Editor_Success"),
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Reset UI
                    StartButton.IsEnabled = true;
                    StopButton.IsEnabled = false;
                    RecordingInfoGrid.Visibility = Visibility.Collapsed;
                    FramesInfoGrid.Visibility = Visibility.Collapsed;
                    TimeText.Text = "00:00";
                }
                else
                {
                    // Anulowano - kontynuuj nagrywanie lub zatrzymaj
                    videoManager.StopRecordingAsync("", format).Wait();
                }
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("VideoCapture", "Error stopping recording", ex);
                System.Windows.MessageBox.Show(
                    string.Format(LocalizationHelper.GetString("VideoCapture_ErrorSaving"), ex.Message),
                    LocalizationHelper.GetString("Editor_Error"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (videoManager != null && videoManager.IsRecording)
            {
                var result = System.Windows.MessageBox.Show(
                    "Recording is in progress. Do you want to stop and discard?", 
                    "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        recordingTimer?.Stop();
                        videoManager.StopRecordingAsync("", VideoCaptureManager.VideoFormat.GIF).Wait();
                    }
                    catch { }
                }
                else
                {
                    return;
                }
            }

            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            videoManager?.Dispose();
            recordingTimer?.Stop();
            base.OnClosed(e);
        }

        private void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            CancelButton_Click(sender, e);
        }
    }
}

