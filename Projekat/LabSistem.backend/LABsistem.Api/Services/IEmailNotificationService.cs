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

        Task<bool> SendPasswordResetEmailAsync(
            string recipientEmail,
            string recipientName,
            string resetLink,
            DateTime expiresAtUtc,
            CancellationToken cancellationToken = default);
    }
}
