using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Club.Migrations
{
    /// <inheritdoc />
    public partial class testing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FechaReservacion",
                table: "Reservaciones",
                newName: "FechaInicio");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFin",
                table: "Reservaciones",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaFin",
                table: "Reservaciones");

            migrationBuilder.RenameColumn(
                name: "FechaInicio",
                table: "Reservaciones",
                newName: "FechaReservacion");
        }
    }
}
