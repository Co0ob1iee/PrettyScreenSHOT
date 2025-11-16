using Xunit;
using PrettyScreenSHOT.Services.Security;
using System;
using System.Linq;

namespace PrettyScreenSHOT.Tests.Services
{
    /// <summary>
    /// Unit tests for SecurityManager class.
    /// Tests encryption key generation, salt generation, and key derivation.
    /// </summary>
    public class SecurityManagerTests
    {
        [Fact]
        public void GenerateRandomKey_ShouldReturnNonNullKey()
        {
            // Act
            var key = SecurityManager.GenerateRandomKey();

            // Assert
            Assert.NotNull(key);
        }

        [Fact]
        public void GenerateRandomKey_ShouldReturn32Bytes()
        {
            // Act
            var key = SecurityManager.GenerateRandomKey();

            // Assert
            Assert.Equal(32, key.Length); // 256 bits = 32 bytes
        }

        [Fact]
        public void GenerateRandomKey_ShouldGenerateDifferentKeys()
        {
            // Act
            var key1 = SecurityManager.GenerateRandomKey();
            var key2 = SecurityManager.GenerateRandomKey();

            // Assert
            Assert.NotEqual(key1, key2);
        }

        [Fact]
        public void GenerateRandomKey_ShouldNotBeAllZeros()
        {
            // Act
            var key = SecurityManager.GenerateRandomKey();

            // Assert
            Assert.True(key.Any(b => b != 0), "Generated key should not be all zeros");
        }

        [Fact]
        public void GenerateSalt_ShouldReturnNonNullSalt()
        {
            // Act
            var salt = SecurityManager.GenerateSalt();

            // Assert
            Assert.NotNull(salt);
        }

        [Fact]
        public void GenerateSalt_ShouldReturn16Bytes()
        {
            // Act
            var salt = SecurityManager.GenerateSalt();

            // Assert
            Assert.Equal(16, salt.Length); // 128 bits = 16 bytes
        }

        [Fact]
        public void GenerateSalt_ShouldGenerateDifferentSalts()
        {
            // Act
            var salt1 = SecurityManager.GenerateSalt();
            var salt2 = SecurityManager.GenerateSalt();

            // Assert
            Assert.NotEqual(salt1, salt2);
        }

        [Fact]
        public void GenerateSalt_ShouldNotBeAllZeros()
        {
            // Act
            var salt = SecurityManager.GenerateSalt();

            // Assert
            Assert.True(salt.Any(b => b != 0), "Generated salt should not be all zeros");
        }

        [Fact]
        public void DeriveKeyFromPassword_ShouldReturnNonNullKey()
        {
            // Arrange
            var password = "TestPassword123!";
            var salt = SecurityManager.GenerateSalt();

            // Act
            var key = SecurityManager.DeriveKeyFromPassword(password, salt);

            // Assert
            Assert.NotNull(key);
        }

        [Fact]
        public void DeriveKeyFromPassword_ShouldReturn32Bytes()
        {
            // Arrange
            var password = "TestPassword123!";
            var salt = SecurityManager.GenerateSalt();

            // Act
            var key = SecurityManager.DeriveKeyFromPassword(password, salt);

            // Assert
            Assert.Equal(32, key.Length); // 256 bits = 32 bytes
        }

        [Fact]
        public void DeriveKeyFromPassword_SamPasswordAndSalt_ShouldProduceSameKey()
        {
            // Arrange
            var password = "TestPassword123!";
            var salt = SecurityManager.GenerateSalt();

            // Act
            var key1 = SecurityManager.DeriveKeyFromPassword(password, salt);
            var key2 = SecurityManager.DeriveKeyFromPassword(password, salt);

            // Assert
            Assert.Equal(key1, key2);
        }

        [Fact]
        public void DeriveKeyFromPassword_DifferentPasswords_ShouldProduceDifferentKeys()
        {
            // Arrange
            var password1 = "TestPassword1";
            var password2 = "TestPassword2";
            var salt = SecurityManager.GenerateSalt();

            // Act
            var key1 = SecurityManager.DeriveKeyFromPassword(password1, salt);
            var key2 = SecurityManager.DeriveKeyFromPassword(password2, salt);

            // Assert
            Assert.NotEqual(key1, key2);
        }

        [Fact]
        public void DeriveKeyFromPassword_DifferentSalts_ShouldProduceDifferentKeys()
        {
            // Arrange
            var password = "TestPassword123!";
            var salt1 = SecurityManager.GenerateSalt();
            var salt2 = SecurityManager.GenerateSalt();

            // Act
            var key1 = SecurityManager.DeriveKeyFromPassword(password, salt1);
            var key2 = SecurityManager.DeriveKeyFromPassword(password, salt2);

            // Assert
            Assert.NotEqual(key1, key2);
        }

        [Fact]
        public void DeriveKeyFromPassword_EmptyPassword_ShouldNotThrow()
        {
            // Arrange
            var password = "";
            var salt = SecurityManager.GenerateSalt();

            // Act & Assert
            var exception = Record.Exception(() =>
                SecurityManager.DeriveKeyFromPassword(password, salt));

            Assert.Null(exception);
        }

        [Fact]
        public void DeriveKeyFromPassword_WithSpecialCharacters_ShouldWork()
        {
            // Arrange
            var password = "P@ssw0rd!#$%^&*()";
            var salt = SecurityManager.GenerateSalt();

            // Act
            var key = SecurityManager.DeriveKeyFromPassword(password, salt);

            // Assert
            Assert.NotNull(key);
            Assert.Equal(32, key.Length);
        }

        [Fact]
        public void DeriveKeyFromPassword_WithUnicodeCharacters_ShouldWork()
        {
            // Arrange
            var password = "Пароль密码🔒";
            var salt = SecurityManager.GenerateSalt();

            // Act
            var key = SecurityManager.DeriveKeyFromPassword(password, salt);

            // Assert
            Assert.NotNull(key);
            Assert.Equal(32, key.Length);
        }
    }
}
