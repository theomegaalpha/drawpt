using DrawPT.Data.Repositories;

namespace DrawPT.ScheduledService
{
    public class WakeUpSleepyHeadService : IHostedService, IDisposable
    {
        private readonly ILogger<WakeUpSleepyHeadService> _logger;
        private Timer? _timer;
        private readonly TimeZoneInfo _estZoneInfo;
        private CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        private readonly IServiceScopeFactory _scopeFactory;

        public WakeUpSleepyHeadService(ILogger<WakeUpSleepyHeadService> logger,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            // On Windows: "Eastern Standard Time"
            // On Linux/macOS: "America/New_York"
            _estZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("America/New_York") ?? TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("WakeUpSleepyHeadService starting.");
            var now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, _estZoneInfo);
            var minutesToAdd = 15 - (now.Minute % 15);
            var next = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0)
                .AddMinutes(minutesToAdd);
            var initialDelay = next - now;
            _timer = new Timer(async _ => await ExecuteAsync(), null, initialDelay, TimeSpan.FromMinutes(15));
            return Task.CompletedTask;
        }

        private Task ExecuteAsync()
        {
            var now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, _estZoneInfo);
            // Only execute between 1 PM and midnight
            if (now.TimeOfDay >= TimeSpan.FromHours(13) && now.TimeOfDay < TimeSpan.FromHours(24))
            {
                using var scope = _scopeFactory.CreateScope();
                // Resolve scoped repository without extension methods
                var repo = scope.ServiceProvider.GetRequiredService<DailiesRepository>();
                var themes = repo.GetDailyThemes();
                _logger.LogInformation($"Retrieved {themes.Count} daily themes at {now}");
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("WakeUpSleepyHeadService stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            _stoppingCts.Cancel();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _stoppingCts.Dispose();
        }
    }
}
