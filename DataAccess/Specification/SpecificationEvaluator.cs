using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Specification
{
    /// <summary>
    /// Verilmiş ISpecification-u bir IQueryable üzərinə tətbiq edir: Criteria (Where),
    /// IncludeStrings (Include), sıralama və (istəyə bağlı) səhifələnməni ardıcıl əlavə edir.
    /// Repository/DAL qatı bu sinif sayəsində filtr detallarından tamamilə asılı olmur.
    /// </summary>
    public class SpecificationEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification, bool applyPaging = true)
        {
            var query = inputQuery;

            if (specification.Criteria != null)
                query = query.Where(specification.Criteria);

            query = specification.IncludeStrings.Aggregate(
                query,
                (current, include) => current.Include(include));

            if (specification.OrderBy != null)
                query = query.OrderBy(specification.OrderBy);
            else if (specification.OrderByDescending != null)
                query = query.OrderByDescending(specification.OrderByDescending);

            if (applyPaging && specification.IsPagingEnabled)
                query = query.Skip(specification.Skip).Take(specification.Take);

            return query;
        }
    }
}
