# Plan implementacji Auto-Update dla PrettyScreenSHOT 🔄

## 📋 Przegląd

System automatycznych aktualizacji pozwoli użytkownikom na łatwe otrzymywanie najnowszych wersji aplikacji bez konieczności ręcznego pobierania i instalacji.

---

## 🎯 Cele

1. **Automatyczne sprawdzanie** dostępności nowych wersji
2. **Powiadomienia** o dostępnych aktualizacjach
3. **Pobieranie** aktualizacji w tle
4. **Instalacja** z zachowaniem ustawień
5. **Obsługa** zarówno EXE jak i MSIX instalatorów

---

## 🏗️ Architektura

### Komponenty:

1. **UpdateChecker** - sprawdzanie dostępności aktualizacji
2. **UpdateDownloader** - pobieranie plików instalatorów
3. **UpdateInstaller** - instalacja aktualizacji
4. **UpdateSettings** - konfiguracja w SettingsManager

### Źródło aktualizacji:

- **GitHub Releases API** - główne źródło
- Format: `https://api.github.com/repos/username/PrettyScreenSHOT/releases/latest`
- Pliki: `PrettyScreenSHOT-Setup-v*.exe` lub `PrettyScreenSHOT_*_x64.msix`

---

## 📐 Struktura implementacji

### 1. UpdateChecker.cs

```csharp
public class UpdateChecker
{
    private const string GitHubApiUrl = "https://api.github.com/repos/{owner}/{repo}/releases/latest";
    
    public async Task<UpdateInfo?> CheckForUpdatesAsync()
    {
        // Sprawdź GitHub Releases API
        // Porównaj wersje
        // Zwróć UpdateInfo jeśli dostępna nowa wersja
    }
    
    public class UpdateInfo
    {
        public string Version { get; set; }
        public string DownloadUrl { get; set; }
        public string ReleaseNotes { get; set; }
        public DateTime ReleaseDate { get; set; }
        public long FileSize { get; set; }
    }
}
```

### 2. UpdateDownloader.cs

```csharp
public class UpdateDownloader
{
    public async Task<string> DownloadUpdateAsync(string url, IProgress<double> progress)
    {
        // Pobierz plik instalatora
        // Zapisz do temp folder
        // Zwróć ścieżkę do pobranego pliku
    }
}
```

### 3. UpdateInstaller.cs

```csharp
public class UpdateInstaller
{
    public void InstallUpdate(string installerPath, bool restartAfterInstall = true)
    {
        // Uruchom instalator
        // Zamknij aplikację
        // Poinstaluj, uruchom ponownie
    }
}
```

### 4. UpdateManager.cs (Główny manager)

```csharp
public class UpdateManager
{
    private UpdateChecker checker;
    private UpdateDownloader downloader;
    private UpdateInstaller installer;
    
    public async Task CheckAndNotifyAsync()
    {
        // Sprawdź aktualizacje
        // Jeśli dostępna, pokaż powiadomienie
    }
    
    public async Task DownloadAndInstallAsync(UpdateInfo updateInfo)
    {
        // Pobierz aktualizację
        // Zainstaluj
        // Uruchom ponownie
    }
}
```

---

## ⚙️ Konfiguracja w SettingsManager

### Nowe właściwości:

```csharp
// Auto-Update Settings
public bool EnableAutoUpdate { get; set; } = true;
public int UpdateCheckIntervalHours { get; set; } = 24;
public bool CheckForUpdatesOnStartup { get; set; } = true;
public bool ShowUpdateNotifications { get; set; } = true;
public string UpdateChannel { get; set; } = "stable"; // stable, beta, alpha
public DateTime LastUpdateCheck { get; set; }
public string SkippedVersion { get; set; } = ""; // Wersja, którą użytkownik pominął
```

---

## 🖥️ UI - Okno aktualizacji

### UpdateWindow.xaml

Okno z:
- Informacją o dostępnej aktualizacji
- Wersją aktualnej i nowej wersji
- Release notes
- Przyciski: "Download Now", "Later", "Skip This Version"
- Progress bar podczas pobierania

---

## 🔄 Workflow

### 1. Sprawdzanie aktualizacji

```
Startup / Timer → UpdateManager.CheckAndNotifyAsync()
    ↓
UpdateChecker.CheckForUpdatesAsync()
    ↓
GitHub API Request
    ↓
Parse Response
    ↓
Compare Versions
    ↓
If New Version Available:
    → Show Notification
    → UpdateWindow
```

### 2. Pobieranie i instalacja

