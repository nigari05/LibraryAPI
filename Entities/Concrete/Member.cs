using Entities.Concrete.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Concrete
{
    public class Member : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public string Email { get; set; }
    }
}
