using Entities.DTOs.CleanupDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    /// <summary>
    /// Planlaşdırılmış (@Scheduled) gündəlik təmizləmə tapşırığının həyata keçirdiyi
    /// iş məntiqini təmsil edir: sahibsiz üz qabığı fayllarının silinməsi və
    /// müddəti keçmiş icarələrin aşkarlanması.
    /// </summary>
    public interface ICleanupService
    {
        Task<DailyCleanupResultDTO> RunDailyCleanupAsync();

    }
}
