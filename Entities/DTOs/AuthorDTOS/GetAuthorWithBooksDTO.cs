using System;
using System.Collections.Generic;

namespace Entities.DTOs.AuthorDTOS
{
    public class GetAuthorWithBooksDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string? Biography { get; set; }
        public int BookCount { get; set; }
        public List<string> BookTitles { get; set; } = new();
    }
}
