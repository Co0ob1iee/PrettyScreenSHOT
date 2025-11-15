# PrettyScreenSHOT 📸

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-GPL%20v3-blue)](LICENSE)
[![Platform](https://img.shields.io/badge/platform-Windows-0078D6?logo=windows)](https://www.microsoft.com/windows)

Zaawansowana aplikacja do przechwytywania i edycji screenshotów dla Windows, napisana w C# WPF.

## ✨ Funkcje

### 🎯 Przechwytywanie
- **Region Selection** - wybór obszaru do przechwycenia
- **Multi-Monitor Support** - obsługa wielu monitorów jednocześnie
- **Global Hotkey** - skrót klawiszowy (domyślnie PRTSCN)
- **Automatic Clipboard** - automatyczne kopiowanie do schowka

### 🎨 Edytor
- **Marker** - rysowanie markerem
- **Prostokąt** - rysowanie prostokątów
- **Strzałka** - rysowanie strzałek
- **Blur** - rozmywanie obszarów (Gaussian blur)
- **Tekst** - dodawanie tekstu z wyborem czcionki i rozmiaru
- **Kolor** - wybór koloru z palety
- **Grubość** - regulacja grubości linii
- **Undo/Clear** - cofanie i czyszczenie zmian

### 📚 Historia
- **Automatyczne zapisywanie** - wszystkie screenshoty są zapisywane
- **Miniatury** - podgląd w historii
- **Usuwanie** - łatwe usuwanie screenshotów
- **Cloud Upload** - upload do chmury (Imgur, Cloudinary, S3, Custom)

### 🌍 Wielojęzyczność
- 🇵🇱 Polski
- 🇬🇧 English
- 🇩🇪 Deutsch
- 🇨🇳 中文 (Mandaryński)
- 🇫🇷 Français

### ⚙️ Ustawienia
- **Język** - wybór języka interfejsu
- **Ścieżka zapisu** - konfigurowalna lokalizacja plików
- **Hotkey** - konfiguracja skrótu klawiszowego
- **Format obrazu** - PNG, JPG, BMP
- **Jakość** - regulacja jakości (10-100%)
- **Auto Save** - automatyczne zapisywanie
- **Copy to Clipboard** - kopiowanie do schowka
- **Show Notifications** - wyświetlanie powiadomień
- **Theme** - motywy kolorystyczne (Dark/Light)

### 🎬 Video Capture
- **GIF Recording** - nagrywanie animowanych GIF (Magick.NET)
- **MP4 Export** - eksport do MP4 (wymaga FFmpeg)
- **Konfigurowalny FPS** - 1-30 klatek na sekundę
- **UI Control Panel** - okno kontroli nagrywania

### 📜 Scroll Capture
- **Automatyczne przewijanie** - przechwytywanie długich stron
- **Inteligentne wykrywanie końca** - zaawansowane porównanie screenshotów
- **Pionowe i poziome** - obsługa obu kierunków
- **Łączenie obrazów** - automatyczne łączenie wielu screenshotów

### 🔒 Bezpieczeństwo
- **Szyfrowanie AES-256** - szyfrowanie screenshotów
- **PBKDF2 Key Derivation** - bezpieczne generowanie kluczy z hasła
- **Znak wodny tekstowy** - dodawanie tekstowego znaku wodnego
- **Znak wodny obrazowy** - dodawanie obrazowego znaku wodnego
- **Usuwanie metadanych** - usuwanie EXIF dla prywatności

### ⚡ Optymalizacja Wydajności
- **Cache miniatur** - inteligentne cache'owanie
- **Lazy loading** - asynchroniczne ładowanie
- **Optymalizacja obrazów** - automatyczne zmniejszanie rozmiaru
- **Zarządzanie pamięcią** - automatyczne czyszczenie cache

### ☁️ Cloud Upload
- **Imgur** - bezpośredni upload
- **Cloudinary** - upload do Cloudinary
- **AWS S3** - upload do S3
- **Custom Server** - własny serwer
- **Auto Upload** - automatyczny upload po zapisaniu

## 🚀 Instalacja

### Wymagania
- Windows 10/11
- .NET 10.0 Runtime

### Dla użytkowników końcowych

Zobacz szczegółową instrukcję w [docs/installation/INSTALLATION.md](docs/installation/INSTALLATION.md)

### Dla programistów

```bash
git clone https://github.com/Co0ob1iee/PrettyScreenSHOT.git
cd PrettyScreenSHOT
dotnet restore
dotnet build
dotnet run
```

Lub uruchom skompilowany plik `PrettyScreenSHOT.exe` z folderu `bin/Debug/net10.0-windows/`

## 📖 Użycie

1. **Uruchom aplikację** - aplikacja uruchomi się w tle (ikona w system tray)
2. **Naciśnij PRTSCN** - pojawi się overlay do wyboru obszaru
3. **Zaznacz obszar** - przeciągnij myszką, aby zaznaczyć obszar
4. **Edytuj** - kliknij prawym przyciskiem na tray icon → "Edit Last Screenshot"
5. **Zapisz** - kliknij "SAVE" w edytorze

### Skróty Klawiszowe
- **PRTSCN** - przechwytywanie screenshotu
- **ESC** - anulowanie (w overlay)

## 🛠️ Technologie

- **.NET 10.0** - framework
- **WPF** - interfejs użytkownika
- **WinAPI** - przechwytywanie ekranu i keyboard hooks
- **System.Windows.Forms** - tray icon
- **Magick.NET** - przetwarzanie obrazów i animowane GIF
- **FFmpeg** - eksport do MP4 (opcjonalnie)
- **AES-256** - szyfrowanie
- **PBKDF2** - key derivation

## 📁 Struktura Projektu

Szczegółowy opis struktury i architektury projektu znajduje się w [docs/PROJECT_STRUCTURE.md](docs/PROJECT_STRUCTURE.md).

### Główne komponenty:
- **App.xaml/.cs** - Punkt wejścia aplikacji
- **TrayIconManager.cs** - Zarządzanie ikoną w tray
- **ScreenshotHelper.cs** - Przechwytywanie screenshotów
- **ScreenshotManager.cs** - Zarządzanie historią
- **ScreenshotEditorWindow.xaml/.cs** - Edytor obrazów
- **ScreenshotHistoryWindow.xaml/.cs** - Historia screenshotów
- **SettingsWindow.xaml/.cs** - Okno ustawień
- **CloudUploadManager.cs** - Upload do chmury
- **VideoCaptureManager.cs** - Nagrywanie GIF/MP4
- **SecurityManager.cs** - Szyfrowanie i watermarking
- **Properties/Resources.*.resx** - Pliki lokalizacji

## 🗺️ Roadmap

Zobacz [ROADMAP.md](ROADMAP.md) dla szczegółowego planu rozwoju.

### Nadchodzące Funkcje
- Więcej narzędzi rysowania (Elipsa, Linia, Fill)
- Zaawansowane narzędzia (Crop, Resize, Rotate)
- Filtry i efekty
- OCR (rozpoznawanie tekstu)
- Więcej cloud providers (Google Drive, Dropbox)
- Skróty klawiszowe w edytorze
- GPU acceleration dla przetwarzania obrazów
- Zaawansowane algorytmy wykrywania zmian w Scroll Capture

## 🤝 Współpraca

Contributions są mile widziane! Proszę:
1. Fork projektu
2. Utwórz branch dla swojej funkcji (`git checkout -b feature/AmazingFeature`)
3. Commit zmian (`git commit -m 'Add some AmazingFeature'`)
4. Push do brancha (`git push origin feature/AmazingFeature`)
5. Otwórz Pull Request

## 📝 Licencja

Ten projekt jest licencjonowany na licencji GNU GPL v3 - zobacz plik [LICENSE](LICENSE) dla szczegółów.

## 🙏 Podziękowania

- Wszystkim contributorom
- Społeczności open source
- Użytkownikom za feedback

## 📧 Kontakt

- Issues: [GitHub Issues](https://github.com/Co0ob1iee/PrettyScreenSHOT/issues)
- Discussions: [GitHub Discussions](https://github.com/Co0ob1iee/PrettyScreenSHOT/discussions)

---

**Made with ❤️ using C# and WPF**
