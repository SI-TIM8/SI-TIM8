using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabSistem.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentArchiving : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedAtUtc",
                table: "Oprema",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Oprema",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArchivedAtUtc",
                table: "Oprema");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Oprema");
        }
    }
}
