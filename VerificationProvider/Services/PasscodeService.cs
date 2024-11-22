using System;
using System.Linq;
using System.Runtime.Caching;
using VerificationProvider.Interfaces;

namespace VerificationProvider.Services
{
    public class PasscodeService : IPasscodeService
    {
        private readonly MemoryCache _cache;
        private const int LengthOfPassCode = 6;
        private const int CacheDurationInMinutes = 5;
        private static readonly Random Random = new Random();

        public PasscodeService(MemoryCache cache)
        {
            _cache = cache;
        }

        public bool ValidatePasscodeAndUserId(string passCode, string userId)
        {
            return _cache.Get(userId) is string cachedPasscode && cachedPasscode == passCode;
        }

        public string GeneratePasscode(string userId)
        {
            var code = GenerateRandomPassCode();

            RemovePasscode(userId);

            SaveNewPasscodeInMemoryCache(userId, code);

            return code;
        }

        private void SaveNewPasscodeInMemoryCache(string userId, string code)
        {
            _cache.Set(userId, code, new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(CacheDurationInMinutes)
            });
        }

        private static string GenerateRandomPassCode()
        {
            return string.Concat(Enumerable.Range(0, LengthOfPassCode).Select(_ => Random.Next(0, 10)));
        }

        public string RetrievePasscode(string key)
        {
            return _cache.Get(key) as string;
        }

        public void RemovePasscode(string key)
        {
            _cache.Remove(key);
        }
    }
}