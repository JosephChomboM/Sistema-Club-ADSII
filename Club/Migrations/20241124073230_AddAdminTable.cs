using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Club.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admins_Usuarios_UsuarioId",
                table: "Admins");

            migrationBuilder.DropIndex(
                name: "IX_Admins_UsuarioId",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Admins");

            migrationBuilder.RenameColumn(
                name: "Rol",
                table: "Admins",
                newName: "Nombre");

            migrationBuilder.AddColumn<string>(
                name: "Contrasena",
                table: "Admins",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Admins",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contrasena",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Admins");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Admins",
                newName: "Rol");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Admins",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Admins_UsuarioId",
                table: "Admins",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admins_Usuarios_UsuarioId",
                table: "Admins",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
