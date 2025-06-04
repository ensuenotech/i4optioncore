using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace i4optioncore.Repositories
{
    public class CacheBL : ICacheBL
    {
        private readonly IMemoryCache cache;

        public CacheBL(IMemoryCache _cache)
        {
            cache = _cache;
        }
        public string GetValue(string Key)
        {
            var value = cache.Get(Key);
            return value?.ToString();
        }
        public bool SetValue(string Key, string Value, double? expiryMinutes=60*24)
        {
            var dateTime = DateTime.Now + (expiryMinutes.HasValue ? TimeSpan.FromMinutes(expiryMinutes.Value) : new TimeSpan());
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            DateTimeOffset utcTime2 = dateTime;
            cache.Set(Key, Value, utcTime2);
            return true;
        }
        public void Delete(string Key)
        {
            cache.Remove(Key);
        }
    }
}
