using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTOs.BookDTOs
{
    public class UpdateBookDTO
    {
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public Guid AuthorId { get; set; }
    }
}
