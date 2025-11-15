using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Wpf.Ui.Controls;
using WindowsColor = System.Windows.Media.Color;
using WindowsFontStyle = System.Windows.FontStyle;
using WindowsFontWeight = System.Windows.FontWeight;

namespace PrettyScreenSHOT
{
    public partial class TextInputWindow : FluentWindow
    {
        public string InputText { get; private set; } = "";
        public new int FontSize { get; private set; } = 24;
        public FontFamily FontFamily { get; private set; } = new FontFamily("Segoe UI");
        public WindowsFontWeight FontWeight { get; private set; } = FontWeights.Normal;
        public WindowsFontStyle FontStyle { get; private set; } = FontStyles.Normal;
        public TextDecorationCollection TextDecorations { get; private set; } = new TextDecorationCollection();
        public WindowsColor TextColor { get; private set; } = Colors.Red;
        public TextAlignment TextAlignment { get; private set; } = TextAlignment.Left;
        public WindowsColor? BackgroundColor { get; private set; } = null;
        public WindowsColor? StrokeColor { get; private set; } = null;
        public double StrokeThickness { get; private set; } = 0;

        public void SetSuggestedFontSize(int size)
        {
            FontSize = size;
            if (FontSizeSlider != null)
                FontSizeSlider.Value = size;
            UpdatePreview();
        }

        public TextInputWindow()
        {
            InitializeComponent();

            // WPF UI applies themes globally - no need to apply per-window

            LoadLocalizedStrings();
            InitializeDefaults();
            UpdatePreview();
        }

        private void LoadLocalizedStrings()
        {
            Title = "Dodaj tekst";
            if (TextLabel != null)
                TextLabel.Text = "Tekst:";
            if (FontSizeTextLabel != null)
                FontSizeTextLabel.Text = "Rozmiar czcionki:";
            if (PreviewLabel != null)
                PreviewLabel.Text = "Podgląd:";
            PreviewText.Text = "Przykładowy tekst";
        }

        private void InitializeDefaults()
        {
            // Załaduj czcionki systemowe
            LoadSystemFonts();

            // Ustaw domyślne wartości
            if (FontSizeSlider != null)
                FontSizeSlider.Value = 24;
            if (ColorComboBox != null)
                ColorComboBox.SelectedIndex = 0; // Czerwony domyślnie
            if (BackgroundColorComboBox != null)
                BackgroundColorComboBox.SelectedIndex = 0; // Brak
            if (StrokeColorComboBox != null)
                StrokeColorComboBox.SelectedIndex = 0; // Brak

            UpdatePreview();
        }

        private void LoadSystemFonts()
        {
            if (FontFamilyComboBox == null) return;

            // Pobierz czcionki systemowe
            var fonts = Fonts.SystemFontFamilies.OrderBy(f => f.Source).ToList();

            FontFamilyComboBox.Items.Clear();
            foreach (var font in fonts)
            {
                FontFamilyComboBox.Items.Add(font.Source);
            }

            // Ustaw domyślną czcionkę (Segoe UI)
            var defaultIndex = fonts.FindIndex(f => f.Source == "Segoe UI");
            FontFamilyComboBox.SelectedIndex = defaultIndex >= 0 ? defaultIndex : 0;
        }

        private void UpdatePreview()
        {
            if (PreviewText == null) return;

            // Pobierz aktualne wartości
            FontSize = (int)(FontSizeSlider?.Value ?? 24);

            // Aktualizuj etykietę rozmiaru
            if (FontSizeValueLabel != null)
                FontSizeValueLabel.Text = FontSize.ToString();

            // Ustaw czcionkę
            if (FontFamilyComboBox != null && FontFamilyComboBox.SelectedItem != null)
            {
                var fontName = FontFamilyComboBox.SelectedItem.ToString();
                PreviewText.FontFamily = new FontFamily(fontName);
            }

            // Ustaw właściwości tekstu
            PreviewText.FontSize = FontSize;
            PreviewText.FontWeight = BoldCheckBox?.IsChecked == true ? FontWeights.Bold : FontWeights.Normal;
            PreviewText.FontStyle = ItalicCheckBox?.IsChecked == true ? FontStyles.Italic : FontStyles.Normal;

            // Ustaw wyrównanie
            if (AlignLeftRadio?.IsChecked == true)
                PreviewText.TextAlignment = TextAlignment.Left;
            else if (AlignCenterRadio?.IsChecked == true)
                PreviewText.TextAlignment = TextAlignment.Center;
            else if (AlignRightRadio?.IsChecked == true)
                PreviewText.TextAlignment = TextAlignment.Right;

            // Ustaw dekoracje tekstu
            var decorations = new TextDecorationCollection();
            if (UnderlineCheckBox?.IsChecked == true)
            {
                decorations.Add(System.Windows.TextDecorations.Underline);
            }
            if (StrikethroughCheckBox?.IsChecked == true)
            {
                decorations.Add(System.Windows.TextDecorations.Strikethrough);
            }
            PreviewText.TextDecorations = decorations;

            // Ustaw kolor tekstu
            UpdateTextColor();

            // Ustaw tło tekstu
            UpdateBackgroundColor();

            // Ustaw obramowanie (WPF nie ma natywnego stroke dla TextBlock, więc symulujemy)
            UpdateStroke();

            // Ustaw tekst do podglądu
            var previewText = TextInputControl?.Text ?? "";
            PreviewText.Text = string.IsNullOrWhiteSpace(previewText) ? "Przykładowy tekst" : previewText;
        }

