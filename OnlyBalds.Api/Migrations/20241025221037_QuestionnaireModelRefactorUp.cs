using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlyBalds.Api.Migrations
{
    /// <inheritdoc />
    public partial class QuestionnaireModelRefactorUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Questions",
                table: "QuestionnaireData");

            migrationBuilder.DropColumn(
                name: "Questions",
                table: "BaldingOption");

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Answer = table.Column<string>(type: "text", nullable: false),
                    BaldingOptionId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuestionnaireDataId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Question", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Question_BaldingOption_BaldingOptionId",
                        column: x => x.BaldingOptionId,
                        principalTable: "BaldingOption",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Question_QuestionnaireData_QuestionnaireDataId",
                        column: x => x.QuestionnaireDataId,
                        principalTable: "QuestionnaireData",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Question_BaldingOptionId",
                table: "Question",
                column: "BaldingOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_QuestionnaireDataId",
                table: "Question",
                column: "QuestionnaireDataId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.AddColumn<string[]>(
                name: "Questions",
                table: "QuestionnaireData",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "Questions",
                table: "BaldingOption",
                type: "text[]",
                nullable: true);
        }
    }
}
