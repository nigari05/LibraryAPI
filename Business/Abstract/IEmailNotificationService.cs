using Entities.DTOs.NotificationDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Abstract
{
    public interface IEmailNotificationService
    {
        Task QueueBorrowConfirmationAsync(string toEmail, string memberFullName, string bookTitle, DateTime dueDate);

        Task QueueReturnConfirmationAsync(string toEmail, string memberFullName, string bookTitle);

        /// <summary>
        /// Faktiki "göndərmə" simulyasiyası. Arxa plan worker-i (QueuedBackgroundEmailService)
        /// tərəfindən çağırılır - birbaşa controller/manager-dən çağırılmamalıdır.
        /// </summary>
        Task SendAsync(EmailMessageDTO message, CancellationToken cancellationToken = default);
    }
}
