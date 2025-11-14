# Instrukcja przygotowania projektu na GitHub

## Krok 1: Zainstaluj Git (jeśli nie masz)

1. Pobierz Git z: https://git-scm.com/download/win
2. Zainstaluj z domyślnymi ustawieniami
3. Otwórz PowerShell lub Git Bash

## Krok 2: Konfiguracja Git (pierwszy raz)

```bash
git config --global user.name "Twoje Imię"
git config --global user.email "twoj@email.com"
```

## Krok 3: Inicjalizacja repozytorium

W katalogu projektu (`C:\Users\clobi\source\repos\PrettyScreenSHOT\PrettyScreenSHOT`):

```bash
# Inicjalizuj repozytorium
git init

# Dodaj wszystkie pliki (zgodnie z .gitignore)
git add .

# Utwórz pierwszy commit
git commit -m "Initial commit: PrettyScreenSHOT - Advanced screenshot tool"
```

## Krok 4: Utwórz repozytorium na GitHub

1. Przejdź do https://github.com
2. Zaloguj się
3. Kliknij "+" w prawym górnym rogu → "New repository"
4. Wpisz nazwę: `PrettyScreenSHOT`
5. Opis: `Advanced screenshot capture and editing tool for Windows`
6. Wybierz **Public** lub **Private**
7. **NIE zaznaczaj** "Initialize this repository with a README" (mamy już README)
8. Kliknij "Create repository"

## Krok 5: Połącz lokalne repozytorium z GitHub

GitHub pokaże instrukcje. Użyj:

```bash
# Dodaj remote (zastąp YOUR_USERNAME swoją nazwą użytkownika)
git remote add origin https://github.com/YOUR_USERNAME/PrettyScreenSHOT.git

# Zmień nazwę głównej gałęzi na main (jeśli potrzeba)
git branch -M main

# Wyślij kod na GitHub
git push -u origin main
```

## Krok 6: Weryfikacja

1. Odśwież stronę repozytorium na GitHub
2. Powinieneś zobaczyć wszystkie pliki projektu
3. README.md powinien być wyświetlony na stronie głównej

## Opcjonalne: Dodaj informacje o repozytorium

Na stronie repozytorium GitHub:
1. Kliknij ⚙️ Settings
2. Scroll down do "Features"
3. Włącz:
   - ✅ Issues
   - ✅ Discussions
   - ✅ Projects
   - ✅ Wiki (opcjonalnie)

## Opcjonalne: Dodaj tematy (Topics)

Na stronie głównej repozytorium:
1. Kliknij ⚙️ (Settings) obok "About"
2. Dodaj tematy:
   - `csharp`
   - `wpf`
   - `screenshot`
   - `windows`
   - `dotnet`
   - `image-editor`
   - `screen-capture`

## Opcjonalne: Dodaj badge do README

Możesz dodać badge na początku README.md:

```markdown
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![License](https://img.shields.io/badge/license-GPL%20v3-blue)
![Platform](https://img.shields.io/badge/platform-Windows-0078D6?logo=windows)
```

## Przydatne komendy Git

```bash
# Sprawdź status
git status

# Dodaj zmiany
git add .
git add nazwa_pliku

# Commit
git commit -m "Opis zmian"

# Push na GitHub
git push

# Pobierz zmiany z GitHub
git pull

# Zobacz historię
git log --oneline

# Utwórz nowy branch
git checkout -b feature/nazwa-funkcji

# Przełącz się na branch
git checkout main
```

## Troubleshooting

### Problem: "fatal: not a git repository"
**Rozwiązanie**: Upewnij się, że jesteś w katalogu projektu i uruchomiłeś `git init`

### Problem: "Permission denied"
**Rozwiązanie**: 
- Sprawdź czy masz dostęp do repozytorium na GitHub
- Użyj Personal Access Token zamiast hasła (Settings → Developer settings → Personal access tokens)

### Problem: "remote origin already exists"
**Rozwiązanie**: 
```bash
git remote remove origin
git remote add origin https://github.com/YOUR_USERNAME/PrettyScreenSHOT.git
```

## Następne kroki

1. ✅ Repozytorium jest gotowe
2. 📝 Możesz teraz tworzyć Issues i Pull Requests
3. 🚀 Rozpocznij pracę nad nowymi funkcjami
4. 📢 Podziel się projektem ze społecznością!

---

**Powodzenia z projektem!** 🎉

