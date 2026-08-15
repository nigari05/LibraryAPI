using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities.FileStorage
{
    /// <summary>
    /// Fayl saxlama əməliyyatlarını (yüklə/oxu/sil) mücərrədləşdirir. Business qatı
    /// birbaşa fiziki disklə deyil, bu interfeys ilə işləyir - beləliklə saxlama
    /// mexanizmi (local disk, blob storage və s.) sonradan dəyişdirilə bilər.
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// Verilən stream-i "subFolder/fileName" yolu ilə saxlayır və nəticədə
        /// yaranan nisbi (relative) yolu qaytarır.
        /// </summary>
        Task<string> SaveAsync(Stream content, string fileName, string subFolder);

        /// <summary>
        /// Nisbi yol üzrə faylı oxuyur. Fayl tapılmazsa null qaytarır.
        /// </summary>
        Task<byte[]?> ReadAsync(string relativePath);

        /// <summary>
        /// Verilən alt qovluqdakı bütün faylların nisbi yollarını qaytarır (mövcud
        /// deyilsə boş siyahı). Planlaşdırılmış təmizləmə tapşırığı sahibsiz faylları
        /// tapmaq üçün istifadə edir.
        /// </summary>
        IEnumerable<string> ListFiles(string subFolder);

        /// <summary>
        /// Nisbi yol üzrə faylın mövcud olub-olmadığını yoxlayır.
        /// </summary>
        bool Exists(string relativePath);

        /// <summary>
        /// Nisbi yol üzrə faylı sistemdən silir (mövcud deyilsə heç nə etmir).
        /// </summary>
        void Delete(string relativePath);
    }
}
