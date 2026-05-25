using LABsistem.Dal.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LabSistem.Dal.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(LabSistemDbContext))]
    [Migration("20260524090000_AddOpremaDocumentation")]
    public partial class AddOpremaDocumentation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DokumentacijaFileName",
                table: "Oprema",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DokumentacijaFilePath",
                table: "Oprema",
                type: "character varying(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DokumentacijaUrl",
                table: "Oprema",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DokumentacijaFileName",
                table: "Oprema");

            migrationBuilder.DropColumn(
                name: "DokumentacijaFilePath",
                table: "Oprema");

            migrationBuilder.DropColumn(
                name: "DokumentacijaUrl",
                table: "Oprema");
        }

        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("LABsistem.Domain.Entities.Oprema", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ID"));

                    b.Property<DateTime?>("ArchivedAtUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DokumentacijaFileName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("DokumentacijaFilePath")
                        .HasMaxLength(260)
                        .HasColumnType("character varying(260)");

                    b.Property<string>("DokumentacijaUrl")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<bool>("IsArchived")
                        .HasColumnType("boolean");

                    b.Property<int>("KabinetID")
                        .HasColumnType("integer");

                    b.Property<string>("Kategorija")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("character varying(40)");

                    b.Property<int>("KreatorID")
                        .HasColumnType("integer");

                    b.Property<string>("Naziv")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)");

                    b.Property<int>("SerijskiBroj")
                        .HasColumnType("integer");

                    b.Property<int>("stanje")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.HasIndex("KabinetID");

                    b.ToTable("Oprema");
                });
#pragma warning restore 612, 618
        }
    }
}
