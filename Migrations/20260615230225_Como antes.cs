using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PracticoOrm.Migrations
{
    /// <inheritdoc />
    public partial class Comoantes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "precioventa",
                table: "productos");

            migrationBuilder.DropColumn(
                name: "ciudad",
                table: "mostradores");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "precioventa",
                table: "productos",
                type: "numeric(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ciudad",
                table: "mostradores",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
