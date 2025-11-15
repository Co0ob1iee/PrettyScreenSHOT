# Struktura projektu PrettyScreenSHOT

Ten dokument opisuje strukturę projektu, organizację kodu i główne komponenty aplikacji.

## Struktura katalogów

```
PrettyScreenSHOT/
├── .github/                    # GitHub Actions i konfiguracja
├── .vscode/                    # Konfiguracja Visual Studio Code
├── Assets/                     # Zasoby graficzne
├── bin/                        # Skompilowane pliki binarne
├── docs/                       # Dokumentacja
│   ├── archive/               # Archiwum notatek deweloperskich
│   └── installation/          # Dokumentacja instalacji
├── obj/                        # Pliki obiektowe kompilacji
├── Properties/                 # Zasoby aplikacji i pliki lokalizacji
│   ├── Resources.resx         # Domyślne tłumaczenia (EN)
│   ├── Resources.pl.resx      # Tłumaczenia polskie
│   ├── Resources.de.resx      # Tłumaczenia niemieckie
│   ├── Resources.zh.resx      # Tłumaczenia chińskie
│   └── Resources.fr.resx      # Tłumaczenia francuskie
├── *.cs                        # Pliki źródłowe C#
├── *.xaml                      # Pliki definicji UI
├── *.xaml.cs                   # Code-behind dla XAML
├── .editorconfig              # Konfiguracja formatowania kodu
├── .gitattributes             # Atrybuty Git
├── .gitignore                 # Ignorowane pliki Git
├── app.ico                     # Ikona aplikacji
├── PrettyScreenSHOT.csproj    # Plik projektu
└── LICENSE                     # Licencja GNU GPL v3
```

## Główne komponenty

### 🎯 Aplikacja i zarządzanie

- **App.xaml / App.xaml.cs** - Punkt wejścia aplikacji, inicjalizacja i globalne zasoby
- **AssemblyInfo.cs** - Informacje o zestawie
- **TrayIconManager.cs** - Zarządzanie ikoną w zasobniku systemowym

### 📸 Przechwytywanie screenshotów

- **ScreenshotHelper.cs** - Podstawowa logika przechwytywania ekranu
- **ScreenshotManager.cs** - Zarządzanie historią i zapisywaniem screenshotów
- **ScreenshotOverlay.xaml/.cs** - Nakładka do wyboru obszaru przechwytywania
- **ScrollCaptureHelper.cs** - Przechwytywanie przewijanych stron

### 🎨 Edytor

- **ScreenshotEditorWindow.xaml/.cs** - Główne okno edytora z narzędziami do edycji
  - Marker, Prostokąt, Strzałka, Blur, Tekst
  - Wybór koloru, grubość linii
  - Cofanie i czyszczenie
- **TextInputWindow.xaml/.cs** - Okno wprowadzania tekstu z zaawansowanymi opcjami

### 📚 Historia i zarządzanie

- **ScreenshotHistoryWindow.xaml/.cs** - Okno historii z miniaturami i zarządzaniem screenshotami
- **SaveScreenshotDialog.xaml/.cs** - Dialog zapisu screenshotu

### ⚙️ Ustawienia

- **SettingsWindow.xaml/.cs** - Okno ustawień aplikacji
- **SettingsManager.cs** - Zarządzanie konfiguracją aplikacji
- **ThemeManager.cs** - Zarządzanie motywami (Dark/Light)
- **NeumorphicStyles.xaml** - Style neumorficzne dla interfejsu

### 🌍 Lokalizacja

- **LocalizationHelper.cs** - Pomocnik do zarządzania tłumaczeniami
- **Properties/Resources.*.resx** - Pliki zasobów dla różnych języków
  - Angielski (domyślny)
  - Polski
  - Niemiecki
  - Chiński
  - Francuski

### ☁️ Upload do chmury

- **CloudUploadManager.cs** - Główny menedżer uploadów
- **ICloudUploadProvider.cs** - Interfejs dla providerów
- **ImgurUploadProvider.cs** - Provider dla Imgur
- **CloudinaryUploadProvider.cs** - Provider dla Cloudinary
- **S3UploadProvider.cs** - Provider dla AWS S3
- **CustomServerUploadProvider.cs** - Provider dla własnego serwera

### 🎬 Nagrywanie wideo

- **VideoCaptureManager.cs** - Zarządzanie nagrywaniem (GIF/MP4)
- **VideoCaptureWindow.xaml/.cs** - Okno kontroli nagrywania

### 🔄 Aktualizacje

- **UpdateManager.cs** - Główny menedżer aktualizacji
- **UpdateChecker.cs** - Sprawdzanie dostępności aktualizacji
- **UpdateDownloader.cs** - Pobieranie aktualizacji
- **UpdateInstaller.cs** - Instalacja aktualizacji
- **UpdateInfo.cs** - Model danych o aktualizacji
- **UpdateWindow.xaml/.cs** - Okno informacji o aktualizacji
- **UpdateNotificationWindow.xaml/.cs** - Powiadomienie o dostępnej aktualizacji
- **UpdateProgressWindow.xaml/.cs** - Okno postępu pobierania

### 🔒 Bezpieczeństwo

- **SecurityManager.cs** - Szyfrowanie, znaki wodne, usuwanie metadanych
  - AES-256 encryption
  - PBKDF2 key derivation
  - Watermarking (tekst i obraz)
  - EXIF metadata removal

