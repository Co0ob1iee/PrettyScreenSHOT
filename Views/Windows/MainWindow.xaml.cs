using System;
using System.Windows;
using System.Windows.Input;
using System.Reflection;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using PrettyScreenSHOT.Helpers;
using PrettyScreenSHOT.Services;
using PrettyScreenSHOT.Services.Settings;
using PrettyScreenSHOT.Services.Update;
using PrettyScreenSHOT.Views.Overlays;

namespace PrettyScreenSHOT.Views.Windows
{
    public partial class MainWindow : FluentWindow
    {
        private ApplicationTheme currentTheme;

        public MainWindow()
        {
            InitializeComponent();
            InitializeWindow();
        }

        private void InitializeWindow()
        {
            // Set version
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            VersionTextBlock.Text = $"v{version?.Major}.{version?.Minor}.{version?.Build}";

            // Initialize theme
            currentTheme = ApplicationThemeManager.GetAppTheme();
            UpdateThemeIcon();

            // Set default navigation
            NavigationView.SelectedItem = NavigationView.MenuItems[0]; // Dashboard

            // Load recent activity
            LoadRecentActivity();

            // Initialize cloud status
            UpdateCloudStatusDisplay();

            // Check for updates on startup
            CheckForUpdatesAsync();

            DebugHelper.LogInfo("MainWindow", "Dashboard initialized");
        }

        private void UpdateCloudStatusDisplay()
        {
            try
            {
                var cloudManager = Services.Cloud.CloudUploadManager.Instance;
                if (!string.IsNullOrEmpty(cloudManager.CurrentProvider))
                {
                    UpdateCloudStatus(cloudManager.CurrentProvider, true);
                }
                else
                {
                    CloudStatusPanel.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("MainWindow", "Error updating cloud status", ex);
            }
        }

        private async void CheckForUpdatesAsync()
        {
            try
            {
                var updateManager = new Services.Update.UpdateManager();
                var updateInfo = await updateManager.CheckForUpdatesAsync(showNotification: false);

                if (updateInfo != null)
                {
                    UpdateButton.Visibility = Visibility.Visible;
                    ToastNotificationService.Instance.ShowInfo($"Dostępna aktualizacja: {updateInfo.LatestVersion}");
                }
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("MainWindow", "Error checking for updates", ex);
            }
        }

        private void UpdateThemeIcon()
        {
            ThemeToggleButton.Icon = currentTheme == ApplicationTheme.Dark
                ? Wpf.Ui.Common.SymbolRegular.WeatherSunny24
                : Wpf.Ui.Common.SymbolRegular.WeatherMoon24;
        }

        private void OnThemeToggleClick(object sender, RoutedEventArgs e)
        {
            // Toggle between Light and Dark
            currentTheme = currentTheme == ApplicationTheme.Dark
                ? ApplicationTheme.Light
                : ApplicationTheme.Dark;

            ApplicationThemeManager.Apply(currentTheme);
            UpdateThemeIcon();

            // Save theme preference
            var themeName = currentTheme == ApplicationTheme.Dark ? "Dark" : "Light";
            SettingsManager.Instance.Theme = themeName;
            SettingsManager.Instance.SaveSettings();

            DebugHelper.LogInfo("MainWindow", $"Theme changed to {themeName}");
        }

        private void OnNavigationSelectionChanged(NavigationView sender, RoutedEventArgs args)
        {
            if (NavigationView.SelectedItem is NavigationViewItem item)
            {
                var tag = item.TargetPageTag as string;

                switch (tag)
                {
                    case "dashboard":
                        ShowDashboard();
                        break;
                    case "history":
                        ShowHistory();
                        break;
                    case "settings":
                        ShowSettings();
                        break;
                    case "about":
                        ShowAbout();
                        break;
                }

                DebugHelper.LogInfo("MainWindow", $"Navigated to {tag}");
            }
        }

        private void ShowDashboard()
        {
            DashboardContent.Visibility = Visibility.Visible;
            ContentPlaceholder.Visibility = Visibility.Collapsed;
            ContentPlaceholder.Content = null;
        }

        private void ShowHistory()
        {
            DashboardContent.Visibility = Visibility.Collapsed;
            ContentPlaceholder.Visibility = Visibility.Visible;

            var historyWindow = new ScreenshotHistoryWindow();
            historyWindow.Show();

            // Reset to dashboard
            NavigationView.SelectedItem = NavigationView.MenuItems[0];
        }

        private void ShowSettings()
        {
            DashboardContent.Visibility = Visibility.Collapsed;
            ContentPlaceholder.Visibility = Visibility.Visible;

            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();

            // Reset to dashboard
            NavigationView.SelectedItem = NavigationView.MenuItems[0];
        }

        private void ShowAbout()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var aboutMessage = $"PrettyScreenSHOT\n\n" +
                             $"Version: {version?.Major}.{version?.Minor}.{version?.Build}\n\n" +
                             $"Nowoczesna aplikacja do przechwytywania ekranu\n" +
                             $"z zaawansowanymi funkcjami edycji i udostępniania.";

            var messageBox = new Wpf.Ui.Controls.MessageBox
            {
                Title = "O aplikacji",
                Content = aboutMessage,
                ButtonLeftName = "OK"
            };

            messageBox.ShowDialogAsync();

            // Reset to dashboard
            NavigationView.SelectedItem = NavigationView.MenuItems[0];
        }

