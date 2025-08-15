using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RentalApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Motos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Identificador = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Ano = table.Column<int>(type: "integer", nullable: false),
                    Modelo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Placa = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motos", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Motos",
                columns: new[] { "Id", "Ano", "Identificador", "Modelo", "Placa" },
                values: new object[] { 1, 2020, "moto123", "Mottu Sport", "CDX-0101" });

            migrationBuilder.CreateIndex(
                name: "IX_Motos_Identificador",
                table: "Motos",
                column: "Identificador",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Motos_Placa",
                table: "Motos",
                column: "Placa",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Motos");
        }
    }
}
