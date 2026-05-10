using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabSistem.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Sprint7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Termini_Korisnici_KreatorID",
                table: "Termini");

            migrationBuilder.DropForeignKey(
                name: "FK_Zahtjevi_Kabineti_KabinetID",
                table: "Zahtjevi");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Zahtjevi");

            migrationBuilder.RenameColumn(
                name: "KabinetID",
                table: "Zahtjevi",
                newName: "StudentID");

            migrationBuilder.RenameIndex(
                name: "IX_Zahtjevi_KabinetID",
                table: "Zahtjevi",
                newName: "IX_Zahtjevi_StudentID");

            migrationBuilder.AddColumn<int>(
                name: "StatusZahtjeva",
                table: "Zahtjevi",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "KorisnikID",
                table: "Termini",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LimitOsoba",
                table: "Termini",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProfesorID",
                table: "Termini",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusTermina",
                table: "Termini",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "VidljivoStudentima",
                table: "Termini",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Termini_KorisnikID",
                table: "Termini",
                column: "KorisnikID");

            migrationBuilder.CreateIndex(
                name: "IX_Termini_ProfesorID",
                table: "Termini",
                column: "ProfesorID");

            migrationBuilder.AddForeignKey(
                name: "FK_Termini_Korisnici_KorisnikID",
                table: "Termini",
                column: "KorisnikID",
                principalTable: "Korisnici",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Termini_Korisnici_KreatorID",
                table: "Termini",
                column: "KreatorID",
                principalTable: "Korisnici",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Termini_Korisnici_KorisnikID",
                table: "Termini");

            migrationBuilder.DropForeignKey(
                name: "FK_Termini_Korisnici_KreatorID",
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

            migrationBuilder.DropIndex(
                name: "IX_Termini_ProfesorID",
                table: "Termini");

            migrationBuilder.DropColumn(
                name: "StatusZahtjeva",
                table: "Zahtjevi");

            migrationBuilder.DropColumn(
                name: "KorisnikID",
                table: "Termini");

            migrationBuilder.DropColumn(
                name: "LimitOsoba",
                table: "Termini");

            migrationBuilder.DropColumn(
                name: "ProfesorID",
                table: "Termini");

            migrationBuilder.DropColumn(
                name: "StatusTermina",
                table: "Termini");

            migrationBuilder.DropColumn(
                name: "VidljivoStudentima",
                table: "Termini");

            migrationBuilder.RenameColumn(
                name: "StudentID",
                table: "Zahtjevi",
                newName: "KabinetID");

            migrationBuilder.RenameIndex(
                name: "IX_Zahtjevi_StudentID",
                table: "Zahtjevi",
                newName: "IX_Zahtjevi_KabinetID");

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Zahtjevi",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Termini_Korisnici_KreatorID",
                table: "Termini",
                column: "KreatorID",
                principalTable: "Korisnici",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Zahtjevi_Kabineti_KabinetID",
                table: "Zahtjevi",
                column: "KabinetID",
                principalTable: "Kabineti",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
