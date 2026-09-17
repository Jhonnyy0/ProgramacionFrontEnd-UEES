using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BibliotecaMVC.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablaLibros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Libros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Autor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Editorial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Isbn = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AnioPublicacion = table.Column<int>(type: "int", nullable: false),
                    Ejemplares = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    Disponible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Libros", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Libros",
                columns: new[] { "Id", "AnioPublicacion", "Autor", "Disponible", "Editorial", "Ejemplares", "FechaRegistro", "Isbn", "Titulo" },
                values: new object[,]
                {
                    { 1, 1967, "Gabriel García Márquez", true, "Sudamericana", 4, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "978-0307474728", "Cien años de soledad" },
                    { 2, 1944, "Jorge Luis Borges", true, "Emecé", 2, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "978-8420633114", "Ficciones" }
                });

            migrationBuilder.InsertData(
                table: "Libros",
                columns: new[] { "Id", "AnioPublicacion", "Autor", "Editorial", "Ejemplares", "FechaRegistro", "Isbn", "Titulo" },
                values: new object[] { 3, 1933, "Salarrué", "Dirección de Publicaciones", 1, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "978-9992300121", "Cuentos de barro" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Libros");
        }
    }
}
