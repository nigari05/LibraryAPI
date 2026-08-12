using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities.Caching
{
    /// <summary>
    /// ICacheService-in in-memory (IMemoryCache) implementasiyası. Tək instansiyalı
    /// (single-server) mühit üçün uyğundur; bir neçə server arasında paylaşılan keş
    /// lazım olarsa, eyni ICacheService kontraktı arxasında Redis-əsaslı
    /// (IDistributedCache) implementasiya ilə əvəz oluna bilər - çağıran kodda heç bir
    /// dəyişiklik tələb olunmadan.
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        // IMemoryCache prefiks üzrə "hansı açarlar keşlənib" məlumatını özü saxlamır,
        // ona görə RemoveByPrefix üçün açarları özümüz izləyirik.
        private readonly ConcurrentDictionary<string, byte> _trackedKeys = new();

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public  async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            if (_cache.TryGetValue(key, out T? cached) && cached != null)
                return cached;

            var value = await factory();

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
            };
            options.RegisterPostEvictionCallback((evictedKey, _, _, _) => _trackedKeys.TryRemove((string)evictedKey, out _));

            _cache.Set(key, value, options);
            _trackedKeys.TryAdd(key, 0);

            return value;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
            _trackedKeys.TryRemove(key, out _);
        }

        public void RemoveByPrefix(string prefix)
        {
            var keysToRemove = _trackedKeys.Keys
                 .Where(k => k.StartsWith(prefix, StringComparison.Ordinal))
                 .ToList();

            foreach (var key in keysToRemove)
                Remove(key);
        }
    }
}

