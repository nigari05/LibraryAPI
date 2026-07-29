using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTOs.UserDTOs
{
    public record RegisterDTO(
        string FirstName,
        string LastName,
        string Email,
        string UserName,
        string Password
        );
    
    
}
