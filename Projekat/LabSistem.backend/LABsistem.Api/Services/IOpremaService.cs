using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LABsistem.Application.DTOs;

namespace LABsistem.Api.Services
{
    public interface IOpremaService
    {
        Task<IEnumerable<OpremaDTO>> VratiSvuOpremu(string prikaz);
        Task<OpremaDTO> KreirajOpremu(OpremaCreateDTO dto);
        Task<bool> AzurirajOpremu(int id, OpremaCreateDTO dto);
        Task<bool> ArhivirajOpremu(int id);
        Task<bool> VratiIzArhive(int id);
        Task<IEnumerable<OpremaDTO>> VratiOpremuPoKabinetu(int kabinetId);
    }
}
