using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PracticoOrm.Migrations
{
    /// <inheritdoc />
    public partial class AgregarprecioVentaproducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "precioventa",
                table: "productos",
                type: "numeric(10,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "precioventa",
                table: "productos");
        }
    }
}
