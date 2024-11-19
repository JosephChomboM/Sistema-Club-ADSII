using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Club.Migrations
{
    /// <inheritdoc />
    public partial class Addclubes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Lugares",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Lugares",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Lugares");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Lugares");
        }
    }
}
