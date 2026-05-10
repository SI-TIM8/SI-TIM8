using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LABsistem.Application.DTOs;

namespace LABsistem.Api.Services
{
    public interface IOpremaService
    {
        Task<IEnumerable<OpremaDTO>> VratiSvuOpremu();
        Task<OpremaDTO> KreirajOpremu(OpremaCreateDTO dto);
        Task<bool> AzurirajOpremu(int id, OpremaCreateDTO dto);
        Task<bool> ObrisiOpremu(int id);
        Task<IEnumerable<OpremaDTO>> VratiOpremuPoKabinetu(int kabinetId);
    }
}