using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HOTEL360___Trabalho_final.Data.Migrations
{
    /// <inheritdoc />
    public partial class NomeUpdate_Quartos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Quartos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Quartos");
        }
    }
}
