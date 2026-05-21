using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LABsistem.Application.DTOs;

namespace LABsistem.Api.Services
{
    public interface IRezervacijaService
    {
        Task RezervisiTermin(int profesorId, int terminId, int limitOsoba, bool vidljivoStudentima);

        Task OtkaziTermin(int profesorId, int terminId);

        Task<StudentReservationCancellationDto> OtkaziStudentovuRezervaciju(int studentId, int terminId);

        Task OtkaziStudentovZahtjev(int studentId, int zahtjevId);

        Task PosaljiZahtjev(int studentId, int terminId);

        Task<IEnumerable<TerminDTO>> GetSlobodniTerminiAsync();
        
        Task<IEnumerable<TerminDTO>> GetMojeRezervacijeAsync(int korisnikId, string uloga);
        
        Task<IEnumerable<ZahtjevDTO>> GetDolazniZahtjeviAsync(int profesorId);

        Task<IEnumerable<StudentZahtjevDTO>> GetMojeZahtjeveAsync(int studentId);
        
        Task<IEnumerable<TerminDTO>> GetDostupniTerminiZaStudenteAsync(int studentId);
    
        Task<OdgovorNaZahtjevDTO> OdgovoriNaZahtjev(int profesorId, int zahtjevId, bool odobri);
    }
}
