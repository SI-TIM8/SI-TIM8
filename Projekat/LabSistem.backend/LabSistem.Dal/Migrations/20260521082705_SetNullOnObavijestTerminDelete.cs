using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabSistem.Dal.Migrations
{
    /// <inheritdoc />
    public partial class SetNullOnObavijestTerminDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Obavijesti_Termini_TerminID",
                table: "Obavijesti");

            migrationBuilder.AddForeignKey(
                name: "FK_Obavijesti_Termini_TerminID",
                table: "Obavijesti",
                column: "TerminID",
                principalTable: "Termini",
                principalColumn: "ID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Obavijesti_Termini_TerminID",
                table: "Obavijesti");

            migrationBuilder.AddForeignKey(
                name: "FK_Obavijesti_Termini_TerminID",
                table: "Obavijesti",
                column: "TerminID",
                principalTable: "Termini",
                principalColumn: "ID");
        }
    }
}
