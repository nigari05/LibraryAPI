using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTOs.RoleDTOs
{
    public record AssignRoleDTO(Guid UserId = default!, string RoleName = default!);
}
