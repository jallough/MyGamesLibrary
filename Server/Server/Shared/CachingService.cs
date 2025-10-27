using Microsoft.Extensions.Caching.Memory;

namespace Server.Shared
{
    public class CachingService : ICachingService
    {
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _memoryCacheEntryOptions;

        public CachingService(IMemoryCache cache)
        {
            _cache = cache;
            _memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(1))
                .SetSlidingExpiration(TimeSpan.FromDays(7))
                .SetPriority(CacheItemPriority.Normal);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            _cache.TryGetValue(key, out T? value);
            return await Task.FromResult(value);
        }

        public void UpdateAsync<T>(string key, T value)
        {
            _cache.Set(key, value, _memoryCacheEntryOptions);
        }
    }
}
