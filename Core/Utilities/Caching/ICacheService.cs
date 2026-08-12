using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities.Caching
{
    /// <summary>
    /// Oxu-yönümlü (read-heavy) endpoint-lər üçün ümumi keşləmə kontraktı - Spring-in
    /// @Cacheable/@CacheEvict abstraksiyasının .NET ekvivalenti. Konkret implementasiya
    /// in-memory (IMemoryCache) və ya Redis (IDistributedCache) ola bilər; çağıran kod
    /// (Business qatı) hansı provider istifadə olunduğunu bilmir.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Açar keşdə varsa onu qaytarır; yoxdursa factory-ni işə salıb nəticəni keşləyir.
        /// </summary>
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);

        /// <summary>Tək bir açarı keşdən silir.</summary>
        void Remove(string key);

        /// <summary>Verilmiş prefiks ilə başlayan BÜTÜN açarları keşdən silir (invalidasiya üçün).</summary>
        void RemoveByPrefix(string prefix);
    }
}
