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
    }
}
