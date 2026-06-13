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
                name: "ingredientes",
                columns: table => new
                {
                    ingredienteid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    costo = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingredientes", x => x.ingredienteid);
                });

            migrationBuilder.CreateTable(
                name: "puntodeventas",
                columns: table => new
                {
                    puntodeventaid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_puntodeventas", x => x.puntodeventaid);
                });

            migrationBuilder.CreateTable(
                name: "recetas",
                columns: table => new
                {
                    recetaid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recetas", x => x.recetaid);
                });

            migrationBuilder.CreateTable(
                name: "tipoproductos",
                columns: table => new
                {
                    tipoproductoid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipoproductos", x => x.tipoproductoid);
                });

            migrationBuilder.CreateTable(
                name: "mostradores",
                columns: table => new
                {
                    mostradorid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    puntodeventaid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mostradores", x => x.mostradorid);
                    table.ForeignKey(
                        name: "FK_mostradores_puntodeventas_puntodeventaid",
                        column: x => x.puntodeventaid,
                        principalTable: "puntodeventas",
                        principalColumn: "puntodeventaid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "detallerecetas",
                columns: table => new
                {
                    detallerecetaid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cantidad = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    recetaid = table.Column<int>(type: "integer", nullable: false),
                    ingredienteid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detallerecetas", x => x.detallerecetaid);
                    table.ForeignKey(
                        name: "FK_detallerecetas_ingredientes_ingredienteid",
                        column: x => x.ingredienteid,
                        principalTable: "ingredientes",
                        principalColumn: "ingredienteid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_detallerecetas_recetas_recetaid",
                        column: x => x.recetaid,
                        principalTable: "recetas",
                        principalColumn: "recetaid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "productos",
                columns: table => new
                {
                    productoid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    porcentajedeganancia = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    recetaid = table.Column<int>(type: "integer", nullable: false),
                    tipoproductoid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_productos", x => x.productoid);
                    table.ForeignKey(
                        name: "FK_productos_recetas_recetaid",
                        column: x => x.recetaid,
                        principalTable: "recetas",
                        principalColumn: "recetaid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_productos_tipoproductos_tipoproductoid",
                        column: x => x.tipoproductoid,
                        principalTable: "tipoproductos",
                        principalColumn: "tipoproductoid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ventas",
                columns: table => new
                {
                    ventaid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fechadeventa = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    mostradorid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ventas", x => x.ventaid);
                    table.ForeignKey(
                        name: "FK_ventas_mostradores_mostradorid",
                        column: x => x.mostradorid,
                        principalTable: "mostradores",
                        principalColumn: "mostradorid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "detalleventas",
                columns: table => new
                {
                    detalleventaid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cantidad = table.Column<int>(type: "integer", nullable: false),
                    ventaid = table.Column<int>(type: "integer", nullable: false),
                    productoid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_detalleventas", x => x.detalleventaid);
                    table.ForeignKey(
                        name: "FK_detalleventas_productos_productoid",
                        column: x => x.productoid,
                        principalTable: "productos",
                        principalColumn: "productoid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_detalleventas_ventas_ventaid",
                        column: x => x.ventaid,
                        principalTable: "ventas",
                        principalColumn: "ventaid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_detallerecetas_ingredienteid",
                table: "detallerecetas",
                column: "ingredienteid");

            migrationBuilder.CreateIndex(
                name: "IX_detallerecetas_recetaid",
                table: "detallerecetas",
                column: "recetaid");

            migrationBuilder.CreateIndex(
                name: "IX_detalleventas_productoid",
                table: "detalleventas",
                column: "productoid");

            migrationBuilder.CreateIndex(
                name: "IX_detalleventas_ventaid",
                table: "detalleventas",
                column: "ventaid");

            migrationBuilder.CreateIndex(
                name: "IX_mostradores_puntodeventaid",
                table: "mostradores",
                column: "puntodeventaid");

            migrationBuilder.CreateIndex(
                name: "IX_productos_recetaid",
                table: "productos",
                column: "recetaid");

            migrationBuilder.CreateIndex(
                name: "IX_productos_tipoproductoid",
                table: "productos",
                column: "tipoproductoid");

            migrationBuilder.CreateIndex(
                name: "IX_ventas_mostradorid",
                table: "ventas",
                column: "mostradorid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "detallerecetas");

            migrationBuilder.DropTable(
                name: "detalleventas");

            migrationBuilder.DropTable(
                name: "ingredientes");

            migrationBuilder.DropTable(
                name: "productos");

            migrationBuilder.DropTable(
                name: "ventas");

            migrationBuilder.DropTable(
                name: "recetas");

            migrationBuilder.DropTable(
                name: "tipoproductos");

            migrationBuilder.DropTable(
                name: "mostradores");

            migrationBuilder.DropTable(
                name: "puntodeventas");
        }
    }
}