```
User clicks "Download Now"
    ↓
UpdateDownloader.DownloadUpdateAsync()
    ↓
Show Progress
    ↓
Download Complete
    ↓
UpdateInstaller.InstallUpdate()
    ↓
Close Application
    ↓
Run Installer
    ↓
Restart Application
```

---

## 📝 Implementacja krok po kroku

### Krok 1: Utwórz klasy podstawowe

1. `UpdateInfo.cs` - model danych aktualizacji
2. `UpdateChecker.cs` - sprawdzanie GitHub API
3. `UpdateDownloader.cs` - pobieranie plików
4. `UpdateInstaller.cs` - instalacja

### Krok 2: UpdateManager

1. Integracja wszystkich komponentów
2. Timer do okresowego sprawdzania
3. Obsługa zdarzeń

### Krok 3: UI

1. `UpdateWindow.xaml` - okno powiadomienia
2. Integracja z SettingsWindow
3. Tray icon notifications

### Krok 4: Integracja

1. Inicjalizacja w `App.xaml.cs`
2. Timer w `TrayIconManager`
3. Ustawienia w `SettingsManager`

---

## 🔐 Bezpieczeństwo

### Weryfikacja:

1. **Sprawdzanie sumy kontrolnej** (SHA256) pliku
2. **Weryfikacja podpisu** (jeśli certyfikat Code Signing)
3. **HTTPS only** - tylko bezpieczne połączenia
4. **Timeout** - maksymalny czas pobierania

### Obsługa błędów:

- Retry mechanism dla pobierania
- Fallback jeśli GitHub API niedostępny
- Logowanie wszystkich operacji

---

## 📊 Wersjonowanie

### Format wersji:

- **Semantic Versioning**: `MAJOR.MINOR.PATCH` (np. `1.2.3`)
- **GitHub Releases**: Tag `v1.2.3`
- **Assembly Version**: W `AssemblyInfo.cs`

### Porównywanie wersji:

```csharp
public static bool IsNewerVersion(string currentVersion, string newVersion)
{
    var current = Version.Parse(currentVersion);
    var newer = Version.Parse(newVersion);
    return newer > current;
}
```

---

## 🧪 Testowanie

### Scenariusze testowe:

1. ✅ Sprawdzanie aktualizacji przy starcie
2. ✅ Powiadomienie o dostępnej aktualizacji
3. ✅ Pobieranie aktualizacji
4. ✅ Instalacja aktualizacji
5. ✅ Pomijanie wersji
6. ✅ Wyłączone auto-update
7. ✅ Błąd pobierania (retry)
8. ✅ Brak połączenia z internetem

---

## 📦 Pliki do utworzenia

1. `UpdateInfo.cs` - model danych
2. `UpdateChecker.cs` - sprawdzanie aktualizacji
3. `UpdateDownloader.cs` - pobieranie
4. `UpdateInstaller.cs` - instalacja
5. `UpdateManager.cs` - główny manager
6. `UpdateWindow.xaml/.cs` - UI
7. Aktualizacja `SettingsManager.cs`
8. Aktualizacja `SettingsWindow.xaml/.cs`

---

## 🚀 Deployment

### GitHub Releases:

1. Utwórz **Release** na GitHub
2. Tag: `v1.2.3` (semantic versioning)
3. Upload: `PrettyScreenSHOT-Setup-v1.2.3.exe`
4. Release notes w opisie

### Automatyzacja:

- GitHub Actions do automatycznego tworzenia releases
- Automatyczne uploadowanie instalatorów

---

## 📈 Metryki (opcjonalnie)

- Liczba użytkowników z włączonym auto-update
- Czas od wydania do aktualizacji
- Wskaźnik sukcesu instalacji
- Najczęstsze błędy

---

## ⏱️ Timeline implementacji

1. **Faza 1** (1-2 dni): Podstawowe klasy (UpdateChecker, UpdateDownloader)
2. **Faza 2** (1 dzień): UpdateManager i integracja
3. **Faza 3** (1 dzień): UI (UpdateWindow)
4. **Faza 4** (1 dzień): Testowanie i poprawki
5. **Faza 5** (0.5 dnia): Dokumentacja

**Łącznie: ~5 dni roboczych**

---

## 📚 Zależności

- **Newtonsoft.Json** lub **System.Text.Json** - parsowanie GitHub API
- **System.Net.Http** - pobieranie plików
- **System.Diagnostics** - uruchamianie instalatora

---

**Gotowe do implementacji! 🎉**
