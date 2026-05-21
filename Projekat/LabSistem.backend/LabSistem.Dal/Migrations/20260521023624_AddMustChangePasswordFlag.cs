using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabSistem.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddMustChangePasswordFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MustChangePassword",
                table: "Korisnici",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MustChangePassword",
                table: "Korisnici");
        }
    }
}
