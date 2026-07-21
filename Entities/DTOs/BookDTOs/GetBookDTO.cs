using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTOs.BookDTOs
{
    public class GetBookDTO
    {
        public Guid Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public string? AuthorName { get; set; }
    }
}
