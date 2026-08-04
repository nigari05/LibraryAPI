using AutoMapper;
using Business.Abstract;
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
        public CategoryManager(ICategoryDAL categoryDAL, IMapper mapper)
        {
            _categoryDAL = categoryDAL;
            _mapper = mapper;
        }

        public async Task<IResult> AddAsync(CreateCategoryDTO entity)
        {
            var category = _mapper.Map<Category>(entity);

            await _categoryDAL.AddAsync(category);
            return new SuccessResult(HttpStatusCode.Created, "Category added successfully.");
        }

        public async Task<IResult> DeleteAsync(Guid id)
        {
            var category = await _categoryDAL.GetByIdAsync(id);

            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            await _categoryDAL.DeleteAsync(category);
            return new SuccessResult(HttpStatusCode.NoContent, "Category deleted successfully.");

        }

        

        public async Task<IDataResult<List<GetCategoryDTO>>> GetAllCategoriesAsync(PaginationParameters pagination)
        {
            var categories = await _categoryDAL.GetAllAsync(pagination);

            var categoryDTOs = _mapper.Map<List<GetCategoryDTO>>(categories);

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
            return new SuccessResult(HttpStatusCode.NoContent, "Category updated successfully.");
        
         }
    }
}
