using Core.Specification;
using Core.Utilities.Pagination;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Specification
{
    public class BookFilterSpecification : BaseSpecification<Book>
    {
        public BookFilterSpecification(BookFilterParameters filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Title))
                AddCriteria(b => b.Title.Contains(filter.Title));

            if (!string.IsNullOrWhiteSpace(filter.AuthorName))
                AddCriteria(b => b.Author != null && b.Author.FullName.Contains(filter.AuthorName));

            if (filter.CategoryId.HasValue)
                AddCriteria(b => b.Categories.Any(c => c.Id == filter.CategoryId.Value));

            if (filter.MinPrice.HasValue)
                AddCriteria(b => b.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                AddCriteria(b => b.Price <= filter.MaxPrice.Value);

            if (filter.InStockOnly == true)
                AddCriteria(b => b.Stock > 0);

            AddInclude(nameof(Book.Author));
            AddInclude(nameof(Book.Categories));

            ApplySorting(filter);

            if (filter.PageNumber > 0 && filter.PageSize > 0)
                ApplyPaging((filter.PageNumber - 1) * filter.PageSize, filter.PageSize);
        }

        private void ApplySorting(BookFilterParameters filter)
        {
            if (string.IsNullOrWhiteSpace(filter.SortBy))
            {
                ApplyOrderBy(b => b.Title);
                return;
            }

            switch (filter.SortBy.ToLower())
            {
                case "price":
                    if (filter.IsDescending) ApplyOrderByDescending(b => b.Price);
                    else ApplyOrderBy(b => b.Price);
                    break;
                case "stock":
                    if (filter.IsDescending) ApplyOrderByDescending(b => b.Stock);
                    else ApplyOrderBy(b => b.Stock);
                    break;
                default:
                    if (filter.IsDescending) ApplyOrderByDescending(b => b.Title);
                    else ApplyOrderBy(b => b.Title);
                    break;
            }
        }
    }
}
