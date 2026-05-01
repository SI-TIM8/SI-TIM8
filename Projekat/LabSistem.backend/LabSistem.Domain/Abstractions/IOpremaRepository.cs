using System;
using System.Collections.Generic;
using System.Text;
using LABsistem.Domain.Entities;

namespace LABsistem.Dal.Interfaces
{
    public interface IOpremaRepository
    {
        Task<IEnumerable<Oprema>> GetAllAsync();
        Task<Oprema> GetByIdAsync(int id);
        Task AddAsync(Oprema oprema);
        Task UpdateAsync(Oprema oprema);
        Task DeleteAsync(int id);
    }
}
