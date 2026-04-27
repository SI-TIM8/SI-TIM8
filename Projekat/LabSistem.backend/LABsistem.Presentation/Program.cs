using System.Text;
using LABsistem.Bll.Models;
using LABsistem.Bll.Services;
using Microsoft.EntityFrameworkCore;
using LABsistem.Dal.Db;
using LABsistem.Domain.Entities;
using LABsistem.Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default");
Console.WriteLine($"TRENUTNI CONNECTION STRING JE: {connectionString}");

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT konfiguracija nije pronađena.");

builder.Services.AddDbContext<LabSistemDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<LabSistemDbContext>();
        context.Database.Migrate();
        Console.WriteLine("Migracije su uspješno provjerene/primijenjene.");

        // Seed admin korisnik ako ne postoji
        if (!context.Korisnici.Any(k => k.Uloga == UlogaKorisnika.Admin))
        {
            context.Korisnici.Add(new Korisnik
            {
                ImePrezime = "Administrator",
                Email = "admin@labsistem.ba",
                Username = "admin",
                Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Uloga = UlogaKorisnika.Admin
            });
            context.SaveChanges();
            Console.WriteLine("Admin korisnik je kreiran. Username: admin");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Greška pri migraciji: {ex.Message}");
    }
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();