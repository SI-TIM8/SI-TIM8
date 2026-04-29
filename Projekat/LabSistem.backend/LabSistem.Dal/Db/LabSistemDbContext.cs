
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

using LABsistem.Domain.Entities; 
using LABsistem.Domain.Enums;

namespace LABsistem.Dal.Db
{
    
    public class LabSistemDbContext : DbContext
    {
        public LabSistemDbContext(DbContextOptions<LabSistemDbContext> options)
            : base(options)
        {
        }

        
        public DbSet<Korisnik> Korisnici { get; set; }
        public DbSet<Kabinet> Kabineti { get; set; }
        public DbSet<Objekat> Objekti { get; set; }
        public DbSet<Termin> Termini { get; set; }
        public DbSet<Zahtjev> Zahtjevi { get; set; }
        public DbSet<Oprema> Oprema { get; set; }
        public DbSet<Evidencija> Evidencije { get; set; }
        public DbSet<Obavijest> Obavijesti { get; set; }
        public DbSet<Recenzija> Recenzije { get; set; }
        public DbSet<Historija> Historije { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<RevokedAccessToken> RevokedAccessTokens { get; set; }

        // Spojne tabele za Many-to-Many veze
        public DbSet<KorisnikObjekat> KorisnikObjekti { get; set; }
        public DbSet<OpremaRecenzija> OpremaRecenzije { get; set; }
        public DbSet<HistorijaTermin> HistorijaTermini { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfiguracija Enuma za bazu (opciono, ali preporučeno)
            modelBuilder.Entity<Korisnik>()
                .Property(u => u.Uloga)
                .HasConversion<int>(); // Sprema Enum kao integer u bazu za US30

            modelBuilder.Entity<Korisnik>()
                .HasIndex(x => x.Username)
                .IsUnique();

            modelBuilder.Entity<Korisnik>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(x => x.TokenHash)
                .IsUnique();

            modelBuilder.Entity<RefreshToken>()
                .HasOne(x => x.Korisnik)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.KorisnikID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RevokedAccessToken>()
                .HasIndex(x => x.Jti)
                .IsUnique();

            // Ovdje možeš dodati Fluent API konfiguracije za strane ključeve ako konvencije nisu dovoljne
        }
    }
}