        // Quick Action Handlers
        private void OnNewScreenshotClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var overlayWindow = new ScreenshotOverlay();
                overlayWindow.Show();
                DebugHelper.LogInfo("MainWindow", "Screenshot overlay opened");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("MainWindow", "Error opening screenshot overlay", ex);
            }
        }

        private void OnVideoCaptureClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var videoWindow = new VideoCaptureWindow();
                var screenBounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
                videoWindow.SetCaptureArea(screenBounds);
                videoWindow.Show();
                DebugHelper.LogInfo("MainWindow", "Video capture window opened");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("MainWindow", "Error opening video capture", ex);
            }
        }

        private void OnScrollCaptureClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var overlayWindow = new ScreenshotOverlay();
                overlayWindow.Show();
                DebugHelper.LogInfo("MainWindow", "Scroll capture initiated");
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("MainWindow", "Error starting scroll capture", ex);
            }
        }

        private void OnHistoryClick(object sender, MouseButtonEventArgs e)
        {
            NavigationView.SelectedItem = NavigationView.MenuItems[1]; // History
        }

        private void OnSettingsClick(object sender, MouseButtonEventArgs e)
        {
            NavigationView.SelectedItem = NavigationView.MenuItems[2]; // Settings
        }

        private async void OnCheckUpdatesClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                StatusTextBlock.Text = "Sprawdzanie aktualizacji...";

                var updateManager = new UpdateManager();
                var updateInfo = await updateManager.CheckForUpdatesAsync(showNotification: false);

                if (updateInfo != null)
                {
                    UpdateButton.Visibility = Visibility.Visible;
                    StatusTextBlock.Text = "Dostępna aktualizacja!";
                }
                else
                {
                    StatusTextBlock.Text = "Używasz najnowszej wersji";

                    var messageBox = new Wpf.Ui.Controls.MessageBox
                    {
                        Title = "Brak aktualizacji",
                        Content = "Używasz najnowszej wersji aplikacji.",
                        ButtonLeftName = "OK"
                    };
                    await messageBox.ShowDialogAsync();
                }
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("MainWindow", "Error checking for updates", ex);
                StatusTextBlock.Text = "Błąd sprawdzania aktualizacji";
            }
        }

        private async void OnUpdateButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var updateManager = new UpdateManager();
                var updateInfo = await updateManager.CheckForUpdatesAsync(showNotification: false);

                if (updateInfo != null)
                {
                    var updateWindow = new UpdateWindow(updateInfo, updateManager);
                    updateWindow.Show();
                }
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("MainWindow", "Error showing update window", ex);
            }
        }

        private void LoadRecentActivity()
        {
            // TODO: Load recent screenshots from history
            // For now, just show placeholder text
            RecentActivityPanel.Children.Clear();

            var placeholderText = new System.Windows.Controls.TextBlock
            {
                Text = "Brak ostatniej aktywności",
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 20, 0, 20)
            };

            placeholderText.SetResourceReference(
                System.Windows.Controls.TextBlock.ForegroundProperty,
                "TextFillColorSecondaryBrush");

            RecentActivityPanel.Children.Add(placeholderText);
        }

        public void UpdateCloudStatus(string provider, bool connected)
        {
            if (connected)
            {
                CloudStatusPanel.Visibility = Visibility.Visible;
                CloudStatusText.Text = $"Połączono z {provider}";
            }
            else
            {
                CloudStatusPanel.Visibility = Visibility.Collapsed;
            }
        }

        public void ShowNotification(string message, bool isError = false)
        {
            StatusTextBlock.Text = message;

            if (isError)
            {
                ToastNotificationService.Instance.ShowError(message);
            }
            else
            {
                ToastNotificationService.Instance.ShowSuccess(message);
            }

            DebugHelper.LogInfo("MainWindow", $"Notification: {message}");
        }
    }
}
