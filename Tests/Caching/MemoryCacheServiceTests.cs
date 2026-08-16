using Core.Utilities.Caching;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Tests.Caching
{
    /// <summary>
    /// Bu testlər
    /// birbaşa MemoryCacheService səviyyəsində Remove/RemoveByPrefix-in DƏQİQ (yalnız
    /// uyğun açarları silən, digərlərinə toxunmayan) işlədiyini yoxlayır - Business
    /// qatındakı CategoryManagerCachingTests isə eyni davranışı manager səviyyəsində
    /// (Add/Update/Delete zamanı) təsdiqləyir.
    /// </summary>
    public class MemoryCacheServiceTests
    {
        private static MemoryCacheService CreateService() => new(new MemoryCache(new MemoryCacheOptions()));

        [Fact]
        public async Task GetOrCreateAsync_Should_Call_Factory_Only_Once_For_The_Same_Key()
        {
            var cache = CreateService();
            var callCount = 0;

            Task<string> Factory()
            {
                callCount++;
                return Task.FromResult("value");
            }

            await cache.GetOrCreateAsync("key-1", Factory);
            await cache.GetOrCreateAsync("key-1", Factory);

            Assert.Equal(1, callCount);
        }

        [Fact]
        public async Task Remove_Should_Only_Clear_The_Specified_Key()
        {
            var cache = CreateService();

            await cache.GetOrCreateAsync("key-1", () => Task.FromResult("value-1"));
            await cache.GetOrCreateAsync("key-2", () => Task.FromResult("value-2"));

            cache.Remove("key-1");

            var key1CallCount = 0;
            var key2CallCount = 0;

            await cache.GetOrCreateAsync("key-1", () => { key1CallCount++; return Task.FromResult("value-1"); });
            await cache.GetOrCreateAsync("key-2", () => { key2CallCount++; return Task.FromResult("value-2"); });

            Assert.Equal(1, key1CallCount);
            Assert.Equal(0, key2CallCount);
        }

        [Fact]
        public async Task RemoveByPrefix_Should_Clear_All_Matching_Keys_And_Nothing_Else()
        {
            var cache = CreateService();

            await cache.GetOrCreateAsync("categories:all:1:10", () => Task.FromResult("page-1"));
            await cache.GetOrCreateAsync("categories:all:2:10", () => Task.FromResult("page-2"));
            await cache.GetOrCreateAsync("books:all:1:10", () => Task.FromResult("books-page-1"));

            cache.RemoveByPrefix("categories:all:");

            var categoriesPage1CallCount = 0;
            var categoriesPage2CallCount = 0;
            var booksCallCount = 0;

            await cache.GetOrCreateAsync("categories:all:1:10", () => { categoriesPage1CallCount++; return Task.FromResult("page-1"); });
            await cache.GetOrCreateAsync("categories:all:2:10", () => { categoriesPage2CallCount++; return Task.FromResult("page-2"); });
            await cache.GetOrCreateAsync("books:all:1:10", () => { booksCallCount++; return Task.FromResult("books-page-1"); });

            Assert.Equal(1, categoriesPage1CallCount);
            Assert.Equal(1, categoriesPage2CallCount);
            Assert.Equal(0, booksCallCount);
        }

        [Fact]
        public async Task RemoveByPrefix_With_No_Matching_Keys_Should_Not_Throw_Or_Affect_Other_Keys()
        {
            var cache = CreateService();

            await cache.GetOrCreateAsync("categories:all:1:10", () => Task.FromResult("page-1"));

            var exception = await Record.ExceptionAsync(() =>
            {
                cache.RemoveByPrefix("members:all:");
                return Task.CompletedTask;
            });

            Assert.Null(exception);

            var callCount = 0;
            await cache.GetOrCreateAsync("categories:all:1:10", () => { callCount++; return Task.FromResult("page-1"); });

            Assert.Equal(0, callCount);
        }
    }
}
