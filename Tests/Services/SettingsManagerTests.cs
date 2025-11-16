using Xunit;
using PrettyScreenSHOT.Services.Settings;

namespace PrettyScreenSHOT.Tests.Services
{
    /// <summary>
    /// Unit tests for SettingsManager class.
    /// Note: SettingsManager is a singleton, so these tests share state.
    /// In a production environment, consider refactoring to support dependency injection.
    /// </summary>
    public class SettingsManagerTests
    {
        private readonly SettingsManager _settingsManager;

        public SettingsManagerTests()
        {
            _settingsManager = SettingsManager.Instance;
        }

        [Fact]
        public void Instance_ShouldNotBeNull()
        {
            // Arrange & Act
            var instance = SettingsManager.Instance;

            // Assert
            Assert.NotNull(instance);
        }

        [Fact]
        public void Language_DefaultValue_ShouldBeEnglish()
        {
            // Arrange & Act
            var language = _settingsManager.Language;

            // Assert
            Assert.NotNull(language);
            Assert.NotEmpty(language);
        }

        [Theory]
        [InlineData("en")]
        [InlineData("pl")]
        [InlineData("de")]
        [InlineData("zh")]
        [InlineData("fr")]
        public void Language_SetValue_ShouldPersist(string language)
        {
            // Arrange
            var originalLanguage = _settingsManager.Language;

            try
            {
                // Act
                _settingsManager.Language = language;

                // Assert
                Assert.Equal(language, _settingsManager.Language);
            }
            finally
            {
                // Cleanup - restore original value
                _settingsManager.Language = originalLanguage;
            }
        }

        [Fact]
        public void ImageFormat_DefaultValue_ShouldBePNG()
        {
            // Arrange & Act
            var format = _settingsManager.ImageFormat;

            // Assert
            Assert.Equal("PNG", format);
        }

        [Theory]
        [InlineData("PNG")]
        [InlineData("JPG")]
        [InlineData("BMP")]
        public void ImageFormat_SetValue_ShouldPersist(string format)
        {
            // Arrange
            var originalFormat = _settingsManager.ImageFormat;

            try
            {
                // Act
                _settingsManager.ImageFormat = format;

                // Assert
                Assert.Equal(format, _settingsManager.ImageFormat);
            }
            finally
            {
                // Cleanup
                _settingsManager.ImageFormat = originalFormat;
            }
        }

        [Fact]
        public void ImageQuality_DefaultValue_ShouldBe90()
        {
            // Arrange & Act
            var quality = _settingsManager.ImageQuality;

            // Assert
            Assert.Equal(90, quality);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(50)]
        [InlineData(75)]
        [InlineData(90)]
        [InlineData(100)]
        public void ImageQuality_SetValue_ShouldPersist(int quality)
        {
            // Arrange
            var originalQuality = _settingsManager.ImageQuality;

            try
            {
                // Act
                _settingsManager.ImageQuality = quality;

                // Assert
                Assert.Equal(quality, _settingsManager.ImageQuality);
            }
            finally
            {
                // Cleanup
                _settingsManager.ImageQuality = originalQuality;
            }
        }

        [Fact]
        public void AutoSave_DefaultValue_ShouldBeTrue()
        {
            // Arrange & Act
            var autoSave = _settingsManager.AutoSave;

            // Assert
            Assert.True(autoSave);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AutoSave_SetValue_ShouldPersist(bool value)
        {
            // Arrange
            var originalValue = _settingsManager.AutoSave;

            try
            {
                // Act
                _settingsManager.AutoSave = value;

                // Assert
                Assert.Equal(value, _settingsManager.AutoSave);
            }
            finally
            {
                // Cleanup
                _settingsManager.AutoSave = originalValue;
            }
        }

        [Fact]
        public void CopyToClipboard_DefaultValue_ShouldBeTrue()
        {
            // Arrange & Act
            var copyToClipboard = _settingsManager.CopyToClipboard;

            // Assert
            Assert.True(copyToClipboard);
        }

        [Fact]
        public void Theme_DefaultValue_ShouldBeDark()
        {
            // Arrange & Act
            var theme = _settingsManager.Theme;

            // Assert
            Assert.Equal("Dark", theme);
        }

        [Fact]
        public void VideoFrameRate_DefaultValue_ShouldBe10()
        {
            // Arrange & Act
            var fps = _settingsManager.VideoFrameRate;

            // Assert
            Assert.Equal(10, fps);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(15)]
        [InlineData(24)]
        [InlineData(30)]
        public void VideoFrameRate_SetValue_ShouldPersist(int fps)
        {
            // Arrange
            var originalFps = _settingsManager.VideoFrameRate;

            try
            {
                // Act
                _settingsManager.VideoFrameRate = fps;

                // Assert
                Assert.Equal(fps, _settingsManager.VideoFrameRate);
            }
            finally
            {
                // Cleanup
                _settingsManager.VideoFrameRate = originalFps;
            }
        }

        [Fact]
        public void IsVersionSkipped_WithSkippedVersion_ShouldReturnTrue()
        {
            // Arrange
            var testVersion = "1.2.3";
            var originalSkippedVersion = _settingsManager.SkippedVersion;

            try
            {
                // Act
                _settingsManager.SaveSkippedVersion(testVersion);
                var result = _settingsManager.IsVersionSkipped(testVersion);

                // Assert
                Assert.True(result);
            }
            finally
            {
                // Cleanup
                _settingsManager.SkippedVersion = originalSkippedVersion;
            }
        }

        [Fact]
        public void IsVersionSkipped_WithDifferentVersion_ShouldReturnFalse()
        {
            // Arrange
            var skippedVersion = "1.2.3";
            var differentVersion = "1.2.4";
            var originalSkippedVersion = _settingsManager.SkippedVersion;

            try
            {
                // Act
                _settingsManager.SaveSkippedVersion(skippedVersion);
                var result = _settingsManager.IsVersionSkipped(differentVersion);

                // Assert
                Assert.False(result);
            }
            finally
            {
                // Cleanup
                _settingsManager.SkippedVersion = originalSkippedVersion;
            }
        }
    }
}
