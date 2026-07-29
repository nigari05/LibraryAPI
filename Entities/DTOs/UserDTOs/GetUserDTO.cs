using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTOs.UserDTOs
{
    public class GetUserDTO
    {
        public Guid Id { get; set; }
        public string  FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? Role { get; set; } 

    }
}
