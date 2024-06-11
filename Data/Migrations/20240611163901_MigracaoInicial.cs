using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HOTEL360___Trabalho_final.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigracaoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Quartos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Capacidade = table.Column<int>(type: "int", nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Imagem = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quartos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Servicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Utilizadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telemovel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataNascimento = table.Column<DateOnly>(type: "date", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    Gerentes_NIF = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NIF = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumReccecionista = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizadores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ValorPago = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataReserva = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataCheckIN = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataCheckOUT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QuartoFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservas_Quartos_QuartoFK",
                        column: x => x.QuartoFK,
                        principalTable: "Quartos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HospedesReservas",
                columns: table => new
                {
                    ListaHospedesId = table.Column<int>(type: "int", nullable: false),
                    ListaReservasId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HospedesReservas", x => new { x.ListaHospedesId, x.ListaReservasId });
                    table.ForeignKey(
                        name: "FK_HospedesReservas_Reservas_ListaReservasId",
                        column: x => x.ListaReservasId,
                        principalTable: "Reservas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_HospedesReservas_Utilizadores_ListaHospedesId",
                        column: x => x.ListaHospedesId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ReccecionistasReservas",
                columns: table => new
                {
                    ListaRececcionistasId = table.Column<int>(type: "int", nullable: false),
                    ListaReservasId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReccecionistasReservas", x => new { x.ListaRececcionistasId, x.ListaReservasId });
                    table.ForeignKey(
                        name: "FK_ReccecionistasReservas_Reservas_ListaReservasId",
                        column: x => x.ListaReservasId,
                        principalTable: "Reservas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ReccecionistasReservas_Utilizadores_ListaRececcionistasId",
                        column: x => x.ListaRececcionistasId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ReservasServicos",
                columns: table => new
                {
                    ReservaFK = table.Column<int>(type: "int", nullable: false),
                    ServicoFK = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservasServicos", x => new { x.ReservaFK, x.ServicoFK });
                    table.ForeignKey(
                        name: "FK_ReservasServicos_Reservas_ReservaFK",
                        column: x => x.ReservaFK,
                        principalTable: "Reservas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ReservasServicos_Servicos_ServicoFK",
                        column: x => x.ServicoFK,
                        principalTable: "Servicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HospedesReservas_ListaReservasId",
                table: "HospedesReservas",
                column: "ListaReservasId");

            migrationBuilder.CreateIndex(
                name: "IX_ReccecionistasReservas_ListaReservasId",
                table: "ReccecionistasReservas",
                column: "ListaReservasId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_QuartoFK",
                table: "Reservas",
                column: "QuartoFK");

            migrationBuilder.CreateIndex(
                name: "IX_ReservasServicos_ServicoFK",
                table: "ReservasServicos",
                column: "ServicoFK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HospedesReservas");

            migrationBuilder.DropTable(
                name: "ReccecionistasReservas");

            migrationBuilder.DropTable(
                name: "ReservasServicos");

            migrationBuilder.DropTable(
                name: "Utilizadores");

            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropTable(
                name: "Servicos");

            migrationBuilder.DropTable(
                name: "Quartos");
        }
    }
}
