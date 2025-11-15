# Plan Migracji PrettyScreenSHOT do WPF UI

**Data utworzenia:** 2025-01-15
**Autor:** Claude AI
**Wersja:** 1.0
**Status:** Projekt wstępny

---

## 1. Streszczenie wykonawcze

### 1.1 Cel migracji
Modernizacja aplikacji PrettyScreenSHOT poprzez zastąpienie niestandardowego systemu stylów neumorficznych biblioteką **WPF UI 4.0.3**, która oferuje:
- Nowoczesny Fluent Design zgodny z Windows 11
- Natywne wsparcie dla motywów jasnego/ciemnego
- Zaawansowane kontrolki (Navigation, Snackbar, Dialog, Card, NumberBox)
- Lepszą wydajność i mniejsze koszty utrzymania
- Spójność z ekosystemem Windows

### 1.2 Korzyści biznesowe
- ✅ Zmniejszenie kodu niestandardowego o ~40%
- ✅ Nowoczesny wygląd zgodny z Windows 11
- ✅ Lepsze wsparcie dla motywów i dostępności
- ✅ Łatwiejsze dodawanie nowych funkcji
- ✅ Redukcja długu technicznego

### 1.3 Oś czasu
- **Faza 1 (Sprint 1-2):** Konfiguracja i infrastruktura - 2 tygodnie
- **Faza 2 (Sprint 3-4):** Migracja głównych okien - 2 tygodnie
- **Faza 3 (Sprint 5-6):** Migracja dialógów i komponentów - 2 tygodnie
- **Faza 4 (Sprint 7):** Testy i optymalizacja - 1 tydzień
- **Całkowity czas:** 7 tygodni

---

## 2. Analiza obecnego stanu

### 2.1 Obecna architektura

#### Struktura plików XAML
```
PrettyScreenSHOT/
├── App.xaml                        # Główna aplikacja
├── NeumorphicStyles.xaml           # Niestandardowe style (643 linie)
├── ScreenshotOverlay.xaml          # Overlay wyboru obszaru
├── ScreenshotEditorWindow.xaml     # Edytor zrzutów
├── ScreenshotHistoryWindow.xaml    # Historia zrzutów
├── SettingsWindow.xaml             # Ustawienia
├── TextInputWindow.xaml            # Dialog dodawania tekstu
├── SaveScreenshotDialog.xaml       # Dialog zapisu
├── UpdateNotificationWindow.xaml   # Powiadomienia o aktualizacjach
├── UpdateProgressWindow.xaml       # Progress aktualizacji
├── UpdateWindow.xaml               # Główne okno aktualizacji
└── VideoCaptureWindow.xaml         # Nagrywanie wideo
```

#### Niestandardowe komponenty w NeumorphicStyles.xaml
| Komponent | Linie kodu | Złożoność | Priorytet migracji |
|-----------|-----------|-----------|-------------------|
| NeumorphicRaisedButton | 48 | Wysoka | P1 |
| NeumorphicDepressedButton | 25 | Średnia | P1 |
| NeumorphicCircularButton | 46 | Wysoka | P1 |
| NeumorphicTextBox | 37 | Średnia | P1 |
| NeumorphicComboBox | 88 | Bardzo wysoka | P2 |
| NeumorphicCheckBox | 56 | Wysoka | P2 |
| NeumorphicRadioButton | 41 | Średnia | P2 |
| NeumorphicSlider | 65 | Wysoka | P2 |
| NeumorphicScrollBar | 58 | Wysoka | P3 |
| NeumorphicPanel | 8 | Niska | P1 |

#### Niestandardowe managery
```csharp
ThemeManager.cs              // System motywów (Dark/Light)
LocalizationHelper.cs        // Lokalizacja (en, pl, de, zh, fr)
SettingsManager.cs           // Zarządzanie ustawieniami
```

### 2.2 Statystyki kodu
- **Całkowity kod XAML:** ~3,200 linii
- **Kod stylów niestandardowych:** ~643 linie (20%)
- **Liczba okien:** 11
- **Liczba niestandardowych stylów:** 17
- **Pakiety NuGet:** 2 (Magick.NET)

---

