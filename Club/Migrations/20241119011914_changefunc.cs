using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Club.Migrations
{
    /// <inheritdoc />
    public partial class changefunc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Disponible",
                table: "Espacios");

            migrationBuilder.CreateIndex(
                name: "IX_Reservaciones_EspacioId",
                table: "Reservaciones",
                column: "EspacioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservaciones_Espacios_EspacioId",
                table: "Reservaciones",
                column: "EspacioId",
                principalTable: "Espacios",
                principalColumn: "EspacioId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservaciones_Espacios_EspacioId",
                table: "Reservaciones");

            migrationBuilder.DropIndex(
                name: "IX_Reservaciones_EspacioId",
                table: "Reservaciones");

            migrationBuilder.AddColumn<bool>(
                name: "Disponible",
                table: "Espacios",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
