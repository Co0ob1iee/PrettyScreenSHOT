# Przewodnik tworzenia instalatora dla Windows 10

PrettyScreenSHOT można spakować na kilka sposobów. Poniżej znajdziesz instrukcje dla różnych metod.

## Metoda 1: MSIX Package (Zalecana) ⭐

MSIX to nowoczesny format pakietów Microsoft, który działa na Windows 10/11.

### Wymagania
- Windows 10 wersja 1809 lub nowsza
- Visual Studio 2022 lub Windows SDK
- Certyfikat podpisywania (opcjonalnie, ale zalecany)

### Krok 1: Przygotuj projekt

1. Otwórz `PrettyScreenSHOT.csproj`
2. Dodaj PackageReference (zobacz `PrettyScreenSHOT.csproj` - już dodane)

### Krok 2: Utwórz manifest AppxManifest

Utwórz plik `Package.appxmanifest` (zobacz przykład poniżej)

### Krok 3: Zbuduj pakiet MSIX

```powershell
# W katalogu projektu
dotnet publish -c Release -r win-x64 --self-contained false

# Utwórz pakiet MSIX
msbuild PrettyScreenSHOT.csproj /t:CreateMsixPackage /p:Configuration=Release
```

Lub użyj Visual Studio:
1. Kliknij prawym na projekt → **Publish**
2. Wybierz **MSIX Package**
3. Kliknij **Create**
4. Wybierz lokalizację i kliknij **Create**

### Krok 4: Podpisz pakiet (opcjonalnie, ale zalecane)

```powershell
# Utwórz certyfikat testowy
New-SelfSignedCertificate -Type Custom -Subject "CN=PrettyScreenSHOT" -KeyUsage DigitalSignature -FriendlyName "PrettyScreenSHOT Certificate" -CertStoreLocation "Cert:\CurrentUser\My" -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3", "2.5.29.19={text}")

# Podpisz pakiet
signtool sign /fd SHA256 /a /f "certificate.pfx" /p "password" "PrettyScreenSHOT_1.0.0.0_x64.msix"
```

---

## Metoda 2: WiX Toolset (Tradycyjny MSI) 🔧

WiX tworzy profesjonalne instalatory MSI.

### Wymagania
- WiX Toolset v3.11 lub nowszy: https://wixtoolset.org/
- Visual Studio Extension "WiX Toolset Visual Studio Extension"

### Instalacja WiX

1. Pobierz WiX z: https://wixtoolset.org/docs/wix3/
2. Zainstaluj WiX Toolset Build Tools
3. (Opcjonalnie) Zainstaluj Visual Studio Extension

### Utwórz plik .wxs

Utwórz `Installer.wxs` (zobacz przykład w pliku `Installer.wxs`)

### Zbuduj instalator

```powershell
# Kompiluj projekt
dotnet publish -c Release -r win-x64 --self-contained false

# Zbuduj MSI
candle Installer.wxs
light Installer.wixobj -ext WixUIExtension
```

---

## Metoda 3: Inno Setup (Najłatwiejsza) 🚀

Inno Setup to popularne narzędzie do tworzenia instalatorów Windows.

### Wymagania
- Inno Setup: https://jrsoftware.org/isinfo.php
- Skrypt ISS (zobacz `Installer.iss`)

### Instalacja

1. Pobierz i zainstaluj Inno Setup
2. Otwórz `Installer.iss` w Inno Setup Compiler
3. Kliknij **Build → Compile**

---

## Metoda 4: ClickOnce (Prosty) 📦

ClickOnce jest wbudowany w Visual Studio.

### W Visual Studio

1. Kliknij prawym na projekt → **Properties**
2. Przejdź do zakładki **Publish**
3. Kliknij **Publish Wizard**
4. Wybierz lokalizację publikacji
5. Kliknij **Finish**

---

## Porównanie metod

| Metoda | Trudność | Rozmiar | Auto-Update | Zalecane dla |
|--------|----------|---------|-------------|--------------|
| **MSIX** | Średnia | Mały | ✅ Tak | Nowoczesne aplikacje |
| **WiX** | Wysoka | Średni | ❌ Nie* | Profesjonalne instalatory |
| **Inno Setup** | Niska | Średni | ❌ Nie* | Proste instalatory |
| **ClickOnce** | Niska | Mały | ✅ Tak | Szybkie wdrożenie |

*Możliwe z dodatkowymi narzędziami

---

## Automatyczne budowanie

Użyj skryptu `build-installer.ps1` do automatycznego budowania:

```powershell
.\build-installer.ps1 -Method MSIX
.\build-installer.ps1 -Method WiX
.\build-installer.ps1 -Method InnoSetup
```

---

## Dystrybucja

### MSIX
- Można opublikować w Microsoft Store
- Można dystrybuować bezpośrednio (.msix plik)
- Wymaga Windows 10 1809+

### MSI (WiX)
- Działa na wszystkich wersjach Windows
- Standardowy format dla enterprise
- Wymaga uprawnień administratora

### EXE (Inno Setup)
- Najbardziej kompatybilny
- Działa na wszystkich wersjach Windows
- Łatwy w użyciu dla użytkowników końcowych

---

## Rekomendacja

Dla **PrettyScreenSHOT** polecam:
1. **MSIX** - jeśli chcesz nowoczesny, łatwy w dystrybucji pakiet
2. **Inno Setup** - jeśli chcesz prosty, kompatybilny instalator EXE
3. **WiX** - jeśli potrzebujesz profesjonalnego MSI dla enterprise

---

## Troubleshooting

### Problem: "MSIX requires Windows 10 1809+"
**Rozwiązanie**: Użyj Inno Setup lub WiX dla starszych wersji Windows

### Problem: "Certificate error"
**Rozwiązanie**: 
- Użyj certyfikatu testowego dla development
- Kup certyfikat Code Signing dla produkcji

### Problem: "Dependencies missing"
**Rozwiązanie**: 
- Użyj `--self-contained true` dla standalone
- Lub dołącz Visual C++ Redistributable

---

**Powodzenia z tworzeniem instalatora!** 🎉

