using System.Collections.Generic;
using System.Threading.Tasks;
using LABsistem.Application.DTOs;

namespace LABsistem.Api.Services
{
    public interface ITerminService
    {
        Task<IEnumerable<TerminDTO>> VratiSveTermine();
        Task KreirajTermin(TerminCreateDTO dto);
        Task<bool> AzurirajTermin(int id, TerminCreateDTO dto);
        Task<bool> ObrisiTermin(int id);
    }
}
