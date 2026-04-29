using System;

namespace LABsistem.Bll.Services
{
    public interface IRevokedTokenStore
    {
        Task<bool> IsRevokedAsync(string jti, CancellationToken cancellationToken = default);
        Task RevokeAsync(string jti, DateTime expiresAtUtc, CancellationToken cancellationToken = default);
    }
}
