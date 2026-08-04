using AutoMapper;
using Entities.Concrete;
using Entities.DTOs.CategoryDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Mapping
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<CreateCategoryDTO, Category>();
            CreateMap<UpdateCategoryDTO, Category>();
            CreateMap<Category, GetCategoryDTO>()
                .ForMember(dest => dest.BookTitles,
                    opt => opt.MapFrom(src => src.Books != null
                        ? src.Books.Select(b => b.Title).ToList()
                        : new List<string>()));
        }
    }
}
