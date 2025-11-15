using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using PrettyScreenSHOT.Helpers;
using PrettyScreenSHOT.Services.Settings;
using PrettyScreenSHOT.Services.Theme;

namespace PrettyScreenSHOT.Views.Windows
{
    public partial class SettingsWindow : Window
    {
        private readonly SettingsManager settingsManager;

        public SettingsWindow()
        {
            InitializeComponent();

            // Zastosuj theme
            ThemeManager.Instance.ApplyTheme(this);

            // Subscribe to Loaded to attach event handlers after UI is ready
            this.Loaded += SettingsWindow_Loaded;

            settingsManager = SettingsManager.Instance;
            LoadSettings();
            LoadLocalizedStrings();
        }

        private void SettingsWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            // Attach handlers here to avoid events firing during initialization
            if (LanguageComboBox != null)
            {
                // Ensure not attached multiple times
                LanguageComboBox.SelectionChanged -= LanguageComboBox_SelectionChanged;
                LanguageComboBox.SelectionChanged += LanguageComboBox_SelectionChanged;
            }

            if (QualitySlider != null)
            {
                QualitySlider.ValueChanged -= QualitySlider_ValueChanged;
                QualitySlider.ValueChanged += QualitySlider_ValueChanged;
            }

            if (ThemeComboBox != null)
            {
                ThemeComboBox.SelectionChanged -= ThemeComboBox_SelectionChanged;
                ThemeComboBox.SelectionChanged += ThemeComboBox_SelectionChanged;
            }
        }

        private void LoadLocalizedStrings()
        {
            Title = LocalizationHelper.GetString("Settings_Title");
            TitleText.Text = LocalizationHelper.GetString("Settings_Title");
            LanguageLabel.Text = LocalizationHelper.GetString("Settings_Language");
            SavePathLabel.Text = LocalizationHelper.GetString("Settings_SavePath");
            BrowseButton.Content = LocalizationHelper.GetString("Settings_Browse");
            HotkeyLabel.Text = LocalizationHelper.GetString("Settings_Hotkey");
            ImageFormatLabel.Text = LocalizationHelper.GetString("Settings_ImageFormat");
            ImageQualityLabel.Text = LocalizationHelper.GetString("Settings_ImageQuality");
            AutoSaveCheckBox.Content = LocalizationHelper.GetString("Settings_AutoSave");
            CopyToClipboardCheckBox.Content = LocalizationHelper.GetString("Settings_CopyToClipboard");
            ShowNotificationsCheckBox.Content = LocalizationHelper.GetString("Settings_ShowNotifications");
            if (AutoUploadCheckBox != null)
                AutoUploadCheckBox.Content = LocalizationHelper.GetString("Settings_AutoUpload");
            if (ThemeLabel != null)
                ThemeLabel.Text = LocalizationHelper.GetString("Settings_Theme");
            SaveButton.Content = LocalizationHelper.GetString("Settings_Save");
            CancelButton2.Content = LocalizationHelper.GetString("Settings_Cancel");
            ResetButton.Content = LocalizationHelper.GetString("Settings_Reset");
        }

        private void LoadSettings()
        {
            // Language
            LanguageComboBox.Items.Clear();
            LanguageComboBox.Items.Add(new { Code = "en", Name = "English" });
            LanguageComboBox.Items.Add(new { Code = "pl", Name = "Polski" });
            LanguageComboBox.Items.Add(new { Code = "de", Name = "Deutsch" });
            LanguageComboBox.Items.Add(new { Code = "zh", Name = "中文" });
            LanguageComboBox.Items.Add(new { Code = "fr", Name = "Français" });
            
            LanguageComboBox.DisplayMemberPath = "Name";
            LanguageComboBox.SelectedValuePath = "Code";
            
            var currentLang = settingsManager.Language;
            foreach (var item in LanguageComboBox.Items)
            {
                var langItem = item as dynamic;
                if (langItem?.Code == currentLang)
                {
                    LanguageComboBox.SelectedItem = item;
                    break;
                }
            }

            // Save Path
            SavePathTextBox.Text = settingsManager.SavePath;

            // Hotkey
            HotkeyComboBox.Items.Clear();
            HotkeyComboBox.Items.Add("PRTSCN");
            HotkeyComboBox.Items.Add("CTRL+PRTSCN");
            HotkeyComboBox.Items.Add("ALT+PRTSCN");
            HotkeyComboBox.Items.Add("SHIFT+PRTSCN");
            HotkeyComboBox.SelectedItem = settingsManager.Hotkey;

            // Image Format
            ImageFormatComboBox.Items.Clear();
            ImageFormatComboBox.Items.Add("PNG");
            ImageFormatComboBox.Items.Add("JPG");
            ImageFormatComboBox.Items.Add("BMP");
            ImageFormatComboBox.SelectedItem = settingsManager.ImageFormat;

            // Quality
            QualitySlider.Value = settingsManager.ImageQuality;
            QualityLabel.Text = $"{settingsManager.ImageQuality}%";

            // Options
            AutoSaveCheckBox.IsChecked = settingsManager.AutoSave;
            CopyToClipboardCheckBox.IsChecked = settingsManager.CopyToClipboard;
            ShowNotificationsCheckBox.IsChecked = settingsManager.ShowNotifications;
            if (AutoUploadCheckBox != null)
                AutoUploadCheckBox.IsChecked = settingsManager.AutoUpload;

            // Sprint 3: Advanced Capture Options
            if (PrivacyModeCheckBox != null)
                PrivacyModeCheckBox.IsChecked = settingsManager.PrivacyMode;
            if (CaptureCursorCheckBox != null)
                CaptureCursorCheckBox.IsChecked = settingsManager.CaptureCursor;
            if (TimedCaptureComboBox != null)
            {
                int delay = settingsManager.TimedCaptureDelay;
                foreach (System.Windows.Controls.ComboBoxItem item in TimedCaptureComboBox.Items)
                {
                    if (item.Tag != null && int.Parse(item.Tag.ToString()!) == delay)
                    {
                        TimedCaptureComboBox.SelectedItem = item;
                        break;
                    }
                }
                if (TimedCaptureComboBox.SelectedItem == null)
                    TimedCaptureComboBox.SelectedIndex = 0; // Default to Disabled
            }

            // Theme
            if (ThemeComboBox != null)
            {
                ThemeComboBox.Items.Clear();
                ThemeComboBox.Items.Add("Dark");
                ThemeComboBox.Items.Add("Light");
                ThemeComboBox.SelectedItem = settingsManager.Theme;
            }
        }

