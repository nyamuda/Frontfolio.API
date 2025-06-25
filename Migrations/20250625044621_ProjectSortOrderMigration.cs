using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Frontfolio.API.Migrations
{
    /// <inheritdoc />
    public partial class ProjectSortOrderMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Projects",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Projects");
        }
    }
}
