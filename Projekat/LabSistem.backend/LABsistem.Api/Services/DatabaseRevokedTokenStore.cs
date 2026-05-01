using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LABsistem.Application.Services
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

            var revokedAtUtc = DateTime.UtcNow;
            var normalizedExpiresAtUtc = expiresAtUtc <= revokedAtUtc
                ? revokedAtUtc.AddMinutes(1)
                : expiresAtUtc;

            var existingToken = await _dbContext.RevokedAccessTokens
                .FirstOrDefaultAsync(x => x.Jti == jti, cancellationToken);

            if (existingToken is not null)
            {
                existingToken.RevokedAtUtc = revokedAtUtc;
                existingToken.ExpiresAtUtc = normalizedExpiresAtUtc;
                await _dbContext.SaveChangesAsync(cancellationToken);
                return;
            }

            var revokedToken = new RevokedAccessToken
            {
                Jti = jti,
                RevokedAtUtc = revokedAtUtc,
                ExpiresAtUtc = normalizedExpiresAtUtc
            };

            _dbContext.RevokedAccessTokens.Add(revokedToken);

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                _dbContext.Entry(revokedToken).State = EntityState.Detached;

                existingToken = await _dbContext.RevokedAccessTokens
                    .FirstOrDefaultAsync(x => x.Jti == jti, cancellationToken);

                if (existingToken is null)
                {
                    throw;
                }

                existingToken.RevokedAtUtc = revokedAtUtc;
                existingToken.ExpiresAtUtc = normalizedExpiresAtUtc;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
