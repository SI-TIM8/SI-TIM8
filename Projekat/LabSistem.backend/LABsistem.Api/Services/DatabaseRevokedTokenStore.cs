using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LABsistem.Bll.Services
{
    public class DatabaseRevokedTokenStore : IRevokedTokenStore
    {
        private readonly LabSistemDbContext _dbContext;

        public DatabaseRevokedTokenStore(LabSistemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsRevokedAsync(string jti, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(jti))
            {
                return false;
            }

            return await _dbContext.RevokedAccessTokens
                .AnyAsync(
                    x => x.Jti == jti && x.ExpiresAtUtc > DateTime.UtcNow,
                    cancellationToken);
        }

        public async Task RevokeAsync(string jti, DateTime expiresAtUtc, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(jti))
            {
                return;
            }

            var existingToken = await _dbContext.RevokedAccessTokens
                .FirstOrDefaultAsync(x => x.Jti == jti, cancellationToken);

            if (existingToken is not null)
            {
                existingToken.RevokedAtUtc = DateTime.UtcNow;
                existingToken.ExpiresAtUtc = expiresAtUtc <= DateTime.UtcNow
                    ? DateTime.UtcNow.AddMinutes(1)
                    : expiresAtUtc;
                await _dbContext.SaveChangesAsync(cancellationToken);
                return;
            }

            _dbContext.RevokedAccessTokens.Add(new RevokedAccessToken
            {
                Jti = jti,
                RevokedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = expiresAtUtc <= DateTime.UtcNow
                    ? DateTime.UtcNow.AddMinutes(1)
                    : expiresAtUtc
            });

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