## 3. Architektura docelowa

### 3.1 WPF UI - Przegląd

**WPF UI 4.0.3** (lepo.co)
- **GitHub:** https://github.com/lepoco/wpfui
- **NuGet:** WPF-UI 4.0.3
- **Licencja:** MIT (open source)
- **Ostatnia aktualizacja:** Czerwiec 2025
- **Community:** 8.9k stars, 2500+ projektów

### 3.2 Nowa struktura plików

```
PrettyScreenSHOT/
├── App.xaml                        # Zmodyfikowany: integracja WPF UI
├── Themes/
│   └── WpfUiCustom.xaml           # Niestandardowe nadpisania WPF UI
├── Views/
│   ├── MainWindow.xaml            # NOWY: Główne okno z Navigation
│   ├── ScreenshotOverlay.xaml     # Zmodyfikowany
│   ├── ScreenshotEditorWindow.xaml
│   ├── ScreenshotHistoryWindow.xaml
│   ├── SettingsWindow.xaml
│   ├── TextInputWindow.xaml
│   ├── SaveScreenshotDialog.xaml
│   ├── UpdateNotificationWindow.xaml
│   ├── UpdateProgressWindow.xaml
│   ├── UpdateWindow.xaml
│   └── VideoCaptureWindow.xaml
├── ViewModels/                     # NOWY: MVVM pattern
│   ├── MainViewModel.cs
│   ├── SettingsViewModel.cs
│   └── HistoryViewModel.cs
└── Services/
    ├── ThemeService.cs             # Zastępuje ThemeManager
    └── NavigationService.cs        # NOWY: Nawigacja
```

### 3.3 Nowe pakiety NuGet

```xml
<PackageReference Include="WPF-UI" Version="4.0.3" />
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />
<PackageReference Include="Magick.NET.Core" Version="14.9.1" />
<PackageReference Include="System.Drawing.Common" Version="10.0.0" />
<PackageReference Include="Magick.NET-Q16-AnyCPU" Version="14.9.1" />
```

---

## 4. Mapowanie komponentów

### 4.1 Kontrolki podstawowe

| Obecna kontrolka | WPF UI Zamiennik | Uwagi |
|------------------|------------------|-------|
| NeumorphicRaisedButton | `<ui:Button>` | Fluent design, ikony |
| NeumorphicDepressedButton | `<ui:Button Appearance="Primary">` | Przycisk akcji |
| NeumorphicCircularButton | `<ui:Button Style="{StaticResource CircularButton}">` | Niestandardowy styl |
| NeumorphicTextBox | `<ui:TextBox>` | Wsparcie dla ikon i placeholder |
| NeumorphicComboBox | `<ComboBox>` z motywem WPF UI | Automatyczne stylowanie |
| NeumorphicCheckBox | `<CheckBox>` z motywem WPF UI | Automatyczne stylowanie |
| NeumorphicRadioButton | `<RadioButton>` z motywem WPF UI | Automatyczne stylowanie |
| NeumorphicSlider | `<Slider>` z motywem WPF UI | Automatyczne stylowanie |
| NeumorphicScrollBar | ScrollBar z motywem WPF UI | Automatyczne stylowanie |
| NeumorphicPanel | `<ui:Card>` | Nowoczesny kontener |

### 4.2 Nowe komponenty WPF UI

| Komponent | Zastosowanie w projekcie |
|-----------|--------------------------|
| `<ui:NavigationView>` | Główne okno nawigacji (opcjonalne) |
| `<ui:Snackbar>` | Powiadomienia (zastąpi MessageBox) |
| `<ui:ContentDialog>` | Dialogi (SaveScreenshotDialog, konfirmacje) |
| `<ui:Card>` | Kontenery w ScreenshotHistoryWindow |
| `<ui:NumberBox>` | Edytor liczb (QualitySlider alternatywa) |
| `<ui:TitleBar>` | Niestandardowy title bar z Windows 11 Snap |
| `<ui:InfoBar>` | Komunikaty informacyjne |
| `<ui:ProgressRing>` | Loading indicators |

### 4.3 System motywów

