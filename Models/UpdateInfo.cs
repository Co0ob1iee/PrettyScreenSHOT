using System;

namespace PrettyScreenSHOT.Models
{
    /// <summary>
    /// Informacje o dostępnej aktualizacji
    /// </summary>
    public class UpdateInfo
    {
        /// <summary>
        /// Wersja aktualizacji (np. "1.2.3")
        /// </summary>
        public string Version { get; set; } = "";

        /// <summary>
        /// URL do pobrania instalatora
        /// </summary>
        public string DownloadUrl { get; set; } = "";

        /// <summary>
        /// Notatki wydania (release notes)
        /// </summary>
        public string ReleaseNotes { get; set; } = "";

        /// <summary>
        /// Data wydania
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Rozmiar pliku w bajtach
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Nazwa pliku instalatora
        /// </summary>
        public string FileName { get; set; } = "";

        /// <summary>
        /// Suma kontrolna SHA256 (dla weryfikacji)
        /// </summary>
        public string? Checksum { get; set; }

        /// <summary>
        /// Czy to wersja beta/preview
        /// </summary>
        public bool IsPrerelease { get; set; }

        /// <summary>
        /// URL do strony release na GitHub
        /// </summary>
        public string ReleaseUrl { get; set; } = "";
    }
}

