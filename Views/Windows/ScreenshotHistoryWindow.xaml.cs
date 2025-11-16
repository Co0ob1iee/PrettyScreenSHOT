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
            HistoryListBox.ItemsSource = historyViewSource.View;
            
            LoadLocalizedStrings();
            InitializeSearchAndFilter();
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
                        MessageBoxHelper.Show(message,
                            LocalizationHelper.GetString("History_UploadSuccess"), System.Windows.MessageBoxButton.OK);
                    }
                    else
                    {
                        var errorMsg = result.ErrorMessage ?? LocalizationHelper.GetString("History_Error");
                        var message = string.Format(LocalizationHelper.GetString("History_UploadErrorMessage"), "\n", errorMsg);
                        MessageBoxHelper.Show(message,
                            LocalizationHelper.GetString("History_UploadError"), System.Windows.MessageBoxButton.OK);
                    }
                }
                catch (Exception ex)
                {
                    var message = string.Format(LocalizationHelper.GetString("History_ErrorWithMessage"), ex.Message);
                    MessageBoxHelper.Show(message, LocalizationHelper.GetString("History_Error"), System.Windows.MessageBoxButton.OK);
                }
            }
        }

        private void OnCloudUrlClick(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.DataContext is ScreenshotItem item && !string.IsNullOrEmpty(item.CloudUrl))
            {
                System.Windows.Clipboard.SetText(item.CloudUrl);
                var message = string.Format(LocalizationHelper.GetString("History_UrlCopied"), "\n", item.CloudUrl);
                MessageBoxHelper.Show(message,
                    LocalizationHelper.GetString("History_CloudUrlTitle"), System.Windows.MessageBoxButton.OK);
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
    }
}
