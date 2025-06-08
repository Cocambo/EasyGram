using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyGram.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCorrectToUserProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCorrect",
                table: "UserProgresses",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCorrect",
                table: "UserProgresses");
        }
    }
}
