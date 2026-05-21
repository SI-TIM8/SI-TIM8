namespace LABsistem.Api.Services
{
    public interface IEmailNotificationService
    {
        Task<bool> SendReservationDecisionEmailAsync(
            string recipientEmail,
            string recipientName,
            DateTime datumTermina,
            TimeSpan vrijemePocetka,
            bool odobri,
            string? komentar = null,
            CancellationToken cancellationToken = default);

        Task<bool> SendEquipmentFaultEmailAsync(
            string recipientEmail,
            string recipientName,
            string opremaNaziv,
            DateTime datumTermina,
            TimeSpan vrijemePocetka,
            TimeSpan vrijemeKraja,
            string komentar,
            string? appLinkText = null,
            CancellationToken cancellationToken = default);

        Task<bool> SendPasswordResetEmailAsync(
            string recipientEmail,
            string recipientName,
            string resetLink,
            DateTime expiresAtUtc,
            CancellationToken cancellationToken = default);

        Task<bool> SendEmailVerificationEmailAsync(
            string recipientEmail,
            string recipientName,
            string verificationLink,
            DateTime expiresAtUtc,
            CancellationToken cancellationToken = default);

        Task<bool> SendReservationReminderEmailAsync(
            string recipientEmail,
            string recipientName,
            DateTime datumTermina,
            TimeSpan vrijemePocetka,
            TimeSpan vrijemeKraja,
            string kabinetNaziv,
            string reminderLeadTimeText,
            CancellationToken cancellationToken = default);
    }
}
