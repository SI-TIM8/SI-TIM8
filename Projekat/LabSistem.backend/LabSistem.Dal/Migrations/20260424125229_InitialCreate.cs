using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LabSistem.Dal.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistorijaTermini",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HistorijaID = table.Column<int>(type: "integer", nullable: false),
                    TerminID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorijaTermini", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Historije",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Datum = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TerminID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Historije", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Korisnici",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ImePrezime = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Email = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Username = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Password = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Uloga = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnici", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Objekti",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Lokacija = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RadnoVrijeme = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objekti", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Recenzije",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Komentar = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Ocjena = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recenzije", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Kabineti",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Naziv = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    KorisnikID = table.Column<int>(type: "integer", nullable: false),
                    ObjekatID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kabineti", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Kabineti_Korisnici_KorisnikID",
                        column: x => x.KorisnikID,
                        principalTable: "Korisnici",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Kabineti_Objekti_ObjekatID",
                        column: x => x.ObjekatID,
                        principalTable: "Objekti",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KorisnikObjekti",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KorisnikID = table.Column<int>(type: "integer", nullable: false),
                    ObjekatID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KorisnikObjekti", x => x.ID);
                    table.ForeignKey(
                        name: "FK_KorisnikObjekti_Korisnici_KorisnikID",
                        column: x => x.KorisnikID,
                        principalTable: "Korisnici",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KorisnikObjekti_Objekti_ObjekatID",
                        column: x => x.ObjekatID,
                        principalTable: "Objekti",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Oprema",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Naziv = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    SerijskiBroj = table.Column<int>(type: "integer", nullable: false),
                    stanje = table.Column<int>(type: "integer", nullable: false),
                    KreatorID = table.Column<int>(type: "integer", nullable: false),
                    KabinetID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oprema", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Oprema_Kabineti_KabinetID",
                        column: x => x.KabinetID,
                        principalTable: "Kabineti",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Termini",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VrijemePocetka = table.Column<TimeSpan>(type: "interval", nullable: false),
                    VrijemeKraja = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Datum = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    KreatorID = table.Column<int>(type: "integer", nullable: false),
                    KabinetID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Termini", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Termini_Kabineti_KabinetID",
                        column: x => x.KabinetID,
                        principalTable: "Kabineti",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Termini_Korisnici_KreatorID",
                        column: x => x.KreatorID,
                        principalTable: "Korisnici",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Evidencije",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Komentar = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    OpremaID = table.Column<int>(type: "integer", nullable: false),
                    KorisnikID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evidencije", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Evidencije_Korisnici_KorisnikID",
                        column: x => x.KorisnikID,
                        principalTable: "Korisnici",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Evidencije_Oprema_OpremaID",
                        column: x => x.OpremaID,
                        principalTable: "Oprema",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpremaRecenzije",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecenzijaID = table.Column<int>(type: "integer", nullable: false),
                    OpremaID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpremaRecenzije", x => x.ID);
                    table.ForeignKey(
                        name: "FK_OpremaRecenzije_Oprema_OpremaID",
                        column: x => x.OpremaID,
                        principalTable: "Oprema",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpremaRecenzije_Recenzije_RecenzijaID",
                        column: x => x.RecenzijaID,
                        principalTable: "Recenzije",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Obavijesti",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Novosti = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Dostupnost = table.Column<bool>(type: "boolean", nullable: false),
                    TerminID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Obavijesti", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Obavijesti_Termini_TerminID",
                        column: x => x.TerminID,
                        principalTable: "Termini",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Zahtjevi",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    Komentar = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TerminID = table.Column<int>(type: "integer", nullable: false),
                    KabinetID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zahtjevi", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Zahtjevi_Kabineti_KabinetID",
                        column: x => x.KabinetID,
                        principalTable: "Kabineti",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Zahtjevi_Termini_TerminID",
                        column: x => x.TerminID,
                        principalTable: "Termini",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Evidencije_KorisnikID",
                table: "Evidencije",
                column: "KorisnikID");

            migrationBuilder.CreateIndex(
                name: "IX_Evidencije_OpremaID",
                table: "Evidencije",
                column: "OpremaID");

            migrationBuilder.CreateIndex(
                name: "IX_Kabineti_KorisnikID",
                table: "Kabineti",
                column: "KorisnikID");

            migrationBuilder.CreateIndex(
                name: "IX_Kabineti_ObjekatID",
                table: "Kabineti",
                column: "ObjekatID");

            migrationBuilder.CreateIndex(
                name: "IX_KorisnikObjekti_KorisnikID",
                table: "KorisnikObjekti",
                column: "KorisnikID");

            migrationBuilder.CreateIndex(
                name: "IX_KorisnikObjekti_ObjekatID",
                table: "KorisnikObjekti",
                column: "ObjekatID");

            migrationBuilder.CreateIndex(
                name: "IX_Obavijesti_TerminID",
                table: "Obavijesti",
                column: "TerminID");

            migrationBuilder.CreateIndex(
                name: "IX_Oprema_KabinetID",
                table: "Oprema",
                column: "KabinetID");

            migrationBuilder.CreateIndex(
                name: "IX_OpremaRecenzije_OpremaID",
                table: "OpremaRecenzije",
                column: "OpremaID");

            migrationBuilder.CreateIndex(
                name: "IX_OpremaRecenzije_RecenzijaID",
                table: "OpremaRecenzije",
                column: "RecenzijaID");

            migrationBuilder.CreateIndex(
                name: "IX_Termini_KabinetID",
                table: "Termini",
                column: "KabinetID");

            migrationBuilder.CreateIndex(
                name: "IX_Termini_KreatorID",
                table: "Termini",
                column: "KreatorID");

            migrationBuilder.CreateIndex(
                name: "IX_Zahtjevi_KabinetID",
                table: "Zahtjevi",
                column: "KabinetID");

            migrationBuilder.CreateIndex(
                name: "IX_Zahtjevi_TerminID",
                table: "Zahtjevi",
                column: "TerminID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Evidencije");

            migrationBuilder.DropTable(
                name: "HistorijaTermini");

            migrationBuilder.DropTable(
                name: "Historije");

            migrationBuilder.DropTable(
                name: "KorisnikObjekti");

            migrationBuilder.DropTable(
                name: "Obavijesti");

            migrationBuilder.DropTable(
                name: "OpremaRecenzije");

            migrationBuilder.DropTable(
                name: "Zahtjevi");

            migrationBuilder.DropTable(
                name: "Oprema");

            migrationBuilder.DropTable(
                name: "Recenzije");

            migrationBuilder.DropTable(
                name: "Termini");

            migrationBuilder.DropTable(
                name: "Kabineti");

            migrationBuilder.DropTable(
                name: "Korisnici");

            migrationBuilder.DropTable(
                name: "Objekti");
        }
    }
}
