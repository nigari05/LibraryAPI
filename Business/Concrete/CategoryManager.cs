using AutoMapper;
using Business.Abstract;
using Core.Utilities.Caching;
using Core.Utilities.Pagination;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete.SuccessResults;
using DataAccess.Absract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Entities.DTOs.BookDTOs;
using Entities.DTOs.CategoryDTOs;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Business.Concrete
{
    public class CategoryManager : ICategoryService
    {
        private readonly ICategoryDAL _categoryDAL;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        // Bütün "categories:all:..." açarları bu prefiksdən başlayır - yazı əməliyyatı
        // (Add/Update/Delete) olduqda hamısı bir dəfəyə invalidasiya olunur.
        private const string CacheKeyPrefix = "categories:all:";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
        public CategoryManager(ICategoryDAL categoryDAL, IMapper mapper, ICacheService cacheService)
        {
            _categoryDAL = categoryDAL;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<IResult> AddAsync(CreateCategoryDTO entity)
        {
            var category = _mapper.Map<Category>(entity);

            await _categoryDAL.AddAsync(category);
            _cacheService.RemoveByPrefix(CacheKeyPrefix);
            return new SuccessResult(HttpStatusCode.Created, "Category added successfully.");
        }

        public async Task<IResult> DeleteAsync(Guid id)
        {
            var category = await _categoryDAL.GetByIdAsync(id);

            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            await _categoryDAL.DeleteAsync(category);
            _cacheService.RemoveByPrefix(CacheKeyPrefix);
            return new SuccessResult(HttpStatusCode.NoContent, "Category deleted successfully.");

        }

        /// <summary>
        /// Kateqoriya siyahısı çox nadir dəyişən, tez-tez oxunan (read-heavy) məlumatdır -
        /// buna görə klassik keşləmə namizədidir. Nəticə səhifələnmə/sıralama
        /// parametrlərinə görə fərqli açarlarla keşlənir; AddAsync/UpdateAsync/DeleteAsync
        /// çağırıldıqda bütün "categories:all:*" açarları invalidasiya olunur ki, köhnəlmiş
        /// (stale) məlumat qaytarılmasın.
        /// </summary>

        public async Task<IDataResult<List<GetCategoryDTO>>> GetAllCategoriesAsync(PaginationParameters pagination)
        {
            var cacheKey = $"{CacheKeyPrefix}{pagination.PageNumber}:{pagination.PageSize}:{pagination.SortBy}:{pagination.IsDescending}";

            var categoryDTOs = await _cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                var categories = await _categoryDAL.GetAllAsync(pagination);
                return _mapper.Map<List<GetCategoryDTO>>(categories);
            }, CacheDuration);

            return new SuccessDataResult<List<GetCategoryDTO>>(HttpStatusCode.OK, categoryDTOs);


        }

        public async Task<IDataResult<GetCategoryDTO?>> GetByIdAsync(Guid id)
        {
            var category = await _categoryDAL.GetByIdAsync(id);

            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            var dto = _mapper.Map<GetCategoryDTO>(category);

            return new SuccessDataResult<GetCategoryDTO?>(HttpStatusCode.OK, dto);
        }

        public async Task<IResult> UpdateAsync(Guid id, UpdateCategoryDTO entity)
        {
            var category = await _categoryDAL.GetByIdAsync(id);

            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            _mapper.Map(entity, category);

            await _categoryDAL.UpdateAsync(category);
            _cacheService.RemoveByPrefix(CacheKeyPrefix);
            return new SuccessResult(HttpStatusCode.NoContent, "Category updated successfully.");
        
         }
    }
}