        private void UpdateTextColor()
        {
            if (PreviewText == null || ColorComboBox == null) return;

            WindowsColor selectedColor;
            switch (ColorComboBox.SelectedIndex)
            {
                case 0: selectedColor = Colors.Red; break;
                case 1: selectedColor = Colors.Green; break;
                case 2: selectedColor = Colors.Blue; break;
                case 3: selectedColor = Colors.Yellow; break;
                case 4: selectedColor = Colors.Purple; break;
                case 5: selectedColor = Colors.Orange; break;
                case 6: selectedColor = Colors.Black; break;
                case 7: selectedColor = Colors.White; break;
                default: selectedColor = Colors.Red; break;
            }

            TextColor = selectedColor;
            PreviewText.Foreground = new SolidColorBrush(selectedColor);
        }

        private void UpdateBackgroundColor()
        {
            if (PreviewText == null || BackgroundColorComboBox == null) return;

            switch (BackgroundColorComboBox.SelectedIndex)
            {
                case 0: // Brak
                    BackgroundColor = null;
                    PreviewText.Background = Brushes.Transparent;
                    break;
                case 1: // Biały
                    BackgroundColor = Colors.White;
                    PreviewText.Background = Brushes.White;
                    break;
                case 2: // Czarny
                    BackgroundColor = Colors.Black;
                    PreviewText.Background = Brushes.Black;
                    break;
                case 3: // Żółty
                    BackgroundColor = Colors.Yellow;
                    PreviewText.Background = Brushes.Yellow;
                    break;
                case 4: // Niebieski
                    BackgroundColor = Colors.LightBlue;
                    PreviewText.Background = Brushes.LightBlue;
                    break;
                case 5: // Zielony
                    BackgroundColor = Colors.LightGreen;
                    PreviewText.Background = Brushes.LightGreen;
                    break;
                case 6: // Różowy
                    BackgroundColor = Colors.Pink;
                    PreviewText.Background = Brushes.Pink;
                    break;
                default:
                    BackgroundColor = null;
                    PreviewText.Background = Brushes.Transparent;
                    break;
            }
        }

        private void UpdateStroke()
        {
            if (StrokeColorComboBox == null || StrokeThicknessSlider == null) return;

            StrokeThickness = StrokeThicknessSlider.Value;
            if (StrokeThicknessLabel != null)
                StrokeThicknessLabel.Text = StrokeThickness.ToString("F0");

            // WPF TextBlock nie ma natywnego stroke, więc zapisujemy tylko wartości
            // Będą użyte w ScreenshotEditorWindow przy renderowaniu
            switch (StrokeColorComboBox.SelectedIndex)
            {
                case 0: // Brak
                    StrokeColor = null;
                    break;
                case 1: // Czarny
                    StrokeColor = Colors.Black;
                    break;
                case 2: // Biały
                    StrokeColor = Colors.White;
                    break;
                case 3: // Szary
                    StrokeColor = Colors.Gray;
                    break;
                default:
                    StrokeColor = null;
                    break;
            }
        }

        private void FontSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdatePreview();
        }

        private void TextInputControl_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void StyleCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdatePreview();
        }

        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateTextColor();
        }

        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void TextAlignment_Changed(object sender, RoutedEventArgs e)
        {
            UpdatePreview();
        }

        private void BackgroundColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateBackgroundColor();
        }

        private void StrokeColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateStroke();
        }

        private void StrokeThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateStroke();
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            if (TextInputControl == null) return;

            InputText = TextInputControl.Text.Trim();

            if (string.IsNullOrWhiteSpace(InputText))
            {
                System.Windows.MessageBox.Show("Wprowadź tekst do wyświetlenia.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Zapisz wszystkie właściwości formatowania
            FontSize = (int)(FontSizeSlider?.Value ?? 24);

            // Czcionka
            if (FontFamilyComboBox != null && FontFamilyComboBox.SelectedItem != null)
            {
                var fontName = FontFamilyComboBox.SelectedItem.ToString();
                FontFamily = new FontFamily(fontName);
            }

            FontWeight = BoldCheckBox?.IsChecked == true ? FontWeights.Bold : FontWeights.Normal;
            FontStyle = ItalicCheckBox?.IsChecked == true ? FontStyles.Italic : FontStyles.Normal;

            // Wyrównanie
            if (AlignLeftRadio?.IsChecked == true)
                TextAlignment = TextAlignment.Left;
            else if (AlignCenterRadio?.IsChecked == true)
                TextAlignment = TextAlignment.Center;
            else if (AlignRightRadio?.IsChecked == true)
                TextAlignment = TextAlignment.Right;

            // Utwórz dekoracje
            TextDecorations = new TextDecorationCollection();
            if (UnderlineCheckBox?.IsChecked == true)
                TextDecorations.Add(System.Windows.TextDecorations.Underline);
            if (StrikethroughCheckBox?.IsChecked == true)
                TextDecorations.Add(System.Windows.TextDecorations.Strikethrough);

            // Ustaw kolor tekstu
            UpdateTextColor();

            // Ustaw tło
            UpdateBackgroundColor();

            // Ustaw obramowanie
            UpdateStroke();

            this.DialogResult = true;
            this.Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
