
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
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
        public DbSet<RevokedAccessToken> RevokedAccessTokens { get; set; }
        public DbSet<ReservationReminderDispatch> ReservationReminderDispatches { get; set; }

        // Spojne tabele za Many-to-Many veze
        public DbSet<KorisnikObjekat> KorisnikObjekti { get; set; }
        public DbSet<OpremaRecenzija> OpremaRecenzije { get; set; }
        public DbSet<HistorijaTermin> HistorijaTermini { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfiguracija Enuma za bazu (opciono, ali preporuceno)
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

            modelBuilder.Entity<PasswordResetToken>()
                .HasIndex(x => x.TokenHash)
                .IsUnique();

            modelBuilder.Entity<PasswordResetToken>()
                .HasOne(x => x.Korisnik)
                .WithMany(x => x.PasswordResetTokens)
                .HasForeignKey(x => x.KorisnikID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmailVerificationToken>()
                .HasIndex(x => x.TokenHash)
                .IsUnique();

            modelBuilder.Entity<EmailVerificationToken>()
                .HasOne(x => x.Korisnik)
                .WithMany(x => x.EmailVerificationTokens)
                .HasForeignKey(x => x.KorisnikID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReservationReminderDispatch>()
                .HasIndex(x => new { x.ZahtjevID, x.ReminderOffsetMinutes })
                .IsUnique();

            modelBuilder.Entity<ReservationReminderDispatch>()
                .HasOne(x => x.Zahtjev)
                .WithMany(x => x.ReservationReminderDispatches)
                .HasForeignKey(x => x.ZahtjevID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RevokedAccessToken>()
                .HasIndex(x => x.Jti)
                .IsUnique();

            // Fluent API konfiguracije


            modelBuilder.Entity<Termin>()
                .HasOne(t => t.Kreator)
                .WithMany(k => k.KreiraniTermini) 
                .HasForeignKey(t => t.KreatorID)
                .OnDelete(DeleteBehavior.Restrict); 

            
            modelBuilder.Entity<Termin>()
                .HasOne(t => t.Profesor)
                .WithMany(k => k.RezervisaniTermini) 
                .HasForeignKey(t => t.ProfesorID)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Termin>()
                .Property(t => t.StatusTermina)
                .HasConversion<int>();


            modelBuilder.Entity<Termin>()
                .Property(t => t.LimitOsoba)
                .IsRequired(false);

            modelBuilder.Entity<Evidencija>()
                .HasOne(e => e.Korisnik)
                .WithMany(k => k.Evidencije)
                .HasForeignKey(e => e.KorisnikID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Evidencija>()
                .HasOne(e => e.Profesor)
                .WithMany()
                .HasForeignKey(e => e.ProfesorID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Evidencija>()
                .HasOne(e => e.ObradioKorisnik)
                .WithMany()
                .HasForeignKey(e => e.ObradioKorisnikID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Evidencija>()
                .HasOne(e => e.Termin)
                .WithMany()
                .HasForeignKey(e => e.TerminID)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<Zahtjev>()
                .HasOne(z => z.Student)
                .WithMany(s => s.MojiZahtjevi)
                .HasForeignKey(z => z.StudentID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Zahtjev>()
                .HasOne(z => z.Termin)
                .WithMany(t => t.Zahtjevi)
                .HasForeignKey(z => z.TerminID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Zahtjev>()
                .Property(z => z.StatusZahtjeva)
                .HasConversion<int>();

            modelBuilder.Entity<Obavijest>()
                .HasOne(o => o.Korisnik)
                .WithMany(k => k.Obavijesti)
                .HasForeignKey(o => o.KorisnikID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
