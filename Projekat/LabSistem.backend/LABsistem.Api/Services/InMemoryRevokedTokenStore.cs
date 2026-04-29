using System;
using Microsoft.Extensions.Caching.Memory;

namespace LABsistem.Bll.Services
{
    public class InMemoryRevokedTokenStore : IRevokedTokenStore
    {
        private readonly IMemoryCache _cache;

        public InMemoryRevokedTokenStore(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<bool> IsRevokedAsync(string jti, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(jti))
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(_cache.TryGetValue(GetCacheKey(jti), out _));
        }

        public Task RevokeAsync(string jti, DateTime expiresAtUtc, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(jti))
            {
                return Task.CompletedTask;
            }

            var effectiveExpiration = expiresAtUtc <= DateTime.UtcNow
                ? DateTime.UtcNow.AddMinutes(1)
                : expiresAtUtc;

            _cache.Set(
                GetCacheKey(jti),
                true,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = effectiveExpiration
                });

            return Task.CompletedTask;
        }

        private static string GetCacheKey(string jti) => $"revoked-token:{jti}";
    }
}
