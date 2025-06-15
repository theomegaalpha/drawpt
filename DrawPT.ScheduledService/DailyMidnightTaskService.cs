using DrawPT.Common.Services.AI;
using DrawPT.Data.Repositories;
using DrawPT.Data.Repositories.Game;

namespace DrawPT.ScheduledService
{
    public class DailyMidnightTaskService : IHostedService, IDisposable
    {
        private readonly ILogger<DailyMidnightTaskService> _logger;
        private Timer? _timer;
        private readonly TimeZoneInfo _estZoneInfo;
        private CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        private readonly DailiesRepository _dailiesRepository;
        private readonly DailyAIService _dailyAIService;

        public DailyMidnightTaskService(ILogger<DailyMidnightTaskService> logger, DailyAIService dailyAIService,
            DailiesRepository dailiesRepository)
        {
            _logger = logger;
            // On Windows: "Eastern Standard Time"
            // On Linux/macOS: "America/New_York"
            _estZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("America/New_York") ?? TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            _dailyAIService = dailyAIService ?? throw new ArgumentNullException(nameof(dailyAIService), "IAIService cannot be null.");
            _dailiesRepository = dailiesRepository ?? throw new ArgumentNullException(nameof(dailiesRepository), "DailiesRepository cannot be null.");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Daily Midnight Task Service is starting.");

            DateTimeOffset now = DateTimeOffset.UtcNow;
            DateTimeOffset estNow = TimeZoneInfo.ConvertTime(now, _estZoneInfo);

            // Calculate the next midnight EST
            DateTimeOffset scheduledRunTimeTodayEst = new DateTimeOffset(estNow.Year, estNow.Month, estNow.Day, 0, 0, 0, estNow.Offset);

            if (estNow >= scheduledRunTimeTodayEst) // If current EST time is at or past midnight today EST
            {
                // Schedule for midnight tomorrow EST
                scheduledRunTimeTodayEst = scheduledRunTimeTodayEst.AddDays(1);
            }

            TimeSpan initialDelay = scheduledRunTimeTodayEst - estNow;
            if (initialDelay < TimeSpan.Zero) // Should ideally not happen with the logic above
            {
                initialDelay = TimeSpan.Zero; // Or handle as an error/log
            }

            _logger.LogInformation($"Scheduled task will run for the first time at {scheduledRunTimeTodayEst} (EST), which is in {initialDelay}.");

            _timer = new Timer(DoWork, null, initialDelay, TimeSpan.FromHours(24));

            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            var nowEst = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, _estZoneInfo);
            _logger.LogInformation($"Daily Midnight Task triggered at {nowEst} (EST). Executing command.");

            // Execute the command asynchronously to handle variable run times without blocking the timer.
            // The _stoppingCts.Token can be passed to your command if it supports cancellation.
            _ = ExecuteCommandAsync(_stoppingCts.Token);
        }

        private async Task ExecuteCommandAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Command execution started.");
            try
            {
                var question = await _dailyAIService.GenerateGameQuestionAsync("");
                var daily = new DailyQuestionEntity
                {
                    Date = DateTime.UtcNow.AddDays(1).Date,
                    Style = "anime",
                    ImageUrl = question.ImageUrl,
                    OriginalPrompt = question.OriginalPrompt
                };
                await _dailiesRepository.AddDailyQuestion(daily);

                _logger.LogInformation($"Successfully saved daily {daily}");
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Command execution was canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during command execution.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Daily Midnight Task Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            _stoppingCts.Cancel();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _stoppingCts.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
