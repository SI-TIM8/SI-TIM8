using System.Threading;
using System.Threading.Tasks;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LABsistem.Dal.Db
{
    public static class LabSistemDbSeeder
    {
        public static async Task SeedDefaultUsersAsync(LabSistemDbContext dbContext, CancellationToken cancellationToken = default)
        {
            var defaultUsers = new[]
            {
                new Korisnik
                {
                    ImePrezime = "Admin Korisnik",
                    Email = "admin@labsistem.local",
                    Username = "admin",
                    Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Uloga = UlogaKorisnika.Admin
                },
                new Korisnik
                {
                    ImePrezime = "Profesor Korisnik",
                    Email = "profesor@labsistem.local",
                    Username = "profesor",
                    Password = BCrypt.Net.BCrypt.HashPassword("profesor123"),
                    Uloga = UlogaKorisnika.Profesor
                },
                new Korisnik
                {
                    ImePrezime = "Student Korisnik",
                    Email = "student@labsistem.local",
                    Username = "student",
                    Password = BCrypt.Net.BCrypt.HashPassword("student123"),
                    Uloga = UlogaKorisnika.Student
                },
                new Korisnik
                {
                    ImePrezime = "Tehnicar Korisnik",
                    Email = "tehnicar@labsistem.local",
                    Username = "tehnicar",
                    Password = BCrypt.Net.BCrypt.HashPassword("tehnicar123"),
                    Uloga = UlogaKorisnika.Tehnicar
                }
            };

            foreach (var user in defaultUsers)
            {
                var conflictingUsers = await dbContext.Korisnici
                    .Where(x => x.Username == user.Username || x.Email == user.Email)
                    .OrderBy(x => x.ID)
                    .ToListAsync(cancellationToken);

                var existingUser = conflictingUsers.FirstOrDefault();

                if (conflictingUsers.Count > 1)
                {
                    var duplicateUsers = conflictingUsers.Skip(1).ToList();
                    dbContext.Korisnici.RemoveRange(duplicateUsers);
                }

                if (existingUser is null)
                {
                    dbContext.Korisnici.Add(user);
                    continue;
                }

                existingUser.ImePrezime = user.ImePrezime;
                existingUser.Email = user.Email;
                existingUser.Username = user.Username;
                existingUser.Password = user.Password;
                existingUser.Uloga = user.Uloga;
                existingUser.DeactivatedAt = null;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public static async Task SeedDefaultObjektiAsync(LabSistemDbContext dbContext, CancellationToken cancellationToken = default)
        {
            if (await dbContext.Objekti.AnyAsync(cancellationToken)) return; // Ako ima išta, preskoči

            var defaultObjekti = new[]
            {
                new Objekat { ID = 1, Lokacija = "Zgrada ET", RadnoVrijeme = "08:00 - 20:00" },
                new Objekat { ID = 2, Lokacija = "Druga zgrada", RadnoVrijeme = "07:00 - 22:00" }
            };

            dbContext.Objekti.AddRange(defaultObjekti);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public static async Task SeedDefaultKabinetiAsync(LabSistemDbContext dbContext, CancellationToken cancellationToken = default)
        {
            if (await dbContext.Kabineti.AnyAsync(cancellationToken)) return;

            // Uzimamo ID-eve direktno jer znamo da smo ih gore fixirali na 1 i 4 (tehnicar)
            var tehnicar = await dbContext.Korisnici.FirstOrDefaultAsync(u => u.Username == "tehnicar", cancellationToken);
            
            if (tehnicar == null) return;

            var defaultKabineti = new[]
            {
                new Kabinet { ID = 1, Naziv = "Lab 101", KorisnikID = tehnicar.ID, ObjekatID = 1 },
                new Kabinet { ID = 2, Naziv = "Lab 102", KorisnikID = tehnicar.ID, ObjekatID = 1 }
            };

            dbContext.Kabineti.AddRange(defaultKabineti);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

    }
}