#### Obecny system (ThemeManager.cs)
```csharp
public enum Theme { Dark, Light }
ThemeManager.Instance.SetTheme(Theme.Dark);
ThemeManager.Instance.ApplyTheme(window);
```

#### Nowy system (WPF UI)
```csharp
// W App.xaml.cs
using Wpf.Ui.Appearance;

ApplicationThemeManager.Apply(
    ApplicationTheme.Dark,  // lub Light
    WindowBackdropType.Mica // Windows 11 efekt
);

// Automatyczne przełączanie
ApplicationThemeManager.ApplySystemTheme();
```

---

## 5. Plan migracji krok po kroku

### FAZA 1: Konfiguracja i infrastruktura (Sprint 1-2)

#### Sprint 1: Instalacja i konfiguracja podstawowa

**Zadanie 1.1: Dodanie pakietów NuGet**
```bash
dotnet add package WPF-UI --version 4.0.3
dotnet add package CommunityToolkit.Mvvm --version 8.2.2
```

**Zadanie 1.2: Aktualizacja App.xaml**
```xml
<Application x:Class="PrettyScreenSHOT.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml"
             xmlns:local="clr-namespace:PrettyScreenSHOT">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <!-- WPF UI Theme -->
                <ui:ThemesDictionary Theme="Dark" />
                <ui:ControlsDictionary />

                <!-- Niestandardowe nadpisania -->
                <ResourceDictionary Source="/Themes/WpfUiCustom.xaml"/>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

**Zadanie 1.3: Aktualizacja App.xaml.cs**
```csharp
using Wpf.Ui.Appearance;

protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);

    // WPF UI Theme Manager
    ApplicationThemeManager.Apply(
        ApplicationTheme.Dark,
        WindowBackdropType.Mica
    );

    // Reszta inicjalizacji...
}
```

**Zadanie 1.4: Utworzenie WpfUiCustom.xaml**
- Niestandardowe kolory marki (jeśli potrzebne)
- Nadpisania dla specyficznych kontrolek
- Dodatkowe style (np. CircularButton)

#### Sprint 2: Migracja ThemeManager

**Zadanie 2.1: Utworzenie ThemeService.cs**
```csharp
using Wpf.Ui.Appearance;

namespace PrettyScreenSHOT.Services
{
    public class ThemeService
    {
        private static ThemeService? _instance;
        public static ThemeService Instance => _instance ??= new ThemeService();

        public void SetTheme(Theme theme)
        {
            var appTheme = theme == Theme.Dark
                ? ApplicationTheme.Dark
                : ApplicationTheme.Light;

            ApplicationThemeManager.Apply(appTheme);
        }

        public void ApplySystemTheme()
        {
            ApplicationThemeManager.ApplySystemTheme();
        }
    }
}
```

**Zadanie 2.2: Migracja wywołań**
- Zamień `ThemeManager.Instance` na `ThemeService.Instance`
- Usuń `ApplyTheme(window)` - WPF UI robi to automatycznie
- Testy działania

---

### FAZA 2: Migracja głównych okien (Sprint 3-4)

#### Sprint 3: SettingsWindow.xaml

**Priorytet:** WYSOKI - Najczęściej używane okno

**Zadanie 3.1: Aktualizacja struktury okna**

**PRZED (obecne):**
```xml
<Window x:Class="PrettyScreenSHOT.SettingsWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="{Binding Title}" Height="650" Width="720"
        Background="Transparent" Foreground="{StaticResource NeumorphicTextBrush}"
        WindowStyle="None" AllowsTransparency="True">

    <Border Style="{StaticResource NeumorphicPanel}" CornerRadius="18" Margin="15">
        <!-- Content -->
    </Border>
</Window>
```

**PO (WPF UI):**
```xml
<ui:FluentWindow x:Class="PrettyScreenSHOT.SettingsWindow"
                 xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                 xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                 xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml"
                 Title="Ustawienia" Height="650" Width="720"
                 ExtendsContentIntoTitleBar="True"
                 WindowBackdropType="Mica"
                 WindowCornerPreference="Round">

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <!-- Title Bar -->
        <ui:TitleBar Grid.Row="0" Title="Ustawienia" />

        <!-- Content -->
        <ScrollViewer Grid.Row="1" Margin="20">
            <!-- Settings controls -->
        </ScrollViewer>
    </Grid>
