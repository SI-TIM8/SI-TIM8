using Microsoft.EntityFrameworkCore;
using LABsistem.Dal.Db; 


var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("Default");
Console.WriteLine($"TRENUTNI CONNECTION STRING JE: {connectionString}");


builder.Services.AddDbContext<LabSistemDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<LabSistemDbContext>();
        // Ova linija provjerava migracije i izvršava ih ako već nisu izvršene
        context.Database.Migrate();
        Console.WriteLine("Migracije su uspješno provjerene/primijenjene.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Greška pri migraciji: {ex.Message}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();