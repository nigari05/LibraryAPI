using AutoMapper;
using Business.Concrete;
using Business.Mapping;
using Core.Utilities.Caching;
using Core.Utilities.Pagination;
using DataAccess.Absract;
using Entities.Concrete;
using Entities.DTOs.CategoryDTOs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Tests.Managers
{
    public class CategoryManagerCachingTests
    {
        private readonly Mock<ICategoryDAL> _categoryDalMock;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly CategoryManager _categoryManager;

        public CategoryManagerCachingTests()
        {
            _categoryDalMock = new Mock<ICategoryDAL>();

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<CategoryProfile>(), NullLoggerFactory.Instance);
            _mapper = mapperConfig.CreateMapper();

            // Real MemoryCache istifadə olunur (mock deyil) ki, faktiki keş davranışı
            // (ikinci çağırışın DAL-a getmədən keşdən qayıtması) yoxlanılsın.

            _cacheService = new MemoryCacheService(new MemoryCache(new MemoryCacheOptions()));

            _categoryManager = new CategoryManager(_categoryDalMock.Object, _mapper, _cacheService);
        }
        [Fact]
        public async Task GetAllCategoriesAsync_Should_Call_DAL_Only_Once_When_Cached()
        {
            var pagination = new PaginationParameters { PageNumber = 1, PageSize = 10 };

            var categories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "Fiction" }
            };

            _categoryDalMock
                .Setup(x => x.GetAllAsync(pagination))
                .ReturnsAsync(categories);

            var first = await _categoryManager.GetAllCategoriesAsync(pagination);
            var second = await _categoryManager.GetAllCategoriesAsync(pagination);

            Assert.True(first.Success);
            Assert.True(second.Success);
            Assert.Single(second.Data!);

            // İkinci çağırış keşdən qayıtmalıdır - DAL yalnız BİR dəfə çağırılmalıdır.
            _categoryDalMock.Verify(x => x.GetAllAsync(pagination), Times.Once);
        }

        [Fact]
        public async Task AddAsync_Should_Invalidate_Cache_So_Next_Read_Hits_DAL_Again()
        {
            var pagination = new PaginationParameters { PageNumber = 1, PageSize = 10 };

            _categoryDalMock
                .SetupSequence(x => x.GetAllAsync(pagination))
                .ReturnsAsync(new List<Category>())
                .ReturnsAsync(new List<Category> { new Category { Id = Guid.NewGuid(), Name = "Sci-Fi" } });

            var beforeAdd = await _categoryManager.GetAllCategoriesAsync(pagination);
            Assert.Empty(beforeAdd.Data!);

            await _categoryManager.AddAsync(new CreateCategoryDTO { Name = "Sci-Fi" });

            var afterAdd = await _categoryManager.GetAllCategoriesAsync(pagination);

            Assert.Single(afterAdd.Data!);
            // AddAsync keşi invalidasiya etdiyi üçün DAL YENİDƏN çağırılmalıdır (cəmi 2 dəfə).
            _categoryDalMock.Verify(x => x.GetAllAsync(pagination), Times.Exactly(2));
        }

        [Fact]
        public async Task UpdateAsync_Should_Invalidate_Cache_So_Next_Read_Reflects_The_Update()
        {
            var pagination = new PaginationParameters { PageNumber = 1, PageSize = 10 };
            var categoryId = Guid.NewGuid();
            var existingCategory = new Category { Id = categoryId, Name = "Fiction" };

            _categoryDalMock
                .SetupSequence(x => x.GetAllAsync(pagination))
                .ReturnsAsync(new List<Category> { new Category { Id = categoryId, Name = "Fiction" } })
                .ReturnsAsync(new List<Category> { new Category { Id = categoryId, Name = "Fiction & Fantasy" } });

            _categoryDalMock
                .Setup(x => x.GetByIdAsync(categoryId))
                .ReturnsAsync(existingCategory);

            var beforeUpdate = await _categoryManager.GetAllCategoriesAsync(pagination);
            Assert.Equal("Fiction", beforeUpdate.Data!.Single().Name);

            await _categoryManager.UpdateAsync(categoryId, new UpdateCategoryDTO { Name = "Fiction & Fantasy" });

            var afterUpdate = await _categoryManager.GetAllCategoriesAsync(pagination);

            // Köhnəlmiş (stale) keşlənmiş dəyər deyil, YENİLƏNMİŞ məlumat qayıtmalıdır.
            Assert.Equal("Fiction & Fantasy", afterUpdate.Data!.Single().Name);
            _categoryDalMock.Verify(x => x.GetAllAsync(pagination), Times.Exactly(2));
        }

        [Fact]
        public async Task DeleteAsync_Should_Invalidate_Cache_So_Next_Read_Excludes_Deleted_Item()
        {
            var pagination = new PaginationParameters { PageNumber = 1, PageSize = 10 };
            var categoryId = Guid.NewGuid();
            var category = new Category { Id = categoryId, Name = "Deprecated" };

            _categoryDalMock
                .SetupSequence(x => x.GetAllAsync(pagination))
                .ReturnsAsync(new List<Category> { category })
                .ReturnsAsync(new List<Category>());

            _categoryDalMock
                .Setup(x => x.GetByIdAsync(categoryId))
                .ReturnsAsync(category);

            var beforeDelete = await _categoryManager.GetAllCategoriesAsync(pagination);
            Assert.Single(beforeDelete.Data!);

            await _categoryManager.DeleteAsync(categoryId);

            var afterDelete = await _categoryManager.GetAllCategoriesAsync(pagination);

            // Silinmiş element köhnəlmiş keşdən deyil, YENİDƏN DAL-dan gətirilməli məlumatda olmamalıdır.
            Assert.Empty(afterDelete.Data!);
            _categoryDalMock.Verify(x => x.GetAllAsync(pagination), Times.Exactly(2));
        }

        [Fact]
        public async Task Invalidation_Should_Only_Clear_Entries_Matching_The_Category_Prefix()
        {
            // Digər (kateqoriyaya aid olmayan) keş açarları yazı əməliyyatından TƏSİRLƏNMƏMƏLİDİR -
            // RemoveByPrefix yalnız "categories:all:*" ilə uyğun gələn açarları silməlidir.
            var unrelatedCallCount = 0;
            var unrelatedValue = await _cacheService.GetOrCreateAsync("books:all:1:10", async () =>
            {
                unrelatedCallCount++;
                return await Task.FromResult("cached-books-payload");
            });

            var pagination = new PaginationParameters { PageNumber = 1, PageSize = 10 };
            _categoryDalMock
                .Setup(x => x.GetAllAsync(pagination))
                .ReturnsAsync(new List<Category>());

            await _categoryManager.GetAllCategoriesAsync(pagination);
            await _categoryManager.AddAsync(new CreateCategoryDTO { Name = "New" });

            // Aidiyyatsız açar hələ də keşdə qalmalı, factory YENİDƏN çağırılmamalıdır.
            await _cacheService.GetOrCreateAsync("books:all:1:10", async () =>
            {
                unrelatedCallCount++;
                return await Task.FromResult("cached-books-payload");
            });

            Assert.Equal(1, unrelatedCallCount);
        }
    }
}
