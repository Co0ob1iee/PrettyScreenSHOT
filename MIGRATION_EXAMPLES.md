# Przykłady Kodu - Migracja WPF UI

Ten dokument zawiera praktyczne przykłady konwersji z obecnego systemu neumorficznego na WPF UI.

---

## Spis treści
1. [Konfiguracja projektu](#1-konfiguracja-projektu)
2. [Okna i dialogi](#2-okna-i-dialogi)
3. [Kontrolki formularzy](#3-kontrolki-formularzy)
4. [Powiadomienia](#4-powiadomienia)
5. [Motyw i stylowanie](#5-motyw-i-stylowanie)
6. [Nawigacja](#6-nawigacja)

---

## 1. Konfiguracja projektu

### 1.1 PrettyScreenSHOT.csproj

**Dodaj do pliku projektu:**

```xml
<ItemGroup>
  <!-- WPF UI -->
  <PackageReference Include="WPF-UI" Version="4.0.3" />
  <PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />

  <!-- Istniejące pakiety -->
  <PackageReference Include="Magick.NET.Core" Version="14.9.1" />
  <PackageReference Include="System.Drawing.Common" Version="10.0.0" />
  <PackageReference Include="Magick.NET-Q16-AnyCPU" Version="14.9.1" />
</ItemGroup>
```

### 1.2 App.xaml - KOMPLETNA WERSJA

**PRZED:**
```xml
<Application x:Class="PrettyScreenSHOT.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:local="clr-namespace:PrettyScreenSHOT">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="/PrettyScreenSHOT;component/NeumorphicStyles.xaml"/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

**PO:**
```xml
<Application x:Class="PrettyScreenSHOT.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml"
             xmlns:local="clr-namespace:PrettyScreenSHOT">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <!-- WPF UI Theme Resources -->
                <ui:ThemesDictionary Theme="Dark" />
                <ui:ControlsDictionary />

                <!-- Niestandardowe style (opcjonalne) -->
                <ResourceDictionary Source="pack://application:,,,/Themes/WpfUiCustom.xaml"/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

### 1.3 App.xaml.cs - KOMPLETNA WERSJA

**PRZED:**
```csharp
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PrettyScreenSHOT
{
    public partial class App : System.Windows.Application
    {
        private TrayIconManager? trayIconManager;
        private UpdateManager? updateManager;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize localization
            LocalizationHelper.Initialize();

            // Initialize cloud upload manager
            var cloudManager = CloudUploadManager.Instance;
            if (!string.IsNullOrWhiteSpace(SettingsManager.Instance.CloudProvider))
            {
                cloudManager.CurrentProvider = SettingsManager.Instance.CloudProvider;
            }

            // Initialize theme manager
            var themeName = SettingsManager.Instance.Theme;
            if (System.Enum.TryParse<Theme>(themeName, true, out var theme))
            {
                ThemeManager.Instance.SetTheme(theme);
            }

            // Initialize tray icon
            trayIconManager = new TrayIconManager();
            trayIconManager.Initialize();

            // Initialize auto-update
            InitializeAutoUpdate();

            DebugHelper.LogDebug("Application started - press PRTSCN for screenshot");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            trayIconManager?.Dispose();
            updateManager?.Dispose();
            ScreenshotHelper.RemoveKeyboardHook();
            base.OnExit(e);
        }
    }
}
```

**PO:**
```csharp
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Wpf.Ui;
using Wpf.Ui.Appearance;

namespace PrettyScreenSHOT
{
    public partial class App : System.Windows.Application
    {
        private TrayIconManager? trayIconManager;
        private UpdateManager? updateManager;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize localization
            LocalizationHelper.Initialize();

            // Initialize cloud upload manager
            var cloudManager = CloudUploadManager.Instance;
            if (!string.IsNullOrWhiteSpace(SettingsManager.Instance.CloudProvider))
            {
                cloudManager.CurrentProvider = SettingsManager.Instance.CloudProvider;
            }

            // WPF UI Theme Manager - Zastępuje ThemeManager.Instance
            InitializeWpfUiTheme();

            // Initialize tray icon
            trayIconManager = new TrayIconManager();
            trayIconManager.Initialize();

            // Initialize auto-update
            InitializeAutoUpdate();

            DebugHelper.LogDebug("Application started - press PRTSCN for screenshot");
        }

        private void InitializeWpfUiTheme()
        {
            var themeName = SettingsManager.Instance.Theme;

            ApplicationTheme appTheme = themeName?.ToLower() switch
            {
                "light" => ApplicationTheme.Light,
                "dark" => ApplicationTheme.Dark,
                "system" => ApplicationTheme.Unknown, // Auto detect
                _ => ApplicationTheme.Dark
            };

            // Zastosuj motyw globalnie
            if (appTheme == ApplicationTheme.Unknown)
            {
                ApplicationThemeManager.ApplySystemTheme();
            }
            else
            {
                ApplicationThemeManager.Apply(
                    appTheme,
                    WindowBackdropType.Mica,  // Windows 11 Mica effect
                    true  // updateAccents
                );
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            trayIconManager?.Dispose();
            updateManager?.Dispose();
            ScreenshotHelper.RemoveKeyboardHook();
            base.OnExit(e);
        }
    }
}
```

---

## 2. Okna i dialogi

### 2.1 Standardowe okno - SettingsWindow

**PRZED:**
```xml
<Window x:Class="PrettyScreenSHOT.SettingsWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Ustawienia" Height="650" Width="720"
        WindowStartupLocation="CenterScreen"
        Background="Transparent"
        Foreground="{StaticResource NeumorphicTextBrush}"
        ShowInTaskbar="False"
        WindowStyle="None"
        AllowsTransparency="True">

    <Window.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="/PrettyScreenSHOT;component/NeumorphicStyles.xaml"/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Window.Resources>

    <Border Style="{StaticResource NeumorphicPanel}" CornerRadius="18" Margin="15">
        <Grid>
            <Grid.RowDefinitions>
                <RowDefinition Height="48"/>
                <RowDefinition Height="*"/>
            </Grid.RowDefinitions>

            <!-- Custom Title Bar -->
            <Border Grid.Row="0" Background="Transparent"
                    Padding="15,10" CornerRadius="18,18,0,0"
                    MouseLeftButtonDown="OnTitleBarMouseDown">
                <DockPanel>
                    <TextBlock Text="Ustawienia" FontSize="20"
                               Style="{StaticResource NeumorphicHeading}"/>
                    <Button Click="CancelButton_Click"
                            Style="{StaticResource NeumorphicCircularButton}"
                            Width="32" Height="32" DockPanel.Dock="Right">
                        <TextBlock Text="✕" FontSize="16"/>
                    </Button>
                </DockPanel>
            </Border>

            <!-- Content -->
            <ScrollViewer Grid.Row="1">
                <!-- Settings controls -->
            </ScrollViewer>
        </Grid>
    </Border>
</Window>
```

**PO:**
```xml
<ui:FluentWindow x:Class="PrettyScreenSHOT.SettingsWindow"
                 xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                 xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                 xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml"
                 Title="Ustawienia"
                 Height="650"
                 Width="720"
                 WindowStartupLocation="CenterScreen"
                 ExtendsContentIntoTitleBar="True"
                 WindowBackdropType="Mica"
                 WindowCornerPreference="Round">

    <Grid Margin="0">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <!-- WPF UI Title Bar -->
        <ui:TitleBar Grid.Row="0"
                     Title="Ustawienia"
                     Icon="pack://application:,,,/app.ico"
                     UseSnapLayout="True"/>

        <!-- Content -->
        <ScrollViewer Grid.Row="1" Margin="20">
            <StackPanel Spacing="16">
                <!-- Settings controls -->
            </StackPanel>
        </ScrollViewer>
    </Grid>
</ui:FluentWindow>
```

**Code-behind zmiana:**
```csharp
// PRZED
public partial class SettingsWindow : Window

// PO
using Wpf.Ui.Controls;

public partial class SettingsWindow : FluentWindow
```

### 2.2 Dialog - SaveScreenshotDialog

**PRZED (Window):**
```csharp
var dialog = new SaveScreenshotDialog();
if (dialog.ShowDialog() == true)
{
    var filename = dialog.GetFilename();
}
```

**PO (ContentDialog):**

**XAML w głównym oknie:**
```xml
<ui:ContentDialog x:Name="SaveDialog"
                  Grid.Row="1"
                  Title="Zapisz zrzut ekranu"
                  PrimaryButtonText="Zapisz"
                  SecondaryButtonText="Anuluj"
                  CloseButtonText="Odrzuć"
                  DefaultButton="Primary"
                  DialogMaxWidth="500">
    <StackPanel Spacing="12">
        <ui:TextBox x:Name="FilenameTextBox"
                    PlaceholderText="screenshot.png"
                    Icon="Document24"/>

        <ComboBox x:Name="FormatComboBox" Header="Format">
            <ComboBoxItem Content="PNG" IsSelected="True"/>
            <ComboBoxItem Content="JPG"/>
            <ComboBoxItem Content="BMP"/>
        </ComboBox>

        <ui:NumberBox x:Name="QualityNumberBox"
                      Header="Jakość"
                      Minimum="10"
                      Maximum="100"
                      Value="90"
                      SpinButtonPlacementMode="Inline"/>
    </StackPanel>
</ui:ContentDialog>
```

**C# Code:**
```csharp
private async void OnSaveScreenshot()
{
    FilenameTextBox.Text = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";

    var result = await SaveDialog.ShowAsync();

    if (result == ContentDialogResult.Primary)
    {
        var filename = FilenameTextBox.Text;
        var format = (FormatComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
        var quality = (int)QualityNumberBox.Value;

        // Save screenshot logic
    }
}
```

---

## 3. Kontrolki formularzy

### 3.1 Przyciski

**PRZED:**
```xml
<!-- Przycisk standardowy -->
<Button Content="Browse..."
        Style="{StaticResource NeumorphicRaisedButton}"
        Click="BrowseButton_Click"/>

<!-- Przycisk główny (akcja) -->
<Button Content="Save"
        Style="{StaticResource NeumorphicDepressedButton}"
        Click="SaveButton_Click"/>

<!-- Przycisk okrągły (zamknij) -->
<Button Style="{StaticResource NeumorphicCircularButton}"
        Width="32" Height="32">
    <TextBlock Text="✕" FontSize="16"/>
</Button>
```

**PO:**
```xml
<!-- Przycisk standardowy -->
<ui:Button Content="Browse..."
           Icon="FolderOpen24"
           Click="BrowseButton_Click"/>

<!-- Przycisk główny (akcja) -->
<ui:Button Content="Save"
           Icon="Save24"
           Appearance="Primary"
           Click="SaveButton_Click"/>

<!-- Przycisk okrągły (zamknij) - niestandardowy styl -->
<ui:Button Icon="Dismiss24"
           Style="{StaticResource CircularIconButton}"
           Click="CloseButton_Click"/>
```

### 3.2 TextBox

**PRZED:**
```xml
<TextBox x:Name="SavePathTextBox"
         Style="{StaticResource NeumorphicTextBox}"
         Margin="0,0,10,0"/>
```

**PO:**
```xml
<!-- Podstawowy TextBox -->
<ui:TextBox x:Name="SavePathTextBox"
            Icon="Folder24"
            PlaceholderText="Wybierz ścieżkę..."/>

<!-- TextBox z przyciskiem -->
<ui:TextBox x:Name="SearchTextBox"
            Icon="Search24"
            PlaceholderText="Szukaj..."
            ClearButtonEnabled="True"
            TextChanged="OnSearchTextChanged"/>

<!-- MultiLine TextBox -->
<ui:TextBox x:Name="NotesTextBox"
            Height="100"
            TextWrapping="Wrap"
            AcceptsReturn="True"
            VerticalScrollBarVisibility="Auto"/>
```

### 3.3 ComboBox

**PRZED:**
```xml
<ComboBox x:Name="LanguageComboBox"
          Style="{StaticResource NeumorphicComboBox}"/>
```

**PO:**
```xml
<!-- Auto-styled przez WPF UI -->
<ComboBox x:Name="LanguageComboBox"
          Header="Language"
          PlaceholderText="Select language..."/>

<!-- Z ikoną (custom template) -->
<ComboBox x:Name="ThemeComboBox" Header="Motyw">
    <ComboBoxItem>
        <StackPanel Orientation="Horizontal">
            <ui:SymbolIcon Symbol="WeatherMoon24" Margin="0,0,8,0"/>
            <TextBlock Text="Ciemny"/>
        </StackPanel>
    </ComboBoxItem>
    <ComboBoxItem>
        <StackPanel Orientation="Horizontal">
            <ui:SymbolIcon Symbol="WeatherSunny24" Margin="0,0,8,0"/>
            <TextBlock Text="Jasny"/>
        </StackPanel>
    </ComboBoxItem>
</ComboBox>
```

### 3.4 Slider vs NumberBox

**PRZED:**
```xml
<Grid>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*"/>
        <ColumnDefinition Width="Auto"/>
    </Grid.ColumnDefinitions>
    <Slider x:Name="QualitySlider"
            Grid.Column="0"
            Minimum="10" Maximum="100" Value="90"
            Style="{StaticResource NeumorphicSlider}"
            ValueChanged="QualitySlider_ValueChanged"/>
    <TextBlock x:Name="QualityLabel"
               Grid.Column="1"
               Text="90%"
               MinWidth="50"
               Style="{StaticResource NeumorphicTextBlock}"/>
</Grid>
```

**PO (Opcja 1 - NumberBox):**
```xml
<ui:NumberBox x:Name="QualityNumberBox"
              Header="Jakość obrazu"
              Minimum="10"
              Maximum="100"
              Value="90"
              SpinButtonPlacementMode="Inline"
              ValueChanged="QualityNumberBox_ValueChanged"/>
```

**PO (Opcja 2 - Slider zachowany):**
```xml
<!-- Slider jest automatycznie stylowany przez WPF UI -->
<StackPanel>
    <TextBlock Text="Jakość obrazu" Margin="0,0,0,8"/>
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="*"/>
            <ColumnDefinition Width="60"/>
        </Grid.ColumnDefinitions>
        <Slider x:Name="QualitySlider"
                Grid.Column="0"
                Minimum="10" Maximum="100" Value="90"
                TickFrequency="10"
                IsSnapToTickEnabled="True"
                ValueChanged="QualitySlider_ValueChanged"/>
        <TextBlock Grid.Column="1"
                   Text="{Binding Value, ElementName=QualitySlider, StringFormat={}{0:F0}%}"
                   HorizontalAlignment="Right"
                   VerticalAlignment="Center"/>
    </Grid>
</StackPanel>
```

### 3.5 CheckBox

**PRZED:**
```xml
<CheckBox x:Name="AutoSaveCheckBox"
          Content="Auto Save"
          Style="{StaticResource NeumorphicCheckBox}"/>
```

**PO:**
```xml
<!-- Auto-styled -->
<CheckBox x:Name="AutoSaveCheckBox"
          Content="Auto Save"/>

<!-- Z ikoną -->
<StackPanel Orientation="Horizontal">
    <CheckBox x:Name="AutoUploadCheckBox"
              Content="Automatyczne wgrywanie"
              VerticalAlignment="Center"/>
    <ui:InfoBadge x:Name="UploadBadge"
                  Severity="Success"
                  Value="3"
                  Margin="8,0,0,0"
                  Visibility="Collapsed"/>
</StackPanel>
```

### 3.6 RadioButton

**PRZED:**
```xml
<RadioButton x:Name="AlignLeftRadio"
             Content="◀ Lewo"
             GroupName="Alignment"
             IsChecked="True"
             Style="{StaticResource NeumorphicRadioButton}"/>
```

**PO:**
```xml
<!-- Auto-styled -->
<RadioButton x:Name="AlignLeftRadio"
             Content="Wyrównaj do lewej"
             GroupName="Alignment"
             IsChecked="True"/>

<!-- Segmented Control (lepsze dla 2-3 opcji) -->
<ui:CardControl>
    <StackPanel>
        <TextBlock Text="Wyrównanie tekstu" Margin="0,0,0,8"/>
        <StackPanel Orientation="Horizontal">
            <RadioButton Content="Lewo" GroupName="Align" IsChecked="True" Margin="0,0,12,0"/>
            <RadioButton Content="Środek" GroupName="Align" Margin="0,0,12,0"/>
            <RadioButton Content="Prawo" GroupName="Align"/>
        </StackPanel>
    </StackPanel>
</ui:CardControl>
```

---

## 4. Powiadomienia

### 4.1 MessageBox → Snackbar

**PRZED:**
```csharp
MessageBox.Show(
    "Ustawienia zostały zapisane pomyślnie",
    "Sukces",
    MessageBoxButton.OK,
    MessageBoxImage.Information
);
```

**PO:**

**XAML:**
```xml
<ui:Snackbar x:Name="SettingsSnackbar"
             Grid.Row="1"
             Timeout="4000"
             CloseButtonEnabled="True"/>
```

**C#:**
```csharp
SettingsSnackbar.Show(
    "Sukces",
    "Ustawienia zostały zapisane pomyślnie",
    ControlAppearance.Success,
    new SymbolIcon(SymbolRegular.CheckmarkCircle24),
    TimeSpan.FromSeconds(4)
);
```

**Różne typy powiadomień:**
```csharp
// Success
SettingsSnackbar.Show("Sukces", message, ControlAppearance.Success);

// Info
SettingsSnackbar.Show("Informacja", message, ControlAppearance.Info);

// Warning
SettingsSnackbar.Show("Uwaga", message, ControlAppearance.Caution);

// Error
SettingsSnackbar.Show("Błąd", message, ControlAppearance.Danger);
```

### 4.2 MessageBox (Confirm) → ContentDialog

**PRZED:**
```csharp
var result = MessageBox.Show(
    "Czy na pewno chcesz zresetować ustawienia?",
    "Potwierdzenie",
    MessageBoxButton.YesNo,
    MessageBoxImage.Question
);

if (result == MessageBoxResult.Yes)
{
    // Reset settings
}
```

**PO:**

**XAML:**
```xml
<ui:ContentDialog x:Name="ConfirmDialog"
                  Grid.Row="1"
                  Title="Potwierdzenie"
                  PrimaryButtonText="Tak"
                  SecondaryButtonText="Nie"
                  DefaultButton="Secondary">
    <TextBlock TextWrapping="Wrap"
               Text="Czy na pewno chcesz zresetować ustawienia?"/>
</ui:ContentDialog>
```

**C#:**
```csharp
private async void ResetButton_Click(object sender, RoutedEventArgs e)
{
    var result = await ConfirmDialog.ShowAsync();

    if (result == ContentDialogResult.Primary)
    {
        // Reset settings
        settingsManager.ResetToDefaults();
    }
}
```

### 4.3 InfoBar dla stałych komunikatów

```xml
<!-- W górnej części okna -->
<ui:InfoBar x:Name="UpdateInfoBar"
            Title="Dostępna aktualizacja"
            Message="Wersja 1.2.0 jest dostępna"
            Severity="Informational"
            IsOpen="{Binding IsUpdateAvailable}"
            IsClosable="True"
            Margin="0,0,0,12">
    <ui:InfoBar.ActionButton>
        <ui:Button Content="Aktualizuj"
                   Icon="ArrowDownload24"
                   Click="OnUpdateClick"/>
    </ui:InfoBar.ActionButton>
</ui:InfoBar>
```

---

## 5. Motyw i stylowanie

### 5.1 ThemeService.cs - Nowy serwis

```csharp
using Wpf.Ui.Appearance;

namespace PrettyScreenSHOT.Services
{
    public class ThemeService
    {
        private static ThemeService? _instance;
        public static ThemeService Instance => _instance ??= new ThemeService();

        private ThemeService() { }

        public void SetTheme(string themeName)
        {
            ApplicationTheme appTheme = themeName.ToLower() switch
            {
                "light" => ApplicationTheme.Light,
                "dark" => ApplicationTheme.Dark,
                "system" => ApplicationTheme.Unknown,
                _ => ApplicationTheme.Dark
            };

            ApplyTheme(appTheme);
        }

        public void SetTheme(Theme theme)
        {
            ApplicationTheme appTheme = theme switch
            {
                Theme.Light => ApplicationTheme.Light,
                Theme.Dark => ApplicationTheme.Dark,
                _ => ApplicationTheme.Dark
            };

            ApplyTheme(appTheme);
        }

        public void ApplySystemTheme()
        {
            ApplicationThemeManager.ApplySystemTheme();
        }

        private void ApplyTheme(ApplicationTheme theme)
        {
            if (theme == ApplicationTheme.Unknown)
            {
                ApplySystemTheme();
            }
            else
            {
                ApplicationThemeManager.Apply(
                    theme,
                    WindowBackdropType.Mica,
                    true // updateAccents
                );
            }
        }

        public string GetCurrentTheme()
        {
            var current = ApplicationThemeManager.GetAppTheme();
            return current switch
            {
                ApplicationTheme.Light => "Light",
                ApplicationTheme.Dark => "Dark",
                _ => "System"
            };
        }
    }
}
```

### 5.2 Themes/WpfUiCustom.xaml - Niestandardowe style

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml">

    <!-- Circular Icon Button (zamiennik NeumorphicCircularButton) -->
    <Style x:Key="CircularIconButton" TargetType="ui:Button" BasedOn="{StaticResource {x:Type ui:Button}}">
        <Setter Property="Width" Value="40"/>
        <Setter Property="Height" Value="40"/>
        <Setter Property="CornerRadius" Value="20"/>
        <Setter Property="Padding" Value="0"/>
        <Setter Property="FontSize" Value="16"/>
    </Style>

    <!-- Card Header Style -->
    <Style x:Key="CardHeaderText" TargetType="TextBlock">
        <Setter Property="FontSize" Value="18"/>
        <Setter Property="FontWeight" Value="SemiBold"/>
        <Setter Property="Margin" Value="0,0,0,8"/>
    </Style>

    <!-- Subtelny Panel (zamiennik NeumorphicPanel) -->
    <Style x:Key="SubtleCard" TargetType="ui:Card" BasedOn="{StaticResource {x:Type ui:Card}}">
        <Setter Property="Padding" Value="16"/>
        <Setter Property="Margin" Value="0,0,0,12"/>
    </Style>

    <!-- Spacing helper -->
    <Style x:Key="SectionSpacing" TargetType="StackPanel">
        <Setter Property="Spacing" Value="12"/>
    </Style>

    <!-- Niestandardowe kolory (opcjonalne) -->
    <SolidColorBrush x:Key="CustomAccentBrush" Color="#007ACC"/>

</ResourceDictionary>
```

### 5.3 Użycie niestandardowych stylów

```xml
<ui:Card Style="{StaticResource SubtleCard}">
    <StackPanel Style="{StaticResource SectionSpacing}">
        <TextBlock Text="Ustawienia podstawowe"
                   Style="{StaticResource CardHeaderText}"/>

        <ui:TextBox Header="Nazwa użytkownika"/>
        <ComboBox Header="Język"/>
    </StackPanel>
</ui:Card>

<ui:Button Icon="Dismiss24"
           Style="{StaticResource CircularIconButton}"
           Click="OnCloseClick"/>
```

---

## 6. Nawigacja

### 6.1 MainWindow z NavigationView (opcjonalne)

```xml
<ui:FluentWindow x:Class="PrettyScreenSHOT.MainWindow"
                 xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml"
                 xmlns:pages="clr-namespace:PrettyScreenSHOT.Views.Pages"
                 Title="PrettyScreenSHOT"
                 Width="1200" Height="800"
                 ExtendsContentIntoTitleBar="True"
                 WindowBackdropType="Mica">

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <!-- Title Bar -->
        <ui:TitleBar Grid.Row="0"
                     Title="PrettyScreenSHOT"
                     Icon="pack://application:,,,/app.ico"
                     UseSnapLayout="True"/>

        <!-- Navigation -->
        <ui:NavigationView Grid.Row="1"
                          x:Name="NavView"
                          IsBackButtonVisible="Collapsed"
                          IsPaneToggleVisible="True"
                          OpenPaneLength="220"
                          IsBackEnabled="False"
                          Loaded="NavView_Loaded">

            <ui:NavigationView.MenuItems>
                <ui:NavigationViewItem Content="Historia"
                                      Icon="Image24"
                                      TargetPageType="{x:Type pages:HistoryPage}"/>

                <ui:NavigationViewItem Content="Ustawienia"
                                      Icon="Settings24"
                                      TargetPageType="{x:Type pages:SettingsPage}"/>
            </ui:NavigationView.MenuItems>

            <ui:NavigationView.FooterMenuItems>
                <ui:NavigationViewItem Content="O aplikacji"
                                      Icon="Info24"
                                      Click="OnAboutClick"/>
            </ui:NavigationView.FooterMenuItems>

            <!-- Content Frame -->
            <ui:NavigationView.ContentOverlay>
                <Grid>
                    <Frame x:Name="ContentFrame"/>
                </Grid>
            </ui:NavigationView.ContentOverlay>

        </ui:NavigationView>
    </Grid>
</ui:FluentWindow>
```

**Code-behind:**
```csharp
using Wpf.Ui.Controls;

public partial class MainWindow : FluentWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void NavView_Loaded(object sender, RoutedEventArgs e)
    {
        // Nawiguj do pierwszej strony
        NavView.Navigate(typeof(HistoryPage));
    }

    private void OnAboutClick(object sender, RoutedEventArgs e)
    {
        // Pokaż dialog "O aplikacji"
    }
}
```

### 6.2 Page przykład - HistoryPage

```xml
<Page x:Class="PrettyScreenSHOT.Views.Pages.HistoryPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml"
      Title="Historia">

    <Grid Margin="20">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <!-- Search bar -->
        <ui:TextBox Grid.Row="0"
                    Icon="Search24"
                    PlaceholderText="Szukaj zrzutów..."
                    ClearButtonEnabled="True"
                    Margin="0,0,0,16"/>

        <!-- Screenshots list -->
        <ScrollViewer Grid.Row="1">
            <ItemsControl ItemsSource="{Binding Screenshots}">
                <ItemsControl.ItemTemplate>
                    <DataTemplate>
                        <ui:Card Margin="0,0,0,12" Padding="16">
                            <!-- Screenshot item -->
                        </ui:Card>
                    </DataTemplate>
                </ItemsControl.ItemTemplate>
            </ItemsControl>
        </ScrollViewer>
    </Grid>
</Page>
```

---

## 7. Kompletne przykłady konwersji

### 7.1 ScreenshotHistoryWindow - Item Template

**PRZED:**
```xml
<Border Style="{StaticResource NeumorphicRaisedBorder}" Margin="0,0,0,10" Padding="15">
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="Auto"/>
            <ColumnDefinition Width="*"/>
            <ColumnDefinition Width="Auto"/>
        </Grid.ColumnDefinitions>

        <!-- Thumbnail -->
        <Border Grid.Column="0" Width="100" Height="75" CornerRadius="8"
                Margin="0,0,15,0">
            <Border.Background>
                <ImageBrush ImageSource="{Binding Thumbnail}"/>
            </Border.Background>
        </Border>

        <!-- Info -->
        <StackPanel Grid.Column="1" VerticalAlignment="Center">
            <TextBlock Text="{Binding Filename}"
                       Style="{StaticResource NeumorphicTextBlock}"/>
            <TextBlock Text="{Binding Timestamp}"
                       Style="{StaticResource NeumorphicTextBlockSecondary}"/>
        </StackPanel>

        <!-- Actions -->
        <StackPanel Grid.Column="2" Orientation="Horizontal">
            <Button Click="OnDeleteClick"
                    Style="{StaticResource NeumorphicCircularButton}">
                <TextBlock Text="🗑" FontSize="16"/>
            </Button>
        </StackPanel>
    </Grid>
</Border>
```

**PO:**
```xml
<ui:Card Margin="0,0,0,12" Padding="16">
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="Auto"/>
            <ColumnDefinition Width="*"/>
            <ColumnDefinition Width="Auto"/>
        </Grid.ColumnDefinitions>

        <!-- Thumbnail -->
        <Border Grid.Column="0"
                Width="100" Height="75"
                CornerRadius="8"
                Margin="0,0,16,0"
                ClipToBounds="True">
            <Image Source="{Binding Thumbnail}"
                   Stretch="UniformToFill"/>
        </Border>

        <!-- Info -->
        <StackPanel Grid.Column="1"
                    VerticalAlignment="Center"
                    Spacing="4">
            <TextBlock Text="{Binding Filename}"
                       FontWeight="SemiBold"/>
            <TextBlock Text="{Binding Timestamp, StringFormat='dd.MM.yyyy HH:mm'}"
                       Foreground="{DynamicResource TextFillColorSecondaryBrush}"
                       FontSize="12"/>

            <!-- Cloud info -->
            <StackPanel Orientation="Horizontal"
                        Spacing="6"
                        Visibility="{Binding CloudUrl, Converter={StaticResource NullToVisibilityConverter}}">
                <ui:SymbolIcon Symbol="Cloud24" FontSize="12"/>
                <TextBlock Text="{Binding CloudProvider}"
                           FontSize="12"/>
            </StackPanel>
        </StackPanel>

        <!-- Actions -->
        <StackPanel Grid.Column="2"
                    Orientation="Horizontal"
                    Spacing="8"
                    VerticalAlignment="Center">
            <ui:Button Icon="Share24"
                       Appearance="Secondary"
                       Click="OnShareClick"
                       ToolTip="Udostępnij"/>

            <ui:Button Icon="Delete24"
                       Appearance="Danger"
                       Click="OnDeleteClick"
                       ToolTip="Usuń"/>
        </StackPanel>
    </Grid>
</ui:Card>
```

---

## 8. Helpers i Utilities

### 8.1 Konwertery (zachowane)

```csharp
// NullToVisibilityConverter - działa bez zmian
public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isInverse = parameter?.ToString() == "Inverse";
        bool isNull = value == null || string.IsNullOrWhiteSpace(value.ToString());

        if (isInverse)
            return isNull ? Visibility.Visible : Visibility.Collapsed;
        else
            return isNull ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
```

### 8.2 StackPanel Spacing

WPF UI dodaje property `Spacing` do StackPanel:

```xml
<!-- PRZED - marginesy ręczne -->
<StackPanel>
    <TextBlock Text="Item 1" Margin="0,0,0,10"/>
    <TextBlock Text="Item 2" Margin="0,0,0,10"/>
    <TextBlock Text="Item 3"/>
</StackPanel>

<!-- PO - automatyczny spacing -->
<StackPanel Spacing="10">
    <TextBlock Text="Item 1"/>
    <TextBlock Text="Item 2"/>
    <TextBlock Text="Item 3"/>
</StackPanel>
```

---

## 9. Checklist migracji pojedynczego okna

Dla każdego okna:

- [ ] Zamień `<Window>` na `<ui:FluentWindow>`
- [ ] Dodaj `xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml"`
- [ ] Zamień custom title bar na `<ui:TitleBar>`
- [ ] Usuń `WindowStyle="None"` i `AllowsTransparency="True"`
- [ ] Dodaj `ExtendsContentIntoTitleBar="True"` i `WindowBackdropType="Mica"`
- [ ] Zamień `NeumorphicRaisedButton` → `<ui:Button>`
- [ ] Zamień `NeumorphicDepressedButton` → `<ui:Button Appearance="Primary">`
- [ ] Zamień `NeumorphicTextBox` → `<ui:TextBox>`
- [ ] Zamień `NeumorphicPanel` → `<ui:Card>`
- [ ] Usuń wszystkie `Style="{StaticResource ...}"` dla standardowych kontrolek
- [ ] Zamień `MessageBox` na `Snackbar` lub `ContentDialog`
- [ ] Dodaj `Spacing` do `StackPanel` gdzie można
- [ ] Testuj renderowanie i funkcjonalność
- [ ] Code review

---

**Koniec dokumentu**

Dla pytań i wsparcia: https://github.com/lepoco/wpfui
