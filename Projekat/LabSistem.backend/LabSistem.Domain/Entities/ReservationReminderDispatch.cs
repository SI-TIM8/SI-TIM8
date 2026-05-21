using System;
using System.ComponentModel.DataAnnotations;

namespace LABsistem.Domain.Entities
{
    public class ReservationReminderDispatch
    {
        [Key]
        public int ID { get; set; }

        public int ZahtjevID { get; set; }

        public Zahtjev Zahtjev { get; set; } = null!;

        public int ReminderOffsetMinutes { get; set; }

        public DateTime SentAtUtc { get; set; } = DateTime.UtcNow;
    }
}
