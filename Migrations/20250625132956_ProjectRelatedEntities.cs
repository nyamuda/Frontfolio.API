using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Frontfolio.API.Migrations
{
    /// <inheritdoc />
    public partial class ProjectRelatedEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Achievement_Projects_ProjectId",
                table: "Achievement");

            migrationBuilder.DropForeignKey(
                name: "FK_Challenge_Projects_ProjectId",
                table: "Challenge");

            migrationBuilder.DropForeignKey(
                name: "FK_Paragraph_Projects_ProjectId",
                table: "Paragraph");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Paragraph",
                table: "Paragraph");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Challenge",
                table: "Challenge");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Achievement",
                table: "Achievement");

            migrationBuilder.RenameTable(
                name: "Paragraph",
                newName: "Paragraphs");

            migrationBuilder.RenameTable(
                name: "Challenge",
                newName: "Challenges");

            migrationBuilder.RenameTable(
                name: "Achievement",
                newName: "Achievements");

            migrationBuilder.RenameIndex(
                name: "IX_Paragraph_ProjectId",
                table: "Paragraphs",
                newName: "IX_Paragraphs_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Challenge_ProjectId",
                table: "Challenges",
                newName: "IX_Challenges_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Achievement_ProjectId",
                table: "Achievements",
                newName: "IX_Achievements_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Paragraphs",
                table: "Paragraphs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Challenges",
                table: "Challenges",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Achievements",
                table: "Achievements",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Achievements_Projects_ProjectId",
                table: "Achievements",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Challenges_Projects_ProjectId",
                table: "Challenges",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Paragraphs_Projects_ProjectId",
                table: "Paragraphs",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Achievements_Projects_ProjectId",
                table: "Achievements");

            migrationBuilder.DropForeignKey(
                name: "FK_Challenges_Projects_ProjectId",
                table: "Challenges");

            migrationBuilder.DropForeignKey(
                name: "FK_Paragraphs_Projects_ProjectId",
                table: "Paragraphs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Paragraphs",
                table: "Paragraphs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Challenges",
                table: "Challenges");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Achievements",
                table: "Achievements");

            migrationBuilder.RenameTable(
                name: "Paragraphs",
                newName: "Paragraph");

            migrationBuilder.RenameTable(
                name: "Challenges",
                newName: "Challenge");

            migrationBuilder.RenameTable(
                name: "Achievements",
                newName: "Achievement");

            migrationBuilder.RenameIndex(
                name: "IX_Paragraphs_ProjectId",
                table: "Paragraph",
                newName: "IX_Paragraph_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Challenges_ProjectId",
                table: "Challenge",
                newName: "IX_Challenge_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Achievements_ProjectId",
                table: "Achievement",
                newName: "IX_Achievement_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Paragraph",
                table: "Paragraph",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Challenge",
                table: "Challenge",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Achievement",
                table: "Achievement",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Achievement_Projects_ProjectId",
                table: "Achievement",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Challenge_Projects_ProjectId",
                table: "Challenge",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Paragraph_Projects_ProjectId",
                table: "Paragraph",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
