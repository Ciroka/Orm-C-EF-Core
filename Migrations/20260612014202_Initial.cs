using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PracticoOrm.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ingredientes",
                columns: table => new
                {
                    IngredienteId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Costo = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredientes", x => x.IngredienteId);
                });

            migrationBuilder.CreateTable(
                name: "PuntoDeVentas",
                columns: table => new
                {
                    PuntoDeVentaId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuntoDeVentas", x => x.PuntoDeVentaId);
                });

            migrationBuilder.CreateTable(
                name: "Recetas",
                columns: table => new
                {
                    RecetaId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recetas", x => x.RecetaId);
                });

            migrationBuilder.CreateTable(
                name: "TipoProductos",
                columns: table => new
                {
                    TipoProductoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoProductos", x => x.TipoProductoId);
                });

            migrationBuilder.CreateTable(
                name: "Mostradores",
                columns: table => new
                {
                    MostradorId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PuntoDeVentaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mostradores", x => x.MostradorId);
                    table.ForeignKey(
                        name: "FK_Mostradores_PuntoDeVentas_PuntoDeVentaId",
                        column: x => x.PuntoDeVentaId,
                        principalTable: "PuntoDeVentas",
                        principalColumn: "PuntoDeVentaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetalleRecetas",
                columns: table => new
                {
                    DetalleRecetaId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cantidad = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    RecetaId = table.Column<int>(type: "integer", nullable: false),
                    IngredienteId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleRecetas", x => x.DetalleRecetaId);
                    table.ForeignKey(
                        name: "FK_DetalleRecetas_Ingredientes_IngredienteId",
                        column: x => x.IngredienteId,
                        principalTable: "Ingredientes",
                        principalColumn: "IngredienteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetalleRecetas_Recetas_RecetaId",
                        column: x => x.RecetaId,
                        principalTable: "Recetas",
                        principalColumn: "RecetaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    ProductoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PorcentajeDeGanancia = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    RecetaId = table.Column<int>(type: "integer", nullable: false),
                    TipoProductoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.ProductoId);
                    table.ForeignKey(
                        name: "FK_Productos_Recetas_RecetaId",
                        column: x => x.RecetaId,
                        principalTable: "Recetas",
                        principalColumn: "RecetaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Productos_TipoProductos_TipoProductoId",
                        column: x => x.TipoProductoId,
                        principalTable: "TipoProductos",
                        principalColumn: "TipoProductoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    VentaId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FechaDeVenta = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MostradorId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.VentaId);
                    table.ForeignKey(
                        name: "FK_Ventas_Mostradores_MostradorId",
                        column: x => x.MostradorId,
                        principalTable: "Mostradores",
                        principalColumn: "MostradorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetalleVentas",
                columns: table => new
                {
                    DetalleVentaId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    VentaId = table.Column<int>(type: "integer", nullable: false),
                    ProductoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleVentas", x => x.DetalleVentaId);
                    table.ForeignKey(
                        name: "FK_DetalleVentas_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "ProductoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetalleVentas_Ventas_VentaId",
                        column: x => x.VentaId,
                        principalTable: "Ventas",
                        principalColumn: "VentaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetalleRecetas_IngredienteId",
                table: "DetalleRecetas",
                column: "IngredienteId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleRecetas_RecetaId",
                table: "DetalleRecetas",
                column: "RecetaId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleVentas_ProductoId",
                table: "DetalleVentas",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleVentas_VentaId",
                table: "DetalleVentas",
                column: "VentaId");

            migrationBuilder.CreateIndex(
                name: "IX_Mostradores_PuntoDeVentaId",
                table: "Mostradores",
                column: "PuntoDeVentaId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_RecetaId",
                table: "Productos",
                column: "RecetaId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_TipoProductoId",
                table: "Productos",
                column: "TipoProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_MostradorId",
                table: "Ventas",
                column: "MostradorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetalleRecetas");

            migrationBuilder.DropTable(
                name: "DetalleVentas");

            migrationBuilder.DropTable(
                name: "Ingredientes");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Ventas");

            migrationBuilder.DropTable(
                name: "Recetas");

            migrationBuilder.DropTable(
                name: "TipoProductos");

            migrationBuilder.DropTable(
                name: "Mostradores");

            migrationBuilder.DropTable(
                name: "PuntoDeVentas");
        }
    }
}
