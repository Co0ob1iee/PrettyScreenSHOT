using Xunit;
using PrettyScreenSHOT.Helpers;
using System;

namespace PrettyScreenSHOT.Tests.Helpers
{
    /// <summary>
    /// Unit tests for DebugHelper class.
    /// Note: All DebugHelper methods are conditional on DEBUG compilation,
    /// so these tests verify that methods can be called without exceptions.
    /// In RELEASE builds, these method calls are removed by the compiler.
    /// </summary>
    public class DebugHelperTests
    {
        [Fact]
        public void LogDebug_ShouldNotThrowException()
        {
            // Arrange
            var message = "Test debug message";

            // Act & Assert
            var exception = Record.Exception(() => DebugHelper.LogDebug(message));
            Assert.Null(exception);
        }

        [Fact]
        public void LogInfo_ShouldNotThrowException()
        {
            // Arrange
            var category = "TestCategory";
            var message = "Test info message";

            // Act & Assert
            var exception = Record.Exception(() => DebugHelper.LogInfo(category, message));
            Assert.Null(exception);
        }

        [Fact]
        public void LogError_WithoutException_ShouldNotThrowException()
        {
            // Arrange
            var category = "TestCategory";
            var message = "Test error message";

            // Act & Assert
            var exception = Record.Exception(() => DebugHelper.LogError(category, message));
            Assert.Null(exception);
        }

        [Fact]
        public void LogError_WithException_ShouldNotThrowException()
        {
            // Arrange
            var category = "TestCategory";
            var message = "Test error message";
            var testException = new InvalidOperationException("Test exception");

            // Act & Assert
            var exception = Record.Exception(() => DebugHelper.LogError(category, message, testException));
            Assert.Null(exception);
        }

        [Fact]
        public void ShowMessage_ShouldNotThrowException()
        {
            // Arrange
            var title = "Test Title";
            var message = "Test Message";

            // Act & Assert
            // Note: This won't actually show a message box in unit tests
            var exception = Record.Exception(() => DebugHelper.ShowMessage(title, message));
            Assert.Null(exception);
        }

        [Fact]
        public void LogDebug_WithNullMessage_ShouldNotThrowException()
        {
            // Arrange
            string? message = null;

            // Act & Assert
            var exception = Record.Exception(() => DebugHelper.LogDebug(message!));
            Assert.Null(exception);
        }

        [Fact]
        public void LogDebug_WithEmptyMessage_ShouldNotThrowException()
        {
            // Arrange
            var message = "";

            // Act & Assert
            var exception = Record.Exception(() => DebugHelper.LogDebug(message));
            Assert.Null(exception);
        }

        [Fact]
        public void LogDebug_WithLongMessage_ShouldNotThrowException()
        {
            // Arrange
            var message = new string('A', 10000);

            // Act & Assert
            var exception = Record.Exception(() => DebugHelper.LogDebug(message));
            Assert.Null(exception);
        }

        [Fact]
        public void LogDebug_WithSpecialCharacters_ShouldNotThrowException()
        {
            // Arrange
            var message = "Test with special chars: \n\r\t\\/'\"<>@#$%^&*()";

            // Act & Assert
            var exception = Record.Exception(() => DebugHelper.LogDebug(message));
            Assert.Null(exception);
        }

        [Fact]
        public void LogDebug_WithUnicodeCharacters_ShouldNotThrowException()
        {
            // Arrange
            var message = "Unicode test: こんにちは 你好 Привет 🔒🎯📊";

            // Act & Assert
            var exception = Record.Exception(() => DebugHelper.LogDebug(message));
            Assert.Null(exception);
        }

        [Fact]
        public void LogInfo_WithMultipleCalls_ShouldNotThrowException()
        {
            // Act & Assert
            var exception = Record.Exception(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    DebugHelper.LogInfo($"Category{i}", $"Message {i}");
                }
            });
            Assert.Null(exception);
        }

        [Fact]
        public void LogError_WithNullException_ShouldNotThrowException()
        {
            // Arrange
            var category = "TestCategory";
            var message = "Test error message";
            Exception? nullException = null;

            // Act & Assert
            var exception = Record.Exception(() => DebugHelper.LogError(category, message, nullException));
            Assert.Null(exception);
        }

        [Fact]
        public void LogError_WithNestedExceptions_ShouldNotThrowException()
        {
            // Arrange
            var category = "TestCategory";
            var message = "Test error message";
            var innerException = new ArgumentException("Inner exception");
            var outerException = new InvalidOperationException("Outer exception", innerException);

            // Act & Assert
            var exception = Record.Exception(() => DebugHelper.LogError(category, message, outerException));
            Assert.Null(exception);
        }
    }
}
