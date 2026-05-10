namespace LABsistem.Application.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class TerminCreateDTO
    {
        public TimeSpan VrijemePocetka { get; set; }
        public TimeSpan VrijemeKraja { get; set; }
        public DateTime Datum { get; set; }
        public int KreatorID { get; set; }
        public int KabinetID { get; set; }
    }

    public class TerminDTO
    {
        public int ID { get; set; }
        public TimeSpan VrijemePocetka { get; set; }
        public TimeSpan VrijemeKraja { get; set; }
        public DateTime Datum { get; set; }
        public int KreatorID { get; set; }
        public string KreatorIme { get; set; } = default!;
        public int KabinetID { get; set; }
        public string KabinetNaziv { get; set; } = default!;


        public string StatusTermina { get; set; }
        public int? LimitOsoba { get; set; }
        public bool VidljivoStudentima { get; set; }
        public string? ProfesorIme { get; set; }
        public int BrojOdobrenih { get; set; }  // za prikaz popunjenosti
        public string? StatusPrijave { get; set; } // za studenta
    }

    // TerminDtos.cs
    public class RezervacijaCreateDTO
    {
        [Range(1, 500)]
        public int LimitOsoba { get; set; }
        public bool VidljivoStudentima { get; set; }
    }

  

  
}
