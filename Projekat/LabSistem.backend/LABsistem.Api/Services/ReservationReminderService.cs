using LABsistem.Api.Options;
using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using LabSistem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LABsistem.Api.Services
{
    public class ReservationReminderService : IReservationReminderService
    {
        private readonly LabSistemDbContext _context;
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly ILogger<ReservationReminderService> _logger;
        private readonly ReservationReminderOptions _options;
        private readonly TimeZoneInfo _timeZone;

        public ReservationReminderService(
            LabSistemDbContext context,
            IEmailNotificationService emailNotificationService,
            IOptions<ReservationReminderOptions> options,
            ILogger<ReservationReminderService> logger)
        {
            _context = context;
            _emailNotificationService = emailNotificationService;
            _logger = logger;
            _options = options.Value;
            _timeZone = ResolveReminderTimeZone(_options.TimeZoneId);
        }

        public async Task<int> SendDueRemindersAsync(
            DateTime? currentLocalTime = null,
            CancellationToken cancellationToken = default)
        {
            if (!_options.Enabled)
            {
                return 0;
            }

            var offsets = (_options.OffsetsMinutes ?? [])
                .Where(offset => offset > 0)
                .Distinct()
                .OrderByDescending(offset => offset)
                .ToArray();

            if (offsets.Length == 0)
            {
                return 0;
            }

            var now = currentLocalTime.HasValue
                ? DateTime.SpecifyKind(currentLocalTime.Value, DateTimeKind.Unspecified)
                : TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, _timeZone).DateTime;
            var deliveryWindow = TimeSpan.FromMinutes(Math.Max(1, _options.DeliveryWindowMinutes));
            var sentAtUtc = DateTime.UtcNow;

            var kandidati = await _context.Zahtjevi
                .Include(z => z.Student)
                .Include(z => z.Termin)
                    .ThenInclude(t => t.Kabinet)
                .Include(z => z.ReservationReminderDispatches)
                .Where(z =>
                    z.StatusZahtjeva == StatusZahtjeva.Odobren &&
                    z.Termin.StatusTermina == StatusTermina.Rezervisan)
                .ToListAsync(cancellationToken);

            var pendingEmails = new List<ReminderEmailPayload>();
            var sentCount = 0;

            foreach (var zahtjev in kandidati)
            {
                var terminStart = zahtjev.Termin.Datum.Date.Add(zahtjev.Termin.VrijemePocetka);
                if (terminStart <= now)
                {
                    continue;
                }

                foreach (var offset in offsets)
                {
                    if (zahtjev.ReservationReminderDispatches.Any(x => x.ReminderOffsetMinutes == offset))
                    {
                        continue;
                    }

                    var reminderAt = terminStart.AddMinutes(-offset);
                    if (reminderAt > now)
                    {
                        continue;
                    }

                    if (now - reminderAt > deliveryWindow)
                    {
                        continue;
                    }

                    var reminderText = BuildReminderLeadTimeText(offset);
                    var message =
                        $"Podsjetnik: imate termin {zahtjev.Termin.Datum:dd.MM.yyyy} od {zahtjev.Termin.VrijemePocetka:hh\\:mm} do {zahtjev.Termin.VrijemeKraja:hh\\:mm} u laboratoriji {zahtjev.Termin.Kabinet?.Naziv ?? "N/A"} {reminderText}.";

                    _context.Obavijesti.Add(new Obavijest
                    {
                        KorisnikID = zahtjev.StudentID,
                        Novosti = message,
                        Dostupnost = false,
                        DatumKreiranja = sentAtUtc,
                        TerminID = zahtjev.TerminID
                    });

                    zahtjev.ReservationReminderDispatches.Add(new ReservationReminderDispatch
                    {
                        ReminderOffsetMinutes = offset,
                        SentAtUtc = sentAtUtc
                    });

                    sentCount++;

                    if (zahtjev.Student.EmailVerified && !string.IsNullOrWhiteSpace(zahtjev.Student.Email))
                    {
                        pendingEmails.Add(new ReminderEmailPayload(
                            zahtjev.Student.Email,
                            zahtjev.Student.ImePrezime,
                            zahtjev.Termin.Datum,
                            zahtjev.Termin.VrijemePocetka,
                            zahtjev.Termin.VrijemeKraja,
                            zahtjev.Termin.Kabinet?.Naziv ?? "N/A",
                            reminderText));
                    }
                }
            }

            if (sentCount == 0)
            {
                return 0;
            }

            await _context.SaveChangesAsync(cancellationToken);

            foreach (var emailPayload in pendingEmails)
            {
                await _emailNotificationService.SendReservationReminderEmailAsync(
                    emailPayload.RecipientEmail,
                    emailPayload.RecipientName,
                    emailPayload.DatumTermina,
                    emailPayload.VrijemePocetka,
                    emailPayload.VrijemeKraja,
                    emailPayload.KabinetNaziv,
                    emailPayload.ReminderLeadTimeText,
                    cancellationToken);
            }

            _logger.LogInformation("Poslano je {Count} podsjetnika za termine.", sentCount);
            return sentCount;
        }

        private static string BuildReminderLeadTimeText(int offsetMinutes)
        {
            if (offsetMinutes % 1440 == 0)
            {
                var days = offsetMinutes / 1440;
                return days == 1 ? "za 1 dan" : $"za {days} dana";
            }

            if (offsetMinutes % 60 == 0)
            {
                var hours = offsetMinutes / 60;
                return hours == 1 ? "za 1 sat" : $"za {hours} sata";
            }

            return $"za {offsetMinutes} minuta";
        }

        private static TimeZoneInfo ResolveReminderTimeZone(string? configuredTimeZoneId)
        {
            var candidateIds = new[]
            {
                configuredTimeZoneId,
                "Europe/Sarajevo",
                "Central European Standard Time",
                "Central Europe Standard Time"
            };

            foreach (var candidateId in candidateIds.Where(id => !string.IsNullOrWhiteSpace(id)))
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(candidateId!);
                }
                catch (TimeZoneNotFoundException)
                {
                }
                catch (InvalidTimeZoneException)
                {
                }
            }

            return TimeZoneInfo.Local;
        }

        private sealed record ReminderEmailPayload(
            string RecipientEmail,
            string RecipientName,
            DateTime DatumTermina,
            TimeSpan VrijemePocetka,
            TimeSpan VrijemeKraja,
            string KabinetNaziv,
            string ReminderLeadTimeText);
    }
}
