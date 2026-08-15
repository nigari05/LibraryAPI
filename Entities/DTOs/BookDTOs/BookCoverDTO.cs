using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTOs.BookDTOs
{
    /// <summary>
    /// Kitabın üz qabığı şəklini endirmək üçün istifadə olunan DTO. Fayl məzmununu,
    /// MIME tipini və faylın adını controller-ə ötürür ki, orada File() nəticəsi
    /// kimi qaytarıla bilsin.
    /// </summary>
    public class BookCoverDTO
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = "application/octet-stream";
        public string FileName { get; set; } = string.Empty;
    }
}
