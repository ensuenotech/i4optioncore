using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace i4optioncore.Repositories
{
    public class RedisBL : IRedisBL
    {
        private readonly IConnectionMultiplexer _redis;
        IDatabase db;
        public RedisBL(IConnectionMultiplexer redis)
        {
            _redis = redis;
            db = _redis.GetDatabase();
        }

        public async Task<string> GetValue(string Key)
        {
            var value = await db.StringGetAsync(Key);
            if (value != RedisValue.Null)
                return value.ToString();
            return null;
        }
        public async Task SetValue(string Key, string Value)
        {
            await db.StringSetAsync(Key, Value);
        }
        public async Task SetValueWithExpiry(string Key, string Value, double? expiryMinutes)
        {
            await db.StringSetAsync(Key, Value, expiryMinutes.HasValue ? TimeSpan.FromMinutes(expiryMinutes.Value) : null);
            //if (expiryMinutes.HasValue)
            //{
            //    await db.StringGetSetExpiryAsync(Key, DateTime.Now.AddMinutes(expiryMinutes.Value));
            //}
        }

        public async Task<object> GetHash(string Key)
        {
            var value = await db.HashGetAllAsync(Key);
            return value.ToList();
        }
        public async Task<bool> SetHash(string Key, string Value)
        {
            var hashFields = new List<HashEntry>();
            await db.HashSetAsync(Key, hashFields.ToArray());
            return true;
        }
        public async Task<bool> DeleteKey(string Key)
        {
            await db.KeyDeleteAsync(Key);
            return true;
        }
        public async Task<bool> FlushAll()
        {
            var endpoints = _redis.GetEndPoints();
            var server = _redis.GetServer(endpoints.First());

            var keys = server.Keys();
            foreach (var key in keys)
            {
                await db.KeyDeleteAsync(key);
            }
            return true;
        }
    }
}
