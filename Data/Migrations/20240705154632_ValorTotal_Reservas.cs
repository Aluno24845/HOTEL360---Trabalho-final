using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HOTEL360___Trabalho_final.Data.Migrations
{
    /// <inheritdoc />
    public partial class ValorTotal_Reservas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ValorTotal",
                table: "Reservas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValorTotal",
                table: "Reservas");
        }
    }
}
