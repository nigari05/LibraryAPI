using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTOs.CategoryDTOs
{
    public class GetCategoryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public List<string> BookTitles { get; set; } = new();
    }
}
