using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HOTEL360___Trabalho_final.Data.Migrations
{
    /// <inheritdoc />
    public partial class Localizacao_Quartos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Localizacao",
                table: "Quartos",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Localizacao",
                table: "Quartos");
        }
    }
}
