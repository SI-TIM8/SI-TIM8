using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabSistem.Dal.Migrations
{
    /// <inheritdoc />
    public partial class EvidencijaFaultTrackingFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evidencije_Korisnici_KorisnikID",
                table: "Evidencije");

            migrationBuilder.AddColumn<int>(
                name: "ObradioKorisnikID",
                table: "Evidencije",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PrijavljenoU",
                table: "Evidencije",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<int>(
                name: "ProfesorID",
                table: "Evidencije",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RijesenoU",
                table: "Evidencije",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Rjesenje",
                table: "Evidencije",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TerminID",
                table: "Evidencije",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Evidencije_ObradioKorisnikID",
                table: "Evidencije",
                column: "ObradioKorisnikID");

            migrationBuilder.CreateIndex(
                name: "IX_Evidencije_ProfesorID",
                table: "Evidencije",
                column: "ProfesorID");

            migrationBuilder.CreateIndex(
                name: "IX_Evidencije_TerminID",
                table: "Evidencije",
                column: "TerminID");

            migrationBuilder.AddForeignKey(
                name: "FK_Evidencije_Korisnici_KorisnikID",
                table: "Evidencije",
                column: "KorisnikID",
                principalTable: "Korisnici",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Evidencije_Korisnici_ObradioKorisnikID",
                table: "Evidencije",
                column: "ObradioKorisnikID",
                principalTable: "Korisnici",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Evidencije_Korisnici_ProfesorID",
                table: "Evidencije",
                column: "ProfesorID",
                principalTable: "Korisnici",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Evidencije_Termini_TerminID",
                table: "Evidencije",
                column: "TerminID",
                principalTable: "Termini",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evidencije_Korisnici_KorisnikID",
                table: "Evidencije");

            migrationBuilder.DropForeignKey(
                name: "FK_Evidencije_Korisnici_ObradioKorisnikID",
                table: "Evidencije");

            migrationBuilder.DropForeignKey(
                name: "FK_Evidencije_Korisnici_ProfesorID",
                table: "Evidencije");

            migrationBuilder.DropForeignKey(
                name: "FK_Evidencije_Termini_TerminID",
                table: "Evidencije");

            migrationBuilder.DropIndex(
                name: "IX_Evidencije_ObradioKorisnikID",
                table: "Evidencije");

            migrationBuilder.DropIndex(
                name: "IX_Evidencije_ProfesorID",
                table: "Evidencije");

            migrationBuilder.DropIndex(
                name: "IX_Evidencije_TerminID",
                table: "Evidencije");

            migrationBuilder.DropColumn(
                name: "ObradioKorisnikID",
                table: "Evidencije");

            migrationBuilder.DropColumn(
                name: "PrijavljenoU",
                table: "Evidencije");

            migrationBuilder.DropColumn(
                name: "ProfesorID",
                table: "Evidencije");

            migrationBuilder.DropColumn(
                name: "RijesenoU",
                table: "Evidencije");

            migrationBuilder.DropColumn(
                name: "Rjesenje",
                table: "Evidencije");

            migrationBuilder.DropColumn(
                name: "TerminID",
                table: "Evidencije");

            migrationBuilder.AddForeignKey(
                name: "FK_Evidencije_Korisnici_KorisnikID",
                table: "Evidencije",
                column: "KorisnikID",
                principalTable: "Korisnici",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
