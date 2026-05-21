using LABsistem.Api.Options;
using LABsistem.Api.Services;
using Microsoft.Extensions.Options;

namespace LABsistem.Presentation.BackgroundServices
{
    public class ReservationReminderBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ReservationReminderOptions _options;
        private readonly ILogger<ReservationReminderBackgroundService> _logger;

        public ReservationReminderBackgroundService(
            IServiceScopeFactory scopeFactory,
            IOptions<ReservationReminderOptions> options,
            ILogger<ReservationReminderBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_options.Enabled)
            {
                _logger.LogInformation("Reservation reminder background service je onemogucen.");
                return;
            }

            _logger.LogInformation(
                "Reservation reminder background service koristi vremensku zonu {TimeZoneId}.",
                _options.TimeZoneId);

            var intervalSeconds = Math.Max(30, _options.PollIntervalSeconds);
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(intervalSeconds));

            await RunOnceAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (!await timer.WaitForNextTickAsync(stoppingToken))
                    {
                        break;
                    }

                    await RunOnceAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private async Task RunOnceAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var reminderService = scope.ServiceProvider.GetRequiredService<IReservationReminderService>();
                await reminderService.SendDueRemindersAsync(cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greska pri obradi podsjetnika za termine.");
            }
        }
    }
}
