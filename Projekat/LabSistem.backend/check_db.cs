using LABsistem.Dal.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading.Tasks;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<LabSistemDbContext>(options =>
            options.UseNpgsql("Host=localhost;Port=5432;Database=labsistem;Username=labsistem;Password=labsistem"));
    })
    .Build();

using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<LabSistemDbContext>();

var termini = await db.Termini.ToListAsync();
Console.WriteLine($"Ukupno termina: {termini.Count}");

foreach (var t in termini)
{
    Console.WriteLine($"ID: {t.ID}, Status: {t.StatusTermina}, Vidljivo: {t.VidljivoStudentima}, Profesor: {t.ProfesorID}");
}

var zahtjevi = await db.Zahtjevi.ToListAsync();
Console.WriteLine($"Ukupno zahtjeva: {zahtjevi.Count}");
