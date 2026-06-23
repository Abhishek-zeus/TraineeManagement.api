using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using TraineeManagement.myapp.Interfaces;
using TraineeManagement.myapp.Utility;


namespace TraineeManagement.myapp.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = await _cache.GetStringAsync(key, cancellationToken);
                if (string.IsNullOrEmpty(json))
                {
                    _logger.LogInformation("Cache MISS for key: {Key}", key);
                    return default;
                }
                _logger.LogInformation("Cache HIT for key: {Key}", key);
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                //If Redis fails
                _logger.LogWarning(ex, "Cache GET failed for key: {Key}. Falling back to source.", key);
                return default; //Treat as a Normal Miss
            }

        }


        public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonSerializer.Serialize(value);
                //to setup TTL
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = ttl
                };
                await _cache.SetStringAsync(key, json, options, cancellationToken);
                _logger.LogInformation("Cache SET for key: {Key}, TTL: {Ttl}s", key, ttl.TotalSeconds);
            }
            catch (Exception ex)
            {
                //If Redis fails
                _logger.LogWarning(ex, "Cache SET failed for key: {Key}. Continuing without caching.", key);
                return; //Treat as a Normal Miss
            }



        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                await _cache.RemoveAsync(key, cancellationToken);
                _logger.LogInformation("Cache INVALIDATED for key: {Key}", key);
            }
            catch (Exception ex)
            {
                //If Redis fails
                _logger.LogWarning(ex, "Cache INVALIDATE failed for key: {Key}", key);
                return; //Treat as a Normal Miss
            }


        }


    }
}