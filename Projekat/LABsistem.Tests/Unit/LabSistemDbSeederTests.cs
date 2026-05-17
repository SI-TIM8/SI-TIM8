using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

public class LabSistemDbSeederTests
{
    private static LabSistemDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<LabSistemDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new LabSistemDbContext(options);
    }

    [Fact]
    public async Task SeedDefaultUsersAsync_WithExistingSeedUsersAndEmailConflicts_OverwritesExpectedValues()
    {
        using var context = GetInMemoryDbContext();

        context.Korisnici.AddRange(
            new Korisnik
            {
                ImePrezime = "Stari Admin",
                Email = "stari.admin@test.com",
                Username = "admin",
                Password = BCrypt.Net.BCrypt.HashPassword("staraadminlozinka"),
                Uloga = UlogaKorisnika.Student,
                DeactivatedAt = DateTime.UtcNow
            },
            new Korisnik
            {
                ImePrezime = "Stari Profesor",
                Email = "stari.profesor@test.com",
                Username = "profesor",
                Password = BCrypt.Net.BCrypt.HashPassword("staraprofesorlozinka"),
                Uloga = UlogaKorisnika.Student,
                DeactivatedAt = DateTime.UtcNow
            },
            new Korisnik
            {
                ImePrezime = "Konflikt Admin Email",
                Email = "admin@labsistem.local",
                Username = "adminkonflikt",
                Password = BCrypt.Net.BCrypt.HashPassword("konfliktadmin"),
                Uloga = UlogaKorisnika.Tehnicar,
                DeactivatedAt = DateTime.UtcNow
            },
            new Korisnik
            {
                ImePrezime = "Konflikt Profesor Email",
                Email = "profesor@labsistem.local",
                Username = "profesorkonflikt",
                Password = BCrypt.Net.BCrypt.HashPassword("konfliktprofesor"),
                Uloga = UlogaKorisnika.Admin,
                DeactivatedAt = DateTime.UtcNow
            });

        await context.SaveChangesAsync();

        await LabSistemDbSeeder.SeedDefaultUsersAsync(context);

        var users = await context.Korisnici
            .OrderBy(x => x.Username)
            .ToListAsync();

        Assert.Equal(4, users.Count);

        var admin = users.Single(x => x.Username == "admin");
        Assert.Equal("Admin Korisnik", admin.ImePrezime);
        Assert.Equal("admin@labsistem.local", admin.Email);
        Assert.Equal(UlogaKorisnika.Admin, admin.Uloga);
        Assert.Null(admin.DeactivatedAt);
        Assert.True(BCrypt.Net.BCrypt.Verify("admin123", admin.Password));

        var profesor = users.Single(x => x.Username == "profesor");
        Assert.Equal("Profesor Korisnik", profesor.ImePrezime);
        Assert.Equal("profesor@labsistem.local", profesor.Email);
        Assert.Equal(UlogaKorisnika.Profesor, profesor.Uloga);
        Assert.Null(profesor.DeactivatedAt);
        Assert.True(BCrypt.Net.BCrypt.Verify("profesor123", profesor.Password));

        var student = users.Single(x => x.Username == "student");
        Assert.Equal("Student Korisnik", student.ImePrezime);
        Assert.Equal("runtyfly34@gmail.com", student.Email);
        Assert.Equal(UlogaKorisnika.Student, student.Uloga);
        Assert.True(BCrypt.Net.BCrypt.Verify("student123", student.Password));

        var tehnicar = users.Single(x => x.Username == "tehnicar");
        Assert.Equal("Tehnicar Korisnik", tehnicar.ImePrezime);
        Assert.Equal("tehnicar@labsistem.local", tehnicar.Email);
        Assert.Equal(UlogaKorisnika.Tehnicar, tehnicar.Uloga);
        Assert.True(BCrypt.Net.BCrypt.Verify("tehnicar123", tehnicar.Password));
    }

    [Fact]
    public async Task SeedDefaultObjektiAsync_WithEmptyDatabase_AddsDefaultsWithoutExplicitIds()
    {
        using var context = GetInMemoryDbContext();

        await LabSistemDbSeeder.SeedDefaultObjektiAsync(context);

        var objekti = await context.Objekti
            .OrderBy(x => x.Lokacija)
            .ToListAsync();

        Assert.Equal(2, objekti.Count);
        Assert.All(objekti, objekat => Assert.True(objekat.ID > 0));
        Assert.Contains(objekti, x => x.Lokacija == "Zgrada ET");
        Assert.Contains(objekti, x => x.Lokacija == "Druga zgrada");
    }

    [Fact]
    public async Task SeedDefaultKabinetiAsync_WithSeededObjektiAndTehnicar_AddsDefaults()
    {
        using var context = GetInMemoryDbContext();

        await LabSistemDbSeeder.SeedDefaultUsersAsync(context);
        await LabSistemDbSeeder.SeedDefaultObjektiAsync(context);
        await LabSistemDbSeeder.SeedDefaultKabinetiAsync(context);

        var kabineti = await context.Kabineti
            .OrderBy(x => x.Naziv)
            .ToListAsync();
        var glavniObjekat = await context.Objekti.SingleAsync(x => x.Lokacija == "Zgrada ET");
        var tehnicar = await context.Korisnici.SingleAsync(x => x.Username == "tehnicar");

        Assert.Equal(2, kabineti.Count);
        Assert.All(kabineti, kabinet => Assert.True(kabinet.ID > 0));
        Assert.All(kabineti, kabinet => Assert.Equal(glavniObjekat.ID, kabinet.ObjekatID));
        Assert.All(kabineti, kabinet => Assert.Equal(tehnicar.ID, kabinet.KorisnikID));
    }
}
