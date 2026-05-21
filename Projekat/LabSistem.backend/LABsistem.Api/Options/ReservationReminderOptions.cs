namespace LABsistem.Api.Options
{
    public class ReservationReminderOptions
    {
        public const string SectionName = "ReservationReminders";

        public bool Enabled { get; set; } = true;

        public int PollIntervalSeconds { get; set; } = 60;

        public int DeliveryWindowMinutes { get; set; } = 5;

        public int[] OffsetsMinutes { get; set; } = [1440, 60];
    }
}
