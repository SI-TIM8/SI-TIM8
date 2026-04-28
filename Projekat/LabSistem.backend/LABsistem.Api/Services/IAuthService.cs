using System.Security.Claims;
using LABsistem.Bll.DTOs.Auth;
using LABsistem.Domain.Enums;

namespace LABsistem.Bll.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
        Task<(bool Success, string Message)> CreateUserAsync(RegisterRequestDto request, UlogaKorisnika uloga);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
