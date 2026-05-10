using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabSistem.Dal.Migrations
{
    /// <inheritdoc />
    public partial class FixZahtjevShadowProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Zahtjevi_Korisnici_KorisnikID",
                table: "Zahtjevi");

            migrationBuilder.DropIndex(
                name: "IX_Zahtjevi_KorisnikID",
                table: "Zahtjevi");

            migrationBuilder.DropColumn(
                name: "KorisnikID",
                table: "Zahtjevi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "FK_Zahtjevi_Korisnici_KorisnikID",
                table: "Zahtjevi",
                column: "KorisnikID",
                principalTable: "Korisnici",
                principalColumn: "ID");
        }
    }
}
