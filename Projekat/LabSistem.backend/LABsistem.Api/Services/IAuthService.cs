using System.Security.Claims;
using LABsistem.Bll.DTOs.Auth;
using LABsistem.Domain.Enums;

namespace LABsistem.Bll.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
        Task<ProfileResponseDto?> GetProfileAsync(int userId);
        Task<List<UserListItemDto>> GetUsersAsync();
        Task<(bool Success, string Message, ProfileResponseDto? Profile)> UpdateProfileAsync(int userId, UpdateProfileRequestDto request);
        Task<(bool Success, string Message)> ChangePasswordAsync(int userId, ChangePasswordRequestDto request);
        Task<(bool Success, string Message)> CreateUserAsync(RegisterRequestDto request, UlogaKorisnika uloga);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
