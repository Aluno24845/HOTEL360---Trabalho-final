using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HOTEL360___Trabalho_final.Data.Migrations
{
    /// <inheritdoc />
    public partial class HospedeFK_Reservas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Utilizadores_HospedesId",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_HospedesId",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "HospedesId",
                table: "Reservas");

            migrationBuilder.AddColumn<int>(
                name: "HospedeId",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_HospedeId",
                table: "Reservas",
                column: "HospedeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Utilizadores_HospedeId",
                table: "Reservas",
                column: "HospedeId",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Utilizadores_HospedeId",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_HospedeId",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "HospedeId",
                table: "Reservas");

            migrationBuilder.AddColumn<int>(
                name: "HospedesId",
                table: "Reservas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_HospedesId",
                table: "Reservas",
                column: "HospedesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Utilizadores_HospedesId",
                table: "Reservas",
                column: "HospedesId",
                principalTable: "Utilizadores",
                principalColumn: "Id");
        }
    }
}
