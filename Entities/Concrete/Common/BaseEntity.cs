using Core.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Concrete.Common
{
    public class BaseEntity : IEntity
    {
        public Guid  Id { get; set; }   
    }
}
