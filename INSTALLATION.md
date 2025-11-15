# Instalacja PrettyScreenSHOT 📥

Przewodnik instalacji dla użytkowników końcowych.

## 📋 Wymagania systemowe

- **Windows 10** (wersja 1809 lub nowsza) / **Windows 11**
- **.NET 10.0 Runtime** (zainstaluje się automatycznie jeśli brakuje)
- **4 GB RAM** (zalecane)
- **100 MB** wolnego miejsca na dysku

---

## 🚀 Metoda 1: Instalator EXE (Inno Setup) - Zalecana ⭐

### Krok 1: Pobierz instalator

1. Przejdź do [GitHub Releases](https://github.com/yourusername/PrettyScreenSHOT/releases)
2. Znajdź najnowszą wersję
3. Pobierz plik `PrettyScreenSHOT-Setup-vX.X.X.exe`

### Krok 2: Uruchom instalator

1. **Kliknij dwukrotnie** na pobrany plik `PrettyScreenSHOT-Setup.exe`
2. Jeśli pojawi się ostrzeżenie **Windows Defender SmartScreen**:
   - Kliknij **Więcej informacji**
   - Kliknij **Uruchom mimo to**
   - *(To normalne dla aplikacji bez certyfikatu Code Signing)*

### Krok 3: Postępuj zgodnie z instrukcjami

1. **Wybierz język** instalatora (jeśli dostępny)
2. **Przeczytaj licencję** (GNU GPL v3)
3. **Wybierz lokalizację instalacji** (domyślnie: `C:\Program Files\PrettyScreenSHOT`)
4. **Wybierz komponenty**:
   - ✅ PrettyScreenSHOT (zawsze wymagane)
   - ✅ Skrót na pulpicie (opcjonalnie)
   - ✅ Skrót w menu Start (zawsze)
5. Kliknij **Zainstaluj**
6. Poczekaj na zakończenie instalacji

### Krok 4: Uruchom aplikację

1. Po zakończeniu instalacji możesz:
   - ✅ Zaznaczyć **"Uruchom PrettyScreenSHOT"** i kliknąć **Zakończ**
   - ✅ Lub znaleźć aplikację w menu Start
   - ✅ Lub użyć skrótu na pulpicie (jeśli utworzony)

### Krok 5: Pierwsze uruchomienie

1. Aplikacja uruchomi się w **tray icon** (ikona w zasobniku systemowym)
2. Kliknij prawym przyciskiem na ikonę, aby zobaczyć menu
3. Kliknij **Settings**, aby skonfigurować aplikację

---

## 📦 Metoda 2: Pakiet MSIX (Microsoft Store) 🏪

### Krok 1: Pobierz pakiet MSIX

1. Przejdź do [GitHub Releases](https://github.com/yourusername/PrettyScreenSHOT/releases)
2. Znajdź najnowszą wersję
3. Pobierz plik `PrettyScreenSHOT_X.X.X.X_x64.msix`

### Krok 2: Zainstaluj pakiet

**Opcja A: Podwójne kliknięcie**
1. Kliknij dwukrotnie na plik `.msix`
2. Kliknij **Zainstaluj** w oknie Windows
3. Poczekaj na zakończenie instalacji

**Opcja B: PowerShell (dla zaawansowanych)**
```powershell
# Otwórz PowerShell jako Administrator
Add-AppxPackage -Path "C:\ścieżka\do\PrettyScreenSHOT_X.X.X.X_x64.msix"
```

### Krok 3: Uruchom aplikację

1. Znajdź **PrettyScreenSHOT** w menu Start
2. Kliknij, aby uruchomić

### Zalety MSIX:
- ✅ Automatyczne aktualizacje przez Windows Update
- ✅ Lepsze zarządzanie uprawnieniami
- ✅ Możliwość publikacji w Microsoft Store
- ✅ Izolacja aplikacji

---

## 🔄 Aktualizacja aplikacji

### Automatyczne aktualizacje (Auto-Update)

PrettyScreenSHOT ma wbudowany system automatycznych aktualizacji!

#### Jak działa:
1. **Automatyczne sprawdzanie** - aplikacja sprawdza dostępność nowych wersji
2. **Powiadomienia** - jeśli dostępna jest nowa wersja, pojawi się powiadomienie
3. **Pobieranie** - możesz pobrać aktualizację jednym kliknięciem
4. **Instalacja** - instalator uruchomi się automatycznie

#### Konfiguracja Auto-Update:

1. Kliknij prawym na **tray icon** → **Settings**
2. Przejdź do zakładki **Updates**
3. Skonfiguruj:
   - ✅ **Enable Auto-Update** - włącz/wyłącz automatyczne aktualizacje
   - **Check Interval** - jak często sprawdzać (domyślnie: codziennie)
   - ✅ **Check on Startup** - sprawdzaj przy starcie aplikacji
   - ✅ **Show Notifications** - pokazuj powiadomienia o aktualizacjach

#### Ręczna aktualizacja:

1. Kliknij prawym na **tray icon** → **Check for Updates**
2. Jeśli dostępna jest nowa wersja, kliknij **Download Update**
3. Postępuj zgodnie z instrukcjami instalatora

### Aktualizacja MSIX:

Pakiet MSIX aktualizuje się automatycznie przez Windows Update lub Microsoft Store (jeśli opublikowany).

---

## 🗑️ Odinstalowanie

### Metoda 1: Panel Sterowania

1. Otwórz **Panel Sterowania** → **Programy i funkcje**
2. Znajdź **PrettyScreenSHOT**
3. Kliknij **Odinstaluj**
4. Postępuj zgodnie z instrukcjami

### Metoda 2: Ustawienia Windows

1. Otwórz **Ustawienia** → **Aplikacje**
2. Znajdź **PrettyScreenSHOT**
3. Kliknij **Odinstaluj**

### Metoda 3: MSIX

1. Kliknij prawym na ikonę aplikacji w menu Start
2. Wybierz **Odinstaluj**

---

## ❓ Rozwiązywanie problemów

### Problem: "Aplikacja nie uruchamia się"

**Rozwiązanie:**
1. Sprawdź czy masz zainstalowany **.NET 10.0 Runtime**
   - Pobierz z: https://dotnet.microsoft.com/download
2. Sprawdź **Windows Event Viewer** dla błędów
3. Uruchom aplikację jako **Administrator** (tymczasowo do testów)

### Problem: "Windows Defender blokuje instalator"

**Rozwiązanie:**
1. To normalne dla aplikacji bez certyfikatu Code Signing
2. Kliknij **Więcej informacji** → **Uruchom mimo to**
3. Lub dodaj wyjątek w Windows Defender

### Problem: "Nie można zainstalować MSIX"

**Rozwiązanie:**
1. Upewnij się, że masz **Windows 10 wersja 1809 lub nowsza**
2. Sprawdź czy pakiet nie jest już zainstalowany
3. Użyj PowerShell jako Administrator:
   ```powershell
   Get-AppxPackage | Where-Object {$_.Name -like "*PrettyScreenSHOT*"} | Remove-AppxPackage
   ```

### Problem: "Auto-Update nie działa"

**Rozwiązanie:**
1. Sprawdź połączenie z internetem
2. Sprawdź ustawienia firewall/antywirusa
3. Sprawdź ustawienia Auto-Update w aplikacji
4. Sprawdź czy GitHub Releases są dostępne

### Problem: "Brakuje .NET Runtime"

**Rozwiązanie:**
1. Pobierz i zainstaluj **.NET 10.0 Desktop Runtime**:
   - https://dotnet.microsoft.com/download/dotnet/10.0
2. Wybierz wersję **Desktop Runtime** (nie SDK)
3. Zainstaluj i uruchom aplikację ponownie

---

## 📞 Wsparcie

Jeśli masz problemy z instalacją:

1. **Sprawdź dokumentację**: [README.md](README.md)
2. **Zgłoś problem**: [GitHub Issues](https://github.com/yourusername/PrettyScreenSHOT/issues)
3. **Zadaj pytanie**: [GitHub Discussions](https://github.com/yourusername/PrettyScreenSHOT/discussions)

---

## 📝 Uwagi

- **Pierwsza instalacja** może zająć kilka minut (pobieranie zależności)
- **Ustawienia** są zapisywane w `%AppData%\PrettyScreenSHOT\`
- **Screenshoty** są domyślnie zapisywane w `%Pictures%\PrettyScreenSHOT\`
- **Logi** aplikacji znajdują się w `%AppData%\PrettyScreenSHOT\logs\`

---

**Gotowe! Ciesz się używaniem PrettyScreenSHOT! 🎉**