</ui:FluentWindow>
```

**Zadanie 3.2: Migracja kontrolek**

| Kontrolka | Przed | Po |
|-----------|-------|-----|
| Language ComboBox | `<ComboBox Style="{StaticResource NeumorphicComboBox}">` | `<ComboBox>` (auto-styled) |
| Save Path TextBox | `<TextBox Style="{StaticResource NeumorphicTextBox}">` | `<ui:TextBox Icon="Folder24">` |
| Browse Button | `<Button Style="{StaticResource NeumorphicRaisedButton}">` | `<ui:Button Icon="FolderOpen24">` |
| Quality Slider | `<Slider Style="{StaticResource NeumorphicSlider}">` | `<Slider>` lub `<ui:NumberBox>` |
| CheckBox | `<CheckBox Style="{StaticResource NeumorphicCheckBox}">` | `<CheckBox>` (auto-styled) |
| Save Button | `<Button Style="{StaticResource NeumorphicDepressedButton}">` | `<ui:Button Appearance="Primary">` |

**Zadanie 3.3: Zamiana MessageBox na Snackbar**

**PRZED:**
```csharp
MessageBox.Show(
    LocalizationHelper.GetString("Settings_SaveSuccessMessage"),
    LocalizationHelper.GetString("Settings_SaveSuccess"),
    MessageBoxButton.OK,
    MessageBoxImage.Information
);
```

**PO:**
```csharp
// W XAML dodaj:
<ui:Snackbar x:Name="SettingsSnackbar"
             Grid.Row="1"
             Timeout="3000" />

// W code-behind:
SettingsSnackbar.Show(
    LocalizationHelper.GetString("Settings_SaveSuccess"),
    LocalizationHelper.GetString("Settings_SaveSuccessMessage"),
    ControlAppearance.Success
);
```

#### Sprint 4: ScreenshotHistoryWindow.xaml

**Zadanie 4.1: Migracja na ui:Card**

**PRZED:**
```xml
<Border Style="{StaticResource NeumorphicRaisedBorder}" Margin="0,0,0,10" Padding="15">
    <Grid>
        <!-- Screenshot info -->
    </Grid>
</Border>
```

**PO:**
```xml
<ui:Card Margin="0,0,0,10" Padding="15">
    <Grid>
        <!-- Screenshot info -->
    </Grid>
</ui:Card>
```

**Zadanie 4.2: Search box z ikoną**
```xml
<ui:TextBox x:Name="SearchTextBox"
            Icon="Search24"
            PlaceholderText="Szukaj zrzutów..."
            TextChanged="OnSearchTextChanged"/>
```

---

### FAZA 3: Migracja dialógów i komponentów (Sprint 5-6)

#### Sprint 5: TextInputWindow.xaml → ContentDialog

**Zadanie 5.1: Konwersja na ContentDialog**

**PRZED (Window):**
```csharp
var dialog = new TextInputWindow();
if (dialog.ShowDialog() == true)
{
    var result = dialog.GetTextInput();
}
```

**PO (ContentDialog):**
```xml
<ui:ContentDialog x:Name="TextInputDialog"
                  Title="Dodaj tekst"
                  PrimaryButtonText="OK"
                  SecondaryButtonText="Anuluj"
                  DefaultButton="Primary">
    <StackPanel Spacing="12">
        <ui:TextBox x:Name="TextInput"
                    PlaceholderText="Wpisz tekst..."
                    Height="80"
                    TextWrapping="Wrap"/>

        <ComboBox x:Name="FontFamily" Header="Czcionka"/>

        <ui:NumberBox x:Name="FontSize"
                      Header="Rozmiar"
                      Minimum="8"
                      Maximum="120"
                      Value="24"/>

        <!-- Inne kontrolki -->
    </StackPanel>
