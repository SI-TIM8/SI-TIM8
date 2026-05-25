using System.Collections.Generic;
using System.Threading.Tasks;
using LABsistem.Domain.Entities;

namespace LABsistem.Dal.Interfaces
{
    public interface IKabinetRepository
    {
        Task<IEnumerable<(Kabinet kabinet, string odgovorniKorisnik, string objekatLokacija)>> GetAllWithDetailsAsync();
        Task<Kabinet?> GetByIdAsync(int id);
        Task AddAsync(Kabinet kabinet);
        Task UpdateAsync(Kabinet kabinet);
        Task DeleteAsync(int id);
        Task<(Kabinet kabinet, string odgovorniKorisnik, string objekatLokacija)?> GetByIdWithDetailsAsync(int id);
    }
}