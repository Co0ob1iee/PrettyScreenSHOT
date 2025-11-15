# Baza Wiedzy - Migracja WPF UI

**Data utworzenia:** 2025-01-15
**Cel:** Kompletny inwentarz elementów przed migracją
**Status:** Pre-implementacja

---

## Spis treści

1. [Statystyki projektu](#1-statystyki-projektu)
2. [Inwentarz plików](#2-inwentarz-plik%C3%B3w)
3. [Mapowanie stylów neumorficznych](#3-mapowanie-styl%C3%B3w-neumorficznych)
4. [Inwentarz kontrolek z nazwami (x:Name)](#4-inwentarz-kontrolek-z-nazwami-xname)
5. [Event handlery](#5-event-handlery)
6. [ThemeManager - lokalizacje użycia](#6-thememanager---lokalizacje-u%C5%BCycia)
7. [Code-behind dependencies](#7-code-behind-dependencies)
8. [Impact Matrix](#8-impact-matrix)
9. [Priorytety migracji](#9-priorytety-migracji)
10. [Checklist przygotowawcza](#10-checklist-przygotowawcza)

---

## 1. Statystyki projektu

### Liczba linii kodu

| Typ pliku | Liczba linii | Procent |
|-----------|--------------|---------|
| **XAML (wszystkie)** | 2,597 | 100% |
| NeumorphicStyles.xaml | 643 | 24.8% |
| Pozostałe XAML | 1,954 | 75.2% |

### Struktura projektu

- **Pliki XAML:** 12 (11 okien + 1 styles)
- **Pliki code-behind:** 11 (*.xaml.cs)
- **Managery C#:** 15+ (ThemeManager, SettingsManager, etc.)
- **Pakiety NuGet:** 2 (Magick.NET)

---

## 2. Inwentarz plików

### 2.1 Okna główne (Windows)

| Plik | Linie | Priorytet | Złożoność | Kontrolki z Style |
|------|-------|-----------|-----------|-------------------|
| **SettingsWindow.xaml** | ~152 | P1 - WYSOKI | Średnia | 18 |
| **ScreenshotHistoryWindow.xaml** | ~164 | P1 - WYSOKI | Średnia | 12 |
| **TextInputWindow.xaml** | ~219 | P2 - ŚREDNI | Wysoka | 25 |
| **ScreenshotEditorWindow.xaml** | ~186 | P2 - ŚREDNI | Wysoka | 15 |
| **UpdateWindow.xaml** | ~136 | P3 - NISKI | Średnia | 17 |
| **UpdateNotificationWindow.xaml** | ~96 | P3 - NISKI | Niska | 10 |
| **VideoCaptureWindow.xaml** | ~127 | P3 - NISKI | Średnia | 14 |
| **SaveScreenshotDialog.xaml** | ~91 | P2 - ŚREDNI | Niska | 8 |
| **UpdateProgressWindow.xaml** | ~38 | P3 - NISKI | Niska | 5 |
| **ScreenshotOverlay.xaml** | ~78 | P1 - WYSOKI | Średnia | 0 (custom) |
| **App.xaml** | ~13 | P1 - WYSOKI | Niska | 0 |

### 2.2 Code-behind files

| Plik | ThemeManager | Kontrolki z x:Name | Event Handlers |
|------|-------------|-------------------|----------------|
| App.xaml.cs | ✅ SetTheme | 0 | 2 |
| SettingsWindow.xaml.cs | ✅ ApplyTheme, SetTheme | 13 | 7 |
| ScreenshotHistoryWindow.xaml.cs | ✅ ApplyTheme | 4 | 5 |
| TextInputWindow.xaml.cs | ✅ ApplyTheme | 15 | 9 |
| ScreenshotEditorWindow.xaml.cs | ✅ ApplyTheme | 3 | 7 |
| SaveScreenshotDialog.xaml.cs | ✅ ApplyTheme | 4 | 2 |
| UpdateWindow.xaml.cs | ❌ | 9 | 3 |
| UpdateNotificationWindow.xaml.cs | ❌ | 4 | 4 |
| VideoCaptureWindow.xaml.cs | ❌ | 6 | 4 |
| UpdateProgressWindow.xaml.cs | ❌ | 2 | 1 |
| ScreenshotOverlay.xaml.cs | ❌ | 0 | 3 |

### 2.3 Managery i serwisy

| Plik | Funkcja | Czy migrować? |
|------|---------|---------------|
| **ThemeManager.cs** | Zarządzanie motywami Dark/Light/Neumorphic | ✅ DO WPF UI ThemeService |
| SettingsManager.cs | Ustawienia aplikacji | ⚠️ Zaktualizować Theme handling |
| LocalizationHelper.cs | Lokalizacja | ⬜ BEZ ZMIAN |
| ScreenshotManager.cs | Logika zrzutów | ⬜ BEZ ZMIAN |
| CloudUploadManager.cs | Upload do chmury | ⬜ BEZ ZMIAN |
| TrayIconManager.cs | Ikona w zasobniku | ⬜ BEZ ZMIAN |
| UpdateManager.cs | Aktualizacje | ⬜ BEZ ZMIAN |

---

## 3. Mapowanie stylów neumorficznych

### 3.1 Style z NeumorphicStyles.xaml

| Styl źródłowy | Użycie (pliki) | WPF UI Target | Notatki |
|---------------|----------------|---------------|---------|
| **NeumorphicRaisedButton** | 15x w 8 plikach | `<ui:Button>` | Standard button |
| **NeumorphicDepressedButton** | 7x w 5 plikach | `<ui:Button Appearance="Primary">` | Action button |
| **NeumorphicCircularButton** | 8x w 7 plikach | Custom style `CircularIconButton` | Close/icon buttons |
| **NeumorphicPanel** | 10x w 10 plikach | `<ui:Card>` | Main container |
| **NeumorphicTextBox** | 4x w 3 plikach | `<ui:TextBox>` | Input field |
| **NeumorphicComboBox** | 9x w 4 plikach | `<ComboBox>` (auto-styled) | Dropdown |
| **NeumorphicComboBoxItem** | 2x w 1 pliku | Auto-styled | Dropdown items |
| **NeumorphicCheckBox** | 8x w 2 plikach | `<CheckBox>` (auto-styled) | Checkbox |
| **NeumorphicRadioButton** | 3x w 1 pliku | `<RadioButton>` (auto-styled) | Radio button |
| **NeumorphicSlider** | 4x w 3 plikach | `<Slider>` (auto-styled) lub `<ui:NumberBox>` | Numeric input |
| **NeumorphicTextBlock** | 35x w 7 plikach | `<TextBlock>` (auto-styled) | Text display |
| **NeumorphicTextBlockSecondary** | 15x w 5 plikach | `<TextBlock Foreground="{DynamicResource TextFillColorSecondaryBrush}">` | Secondary text |
| **NeumorphicHeading** | 6x w 6 plikach | `<TextBlock Style="{StaticResource TitleTextBlockStyle}">` | H1 heading |
| **NeumorphicHeadingLarge** | 1x w 1 pliku | `<TextBlock Style="{StaticResource SubtitleTextBlockStyle}">` | H2 heading |
| **NeumorphicHeadingSmall** | 1x w 1 pliku | `<TextBlock Style="{StaticResource BodyStrongTextBlockStyle}">` | H3 heading |
| **NeumorphicRaisedBorder** | 1x w 1 pliku | `<ui:Card>` | Card container |
| **NeumorphicDepressedBorder** | 3x w 2 plikach | `<Border Background="{DynamicResource ControlFillColorDefaultBrush}">` | Inset panel |
| **NeumorphicScrollBar** | 0x (auto) | Auto-styled | Scrollbar |

### 3.2 Szczegółowe użycie stylów

#### NeumorphicRaisedButton (15 użyć)
| Plik | Kontrolka | Funkcja |
|------|-----------|---------|
| SettingsWindow.xaml | BrowseButton | Wybór folderu |
| SettingsWindow.xaml | ResetButton | Reset ustawień |
| SettingsWindow.xaml | CancelButton2 | Anuluj |
| ScreenshotHistoryWindow.xaml | ClearFiltersButton | Wyczyść filtry |
| ScreenshotHistoryWindow.xaml | (inline) OnUploadClick | Upload do chmury |
| ScreenshotHistoryWindow.xaml | (inline) OnCloudUrlClick | Otwórz URL |
| TextInputWindow.xaml | (Cancel button) | Anuluj |
| ScreenshotEditorWindow.xaml | 5x tool buttons | Narzędzia edycji |
| UpdateWindow.xaml | SkipButton, LaterButton | Pomiń/Później |
| UpdateNotificationWindow.xaml | LaterButton, SkipVersionButton, ViewOnGitHubButton | Akcje aktualizacji |
| VideoCaptureWindow.xaml | StopButton, CancelButton | Stop/Anuluj nagrywanie |
| UpdateProgressWindow.xaml | CancelButton | Anuluj pobieranie |
| SaveScreenshotDialog.xaml | CancelButton | Anuluj |

#### NeumorphicDepressedButton (7 użyć)
| Plik | Kontrolka | Funkcja |
|------|-----------|---------|
| SettingsWindow.xaml | SaveButton | Zapisz ustawienia |
| TextInputWindow.xaml | OK button | Zatwierdź tekst |
| UpdateWindow.xaml | DownloadButton | Pobierz aktualizację |
| UpdateNotificationWindow.xaml | UpdateNowButton | Aktualizuj teraz |
| VideoCaptureWindow.xaml | StartButton | Rozpocznij nagrywanie |
| SaveScreenshotDialog.xaml | SaveButton | Zapisz zrzut |

#### NeumorphicCircularButton (8 użyć)
| Plik | Kontrolka | Funkcja |
|------|-----------|---------|
| SettingsWindow.xaml | (Close) | Zamknij okno |
| ScreenshotHistoryWindow.xaml | (Close), (Delete) | Zamknij/Usuń |
| TextInputWindow.xaml | (Close) | Zamknij okno |
| ScreenshotEditorWindow.xaml | (Close) | Zamknij okno |
| SaveScreenshotDialog.xaml | (Close) | Zamknij okno |
| UpdateWindow.xaml | (Close) | Zamknij okno |
| UpdateNotificationWindow.xaml | (Close) | Zamknij okno |
| VideoCaptureWindow.xaml | (Close) | Zamknij okno |

#### NeumorphicPanel (10 użyć)
**Każde okno używa jako głównego kontenera:**
- SettingsWindow.xaml (line 17)
- ScreenshotHistoryWindow.xaml (line 21)
- TextInputWindow.xaml (line 17)
- ScreenshotEditorWindow.xaml (implied)
- SaveScreenshotDialog.xaml (implied)
- UpdateWindow.xaml (line 17)
- UpdateNotificationWindow.xaml (line 17)
- VideoCaptureWindow.xaml (line 17)
- UpdateProgressWindow.xaml (line 17)

---

## 4. Inwentarz kontrolek z nazwami (x:Name)

### 4.1 SettingsWindow.xaml (13 kontrolek)

| x:Name | Typ | Styl | Binding/Event | Akcja migracji |
|--------|-----|------|---------------|----------------|
| TitleText | TextBlock | NeumorphicHeading | Binding Title | Auto-styled |
| LanguageLabel | TextBlock | NeumorphicTextBlock | - | Auto-styled |
| LanguageComboBox | ComboBox | NeumorphicComboBox | SelectionChanged (code) | Auto-styled, sprawdź event |
| SavePathLabel | TextBlock | NeumorphicTextBlock | - | Auto-styled |
| SavePathTextBox | TextBox | NeumorphicTextBox | - | → `<ui:TextBox Icon="Folder24">` |
| BrowseButton | Button | NeumorphicRaisedButton | Click | → `<ui:Button Icon="FolderOpen24">` |
| HotkeyLabel | TextBlock | NeumorphicTextBlock | - | Auto-styled |
| HotkeyComboBox | ComboBox | NeumorphicComboBox | - | Auto-styled |
| ImageFormatLabel | TextBlock | NeumorphicTextBlock | - | Auto-styled |
| ImageFormatComboBox | ComboBox | NeumorphicComboBox | - | Auto-styled |
| ImageQualityLabel | TextBlock | NeumorphicTextBlock | - | Auto-styled |
| QualitySlider | Slider | NeumorphicSlider | ValueChanged | → `<ui:NumberBox>` lub `<Slider>` |
| QualityLabel | TextBlock | NeumorphicTextBlock | Binding Slider.Value | Zachować lub usunąć (NumberBox) |
| AutoSaveCheckBox | CheckBox | NeumorphicCheckBox | - | Auto-styled |
| CopyToClipboardCheckBox | CheckBox | NeumorphicCheckBox | - | Auto-styled |
| ShowNotificationsCheckBox | CheckBox | NeumorphicCheckBox | - | Auto-styled |
| AutoUploadCheckBox | CheckBox | NeumorphicCheckBox | - | Auto-styled |
| ThemeLabel | TextBlock | NeumorphicTextBlock | - | Auto-styled |
| ThemeComboBox | ComboBox | NeumorphicComboBox | SelectionChanged | Auto-styled, **ZAKTUALIZUJ LOGIC** |
| ResetButton | Button | NeumorphicRaisedButton | Click | → `<ui:Button>` |
| CancelButton2 | Button | NeumorphicRaisedButton | Click | → `<ui:Button>` |
| SaveButton | Button | NeumorphicDepressedButton | Click | → `<ui:Button Appearance="Primary">` |

### 4.2 ScreenshotHistoryWindow.xaml (4 kontrolki)

| x:Name | Typ | Styl | Binding/Event | Akcja migracji |
|--------|-----|------|---------------|----------------|
| SearchTextBox | TextBox | NeumorphicTextBox | TextChanged | → `<ui:TextBox Icon="Search24" ClearButtonEnabled="True">` |
| CategoryComboBox | ComboBox | NeumorphicComboBox | SelectionChanged | Auto-styled |
| ClearFiltersButton | Button | NeumorphicRaisedButton | Click | → `<ui:Button>` |
| HistoryListBox | ItemsControl | - | ItemsSource Binding | Zmień ItemTemplate na `<ui:Card>` |

### 4.3 TextInputWindow.xaml (15 kontrolek)

| x:Name | Typ | Styl | Binding/Event | Akcja migracji |
|--------|-----|------|---------------|----------------|
| TextLabel | TextBlock | NeumorphicTextBlockSecondary | - | Auto-styled |
| TextInputControl | TextBox | NeumorphicTextBox | TextChanged | → `<ui:TextBox>` multiline |
| FontFamilyComboBox | ComboBox | NeumorphicComboBox | SelectionChanged | Auto-styled |
| FontSizeTextLabel | TextBlock | NeumorphicTextBlockSecondary | - | Auto-styled |
| FontSizeSlider | Slider | NeumorphicSlider | ValueChanged | → `<ui:NumberBox>` |
| FontSizeValueLabel | TextBlock | NeumorphicTextBlockSecondary | - | Może być usunięty z NumberBox |
| AlignLeftRadio | RadioButton | NeumorphicRadioButton | Checked | Auto-styled |
| AlignCenterRadio | RadioButton | NeumorphicRadioButton | Checked | Auto-styled |
| AlignRightRadio | RadioButton | NeumorphicRadioButton | Checked | Auto-styled |
| BoldCheckBox | CheckBox | NeumorphicCheckBox | Checked/Unchecked | Auto-styled |
| ItalicCheckBox | CheckBox | NeumorphicCheckBox | Checked/Unchecked | Auto-styled |
| UnderlineCheckBox | CheckBox | NeumorphicCheckBox | Checked/Unchecked | Auto-styled |
| StrikethroughCheckBox | CheckBox | NeumorphicCheckBox | Checked/Unchecked | Auto-styled |
| ColorComboBox | ComboBox | NeumorphicComboBox | SelectionChanged | Auto-styled |
| BackgroundColorComboBox | ComboBox | NeumorphicComboBox | SelectionChanged | Auto-styled |
| StrokeColorComboBox | ComboBox | NeumorphicComboBox | SelectionChanged | Auto-styled |
| StrokeThicknessSlider | Slider | NeumorphicSlider | ValueChanged | → `<ui:NumberBox>` lub `<Slider>` |
| StrokeThicknessLabel | TextBlock | NeumorphicTextBlockSecondary | - | Może być usunięty |
| PreviewLabel | TextBlock | NeumorphicTextBlockSecondary | - | Auto-styled |
| PreviewText | TextBlock | NeumorphicTextBlock | - | Auto-styled |

### 4.4 ScreenshotEditorWindow.xaml (3 kontrolki)

| x:Name | Typ | Styl | Binding/Event | Akcja migracji |
|--------|-----|------|---------------|----------------|
| EditorCanvas | Canvas | - | Mouse events | Bez zmian |
| ColorButton | Button | NeumorphicRaisedButton | Click | → `<ui:Button>` z ColorPicker |
| SizeSlider | Slider | NeumorphicSlider | ValueChanged | → `<ui:NumberBox>` lub `<Slider>` |
| SizeLabel | TextBlock | NeumorphicTextBlock | - | Może być usunięty |

### 4.5 UpdateWindow.xaml (9 kontrolek)

| x:Name | Typ | Styl | Binding/Event | Akcja migracji |
|--------|-----|------|---------------|----------------|
| SubtitleText | TextBlock | NeumorphicTextBlockSecondary | - | Auto-styled |
| CurrentVersionText | TextBlock | NeumorphicTextBlock | - | Auto-styled |
| NewVersionText | TextBlock | NeumorphicTextBlock | - | Auto-styled |
| ReleaseDateText | TextBlock | NeumorphicTextBlock | - | Auto-styled |
| FileSizeText | TextBlock | NeumorphicTextBlock | - | Auto-styled |
| ReleaseNotesText | TextBlock | NeumorphicTextBlockSecondary | - | Auto-styled |
| ProgressPanel | StackPanel | - | Visibility | Bez zmian |
| DownloadProgressBar | Border | NeumorphicDepressedBorder | Width animation | → `<ProgressBar>` |
| ProgressText | TextBlock | NeumorphicTextBlockSecondary | - | Auto-styled |
| SkipButton | Button | NeumorphicRaisedButton | Click | → `<ui:Button>` |
| LaterButton | Button | NeumorphicRaisedButton | Click | → `<ui:Button>` |
| DownloadButton | Button | NeumorphicDepressedButton | Click | → `<ui:Button Appearance="Primary">` |

### 4.6 UpdateNotificationWindow.xaml (4 kontrolki)

| x:Name | Typ | Styl | Binding/Event | Akcja migracji |
|--------|-----|------|---------------|----------------|
| VersionText | TextBlock | NeumorphicTextBlockSecondary | - | Auto-styled |
| ReleaseNotesText | TextBlock | NeumorphicTextBlockSecondary | - | Auto-styled |
| FileSizeText | TextBlock | NeumorphicTextBlock | - | Auto-styled |
| PublishedText | TextBlock | NeumorphicTextBlock | - | Auto-styled |
| UpdateNowButton | Button | NeumorphicDepressedButton | Click | → `<ui:Button Appearance="Primary">` |
| LaterButton | Button | NeumorphicRaisedButton | Click | → `<ui:Button>` |
| SkipVersionButton | Button | NeumorphicRaisedButton | Click | → `<ui:Button>` |
| ViewOnGitHubButton | Button | NeumorphicRaisedButton | Click | → `<ui:Button Icon="Open24">` |

### 4.7 VideoCaptureWindow.xaml (6 kontrolek)

| x:Name | Typ | Styl | Binding/Event | Akcja migracji |
|--------|-----|------|---------------|----------------|
| StatusText | TextBlock | NeumorphicTextBlockSecondary | - | Auto-styled |
| AreaText | TextBlock | NeumorphicTextBlock | - | Auto-styled |
| FrameRateSlider | Slider | NeumorphicSlider | ValueChanged | → `<ui:NumberBox>` lub `<Slider>` |
| FrameRateLabel | TextBlock | NeumorphicTextBlock | - | Może być usunięty |
| StartButton | Button | NeumorphicDepressedButton | Click | → `<ui:Button Appearance="Primary">` |
| StopButton | Button | NeumorphicRaisedButton | Click | → `<ui:Button>` |
| CancelButton | Button | NeumorphicRaisedButton | Click | → `<ui:Button>` |

### 4.8 SaveScreenshotDialog.xaml (4 kontrolki)

| x:Name | Typ | Styl | Binding/Event | Akcja migracji |
|--------|-----|------|---------------|----------------|
| CategoryLabel | TextBlock | NeumorphicTextBlock | - | Auto-styled |
| CategoryComboBox | ComboBox | NeumorphicComboBox | - | Auto-styled |
| TagsLabel | TextBlock | NeumorphicTextBlock | - | Auto-styled |
| TagsTextBox | TextBox | NeumorphicTextBox | - | → `<ui:TextBox>` |
| NotesLabel | TextBlock | NeumorphicTextBlock | - | Auto-styled |
| NotesTextBox | TextBox | NeumorphicTextBox | - | → `<ui:TextBox>` multiline |
| CancelButton | Button | NeumorphicRaisedButton | Click | → `<ui:Button>` |
| SaveButton | Button | NeumorphicDepressedButton | Click | → `<ui:Button Appearance="Primary">` |

### 4.9 UpdateProgressWindow.xaml (2 kontrolki)

| x:Name | Typ | Styl | Binding/Event | Akcja migracji |
|--------|-----|------|---------------|----------------|
| ProgressBar | Border | NeumorphicDepressedBorder | Width animation | → `<ProgressBar>` |
| ProgressText | TextBlock | NeumorphicTextBlock | - | Auto-styled |
| CancelButton | Button | NeumorphicRaisedButton | Click | → `<ui:Button>` |

**SUMA KONTROLEK Z x:Name: 65+**

---

## 5. Event handlery

### 5.1 Click handlers (przyciski)

| Event Handler | Pliki | Liczba użyć | Akcja |
|---------------|-------|-------------|-------|
| OnCloseClick | 7 plików | 8 | Zachować |
| OnCancelClick | 4 pliki | 5 | Zachować |
| SaveButton_Click | 1 plik | 1 | Zachować |
| CancelButton_Click | 3 pliki | 4 | Zachować |
| BrowseButton_Click | 1 plik | 1 | Zachować |
| ResetButton_Click | 1 plik | 1 | Zachować |
| OnSaveClick | 1 plik | 1 | Zachować |
| OnDeleteClick | 1 plik | 1 | Zachować |
| OnUploadClick | 2 pliki | 2 | Zachować |
| OnCloudUrlClick | 1 plik | 1 | Zachować |
| OnClearFiltersClick | 1 plik | 1 | Zachować |
| UpdateNowButton_Click | 1 plik | 1 | Zachować |
| LaterButton_Click | 1 plik | 1 | Zachować |
| SkipVersionButton_Click | 1 plik | 1 | Zachować |
| ViewOnGitHubButton_Click | 1 plik | 1 | Zachować |
| DownloadButton_Click | 1 plik | 1 | Zachować |
| SkipButton_Click | 1 plik | 1 | Zachować |
| StartButton_Click | 1 plik | 1 | Zachować |
| StopButton_Click | 1 plik | 1 | Zachować |
| OnToolButtonClick | 1 plik | 5 | Zachować (ScreenshotEditor) |
| OnColorPickerClick | 1 plik | 1 | Zachować |
| OnClearClick | 1 plik | 1 | Zachować |
| OnUndoClick | 1 plik | 1 | Zachować |
| OnOkClick | 1 plik | 1 | Zachować |

### 5.2 ValueChanged handlers (slidery)

| Event Handler | Plik | Kontrolka | Akcja |
|---------------|------|-----------|-------|
| QualitySlider_ValueChanged | SettingsWindow.xaml.cs | QualitySlider | ⚠️ Zmienić na NumberBox_ValueChanged lub zachować |
| FontSizeSlider_ValueChanged | TextInputWindow.xaml.cs | FontSizeSlider | ⚠️ Zmienić na NumberBox_ValueChanged lub zachować |
| StrokeThicknessSlider_ValueChanged | TextInputWindow.xaml.cs | StrokeThicknessSlider | ⚠️ Zmienić na NumberBox_ValueChanged lub zachować |
| SizeSlider_ValueChanged | ScreenshotEditorWindow.xaml.cs | SizeSlider | ⚠️ Zmienić na NumberBox_ValueChanged lub zachować |
| FrameRateSlider_ValueChanged | VideoCaptureWindow.xaml.cs | FrameRateSlider | ⚠️ Zmienić na NumberBox_ValueChanged lub zachować |

### 5.3 SelectionChanged handlers

| Event Handler | Plik | Kontrolka | Akcja |
|---------------|------|-----------|-------|
| LanguageComboBox_SelectionChanged | SettingsWindow.xaml.cs | LanguageComboBox | ⚠️ Sprawdzić czy działa po auto-styling |
| ThemeComboBox_SelectionChanged | SettingsWindow.xaml.cs | ThemeComboBox | ✅ **ZAKTUALIZOWAĆ** - zmienić na WPF UI theme |
| OnCategoryChanged | ScreenshotHistoryWindow.xaml.cs | CategoryComboBox | Zachować |
| FontFamilyComboBox_SelectionChanged | TextInputWindow.xaml.cs | FontFamilyComboBox | Zachować |
| ColorComboBox_SelectionChanged | TextInputWindow.xaml.cs | ColorComboBox | Zachować |
| BackgroundColorComboBox_SelectionChanged | TextInputWindow.xaml.cs | BackgroundColorComboBox | Zachować |
| StrokeColorComboBox_SelectionChanged | TextInputWindow.xaml.cs | StrokeColorComboBox | Zachować |

### 5.4 TextChanged handlers

| Event Handler | Plik | Kontrolka | Akcja |
|---------------|------|-----------|-------|
| OnSearchTextChanged | ScreenshotHistoryWindow.xaml.cs | SearchTextBox | Zachować |
| TextInputControl_TextChanged | TextInputWindow.xaml.cs | TextInputControl | Zachować |

### 5.5 Checked/Unchecked handlers

| Event Handler | Plik | Kontrolki | Akcja |
|---------------|------|-----------|-------|
| StyleCheckBox_Checked | TextInputWindow.xaml.cs | BoldCheckBox, ItalicCheckBox, UnderlineCheckBox, StrikethroughCheckBox | Zachować |
| TextAlignment_Changed | TextInputWindow.xaml.cs | AlignLeftRadio, AlignCenterRadio, AlignRightRadio | Zachować |

### 5.6 Inne event handlers

| Event Handler | Plik | Kontrolka | Akcja |
|---------------|------|-----------|-------|
| OnTitleBarMouseDown | 6 plików | Border (Title bar) | ⚠️ USUNĄĆ - WPF UI ma własny TitleBar |
| SettingsWindow_Loaded | SettingsWindow.xaml.cs | Window | Zachować |

---

## 6. ThemeManager - lokalizacje użycia

### 6.1 ThemeManager.cs (źródło)

**Lokalizacja:** `/ThemeManager.cs`
**Linie kodu:** 180
**Status:** ✅ **DO ZASTĄPIENIA ThemeService.cs**

**Funkcje publiczne:**
```csharp
public static ThemeManager Instance { get; }
public Theme CurrentTheme { get; private set; }
public event EventHandler<Theme>? ThemeChanged;

public void SetTheme(Theme theme)
public void ToggleTheme()
public void ApplyTheme(Window window)
public ThemeColors GetThemeColors()

public static ThemeColors DarkColors { get; }
public static ThemeColors LightColors { get; }
public static ThemeColors NeumorphicColors { get; }
```

### 6.2 Wywołania ThemeManager.Instance

| Plik | Linia | Wywołanie | Akcja migracji |
|------|-------|-----------|----------------|
| **App.xaml.cs** | 31 | `ThemeManager.Instance.SetTheme(theme)` | → `ThemeService.Instance.SetTheme(theme)` |
| **SettingsManager.cs** | 142 | `ThemeManager.Instance.SetTheme(theme)` | → `ThemeService.Instance.SetTheme(theme)` |
| **SettingsWindow.xaml.cs** | 17 | `ThemeManager.Instance.ApplyTheme(this)` | ⚠️ USUNĄĆ - WPF UI auto-applies |
| **SettingsWindow.xaml.cs** | 256 | `ThemeManager.Instance.SetTheme(theme)` | → `ThemeService.Instance.SetTheme(theme)` |
| **TextInputWindow.xaml.cs** | 39 | `ThemeManager.Instance.ApplyTheme(this)` | ⚠️ USUNĄĆ - WPF UI auto-applies |
| **SaveScreenshotDialog.xaml.cs** | 21 | `ThemeManager.Instance.ApplyTheme(this)` | ⚠️ USUNĄĆ - WPF UI auto-applies |
| **ScreenshotHistoryWindow.xaml.cs** | 22 | `ThemeManager.Instance.ApplyTheme(this)` | ⚠️ USUNĄĆ - WPF UI auto-applies |
| **ScreenshotEditorWindow.xaml.cs** | 74 | `ThemeManager.Instance.ApplyTheme(this)` | ⚠️ USUNĄĆ - WPF UI auto-applies |

**SUMA:** 8 wywołań w 6 plikach

### 6.3 Akcje migracji ThemeManager

#### ✅ DO ZROBIENIA:

1. **Utworzyć ThemeService.cs** (nowy plik)
   - Wrapper dla `Wpf.Ui.Appearance.ApplicationThemeManager`
   - Kompatybilność z istniejącym enum `Theme`
   - Metody: `SetTheme(Theme)`, `SetTheme(string)`, `ApplySystemTheme()`

2. **Zaktualizować wywołania:**
   - App.xaml.cs (line 31)
   - SettingsManager.cs (line 142)
   - SettingsWindow.xaml.cs (line 256)

3. **Usunąć wywołania ApplyTheme:**
   - SettingsWindow.xaml.cs (line 17)
   - TextInputWindow.xaml.cs (line 39)
   - SaveScreenshotDialog.xaml.cs (line 21)
   - ScreenshotHistoryWindow.xaml.cs (line 22)
   - ScreenshotEditorWindow.xaml.cs (line 74)

4. **Zaktualizować SettingsWindow.xaml.cs:**
   - `ThemeComboBox_SelectionChanged` handler
   - Zamienić `ThemeManager.Instance.SetTheme` na `ApplicationThemeManager.Apply`

5. **Po migracji - USUNĄĆ:**
   - ThemeManager.cs (całkowicie)
   - Enum `Theme` (opcjonalnie zachować dla backward compatibility)

---

## 7. Code-behind dependencies

### 7.1 SettingsWindow.xaml.cs

**Zależności:**
- `ThemeManager` ✅ Migracja
- `SettingsManager` ⬜ Bez zmian
- `LocalizationHelper` ⬜ Bez zmian
- `System.Windows.Forms.FolderBrowserDialog` ⬜ Bez zmian

**Kontrolki wymagające aktualizacji:**
- `QualitySlider` → Możliwa zmiana na `NumberBox`
- `ThemeComboBox` → Zaktualizuj handler `ThemeComboBox_SelectionChanged`
- `LanguageComboBox` → Sprawdź po auto-styling

**Event handlery do sprawdzenia:**
- `LanguageComboBox_SelectionChanged` (line 137)
- `QualitySlider_ValueChanged` (line 168)
- `ThemeComboBox_SelectionChanged` (line 249) ✅ **KRYTYCZNE**

### 7.2 TextInputWindow.xaml.cs

**Zależności:**
- `ThemeManager` ✅ Migracja (line 39)
- `System.Windows.Media` ⬜ Bez zmian

**Kontrolki wymagające aktualizacji:**
- `FontSizeSlider` → Możliwa zmiana na `NumberBox`
- `StrokeThicknessSlider` → Możliwa zmiana na `NumberBox`

**Logika do zachowania:**
- Text preview logic
- Font family enumeration
- Color selection
- Text alignment

### 7.3 ScreenshotHistoryWindow.xaml.cs

**Zależności:**
- `ThemeManager` ✅ Migracja (line 22)

**Kontrolki wymagające aktualizacji:**
- `SearchTextBox` → `<ui:TextBox Icon="Search24" ClearButtonEnabled="True">`
- ItemTemplate → Zmiana na `<ui:Card>`

### 7.4 ScreenshotEditorWindow.xaml.cs

**Zależności:**
- `ThemeManager` ✅ Migracja (line 74)
- Canvas drawing logic ⬜ Bez zmian

**Kontrolki wymagające aktualizacji:**
- `SizeSlider` → Możliwa zmiana na `NumberBox`

### 7.5 SaveScreenshotDialog.xaml.cs

**Zależności:**
- `ThemeManager` ✅ Migracja (line 21)

**Możliwa konwersja na ContentDialog:**
- Może być przekonwertowane na `<ui:ContentDialog>`
- Ale wymaga refactoringu wywołań w rodzicu

### 7.6 App.xaml.cs

**Zależności:**
- `ThemeManager` ✅ Migracja (line 31)
- `SettingsManager` ⬜ Bez zmian
- `LocalizationHelper` ⬜ Bez zmian

**Wymagana aktualizacja:**
- `OnStartup` metoda → Dodać WPF UI theme initialization
- Zamienić `ThemeManager.Instance.SetTheme` na `ApplicationThemeManager.Apply`

---

## 8. Impact Matrix

### 8.1 Wpływ na pliki

| Plik | Typ wpływu | Ryzyko | Szacowany czas | Zależności |
|------|------------|--------|----------------|------------|
| **App.xaml** | WYSOKI | Niskie | 30 min | Wszystkie okna |
| **App.xaml.cs** | WYSOKI | Średnie | 1h | ThemeManager, wszystkie okna |
| **NeumorphicStyles.xaml** | USUNIĘCIE | Niskie | - | Wszystkie okna XAML |
| **ThemeManager.cs** | USUNIĘCIE | Średnie | - | 6 plików .cs |
| **ThemeService.cs** | NOWY | Średnie | 2h | App.xaml.cs, SettingsManager.cs |
| **SettingsWindow.xaml** | WYSOKI | Średnie | 3h | ThemeService |
| **SettingsWindow.xaml.cs** | ŚREDNI | Średnie | 1h | ThemeService |
| **ScreenshotHistoryWindow.xaml** | WYSOKI | Niskie | 2h | - |
| **TextInputWindow.xaml** | WYSOKI | Średnie | 3h | - |
| **ScreenshotEditorWindow.xaml** | ŚREDNI | Niskie | 2h | - |
| **UpdateWindow.xaml** | ŚREDNI | Niskie | 2h | - |
| Pozostałe okna | NISKI-ŚREDNI | Niskie | 1-2h każde | - |

### 8.2 Wpływ na funkcjonalność

| Funkcjonalność | Ryzyko | Test wymagany | Priorytet testów |
|----------------|--------|---------------|------------------|
| **Theme switching (Dark/Light)** | WYSOKIE | TAK | P1 - KRYTYCZNY |
| **Localization** | NISKIE | TAK | P2 |
| **Settings save/load** | ŚREDNIE | TAK | P1 - KRYTYCZNY |
| **Screenshot capture** | NISKIE | TAK | P1 - KRYTYCZNY |
| **Screenshot editing** | NISKIE | TAK | P2 |
| **Cloud upload** | NISKIE | TAK | P2 |
| **Auto-update** | NISKIE | TAK | P3 |
| **Keyboard shortcuts** | NISKIE | TAK | P2 |
| **Window dragging** | WYSOKIE | TAK | P1 - KRYTYCZNY (zmiana na ui:TitleBar) |

---

## 9. Priorytety migracji

### 9.1 Faza 1 - Infrastruktura (Sprint 1-2)

**Sprint 1: Konfiguracja**
- [ ] Dodać pakiet WPF-UI 4.0.3
- [ ] Zaktualizować App.xaml (dodać ThemesDictionary)
- [ ] Zaktualizować App.xaml.cs (inicjalizacja WPF UI)
- [ ] Utworzyć `Themes/WpfUiCustom.xaml`
- [ ] Test: Uruchomić aplikację, sprawdzić czy się kompiluje

**Sprint 2: ThemeService**
- [ ] Utworzyć ThemeService.cs
- [ ] Zaktualizować App.xaml.cs (zamienić ThemeManager → ThemeService)
- [ ] Zaktualizować SettingsManager.cs (zamienić ThemeManager → ThemeService)
- [ ] Test: Sprawdzić przełączanie motywów

### 9.2 Faza 2 - Główne okna (Sprint 3-4)

**Sprint 3: SettingsWindow**
- [ ] Zamienić `<Window>` → `<ui:FluentWindow>`
- [ ] Zamienić custom title bar → `<ui:TitleBar>`
- [ ] Zamienić wszystkie style kontrolek
- [ ] Zaktualizować SettingsWindow.xaml.cs (usunąć ApplyTheme, zaktualizować ThemeComboBox handler)
- [ ] Test: Funkcjonalność ustawień, zapis/odczyt, theme switching

**Sprint 4: ScreenshotHistoryWindow**
- [ ] Zamienić `<Window>` → `<ui:FluentWindow>`
- [ ] Zamienić ItemTemplate → `<ui:Card>`
- [ ] Zamienić SearchTextBox → `<ui:TextBox Icon="Search24">`
- [ ] Zaktualizować code-behind (usunąć ApplyTheme)
- [ ] Test: Wyszukiwanie, filtrowanie, usuwanie

### 9.3 Faza 3 - Dialogi i edytory (Sprint 5-6)

**Sprint 5: TextInputWindow + ScreenshotEditorWindow**
- [ ] Migracja TextInputWindow.xaml
- [ ] Migracja ScreenshotEditorWindow.xaml
- [ ] Zaktualizować code-behind
- [ ] Test: Dodawanie tekstu, edycja zrzutów

**Sprint 6: Pozostałe okna**
- [ ] UpdateWindow.xaml + UpdateNotificationWindow.xaml
- [ ] VideoCaptureWindow.xaml
- [ ] SaveScreenshotDialog.xaml
- [ ] UpdateProgressWindow.xaml
- [ ] Test: Wszystkie okna

### 9.4 Faza 4 - Finalizacja (Sprint 7)

- [ ] Comprehensive testing (wszystkie funkcje)
- [ ] Performance testing
- [ ] Accessibility testing
- [ ] Usunąć NeumorphicStyles.xaml
- [ ] Usunąć ThemeManager.cs
- [ ] Code review
- [ ] Dokumentacja

---

## 10. Checklist przygotowawcza

### 10.1 Przed rozpoczęciem migracji

- [ ] **Backup projektu** (git commit + tag)
- [ ] **Utworzenie brancha** `feature/wpf-ui-migration`
- [ ] **Code freeze** dla nowych funkcji (komunikacja z zespołem)
- [ ] **Przeczytanie dokumentacji WPF UI** (https://wpfui.lepo.co/)
- [ ] **Instalacja WPF UI Gallery** (Microsoft Store) - testowanie kontrolek
- [ ] **Przegląd przykładów** z repozytorium WPF UI
- [ ] **Utworzenie kopii zapasowej** ustawień użytkownika (SettingsManager)

### 10.2 Narzędzia i środowisko

- [ ] Visual Studio 2022 (najnowsza wersja)
- [ ] .NET 10.0 SDK
- [ ] NuGet Package Manager
- [ ] Git (kontrola wersji)
- [ ] WinDbg lub inny debugger (optional, na wypadek problemów)

### 10.3 Dokumenty referencyjne

- [ ] Ten dokument (MIGRATION_KNOWLEDGE_BASE.md)
- [ ] MIGRATION_PLAN_WPF_UI.md
- [ ] MIGRATION_EXAMPLES.md
- [ ] WPF UI API Reference (https://wpfui.lepo.co/api/)

### 10.4 Team communication

- [ ] Powiadomić zespół o rozpoczęciu migracji
- [ ] Ustalić code review process
- [ ] Ustalić testing protocol
- [ ] Daily standup (jeśli zespołowy)

---

## 11. Metryki przed migracją (Baseline)

### 11.1 Metryki kodu

| Metryka | Wartość |
|---------|---------|
| Całkowite linie XAML | 2,597 |
| Linie NeumorphicStyles.xaml | 643 (24.8%) |
| Linie pozostałych XAML | 1,954 |
| Kontrolki z x:Name | 65+ |
| Użycia stylów neumorficznych | ~150 |
| ThemeManager wywołania | 8 w 6 plikach |
| Event handlery | ~60 |

### 11.2 Metryki wydajności (do zmierzenia)

- [ ] **Czas uruchomienia aplikacji:** _______ ms
- [ ] **Zużycie pamięci (startup):** _______ MB
- [ ] **Zużycie pamięci (idle):** _______ MB
- [ ] **Czas otwarcia SettingsWindow:** _______ ms
- [ ] **Czas otwarcia ScreenshotHistoryWindow:** _______ ms
- [ ] **FPS (animacje, jeśli są):** _______ fps
- [ ] **Rozmiar build (Release):** _______ MB

*(Wypełnić przed migracją jako baseline dla porównania)*

---

## 12. Ryzyka i mitygacje

### 12.1 Ryzyka techniczne

| Ryzyko | Prawdopodobieństwo | Wpływ | Mitygacja |
|--------|-------------------|-------|-----------|
| Breaking changes w kontrolkach | Średnie | Wysoki | Testy każdego okna po migracji |
| Theme switching nie działa | Niskie | Bardzo wysoki | Priorytetowe testy ThemeService |
| Performance degradation | Niskie | Średni | Benchmark przed/po |
| Regresje w istniejących funkcjach | Wysokie | Wysoki | Comprehensive testing |
| Custom title bar problemy | Średnie | Średni | Użycie `<ui:TitleBar>` zamiast custom |
| Event handlery przestają działać | Niskie | Wysoki | Code review, testy jednostkowe |
| Slider → NumberBox problemy | Średnie | Niski | Zachować Slider jako backup |
| MessageBox → Snackbar UX | Niskie | Średni | User testing |

### 12.2 Ryzyka biznesowe

| Ryzyko | Prawdopodobieństwo | Wpływ | Mitygacja |
|--------|-------------------|-------|-----------|
| Opóźnienie w dostawie | Średnie | Średni | Buffor czasowy (7 vs 5 tygodni) |
| Bugs w produkcji | Średnie | Wysoki | Beta testing, gradual rollout |
| User confusion (nowy UI) | Niskie | Niski | Release notes, help documentation |
| Rollback konieczny | Niskie | Bardzo wysoki | Feature branch, tagged commits |

---

## 13. Checklist kontrolny dla każdego okna

Kopiuj ten checklist dla każdego migrowanego okna:

### Okno: __________________

**Pre-migracja:**
- [ ] Przeczytać kod XAML
- [ ] Przeczytać kod code-behind
- [ ] Zidentyfikować wszystkie kontrolki z x:Name
- [ ] Zidentyfikować wszystkie event handlery
- [ ] Zidentyfikować wszystkie wywołania ThemeManager
- [ ] Utworzyć screenshot przed migracją (dla porównania)

**Migracja XAML:**
- [ ] Zamienić `<Window>` → `<ui:FluentWindow>`
- [ ] Dodać `xmlns:ui="http://schemas.lepo.co/wpfui/2022/xaml"`
- [ ] Zamienić custom title bar → `<ui:TitleBar>`
- [ ] Usunąć `WindowStyle="None"`, `AllowsTransparency="True"`
- [ ] Dodać `ExtendsContentIntoTitleBar="True"`, `WindowBackdropType="Mica"`
- [ ] Zamienić `NeumorphicPanel` → `<ui:Card>`
- [ ] Zamienić `NeumorphicRaisedButton` → `<ui:Button>`
- [ ] Zamienić `NeumorphicDepressedButton` → `<ui:Button Appearance="Primary">`
- [ ] Zamienić `NeumorphicCircularButton` → Custom style
- [ ] Zamienić `NeumorphicTextBox` → `<ui:TextBox>`
- [ ] Usunąć wszystkie `Style="{StaticResource ...}"` dla standardowych kontrolek
- [ ] Dodać `Spacing` do `StackPanel` gdzie można

**Migracja code-behind:**
- [ ] Zmienić bazę klasy: `Window` → `FluentWindow`
- [ ] Dodać `using Wpf.Ui.Controls;`
- [ ] Usunąć wywołania `ThemeManager.Instance.ApplyTheme(this)`
- [ ] Zamienić `ThemeManager.Instance.SetTheme` → `ThemeService.Instance.SetTheme`
- [ ] Zaktualizować ValueChanged handlery (jeśli Slider → NumberBox)
- [ ] Sprawdzić wszystkie event handlery

**Testing:**
- [ ] Kompilacja bez błędów
- [ ] Okno się otwiera
- [ ] Wszystkie kontrolki widoczne
- [ ] Wszystkie przyciski działają
- [ ] Theme switching działa (jeśli dotyczy)
- [ ] Localization działa (jeśli dotyczy)
- [ ] Zrzut ekranu po migracji
- [ ] Porównanie przed/po (wizualne)
- [ ] Performance check (szybkość otwarcia)

**Dokumentacja:**
- [ ] Commit z opisem zmian
- [ ] Aktualizacja TODO (jeśli były problemy)
- [ ] Note'y do code review (jeśli są wątpliwości)

---

## 14. Glossary (Słowniczek)

| Termin | Znaczenie |
|--------|-----------|
| **WPF UI** | Biblioteka UI dla WPF (lepo.co) |
| **Fluent Design** | System designu Microsoft (Windows 11) |
| **Neumorphic** | Obecny styl designu projektu (soft UI) |
| **FluentWindow** | Typ okna w WPF UI z Fluent design |
| **TitleBar** | Górna belka okna (kontrolka WPF UI) |
| **Mica** | Półprzezroczysty efekt tła Windows 11 |
| **Snackbar** | Powiadomienie typu toast (WPF UI) |
| **ContentDialog** | Modal dialog (WPF UI) |
| **Card** | Kontener z podniesionym efektem |
| **NumberBox** | Kontrolka do wprowadzania liczb |
| **InfoBar** | Permanentny komunikat informacyjny |
| **ThemeService** | Nowy serwis zastępujący ThemeManager |
| **ApplicationThemeManager** | Klasa WPF UI do zarządzania motywami |
| **Auto-styled** | Kontrolka automatycznie stylowana przez WPF UI |

---

## 15. FAQ - Najczęstsze pytania

### Q: Czy musimy migrować wszystkie okna naraz?
**A:** Nie. Migracja jest inkrementalna - możemy migrować okno po oknie.

### Q: Co jeśli Slider → NumberBox nie działa?
**A:** Zachowaj Slider - WPF UI automatycznie go wystyluje.

### Q: Czy musimy usunąć NeumorphicStyles.xaml od razu?
**A:** Nie. Możemy go usunąć po zakończeniu migracji wszystkich okien.

### Q: Czy ThemeManager musi być usunięty?
**A:** Tak, ale dopiero po utworzeniu ThemeService i aktualizacji wszystkich wywołań.

### Q: Co z custom title bar - jak przenieść funkcjonalność?
**A:** Używamy `<ui:TitleBar>` - ma wbudowane drag & drop, snap layouts, close button.

### Q: Czy MessageBox musi być zamieniony na Snackbar?
**A:** Nie od razu. Możemy to zrobić stopniowo. MessageBox będzie działać.

### Q: Co jeśli po migracji coś nie działa?
**A:** Rollback do poprzedniego commita (branch strategy) i analiza problemu.

### Q: Czy WPF UI wspiera .NET 10.0?
**A:** Tak, WPF UI 4.0.3 wspiera .NET 6.0+.

---

## 16. Kontakty i zasoby

### Dokumentacja WPF UI
- **Official Docs:** https://wpfui.lepo.co/
- **API Reference:** https://wpfui.lepo.co/api/
- **GitHub:** https://github.com/lepoco/wpfui
- **Discussions:** https://github.com/lepoco/wpfui/discussions

### Wsparcie
- **GitHub Issues:** https://github.com/lepoco/wpfui/issues
- **Stack Overflow:** Tag `wpf-ui`

### Przykłady
- **WPF UI Gallery (source):** https://github.com/lepoco/wpfui/tree/main/samples
- **Community examples:** GitHub search "wpfui"

---

## 17. Historia zmian dokumentu

| Data | Wersja | Autor | Zmiany |
|------|--------|-------|--------|
| 2025-01-15 | 1.0 | Claude AI | Utworzenie dokumentu bazowego |

---

**KONIEC BAZY WIEDZY**

**Status:** ✅ Gotowe do rozpoczęcia migracji
**Następny krok:** Sprint 1 - Instalacja pakietów i konfiguracja

---

*Dokument żywy - aktualizuj podczas migracji gdy odkrywasz nowe informacje!*
