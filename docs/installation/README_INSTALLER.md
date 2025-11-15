# Szybki start - Tworzenie instalatora

## Najprostsza metoda: Inno Setup (Rekomendowana dla początkujących) ⭐

### 1. Zainstaluj Inno Setup
Pobierz z: https://jrsoftware.org/isinfo.php

### 2. Zbuduj aplikację
```powershell
dotnet publish -c Release -r win-x64 --self-contained false
```

### 3. Otwórz Installer.iss
- Otwórz plik `Installer.iss` w Inno Setup Compiler
- Upewnij się, że ścieżka w sekcji `[Files]` wskazuje na `bin\Release\net10.0-windows\win-x64\publish\`
- Kliknij **Build → Compile**

### 4. Gotowe!
Instalator znajdziesz w folderze `installer\PrettyScreenSHOT-Setup.exe`

---

## Metoda 2: MSIX Package (Microsoft Store) 🏪

### 1. Zbuduj aplikację
```powershell
dotnet publish -c Release -r win-x64 --self-contained false
```

### 2. Utwórz pakiet MSIX
```powershell
# Wymaga Windows SDK
msbuild PrettyScreenSHOT.csproj /t:CreateMsixPackage /p:Configuration=Release
```

Lub użyj Visual Studio:
1. Kliknij prawym na projekt → **Publish**
2. Wybierz **MSIX Package**
3. Kliknij **Create**

### 3. Gotowe!
Pakiet znajdziesz w folderze `installer\PrettyScreenSHOT_1.0.0.0_x64.msix`

---

## Automatyczne budowanie

Użyj skryptu PowerShell:

```powershell
# Inno Setup (najłatwiejsze)
.\build-installer.ps1 -Method InnoSetup

# MSIX (wymaga Windows SDK)
.\build-installer.ps1 -Method MSIX
```

---

## Auto-Update System 🔄

PrettyScreenSHOT ma wbudowany system automatycznych aktualizacji!

### Jak działa:
1. **Sprawdzanie aktualizacji** - aplikacja automatycznie sprawdza GitHub Releases
2. **Powiadomienia** - jeśli dostępna jest nowa wersja, pojawi się okno
3. **Pobieranie** - użytkownik może pobrać i zainstalować aktualizację jednym kliknięciem
4. **Zachowanie ustawień** - wszystkie ustawienia są zachowane podczas aktualizacji

### Konfiguracja:
- Włącz/Wyłącz auto-update w Settings
- Ustaw interwał sprawdzania (domyślnie: 24 godziny)
- Wybierz czy sprawdzać przy starcie

### Wymagania dla auto-update:
- **EXE Installer**: Wymaga GitHub Releases z plikiem `.exe`
- **MSIX**: Automatyczne aktualizacje przez Windows Update / Microsoft Store

### ⚠️ Ważne: Konfiguracja GitHub

Przed użyciem auto-update, musisz zaktualizować dane repozytorium w `UpdateChecker.cs`:

```csharp
private const string RepositoryOwner = "yourusername"; // Zmień na swoje
private const string RepositoryName = "PrettyScreenSHOT"; // Zmień jeśli potrzeba
```

Zobacz [AUTO_UPDATE_PLAN.md](AUTO_UPDATE_PLAN.md) dla szczegółów implementacji.

---

## Publikacja na GitHub Releases

### 1. Utwórz Release
1. Przejdź do repozytorium na GitHub
2. Kliknij **Releases** → **Create a new release**
3. Wpisz tag wersji (np. `v1.0.0`) - **ważne**: użyj formatu `vX.Y.Z`
4. Dodaj opis (release notes)

### 2. Dodaj pliki instalatorów
- Przeciągnij `PrettyScreenSHOT-Setup.exe` (Inno Setup)
- Przeciągnij `PrettyScreenSHOT_1.0.0.0_x64.msix` (MSIX)
- **Nazwy plików powinny zawierać wersję** (np. `PrettyScreenSHOT-Setup-v1.0.0.exe`)
- Kliknij **Publish release**

### 3. Auto-update będzie działać automatycznie!
Aplikacja wykryje nową wersję i powiadomi użytkowników.

---

## Dokumentacja

- 📖 [INSTALLATION.md](INSTALLATION.md) - Przewodnik instalacji dla użytkowników
- 🔧 [INSTALLER_GUIDE.md](INSTALLER_GUIDE.md) - Szczegółowy przewodnik dla deweloperów
- 🔄 [AUTO_UPDATE_PLAN.md](AUTO_UPDATE_PLAN.md) - Plan i implementacja auto-update

---

**Gotowe! Masz pełny system instalacji i aktualizacji! 🚀**
