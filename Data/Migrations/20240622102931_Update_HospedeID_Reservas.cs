using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HOTEL360___Trabalho_final.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_HospedeID_Reservas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservas_Utilizadores_HospedeId1",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_Reservas_HospedeId1",
                table: "Reservas");

            migrationBuilder.DropColumn(
                name: "HospedeId1",
                table: "Reservas");

            migrationBuilder.AlterColumn<int>(
                name: "HospedeId",
                table: "Reservas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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

            migrationBuilder.AlterColumn<string>(
                name: "HospedeId",
                table: "Reservas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "HospedeId1",
                table: "Reservas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_HospedeId1",
                table: "Reservas",
                column: "HospedeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservas_Utilizadores_HospedeId1",
                table: "Reservas",
                column: "HospedeId1",
                principalTable: "Utilizadores",
                principalColumn: "Id");
        }
    }
}
