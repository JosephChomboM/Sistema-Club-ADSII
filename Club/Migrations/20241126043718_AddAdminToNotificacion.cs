using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Club.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminToNotificacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "Notificaciones",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_AdminId",
                table: "Notificaciones",
                column: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notificaciones_Admins_AdminId",
                table: "Notificaciones",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "AdminId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notificaciones_Admins_AdminId",
                table: "Notificaciones");

            migrationBuilder.DropIndex(
                name: "IX_Notificaciones_AdminId",
                table: "Notificaciones");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Notificaciones");
        }
    }
}
