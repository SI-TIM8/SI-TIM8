using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LABSistem.Bll.DTOs; // Mora biti SVA VELIKA SLOVA (LABS)

namespace LABsistem.Api.Services
{
    public interface IOpremaService
    {
        // PAZI: Ako OpremaDTO nije u LABSistem.Bll.DTOs, javit će grešku
        Task<IEnumerable<OpremaDTO>> VratiSvuOpremu();
        Task<OpremaDTO> KreirajOpremu(OpremaCreateDTO dto);
        Task<bool> AzurirajOpremu(int id, OpremaCreateDTO dto);
        Task<bool> ObrisiOpremu(int id);
    }
}