</ui:ContentDialog>
```

```csharp
var result = await TextInputDialog.ShowAsync();
if (result == ContentDialogResult.Primary)
{
    var text = TextInput.Text;
}
```

#### Sprint 6: UpdateWindow i powiadomienia

**Zadanie 6.1: UpdateNotificationWindow → InfoBar**
```xml
<ui:InfoBar x:Name="UpdateInfoBar"
            Title="Dostępna aktualizacja"
            Message="Wersja 1.2.0 jest dostępna do pobrania"
            Severity="Informational"
            IsOpen="True"
            IsClosable="True">
    <ui:InfoBar.ActionButton>
        <ui:Button Content="Aktualizuj" Click="OnUpdateClick"/>
    </ui:InfoBar.ActionButton>
</ui:InfoBar>
```

**Zadanie 6.2: UpdateProgressWindow → ProgressRing**
```xml
<ui:Card>
    <StackPanel Spacing="12" Padding="20">
        <TextBlock Text="Pobieranie aktualizacji..."
                   Style="{StaticResource SubtitleTextBlockStyle}"/>

        <ProgressBar Value="{Binding DownloadProgress}"
                     Maximum="100"
                     Height="8"/>

        <TextBlock Text="{Binding ProgressText}"
                   HorizontalAlignment="Center"/>
    </StackPanel>
</ui:Card>
```

---

### FAZA 4: Testy i optymalizacja (Sprint 7)

#### Zadanie 7.1: Testy funkcjonalne
- [ ] Wszystkie okna renderują się poprawnie
- [ ] Przełączanie motywów działa (Dark/Light)
- [ ] Wszystkie przyciski są klikalne
- [ ] Dialogi otwierają się i zamykają
- [ ] Formularze zapisują dane
- [ ] Lokalizacja działa we wszystkich językach

#### Zadanie 7.2: Testy wydajności
- [ ] Czas uruchomienia aplikacji
- [ ] Zużycie pamięci (przed/po)
- [ ] Płynność animacji
- [ ] Responsywność UI

#### Zadanie 7.3: Testy wizualne
- [ ] Spójność kolorów
- [ ] Wyrównanie elementów
- [ ] Spacing i margins
- [ ] Dark mode kontrast
- [ ] Accessibility (narrator, keyboard)

#### Zadanie 7.4: Cleanup
- [ ] Usunięcie NeumorphicStyles.xaml
- [ ] Usunięcie ThemeManager.cs
- [ ] Aktualizacja dokumentacji
- [ ] Przegląd TODO w kodzie

---

## 6. Zarządzanie ryzykiem

### 6.1 Ryzyka techniczne

| Ryzyko | Prawdopodobieństwo | Wpływ | Mitygacja |
|--------|-------------------|-------|-----------|
| Niezgodność stylów z brand identity | Średnie | Wysoki | Utworzenie WpfUiCustom.xaml z dostosowaniami |
| Problemy z backward compatibility | Niskie | Średni | Testy na Windows 10/11 |
| Breaking changes w WPF UI 4.x | Średnie | Wysoki | Przypięcie wersji w .csproj |
| Regresje w istniejących funkcjach | Wysokie | Bardzo wysoki | Comprehensive testing, CI/CD |
| Problemy z custom window chrome | Średnie | Średni | Użycie ui:FluentWindow z Mica |

### 6.2 Plan awaryjny
- Jeśli migracja nie powiedzie się: zachowanie obecnego systemu
- Branch strategy: `feature/wpf-ui-migration` + code reviews
- Możliwość rollbacku na każdym etapie
- Parallel development: nowe funkcje w starym systemie

---

## 7. Przykłady kodu

### 7.1 Główne okno z nawigacją (opcjonalne)

```xml
<ui:FluentWindow x:Class="PrettyScreenSHOT.MainWindow"
                 xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml"
                 Title="PrettyScreenSHOT"
                 Width="1200" Height="800"
                 ExtendsContentIntoTitleBar="True"
                 WindowBackdropType="Mica">

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <ui:TitleBar Grid.Row="0" Title="PrettyScreenSHOT">
            <ui:TitleBar.Icon>
                <ui:ImageIcon Source="pack://application:,,,/app.ico"/>
            </ui:TitleBar.Icon>
        </ui:TitleBar>

        <ui:NavigationView Grid.Row="1"
                          x:Name="NavigationView"
                          IsBackButtonVisible="Collapsed"
                          IsPaneToggleVisible="True"
                          OpenPaneLength="220">

            <ui:NavigationView.MenuItems>
                <ui:NavigationViewItem Content="Historia"
                                      Icon="Image24"
                                      TargetPageType="local:ScreenshotHistoryPage"/>
                <ui:NavigationViewItem Content="Ustawienia"
                                      Icon="Settings24"
                                      TargetPageType="local:SettingsPage"/>
            </ui:NavigationView.MenuItems>

            <ui:NavigationView.FooterMenuItems>
                <ui:NavigationViewItem Content="O aplikacji" Icon="Info24"/>
            </ui:NavigationView.FooterMenuItems>

        </ui:NavigationView>
    </Grid>
