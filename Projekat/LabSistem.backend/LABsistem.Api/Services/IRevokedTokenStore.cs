using System;

namespace LABsistem.Bll.Services
{
    public interface IRevokedTokenStore
    {
        bool IsRevoked(string jti);
        void Revoke(string jti, DateTime expiresAtUtc);
    }
}
