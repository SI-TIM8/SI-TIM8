using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LABsistem.Domain.Entities;

namespace LABsistem.Dal.Interfaces
{
    public interface IEvidencijaRepository
    {
        Task<IEnumerable<(Evidencija evidencija, string opremaNaziv, string korisnikImePrezime)>> GetAllWithDetailsAsync();
        Task AddAsync(Evidencija evidencija);
        Task<Evidencija?> GetByIdAsync(int id);
        Task UpdateAsync(Evidencija evidencija);
        Task DeleteAsync(int id);
    }
}