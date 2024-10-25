using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlyBalds.Api.Migrations
{
    /// <inheritdoc />
    public partial class QuestionnaireItemsUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BaldingOption_BaldingOptionQuestions_BaldingOptionQuestions~",
                table: "BaldingOption");

            migrationBuilder.DropTable(
                name: "BaldingOptionQuestions");

            migrationBuilder.DropIndex(
                name: "IX_BaldingOption_BaldingOptionQuestionsId",
                table: "BaldingOption");

            migrationBuilder.DropColumn(
                name: "BaldingOptionQuestionsId",
                table: "BaldingOption");

            migrationBuilder.AddColumn<string[]>(
                name: "Questions",
                table: "BaldingOption",
                type: "text[]",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Questions",
                table: "BaldingOption");

            migrationBuilder.AddColumn<Guid>(
                name: "BaldingOptionQuestionsId",
                table: "BaldingOption",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BaldingOptionQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Questions = table.Column<string[]>(type: "text[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaldingOptionQuestions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaldingOption_BaldingOptionQuestionsId",
                table: "BaldingOption",
                column: "BaldingOptionQuestionsId");

            migrationBuilder.AddForeignKey(
                name: "FK_BaldingOption_BaldingOptionQuestions_BaldingOptionQuestions~",
                table: "BaldingOption",
                column: "BaldingOptionQuestionsId",
                principalTable: "BaldingOptionQuestions",
                principalColumn: "Id");
        }
    }
}
