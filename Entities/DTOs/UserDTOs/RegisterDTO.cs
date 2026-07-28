using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTOs.UserDTOs
{
    public record RegisterDTO(
        string FirstName,
        string LastName,
        string UserName,
        string Email,
        string Password
        );
    
    
}
