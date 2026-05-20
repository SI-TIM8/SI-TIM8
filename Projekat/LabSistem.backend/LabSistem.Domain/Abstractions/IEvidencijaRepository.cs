using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LABsistem.Domain.Entities;

namespace LABsistem.Dal.Interfaces
{
    public interface IEvidencijaRepository
    {
        Task<IEnumerable<(Evidencija evidencija, string opremaNaziv, string? opremaKategorija, int opremaSerijskiBroj, int opremaStanje, int opremaKabinetID, string? opremaKabinetNaziv, string? opremaZgradaNaziv, string korisnikImePrezime, string? profesorImePrezime, string? obradioImePrezime, DateTime? terminDatum, TimeSpan? terminVrijemePocetka, TimeSpan? terminVrijemeKraja)>> GetAllWithDetailsAsync();
        Task AddAsync(Evidencija evidencija);
        Task<Evidencija?> GetByIdAsync(int id);
        Task UpdateAsync(Evidencija evidencija);
        Task DeleteAsync(int id);
    }
}