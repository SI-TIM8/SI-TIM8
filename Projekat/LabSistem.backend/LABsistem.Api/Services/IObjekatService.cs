using LABsistem.Application.DTOs;

namespace LABsistem.Api.Services
{
    public interface IObjekatService
    {
        Task<IEnumerable<ObjekatDTO>> VratiSveObjekte();
        Task KreirajObjekat(ObjekatCreateDTO dto);
        Task<bool> AzurirajObjekat(int id, ObjekatCreateDTO dto);
        Task<bool> ObrisiObjekat(int id);
    }
}