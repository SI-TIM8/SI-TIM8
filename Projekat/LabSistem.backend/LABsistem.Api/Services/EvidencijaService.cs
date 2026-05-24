using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LABsistem.Application.DTOs;
using LABsistem.Dal.Db;
using LABsistem.Dal.Interfaces;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using LabSistem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LABsistem.Api.Services
{
    public class EvidencijaService : IEvidencijaService
    {
        private static readonly string[] BusinessTimeZoneIds = { "Central European Standard Time", "Europe/Sarajevo" };

        private readonly IEvidencijaRepository _repo;
        private readonly LabSistemDbContext? _context;
        private readonly IEmailNotificationService? _emailNotificationService;

        public EvidencijaService(IEvidencijaRepository repo)
        {
            _repo = repo;
        }

        public EvidencijaService(
            IEvidencijaRepository repo,
            LabSistemDbContext context,
            IEmailNotificationService emailNotificationService)
        {
            _repo = repo;
            _context = context;
            _emailNotificationService = emailNotificationService;
        }

        public async Task<IEnumerable<EvidencijaDTO>> VratiSveEvidencije()
        {
            var rezultat = await _repo.GetAllWithDetailsAsync();
            return rezultat.Select(x => new EvidencijaDTO
            {
                ID = x.evidencija.ID,
                Status = x.evidencija.Status,
                Komentar = x.evidencija.Komentar,
                Rjesenje = x.evidencija.Rjesenje,
                PrijavljenoU = x.evidencija.PrijavljenoU,
                RijesenoU = x.evidencija.RijesenoU,
                OpremaID = x.evidencija.OpremaID,
                OpremaNaziv = x.opremaNaziv,
                OpremaKategorija = x.opremaKategorija,
                OpremaSerijskiBroj = x.opremaSerijskiBroj,
                OpremaStanje = x.opremaStanje,
                OpremaKabinetID = x.opremaKabinetID,
                OpremaKabinetNaziv = x.opremaKabinetNaziv,
                OpremaZgradaNaziv = x.opremaZgradaNaziv,
                KorisnikID = x.evidencija.KorisnikID,
                KorisnikImePrezime = x.korisnikImePrezime,
                TerminID = x.evidencija.TerminID,
                TerminDatum = x.terminDatum,
                TerminVrijemePocetka = x.terminVrijemePocetka,
                TerminVrijemeKraja = x.terminVrijemeKraja,
                ProfesorID = x.evidencija.ProfesorID,
                ProfesorImePrezime = x.profesorImePrezime,
                ObradioKorisnikID = x.evidencija.ObradioKorisnikID,
                ObradioImePrezime = x.obradioImePrezime
            }).ToList();
        }

        public async Task KreirajEvidenciju(EvidencijaCreateDTO dto, int prijavioKorisnikId)
        {
            if (_context is null)
            {
                throw new InvalidOperationException("Context nije konfigurisan za ovaj poziv.");
            }

            var oprema = await _context.Oprema.FirstOrDefaultAsync(o => o.ID == dto.OpremaID);
            if (oprema == null)
            {
                throw new KeyNotFoundException("Oprema nije pronadjena.");
            }

            if (oprema.stanje != StatusOpreme.Ispravno)
            {
                throw new InvalidOperationException("Kvar se moze prijaviti samo za ispravnu opremu.");
            }

            Termin? termin = null;
            if (dto.TerminID.HasValue)
            {
                termin = await _context.Termini
                    .Include(t => t.Profesor)
                    .FirstOrDefaultAsync(t => t.ID == dto.TerminID.Value);

                if (termin == null)
                {
                    throw new KeyNotFoundException("Termin nije pronadjen.");
                }

                if (termin.ProfesorID != prijavioKorisnikId)
                {
                    throw new InvalidOperationException("Kvar moze prijaviti samo profesor koji je bio na terminu.");
                }

                var businessTimeZone = GetBusinessTimeZone();
                var terminDatumLocal = ToBusinessDate(termin.Datum, businessTimeZone).Date;
                var krajTermina = terminDatumLocal.Add(termin.VrijemeKraja);
                var sada = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, businessTimeZone);
                var dozvoljenoDo = krajTermina.AddHours(24);

                if (sada < krajTermina || sada > dozvoljenoDo)
                {
                    throw new InvalidOperationException("Kvar se moze prijaviti najranije po zavrsetku termina i najkasnije 24 sata nakon toga.");
                }
            }

            var nova = new Evidencija
            {
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "Kvar" : dto.Status,
                Komentar = dto.Komentar,
                OpremaID = dto.OpremaID,
                KorisnikID = prijavioKorisnikId,
                TerminID = dto.TerminID,
                ProfesorID = termin?.ProfesorID ?? prijavioKorisnikId,
                PrijavljenoU = DateTime.UtcNow
            };

            oprema.stanje = StatusOpreme.UKvaru;

            await _context.Evidencije.AddAsync(nova);
            await OtkaziBuduceRezervacijeZaKabinetAsync(oprema);
            await _context.SaveChangesAsync();

            if (termin != null)
            {
                await PosaljiEmailTehnicarimaAsync(nova, oprema, termin);
            }
        }

        public async Task KreirajEvidenciju(EvidencijaCreateDTO dto)
        {
            var nova = new Evidencija
            {
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "Kvar" : dto.Status,
                Komentar = dto.Komentar,
                OpremaID = dto.OpremaID,
                KorisnikID = dto.KorisnikID,
                TerminID = dto.TerminID,
                ProfesorID = dto.KorisnikID,
                PrijavljenoU = DateTime.UtcNow
            };

            await _repo.AddAsync(nova);
        }

        public async Task AzurirajStatus(int id, EvidencijaUpdateDTO dto, int obradioKorisnikId)
        {
            if (_context is null)
            {
                throw new InvalidOperationException("Context nije konfigurisan za ovaj poziv.");
            }

            var evidencija = await _repo.GetByIdAsync(id);
            if (evidencija == null)
            {
                return;
            }

            var normalizedStatus = NormalizeStatusKey(dto.Status);

            evidencija.Status = dto.Status;
            evidencija.ObradioKorisnikID = obradioKorisnikId;
            evidencija.RijesenoU = normalizedStatus == "rijeseno" ? DateTime.UtcNow : null;

            if (!string.IsNullOrWhiteSpace(dto.Rjesenje))
            {
                evidencija.Rjesenje = dto.Rjesenje.Trim();
            }

            var oprema = await _context.Oprema.FirstOrDefaultAsync(o => o.ID == evidencija.OpremaID);
            if (oprema != null)
            {
                if (normalizedStatus == "rijeseno")
                {
                    oprema.stanje = StatusOpreme.Ispravno;
                }
                else if (normalizedStatus == "u obradi")
                {
                    oprema.stanje = StatusOpreme.NaServisu;
                }
            }

            await _repo.UpdateAsync(evidencija);
        }

        public async Task AzurirajStatus(int id, string status)
        {
            var evidencija = await _repo.GetByIdAsync(id);
            if (evidencija == null)
            {
                return;
            }

            evidencija.Status = status;
            await _repo.UpdateAsync(evidencija);
        }

        public async Task ObrisiEvidenciju(int id)
        {
            await _repo.DeleteAsync(id);
        }

        private async Task PosaljiEmailTehnicarimaAsync(Evidencija evidencija, Oprema oprema, Termin termin)
        {
            if (_context is null || _emailNotificationService is null)
            {
                return;
            }

            var tehnicari = await _context.Korisnici
                .Where(k =>
                    k.Uloga == UlogaKorisnika.Tehnicar &&
                    k.EmailVerified &&
                    !string.IsNullOrWhiteSpace(k.Email))
                .Select(k => new { k.Email, k.ImePrezime })
                .ToListAsync();

            if (!tehnicari.Any())
            {
                return;
            }

            var appLinkText = "Otvorite sekciju kvarova u LABsistem aplikaciji.";

            foreach (var tehnicar in tehnicari)
            {
                await _emailNotificationService.SendEquipmentFaultEmailAsync(
                    tehnicar.Email,
                    tehnicar.ImePrezime,
                    oprema.Naziv,
                    termin.Datum,
                    termin.VrijemePocetka,
                    termin.VrijemeKraja,
                    evidencija.Komentar,
                    appLinkText);
            }
        }

        private async Task<int> OtkaziBuduceRezervacijeZaKabinetAsync(Oprema oprema)
        {
            if (_context is null)
            {
                return 0;
            }

            var businessTimeZone = GetBusinessTimeZone();
            var sada = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, businessTimeZone);

            var termini = await _context.Termini
                .Include(t => t.Zahtjevi)
                    .ThenInclude(z => z.Student)
                .Where(t =>
                    t.KabinetID == oprema.KabinetID &&
                    t.StatusTermina == StatusTermina.Rezervisan)
                .ToListAsync();

            var buduciTermini = termini
                .Where(t =>
                {
                    var datumTermina = ToBusinessDate(t.Datum, businessTimeZone).Date;
                    return datumTermina > sada.Date ||
                        (datumTermina == sada.Date && t.VrijemePocetka > sada.TimeOfDay);
                })
                .ToList();

            var brojOtkazanihZahtjeva = 0;
            foreach (var termin in buduciTermini)
            {
                var aktivniZahtjevi = termin.Zahtjevi
                    .Where(z =>
                        z.StatusZahtjeva == StatusZahtjeva.NaCekanju ||
                        z.StatusZahtjeva == StatusZahtjeva.Odobren)
                    .ToList();

                if (!aktivniZahtjevi.Any())
                {
                    continue;
                }

                foreach (var zahtjev in aktivniZahtjevi)
                {
                    zahtjev.StatusZahtjeva = StatusZahtjeva.Otkazan;
                    brojOtkazanihZahtjeva++;

                    var poruka =
                        $"Vasa rezervacija/zahtjev za termin {termin.Datum:dd.MM.yyyy} u {termin.VrijemePocetka:hh\\:mm} je otkazana zbog kvara opreme {oprema.Naziv}.";

                    await _context.Obavijesti.AddAsync(new Obavijest
                    {
                        KorisnikID = zahtjev.StudentID,
                        Novosti = poruka,
                        Dostupnost = false,
                        DatumKreiranja = DateTime.UtcNow,
                        TerminID = termin.ID
                    });
                }

                termin.StatusTermina = StatusTermina.Otkazan;
                termin.VidljivoStudentima = false;
            }

            return brojOtkazanihZahtjeva;
        }

        private static TimeZoneInfo GetBusinessTimeZone()
        {
            foreach (var timeZoneId in BusinessTimeZoneIds)
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                }
                catch (TimeZoneNotFoundException)
                {
                }
                catch (InvalidTimeZoneException)
                {
                }
            }

            return TimeZoneInfo.Local;
        }

        private static DateTime ToBusinessDate(DateTime value, TimeZoneInfo businessTimeZone)
        {
            return value.Kind == DateTimeKind.Utc
                ? TimeZoneInfo.ConvertTimeFromUtc(value, businessTimeZone)
                : value;
        }

        private static string NormalizeStatusKey(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return string.Empty;
            }

            var normalized = status.Trim().Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(normalized.Length);

            foreach (var character in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(char.ToLowerInvariant(character));
                }
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
