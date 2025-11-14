using System;
using System.Windows;
using System.Windows.Media;

namespace PrettyScreenSHOT
{
    public enum Theme
    {
        Dark,
        Light
    }

    public class ThemeManager
    {
        private static readonly ThemeManager instance = new();
        public static ThemeManager Instance => instance;

        public Theme CurrentTheme { get; private set; } = Theme.Dark;

        public event EventHandler<Theme>? ThemeChanged;

        private ThemeManager()
        {
            // Załaduj theme z ustawień
            var themeName = SettingsManager.Instance.Theme;
            if (Enum.TryParse<Theme>(themeName, true, out var theme))
            {
                CurrentTheme = theme;
            }
        }

        public void SetTheme(Theme theme)
        {
            if (CurrentTheme == theme) return;

            CurrentTheme = theme;
            SettingsManager.Instance.Theme = theme.ToString();
            
            ApplyThemeToAllWindows();
            ThemeChanged?.Invoke(this, theme);
            
            DebugHelper.LogInfo("Theme", $"Theme changed to: {theme}");
        }

        public void ToggleTheme()
        {
            SetTheme(CurrentTheme == Theme.Dark ? Theme.Light : Theme.Dark);
        }

        private void ApplyThemeToAllWindows()
        {
            foreach (Window window in Application.Current.Windows)
            {
                ApplyTheme(window);
            }
        }

        public void ApplyTheme(Window window)
        {
            if (window == null) return;

            var colors = GetThemeColors();
            
            // Zastosuj kolory do okna
            window.Background = new SolidColorBrush(colors.WindowBackground);
            window.Foreground = new SolidColorBrush(colors.TextPrimary);

            // Zastosuj do wszystkich kontrolek w oknie
            ApplyThemeToElement(window, colors);
        }

        private void ApplyThemeToElement(DependencyObject element, ThemeColors colors)
        {
            if (element == null) return;

            // Rekurencyjnie zastosuj do wszystkich elementów
            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(element, i);
                ApplyThemeToElement(child, colors);
            }

            // Zastosuj style do konkretnych typów kontrolek
            if (element is System.Windows.Controls.Border border)
            {
                if (border.Background is SolidColorBrush bgBrush && bgBrush.Color == Color.FromRgb(0x1E, 0x1E, 0x1E))
                    border.Background = new SolidColorBrush(colors.PanelBackground);
                if (border.BorderBrush is SolidColorBrush bbBrush && bbBrush.Color == Color.FromRgb(0x40, 0x40, 0x40))
                    border.BorderBrush = new SolidColorBrush(colors.Border);
            }
            else if (element is System.Windows.Controls.TextBlock textBlock)
            {
                if (textBlock.Foreground is SolidColorBrush fgBrush && fgBrush.Color == Colors.White)
                    textBlock.Foreground = new SolidColorBrush(colors.TextPrimary);
                else if (textBlock.Foreground is SolidColorBrush fgBrush2 && fgBrush2.Color == Color.FromRgb(0xAA, 0xAA, 0xAA))
                    textBlock.Foreground = new SolidColorBrush(colors.TextSecondary);
            }
            else if (element is System.Windows.Controls.TextBox textBox)
            {
                if (textBox.Background is SolidColorBrush bgBrush && bgBrush.Color == Color.FromRgb(0x2D, 0x2D, 0x2D))
                    textBox.Background = new SolidColorBrush(colors.InputBackground);
                if (textBox.Foreground is SolidColorBrush fgBrush && fgBrush.Color == Colors.White)
                    textBox.Foreground = new SolidColorBrush(colors.TextPrimary);
                if (textBox.BorderBrush is SolidColorBrush bbBrush && bbBrush.Color == Color.FromRgb(0x40, 0x40, 0x40))
                    textBox.BorderBrush = new SolidColorBrush(colors.Border);
            }
            else if (element is System.Windows.Controls.Button button)
            {
                // Nie zmieniaj kolorów przycisków akcji (zachowaj ich kolory)
                // Tylko zmień tło jeśli jest domyślne dark
            }
        }

        public ThemeColors GetThemeColors()
        {
            return CurrentTheme == Theme.Dark ? DarkColors : LightColors;
        }

        public static ThemeColors DarkColors => new()
        {
            WindowBackground = Color.FromRgb(0x1E, 0x1E, 0x1E),
            PanelBackground = Color.FromRgb(0x2D, 0x2D, 0x2D),
            InputBackground = Color.FromRgb(0x2D, 0x2D, 0x2D),
            TextPrimary = Colors.White,
            TextSecondary = Color.FromRgb(0xAA, 0xAA, 0xAA),
            Border = Color.FromRgb(0x40, 0x40, 0x40),
            Accent = Color.FromRgb(0x60, 0xA5, 0xFA),
            ButtonBackground = Color.FromRgb(0x3F, 0x3F, 0x3F),
            ButtonHover = Color.FromRgb(0x4F, 0x4F, 0x4F)
        };

        public static ThemeColors LightColors => new()
        {
            WindowBackground = Color.FromRgb(0xFA, 0xFA, 0xFA),
            PanelBackground = Colors.White,
            InputBackground = Colors.White,
            TextPrimary = Color.FromRgb(0x21, 0x21, 0x21),
            TextSecondary = Color.FromRgb(0x66, 0x66, 0x66),
            Border = Color.FromRgb(0xE0, 0xE0, 0xE0),
            Accent = Color.FromRgb(0x21, 0x96, 0xF3),
            ButtonBackground = Color.FromRgb(0xF5, 0xF5, 0xF5),
            ButtonHover = Color.FromRgb(0xE0, 0xE0, 0xE0)
        };
    }

    public class ThemeColors
    {
        public Color WindowBackground { get; set; }
        public Color PanelBackground { get; set; }
        public Color InputBackground { get; set; }
        public Color TextPrimary { get; set; }
        public Color TextSecondary { get; set; }
        public Color Border { get; set; }
        public Color Accent { get; set; }
        public Color ButtonBackground { get; set; }
        public Color ButtonHover { get; set; }
    }
}

