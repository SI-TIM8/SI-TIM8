using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabSistem.Dal.Migrations
{
    /// <inheritdoc />
    public partial class SetDefaultKabinetCapacity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"Kabineti\" SET \"Kapacitet\" = 20 WHERE \"Kapacitet\" = 0;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"Kabineti\" SET \"Kapacitet\" = 0 WHERE \"Kapacitet\" = 20;");
        }
    }
}
