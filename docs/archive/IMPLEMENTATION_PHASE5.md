# Faza 5: Funkcje Techniczne - Dokumentacja Implementacji

## ✅ Zaimplementowane Funkcje

### 1. Optymalizacja Wydajności (`PerformanceOptimizer.cs`)

#### Funkcje:
- **Cache miniatur** - inteligentne cache'owanie miniatur screenshotów
- **Lazy loading** - asynchroniczne ładowanie miniatur w tle
- **Optymalizacja obrazów** - automatyczne zmniejszanie rozmiaru przed zapisem
- **Zarządzanie pamięcią** - automatyczne czyszczenie wygasłych wpisów

#### Użycie:
```csharp
// Tworzenie/zapytanie cache miniatury
var thumbnail = PerformanceOptimizer.GetOrCreateThumbnail(bitmap, cacheKey);

// Optymalizacja przed zapisem
var optimized = PerformanceOptimizer.OptimizeForSave(bitmap, maxWidth: 1920, maxHeight: 1080);

// Asynchroniczne ładowanie
var thumbnail = await PerformanceOptimizer.LoadThumbnailAsync(filePath);

// Statystyki cache
var stats = PerformanceOptimizer.GetCacheStatistics();
```

#### Konfiguracja:
- `MaxCacheSize` - maksymalna liczba cache'owanych miniatur (domyślnie: 50)
- `CacheExpirationMinutes` - czas wygaśnięcia cache (domyślnie: 30 minut)
- `ThumbnailSize` - rozmiar miniatury (domyślnie: 80px)

---

### 2. Video Capture (`VideoCaptureManager.cs`)

#### Funkcje:
- **Nagrywanie ekranu** - przechwytywanie klatek w czasie rzeczywistym
- **Eksport do GIF** - zapis jako animowany GIF
- **Eksport do MP4** - zapis jako MP4 (wymaga FFmpeg)
- **Konfigurowalny FPS** - regulacja liczby klatek na sekundę

#### Użycie:
```csharp
var videoManager = new VideoCaptureManager();

// Rozpocznij nagrywanie
var area = new Rectangle(x, y, width, height);
videoManager.StartRecording(area, fps: 10);

// Automatyczne nagrywanie klatek
await videoManager.RecordFramesAsync();

// Zatrzymaj i zapisz
string outputPath = await videoManager.StopRecordingAsync("output.gif", VideoFormat.GIF);
```

#### Ustawienia:
- `FrameRate` - liczba klatek na sekundę (1-30)
- `VideoFormat` - format wyjściowy (GIF/MP4)

#### Uwagi:
- MP4 wymaga FFmpeg (do zaimplementowania)
- GIF używa prostego encoder (dla pełnej funkcjonalności potrzebna biblioteka zewnętrzna)

---

### 3. Scroll Capture (`ScrollCaptureHelper.cs`)

#### Funkcje:
- **Automatyczne przewijanie** - automatyczne przewijanie strony
- **Łączenie screenshotów** - łączenie wielu screenshotów w jeden
- **Wykrywanie końca** - automatyczne wykrywanie końca przewijalnej zawartości
- **Obsługa pionowa i pozioma** - przewijanie w obu kierunkach

#### Użycie:
```csharp
var initialArea = new Rectangle(x, y, width, height);

// Przechwyć długą stronę
var longScreenshot = await ScrollCaptureHelper.CaptureScrollableAreaAsync(
    initialArea, 
    ScrollDirection.Vertical,
    maxScrolls: 20);
```

#### Parametry:
- `initialArea` - początkowy obszar do przechwycenia
- `direction` - kierunek przewijania (Vertical/Horizontal)
- `maxScrolls` - maksymalna liczba przewinięć (domyślnie: 20)

#### Uwagi:
- Wymaga aktywnego okna do przewijania
- Może nie działać z wszystkimi aplikacjami
- Porównanie screenshotów jest uproszczone (może wymagać ulepszeń)

---

### 4. Bezpieczeństwo (`SecurityManager.cs`)

#### Funkcje:
- **Szyfrowanie** - szyfrowanie screenshotów (AES)
- **Znak wodny tekstowy** - dodawanie tekstowego znaku wodnego
- **Znak wodny obrazowy** - dodawanie obrazowego znaku wodnego
- **Usuwanie metadanych** - usuwanie metadanych EXIF

#### Użycie:
```csharp
// Szyfrowanie
SecurityManager.EncryptScreenshot("input.png", "output.encrypted", password: "mypassword");

// Deszyfrowanie
SecurityManager.DecryptScreenshot("output.encrypted", "decrypted.png", password: "mypassword");

// Znak wodny tekstowy
var watermarked = SecurityManager.AddWatermark(
    bitmap, 
    "PrettyScreenSHOT", 
    WatermarkPosition.BottomRight,
    opacity: 0.5);

// Znak wodny obrazowy
var watermarked = SecurityManager.AddImageWatermark(
    bitmap,
    watermarkImage,
    WatermarkPosition.BottomRight,
    opacity: 0.5,
    scale: 0.2);

// Usuwanie metadanych
var cleanBitmap = SecurityManager.RemoveMetadata(bitmap);
```

