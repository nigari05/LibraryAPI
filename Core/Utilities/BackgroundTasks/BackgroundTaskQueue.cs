using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace Core.Utilities.BackgroundTasks
{
    /// <summary>
    /// IBackgroundTaskQueue-nun System.Threading.Channels əsaslı implementasiyası
    /// (Microsoft-un rəsmi "queued background tasks" nümunəsi ilə eyni yanaşma).
    /// Singleton kimi qeydiyyatdan keçməlidir ki, bütün scoped request-lər eyni
    /// növbəni paylaşsın.
    /// </summary>
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

        public BackgroundTaskQueue(int capacity = 100)
        {
            // BoundedChannel: növbə doluramsa, yeni yazan tərəf digər elementlər
            // götürülənə qədər gözləyir (backpressure) - yaddaş limitsiz artmır.
            var options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };

            _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
        }
        public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);

        }

        public async ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem)
        {
            if (workItem == null)
                throw new ArgumentNullException(nameof(workItem));

            await _queue.Writer.WriteAsync(workItem);
        }
    }
}