        private void LanguageComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (LanguageComboBox.SelectedItem != null)
            {
                var selected = LanguageComboBox.SelectedItem as dynamic;
                if (selected != null)
                {
                    var code = selected.Code as string;
                    if (!string.IsNullOrEmpty(code))
                    {
                        LocalizationHelper.SetLanguage(code);
                        LoadLocalizedStrings();
                    }
                }
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = LocalizationHelper.GetString("Settings_SavePath");
                dialog.SelectedPath = SavePathTextBox.Text;
                
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SavePathTextBox.Text = dialog.SelectedPath;
                }
            }
        }

        private void QualitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Guard against cases where the slider event fires before UI elements are initialized
            if (QualityLabel == null)
                return;

            QualityLabel.Text = $"{(int)e.NewValue}%";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate save path
                if (!string.IsNullOrEmpty(SavePathTextBox.Text))
                {
                    if (!Directory.Exists(SavePathTextBox.Text))
                    {
                        try
                        {
                            Directory.CreateDirectory(SavePathTextBox.Text);
                        }
                        catch
                        {
                            System.Windows.MessageBox.Show(
                                LocalizationHelper.GetString("Settings_InvalidPath"),
                                LocalizationHelper.GetString("Settings_SaveError"),
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                            return;
                        }
                    }
                }

                // Save settings
                if (LanguageComboBox.SelectedItem != null)
                {
                    var selected = LanguageComboBox.SelectedItem as dynamic;
                    settingsManager.Language = selected?.Code ?? "en";
                }

                settingsManager.SavePath = SavePathTextBox.Text;
                settingsManager.Hotkey = HotkeyComboBox.SelectedItem?.ToString() ?? "PRTSCN";
                settingsManager.ImageFormat = ImageFormatComboBox.SelectedItem?.ToString() ?? "PNG";
                settingsManager.ImageQuality = (int)QualitySlider.Value;
                settingsManager.AutoSave = AutoSaveCheckBox.IsChecked ?? true;
                settingsManager.CopyToClipboard = CopyToClipboardCheckBox.IsChecked ?? true;
                settingsManager.ShowNotifications = ShowNotificationsCheckBox.IsChecked ?? true;
                if (AutoUploadCheckBox != null)
                    settingsManager.AutoUpload = AutoUploadCheckBox.IsChecked ?? false;
                
                // Theme
                if (ThemeComboBox != null && ThemeComboBox.SelectedItem != null)
                {
                    settingsManager.Theme = ThemeComboBox.SelectedItem.ToString() ?? "Dark";
                }

                // Sprint 3: Advanced Capture Options
                if (PrivacyModeCheckBox != null)
                    settingsManager.PrivacyMode = PrivacyModeCheckBox.IsChecked ?? false;
                if (CaptureCursorCheckBox != null)
                    settingsManager.CaptureCursor = CaptureCursorCheckBox.IsChecked ?? false;
                if (TimedCaptureComboBox != null && TimedCaptureComboBox.SelectedItem is System.Windows.Controls.ComboBoxItem selectedItem)
                {
                    if (selectedItem.Tag != null)
                        settingsManager.TimedCaptureDelay = int.Parse(selectedItem.Tag.ToString()!);
                }

                System.Windows.MessageBox.Show(
                    LocalizationHelper.GetString("Settings_SaveSuccessMessage"),
                    LocalizationHelper.GetString("Settings_SaveSuccess"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("Settings", "Failed to save settings", ex);
                var message = string.Format(LocalizationHelper.GetString("Settings_ErrorWithMessage"), ex.Message);
                System.Windows.MessageBox.Show(
                    message,
                    LocalizationHelper.GetString("Settings_SaveError"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ThemeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ThemeComboBox != null && ThemeComboBox.SelectedItem != null)
            {
                var themeName = ThemeComboBox.SelectedItem.ToString();
                if (!string.IsNullOrEmpty(themeName))
                {
                    // Apply theme using WPF UI ThemeService
                    ThemeService.Instance.SetTheme(themeName);
                }
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            var result = System.Windows.MessageBox.Show(
                LocalizationHelper.GetString("Settings_ResetConfirm"),
                LocalizationHelper.GetString("Editor_Confirm"),
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                settingsManager.ResetToDefaults();
                LoadSettings();
                LoadLocalizedStrings();
            }
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
}

