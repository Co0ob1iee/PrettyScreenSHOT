# Logging System Documentation

## Overview

PrettyScreenSHOT uses a custom debug-only logging system built on .NET's `System.Diagnostics` namespace. The logging system is designed to provide comprehensive debugging information during development while having **zero overhead** in production builds.

## Key Characteristics

- **Framework**: Custom system using `System.Diagnostics`
- **Conditional Compilation**: All logging removed in RELEASE builds
- **Zero Dependencies**: No external logging frameworks required
- **Location**: `Helpers/DebugHelper.cs`
- **Log File**: `%TEMP%/PrettyScreenSHOT_Debug.log` (DEBUG builds only)

## DebugHelper API

### Available Methods

#### 1. LogDebug(string message)
Logs debug messages to both the Visual Studio Debug Output and a log file.

```csharp
DebugHelper.LogDebug("Application started successfully");
```

**Output**:
- Debug Output window
- File: `%TEMP%/PrettyScreenSHOT_Debug.log`

**Format**: `[YYYY-MM-DD HH:MM:SS] message`

#### 2. LogInfo(string category, string message)
Logs informational messages with a category label. **Most commonly used** (71 calls).

```csharp
DebugHelper.LogInfo("Screenshot", "Capture initiated");
```

**Output**: Debug Output window only (not written to file)

**Format**: `[INFO] [category] message`

#### 3. LogError(string category, string message, Exception ex = null)
Logs error messages with optional exception details (61 calls).

```csharp
try {
    // Some operation
} catch (Exception ex) {
    DebugHelper.LogError("Upload", "Failed to upload screenshot", ex);
}
```

**Output**: Debug Output window only

**Format**:
```
[ERROR] [category] message
Exception: exception.Message
StackTrace: exception.StackTrace
```

#### 4. ShowMessage(string title, string message)
Shows a debug message box popup. Use sparingly (3 calls total).

```csharp
DebugHelper.ShowMessage("Debug", "Critical state reached");
```

**Output**: Windows MessageBox (blocking)

**Use Case**: Critical debugging points where immediate attention is needed

## Usage Statistics

| Method | Calls | Percentage | Primary Use |
|--------|-------|------------|-------------|
| LogInfo | 71 | 44.4% | General information flow |
| LogError | 61 | 38.1% | Error handling |
| LogDebug | 25 | 15.6% | Detailed debugging |
| ShowMessage | 3 | 1.9% | Critical alerts |
| **Total** | **160** | **100%** | **Across 26 files** |

## Common Usage Patterns

### 1. Application Lifecycle
```csharp
// Application startup
DebugHelper.LogInfo("App", "Application starting");
DebugHelper.LogInfo("Settings", "Loading user settings");
```

### 2. Screenshot Operations
```csharp
// Screenshot capture
DebugHelper.LogInfo("Screenshot", "Starting capture");
DebugHelper.LogInfo("Screenshot", $"Captured region: {bounds}");
DebugHelper.LogInfo("Screenshot", "Capture completed successfully");
```

### 3. Error Handling
```csharp
try {
    await PerformUploadAsync();
} catch (Exception ex) {
    DebugHelper.LogError("CloudUpload", "Upload failed", ex);
    // Handle error
}
```

### 4. Update Process
```csharp
DebugHelper.LogInfo("Update", "Checking for updates");
DebugHelper.LogInfo("Update", $"Current version: {currentVersion}");
DebugHelper.LogInfo("Update", $"Latest version: {latestVersion}");
```

## Most Logging-Heavy Files

1. **ScreenshotEditorWindow.xaml.cs** - 14 calls
   - UI interaction tracking
   - Tool selection logging
   - Image manipulation operations

2. **UpdateChecker.cs** - 6 calls
   - Update check process
   - Version comparison
   - Network operations

3. **ScreenshotManager.cs** - 5+ calls
   - Screenshot lifecycle
   - Save operations
   - Error handling

4. **ScrollCaptureHelper.cs** - 4 calls
   - Scroll capture process
   - Image stitching

5. **UpdateInstaller.cs** - 4+ calls
   - Installation process
   - File operations

## Build Behavior

### DEBUG Build
```csharp
[Conditional("DEBUG")]
public static void LogInfo(string category, string message)
{
    // This code IS included
    Debug.WriteLine($"[INFO] [{category}] {message}");
}
```

**Result**: Full logging functionality available

### RELEASE Build
```csharp
[Conditional("DEBUG")]
public static void LogInfo(string category, string message)
{
    // This code IS REMOVED by the compiler
}
```

**Result**:
- All calls to `DebugHelper` methods are removed
- Zero performance overhead
- No log file created
- Smaller executable size

## Advantages

