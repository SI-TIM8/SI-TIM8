using System.Security.Claims;

namespace LABsistem.Bll.Services
{
    public interface IJwtService
    {
        string GenerateToken(string userId, string username, string role);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
