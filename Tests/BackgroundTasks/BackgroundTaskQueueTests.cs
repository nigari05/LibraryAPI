using Core.Utilities.BackgroundTasks;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Tests.BackgroundTasks
{
    public class BackgroundTaskQueueTests
    {
        [Fact]
        public async Task QueueBackgroundWorkItemAsync_Should_Throw_When_WorkItem_Is_Null()
        {
            var queue = new BackgroundTaskQueue();

            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await queue.QueueBackgroundWorkItemAsync(null!));
        }

        [Fact]
        public async Task DequeueAsync_Should_Return_The_Same_WorkItem_That_Was_Queued()
        {
            var queue = new BackgroundTaskQueue();
            var executed = false;

            await queue.QueueBackgroundWorkItemAsync(_ =>
            {
                executed = true;
                return ValueTask.CompletedTask;
            });

            var dequeuedWorkItem = await queue.DequeueAsync(CancellationToken.None);
            await dequeuedWorkItem(CancellationToken.None);

            Assert.True(executed);
        }

        [Fact]
        public async Task DequeueAsync_Should_Return_Items_In_FIFO_Order()
        {
            var queue = new BackgroundTaskQueue();
            var executionOrder = new List<int>();

            for (var i = 0; i < 3; i++)
            {
                var index = i;
                await queue.QueueBackgroundWorkItemAsync(_ =>
                {
                    executionOrder.Add(index);
                    return ValueTask.CompletedTask;
                });
            }

            for (var i = 0; i < 3; i++)
            {
                var workItem = await queue.DequeueAsync(CancellationToken.None);
                await workItem(CancellationToken.None);
            }

            Assert.Equal(new List<int> { 0, 1, 2 }, executionOrder);
        }

        [Fact]
        public async Task DequeueAsync_Should_Respect_Cancellation()
        {
            var queue = new BackgroundTaskQueue();
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(
                async () => await queue.DequeueAsync(cts.Token));
        }
    }
}
