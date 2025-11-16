# Sprint 1 – Lista zadań

1. **Audyt UI**
   - [ ] Zebrać zrzuty ekranu i krótkie nagrania z każdego widoku.
   - [ ] Spisać wszystkie obecne dialogi WinForms, konwertery i helpery specyficzne dla widoku.
2. **Migracja z WinForms**
   - [ ] Zastąpić `FolderBrowserDialog`, `ColorDialog` i inne kontrolki WinForms natywnymi komponentami WPF/Wpf.Ui.
   - [ ] Ujednolicić MessageBoxy do jednego wariantu (np. `Wpf.Ui.Controls.MessageBox`).
3. **Porządki w aliasach i helperach**
   - [ ] Usunąć mieszane aliasy `TextBlock`/`MessageBoxButton` poprzez w pełni kwalifikowane nazwy lub middleware.
   - [ ] Skonsolidować konwertery (np. `NullToVisibilityConverter`) w jednym katalogu i udostępnić je przez `App.xaml`.
4. **Baza stylów**
   - [ ] Utworzyć `Themes/Base.xaml` + pliki kolorów/typografii i włączyć je w `App.xaml`.
   - [ ] Przenieść lokalne style z widoków do nowych dictionary (Card, Button, TextBox).
5. **Walidacja**
   - [ ] Ręcznie przejść główne scenariusze (zrzut, edycja, historia) i potwierdzić brak regresji.
   - [ ] Zaktualizować dokument z wnioskami i listą otwartych tematów na kolejne sprinty.

