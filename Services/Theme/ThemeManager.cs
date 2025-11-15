using System;
using System.Windows;
using System.Windows.Media;
using PrettyScreenSHOT.Helpers;
using PrettyScreenSHOT.Services.Settings;

namespace PrettyScreenSHOT.Services.Theme
{
    public enum Theme
    {
        Dark,
        Light,
        Neumorphic
    }

    public class ThemeManager
    {
        private static readonly ThemeManager instance = new();
        public static ThemeManager Instance => instance;

        public Theme CurrentTheme { get; private set; } = Theme.Neumorphic;

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
            foreach (Window window in System.Windows.Application.Current.Windows)
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
                if (border.Background is SolidColorBrush bgBrush && bgBrush.Color == System.Windows.Media.Color.FromRgb(0x1E, 0x1E, 0x1E))
                    border.Background = new SolidColorBrush(colors.PanelBackground);
                if (border.BorderBrush is SolidColorBrush bbBrush && bbBrush.Color == System.Windows.Media.Color.FromRgb(0x40, 0x40, 0x40))
                    border.BorderBrush = new SolidColorBrush(colors.Border);
            }
            else if (element is System.Windows.Controls.TextBlock textBlock)
            {
                if (textBlock.Foreground is SolidColorBrush fgBrush && fgBrush.Color == System.Windows.Media.Colors.White)
                    textBlock.Foreground = new SolidColorBrush(colors.TextPrimary);
                else if (textBlock.Foreground is SolidColorBrush fgBrush2 && fgBrush2.Color == System.Windows.Media.Color.FromRgb(0xAA, 0xAA, 0xAA))
                    textBlock.Foreground = new SolidColorBrush(colors.TextSecondary);
            }
            else if (element is System.Windows.Controls.TextBox textBox)
            {
                if (textBox.Background is SolidColorBrush bgBrush && bgBrush.Color == System.Windows.Media.Color.FromRgb(0x2D, 0x2D, 0x2D))
                    textBox.Background = new SolidColorBrush(colors.InputBackground);
                if (textBox.Foreground is SolidColorBrush fgBrush && fgBrush.Color == System.Windows.Media.Colors.White)
                    textBox.Foreground = new SolidColorBrush(colors.TextPrimary);
                if (textBox.BorderBrush is SolidColorBrush bbBrush && bbBrush.Color == System.Windows.Media.Color.FromRgb(0x40, 0x40, 0x40))
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
            return CurrentTheme switch
            {
                Theme.Dark => DarkColors,
                Theme.Light => LightColors,
                Theme.Neumorphic => NeumorphicColors,
                _ => DarkColors
            };
        }

        public static ThemeColors DarkColors => new()
        {
            WindowBackground = System.Windows.Media.Color.FromRgb(0x1E, 0x1E, 0x1E),
            PanelBackground = System.Windows.Media.Color.FromRgb(0x2D, 0x2D, 0x2D),
            InputBackground = System.Windows.Media.Color.FromRgb(0x2D, 0x2D, 0x2D),
            TextPrimary = System.Windows.Media.Colors.White,
            TextSecondary = System.Windows.Media.Color.FromRgb(0xAA, 0xAA, 0xAA),
            Border = System.Windows.Media.Color.FromRgb(0x40, 0x40, 0x40),
            Accent = System.Windows.Media.Color.FromRgb(0x60, 0xA5, 0xFA),
            ButtonBackground = System.Windows.Media.Color.FromRgb(0x3F, 0x3F, 0x3F),
            ButtonHover = System.Windows.Media.Color.FromRgb(0x4F, 0x4F, 0x4F)
        };

        public static ThemeColors LightColors => new()
        {
            WindowBackground = System.Windows.Media.Color.FromRgb(0xFA, 0xFA, 0xFA),
            PanelBackground = System.Windows.Media.Colors.White,
            InputBackground = System.Windows.Media.Colors.White,
            TextPrimary = System.Windows.Media.Color.FromRgb(0x21, 0x21, 0x21),
            TextSecondary = System.Windows.Media.Color.FromRgb(0x66, 0x66, 0x66),
            Border = System.Windows.Media.Color.FromRgb(0xE0, 0xE0, 0xE0),
            Accent = System.Windows.Media.Color.FromRgb(0x21, 0x96, 0xF3),
            ButtonBackground = System.Windows.Media.Color.FromRgb(0xF5, 0xF5, 0xF5),
            ButtonHover = System.Windows.Media.Color.FromRgb(0xE0, 0xE0, 0xE0)
        };

        public static ThemeColors NeumorphicColors => new()
        {
            WindowBackground = System.Windows.Media.Color.FromRgb(0xE5, 0xE5, 0xE5), // #E5E5E5
            PanelBackground = System.Windows.Media.Color.FromRgb(0xE5, 0xE5, 0xE5), // #E5E5E5
            InputBackground = System.Windows.Media.Color.FromRgb(0xE5, 0xE5, 0xE5), // #E5E5E5
            TextPrimary = System.Windows.Media.Color.FromRgb(0x21, 0x21, 0x21),      // #212121
            TextSecondary = System.Windows.Media.Color.FromRgb(0x66, 0x66, 0x66),    // #666666
            Border = System.Windows.Media.Color.FromRgb(0xD0, 0xD0, 0xD0),           // #D0D0D0
            Accent = System.Windows.Media.Color.FromRgb(0x4C, 0xAF, 0x50),           // #4CAF50
            ButtonBackground = System.Windows.Media.Color.FromRgb(0xE5, 0xE5, 0xE5), // #E5E5E5
            ButtonHover = System.Windows.Media.Color.FromRgb(0xED, 0xED, 0xED)       // #EDEDED
        };
    }

    public class ThemeColors
    {
        public System.Windows.Media.Color WindowBackground { get; set; }
        public System.Windows.Media.Color PanelBackground { get; set; }
        public System.Windows.Media.Color InputBackground { get; set; }
        public System.Windows.Media.Color TextPrimary { get; set; }
        public System.Windows.Media.Color TextSecondary { get; set; }
        public System.Windows.Media.Color Border { get; set; }
        public System.Windows.Media.Color Accent { get; set; }
        public System.Windows.Media.Color ButtonBackground { get; set; }
        public System.Windows.Media.Color ButtonHover { get; set; }
    }
}

