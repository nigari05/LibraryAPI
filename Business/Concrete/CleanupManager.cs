using Business.Abstract;
using Core.Utilities.FileStorage;
using DataAccess.Absract;
using Entities.DTOs.CleanupDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Concrete
{
    /// <summary>
    /// Gündəlik təmizləmə (@Scheduled) tapşırığının iş məntiqi. WebAPI qatındakı
    /// BackgroundService bu servisi hər 24 saatdan bir çağırır (bax:
    /// WebAPI/BackgroundServices/DailyCleanupService.cs).
    /// </summary>
    public class CleanupManager : ICleanupService
    {
        private const string CoverImagesSubFolder = "covers";

        private readonly IBookDAL _bookDAL;
        private readonly IBookLoanDAL _bookLoanDAL;
        private readonly IFileStorageService _fileStorageService;
        public CleanupManager(IBookDAL bookDAL, IBookLoanDAL bookLoanDAL, IFileStorageService fileStorageService)
        {
            _bookDAL = bookDAL;
            _bookLoanDAL = bookLoanDAL;
            _fileStorageService = fileStorageService;
        }

        public async Task<DailyCleanupResultDTO> RunDailyCleanupAsync()
        {
            var orphanedRemoved = await RemoveOrphanedCoverImagesAsync();
            var overdueLoans = await _bookLoanDAL.GetOverdueLoansAsync();

            return new DailyCleanupResultDTO
            {
                OrphanedCoverImagesRemoved = orphanedRemoved,
                OverdueLoansFound = overdueLoans.Count
            }; 
        }

        /// <summary>
        /// Kitab silinərkən (və ya bazaya birbaşa müdaxilə zamanı) diskdə qalmış,
        /// artıq heç bir kitaba istinad olunmayan üz qabığı fayllarını tapıb silir.
        /// </summary>
        private async Task<int> RemoveOrphanedCoverImagesAsync()
        {
            var referencedPaths = (await _bookDAL.GetAllCoverImagePathsAsync())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var filesOnDisk = _fileStorageService.ListFiles(CoverImagesSubFolder);

            var removedCount = 0;

            foreach (var relativePath in filesOnDisk)
            {
                if (referencedPaths.Contains(relativePath))
                    continue;

                _fileStorageService.Delete(relativePath);
                removedCount++;
            }

            return removedCount;
        }
    }
}
