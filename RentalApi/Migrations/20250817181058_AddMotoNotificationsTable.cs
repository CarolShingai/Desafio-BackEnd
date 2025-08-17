using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMotoNotificationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "Motos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "NotifiedAt",
                table: "Motos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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

            migrationBuilder.UpdateData(
                table: "Motos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Message", "NotifiedAt" },
                values: new object[] { "", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.CreateIndex(
                name: "IX_MotoNotifications_MotorcycleId",
                table: "MotoNotifications",
                column: "MotorcycleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MotoNotifications");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "Motos");

            migrationBuilder.DropColumn(
                name: "NotifiedAt",
                table: "Motos");
        }
    }
}
