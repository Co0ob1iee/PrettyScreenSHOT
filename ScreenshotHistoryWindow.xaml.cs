using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PrettyScreenSHOT
{
    public partial class ScreenshotHistoryWindow : Window
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
            HistoryListBox.ItemsSource = historyViewSource.View;
            
            LoadLocalizedStrings();
            InitializeSearchAndFilter();
        }

        private void InitializeSearchAndFilter()
        {
            // Ustaw placeholder dla wyszukiwania
            if (SearchTextBox != null)
            {
                SearchTextBox.Tag = LocalizationHelper.GetString("History_SearchPlaceholder");
                SearchTextBox.GotFocus += (s, e) =>
                {
                    if (SearchTextBox.Text == SearchTextBox.Tag?.ToString())
                        SearchTextBox.Text = "";
                };
                SearchTextBox.LostFocus += (s, e) =>
                {
                    if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
                        SearchTextBox.Text = SearchTextBox.Tag?.ToString() ?? "";
                };
                SearchTextBox.Text = SearchTextBox.Tag?.ToString() ?? "";
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
            if (TitleText != null)
                TitleText.Text = LocalizationHelper.GetString("History_Title");
            if (SubtitleText != null)
                SubtitleText.Text = LocalizationHelper.GetString("History_Subtitle");
            // DeleteText jest w DataTemplate, więc nie jest dostępny tutaj
        }

        private async void OnUploadClick(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.DataContext is ScreenshotItem item)
            {
                try
                {
                    btn.IsEnabled = false;
                    btn.Content = new TextBlock { Text = LocalizationHelper.GetString("History_Uploading"), FontSize = 11, FontWeight = FontWeights.Bold };

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(item.FilePath);
                    bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    var result = await CloudUploadManager.Instance.UploadScreenshotAsync(bitmap, item.Filename);
                    
                    btn.IsEnabled = true;
                    btn.Content = new TextBlock { Text = LocalizationHelper.GetString("History_Upload"), FontSize = 11, FontWeight = FontWeights.Bold };

                    if (result.Success && !string.IsNullOrEmpty(result.Url))
                    {
                        item.CloudUrl = result.Url;
                        item.CloudProvider = result.ProviderName;
                        System.Windows.Clipboard.SetText(result.Url);
                        var message = string.Format(LocalizationHelper.GetString("History_UploadSuccessMessage"), "\n", result.Url);
                        System.Windows.MessageBox.Show(message, 
                            LocalizationHelper.GetString("History_UploadSuccess"), MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        var errorMsg = result.ErrorMessage ?? LocalizationHelper.GetString("History_Error");
                        var message = string.Format(LocalizationHelper.GetString("History_UploadErrorMessage"), "\n", errorMsg);
                        System.Windows.MessageBox.Show(message, 
                            LocalizationHelper.GetString("History_UploadError"), MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    var message = string.Format(LocalizationHelper.GetString("History_ErrorWithMessage"), ex.Message);
                    System.Windows.MessageBox.Show(message, LocalizationHelper.GetString("History_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
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
                    LocalizationHelper.GetString("History_CloudUrlTitle"), MessageBoxButton.OK, MessageBoxImage.Information);
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

            ApplyFilters();
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
            this.Close();
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