</ui:FluentWindow>
```

### 7.2 Niestandardowy CircularButton

W **Themes/WpfUiCustom.xaml:**
```xml
<Style x:Key="CircularButton" TargetType="ui:Button">
    <Setter Property="Width" Value="40"/>
    <Setter Property="Height" Value="40"/>
    <Setter Property="CornerRadius" Value="20"/>
    <Setter Property="Padding" Value="0"/>
</Style>
```

Użycie:
```xml
<ui:Button Style="{StaticResource CircularButton}"
           Icon="Dismiss24"
           Click="OnCloseClick"/>
```

### 7.3 Theme switching

```csharp
// W SettingsWindow.xaml.cs
private void OnThemeChanged(object sender, SelectionChangedEventArgs e)
{
    if (ThemeComboBox.SelectedIndex == 0) // Dark
    {
        ApplicationThemeManager.Apply(ApplicationTheme.Dark);
    }
    else if (ThemeComboBox.SelectedIndex == 1) // Light
    {
        ApplicationThemeManager.Apply(ApplicationTheme.Light);
    }
    else if (ThemeComboBox.SelectedIndex == 2) // System
    {
        ApplicationThemeManager.ApplySystemTheme();
    }
}
```

---

## 8. Checklist migracji

### Pre-migracja
- [ ] Backup całego projektu
- [ ] Utworzenie brancha `feature/wpf-ui-migration`
- [ ] Code freeze dla nowych funkcji (opcjonalne)
- [ ] Komunikacja z zespołem

### Podczas migracji
- [ ] Instalacja WPF-UI 4.0.3
- [ ] Aktualizacja App.xaml i App.xaml.cs
- [ ] Utworzenie Themes/WpfUiCustom.xaml
- [ ] Migracja ThemeManager → ThemeService
- [ ] Migracja SettingsWindow
- [ ] Migracja ScreenshotHistoryWindow
- [ ] Migracja TextInputWindow → ContentDialog
- [ ] Migracja pozostałych okien
- [ ] Usunięcie NeumorphicStyles.xaml
- [ ] Testy funkcjonalne
- [ ] Testy wydajnościowe
- [ ] Code review

### Post-migracja
- [ ] Merge do main branch
- [ ] Release notes
- [ ] Aktualizacja dokumentacji użytkownika
- [ ] Monitoring bugów
- [ ] Retrospektywa zespołowa

---

## 9. Metryki sukcesu

### KPI
- **Zmniejszenie kodu:** Min. 30% redukcja kodu XAML
- **Performance:** Brak regresji w czasie uruchomienia
- **Bugs:** Nie więcej niż 5 krytycznych bugów po migracji
- **User satisfaction:** Min. 80% pozytywnych opinii
- **Czas migracji:** Max. 7 tygodni

### Metryki techniczne
| Metryka | Przed | Cel po migracji |
|---------|-------|----------------|
| Linie kodu XAML | 3,200 | 2,240 (-30%) |
| Niestandardowe style | 17 | 3-5 |
| Czas uruchomienia | Baseline | ≤ Baseline + 10% |
| Zużycie pamięci | Baseline | ≤ Baseline + 15% |
| Rozmiar build | Baseline | ≤ Baseline + 2MB |

---

## 10. Zasoby i odniesienia

### Dokumentacja
- **WPF UI Official Docs:** https://wpfui.lepo.co/
- **WPF UI GitHub:** https://github.com/lepoco/wpfui
- **WPF UI Gallery (Microsoft Store):** App demonstracyjna
- **API Reference:** https://wpfui.lepo.co/api/

### Przykłady
- **WPF UI Gallery Source:** https://github.com/lepoco/wpfui/tree/main/samples
- **Community Examples:** GitHub Topics: wpfui

### Wsparcie
- **GitHub Issues:** https://github.com/lepoco/wpfui/issues
- **Discussions:** https://github.com/lepoco/wpfui/discussions

---

## 11. Wnioski i rekomendacje

### Dlaczego WPF UI?
1. **Nowoczesność:** Fluent Design, Windows 11 integration
2. **Maintenance:** Aktywnie rozwijana biblioteka (8.9k stars)
3. **Ecosystem:** Duża społeczność, dużo przykładów
4. **Performance:** Natywna implementacja, bez overheadu
5. **Future-proof:** Zgodność z Microsoft design language

### Alternatywne rozwiązania rozważane
| Biblioteka | Zalety | Wady | Decyzja |
|------------|--------|------|---------|
| **WPF UI** ✅ | Fluent, aktywna, MIT | Młodsza biblioteka | **WYBRANO** |
| MaterialDesignInXAML | Dojrzała, Material Design | Nie Windows-native | Odrzucono |
| ModernWPF | Fluent Design | Mniej aktywna | Odrzucono |
| HandyControl | Bogate kontrolki | Chiński projekt, dokumentacja | Odrzucono |
| Custom (obecne) | Pełna kontrola | Wysoki koszt utrzymania | **Zastępujemy** |

### Następne kroki
1. **Zatwierdzenie planu** przez stakeholderów
2. **Alokacja zasobów** (developer time)
3. **Utworzenie feature branch**
4. **Rozpoczęcie Sprint 1**
5. **Weekly status updates**

---

## 12. Harmonogram szczegółowy

### Sprint 1 (Tydzień 1)
- **Pon:** Instalacja pakietów, konfiguracja projektu
- **Wt-Śr:** Aktualizacja App.xaml, App.xaml.cs
- **Czw:** Utworzenie ThemeService
- **Pt:** Testy integracyjne, code review

### Sprint 2 (Tydzień 2)
- **Pon-Wt:** Migracja ThemeService, refactoring wywołań
- **Śr-Czw:** Utworzenie WpfUiCustom.xaml
- **Pt:** Testing, dokumentacja

### Sprint 3 (Tydzień 3)
- **Cały tydzień:** SettingsWindow.xaml migracja
- Daily checkpoints

### Sprint 4 (Tydzień 4)
- **Cały tydzień:** ScreenshotHistoryWindow.xaml migracja
- Performance testing

### Sprint 5 (Tydzień 5)
- **Pon-Śr:** TextInputWindow → ContentDialog
- **Czw-Pt:** UpdateWindow, SaveScreenshotDialog

### Sprint 6 (Tydzień 6)
- **Pon-Śr:** Pozostałe okna (VideoCaptureWindow, etc.)
- **Czw-Pt:** InfoBar, Snackbar integration

### Sprint 7 (Tydzień 7)
- **Pon-Wt:** Comprehensive testing
- **Śr:** Bug fixing
- **Czw:** Documentation, cleanup
- **Pt:** Code review, merge preparation

---

## Podsumowanie

Migracja do WPF UI 4.0.3 to strategiczna decyzja, która:
- Modernizuje aplikację zgodnie z najnowszymi trendami Windows 11
- Redukuje dług techniczny i koszty utrzymania
- Poprawia experience użytkownika
- Ułatwia rozwój nowych funkcji

**Szacowany czas:** 7 tygodni
**Szacowane zasoby:** 1 developer full-time
**Ryzyko:** Średnie (z planem mitygacji)
**ROI:** Wysoki (długoterminowe korzyści)

**Rekomendacja:** ✅ **ZATWIERDŹ MIGRACJĘ**

---

**Dokument przygotowany przez:** Claude AI
**Data:** 2025-01-15
**Wersja:** 1.0
**Status:** Do zatwierdzenia
