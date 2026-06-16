using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PracticoOrm.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ingredientes",
                columns: table => new
                {
                    ingrediente_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    costo = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ingredientes", x => x.ingrediente_id);
                });

            migrationBuilder.CreateTable(
                name: "punto_de_ventas",
                columns: table => new
                {
                    punto_de_venta_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_punto_de_ventas", x => x.punto_de_venta_id);
                });

            migrationBuilder.CreateTable(
                name: "recetas",
                columns: table => new
                {
                    receta_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recetas", x => x.receta_id);
                });

            migrationBuilder.CreateTable(
                name: "tipo_productos",
                columns: table => new
                {
                    tipo_producto_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tipo_productos", x => x.tipo_producto_id);
                });

            migrationBuilder.CreateTable(
                name: "mostradores",
                columns: table => new
                {
                    mostrador_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    punto_de_venta_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mostradores", x => x.mostrador_id);
                    table.ForeignKey(
                        name: "fk_mostradores_punto_de_ventas_punto_de_venta_id",
                        column: x => x.punto_de_venta_id,
                        principalTable: "punto_de_ventas",
                        principalColumn: "punto_de_venta_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "detalle_recetas",
                columns: table => new
                {
                    detalle_receta_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cantidad = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    receta_id = table.Column<int>(type: "integer", nullable: false),
                    ingrediente_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_detalle_recetas", x => x.detalle_receta_id);
                    table.ForeignKey(
                        name: "fk_detalle_recetas_ingredientes_ingrediente_id",
                        column: x => x.ingrediente_id,
                        principalTable: "ingredientes",
                        principalColumn: "ingrediente_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_detalle_recetas_recetas_receta_id",
                        column: x => x.receta_id,
                        principalTable: "recetas",
                        principalColumn: "receta_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "productos",
                columns: table => new
                {
                    producto_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    porcentaje_de_ganancia = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    receta_id = table.Column<int>(type: "integer", nullable: false),
                    tipo_producto_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_productos", x => x.producto_id);
                    table.ForeignKey(
                        name: "fk_productos_recetas_receta_id",
                        column: x => x.receta_id,
                        principalTable: "recetas",
                        principalColumn: "receta_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_productos_tipo_productos_tipo_producto_id",
                        column: x => x.tipo_producto_id,
                        principalTable: "tipo_productos",
                        principalColumn: "tipo_producto_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ventas",
                columns: table => new
                {
                    venta_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fecha_de_venta = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    mostrador_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ventas", x => x.venta_id);
                    table.ForeignKey(
                        name: "fk_ventas_mostradores_mostrador_id",
                        column: x => x.mostrador_id,
                        principalTable: "mostradores",
                        principalColumn: "mostrador_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "detalle_ventas",
                columns: table => new
                {
                    detalle_venta_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cantidad = table.Column<int>(type: "integer", nullable: false),
                    venta_id = table.Column<int>(type: "integer", nullable: false),
                    producto_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_detalle_ventas", x => x.detalle_venta_id);
                    table.ForeignKey(
                        name: "fk_detalle_ventas_productos_producto_id",
                        column: x => x.producto_id,
                        principalTable: "productos",
                        principalColumn: "producto_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_detalle_ventas_ventas_venta_id",
                        column: x => x.venta_id,
                        principalTable: "ventas",
                        principalColumn: "venta_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_detalle_recetas_ingrediente_id",
                table: "detalle_recetas",
                column: "ingrediente_id");

            migrationBuilder.CreateIndex(
                name: "ix_detalle_recetas_receta_id",
                table: "detalle_recetas",
                column: "receta_id");

            migrationBuilder.CreateIndex(
                name: "ix_detalle_ventas_producto_id",
                table: "detalle_ventas",
                column: "producto_id");

            migrationBuilder.CreateIndex(
                name: "ix_detalle_ventas_venta_id",
                table: "detalle_ventas",
                column: "venta_id");

            migrationBuilder.CreateIndex(
                name: "ix_mostradores_punto_de_venta_id",
                table: "mostradores",
                column: "punto_de_venta_id");

            migrationBuilder.CreateIndex(
                name: "ix_productos_receta_id",
                table: "productos",
                column: "receta_id");

            migrationBuilder.CreateIndex(
                name: "ix_productos_tipo_producto_id",
                table: "productos",
                column: "tipo_producto_id");

            migrationBuilder.CreateIndex(
                name: "ix_ventas_mostrador_id",
                table: "ventas",
                column: "mostrador_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "detalle_recetas");

            migrationBuilder.DropTable(
                name: "detalle_ventas");

            migrationBuilder.DropTable(
                name: "ingredientes");

            migrationBuilder.DropTable(
                name: "productos");

            migrationBuilder.DropTable(
                name: "ventas");

            migrationBuilder.DropTable(
                name: "recetas");

            migrationBuilder.DropTable(
                name: "tipo_productos");

            migrationBuilder.DropTable(
                name: "mostradores");

            migrationBuilder.DropTable(
                name: "punto_de_ventas");
        }
    }
}
