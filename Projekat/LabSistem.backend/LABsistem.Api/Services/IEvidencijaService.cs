using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LABsistem.Bll.DTOs;

namespace LABsistem.Api.Services
{
    public interface IEvidencijaService
    {
        Task<IEnumerable<EvidencijaDTO>> VratiSveEvidencije();
        Task KreirajEvidenciju(EvidencijaCreateDTO dto);
    }
}