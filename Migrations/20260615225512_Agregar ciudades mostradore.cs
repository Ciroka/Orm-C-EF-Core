using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PracticoOrm.Migrations
{
    /// <inheritdoc />
    public partial class Agregarciudadesmostradore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ciudad",
                table: "mostradores",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ciudad",
                table: "mostradores");
        }
    }
}
