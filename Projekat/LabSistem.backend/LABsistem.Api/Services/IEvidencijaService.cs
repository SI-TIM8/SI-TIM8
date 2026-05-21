using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LABsistem.Application.DTOs;

namespace LABsistem.Api.Services
{
    public interface IEvidencijaService
    {
        Task<IEnumerable<EvidencijaDTO>> VratiSveEvidencije();
        Task KreirajEvidenciju(EvidencijaCreateDTO dto, int prijavioKorisnikId);
        Task AzurirajStatus(int id, EvidencijaUpdateDTO dto, int obradioKorisnikId);
        Task ObrisiEvidenciju(int id);
    }
}