# PrettyScreenSHOT 📸

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-GPL%20v3-blue)](LICENSE)
[![Platform](https://img.shields.io/badge/platform-Windows-0078D6?logo=windows)](https://www.microsoft.com/windows)

Advanced screenshot capture and editing application for Windows, built with C# WPF.

## ✨ Features

### 🎯 Capture
- **Region Selection** - select area to capture
- **Multi-Monitor Support** - capture from multiple monitors
- **Global Hotkey** - keyboard shortcut (default: PRTSCN)
- **Automatic Clipboard** - auto-copy to clipboard

### 🎨 Editor
- **Marker** - freehand drawing
- **Rectangle** - draw rectangles
- **Arrow** - draw arrows
- **Blur** - Gaussian blur for privacy
- **Text** - add text with font and size options
- **Color** - color picker with palette
- **Thickness** - adjustable line width
- **Undo/Clear** - undo and clear changes

### 📚 History
- **Automatic Saving** - all screenshots are saved
- **Thumbnails** - preview in history window
- **Delete** - easy screenshot deletion
- **Cloud Upload** - upload to cloud (Imgur, Cloudinary, S3, Custom)

### 🌍 Internationalization
- 🇬🇧 English
- 🇵🇱 Polski
- 🇩🇪 Deutsch
- 🇨🇳 中文 (Mandarin)
- 🇫🇷 Français

### ⚙️ Settings
- **Language** - interface language selection
- **Save Path** - configurable file location
- **Hotkey** - keyboard shortcut configuration
- **Image Format** - PNG, JPG, BMP
- **Quality** - quality adjustment (10-100%)
- **Auto Save** - automatic saving
- **Copy to Clipboard** - clipboard copying
- **Show Notifications** - notification display
- **Theme** - color themes (Dark/Light)

### 🎬 Video Capture
- **GIF Recording** - animated GIF recording (Magick.NET)
- **MP4 Export** - MP4 export (requires FFmpeg)
- **Configurable FPS** - 1-30 frames per second
- **UI Control Panel** - recording control window

### 📜 Scroll Capture
- **Automatic Scrolling** - capture long pages
- **Smart End Detection** - advanced screenshot comparison
- **Vertical and Horizontal** - both directions supported
- **Image Stitching** - automatic screenshot merging

### 🔒 Security
- **AES-256 Encryption** - screenshot encryption
- **PBKDF2 Key Derivation** - secure key generation from password
- **Text Watermark** - add text watermark
- **Image Watermark** - add image watermark
- **Metadata Removal** - remove EXIF for privacy

### ⚡ Performance Optimization
- **Thumbnail Cache** - intelligent caching
- **Lazy Loading** - asynchronous loading
- **Image Optimization** - automatic size reduction
- **Memory Management** - automatic cache cleanup

### ☁️ Cloud Upload
- **Imgur** - direct upload
- **Cloudinary** - Cloudinary upload
- **AWS S3** - S3 upload
- **Custom Server** - custom server support
- **Auto Upload** - automatic upload after save

## 🚀 Installation

### Requirements
- Windows 10/11
- .NET 10.0 Runtime

### For End Users

See detailed instructions in [docs/installation/INSTALLATION.md](docs/installation/INSTALLATION.md)

### For Developers

```bash
git clone https://github.com/Co0ob1iee/PrettyScreenSHOT.git
cd PrettyScreenSHOT
dotnet restore
dotnet build
dotnet run
```

Or run the compiled `PrettyScreenSHOT.exe` from `bin/Debug/net10.0-windows/` folder

## 📖 Usage

1. **Launch Application** - runs in background (system tray icon)
2. **Press PRTSCN** - overlay appears for area selection
3. **Select Area** - drag mouse to select area
4. **Edit** - right-click tray icon → "Edit Last Screenshot"
5. **Save** - click "SAVE" in editor

### Keyboard Shortcuts
- **PRTSCN** - capture screenshot
- **ESC** - cancel (in overlay)

## 🛠️ Technologies

- **.NET 10.0** - framework
- **WPF** - user interface
- **WinAPI** - screen capture and keyboard hooks
- **System.Windows.Forms** - tray icon
- **Magick.NET** - image processing and animated GIF
- **FFmpeg** - MP4 export (optional)
- **AES-256** - encryption
- **PBKDF2** - key derivation

## 📁 Project Structure

Detailed structure and architecture description at [docs/PROJECT_STRUCTURE.md](docs/PROJECT_STRUCTURE.md).

### Main Components:
- **App.xaml/.cs** - Application entry point
- **Views/** - All Windows, Dialogs, and Overlays
- **Services/** - Business logic and managers
  - Screenshot, Cloud, Update, Theme, Video, Settings, Security
- **Models/** - Data models
- **Helpers/** - Utility classes
- **Properties/Resources.*.resx** - Localization files

## 🗺️ Roadmap

See [ROADMAP.md](ROADMAP.md) for detailed development plan.

### Upcoming Features
- More drawing tools (Ellipse, Line, Fill)
- Advanced tools (Crop, Resize, Rotate)
- Filters and effects
- OCR (text recognition)
- More cloud providers (Google Drive, Dropbox)
- Editor keyboard shortcuts
- GPU acceleration for image processing
- Advanced scroll capture algorithms

## 🤝 Contributing

Contributions are welcome! Please:
1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

See [CONTRIBUTING.md](CONTRIBUTING.md) for detailed guidelines.

## 📝 License

This project is licensed under the GNU GPL v3 License - see [LICENSE](LICENSE) for details.

## 🙏 Acknowledgments

- All contributors
- Open source community
- Users for feedback

## 📧 Contact

- Issues: [GitHub Issues](https://github.com/Co0ob1iee/PrettyScreenSHOT/issues)
- Discussions: [GitHub Discussions](https://github.com/Co0ob1iee/PrettyScreenSHOT/discussions)

---

**Made with ❤️ using C# and WPF**
