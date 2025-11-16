using System.Windows;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace PrettyScreenSHOT.Views.Dialogs
{
    public partial class ColorPickerDialog : FluentWindow
    {
        public Color SelectedColor { get; private set; }

        public ColorPickerDialog()
        {
            InitializeComponent();
            SelectedColor = Colors.Red;
            UpdateColorPreview();
        }

        public ColorPickerDialog(Color initialColor) : this()
        {
            SelectedColor = initialColor;
            RedSlider.Value = initialColor.R;
            GreenSlider.Value = initialColor.G;
            BlueSlider.Value = initialColor.B;
            UpdateColorPreview();
        }

        private void OnColorChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (RedValueText == null || GreenValueText == null || BlueValueText == null)
                return;

            RedValueText.Text = ((int)RedSlider.Value).ToString();
            GreenValueText.Text = ((int)GreenSlider.Value).ToString();
            BlueValueText.Text = ((int)BlueSlider.Value).ToString();

            UpdateColorPreview();
        }

        private void UpdateColorPreview()
        {
            if (ColorPreview == null)
                return;

            byte r = (byte)RedSlider.Value;
            byte g = (byte)GreenSlider.Value;
            byte b = (byte)BlueSlider.Value;

            SelectedColor = Color.FromRgb(r, g, b);
            ColorPreview.Background = new SolidColorBrush(SelectedColor);
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
