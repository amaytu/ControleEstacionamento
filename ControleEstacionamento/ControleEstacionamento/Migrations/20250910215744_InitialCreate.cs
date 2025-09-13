using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleEstacionamento.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegistrosEstacionamento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Placa = table.Column<string>(type: "TEXT", nullable: false),
                    DataEntrada = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataSaida = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ValorTotal = table.Column<decimal>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosEstacionamento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TabelasDePrecos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InicioVigencia = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FimVigencia = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ValorHoraInicial = table.Column<decimal>(type: "TEXT", nullable: false),
                    ValorHoraAdicional = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TabelasDePrecos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrosEstacionamento");

            migrationBuilder.DropTable(
                name: "TabelasDePrecos");
        }
    }
}
