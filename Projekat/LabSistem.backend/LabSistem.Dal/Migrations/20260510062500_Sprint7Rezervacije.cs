using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabSistem.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Sprint7Rezervacije : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Termini_Korisnici_KorisnikID",
                table: "Termini");

            migrationBuilder.DropForeignKey(
                name: "FK_Termini_Korisnici_ProfesorID",
                table: "Termini");

            migrationBuilder.DropForeignKey(
                name: "FK_Zahtjevi_Korisnici_StudentID",
                table: "Zahtjevi");

            migrationBuilder.DropIndex(
                name: "IX_Termini_KorisnikID",
                table: "Termini");

            migrationBuilder.DropColumn(
                name: "KorisnikID",
                table: "Termini");

            migrationBuilder.AddColumn<int>(
                name: "KorisnikID",
                table: "Zahtjevi",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Zahtjevi_KorisnikID",
                table: "Zahtjevi",
                column: "KorisnikID");

            migrationBuilder.AddForeignKey(
                name: "FK_Termini_Korisnici_ProfesorID",
                table: "Termini",
                column: "ProfesorID",
                principalTable: "Korisnici",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Zahtjevi_Korisnici_KorisnikID",
                table: "Zahtjevi",
                column: "KorisnikID",
                principalTable: "Korisnici",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Zahtjevi_Korisnici_StudentID",
                table: "Zahtjevi",
                column: "StudentID",
                principalTable: "Korisnici",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Termini_Korisnici_ProfesorID",
                table: "Termini");

            migrationBuilder.DropForeignKey(
                name: "FK_Zahtjevi_Korisnici_KorisnikID",
                table: "Zahtjevi");

            migrationBuilder.DropForeignKey(
                name: "FK_Zahtjevi_Korisnici_StudentID",
                table: "Zahtjevi");

            migrationBuilder.DropIndex(
                name: "IX_Zahtjevi_KorisnikID",
                table: "Zahtjevi");

            migrationBuilder.DropColumn(
                name: "KorisnikID",
                table: "Zahtjevi");

            migrationBuilder.AddColumn<int>(
                name: "KorisnikID",
                table: "Termini",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Termini_KorisnikID",
                table: "Termini",
                column: "KorisnikID");

            migrationBuilder.AddForeignKey(
                name: "FK_Termini_Korisnici_KorisnikID",
                table: "Termini",
                column: "KorisnikID",
                principalTable: "Korisnici",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Termini_Korisnici_ProfesorID",
                table: "Termini",
                column: "ProfesorID",
                principalTable: "Korisnici",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Zahtjevi_Korisnici_StudentID",
                table: "Zahtjevi",
                column: "StudentID",
                principalTable: "Korisnici",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
