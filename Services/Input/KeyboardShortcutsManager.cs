using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using PrettyScreenSHOT.Views.Windows;

namespace PrettyScreenSHOT.Services.Input
{
    public class KeyboardShortcutsManager
    {
        private static readonly KeyboardShortcutsManager instance = new();
        public static KeyboardShortcutsManager Instance => instance;

        private readonly Dictionary<KeyGesture, Action> shortcuts = new();
        private readonly Dictionary<Window, List<KeyBinding>> windowBindings = new();

        private KeyboardShortcutsManager()
        {
            RegisterDefaultShortcuts();
        }

        private void RegisterDefaultShortcuts()
        {
            // Globalne skróty (działają w całej aplikacji)
            RegisterShortcut(new KeyGesture(Key.S, ModifierKeys.Control), () =>
            {
                // Zapisz screenshot (jeśli edytor jest otwarty)
                var editor = GetActiveEditorWindow();
                if (editor != null)
                {
                    editor.SaveScreenshot();
                }
            });

            RegisterShortcut(new KeyGesture(Key.Z, ModifierKeys.Control), () =>
            {
                // Undo w edytorze
                var editor = GetActiveEditorWindow();
                if (editor != null)
                {
                    editor.Undo();
                }
            });

            RegisterShortcut(new KeyGesture(Key.Escape), () =>
            {
                // Zamknij aktywne okno
                if (System.Windows.Application.Current.MainWindow != null && System.Windows.Application.Current.MainWindow.IsActive)
                {
                    // Nie zamykaj głównego okna (aplikacja działa w tle)
                    return;
                }

                foreach (Window window in System.Windows.Application.Current.Windows)
                {
                    if (window.IsActive && window != System.Windows.Application.Current.MainWindow)
                    {
                        window.Close();
                        break;
                    }
                }
            });

            RegisterShortcut(new KeyGesture(Key.H, ModifierKeys.Control), () =>
            {
                // Otwórz historię
                var historyWindow = new ScreenshotHistoryWindow();
                historyWindow.Show();
            });

            RegisterShortcut(new KeyGesture(Key.OemComma, ModifierKeys.Control), () =>
            {
                // Otwórz ustawienia
                var settingsWindow = new SettingsWindow();
                settingsWindow.ShowDialog();
            });
        }

        public void RegisterShortcut(KeyGesture gesture, Action action)
        {
            shortcuts[gesture] = action;
        }

        public void RegisterWindowShortcuts(Window window)
        {
            if (window == null) return;

            var bindings = new List<KeyBinding>();

            foreach (var shortcut in shortcuts)
            {
                var binding = new KeyBinding
                {
                    Gesture = shortcut.Key,
                    Command = new RelayCommand(shortcut.Value)
                };

                window.InputBindings.Add(binding);
                bindings.Add(binding);
            }

            windowBindings[window] = bindings;
        }

        public void UnregisterWindowShortcuts(Window window)
        {
            if (window == null || !windowBindings.ContainsKey(window)) return;

            foreach (var binding in windowBindings[window])
            {
                window.InputBindings.Remove(binding);
            }

            windowBindings.Remove(window);
        }

        private ScreenshotEditorWindow? GetActiveEditorWindow()
        {
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if (window is ScreenshotEditorWindow editor && window.IsActive)
                {
                    return editor;
                }
            }
            return null;
        }

        public string GetShortcutDescription(KeyGesture gesture)
        {
            var modifiers = gesture.Modifiers != ModifierKeys.None 
                ? $"{gesture.Modifiers}+" 
                : "";
            return $"{modifiers}{gesture.Key}";
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action execute;

        public RelayCommand(Action execute)
        {
            this.execute = execute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { } // Event nie jest używany, ale wymagany przez interfejs
            remove { }
        }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            execute?.Invoke();
        }
    }
}

