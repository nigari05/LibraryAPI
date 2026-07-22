using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities.Pagination
{
    public class PaginationParameters
    {
        private const int MaxPageSize = 50;

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        public string? SortBy { get; set; }

        public bool IsDescending { get; set; } = false;
    }
}
