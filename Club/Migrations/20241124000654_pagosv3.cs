using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Club.Migrations
{
    /// <inheritdoc />
    public partial class pagosv3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservaciones_Pagos_PagoId",
                table: "Reservaciones");

            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.DropIndex(
                name: "IX_Reservaciones_PagoId",
                table: "Reservaciones");

            migrationBuilder.DropColumn(
                name: "PagoId",
                table: "Reservaciones");
        }
    }
}
