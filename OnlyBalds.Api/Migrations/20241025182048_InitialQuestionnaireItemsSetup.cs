using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlyBalds.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialQuestionnaireItemsSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "QuestionnaireData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Questions = table.Column<string[]>(type: "text[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BaldingOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionnaireDataId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaldingOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaldingOptions_QuestionnaireData_QuestionnaireDataId",
                        column: x => x.QuestionnaireDataId,
                        principalTable: "QuestionnaireData",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionnaireItems_QuestionnaireData_DataId",
                        column: x => x.DataId,
                        principalTable: "QuestionnaireData",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BaldingOption",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BaldingOptionTitle = table.Column<string>(type: "text", nullable: false),
                    BaldingOptionQuestionsId = table.Column<Guid>(type: "uuid", nullable: true),
                    BaldingOptionsId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaldingOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaldingOption_BaldingOptionQuestions_BaldingOptionQuestions~",
                        column: x => x.BaldingOptionQuestionsId,
                        principalTable: "BaldingOptionQuestions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BaldingOption_BaldingOptions_BaldingOptionsId",
                        column: x => x.BaldingOptionsId,
                        principalTable: "BaldingOptions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaldingOption_BaldingOptionQuestionsId",
                table: "BaldingOption",
                column: "BaldingOptionQuestionsId");

            migrationBuilder.CreateIndex(
                name: "IX_BaldingOption_BaldingOptionsId",
                table: "BaldingOption",
                column: "BaldingOptionsId");

            migrationBuilder.CreateIndex(
                name: "IX_BaldingOptions_QuestionnaireDataId",
                table: "BaldingOptions",
                column: "QuestionnaireDataId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireItems_DataId",
                table: "QuestionnaireItems",
                column: "DataId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaldingOption");

            migrationBuilder.DropTable(
                name: "QuestionnaireItems");

            migrationBuilder.DropTable(
                name: "BaldingOptionQuestions");

            migrationBuilder.DropTable(
                name: "BaldingOptions");

            migrationBuilder.DropTable(
                name: "QuestionnaireData");
        }
    }
}
