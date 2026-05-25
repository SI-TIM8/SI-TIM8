using System.Collections.Generic;
using System.Threading.Tasks;
using LABsistem.Application.DTOs;

namespace LABsistem.Api.Services
{
    public interface IKabinetService
    {
        Task<IEnumerable<KabinetDTO>> VratiSveKabinete();
        Task KreirajKabinet(KabinetCreateDTO dto);
        Task<bool> AzurirajKabinet(int id, KabinetCreateDTO dto);
        Task<bool> ObrisiKabinet(int id);
        Task<KabinetDTO?> VratiKabinetPoId(int id);
    }
}