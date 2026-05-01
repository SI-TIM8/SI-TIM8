using System.Security.Claims;
using LABsistem.Application.Models;
using LABsistem.Application.Services;

namespace LABsistem.Tests.Unit
{
    public class JwtServiceTests
    {
        private readonly JwtService _jwtService;

        public JwtServiceTests()
        {
            _jwtService = new JwtService(new JwtSettings
            {
                Key = "TestSuperSecretKeyThatMustBeLongEnough123!",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpireMinutes = 60,
                RefreshExpireDays = 7
            });
        }

        [Fact]
        public void GenerateToken_WithValidData_ReturnsToken()
        {
            var token = _jwtService.GenerateToken("1", "testuser", "Admin");

            Assert.False(string.IsNullOrWhiteSpace(token));
            Assert.Contains(".", token);
        }

        [Fact]
        public void GenerateRefreshToken_CalledTwice_ReturnsDifferentTokens()
        {
            var first = _jwtService.GenerateRefreshToken();
            var second = _jwtService.GenerateRefreshToken();

            Assert.False(string.IsNullOrWhiteSpace(first));
            Assert.False(string.IsNullOrWhiteSpace(second));
            Assert.NotEqual(first, second);
        }

        [Fact]
        public void ValidateToken_WithValidToken_ReturnsClaimsPrincipal()
        {
            var token = _jwtService.GenerateToken("1", "testuser", "Admin");

            var principal = _jwtService.ValidateToken(token);

            Assert.NotNull(principal);
            Assert.Equal("1", principal.FindFirstValue(ClaimTypes.NameIdentifier));
            Assert.Equal("testuser", principal.FindFirstValue(ClaimTypes.Name));
            Assert.Equal("Admin", principal.FindFirstValue(ClaimTypes.Role));
        }

        [Fact]
        public void GetTokenExpirationUtc_WithValidToken_ReturnsFutureUtcDate()
        {
            var beforeGeneration = DateTime.UtcNow;
            var token = _jwtService.GenerateToken("1", "testuser", "Admin");

            var expiresAtUtc = _jwtService.GetTokenExpirationUtc(token);

            Assert.True(expiresAtUtc > beforeGeneration);
        }

        [Theory]
        [InlineData("")]
        [InlineData("completely.invalid.token")]
        public void ValidateToken_WithInvalidToken_ReturnsNull(string invalidToken)
        {
            var principal = _jwtService.ValidateToken(invalidToken);

            Assert.Null(principal);
        }

        [Fact]
        public void ValidateToken_WithNullToken_ReturnsNull()
        {
            var principal = _jwtService.ValidateToken(null!);

            Assert.Null(principal);
        }
    }
}
