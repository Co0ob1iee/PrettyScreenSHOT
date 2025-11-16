using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PrettyScreenSHOT.Views.Controls;

namespace PrettyScreenSHOT.Services
{
    public class ToastNotificationManager
    {
        private static ToastNotificationManager? _instance;
        public static ToastNotificationManager Instance => _instance ??= new ToastNotificationManager();

        private Grid? toastContainer;
        private readonly List<ToastNotification> activeToasts = new();
        private const int MaxToasts = 5;
        private const int ToastSpacing = 8;

        private ToastNotificationManager()
        {
        }

        public void Initialize(Grid container)
        {
            toastContainer = container;
        }

        public void ShowSuccess(string title, string message, int durationMs = 4000)
        {
            ShowToast(title, message, ToastNotification.ToastType.Success, durationMs);
        }

        public void ShowInfo(string title, string message, int durationMs = 4000)
        {
            ShowToast(title, message, ToastNotification.ToastType.Info, durationMs);
        }

        public void ShowWarning(string title, string message, int durationMs = 4000)
        {
            ShowToast(title, message, ToastNotification.ToastType.Warning, durationMs);
        }

        public void ShowError(string title, string message, int durationMs = 5000)
        {
            ShowToast(title, message, ToastNotification.ToastType.Error, durationMs);
        }

        private void ShowToast(string title, string message, ToastNotification.ToastType type, int durationMs)
        {
            if (toastContainer == null)
            {
                // Fallback to window-based toast if container not initialized
                ShowToastInWindow(title, message, type, durationMs);
                return;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                // Limit number of toasts
                if (activeToasts.Count >= MaxToasts)
                {
                    // Remove oldest toast
                    var oldest = activeToasts.First();
                    oldest.Hide();
                }

                var toast = new ToastNotification();
                toast.Closed += (s, e) =>
                {
                    activeToasts.Remove(toast);
                    toastContainer.Children.Remove(toast);
                    RepositionToasts();
                };

                // Position toast
                toast.HorizontalAlignment = HorizontalAlignment.Right;
                toast.VerticalAlignment = VerticalAlignment.Top;
                toast.Margin = new Thickness(0, CalculateTopMargin(), 0, 0);

                toastContainer.Children.Add(toast);
                activeToasts.Add(toast);

                toast.Show(title, message, type, durationMs);
            });
        }

        private double CalculateTopMargin()
        {
            double margin = 20; // Initial top margin
            foreach (var toast in activeToasts)
            {
                margin += toast.ActualHeight + ToastSpacing;
            }
            return margin;
        }

        private void RepositionToasts()
        {
            double margin = 20;
            foreach (var toast in activeToasts)
            {
                toast.Margin = new Thickness(0, margin, 0, 0);
                margin += toast.ActualHeight + ToastSpacing;
            }
        }

        private void ShowToastInWindow(string title, string message, ToastNotification.ToastType type, int durationMs)
        {
            // Create a standalone window for the toast
            var toastWindow = new Window
            {
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Background = System.Windows.Media.Brushes.Transparent,
                Topmost = true,
                ShowInTaskbar = false,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.Manual
            };

            // Position at bottom-right of screen
            var workingArea = SystemParameters.WorkArea;
            toastWindow.Left = workingArea.Right - 420; // 400px toast + 20px margin
            toastWindow.Top = workingArea.Bottom - 200; // Adjust based on toast height

            var toast = new ToastNotification();
            toast.Closed += (s, e) => toastWindow.Close();

            toastWindow.Content = toast;
            toastWindow.Show();

            toast.Show(title, message, type, durationMs);
        }
    }
}
