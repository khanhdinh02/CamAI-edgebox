using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamAI.EdgeBox.Models.Migrations
{
    /// <inheritdoc />
    public partial class Camera : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Area",
                table: "Cameras",
                newName: "Zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Zone",
                table: "Cameras",
                newName: "Area");
        }
    }
}
