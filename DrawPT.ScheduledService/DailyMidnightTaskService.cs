using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DrawPT.ScheduledService
{
    public class DailyMidnightTaskService : IHostedService, IDisposable
    {
        private readonly ILogger<DailyMidnightTaskService> _logger;
        private Timer? _timer;
        private readonly TimeZoneInfo _estZoneInfo;
        private CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        public DailyMidnightTaskService(ILogger<DailyMidnightTaskService> logger)
        {
            _logger = logger;
            // On Windows: "Eastern Standard Time"
            // On Linux/macOS: "America/New_York"
            _estZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
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

            _logger.LogInformation("Scheduled task will run for the first time at {ScheduledTimeEst} (EST), which is in {InitialDelay}.", scheduledRunTimeTodayEst, initialDelay);

            _timer = new Timer(DoWork, null, initialDelay, TimeSpan.FromHours(24));

            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            var nowEst = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, _estZoneInfo);
            _logger.LogInformation("Daily Midnight Task triggered at {TimeEst} (EST). Executing command.", nowEst);

            // Execute the command asynchronously to handle variable run times without blocking the timer.
            // The _stoppingCts.Token can be passed to your command if it supports cancellation.
            _ = ExecuteCommandAsync(_stoppingCts.Token);
        }

        private async Task ExecuteCommandAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Command execution started.");
            try
            {
                //
                // TODO: implement dailies
                //

                var random = new Random();
                int workDurationMinutes = random.Next(5, 60); // Simulate work between 5 and 60 minutes
                _logger.LogInformation("Simulating work for {WorkDurationMinutes} minutes.", workDurationMinutes);
                await Task.Delay(TimeSpan.FromMinutes(workDurationMinutes), cancellationToken);

                _logger.LogInformation("Command execution finished successfully.");
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
