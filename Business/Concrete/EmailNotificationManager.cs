using Business.Abstract;
using Core.Utilities.BackgroundTasks;
using Entities.DTOs.NotificationDTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Concrete
{
    /// <summary>
    /// Checkpoint 4 - Asinxron emal (@Async): email göndərilməsi simulyasiya edilir və
    /// bloklamadan (non-blocking) icra olunur. "Queue*" metodları işi
    /// IBackgroundTaskQueue-ya atır və dərhal qayıdır - çağıran kod (BookLoanManager,
    /// deməli HTTP request) real email göndərilməsini GÖZLƏMİR.
    /// </summary>
    public class EmailNotificationManager : IEmailNotificationService
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<EmailNotificationManager> _logger;
        private readonly TimeSpan _simulatedSendDelay;

        public EmailNotificationManager(IBackgroundTaskQueue taskQueue,ILogger<EmailNotificationManager> logger,IConfiguration configuration)
        {
            _taskQueue = taskQueue;
            _logger = logger;

            // Real bir SMTP/API çağırışının gecikməsini simulyasiya etmək üçün. Testlərdə
            // 0-a yaxın dəyərlə konfiqurasiya edilə bilər ki, unit testlər yavaşımasın.
            var delayMs = configuration.GetValue<int?>("Notifications:SimulatedSendDelayMs") ?? 2000;
            _simulatedSendDelay = TimeSpan.FromMilliseconds(Math.Max(delayMs, 0));
        }
        public async Task QueueBorrowConfirmationAsync(string toEmail, string memberFullName, string bookTitle, DateTime dueDate)
        {
            var message = new EmailMessageDTO
            {
                To = toEmail,
                Subject = "Kitab icarəyə verildi",
                Body = $"Salam {memberFullName}, \"{bookTitle}\" kitabını icarəyə götürdünüz. " +
                      $"Son qaytarma tarixi: {dueDate:dd.MM.yyyy}."
            };

            // ƏSAS MƏQAM: burada "await SendAsync(...)" ÇAĞIRMIRIQ - iş növbəyə atılır,
            // metod dərhal qayıdır. Email-in faktiki "göndərilməsi" arxa planda baş verir.
            await _taskQueue.QueueBackgroundWorkItemAsync(token => SendInBackgroundAsync(message, token));
        }

        public async Task QueueReturnConfirmationAsync(string toEmail, string memberFullName, string bookTitle)
        {
            var message = new EmailMessageDTO
            {
                To = toEmail,
                Subject = "Kitab qaytarıldı",
                Body = $"Salam {memberFullName}, \"{bookTitle}\" kitabını uğurla qaytardınız. Təşəkkür edirik!"
            };

            await _taskQueue.QueueBackgroundWorkItemAsync(token => SendInBackgroundAsync(message, token));
        }

        public async Task SendAsync(EmailMessageDTO message, CancellationToken cancellationToken = default)
        {
            // Real SMTP/API çağırışının gecikməsini (bloklayan I/O) simulyasiya edir.
            if (_simulatedSendDelay > TimeSpan.Zero)
                await Task.Delay(_simulatedSendDelay, cancellationToken);

            _logger.LogInformation(
                "[EMAIL SİMULYASİYASI] Kimə: {To} | Mövzu: {Subject} | Mətn: {Body}",
                message.To, message.Subject, message.Body);
        }

        private async ValueTask SendInBackgroundAsync(EmailMessageDTO message, CancellationToken cancellationToken)
        {
            try
            {
                await SendAsync(message, cancellationToken);
            }
            catch (Exception ex)
            {
                // Arxa plan worker-i bütün növbəni saxlaya bilər - ona görə tək bir email-in
                // uğursuzluğu udulub loglanır, exception yuxarı atılmır.
                _logger.LogError(ex, "Email növbədə göndərilə bilmədi: {To}", message.To);
            }
        }
    }
}
