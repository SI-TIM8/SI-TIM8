using LABsistem.Application.DTOs;

namespace LABsistem.Api.Services
{
    public interface IObavijestService
    {
        Task KreirajAsync(int korisnikId, string poruka, int? terminId = null);
        Task<IEnumerable<ObavijestDTO>> VratiZaKorisnikaAsync(int korisnikId);
        Task OznaciKaoProcitanuAsync(int id, int korisnikId);
        Task OznaciSveKaoProcitaneAsync(int korisnikId);
        Task<int> BrojNeprocitanihAsync(int korisnikId);
    }
}