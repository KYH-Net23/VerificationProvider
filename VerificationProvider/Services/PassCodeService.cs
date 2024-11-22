using System;
using System.Linq;
using System.Runtime.Caching;
using VerificationProvider.Interfaces;

namespace VerificationProvider.Services
{
    public class PassCodeService : IPassCodeService
    {
        private readonly MemoryCache _cache;
        private const int LengthOfPassCode = 6;
        private const int CacheDurationInMinutes = 5;

        public PassCodeService(MemoryCache cache)
        {
            _cache = cache;
        }

        public bool ValidatePassCode(string passCode, string userId)
        {
            return _cache.Get(userId) as string == passCode;
        }

        public string GeneratePasscode(string userId)
        {
            var code = GenerateRandomPassCode();

            SavePasscodeInMemoryCache(userId, code);

            return code;
        }

        private void SavePasscodeInMemoryCache(string userId, string code)
        {
            _cache.Set(userId, code, new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(CacheDurationInMinutes)
            });
        }

        private static string GenerateRandomPassCode()
        {
            var random = new Random();
            return string.Concat(Enumerable.Range(0, LengthOfPassCode).Select(_ => random.Next(0, 10)));
        }

        public string RetrievePasscode(string key)
        {
            return _cache.Get(key) as string;
        }
    }
}