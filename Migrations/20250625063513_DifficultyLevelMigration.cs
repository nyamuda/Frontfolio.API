﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Frontfolio.API.Migrations
{
    /// <inheritdoc />
    public partial class DifficultyLevelMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DifficultyLevel",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DifficultyLevel",
                table: "Projects");
        }
    }
}