#### Pozycje znaku wodnego:
- `TopLeft` - lewy górny róg
- `TopRight` - prawy górny róg
- `BottomLeft` - lewy dolny róg
- `BottomRight` - prawy dolny róg (domyślnie)
- `Center` - środek

#### Uwagi bezpieczeństwa:
- Domyślny klucz szyfrowania jest statyczny (dla produkcji użyj losowego klucza)
- Hasło jest hashowane przez SHA256
- Szyfrowanie używa AES (Advanced Encryption Standard)

---

## 🔧 Integracja z Istniejącym Kodem

### ScreenshotManager
- Używa `PerformanceOptimizer` do cache'owania miniatur
- Używa `SecurityManager` do watermarkingu i szyfrowania
- Automatyczna optymalizacja rozmiaru przed zapisem

### SettingsManager
- Dodane ustawienia dla wszystkich nowych funkcji
- Automatyczne zapisywanie przy zmianie ustawień

### TrayIconManager
- Dodane opcje menu: "Scroll Capture" i "Video Capture"
- Integracja z ScreenshotOverlay

---

## 📋 Konfiguracja w Ustawieniach

### Security
- `EnableEncryption` - włącz/wyłącz szyfrowanie
- `EnableWatermark` - włącz/wyłącz znak wodny
- `WatermarkText` - tekst znaku wodnego
- `RemoveMetadata` - usuń metadane EXIF

### Performance
- `EnableCache` - włącz/wyłącz cache
- `CacheSize` - rozmiar cache (liczba wpisów)
- `MaxImageWidth` - maksymalna szerokość (0 = bez limitu)
- `MaxImageHeight` - maksymalna wysokość (0 = bez limitu)

### Video Capture
- `EnableVideoCapture` - włącz/wyłącz video capture
- `VideoFrameRate` - liczba klatek na sekundę
- `VideoFormat` - format (GIF/MP4)

### Scroll Capture
- `EnableScrollCapture` - włącz/wyłącz scroll capture
- `MaxScrolls` - maksymalna liczba przewinięć

---

## 🚀 Przykłady Użycia

### Przykład 1: Optymalizacja wydajności
```csharp
// Automatyczna optymalizacja w ScreenshotManager
// Obrazy są automatycznie zmniejszane jeśli przekraczają limity
SettingsManager.Instance.MaxImageWidth = 1920;
SettingsManager.Instance.MaxImageHeight = 1080;
```

### Przykład 2: Video Capture
```csharp
var videoManager = new VideoCaptureManager();
videoManager.StartRecording(new Rectangle(0, 0, 1920, 1080), fps: 15);

// Nagraj przez 5 sekund
await Task.Delay(5000);

string output = await videoManager.StopRecordingAsync("recording.gif", VideoFormat.GIF);
```

### Przykład 3: Scroll Capture
```csharp
var area = new Rectangle(100, 100, 800, 600);
var longScreenshot = await ScrollCaptureHelper.CaptureScrollableAreaAsync(
    area, 
    ScrollDirection.Vertical,
    maxScrolls: 10);

ScreenshotManager.Instance.AddScreenshot(longScreenshot);
```

### Przykład 4: Bezpieczeństwo
```csharp
// Automatyczny watermark w ScreenshotManager
SettingsManager.Instance.EnableWatermark = true;
SettingsManager.Instance.WatermarkText = "Confidential";

// Szyfrowanie
SettingsManager.Instance.EnableEncryption = true;
```

---

## ⚠️ Uwagi i Ograniczenia

### Video Capture
- GIF encoder jest uproszczony - dla pełnej funkcjonalności potrzebna biblioteka (np. Magick.NET)
- MP4 wymaga FFmpeg - do zaimplementowania
- Wysokie FPS mogą powodować duże pliki

### Scroll Capture
- Może nie działać z wszystkimi aplikacjami
- Porównanie screenshotów jest uproszczone
- Wymaga aktywnego okna do przewijania

### Bezpieczeństwo
- Domyślny klucz szyfrowania jest statyczny - dla produkcji użyj losowego klucza
- Hasło powinno być silne (min. 8 znaków)
- Znak wodny może być usunięty przez zaawansowanych użytkowników

### Performance
- Cache może zużywać pamięć - regularnie czyść wygasłe wpisy
- Optymalizacja obrazów może zmienić jakość

---

## 🔄 Następne Kroki

### Krótkoterminowe:
1. Dodaj pełną obsługę animowanego GIF (biblioteka zewnętrzna)
2. Zaimplementuj MP4 export z FFmpeg
3. Ulepsz porównanie screenshotów w Scroll Capture

### Średnioterminowe:
1. Dodaj UI dla video capture (okno kontroli)
2. Dodaj UI dla scroll capture (wybór kierunku)
3. Ulepsz bezpieczeństwo (losowe klucze, key derivation)

### Długoterminowe:
1. GPU acceleration dla przetwarzania obrazów
2. Zaawansowane algorytmy wykrywania zmian w Scroll Capture
3. Integracja z zewnętrznymi bibliotekami (FFmpeg, ImageMagick)

---

**Ostatnia aktualizacja:** 2025-01-14
**Wersja:** 1.0

