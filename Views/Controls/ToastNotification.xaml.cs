using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PrettyScreenSHOT.Views.Controls
{
    public partial class ToastNotification : UserControl
    {
        public event EventHandler? Closed;

        public enum ToastType
        {
            Success,
            Info,
            Warning,
            Error
        }

        public ToastNotification()
        {
            InitializeComponent();
        }

        public void Show(string title, string message, ToastType type = ToastType.Success, int durationMs = 4000)
        {
            TitleText.Text = title;
            MessageText.Text = message;
            SetToastType(type);

            // Slide in animation
            var slideIn = new DoubleAnimation
            {
                From = 400,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            SlideTransform.BeginAnimation(TranslateTransform.XProperty, slideIn);
            this.BeginAnimation(OpacityProperty, fadeIn);

            // Auto-hide timer
            var hideTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(durationMs)
            };
            hideTimer.Tick += (s, e) =>
            {
                hideTimer.Stop();
                Hide();
            };
            hideTimer.Start();
        }

        public void Hide()
        {
            var slideOut = new DoubleAnimation
            {
                From = 0,
                To = 400,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };

            var fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(250)
            };

            slideOut.Completed += (s, e) =>
            {
                Closed?.Invoke(this, EventArgs.Empty);
            };

            SlideTransform.BeginAnimation(TranslateTransform.XProperty, slideOut);
            this.BeginAnimation(OpacityProperty, fadeOut);
        }

        private void SetToastType(ToastType type)
        {
            switch (type)
            {
                case ToastType.Success:
                    IconBorder.Background = new SolidColorBrush(Color.FromRgb(16, 185, 129)); // Green
                    ToastIcon.Symbol = Wpf.Ui.Common.SymbolRegular.Checkmark24;
                    break;

                case ToastType.Info:
                    IconBorder.Background = new SolidColorBrush(Color.FromRgb(0, 120, 212)); // Blue
                    ToastIcon.Symbol = Wpf.Ui.Common.SymbolRegular.Info24;
                    break;

                case ToastType.Warning:
                    IconBorder.Background = new SolidColorBrush(Color.FromRgb(245, 158, 11)); // Orange
                    ToastIcon.Symbol = Wpf.Ui.Common.SymbolRegular.Warning24;
                    break;

                case ToastType.Error:
                    IconBorder.Background = new SolidColorBrush(Color.FromRgb(239, 68, 68)); // Red
                    ToastIcon.Symbol = Wpf.Ui.Common.SymbolRegular.ErrorCircle24;
                    break;
            }
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
