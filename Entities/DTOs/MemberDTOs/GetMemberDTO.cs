using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTOs.MemberDTOs
{
    public class GetMemberDTO
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
    }
}
