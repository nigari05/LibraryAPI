using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.DTOs.NotificationDTOs
{
    public class EmailMessageDTO
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
