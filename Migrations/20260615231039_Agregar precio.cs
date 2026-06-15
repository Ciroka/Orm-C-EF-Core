using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PracticoOrm.Migrations
{
    /// <inheritdoc />
    public partial class Agregarprecio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "precio",
                table: "productos",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "precio",
                table: "productos");
        }
    }
}
