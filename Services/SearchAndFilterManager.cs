using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PrettyScreenSHOT.Services.Screenshot;

namespace PrettyScreenSHOT.Services
{
    public class SearchAndFilterManager
    {
        private static readonly SearchAndFilterManager instance = new();
        public static SearchAndFilterManager Instance => instance;

        public ObservableCollection<string> AvailableCategories { get; } = new();
        public ObservableCollection<string> AvailableTags { get; } = new();

        private SearchAndFilterManager()
        {
            LoadCategoriesAndTags();
        }

        private void LoadCategoriesAndTags()
        {
            // Załaduj kategorie i tagi z historii screenshotów
            foreach (var item in ScreenshotManager.Instance.History)
            {
                if (!string.IsNullOrEmpty(item.Category) && !AvailableCategories.Contains(item.Category))
                {
                    AvailableCategories.Add(item.Category);
                }

                foreach (var tag in item.Tags)
                {
                    if (!string.IsNullOrEmpty(tag) && !AvailableTags.Contains(tag))
                    {
                        AvailableTags.Add(tag);
                    }
                }
            }
        }

        public void AddCategory(string category)
        {
            if (!string.IsNullOrWhiteSpace(category) && !AvailableCategories.Contains(category))
            {
                AvailableCategories.Add(category);
            }
        }

        public void AddTag(string tag)
        {
            if (!string.IsNullOrWhiteSpace(tag) && !AvailableTags.Contains(tag))
            {
                AvailableTags.Add(tag);
            }
        }

        public IEnumerable<ScreenshotItem> Search(IEnumerable<ScreenshotItem> items, string? searchText, string? category, IEnumerable<string>? tags)
        {
            var query = items.AsQueryable();

            // Wyszukiwanie tekstowe
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var searchLower = searchText.ToLowerInvariant();
                query = query.Where(item =>
                    item.Filename.ToLowerInvariant().Contains(searchLower) ||
                    (!string.IsNullOrEmpty(item.Notes) && item.Notes.ToLowerInvariant().Contains(searchLower)) ||
                    item.Tags.Any(tag => tag.ToLowerInvariant().Contains(searchLower))
                );
            }

            // Filtrowanie po kategorii
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(item => item.Category == category);
            }

            // Filtrowanie po tagach
            if (tags != null && tags.Any())
            {
                var tagList = tags.ToList();
                query = query.Where(item => item.Tags.Any(tag => tagList.Contains(tag)));
            }

            return query.ToList();
        }

        public IEnumerable<ScreenshotItem> FilterByDateRange(IEnumerable<ScreenshotItem> items, DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                return items.Where(item => item.Timestamp >= startDate.Value && item.Timestamp <= endDate.Value);
            }
            else if (startDate.HasValue)
            {
                return items.Where(item => item.Timestamp >= startDate.Value);
            }
            else if (endDate.HasValue)
            {
                return items.Where(item => item.Timestamp <= endDate.Value);
            }

            return items;
        }

        public void RefreshCategoriesAndTags()
        {
            AvailableCategories.Clear();
            AvailableTags.Clear();
            LoadCategoriesAndTags();
        }
    }
}

