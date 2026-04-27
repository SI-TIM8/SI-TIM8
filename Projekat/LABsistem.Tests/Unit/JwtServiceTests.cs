using System.Security.Claims;
using LABsistem.Bll.Models;
using LABsistem.Bll.Services;
using Xunit;

namespace LABsistem.Tests.Unit
{
    public class JwtServiceTests
    {
        private readonly JwtSettings _jwtSettings;
        private readonly JwtService _jwtService;

        public JwtServiceTests()
        {
            // Setup valid JWT settings for testing
            _jwtSettings = new JwtSettings
            {
                Key = "TestSuperSecretKeyThatMustBeLongEnough123!",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpireMinutes = 60
            };
            _jwtService = new JwtService(_jwtSettings);
        }

        [Fact]
        public void GenerateToken_WithValidData_ReturnsToken()
        {
            // Arrange
            var userId = "1";
            var username = "testuser";
            var role = "Admin";

            // Act
            var token = _jwtService.GenerateToken(userId, username, role);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(token));
            Assert.Contains(".", token); // A valid JWT consists of 3 parts separated by dots
        }

        [Fact]
        public void ValidateToken_WithValidToken_ReturnsClaimsPrincipal()
        {
            // Arrange
            var token = _jwtService.GenerateToken("1", "testuser", "Admin");

            // Act
            var principal = _jwtService.ValidateToken(token);

            // Assert
            Assert.NotNull(principal);
            Assert.Equal("1", principal.FindFirstValue(ClaimTypes.NameIdentifier));
            Assert.Equal("testuser", principal.FindFirstValue(ClaimTypes.Name));
            Assert.Equal("Admin", principal.FindFirstValue(ClaimTypes.Role));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("completely.invalid.token")]
        public void ValidateToken_WithInvalidToken_ReturnsNull(string invalidToken)
        {
            // Act
            var principal = _jwtService.ValidateToken(invalidToken);

            // Assert
            Assert.Null(principal);
        }
    }
}