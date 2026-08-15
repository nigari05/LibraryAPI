using Core.Utilities.BackgroundTasks;

namespace WebAPI.BackgroundServices
{
   
    public class QueuedBackgroundEmailService  : BackgroundService
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<QueuedBackgroundEmailService> _logger;

        public QueuedBackgroundEmailService(IBackgroundTaskQueue taskQueue, ILogger<QueuedBackgroundEmailService> logger)
        {
            _taskQueue = taskQueue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Asinxron email növbəsi xidməti başladı.");

            while (!stoppingToken.IsCancellationRequested)
            {
                Func<CancellationToken, ValueTask> workItem;

                try
                {
                    workItem = await _taskQueue.DequeueAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Tətbiq dayandırılır (graceful shutdown) - normal haldır.
                    break;
                }

                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    // Tək bir işin xətası bütün worker-i dayandırmamalıdır.
                    _logger.LogError(ex, "Növbədəki tapşırığın icrası zamanı xəta baş verdi.");
                }
            }
        }
    }
}
