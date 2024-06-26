﻿using ChepoAPI.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace ChepoAPI.Services
{
    public class CacheService : ICacheService
    {
        private readonly IConfiguration _configuration;
        private readonly ConnectionMultiplexer _redis;
        private IDatabase _db;

        public CacheService(IConfiguration configuration)
        {
            _configuration = configuration;
            string redisUrl = _configuration["RedisURL"];
            _redis = ConnectionMultiplexer.Connect(redisUrl);

            _db = _redis.GetDatabase();
        }

        public T GetData<T>(string key)
        {
            var value = _db.StringGet(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            return default;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
            var isSet = _db.StringSet(key, JsonConvert.SerializeObject(value), expiryTime);
            return isSet;
        }

        public bool SetData<T>(string key, T value)
        {
            var isSet = _db.StringSet(key, JsonConvert.SerializeObject(value));
            return isSet;
        }

        public object RemoveData(string key)
        {
            bool _isKeyExist = _db.KeyExists(key);
            if (_isKeyExist == true)
            {
                return _db.KeyDelete(key);
            }
            return false;
        }
    }
}
