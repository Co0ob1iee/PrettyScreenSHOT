using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PrettyScreenSHOT.Helpers;
using PrettyScreenSHOT.Services;
using PrettyScreenSHOT.Services.Cloud;
using PrettyScreenSHOT.Services.Input;
using PrettyScreenSHOT.Services.Screenshot;
using Wpf.Ui.Controls;
using WpfMessageBoxButton = Wpf.Ui.Controls.MessageBoxButton;
using WpfTextBlock = Wpf.Ui.Controls.TextBlock;

namespace PrettyScreenSHOT.Views.Windows
{
    public partial class ScreenshotHistoryWindow : FluentWindow
    {
        private CollectionViewSource? historyViewSource;
        private string? currentSearchText;
        private string? currentCategory;
        private List<string>? currentTags;

        public ScreenshotHistoryWindow()
        {
            InitializeComponent();

            // WPF UI applies themes globally - no need to apply per-window

            // Zarejestruj skróty klawiszowe
            KeyboardShortcutsManager.Instance.RegisterWindowShortcuts(this);
            this.Closed += (s, e) => KeyboardShortcutsManager.Instance.UnregisterWindowShortcuts(this);
            
            // Setup filtering
            historyViewSource = new CollectionViewSource
            {
                Source = ScreenshotManager.Instance.History
            };
            HistoryItemsControl.ItemsSource = historyViewSource.View;

            LoadLocalizedStrings();
            InitializeSearchAndFilter();
            UpdateEmptyState();
        }

        private void InitializeSearchAndFilter()
        {
            // Ustaw placeholder dla wyszukiwania (ui:TextBox has built-in PlaceholderText property)
            if (SearchTextBox != null)
            {
                SearchTextBox.PlaceholderText = LocalizationHelper.GetString("History_SearchPlaceholder");
            }

            // Odśwież kategorie i tagi
            SearchAndFilterManager.Instance.RefreshCategoriesAndTags();

            // Ustaw dostępne kategorie i tagi w ComboBox (jeśli istnieją w XAML)
            if (CategoryComboBox != null)
            {
                CategoryComboBox.ItemsSource = SearchAndFilterManager.Instance.AvailableCategories;
            }

            // Ustaw tekst przycisku Clear
            if (ClearFiltersButton != null)
            {
                ClearFiltersButton.Content = LocalizationHelper.GetString("History_ClearFilters");
            }
        }

