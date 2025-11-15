using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WindowsColor = System.Windows.Media.Color;
using WindowsFontStyle = System.Windows.FontStyle;
using WindowsFontWeight = System.Windows.FontWeight;

namespace PrettyScreenSHOT
{
    public partial class TextInputWindow : Window
    {
        public string InputText { get; private set; } = "";
        public new int FontSize { get; private set; } = 24;
        public WindowsFontWeight FontWeight { get; private set; } = FontWeights.Normal;
        public WindowsFontStyle FontStyle { get; private set; } = FontStyles.Normal;
        public TextDecorationCollection TextDecorations { get; private set; } = new TextDecorationCollection();
        public WindowsColor TextColor { get; private set; } = Colors.Red;

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

            // Zastosuj theme
            ThemeManager.Instance.ApplyTheme(this);

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
            // Ustaw domyślne wartości
            if (FontSizeSlider != null)
                FontSizeSlider.Value = 24;
            if (ColorComboBox != null)
                ColorComboBox.SelectedIndex = 0; // Czerwony domyślnie

            UpdatePreview();
        }

        private void UpdatePreview()
        {
            if (PreviewText == null) return;

            // Pobierz aktualne wartości
            FontSize = (int)(FontSizeSlider?.Value ?? 24);

            // Aktualizuj etykietę rozmiaru
            if (FontSizeValueLabel != null)
                FontSizeValueLabel.Text = FontSize.ToString();

            // Ustaw właściwości tekstu
            PreviewText.FontSize = FontSize;
            PreviewText.FontWeight = BoldCheckBox?.IsChecked == true ? FontWeights.Bold : FontWeights.Normal;
            PreviewText.FontStyle = ItalicCheckBox?.IsChecked == true ? FontStyles.Italic : FontStyles.Normal;

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
            FontWeight = BoldCheckBox?.IsChecked == true ? FontWeights.Bold : FontWeights.Normal;
            FontStyle = ItalicCheckBox?.IsChecked == true ? FontStyles.Italic : FontStyles.Normal;

            // Utwórz dekoracje
            TextDecorations = new TextDecorationCollection();
            if (UnderlineCheckBox?.IsChecked == true)
                TextDecorations.Add(System.Windows.TextDecorations.Underline);
            if (StrikethroughCheckBox?.IsChecked == true)
                TextDecorations.Add(System.Windows.TextDecorations.Strikethrough);

            // Ustaw kolor
            UpdateTextColor();

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
