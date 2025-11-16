# PrettyScreenSHOT Architecture

## Overview

PrettyScreenSHOT is a WPF-based screenshot and screen recording application built with .NET 10.0. The architecture follows a clean separation of concerns with distinct layers for UI, business logic, and utilities.

## Project Structure

```
PrettyScreenSHOT/
├── Views/                  # UI Layer (Presentation)
│   ├── Windows/           # Main application windows (10 windows)
│   ├── Dialogs/           # Modal dialogs
│   └── Overlays/          # Non-modal overlays
│
├── Services/              # Business Logic Layer (21 services)
│   ├── Cloud/            # Cloud upload providers (6 classes)
│   ├── Screenshot/       # Screenshot capture & management (3 classes)
│   ├── Video/           # Video capture (1 class)
│   ├── Update/          # Auto-update system (4 classes)
│   ├── Security/        # Encryption & security (1 class)
│   ├── Settings/        # Configuration management (1 class)
│   └── Input/           # Keyboard shortcuts (1 class)
│
├── Models/               # Data Layer
│   └── UpdateInfo.cs    # Data models
│
├── Helpers/             # Utility Layer
│   ├── DebugHelper.cs   # Debug logging utilities
│   └── LocalizationHelper.cs  # i18n utilities
│
├── Properties/          # Resources & Configuration
│   ├── Resources.resx   # Localization files (5 languages)
│   └── AssemblyInfo.cs
│
├── Themes/              # UI Theming
│   └── WpfUiCustom.xaml
│
└── Tests/               # Testing Layer
    ├── Services/        # Service tests
    └── Helpers/         # Helper tests
```

## Design Patterns

### 1. Singleton Pattern
Used for application-wide state management:
- `SettingsManager` - Centralized configuration
- `ScreenshotManager` - Screenshot orchestration

**Rationale**: These classes manage global application state and should have only one instance throughout the application lifecycle.

### 2. Interface-based Design
Used for extensibility and dependency inversion:
- `ICloudUploadProvider` - Abstracts cloud upload implementations
  - Implementations: Imgur, Cloudinary, S3, CustomServer

**Rationale**: Allows adding new cloud providers without modifying existing code, following the Open/Closed Principle.

### 3. Manager Pattern
Used for orchestrating complex operations:
- `CloudUploadManager` - Coordinates cloud upload operations
- `UpdateManager` - Manages update workflow
- `ScreenshotManager` - Manages screenshot lifecycle

**Rationale**: Separates orchestration logic from implementation details, improving maintainability.

### 4. Static Helper Classes
Used for stateless utility functions:
- `DebugHelper` - Logging utilities
- `LocalizationHelper` - Internationalization
- `ScreenshotHelper` - Screenshot utilities
- `ScrollCaptureHelper` - Scroll capture utilities

**Rationale**: These classes contain pure functions with no state, making them simple and reusable.

## Layer Responsibilities

### View Layer (Views/)
**Purpose**: User interface and user interaction

**Responsibilities**:
- Display data to users
- Capture user input
- Delegate business logic to Services
- Handle UI-specific state

**Key Windows**:
- `ScreenshotEditorWindow` - Screenshot editing interface
- `ScreenshotHistoryWindow` - Screenshot history management
- `SettingsWindow` - Application settings
- `VideoCaptureWindow` - Video recording interface
- `UpdateWindow` - Update management

### Service Layer (Services/)
**Purpose**: Business logic and application functionality

**Responsibilities**:
- Implement core application features
- Manage application state
- Coordinate operations across multiple components
- Interface with external systems (cloud, filesystem)

**Key Services**:
- **Cloud Upload** (6 classes)
  - `CloudUploadManager` - Orchestrates uploads
  - Provider implementations for different cloud services

- **Screenshot** (3 classes)
  - `ScreenshotManager` - Manages screenshot lifecycle
  - `ScreenshotHelper` - Capture utilities
  - `ScrollCaptureHelper` - Scrolling capture

- **Update** (4 classes)
  - `UpdateManager` - Coordinates update process
  - `UpdateChecker` - Checks for updates
  - `UpdateDownloader` - Downloads updates
  - `UpdateInstaller` - Installs updates