        private void OnSearchTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox textBox)
            {
                currentSearchText = textBox.Text;
                ApplyFilters();
            }
        }

        private void OnCategoryChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.ComboBox comboBox)
            {
                currentCategory = comboBox.SelectedItem?.ToString();
                ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            if (historyViewSource?.View == null) return;

            var allItems = ScreenshotManager.Instance.History;
            var filtered = SearchAndFilterManager.Instance.Search(allItems, currentSearchText, currentCategory, currentTags);
            
            historyViewSource.View.Filter = item => filtered.Contains(item);
            historyViewSource.View.Refresh();
        }

        private void LoadLocalizedStrings()
        {
            Title = LocalizationHelper.GetString("History_Title");
            // TitleBar displays Title automatically
        }

        private async void OnUploadClick(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.DataContext is ScreenshotItem item)
            {
                try
                {
                    btn.IsEnabled = false;
                    btn.Content = new System.Windows.Controls.TextBlock { Text = LocalizationHelper.GetString("History_Uploading"), FontSize = 11, FontWeight = FontWeights.Bold };

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(item.FilePath);
                    bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    var result = await CloudUploadManager.Instance.UploadScreenshotAsync(bitmap, item.Filename);
                    
                    btn.IsEnabled = true;
                    btn.Content = new System.Windows.Controls.TextBlock { Text = LocalizationHelper.GetString("History_Upload"), FontSize = 11, FontWeight = FontWeights.Bold };

                    if (result.Success && !string.IsNullOrEmpty(result.Url))
                    {
                        item.CloudUrl = result.Url;
                        item.CloudProvider = result.ProviderName;
                        System.Windows.Clipboard.SetText(result.Url);
                        var message = string.Format(LocalizationHelper.GetString("History_UploadSuccessMessage"), "\n", result.Url);
                        System.Windows.MessageBox.Show(message, 
                            LocalizationHelper.GetString("History_UploadSuccess"), System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    }
                    else
                    {
                        var errorMsg = result.ErrorMessage ?? LocalizationHelper.GetString("History_Error");
                        var message = string.Format(LocalizationHelper.GetString("History_UploadErrorMessage"), "\n", errorMsg);
                        System.Windows.MessageBox.Show(message, 
                            LocalizationHelper.GetString("History_UploadError"), System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    var message = string.Format(LocalizationHelper.GetString("History_ErrorWithMessage"), ex.Message);
                    System.Windows.MessageBox.Show(message, LocalizationHelper.GetString("History_Error"), System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private void OnCloudUrlClick(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.DataContext is ScreenshotItem item && !string.IsNullOrEmpty(item.CloudUrl))
            {
                System.Windows.Clipboard.SetText(item.CloudUrl);
                var message = string.Format(LocalizationHelper.GetString("History_UrlCopied"), "\n", item.CloudUrl);
                System.Windows.MessageBox.Show(message, 
                    LocalizationHelper.GetString("History_CloudUrlTitle"), System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.DataContext is ScreenshotItem item)
            {
                ScreenshotManager.Instance.DeleteScreenshot(item);
            }
        }

        private void OnClearFiltersClick(object sender, RoutedEventArgs e)
        {
            currentSearchText = null;
            currentCategory = null;
            currentTags = null;

            if (SearchTextBox != null)
                SearchTextBox.Text = "";
            if (CategoryComboBox != null)
                CategoryComboBox.SelectedItem = null;
            if (DateRangeComboBox != null)
                DateRangeComboBox.SelectedIndex = 0;

            ApplyFilters();
        }

        private void OnDateRangeChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
            UpdateEmptyState();
        }

        private void OnRefreshClick(object sender, RoutedEventArgs e)
        {
            // Refresh the history list
            ScreenshotManager.Instance.LoadHistory();
            historyViewSource = new CollectionViewSource
            {
                Source = ScreenshotManager.Instance.History
            };
            HistoryItemsControl.ItemsSource = historyViewSource.View;
            ApplyFilters();
            UpdateEmptyState();
        }

        private void OnOpenClick(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.DataContext is ScreenshotItem item)
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = item.FilePath,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    DebugHelper.LogError("HistoryWindow", "Error opening file", ex);
                    System.Windows.MessageBox.Show(
                        $"Nie można otworzyć pliku: {ex.Message}",
                        "Błąd",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private void OnEditClick(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.DataContext is ScreenshotItem item)
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(item.FilePath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    var editorWindow = new ScreenshotEditorWindow(bitmap);
                    editorWindow.Show();
                }
                catch (Exception ex)
                {
                    DebugHelper.LogError("HistoryWindow", "Error opening editor", ex);
                    System.Windows.MessageBox.Show(
                        $"Nie można otworzyć edytora: {ex.Message}",
                        "Błąd",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Error);
                }
            }
        }

        private void OnCopyLinkClick(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.DataContext is ScreenshotItem item)
            {
                if (!string.IsNullOrEmpty(item.CloudUrl))
                {
                    System.Windows.Clipboard.SetText(item.CloudUrl);

                    var messageBox = new Wpf.Ui.Controls.MessageBox
                    {
                        Title = "Link skopiowany",
                        Content = $"Link został skopiowany do schowka:\n{item.CloudUrl}",
                        ButtonLeftName = "OK"
                    };
                    messageBox.ShowDialogAsync();
                }
            }
        }

        private void OnShareClick(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.DataContext is ScreenshotItem item)
            {
                // TODO: Implement share dialog
                var messageBox = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "Udostępnij",
                    Content = "Funkcja udostępniania zostanie wkrótce dodana.",
                    ButtonLeftName = "OK"
                };
                messageBox.ShowDialogAsync();
            }
        }

        private void UpdateEmptyState()
        {
            if (EmptyStatePanel != null && HistoryItemsControl != null)
            {
                bool isEmpty = ScreenshotManager.Instance.History.Count == 0;
                EmptyStatePanel.Visibility = isEmpty ? Visibility.Visible : Visibility.Collapsed;
                HistoryItemsControl.Visibility = isEmpty ? Visibility.Collapsed : Visibility.Visible;
            }
        }
    }

    // Converter dla Visibility
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isInverse = parameter?.ToString() == "Inverse";
            bool isNull = value == null || (value is string str && string.IsNullOrEmpty(str));
            
            if (isInverse)
                return isNull ? Visibility.Visible : Visibility.Collapsed;
            else
                return !isNull ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
