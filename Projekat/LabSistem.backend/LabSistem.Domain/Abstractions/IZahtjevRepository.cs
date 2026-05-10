using LABsistem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LABsistem.Dal.Interfaces
{
     public interface IZahtjevRepository
    {

        Task<Zahtjev?> GetByIdAsync(int id);
        Task<IEnumerable<Zahtjev>> GetZahtjeviZaTerminAsync(int terminId);
        Task<IEnumerable<Zahtjev>> GetZahtjeviStudentaAsync(int studentId);
        Task AddAsync(Zahtjev zahtjev);
        Task UpdateAsync(Zahtjev zahtjev);
    }
}
