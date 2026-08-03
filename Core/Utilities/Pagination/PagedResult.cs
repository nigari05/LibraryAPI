using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities.Pagination
{
    public  class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
