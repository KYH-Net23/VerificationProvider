using System;
using System.Runtime.Caching;
using VerificationProvider.Interfaces;

namespace VerificationProvider.Services
{
    public class PassCodeService : IPassCodeService
    {
        private readonly MemoryCache _cache;

        public PassCodeService(MemoryCache cache)
        {
            _cache = cache;
        }

        public bool ValidatePassCode(string passCode, string userId)
        {
            var key = _cache.Get(userId) as string;
            return key == null;
        }

        public string GeneratePasscode(string userId)
        {
            var code = GenerateRandomPassCode();

            _cache.Set(userId, code, new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5)
            });

            return code;
        }

        private string GenerateRandomPassCode()
        {
            return "123456";
        }

        public string RetrievePasscode(string key)
        {
            return _cache.Get(key) as string;
        }
    }
}