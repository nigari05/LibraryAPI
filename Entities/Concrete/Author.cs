using Entities.Concrete.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Concrete
{
    public class Author : BaseEntity
    { 
        public string? FullName { get; set; } 
        public string? Biography { get; set; }

        public ICollection<Book>? Books { get; set; }

    }
}
