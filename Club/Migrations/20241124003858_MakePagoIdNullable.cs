using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Club.Migrations
{
    /// <inheritdoc />
    public partial class MakePagoIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservaciones_Pagos_PagoId",
                table: "Reservaciones");

            migrationBuilder.AlterColumn<int>(
                name: "PagoId",
                table: "Reservaciones",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservaciones_Pagos_PagoId",
                table: "Reservaciones",
                column: "PagoId",
                principalTable: "Pagos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservaciones_Pagos_PagoId",
                table: "Reservaciones");

            migrationBuilder.AlterColumn<int>(
                name: "PagoId",
                table: "Reservaciones",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservaciones_Pagos_PagoId",
                table: "Reservaciones",
                column: "PagoId",
                principalTable: "Pagos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
