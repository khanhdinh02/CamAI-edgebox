using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamAI.EdgeBox.Models.Migrations
{
    /// <inheritdoc />
    public partial class AddEdgeBoxStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EdgeBoxStatus",
                table: "EdgeBoxes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EdgeBoxStatus",
                table: "EdgeBoxes");
        }
    }
}