- **Security** (1 class)
  - `SecurityManager` - AES-256 encryption

### Helper Layer (Helpers/)
**Purpose**: Reusable utility functions

**Responsibilities**:
- Provide stateless utility functions
- Encapsulate common operations
- Support other layers without dependencies

### Model Layer (Models/)
**Purpose**: Data structures and domain objects

**Responsibilities**:
- Define data structures
- Encapsulate domain logic
- Provide data validation

## Code Statistics

| Metric | Value |
|--------|-------|
| Total C# Lines | ~7,612 |
| Total XAML Lines | ~1,620 |
| Service Classes | 21 |
| View Windows | 10 |
| Unit Tests | 40+ |
| Test Coverage | ~40% |
| Supported Languages | 5 (EN, PL, DE, ZH, FR) |

## Dependencies

### NuGet Packages
- **WPF-UI** - Modern Fluent Design UI components
- **Newtonsoft.Json** - JSON serialization
- **xUnit** - Testing framework
- **Moq** - Mocking framework (tests)

### .NET Framework
- **.NET 10.0** - Target framework
- **WPF** - UI framework
- **System.Diagnostics** - Debug logging
- **System.Security.Cryptography** - AES-256 encryption

## Key Architectural Decisions

### 1. WPF over Other UI Frameworks
**Decision**: Use WPF for the UI layer
**Rationale**:
- Mature and stable framework
- Rich UI capabilities for image editing
- Excellent theming support
- Native Windows integration

### 2. Custom Logging System
**Decision**: Custom debug logging using `System.Diagnostics`
**Rationale**:
- Zero overhead in RELEASE builds
- No external dependencies
- Simple debugging during development
- See [LOGGING.md](LOGGING.md) for details

### 3. Singleton Managers
**Decision**: Use singleton pattern for managers
**Rationale**:
- Single source of truth for application state
- Simplified dependency management
- Thread-safe implementation

### 4. Interface-based Cloud Providers
**Decision**: Abstract cloud uploads behind `ICloudUploadProvider`
**Rationale**:
- Easy to add new providers
- Testable through mocking
- Clear separation of concerns

## Testing Strategy

### Unit Tests
- **Location**: `Tests/` directory
- **Framework**: xUnit
- **Mocking**: Moq
- **Coverage**: ~40% (target: 70%+)

### Test Categories
1. **Service Tests** - Business logic validation
2. **Helper Tests** - Utility function validation
3. **Security Tests** - Encryption/decryption validation

### Testing Best Practices
- See `Tests/README.md` for detailed testing guidelines
- Each service should have corresponding tests
- Focus on edge cases and error handling
- Mock external dependencies

## Internationalization (i18n)

### Supported Languages
1. English (EN) - Default
2. Polish (PL)
3. German (DE)
4. Chinese (ZH)
5. French (FR)

### Implementation
- Resource files: `Properties/Resources.*.resx`
- Helper: `LocalizationHelper.cs`
- Runtime language switching supported

## Build & Deployment

### Build Configurations
- **Debug**: Full logging, debug symbols, no optimizations
- **Release**: No logging, optimized, ready for distribution

### Installers
- **InnoSetup**: `build/installer/Installer.iss`
- **WiX**: `build/installer/Installer.wxs`
- **Build Script**: `build/installer/build-installer.ps1`

### CI/CD
- GitHub Actions workflows in `.github/workflows/`
- Automated testing on pull requests
- Automated release builds

## Security Considerations

### Data Encryption
- **Algorithm**: AES-256
- **Implementation**: `SecurityManager.cs`
- **Use Cases**: Sensitive settings storage

### Debug Logging
- **IMPORTANT**: All debug logging is removed in RELEASE builds
- No sensitive data logged in production
- Conditional compilation with `[Conditional("DEBUG")]`

## Performance Optimization

### PerformanceOptimizer Service
- Memory management
- Image processing optimization
- Resource cleanup

### Best Practices
- Lazy loading of resources
- Asynchronous operations for I/O
- Proper disposal of WPF resources

## Future Enhancements

See [ROADMAP.md](ROADMAP.md) for planned features and improvements.

## Contributing

See [../CONTRIBUTING.md](../CONTRIBUTING.md) for contribution guidelines and coding standards.
