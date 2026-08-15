using Business.Concrete;
using Core.Utilities.BackgroundTasks;
using Entities.DTOs.NotificationDTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Tests.Managers
{
    public class EmailNotificationManagerTests
    {
        private static IConfiguration BuildConfiguration(int delayMs = 0)
        {
            var settings = new Dictionary<string, string?>
            {
                ["Notifications:SimulatedSendDelayMs"] = delayMs.ToString()
            };

            return new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        }

        [Fact]
        public async Task QueueBorrowConfirmationAsync_Should_Enqueue_Without_Sending_Immediately()
        {
            var queueMock = new Mock<IBackgroundTaskQueue>();
            Func<CancellationToken, ValueTask>? capturedWorkItem = null;

            queueMock
                .Setup(x => x.QueueBackgroundWorkItemAsync(It.IsAny<Func<CancellationToken, ValueTask>>()))
                .Callback<Func<CancellationToken, ValueTask>>(item => capturedWorkItem = item)
                .Returns(ValueTask.CompletedTask);

            var manager = new EmailNotificationManager(queueMock.Object, NullLogger<EmailNotificationManager>.Instance, BuildConfiguration());

            await manager.QueueBorrowConfirmationAsync("member@example.com", "Nigari Zulfuqarova", "Clean Code", DateTime.UtcNow.AddDays(14));

            queueMock.Verify(x => x.QueueBackgroundWorkItemAsync(It.IsAny<Func<CancellationToken, ValueTask>>()), Times.Once);
            Assert.NotNull(capturedWorkItem);

            // Növbəyə atma anında iş hələ İCRA OLUNMAYIB - yalnız worker onu çağıranda baş verir.
            await capturedWorkItem!(CancellationToken.None);
        }

        [Fact]
        public async Task QueueReturnConfirmationAsync_Should_Enqueue_One_Work_Item()
        {
            var queueMock = new Mock<IBackgroundTaskQueue>();
            queueMock
                .Setup(x => x.QueueBackgroundWorkItemAsync(It.IsAny<Func<CancellationToken, ValueTask>>()))
                .Returns(ValueTask.CompletedTask);

            var manager = new EmailNotificationManager(queueMock.Object, NullLogger<EmailNotificationManager>.Instance, BuildConfiguration());

            await manager.QueueReturnConfirmationAsync("member@example.com", "Nigari Zulfuqarova", "Clean Code");

            queueMock.Verify(x => x.QueueBackgroundWorkItemAsync(It.IsAny<Func<CancellationToken, ValueTask>>()), Times.Once);
        }

        [Fact]
        public async Task SendAsync_Should_Complete_Without_Throwing()
        {
            var queueMock = new Mock<IBackgroundTaskQueue>();
            var manager = new EmailNotificationManager(queueMock.Object, NullLogger<EmailNotificationManager>.Instance, BuildConfiguration());

            var message = new EmailMessageDTO
            {
                To = "member@example.com",
                Subject = "Test",
                Body = "Test body"
            };

            var exception = await Record.ExceptionAsync(() => manager.SendAsync(message));

            Assert.Null(exception);
        }
    }
}
