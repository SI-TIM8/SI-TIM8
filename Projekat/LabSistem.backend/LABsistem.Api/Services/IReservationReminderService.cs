namespace LABsistem.Api.Services
{
    public interface IReservationReminderService
    {
        Task<int> SendDueRemindersAsync(
            DateTime? currentLocalTime = null,
            CancellationToken cancellationToken = default);
    }
}
