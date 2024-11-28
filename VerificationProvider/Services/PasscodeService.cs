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
        private const int CacheDurationInMinutes = 30;
        private static readonly Random Random = new Random();

        public PasscodeService(MemoryCache cache)
        {
            _cache = cache;
        }

        public bool ValidatePasscodeAndUserId(string passCode, string email)
        {
            return _cache.Get(email) is string cachedPasscode && cachedPasscode == passCode;
        }

        public string GeneratePasscode(string email)
        {
            var code = GenerateRandomPassCode();

            RemovePasscode(email);

            SaveNewPasscodeInMemoryCache(email, code);

            return code;
        }

        public string RetrievePasscode(string key)
        {
            return _cache.Get(key) as string;
        }

        public void RemovePasscode(string key)
        {
            _cache.Remove(key);
        }

        private void SaveNewPasscodeInMemoryCache(string email, string code)
        {
            _cache.Set(email, code, new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(CacheDurationInMinutes)
            });
        }

        private static string GenerateRandomPassCode()
        {
            return string.Concat(Enumerable.Range(0, LengthOfPassCode).Select(_ => Random.Next(0, 10)));
        }
    }
}