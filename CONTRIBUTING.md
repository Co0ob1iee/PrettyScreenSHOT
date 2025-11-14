# Contributing to PrettyScreenSHOT

Dziękujemy za zainteresowanie współpracą przy projekcie PrettyScreenSHOT! 🎉

## Jak możesz pomóc

### 🐛 Zgłaszanie błędów
Jeśli znalazłeś błąd:
1. Sprawdź czy błąd nie został już zgłoszony w [Issues](https://github.com/yourusername/PrettyScreenSHOT/issues)
2. Utwórz nowe issue z opisem:
   - Krok po kroku jak odtworzyć błąd
   - Oczekiwane zachowanie
   - Rzeczywiste zachowanie
   - Wersja systemu operacyjnego
   - Wersja aplikacji

### 💡 Propozycje funkcji
Masz pomysł na nową funkcję?
1. Sprawdź [ROADMAP.md](ROADMAP.md) - może już jest w planach
2. Utwórz issue z etykietą "enhancement"
3. Opisz szczegółowo funkcję i jej użyteczność

### 🔧 Pull Requests

#### Proces
1. **Fork** repozytorium
2. **Utwórz branch** dla swojej funkcji (`git checkout -b feature/AmazingFeature`)
3. **Commit** zmian (`git commit -m 'Add some AmazingFeature'`)
4. **Push** do brancha (`git push origin feature/AmazingFeature`)
5. **Otwórz Pull Request**

#### Wytyczne kodu
- **Formatowanie**: Używaj domyślnego formatowania Visual Studio
- **Nazewnictwo**: 
  - Klasy: PascalCase (`ScreenshotManager`)
  - Metody: PascalCase (`CaptureScreenshot`)
  - Zmienne: camelCase (`captureArea`)
  - Stałe: UPPER_CASE (`MAX_SCROLLS`)
- **Komentarze**: Komentuj złożoną logikę po polsku lub angielsku
- **Lokalizacja**: Wszystkie teksty UI muszą być w plikach `.resx`
- **Testowanie**: Przetestuj swoją funkcję przed PR

#### Struktura commitów
```
feat: Dodano funkcję video capture
fix: Naprawiono błąd w Scroll Capture
docs: Zaktualizowano README
refactor: Refaktoryzacja SecurityManager
style: Formatowanie kodu
perf: Optymalizacja wydajności cache
```

### 🌍 Lokalizacja
Jeśli dodajesz nowe teksty UI:
1. Dodaj klucz do wszystkich plików `.resx`:
   - `Properties/Resources.resx` (angielski)
   - `Properties/Resources.pl.resx` (polski)
   - `Properties/Resources.de.resx` (niemiecki)
   - `Properties/Resources.zh.resx` (chiński)
   - `Properties/Resources.fr.resx` (francuski)
2. Użyj `LocalizationHelper.GetString("Key")` w kodzie

### 🧪 Testowanie
Przed wysłaniem PR:
- [ ] Kod kompiluje się bez błędów
- [ ] Funkcja działa poprawnie
- [ ] Nie ma regresji w istniejących funkcjach
- [ ] Kod jest zgodny z wytycznymi
- [ ] Wszystkie teksty są zlokalizowane

### 📝 Dokumentacja
Jeśli dodajesz nową funkcję:
- Zaktualizuj `README.md` jeśli potrzeba
- Dodaj komentarze XML do publicznych metod
- Zaktualizuj `ROADMAP.md` jeśli funkcja była w planach

## Pytania?
Jeśli masz pytania, otwórz issue z etykietą "question" lub użyj [Discussions](https://github.com/yourusername/PrettyScreenSHOT/discussions).

## Licencja
Przez wysłanie PR zgadzasz się, że Twój kod będzie licencjonowany na licencji GNU GPL v3.

---

**Dziękujemy za wkład w rozwój PrettyScreenSHOT!** 🙏

