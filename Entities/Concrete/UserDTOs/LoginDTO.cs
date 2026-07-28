using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Concrete.UserDTOs
{
    public record LoginDTO
   (
       string Email,
       string Password
   );
}
