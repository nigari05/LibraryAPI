using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTOs.AuthorDTOS
{
    public class GetAuthorDTO
    {
        public Guid Id { get; set; }

        public string? FullName { get; set; }

        public string? Biography { get; set; }
    }
}
