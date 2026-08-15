using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities.BackgroundTasks
{
    /// <summary>
    /// Java/Spring-dəki @Async metodunun .NET ekvivalenti üçün əsas: bir iş elementini
    /// dərhal (bloklamadan) növbəyə əlavə etməyə imkan verir. Çağıran tərəf (məs. HTTP
    /// request) işin bitməsini GÖZLƏMİR - iş arxa planda ayrıca bir worker (bax:
    /// WebAPI/BackgroundServices/QueuedBackgroundEmailService.cs) tərəfindən icra olunur.
    /// </summary>
    public interface IBackgroundTaskQueue
    {
        /// <summary>
        /// Verilən iş elementini növbəyə əlavə edir. Metod dərhal qayıdır - işin özü
        /// hələ icra OLUNMAYIB, yalnız növbəyə yazılıb.
        /// </summary>
        ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem);

        /// <summary>
        /// Növbədən növbəti iş elementini götürür. Növbə boşdursa, yeni element
        /// gələnə qədər (bloklamadan, asinxron şəkildə) gözləyir.
        /// </summary>
        ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
    }
}
