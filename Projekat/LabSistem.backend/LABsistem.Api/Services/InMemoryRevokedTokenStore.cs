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

        public bool IsRevoked(string jti)
        {
            if (string.IsNullOrWhiteSpace(jti))
            {
                return false;
            }

            return _cache.TryGetValue(GetCacheKey(jti), out _);
        }

        public void Revoke(string jti, DateTime expiresAtUtc)
        {
            if (string.IsNullOrWhiteSpace(jti))
            {
                return;
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
        }

        private static string GetCacheKey(string jti) => $"revoked-token:{jti}";
    }
}
