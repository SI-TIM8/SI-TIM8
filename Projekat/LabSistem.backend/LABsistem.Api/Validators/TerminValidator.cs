using System;
using System.Threading.Tasks;
using LABsistem.Dal.Db;
using Microsoft.EntityFrameworkCore;

namespace LABsistem.Api.Validators
{
    public interface ITerminValidator
    {
        Task ValidateCreateAsync(DateTime datum, TimeSpan pocetak, TimeSpan kraj, int kabinetId);
        Task ValidateUpdateAsync(int terminId, DateTime datum, TimeSpan pocetak, TimeSpan kraj, int kabinetId);
    }

    public class TerminValidator : ITerminValidator
    {
        private readonly LabSistemDbContext _context;

        public TerminValidator(LabSistemDbContext context)
        {
            _context = context;
        }

        public async Task ValidateCreateAsync(DateTime datum, TimeSpan pocetak, TimeSpan kraj, int kabinetId)
        {
            ValidateBasicRules(datum, pocetak, kraj);
            await ValidateNoOverlapAsync(null, datum, pocetak, kraj, kabinetId);
        }

        public async Task ValidateUpdateAsync(int terminId, DateTime datum, TimeSpan pocetak, TimeSpan kraj, int kabinetId)
        {
            ValidateBasicRules(datum, pocetak, kraj);
            await ValidateNoOverlapAsync(terminId, datum, pocetak, kraj, kabinetId);
        }

        private static void ValidateBasicRules(DateTime datum, TimeSpan pocetak, TimeSpan kraj)
        {
            if (datum.Date < DateTime.Now.Date)
                throw new Exception("Datum ne moze biti u proslosti.");

            if (pocetak >= kraj)
                throw new Exception("Vrijeme pocetka mora biti prije vremena kraja.");
        }

        private async Task ValidateNoOverlapAsync(int? terminId, DateTime datum, TimeSpan pocetak, TimeSpan kraj, int kabinetId)
        {
            var postojiPreklapanje = await _context.Termini.AnyAsync(t =>
                t.KabinetID == kabinetId &&
                t.Datum.Date == datum.Date &&
                (!terminId.HasValue || t.ID != terminId.Value) &&
                pocetak < t.VrijemeKraja &&
                t.VrijemePocetka < kraj);

            if (postojiPreklapanje)
                throw new Exception("Vec postoji termin koji se vremenski preklapa u odabranom kabinetu.");
        }
    }
}
