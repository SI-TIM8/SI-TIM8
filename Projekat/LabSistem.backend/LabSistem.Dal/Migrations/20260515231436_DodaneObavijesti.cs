using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabSistem.Dal.Migrations
{
    /// <inheritdoc />
    public partial class DodaneObavijesti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Obavijesti_Termini_TerminID",
                table: "Obavijesti");

            migrationBuilder.AlterColumn<int>(
                name: "TerminID",
                table: "Obavijesti",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Novosti",
                table: "Obavijesti",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<DateTime>(
                name: "DatumKreiranja",
                table: "Obavijesti",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "KorisnikID",
                table: "Obavijesti",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Obavijesti_KorisnikID",
                table: "Obavijesti",
                column: "KorisnikID");

            migrationBuilder.AddForeignKey(
                name: "FK_Obavijesti_Korisnici_KorisnikID",
                table: "Obavijesti",
                column: "KorisnikID",
                principalTable: "Korisnici",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Obavijesti_Termini_TerminID",
                table: "Obavijesti",
                column: "TerminID",
                principalTable: "Termini",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Obavijesti_Korisnici_KorisnikID",
                table: "Obavijesti");

            migrationBuilder.DropForeignKey(
                name: "FK_Obavijesti_Termini_TerminID",
                table: "Obavijesti");

            migrationBuilder.DropIndex(
                name: "IX_Obavijesti_KorisnikID",
                table: "Obavijesti");

            migrationBuilder.DropColumn(
                name: "DatumKreiranja",
                table: "Obavijesti");

            migrationBuilder.DropColumn(
                name: "KorisnikID",
                table: "Obavijesti");

            migrationBuilder.AlterColumn<int>(
                name: "TerminID",
                table: "Obavijesti",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Novosti",
                table: "Obavijesti",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Obavijesti_Termini_TerminID",
                table: "Obavijesti",
                column: "TerminID",
                principalTable: "Termini",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
