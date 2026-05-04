using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LABsistem.Application.DTOs;

namespace LABsistem.Api.Services
{
    public interface IEvidencijaService
    {
        Task<IEnumerable<EvidencijaDTO>> VratiSveEvidencije();
        Task KreirajEvidenciju(EvidencijaCreateDTO dto);
        Task AzurirajStatus(int id, string status);
        Task ObrisiEvidenciju(int id);
    }
}