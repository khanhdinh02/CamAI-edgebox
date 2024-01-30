using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamAI.EdgeBox.Models.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCamera : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IpAddress",
                table: "Cameras",
                newName: "Path");

            migrationBuilder.AddColumn<string>(
                name: "Host",
                table: "Cameras",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Host",
                table: "Cameras");

            migrationBuilder.RenameColumn(
                name: "Path",
                table: "Cameras",
                newName: "IpAddress");
        }
    }
}
