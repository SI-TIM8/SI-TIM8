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

        Task PosaljiZahtjev(int studentId, int terminId);

        Task OdgovoriNaZahtjev(int profesorId, int zahtjevId, bool odobri);
        
        Task<IEnumerable<TerminDTO>> GetSlobodniTerminiAsync();
        
        Task<IEnumerable<TerminDTO>> GetMojeRezervacijeAsync(int korisnikId, string uloga);
        
        Task<IEnumerable<ZahtjevDTO>> GetDolazniZahtjeviAsync(int profesorId);
        
        Task<IEnumerable<TerminDTO>> GetDostupniTerminiZaStudenteAsync(int studentId);
    }
}
