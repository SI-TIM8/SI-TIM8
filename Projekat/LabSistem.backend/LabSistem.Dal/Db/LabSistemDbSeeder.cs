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
                var exists = await dbContext.Korisnici.AnyAsync(
                    x => x.Username == user.Username,
                    cancellationToken);

                if (!exists)
                {
                    dbContext.Korisnici.Add(user);
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
