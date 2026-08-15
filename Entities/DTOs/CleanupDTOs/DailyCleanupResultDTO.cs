using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTOs.CleanupDTOs
{
    /// <summary>
    /// Gündəlik təmizləmə tapşırığının nəticəsini özündə saxlayır - sahibsiz
    /// (orphaned) faylların sayı və müddəti keçmiş icarələrin sayı.
    /// </summary>
    public class DailyCleanupResultDTO
    {
        public int OrphanedCoverImagesRemoved { get; set; }
        public int OverdueLoansFound { get; set; }
        public DateTime RunAtUtc { get; set; } = DateTime.UtcNow;
    }
}
