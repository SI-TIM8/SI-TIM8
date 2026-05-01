using System.Security.Claims;
using LABsistem.Bll.DTOs.Auth;
using LABsistem.Domain.Enums;

namespace LABsistem.Bll.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, string? ipAddress = null, string? deviceInfo = null);
        Task<LoginResponseDto?> RefreshAsync(string refreshToken, string? ipAddress = null, string? deviceInfo = null);
        Task<ProfileResponseDto?> GetProfileAsync(int userId);
        Task<List<UserListItemDto>> GetUsersAsync();
        Task<(bool Success, string Message, ProfileResponseDto? Profile)> UpdateProfileAsync(int userId, UpdateProfileRequestDto request);
        Task<(bool Success, string Message, UserListItemDto? User)> UpdateUserAsync(int currentUserId, int targetUserId, UpdateManagedUserRequestDto request);
        Task<(bool Success, string Message, UserListItemDto? User)> ActivateUserAsync(int currentUserId, int targetUserId);
        Task<(bool Success, string Message, UserListItemDto? User)> DeactivateUserAsync(int currentUserId, int targetUserId);
        Task<(bool Success, string Message)> ChangePasswordAsync(int userId, ChangePasswordRequestDto request);
        Task<(bool Success, string Message)> CreateUserAsync(RegisterRequestDto request, UlogaKorisnika uloga);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);
        Task<bool> IsUserActiveAsync(int userId);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
