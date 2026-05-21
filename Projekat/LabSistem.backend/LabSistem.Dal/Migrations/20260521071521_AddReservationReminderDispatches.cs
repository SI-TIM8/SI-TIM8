using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LabSistem.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationReminderDispatches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReservationReminderDispatches",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ZahtjevID = table.Column<int>(type: "integer", nullable: false),
                    ReminderOffsetMinutes = table.Column<int>(type: "integer", nullable: false),
                    SentAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationReminderDispatches", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ReservationReminderDispatches_Zahtjevi_ZahtjevID",
                        column: x => x.ZahtjevID,
                        principalTable: "Zahtjevi",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationReminderDispatches_ZahtjevID_ReminderOffsetMinut~",
                table: "ReservationReminderDispatches",
                columns: new[] { "ZahtjevID", "ReminderOffsetMinutes" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservationReminderDispatches");
        }
    }
}
