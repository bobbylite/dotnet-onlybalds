using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlyBalds.Api.Migrations
{
    /// <inheritdoc />
    public partial class CascadingDeleteForAccountQuestionnaire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionnaireId",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "HasSubmittedQuistionnaire",
                table: "Accounts",
                newName: "HasSubmittedQuestionnaire");

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "QuestionnaireItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireItems_AccountId",
                table: "QuestionnaireItems",
                column: "AccountId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionnaireItems_Accounts_AccountId",
                table: "QuestionnaireItems",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireItems_Accounts_AccountId",
                table: "QuestionnaireItems");

            migrationBuilder.DropIndex(
                name: "IX_QuestionnaireItems_AccountId",
                table: "QuestionnaireItems");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "QuestionnaireItems");

            migrationBuilder.RenameColumn(
                name: "HasSubmittedQuestionnaire",
                table: "Accounts",
                newName: "HasSubmittedQuistionnaire");

            migrationBuilder.AddColumn<string>(
                name: "QuestionnaireId",
                table: "Accounts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
