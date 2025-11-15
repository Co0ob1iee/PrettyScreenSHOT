# Lista grafik potrzebnych do projektu PrettyScreenSHOT

## 📋 Spis treści
1. [Ikony aplikacji](#ikony-aplikacji)
2. [Ikony narzędzi edytora](#ikony-narzędzi-edytora)
3. [Ikony akcji](#ikony-akcji)
4. [Grafiki interfejsu](#grafiki-interfejsu)
5. [Placeholdery i obrazy pomocnicze](#placeholdery-i-obrazy-pomocnicze)
6. [Specyfikacje techniczne](#specyfikacje-techniczne)

---

## 🎯 Ikony aplikacji

### 1. **app.ico** ⭐ WYMAGANE
- **Lokalizacja:** `/app.ico` (root projektu)
- **Format:** `.ico` (multi-resolution)
- **Rozmiary:** 16x16, 32x32, 48x48, 64x64, 128x128, 256x256
- **Użycie:** 
  - Ikona w zasobniku systemowym (tray icon)
  - Ikona aplikacji w systemie Windows
  - Ikona okien aplikacji
- **Wymagania:**
  - Powinna być czytelna w małych rozmiarach (16x16)
  - Ciemna wersja dla dark mode (opcjonalnie)
  - Symbol związany z screenshotami (np. aparat, ekran, kadr)

### 2. **Logo aplikacji** (opcjonalne)
- **Nazwa:** `logo.png` / `logo.svg`
- **Format:** PNG (transparent) lub SVG
- **Rozmiar:** 256x256px (lub większy dla skalowania)
- **Użycie:**
  - Ekran startowy (splash screen)
  - Okno "O programie"
  - Dokumentacja
- **Wymagania:**
  - Wysoka jakość
  - Przezroczyste tło
  - Może zawierać tekst "PrettyScreenSHOT"

---

## 🛠️ Ikony narzędzi edytora

Wszystkie ikony powinny być w formacie **PNG** z przezroczystym tłem, rozmiar **32x32px** (lub **24x24px** dla mniejszych przycisków).

### 3. **icon_marker.png**
- **Nazwa:** Marker / Pędzel
- **Rozmiar:** 24x24px lub 32x32px
- **Użycie:** Przycisk "MARKER" w edytorze
- **Opis:** Ikona pędzla/markera do rysowania

### 4. **icon_rectangle.png**
- **Nazwa:** Prostokąt
- **Rozmiar:** 24x24px lub 32x32px
- **Użycie:** Przycisk "RECT" w edytorze
- **Opis:** Ikona prostokąta/ramki

### 5. **icon_arrow.png**
- **Nazwa:** Strzałka
- **Rozmiar:** 24x24px lub 32x32px
- **Użycie:** Przycisk "ARROW" w edytorze
- **Opis:** Ikona strzałki wskaźnika

### 6. **icon_blur.png**
- **Nazwa:** Rozmycie
- **Rozmiar:** 24x24px lub 32x32px
- **Użycie:** Przycisk "BLUR" w edytorze
- **Opis:** Ikona reprezentująca rozmycie/zamazywanie

### 7. **icon_text.png**
- **Nazwa:** Tekst
- **Rozmiar:** 24x24px lub 32x32px
- **Użycie:** Przycisk "TEXT" w edytorze
- **Opis:** Ikona literki "T" lub tekstu

---

## ⚡ Ikony akcji

### 8. **icon_upload.png**
- **Nazwa:** Upload
- **Rozmiar:** 16x16px lub 20x20px
- **Użycie:** 
  - Przycisk "UPLOAD" w edytorze
  - Przycisk "Upload" w historii
- **Opis:** Ikona chmury z strzałką w górę

### 9. **icon_save.png**
- **Nazwa:** Zapisz
- **Rozmiar:** 16x16px lub 20x20px
- **Użycie:** Przycisk "ZAPISZ" w edytorze
- **Opis:** Ikona dyskietki lub zapisu

### 10. **icon_cancel.png**
- **Nazwa:** Anuluj
- **Rozmiar:** 16x16px lub 20x20px
- **Użycie:** Przycisk "ANULUJ" w edytorze
- **Opis:** Ikona X lub krzyżyka

### 11. **icon_clear.png**
- **Nazwa:** Wyczyść
- **Rozmiar:** 16x16px lub 20x20px
- **Użycie:** Przycisk "CLEAR" w edytorze
- **Opis:** Ikona gumki lub czyszczenia

### 12. **icon_undo.png**
- **Nazwa:** Cofnij
- **Rozmiar:** 16x16px lub 20x20px
- **Użycie:** Przycisk "UNDO" w edytorze
- **Opis:** Ikona strzałki w lewo (cofanie)

### 13. **icon_delete.png**
- **Nazwa:** Usuń
- **Rozmiar:** 16x16px lub 20x20px
- **Użycie:** 
  - Przycisk "Delete" w historii
  - Menu kontekstowe
- **Opis:** Ikona kosza/śmietnika

### 14. **icon_color_picker.png**
- **Nazwa:** Wybór koloru
- **Rozmiar:** 16x16px lub 20x20px
- **Użycie:** Przycisk wyboru koloru w edytorze
- **Opis:** Ikona palety kolorów lub koła kolorów

---

## 🖼️ Grafiki interfejsu

### 15. **icon_history.png** / **icon_history.svg**
- **Nazwa:** Historia
- **Rozmiar:** 64x64px lub większy
- **Użycie:** 
  - Nagłówek okna historii (obecnie "??")
  - Menu kontekstowe
- **Opis:** Ikona galerii/zdjęć/historii

### 16. **icon_settings.png**
- **Nazwa:** Ustawienia
- **Rozmiar:** 16x16px lub 20x20px
- **Użycie:** Menu kontekstowe
- **Opis:** Ikona koła zębatego/ustawień

### 17. **icon_edit.png**
- **Nazwa:** Edytuj
- **Rozmiar:** 16x16px lub 20x20px
- **Użycie:** Menu kontekstowe ("Edytuj ostatni screenshot")
- **Opis:** Ikona ołówka/edytora

### 18. **icon_exit.png**
- **Nazwa:** Wyjście
- **Rozmiar:** 16x16px lub 20x20px
- **Użycie:** Menu kontekstowe
- **Opis:** Ikona wyjścia/zamknięcia

### 19. **icon_cloud.png** / **cloud_icon.svg**
- **Nazwa:** Chmura
- **Rozmiar:** 12x12px lub 16x16px
- **Użycie:** 
  - Wskaźnik uploadu w historii (obecnie emoji "☁")
  - Status uploadu
- **Opis:** Prosta ikona chmury

### 20. **icon_folder.png**
- **Nazwa:** Folder
- **Rozmiar:** 16x16px lub 20x20px
- **Użycie:** 
  - Przycisk "Browse..." w ustawieniach (opcjonalnie)
  - Wybór folderu zapisu
- **Opis:** Ikona folderu/katalogu

---

## 📦 Placeholdery i obrazy pomocnicze

### 21. **placeholder_screenshot.png**
- **Nazwa:** Placeholder screenshotu
- **Rozmiar:** 100x75px (lub proporcjonalnie)
- **Użycie:** 
  - Placeholder gdy thumbnail nie może być załadowany
  - Domyślny obraz w historii
- **Opis:** Prosty placeholder z ikoną aparatu/ekranu

### 22. **splash_screen.png** (opcjonalne)
- **Nazwa:** Ekran startowy
- **Rozmiar:** 800x600px lub większy
- **Użycie:** Ekran startowy podczas ładowania aplikacji
- **Opis:** Logo + nazwa aplikacji + wersja

---

## 🎨 Specyfikacje techniczne

### Format plików:
- **Ikony:** PNG (z przezroczystością) lub SVG (dla skalowalności)
- **Logo:** PNG (transparent) lub SVG
- **Placeholdery:** PNG (transparent)
- **Tray icon:** ICO (multi-resolution)

### Kolorystyka:
- **Tło:** Przezroczyste lub zgodne z dark theme (#1E1E1E)
- **Ikony:** 
  - Jasne dla dark theme (białe/szare)
  - Możliwość hover states (kolor #60A5FA)
- **Kontrast:** Wysoki dla czytelności

### Style:
- **Flat design** lub **Material Design**
- **Minimalistyczne** - proste, czytelne ikony
- **Spójne** - jednolity styl wszystkich ikon
- **Responsywne** - czytelne w różnych rozmiarach

### Rozmiary (priorytet):
1. **16x16px** - Menu, małe przyciski
2. **24x24px** - Przyciski narzędzi
3. **32x32px** - Większe ikony, nagłówki
4. **48x48px** - Duże ikony
5. **64x64px+** - Logo, główne grafiki

---

## 📝 Podsumowanie

### Wymagane (krytyczne):
1. ✅ `app.ico` - Ikona aplikacji (multi-resolution)
2. ✅ `icon_history.png` - Ikona historii (zastępuje "??")

### Wysokie priorytety:
3. ⭐ Ikony narzędzi edytora (marker, rectangle, arrow, blur, text)
4. ⭐ Ikony akcji (upload, save, cancel, clear, undo, delete)
5. ⭐ Ikony menu (edit, settings, exit)

### Średnie priorytety:
6. 📌 `icon_cloud.png` - Ikona chmury
7. 📌 `icon_color_picker.png` - Wybór koloru
8. 📌 `icon_folder.png` - Ikona folderu
9. 📌 `placeholder_screenshot.png` - Placeholder

### Opcjonalne:
10. 💡 `logo.png` - Logo aplikacji
11. 💡 `splash_screen.png` - Ekran startowy

---

## 🎯 Rekomendacje

### Dla szybkiego startu:
- Użyj **Font Awesome** lub **Material Icons** jako SVG
- Konwertuj do PNG w odpowiednich rozmiarach
- Użyj narzędzi jak **IcoFX** lub **GIMP** do tworzenia `.ico`

### Dla profesjonalnego wyglądu:
- Zleć projekt graficzny profesjonaliście
- Użyj spójnego design systemu (np. Material Design, Fluent Design)
- Stwórz ikony w stylu flat design z subtelnymi cieniami

### Narzędzia do tworzenia:
- **Figma** / **Adobe Illustrator** - projektowanie
- **IcoFX** / **IconWorkshop** - tworzenie .ico
- **GIMP** / **Photoshop** - edycja PNG
- **Inkscape** - SVG

---

## 📂 Struktura folderów (sugerowana)

```
PrettyScreenSHOT/
├── Assets/
│   ├── Icons/
│   │   ├── app.ico
│   │   ├── Tools/
│   │   │   ├── icon_marker.png
│   │   │   ├── icon_rectangle.png
│   │   │   ├── icon_arrow.png
│   │   │   ├── icon_blur.png
│   │   │   └── icon_text.png
│   │   ├── Actions/
│   │   │   ├── icon_upload.png
│   │   │   ├── icon_save.png
│   │   │   ├── icon_cancel.png
│   │   │   ├── icon_clear.png
│   │   │   ├── icon_undo.png
│   │   │   └── icon_delete.png
│   │   ├── Menu/
│   │   │   ├── icon_edit.png
│   │   │   ├── icon_history.png
│   │   │   ├── icon_settings.png
│   │   │   └── icon_exit.png
│   │   └── Misc/
│   │       ├── icon_cloud.png
│   │       ├── icon_color_picker.png
│   │       ├── icon_folder.png
│   │       └── placeholder_screenshot.png
│   └── Logo/
│       ├── logo.png
│       └── splash_screen.png
```

---

**Ostatnia aktualizacja:** 2024
**Wersja dokumentu:** 1.0

