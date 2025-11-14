using System.Windows;
using System.Windows.Controls;

namespace PrettyScreenSHOT
{
    public partial class TextInputWindow : Window
    {
        public string InputText { get; private set; } = "";
        public new int FontSize { get; private set; } = 24;

        public TextInputWindow()
        {
            InitializeComponent();
            
            // Zastosuj theme
            ThemeManager.Instance.ApplyTheme(this);
            
            FontSizeSlider.ValueChanged += (s, e) => UpdatePreview();
            TextInputControl.TextChanged += (s, e) => UpdatePreview();
            LoadLocalizedStrings();
        }

        private void LoadLocalizedStrings()
        {
            Title = LocalizationHelper.GetString("TextInput_Title");
            if (TextLabel != null)
                TextLabel.Text = LocalizationHelper.GetString("TextInput_Text");
            if (FontSizeTextLabel != null)
                FontSizeTextLabel.Text = LocalizationHelper.GetString("TextInput_FontSize");
            if (PreviewLabel != null)
                PreviewLabel.Text = LocalizationHelper.GetString("TextInput_Preview");
            PreviewText.Text = LocalizationHelper.GetString("TextInput_SampleText");
        }

        private void UpdatePreview()
        {
            FontSize = (int)FontSizeSlider.Value;
            if (FontSizeValueLabel != null)
                FontSizeValueLabel.Text = FontSize.ToString();
            PreviewText.Text = TextInputControl.Text.Length > 0 ? TextInputControl.Text : LocalizationHelper.GetString("TextInput_SampleText");
            PreviewText.FontSize = FontSize;
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            InputText = TextInputControl.Text;
            FontSize = (int)FontSizeSlider.Value;
            
            if (string.IsNullOrWhiteSpace(InputText))
            {
                MessageBox.Show(LocalizationHelper.GetString("TextInput_EnterText"));
                return;
            }

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
