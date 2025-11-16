using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Wpf.Ui.Controls;
using PrettyScreenSHOT.Helpers;

namespace PrettyScreenSHOT.Services
{
    public enum ToastType
    {
        Success,
        Error,
        Warning,
        Info
    }

    public class ToastNotificationService
    {
        private static ToastNotificationService? instance;
        public static ToastNotificationService Instance => instance ??= new ToastNotificationService();

        private ToastNotificationService() { }

        /// <summary>
        /// Shows a toast notification
        /// </summary>
        public void Show(string title, string message, ToastType type = ToastType.Info, int durationMs = 3000)
        {
            try
            {
                // If MainWindow is open, show in-app toast
                if (TryShowInAppToast(title, message, type, durationMs))
                {
                    return;
                }

                // Otherwise, show system tray notification
                ShowTrayNotification(title, message, type);
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("ToastNotificationService", "Error showing toast", ex);
            }
        }

        private bool TryShowInAppToast(string title, string message, ToastType type, int durationMs)
        {
            try
            {
                // Find MainWindow if it's open
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is Views.Windows.MainWindow mainWindow)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ShowToastInWindow(mainWindow, title, message, type, durationMs);
                        });
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("ToastNotificationService", "Error showing in-app toast", ex);
            }

            return false;
        }

        private void ShowToastInWindow(Views.Windows.MainWindow mainWindow, string title, string message, ToastType type, int durationMs)
        {
            // Create toast container if it doesn't exist
            var grid = mainWindow.Content as Grid;
            if (grid == null) return;

            // Create toast card
            var toastCard = new Card
            {
                Padding = new Thickness(16),
                Margin = new Thickness(20, 20, 20, 0),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                MaxWidth = 400
            };

            // Set background color based on type
            var (icon, foreground) = GetToastStyle(type);

            var stackPanel = new StackPanel
            {
                Orientation = System.Windows.Controls.Orientation.Horizontal,
                Children =
                {
                    new SymbolIcon
                    {
                        Symbol = icon,
                        FontSize = 20,
                        Margin = new Thickness(0, 0, 12, 0),
                        Foreground = new SolidColorBrush(foreground)
                    },
                    new StackPanel
                    {
                        Children =
                        {
                            new System.Windows.Controls.TextBlock
                            {
                                Text = title,
                                FontWeight = FontWeights.SemiBold,
                                FontSize = 14
                            },
                            new System.Windows.Controls.TextBlock
                            {
                                Text = message,
                                FontSize = 12,
                                Opacity = 0.8,
                                TextWrapping = TextWrapping.Wrap
                            }
                        }
                    }
                }
            };

            toastCard.Content = stackPanel;

            // Add to grid
            Grid.SetRowSpan(toastCard, grid.RowDefinitions.Count);
            Grid.SetColumnSpan(toastCard, grid.ColumnDefinitions.Count);
            grid.Children.Add(toastCard);

            // Animate in
            toastCard.Opacity = 0;
            toastCard.RenderTransform = new TranslateTransform(0, -20);

            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
            var slideIn = new DoubleAnimation(-20, 0, TimeSpan.FromMilliseconds(200));

            toastCard.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            ((TranslateTransform)toastCard.RenderTransform).BeginAnimation(TranslateTransform.YProperty, slideIn);

            // Auto-hide after duration
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(durationMs)
            };

            timer.Tick += (s, e) =>
            {
                timer.Stop();

                // Animate out
                var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
                var slideOut = new DoubleAnimation(0, -20, TimeSpan.FromMilliseconds(200));

                fadeOut.Completed += (s2, e2) =>
                {
                    grid.Children.Remove(toastCard);
                };

                toastCard.BeginAnimation(UIElement.OpacityProperty, fadeOut);
                ((TranslateTransform)toastCard.RenderTransform).BeginAnimation(TranslateTransform.YProperty, slideOut);
            };

            timer.Start();
        }

        private void ShowTrayNotification(string title, string message, ToastType type)
        {
            var trayIcon = TrayIconManager.Instance;
            if (trayIcon != null)
            {
                var iconType = type switch
                {
                    ToastType.Success => System.Windows.Forms.ToolTipIcon.Info,
                    ToastType.Error => System.Windows.Forms.ToolTipIcon.Error,
                    ToastType.Warning => System.Windows.Forms.ToolTipIcon.Warning,
                    _ => System.Windows.Forms.ToolTipIcon.Info
                };

                trayIcon.ShowNotification(title, message);
                DebugHelper.LogInfo("ToastNotificationService", $"Tray notification: {title} - {message}");
            }
        }

        private (Wpf.Ui.Common.SymbolRegular icon, Color foreground) GetToastStyle(ToastType type)
        {
            return type switch
            {
                ToastType.Success => (Wpf.Ui.Common.SymbolRegular.CheckmarkCircle24, Color.FromRgb(16, 185, 129)),
                ToastType.Error => (Wpf.Ui.Common.SymbolRegular.DismissCircle24, Color.FromRgb(239, 68, 68)),
                ToastType.Warning => (Wpf.Ui.Common.SymbolRegular.Warning24, Color.FromRgb(245, 158, 11)),
                _ => (Wpf.Ui.Common.SymbolRegular.Info24, Color.FromRgb(59, 130, 246))
            };
        }

        // Convenience methods
        public void ShowSuccess(string message) => Show("Sukces", message, ToastType.Success);
        public void ShowError(string message) => Show("Błąd", message, ToastType.Error);
        public void ShowWarning(string message) => Show("Ostrzeżenie", message, ToastType.Warning);
        public void ShowInfo(string message) => Show("Informacja", message, ToastType.Info);
    }
}
