# Raport: Braki w lokalizacji tekstów

## 📋 Podsumowanie
Znaleziono **wiele hardcoded tekstów**, które nie są zlokalizowane. Poniżej lista wszystkich brakujących tłumaczeń.

---

## 🔴 Krytyczne braki (hardcoded w kodzie C#)

### ScreenshotEditorWindow.xaml.cs

1. **Linia 352-356**: Dialog uploadu
   ```csharp
   "Cloud provider nie jest skonfigurowany.\n\nPrzejdź do ustawień..."
   "Brak konfiguracji"
   ```
   **Klucze:** `Editor_CloudNotConfigured`, `Editor_CloudNotConfiguredTitle`

2. **Linia 374**: Status uploadu
   ```csharp
   Text = "UPLOADING..."
   ```
   **Klucz:** `Editor_Uploading`

3. **Linia 384**: Przycisk upload
   ```csharp
   Text = "UPLOAD"
   ```
   **Klucz:** `Editor_Upload` (już istnieje w Resources?)

4. **Linia 393**: Sukces uploadu
   ```csharp
   System.Windows.MessageBox.Show(message, "Sukces", ...)
   ```
   **Klucz:** `Editor_UploadSuccess`, `Editor_UploadSuccessMessage`

5. **Linia 400**: Błąd uploadu
   ```csharp
   System.Windows.MessageBox.Show($"Błąd uploadu:\n\n{errorMsg}", "Błąd", ...)
   ```
   **Klucz:** `Editor_UploadError`, `Editor_UploadErrorMessage`

6. **Linia 407**: Ogólny błąd
   ```csharp
   System.Windows.MessageBox.Show($"Błąd: {ex.Message}", "Błąd", ...)
   ```
   **Klucz:** `Editor_Error` (już istnieje), `Editor_ErrorWithMessage`

7. **Linia 460**: Potwierdzenie wyczyszczenia
   ```csharp
   System.Windows.MessageBox.Show("Czy na pewno chcesz wyczyścić wszystkie zmiany?", "Potwierdzenie", ...)
   ```
   **Klucz:** `Editor_ClearConfirm` (już istnieje), `Editor_Confirm` (już istnieje)

8. **Linia 516**: Etykieta rozmiaru
   ```csharp
   SizeLabel.Text = $"{(int)e.NewValue}px"
   ```
   **Klucz:** `Editor_SizeLabel` (format: "{0}px")

---

### ScreenshotHistoryWindow.xaml.cs

1. **Linia 96**: Status uploadu
   ```csharp
   Text = "Uploading..."
   ```
   **Klucz:** `History_Uploading`

2. **Linia 108**: Przycisk upload
   ```csharp
   Text = "Upload"
   ```
   **Klucz:** `History_Upload`

3. **Linia 115**: Sukces uploadu
   ```csharp
   System.Windows.MessageBox.Show($"Upload successful!\n\nURL: {result.Url}\n\nURL copied to clipboard.", "Success", ...)
   ```
   **Klucz:** `History_UploadSuccess`, `History_UploadSuccessMessage`, `History_Success`

4. **Linia 120**: Błąd uploadu
   ```csharp
   System.Windows.MessageBox.Show($"Upload failed:\n\n{result.ErrorMessage}", "Error", ...)
   ```
   **Klucz:** `History_UploadError`, `History_UploadErrorMessage`, `History_Error`

5. **Linia 126**: Ogólny błąd
   ```csharp
   System.Windows.MessageBox.Show($"Error: {ex.Message}", "Error", ...)
   ```
   **Klucz:** `History_Error`, `History_ErrorWithMessage`

6. **Linia 136**: URL skopiowany
   ```csharp
   System.Windows.MessageBox.Show($"URL copied to clipboard:\n\n{item.CloudUrl}", "Cloud URL", ...)
   ```
   **Klucz:** `History_UrlCopied`, `History_CloudUrlTitle`

---

### SettingsWindow.xaml.cs

1. **Linia 155**: Nieprawidłowa ścieżka
   ```csharp
   System.Windows.MessageBox.Show("Invalid save path!", "Error", ...)
   ```
   **Klucz:** `Settings_InvalidPath`, `Settings_Error`

2. **Linia 184**: Ustawienia zapisane
   ```csharp
   System.Windows.MessageBox.Show("Settings saved successfully!", "Success", ...)
   ```
   **Klucz:** `Settings_SaveSuccess`, `Settings_Success`

3. **Linia 190**: Błąd zapisu
   ```csharp
   System.Windows.MessageBox.Show($"Error saving settings: {ex.Message}", "Error", ...)
   ```
   **Klucz:** `Settings_SaveError`, `Settings_ErrorWithMessage`

---

### SaveScreenshotDialog.xaml.cs

