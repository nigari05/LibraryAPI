using Business.Abstract;

namespace WebAPI.BackgroundServices
{
    /// <summary>
    /// Planlaşdırılmış tapşırıq (@Scheduled) - Java/Spring-dəki @Scheduled(cron = "...")
    /// annotasiyasının .NET ekvivalenti. ASP.NET Core-da bu, BackgroundService
    /// (IHostedService) vasitəsilə həyata keçirilir: tətbiq başlayanda avtomatik
    /// işə düşür və konfiqurasiya olunan interval (default: 24 saat) ilə təkrarlanır.
    ///
    /// Hər işə düşmədə ICleanupService.RunDailyCleanupAsync() çağırılır:
    ///   1) Diskdə qalmış, artıq heç bir kitaba istinad olunmayan üz qabığı şəkilləri silinir.
    ///   2) Müddəti keçmiş (DueDate keçib, hələ qaytarılmamış) icarələr aşkarlanıb loglanır.
    ///
    /// BackgroundService Singleton kimi qeydiyyatdan keçdiyi üçün, scoped servislərə
    /// (IBookDAL, IBookLoanDAL və s.) çıxış üçün hər tsikldə IServiceScopeFactory ilə
    /// ayrıca bir scope yaradılır.
    /// </summary>
    public class DailyCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DailyCleanupService> _logger;
        private readonly TimeSpan _interval;

        public DailyCleanupService(IServiceScopeFactory scopeFactory, ILogger<DailyCleanupService> logger,IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            var intervalHours = configuration.GetValue<double?>("Scheduling:DailyCleanupIntervalHours") ?? 24;
            _interval = TimeSpan.FromHours(intervalHours > 0 ? intervalHours : 24);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Gündəlik təmizləmə xidməti başladı. İşə düşmə intervalı: {Interval}.", _interval);

            while (!stoppingToken.IsCancellationRequested)
            {
                await RunCleanupSafelyAsync();

                try
                {
                    await Task.Delay(_interval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // Tətbiq dayandırılır (graceful shutdown) - normal haldır, xəta deyil.
                }
            }
        }

        private async Task RunCleanupSafelyAsync()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var cleanupService = scope.ServiceProvider.GetRequiredService<ICleanupService>();

                var result = await cleanupService.RunDailyCleanupAsync();

                _logger.LogInformation(
                    "Gündəlik təmizləmə tamamlandı: {OrphanedRemoved} sahibsiz şəkil silindi, {OverdueCount} gecikmiş icarə aşkarlandı.",
                    result.OrphanedCoverImagesRemoved,
                    result.OverdueLoansFound);
            }
            catch (Exception ex)
            {
                // Tapşırığın bir icrası uğursuz olsa belə xidmət tamamilə dayanmamalıdır -
                // xəta loglanır və bir sonrakı intervalda yenidən cəhd olunur.
                _logger.LogError(ex, "Gündəlik təmizləmə tapşırığı zamanı xəta baş verdi.");
            }
        }

    }
}
