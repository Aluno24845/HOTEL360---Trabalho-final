using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HOTEL360___Trabalho_final.Data.Migrations
{
    /// <inheritdoc />
    public partial class Model_Correcao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservasServicos_Reservas_ReservaFK",
                table: "ReservasServicos");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservasServicos_Servicos_ServicoFK",
                table: "ReservasServicos");

            migrationBuilder.DropTable(
                name: "HospedesReservas");

            migrationBuilder.DropTable(
                name: "ReccecionistasReservas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReservasServicos",
                table: "ReservasServicos");

            migrationBuilder.DropIndex(
                name: "IX_ReservasServicos_ServicoFK",
                table: "ReservasServicos");

            migrationBuilder.DropColumn(
                name: "ReservaFK",
                table: "ReservasServicos");

            migrationBuilder.RenameColumn(
                name: "Quantidade",
                table: "ReservasServicos",
                newName: "ListaServicosId");

            migrationBuilder.RenameColumn(
                name: "ServicoFK",
                table: "ReservasServicos",
                newName: "ListaReservasId");

            migrationBuilder.AddColumn<int>(
                name: "HospedesId",
                table: "Reservas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReccecionistasId",
                table: "Reservas",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReservasServicos",
                table: "ReservasServicos",
                columns: new[] { "ListaReservasId", "ListaServicosId" });

            migrationBuilder.CreateIndex(
                name: "IX_ReservasServicos_ListaServicosId",
                table: "ReservasServicos",
                column: "ListaServicosId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_HospedesId",
                table: "Reservas",
                column: "HospedesId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_ReccecionistasId",
                table: "Reservas",
                column: "ReccecionistasId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Utilizadores_HospedesId",
                table: "Reservas",
                column: "HospedesId",
                principalTable: "Utilizadores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Utilizadores_ReccecionistasId",
                table: "Reservas",
                column: "ReccecionistasId",
                principalTable: "Utilizadores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservasServicos_Reservas_ListaReservasId",
                table: "ReservasServicos",
                column: "ListaReservasId",
                principalTable: "Reservas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservasServicos_Servicos_ListaServicosId",
                table: "ReservasServicos",
                column: "ListaServicosId",
                principalTable: "Servicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Utilizadores_HospedesId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Utilizadores_ReccecionistasId",
                table: "Reservas");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservasServicos_Reservas_ListaReservasId",
                table: "ReservasServicos");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservasServicos_Servicos_ListaServicosId",
                table: "ReservasServicos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReservasServicos",
                table: "ReservasServicos");

            migrationBuilder.DropIndex(
                name: "IX_ReservasServicos_ListaServicosId",
                table: "ReservasServicos");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_HospedesId",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_ReccecionistasId",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "HospedesId",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "ReccecionistasId",
                table: "Reservas");

            migrationBuilder.RenameColumn(
                name: "ListaServicosId",
                table: "ReservasServicos",
                newName: "Quantidade");

            migrationBuilder.RenameColumn(
                name: "ListaReservasId",
                table: "ReservasServicos",
                newName: "ServicoFK");

            migrationBuilder.AddColumn<int>(
                name: "ReservaFK",
                table: "ReservasServicos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReservasServicos",
                table: "ReservasServicos",
                columns: new[] { "ReservaFK", "ServicoFK" });

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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HospedesReservas_Utilizadores_ListaHospedesId",
                        column: x => x.ListaHospedesId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReccecionistasReservas_Utilizadores_ListaRececcionistasId",
                        column: x => x.ListaRececcionistasId,
                        principalTable: "Utilizadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservasServicos_ServicoFK",
                table: "ReservasServicos",
                column: "ServicoFK");

            migrationBuilder.CreateIndex(
                name: "IX_HospedesReservas_ListaReservasId",
                table: "HospedesReservas",
                column: "ListaReservasId");

            migrationBuilder.CreateIndex(
                name: "IX_ReccecionistasReservas_ListaReservasId",
                table: "ReccecionistasReservas",
                column: "ListaReservasId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservasServicos_Reservas_ReservaFK",
                table: "ReservasServicos",
                column: "ReservaFK",
                principalTable: "Reservas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservasServicos_Servicos_ServicoFK",
                table: "ReservasServicos",
                column: "ServicoFK",
                principalTable: "Servicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
