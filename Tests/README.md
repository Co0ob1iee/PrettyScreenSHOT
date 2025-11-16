# PrettyScreenSHOT Tests

This directory contains unit tests for the PrettyScreenSHOT application.

## Test Framework

- **xUnit** - Modern, extensible unit testing framework for .NET
- **Moq** - Mocking library for .NET
- **coverlet** - Code coverage tool

## Running Tests

### Command Line

```bash
# Run all tests
dotnet test

# Run tests with verbose output
dotnet test --logger "console;verbosity=detailed"

# Run tests with code coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Run specific test class
dotnet test --filter "FullyQualifiedName~SettingsManagerTests"

# Run specific test method
dotnet test --filter "FullyQualifiedName~SettingsManagerTests.Instance_ShouldNotBeNull"
```

### Visual Studio

1. Open Test Explorer (Test > Test Explorer)
2. Click "Run All" to run all tests
3. Right-click individual tests to run or debug

### Visual Studio Code

1. Install "C# Dev Kit" extension
2. Tests will appear in the Test Explorer sidebar
3. Click the play button to run tests

## Test Structure

```
Tests/
├── Services/
│   ├── SettingsManagerTests.cs      # Tests for settings management
│   └── SecurityManagerTests.cs      # Tests for encryption & security
├── Helpers/
│   └── DebugHelperTests.cs          # Tests for debug utilities
└── README.md                         # This file
```

## Test Coverage

Current test coverage focuses on:

1. **SettingsManager** - Configuration persistence and retrieval
2. **SecurityManager** - Encryption key generation and derivation
3. **DebugHelper** - Debug logging utilities

### Expanding Test Coverage

To improve test coverage, consider adding tests for:

- [ ] Screenshot capture functionality
- [ ] Image processing and editing
- [ ] Cloud upload providers
- [ ] Video capture manager
- [ ] Update system
- [ ] Localization helpers

## Writing New Tests

### Example Test

```csharp
using Xunit;
using PrettyScreenSHOT.Services;

namespace PrettyScreenSHOT.Tests.Services
{
    public class YourServiceTests
    {
        [Fact]
        public void YourMethod_WithValidInput_ShouldReturnExpectedResult()
        {
            // Arrange
            var service = new YourService();
            var input = "test";

            // Act
            var result = service.YourMethod(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("expected", result);
        }

        [Theory]
        [InlineData("input1", "output1")]
        [InlineData("input2", "output2")]
        public void YourMethod_WithMultipleInputs_ShouldReturnCorrectOutputs(
            string input, string expected)
        {
            // Arrange
            var service = new YourService();

            // Act
            var result = service.YourMethod(input);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
```

## Best Practices

1. **AAA Pattern** - Arrange, Act, Assert
2. **Descriptive Names** - Test names should describe what they test
3. **One Assert Per Test** - Keep tests focused
4. **Independent Tests** - Tests should not depend on each other
5. **Fast Tests** - Unit tests should run quickly
6. **Cleanup** - Restore state after tests (especially with singletons)

## Known Limitations

### Singleton Testing

Some classes (like `SettingsManager`) use the Singleton pattern, which makes testing challenging:

- Tests share state through the singleton instance
- Tests include cleanup code to restore original values
- Consider refactoring to use Dependency Injection for better testability

### UI Testing

Current tests focus on business logic and services. UI testing for WPF windows requires:

- UI automation frameworks (e.g., FlaUI, WinAppDriver)
- Additional setup and configuration
- Longer test execution times

## Continuous Integration

Tests are automatically run on:

- Every push to any branch
- Every pull request
- Scheduled nightly builds

See `.github/workflows/dotnet.yml` for CI/CD configuration.

## Code Coverage Reports

Code coverage reports are generated during CI builds and can be viewed:

- Locally: After running with coverage, check `coverage.opencover.xml`
- CI: Coverage reports are uploaded as build artifacts

## Troubleshooting

### Tests fail with "dotnet not found"

Ensure .NET 10.0 SDK is installed:
```bash
dotnet --version
```

### Tests fail due to missing dependencies

Restore NuGet packages:
```bash
dotnet restore
```

### Tests fail in CI but pass locally

- Check for environment-specific dependencies
- Ensure tests don't rely on local file paths
- Verify tests are platform-independent

## Contributing

When adding new features:

1. Write tests for new functionality
2. Ensure all tests pass locally
3. Aim for >70% code coverage
4. Update this README if adding new test categories

---

**Last Updated:** 2025-11-16
**Test Framework:** xUnit 2.9.2
**Target Framework:** .NET 10.0