### ⚡ Optymalizacja

- **PerformanceOptimizer.cs** - Optymalizacja wydajności
  - Cache miniatur
  - Lazy loading
  - Kompresja obrazów
  - Zarządzanie pamięcią

### ⌨️ Skróty klawiszowe

- **KeyboardShortcutsManager.cs** - Globalne skróty klawiszowe i hotkeys

### 🔍 Wyszukiwanie i filtrowanie

- **SearchAndFilterManager.cs** - Wyszukiwanie i filtrowanie w historii

### 🛠️ Narzędzia pomocnicze

- **DebugHelper.cs** - Narzędzia debugowania

## Technologie i biblioteki

### Framework i język
- **.NET 10.0** - Framework aplikacji
- **C# 12** - Język programowania
- **WPF (Windows Presentation Foundation)** - Framework UI

### Biblioteki zewnętrzne
- **Magick.NET-Q16-AnyCPU** (v14.9.1) - Przetwarzanie obrazów i GIF
- **System.Drawing.Common** (v10.0.0) - Dodatkowa obsługa grafiki
- **System.Windows.Forms** (Framework) - Ikona w tray i dialogi

### API i protokoły
- **WinAPI** - Przechwytywanie ekranu i keyboard hooks
- **HTTP/HTTPS** - Upload do chmury i sprawdzanie aktualizacji
- **AWS S3 API** - Upload do S3
- **Imgur API** - Upload do Imgur
- **Cloudinary API** - Upload do Cloudinary

## Wzorce projektowe i architektura

### Wzorce używane w projekcie:

1. **MVVM (Model-View-ViewModel)** - częściowo
   - Separacja logiki biznesowej od UI
   - Data binding w XAML

2. **Singleton**
   - `SettingsManager` - jedna instancja ustawień
   - `TrayIconManager` - jedna ikona w tray

3. **Factory Pattern**
   - `CloudUploadManager` - tworzenie odpowiedniego providera

4. **Strategy Pattern**
   - `ICloudUploadProvider` - różne strategie uploadowania

5. **Observer Pattern**
   - Eventy WPF
   - PropertyChanged notifications

## Przepływ danych

### Przechwytywanie screenshotu
```
Hotkey (PRTSCN)
    ↓
KeyboardShortcutsManager
    ↓
ScreenshotOverlay (wybór obszaru)
    ↓
ScreenshotHelper (przechwycenie)
    ↓
ScreenshotManager (zapis + historia)
    ↓
ScreenshotHistoryWindow (podgląd)
```

### Edycja screenshotu
```
ScreenshotHistoryWindow (wybór)
    ↓
ScreenshotEditorWindow
    ↓
Narzędzia edycji (Marker, Rect, Arrow, Blur, Text)
    ↓
SaveScreenshotDialog
    ↓
ScreenshotManager (zapis)
    ↓
CloudUploadManager (opcjonalnie)
```

### Upload do chmury
```
ScreenshotManager (auto-upload) / ScreenshotHistoryWindow (manual)
    ↓
CloudUploadManager
    ↓
ICloudUploadProvider (Imgur/Cloudinary/S3/Custom)
    ↓
HTTP Request
    ↓
Cloud Service
    ↓
URL zwrócony do schowka
```

## Zarządzanie pamięcią

### IDisposable implementation
Większość komponentów implementuje `IDisposable` dla prawidłowego zarządzania zasobami:
- Bitmaps
- Streams
- GDI+ objects
- HTTP clients

### Optymalizacja
- **Lazy loading** - ładowanie miniatur na żądanie
- **Cache** - inteligentne cache'owanie miniatur
- **Kompresja** - automatyczna kompresja dla dużych obrazów
- **Garbage Collection** - wymuszanie GC po intensywnych operacjach

## Lokalizacja

### Struktura tłumaczeń
Wszystkie teksty UI są przechowywane w plikach `.resx`:
```
Properties/
├── Resources.resx       (EN - default)
├── Resources.pl.resx    (Polski)
├── Resources.de.resx    (Deutsch)
├── Resources.zh.resx    (中文)
└── Resources.fr.resx    (Français)
```

### Użycie
```csharp
LocalizationHelper.GetString("KeyName")
```

## Konfiguracja

### Ustawienia użytkownika
Przechowywane w:
```
%AppData%\PrettyScreenSHOT\settings.json
```

### Domyślna lokalizacja screenshotów
```
%UserProfile%\Pictures\PrettyScreenSHOT\
```

## Build i deployment

### Kompilacja
```bash
dotnet build
```

### Publikacja
```bash
dotnet publish -c Release -r win-x64
```

### Instalatory
- **Inno Setup** - `Installer.iss` dla .exe instalatora
- **WiX Toolset** - `Installer.wxs` dla .msi instalatora
- **MSIX** - `Package.appxmanifest` dla Microsoft Store

## Testing

Projekt obecnie nie ma automatycznych testów, ale jest to planowane w przyszłości (zobacz [ROADMAP.md](../ROADMAP.md)).

## Wkład w projekt

Zobacz [CONTRIBUTING.md](../CONTRIBUTING.md) dla szczegółowych wytycznych dotyczących współpracy.

## Licencja

GNU General Public License v3.0 - zobacz [LICENSE](../LICENSE)

---

**Ostatnia aktualizacja:** 2025-11-15
