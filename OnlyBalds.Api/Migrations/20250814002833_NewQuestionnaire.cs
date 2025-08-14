using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlyBalds.Api.Migrations
{
    /// <inheritdoc />
    public partial class NewQuestionnaire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Question");

            migrationBuilder.DropTable(
                name: "BaldingOption");

            migrationBuilder.DropTable(
                name: "BaldingOptions");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "QuestionnaireItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "QuestionnaireItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "QuestionnaireItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "QuestionnaireItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BaldCareRoutine",
                table: "QuestionnaireData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BaldType",
                table: "QuestionnaireData",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<List<string>>(
                name: "CleaningMethods",
                table: "QuestionnaireData",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "CleaningMethodsOther",
                table: "QuestionnaireData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConfidenceLevel",
                table: "QuestionnaireData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "Goals",
                table: "QuestionnaireData",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "GoalsOther",
                table: "QuestionnaireData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "Interests",
                table: "QuestionnaireData",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "MonthlySpend",
                table: "QuestionnaireData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewRoutine",
                table: "QuestionnaireData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductsUsed",
                table: "QuestionnaireData",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "QuestionnaireItems");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "QuestionnaireItems");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "QuestionnaireItems");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "QuestionnaireItems");

            migrationBuilder.DropColumn(
                name: "BaldCareRoutine",
                table: "QuestionnaireData");

            migrationBuilder.DropColumn(
                name: "BaldType",
                table: "QuestionnaireData");

            migrationBuilder.DropColumn(
                name: "CleaningMethods",
                table: "QuestionnaireData");

            migrationBuilder.DropColumn(
                name: "CleaningMethodsOther",
                table: "QuestionnaireData");

            migrationBuilder.DropColumn(
                name: "ConfidenceLevel",
                table: "QuestionnaireData");

            migrationBuilder.DropColumn(
                name: "Goals",
                table: "QuestionnaireData");

            migrationBuilder.DropColumn(
                name: "GoalsOther",
                table: "QuestionnaireData");

            migrationBuilder.DropColumn(
                name: "Interests",
                table: "QuestionnaireData");

            migrationBuilder.DropColumn(
                name: "MonthlySpend",
                table: "QuestionnaireData");

            migrationBuilder.DropColumn(
                name: "NewRoutine",
                table: "QuestionnaireData");

            migrationBuilder.DropColumn(
                name: "ProductsUsed",
                table: "QuestionnaireData");

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
                name: "BaldingOption",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BaldingOptionTitle = table.Column<string>(type: "text", nullable: false),
                    BaldingOptionsId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaldingOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaldingOption_BaldingOptions_BaldingOptionsId",
                        column: x => x.BaldingOptionsId,
                        principalTable: "BaldingOptions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Question",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Answer = table.Column<string>(type: "text", nullable: false),
                    BaldingOptionId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuestionnaireDataId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false)
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
                name: "IX_BaldingOption_BaldingOptionsId",
                table: "BaldingOption",
                column: "BaldingOptionsId");

            migrationBuilder.CreateIndex(
                name: "IX_BaldingOptions_QuestionnaireDataId",
                table: "BaldingOptions",
                column: "QuestionnaireDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_BaldingOptionId",
                table: "Question",
                column: "BaldingOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Question_QuestionnaireDataId",
                table: "Question",
                column: "QuestionnaireDataId");
        }
    }
}