1. **Zero Production Overhead**
   - All logging code removed in RELEASE builds
   - No runtime checks or conditionals
   - No performance impact

2. **No Dependencies**
   - Uses built-in .NET functionality
   - No NuGet packages required
   - Reduced application size

3. **Simple Integration**
   - Easy to use API
   - Familiar to .NET developers
   - No configuration required

4. **Development-Friendly**
   - Immediate feedback in Visual Studio
   - File logging for debugging
   - Category-based organization

## Limitations

1. **Debug-Only**
   - No logging in production builds
   - Cannot collect user-reported issues with logs
   - No telemetry in production

2. **Inconsistent File Writing**
   - Only `LogDebug()` writes to file
   - `LogInfo()` and `LogError()` only write to Debug Output
   - File I/O errors silently ignored

3. **No Log Rotation**
   - Single log file grows indefinitely
   - Manual cleanup required
   - No automatic archiving

4. **No Structured Logging**
   - Plain text format
   - Not machine-parseable
   - Difficult to analyze programmatically

5. **Limited Features**
   - No log levels (beyond method names)
   - No filtering capabilities
   - No remote logging
   - No log aggregation

## Log File Location

**Path**: `%TEMP%/PrettyScreenSHOT_Debug.log`

**Typical Locations**:
- Windows 10/11: `C:\Users\<username>\AppData\Local\Temp\PrettyScreenSHOT_Debug.log`

**Access**:
```csharp
string logPath = Path.Combine(Path.GetTempPath(), "PrettyScreenSHOT_Debug.log");
```

## Best Practices

### 1. Use Appropriate Methods
```csharp
// Good: Use LogInfo for general information
DebugHelper.LogInfo("Screenshot", "Capture started");

// Good: Use LogError for errors
DebugHelper.LogError("Upload", "Connection failed", ex);

// Good: Use LogDebug for detailed debugging
DebugHelper.LogDebug("Memory allocation: " + GC.GetTotalMemory(false));

// Avoid: Don't use ShowMessage in loops
// DebugHelper.ShowMessage("Loop", "Iteration " + i); // NO!
```

### 2. Include Contextual Information
```csharp
// Good: Include relevant details
DebugHelper.LogInfo("Screenshot", $"Saved to: {filePath}, Size: {fileSize} bytes");

// Poor: Vague messages
DebugHelper.LogInfo("Screenshot", "Saved"); // Not helpful
```

### 3. Use Consistent Categories
Common categories in the codebase:
- "App" - Application lifecycle
- "Screenshot" - Screenshot operations
- "CloudUpload" - Cloud upload operations
- "Update" - Update system
- "Settings" - Settings management
- "Security" - Security operations
- "VideoCapture" - Video recording

### 4. Log Errors with Exceptions
```csharp
// Good: Include exception details
try {
    // operation
} catch (Exception ex) {
    DebugHelper.LogError("Operation", "Failed to complete", ex);
}

// Poor: Missing exception context
try {
    // operation
} catch (Exception ex) {
    DebugHelper.LogError("Operation", "Failed"); // Lost exception info!
}
```

### 5. Avoid Sensitive Information
```csharp
// Good: Sanitized logging
DebugHelper.LogInfo("Login", $"User logged in: {username}");

// Bad: Logging sensitive data
// DebugHelper.LogInfo("Login", $"Password: {password}"); // NO!
```

## Unit Tests

The logging system has comprehensive unit tests in `Tests/Helpers/DebugHelperTests.cs`:

- 16 total tests
- Test all four methods
- Verify conditional compilation behavior
- Exception handling tests
- File I/O tests

Run tests:
```bash
dotnet test --filter "FullyQualifiedName~DebugHelperTests"
```

## Future Improvements

Potential enhancements for the logging system:

1. **Production Logging**
   - Implement opt-in logging for RELEASE builds
   - Allow users to generate diagnostic logs
   - Privacy-aware logging (no sensitive data)

2. **Log Rotation**
   - Automatic log file rotation
   - Size-based limits
   - Archive old logs

3. **Structured Logging**
   - JSON format for machine parsing
   - Log aggregation support
   - Better analysis tools

4. **Configuration**
   - Configurable log levels
   - Category filtering
   - Output format options

5. **Better File Handling**
   - Consistent file writing across all methods
   - Error handling for file I/O
   - Asynchronous logging

See [ROADMAP.md](ROADMAP.md) for planned logging improvements.

## Related Documentation

- [ARCHITECTURE.md](ARCHITECTURE.md) - Overall project architecture
- [../Tests/README.md](../Tests/README.md) - Testing guidelines
- Source code: `Helpers/DebugHelper.cs`
- Unit tests: `Tests/Helpers/DebugHelperTests.cs`
