using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PrettyScreenSHOT.Helpers;
using PrettyScreenSHOT.Services;
using Wpf.Ui.Controls;

namespace PrettyScreenSHOT.Views.Dialogs
{
    public partial class SaveScreenshotDialog : FluentWindow
    {
        public string Category { get; private set; } = "";
        public List<string> Tags { get; private set; } = new();
        public string Notes { get; private set; } = "";

        public SaveScreenshotDialog()
        {
            InitializeComponent();

            // WPF UI applies themes globally - no need to apply per-window

            // Załaduj lokalizowane teksty
            LoadLocalizedStrings();
            
            // Załaduj dostępne kategorie
            CategoryComboBox.ItemsSource = SearchAndFilterManager.Instance.AvailableCategories;
        }

        private void LoadLocalizedStrings()
        {
            Title = LocalizationHelper.GetString("SaveDialog_Title");
            if (CategoryLabel != null)
                CategoryLabel.Text = LocalizationHelper.GetString("SaveDialog_Category");
            if (TagsLabel != null)
                TagsLabel.Text = LocalizationHelper.GetString("SaveDialog_Tags");
            if (TagsTextBox != null)
            {
                // ui:TextBox has built-in PlaceholderText property
                TagsTextBox.PlaceholderText = LocalizationHelper.GetString("SaveDialog_TagsPlaceholder");
            }
            if (NotesLabel != null)
                NotesLabel.Text = LocalizationHelper.GetString("SaveDialog_Notes");
            if (CancelButton != null)
                CancelButton.Content = LocalizationHelper.GetString("SaveDialog_Cancel");
            if (SaveButton != null)
                SaveButton.Content = LocalizationHelper.GetString("SaveDialog_Save");
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            Category = CategoryComboBox.Text?.Trim() ?? "";

            // Parsuj tagi z tekstu
            var tagsText = TagsTextBox.Text?.Trim() ?? "";
            if (!string.IsNullOrEmpty(tagsText))
            {
                Tags = tagsText.Split(',')
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrEmpty(t))
                    .ToList();
            }
            else
            {
                Tags = new List<string>();
            }

            Notes = NotesTextBox.Text?.Trim() ?? "";

            // Dodaj nową kategorię do listy jeśli nie istnieje
            if (!string.IsNullOrEmpty(Category) && !SearchAndFilterManager.Instance.AvailableCategories.Contains(Category))
            {
                SearchAndFilterManager.Instance.AddCategory(Category);
            }

            // Dodaj nowe tagi do listy
            foreach (var tag in Tags)
            {
                SearchAndFilterManager.Instance.AddTag(tag);
            }

            this.DialogResult = true;
            this.Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}