**Brak hardcoded tekstów** - wszystko jest w XAML

---

## 🟡 Braki w XAML (hardcoded teksty)

### SaveScreenshotDialog.xaml

1. **Linia 4**: Tytuł okna
   ```xml
   Title="Save Screenshot"
   ```
   **Klucz:** `SaveDialog_Title`

2. **Linia 10**: Etykieta kategorii
   ```xml
   Text="Category:"
   ```
   **Klucz:** `SaveDialog_Category`

3. **Linia 21**: Etykieta tagów
   ```xml
   Text="Tags (comma separated):"
   ```
   **Klucz:** `SaveDialog_Tags`

4. **Linia 24**: Placeholder tagów
   ```xml
   Tag="e.g., work, important, bug"
   ```
   **Klucz:** `SaveDialog_TagsPlaceholder`

5. **Linia 27**: Etykieta notatek
   ```xml
   Text="Notes:"
   ```
   **Klucz:** `SaveDialog_Notes`

6. **Linia 36**: Przycisk Anuluj
   ```xml
   Text="Cancel"
   ```
   **Klucz:** `SaveDialog_Cancel`

7. **Linia 40**: Przycisk Zapisz
   ```xml
   Text="Save"
   ```
   **Klucz:** `SaveDialog_Save`

---

### ScreenshotEditorWindow.xaml

1. **Linia 48**: Etykieta narzędzi
   ```xml
   Text="NARZEDZIA:"
   ```
   **Klucz:** `Editor_Tools` (już istnieje w Resources)

2. **Linia 50-68**: Przyciski narzędzi
   ```xml
   Text="MARKER", "RECT", "ARROW", "BLUR", "TEXT"
   ```
   **Klucze:** `Editor_Marker`, `Editor_Rectangle`, `Editor_Arrow`, `Editor_Blur`, `Editor_Text` (już istnieją)

3. **Linia 73**: Etykieta koloru
   ```xml
   Text="KOLOR:"
   ```
   **Klucz:** `Editor_Color` (już istnieje)

4. **Linia 80**: Etykieta grubości
   ```xml
   Text="GRUBOSC:"
   ```
   **Klucz:** `Editor_Thickness` (już istnieje)

5. **Linia 83**: Etykieta rozmiaru
   ```xml
   Text="3px"
   ```
   **Klucz:** `Editor_SizeLabel` (format)

6. **Linia 88-94**: Przyciski akcji
   ```xml
   Text="CLEAR", "UNDO"
   ```
   **Klucze:** `Editor_Clear`, `Editor_Undo` (już istnieją)

7. **Linia 98-108**: Przyciski główne
   ```xml
   Text="UPLOAD", "ZAPISZ", "ANULUJ"
   ```
   **Klucze:** `Editor_Upload`, `Editor_Save`, `Editor_Cancel` (już istnieją)

8. **Tooltips**: Wszystkie tooltips są hardcoded
   - `ToolTip="Marker - Rysuj"` → `Editor_Tooltip_Marker`
   - `ToolTip="Prostokat"` → `Editor_Tooltip_Rectangle`
   - `ToolTip="Strzalka"` → `Editor_Tooltip_Arrow`
   - `ToolTip="Blur - Zamazywanie"` → `Editor_Tooltip_Blur`
   - `ToolTip="Text"` → `Editor_Tooltip_Text`
   - `ToolTip="Zmien kolor"` → `Editor_Tooltip_Color`
   - `ToolTip="Wyczysc wszystko"` → `Editor_Tooltip_Clear`
   - `ToolTip="Cofnij ostatnia akcje"` → `Editor_Tooltip_Undo`
   - `ToolTip="Upload do chmury"` → `Editor_Tooltip_Upload`
   - `ToolTip="Zapisz screenshot"` → `Editor_Tooltip_Save`
   - `ToolTip="Anuluj bez zapisu"` → `Editor_Tooltip_Cancel`

---

### ScreenshotHistoryWindow.xaml

1. **Linia 40**: Emoji/ikona (może zostać)
   ```xml
   Text="??"
   ```

2. **Linia 57-61**: Placeholder wyszukiwania
   ```xml
   Tag="Search screenshots..."
   ```
   **Klucz:** `History_SearchPlaceholder`

3. **Linia 76**: Przycisk Clear
   ```xml
   Content="Clear"
   ```
   **Klucz:** `History_ClearFilters`

4. **Linia 87**: Przycisk Upload
   ```xml
   Text="Upload"
   ```
   **Klucz:** `History_Upload`

5. **Linia 94**: Przycisk Delete
   ```xml
   Text="Delete"
   ```
   **Klucz:** `History_Delete` (już istnieje)

---

### SettingsWindow.xaml

1. **Linia 40**: Podtytuł
   ```xml
   Text="Configure application settings"
   ```
   **Klucz:** `Settings_Subtitle`

