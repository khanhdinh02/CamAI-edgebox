using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamAI.EdgeBox.Models.Migrations
{
    /// <inheritdoc />
    public partial class PrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Zone",
                table: "Cameras",
                type: "INTEGER",
                maxLength: 255,
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Zone",
                table: "Cameras",
                type: "TEXT",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldMaxLength: 255);
        }
    }
}
