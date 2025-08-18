using System;
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
                name: "DeliveryPersons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Cnpj = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    BirthDate = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Cnh = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    CnhType = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    CnhImage = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryPersons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MotoNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MotorcycleId = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    LicensePlate = table.Column<string>(type: "text", nullable: false),
                    NotifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotoNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Motos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Identificador = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Ano = table.Column<int>(type: "integer", nullable: false),
                    Modelo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Placa = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsRented = table.Column<bool>(type: "boolean", nullable: false),
                    NotifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motos", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Motos",
                columns: new[] { "Id", "Ano", "Identificador", "IsRented", "Message", "Modelo", "NotifiedAt", "Placa" },
                values: new object[] { 1, 2020, "moto123", true, "", "Mottu Sport", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "CDX-0101" });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryPersons_Cnh",
                table: "DeliveryPersons",
                column: "Cnh",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryPersons_Cnpj",
                table: "DeliveryPersons",
                column: "Cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MotoNotifications_MotorcycleId",
                table: "MotoNotifications",
                column: "MotorcycleId");

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
                name: "DeliveryPersons");

            migrationBuilder.DropTable(
                name: "MotoNotifications");

            migrationBuilder.DropTable(
                name: "Motos");
        }
    }
}
