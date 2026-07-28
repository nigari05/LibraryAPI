using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTOs.UserDTOs
{
    public record LoginDTO
   (
       string Email,
       string Password
   );
}
