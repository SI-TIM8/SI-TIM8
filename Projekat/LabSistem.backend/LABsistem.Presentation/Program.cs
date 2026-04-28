using System.IdentityModel.Tokens.Jwt;
using System.Text;
using LABsistem.Bll.Models;
using LABsistem.Bll.Services;
using LABsistem.Dal.Db;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default");
Console.WriteLine($"TRENUTNI CONNECTION STRING JE: {connectionString}");

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT konfiguracija nije pronadjena.");

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<LabSistemDbContext>(options =>
        options.UseNpgsql(connectionString));
}

builder.Services.AddMemoryCache();
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddSingleton<IRevokedTokenStore, InMemoryRevokedTokenStore>();
builder.Services.AddScoped<IAuthService, AuthService>();

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

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var revokedTokenStore = context.HttpContext.RequestServices.GetRequiredService<IRevokedTokenStore>();
                var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                if (!string.IsNullOrWhiteSpace(jti) && revokedTokenStore.IsRevoked(jti))
                {
                    context.Fail("Token has been revoked.");
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3001")
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
        if (!app.Environment.IsEnvironment("Testing")) { 
        context.Database.Migrate();
        if (app.Environment.IsDevelopment())
        {
            await LabSistemDbSeeder.SeedDefaultUsersAsync(context);
        }
        Console.WriteLine("Migracije su uspjesno provjerene/primijenjene.");
    }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Greska pri migraciji: {ex.Message}");
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
public partial class Program { }
