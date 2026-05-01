using System.Security.Claims;

namespace LABsistem.Application.Services
{
    public interface IJwtService
    {
        string GenerateToken(string userId, string username, string role);
        string GenerateRefreshToken();
        DateTime GetTokenExpirationUtc(string token);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
