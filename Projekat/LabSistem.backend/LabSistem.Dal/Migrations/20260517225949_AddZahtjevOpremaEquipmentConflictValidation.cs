using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LabSistem.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddZahtjevOpremaEquipmentConflictValidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ZahtjevOprema",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ZahtjevID = table.Column<int>(type: "integer", nullable: false),
                    OpremaID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZahtjevOprema", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ZahtjevOprema_Oprema_OpremaID",
                        column: x => x.OpremaID,
                        principalTable: "Oprema",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ZahtjevOprema_Zahtjevi_ZahtjevID",
                        column: x => x.ZahtjevID,
                        principalTable: "Zahtjevi",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ZahtjevOprema_OpremaID",
                table: "ZahtjevOprema",
                column: "OpremaID");

            migrationBuilder.CreateIndex(
                name: "IX_ZahtjevOprema_ZahtjevID_OpremaID",
                table: "ZahtjevOprema",
                columns: new[] { "ZahtjevID", "OpremaID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ZahtjevOprema");
        }
    }
}
