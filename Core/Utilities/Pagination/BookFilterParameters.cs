using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities.Pagination
{
    /// <summary>
    /// Kitabların səhifələnməsi və mürəkkəb filtrasiyası üçün istifadə olunan parametrlər.
    /// Hər bir property boş/null olduqda sorğuya təsir etmir, dolu olduqda isə
    /// LINQ üzərində dinamik olaraq sorğuya əlavə olunur.
    /// </summary>
    public class BookFilterParameters : PaginationParameters
    {
        /// <summary>Kitabın adında axtarış (contains, case-insensitive).</summary>
        public string? Title { get; set; }

        /// <summary>Müəllifin adında axtarış (contains, case-insensitive).</summary>
        public string? AuthorName { get; set; }

        /// <summary>Yalnız müəyyən kateqoriyaya aid kitabları qaytarır.</summary>
        public Guid? CategoryId { get; set; }

        /// <summary>Minimum qiymət filtri (daxil olmaqla).</summary>
        public decimal? MinPrice { get; set; }

        /// <summary>Maksimum qiymət filtri (daxil olmaqla).</summary>
        public decimal? MaxPrice { get; set; }

        /// <summary>true olduqda yalnız stokda olan (Stock &gt; 0) kitablar qaytarılır.</summary>
        public bool? InStockOnly { get; set; }
    }
}
