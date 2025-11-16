using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;
using PrettyScreenSHOT.Helpers;
using PrettyScreenSHOT.Services.Video;
using Wpf.Ui.Controls;

namespace PrettyScreenSHOT.Views.Windows
{
    public partial class VideoCaptureWindow : FluentWindow
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
        }

        public void SetCaptureArea(Rectangle area)
        {
            captureArea = area;
            AreaText.Text = $"{area.Width}x{area.Height} at ({area.X}, {area.Y})";
        }

        private void FrameRateSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int fps = (int)e.NewValue;
            FrameRateLabel.Text = $"{fps} FPS";
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
                MessageBoxHelper.Show($"Error starting recording: {ex.Message}", "Error",
                    System.Windows.MessageBoxButton.OK);
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
                    StatusText.Text = "Saving...";
                    StopButton.IsEnabled = false;

                    string outputPath = await videoManager.StopRecordingAsync(saveDialog.FileName, format);

                    StatusText.Text = "Recording saved!";
                    StatusText.Foreground = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(76, 175, 80)); // Green

                    MessageBoxHelper.Show($"Video saved to:\n{outputPath}", "Success",
                        System.Windows.MessageBoxButton.OK);

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
                System.Windows.MessageBox.Show($"Error saving video: {ex.Message}", "Error", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (videoManager != null && videoManager.IsRecording)
            {
                var result = System.Windows.MessageBox.Show(
                    "Recording is in progress. Do you want to stop and discard?", 
                    "Confirm", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);

                if (result == System.Windows.MessageBoxResult.Yes)
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
    }
}

