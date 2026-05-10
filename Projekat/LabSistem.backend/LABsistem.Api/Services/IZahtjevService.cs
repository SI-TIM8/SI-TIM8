using LABsistem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LABsistem.Api.Services
{
    public interface IZahtjevService
    {
        Task PosaljiZahtjev(int terminId, int studentId);
        Task OdobriZahtjev(int zahtjevId, int profesorId);
        Task OdbijZahtjev(int zahtjevId, int profesorId);
        Task<IEnumerable<ZahtjevDTO>> GetZahtjeviZaTermin(int terminId, int profesorId);
        Task<IEnumerable<ZahtjevDTO>> GetMojiZahtjevi(int studentId);
    }
}