2. **Linia 109**: Checkbox AutoUpload
   ```xml
   Content="Auto Upload"
   ```
   **Klucz:** `Settings_AutoUpload`

3. **Linia 118**: Etykieta Theme
   ```xml
   Text="Theme:"
   ```
   **Klucz:** `Settings_Theme`

---

## 📝 Lista wszystkich brakujących kluczy

### Nowe klucze do dodania:

#### Editor (ScreenshotEditorWindow)
- `Editor_CloudNotConfigured` - "Cloud provider nie jest skonfigurowany..."
- `Editor_CloudNotConfiguredTitle` - "Brak konfiguracji"
- `Editor_Uploading` - "UPLOADING..."
- `Editor_UploadSuccess` - "Upload zakończony pomyślnie!"
- `Editor_UploadSuccessMessage` - "URL: {0}\n\nURL został skopiowany do schowka."
- `Editor_UploadError` - "Błąd uploadu"
- `Editor_UploadErrorMessage` - "Błąd uploadu:\n\n{0}"
- `Editor_ErrorWithMessage` - "Błąd: {0}"
- `Editor_SizeLabel` - "{0}px"
- `Editor_Tooltip_Marker` - "Marker - Rysuj"
- `Editor_Tooltip_Rectangle` - "Prostokat"
- `Editor_Tooltip_Arrow` - "Strzalka"
- `Editor_Tooltip_Blur` - "Blur - Zamazywanie"
- `Editor_Tooltip_Text` - "Tekst"
- `Editor_Tooltip_Color` - "Zmien kolor"
- `Editor_Tooltip_Clear` - "Wyczysc wszystko"
- `Editor_Tooltip_Undo` - "Cofnij ostatnia akcje"
- `Editor_Tooltip_Upload` - "Upload do chmury"
- `Editor_Tooltip_Save` - "Zapisz screenshot"
- `Editor_Tooltip_Cancel` - "Anuluj bez zapisu"

#### History (ScreenshotHistoryWindow)
- `History_Uploading` - "Uploading..."
- `History_Upload` - "Upload"
- `History_UploadSuccess` - "Success"
- `History_UploadSuccessMessage` - "Upload successful!\n\nURL: {0}\n\nURL copied to clipboard."
- `History_UploadError` - "Error"
- `History_UploadErrorMessage` - "Upload failed:\n\n{0}"
- `History_Error` - "Error"
- `History_ErrorWithMessage` - "Error: {0}"
- `History_UrlCopied` - "URL copied to clipboard:\n\n{0}"
- `History_CloudUrlTitle` - "Cloud URL"
- `History_SearchPlaceholder` - "Search screenshots..."
- `History_ClearFilters` - "Clear"

#### Settings (SettingsWindow)
- `Settings_Subtitle` - "Configure application settings"
- `Settings_AutoUpload` - "Auto Upload"
- `Settings_Theme` - "Theme:"
- `Settings_InvalidPath` - "Invalid save path!"
- `Settings_SaveSuccess` - "Success"
- `Settings_SaveSuccessMessage` - "Settings saved successfully!"
- `Settings_SaveError` - "Error"
- `Settings_ErrorWithMessage` - "Error saving settings: {0}"

#### SaveDialog (SaveScreenshotDialog)
- `SaveDialog_Title` - "Save Screenshot"
- `SaveDialog_Category` - "Category:"
- `SaveDialog_Tags` - "Tags (comma separated):"
- `SaveDialog_TagsPlaceholder` - "e.g., work, important, bug"
- `SaveDialog_Notes` - "Notes:"
- `SaveDialog_Cancel` - "Cancel"
- `SaveDialog_Save` - "Save"

---

## ✅ Co już jest zlokalizowane

Następujące elementy **są już zlokalizowane**:
- Menu kontekstowe (TrayIconManager)
- Podstawowe etykiety w SettingsWindow
- Podstawowe etykiety w TextInputWindow
- Podstawowe etykiety w ScreenshotHistoryWindow
- Podstawowe etykiety w ScreenshotEditorWindow (częściowo)

---

## 🔧 Rekomendacje

1. **Dodać wszystkie brakujące klucze** do plików Resources.resx
2. **Zastąpić wszystkie hardcoded stringi** wywołaniami `LocalizationHelper.GetString()`
3. **Dodać tłumaczenia** do wszystkich plików językowych (pl, de, zh, fr)
4. **Użyć formatowania** dla tekstów z parametrami (np. `string.Format()`)
5. **Zaktualizować XAML** aby używał binding do zasobów zamiast hardcoded tekstów

---

**Ostatnia aktualizacja:** 2024
**Status:** Wymaga poprawy - wiele tekstów nie jest zlokalizowanych

