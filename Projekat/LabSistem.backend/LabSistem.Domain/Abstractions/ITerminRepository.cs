using System.Collections.Generic;
using System.Threading.Tasks;
using LABsistem.Domain.Entities;

namespace LABsistem.Dal.Interfaces
{
    public interface ITerminRepository
    {
        Task<IEnumerable<(Termin termin, string kreatorIme, string kabinetNaziv)>> GetAllWithDetailsAsync();
        Task<Termin?> GetByIdAsync(int id);
        Task AddAsync(Termin termin);
        Task UpdateAsync(Termin termin);
        Task DeleteAsync(int id);
    }
}
