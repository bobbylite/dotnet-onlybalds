using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlyBalds.Api.Migrations
{
    /// <inheritdoc />
    public partial class MakeCleaningMethodsNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Make CleaningMethods nullable
            migrationBuilder.AlterColumn<string[]>(
                name: "CleaningMethods",
                table: "QuestionnaireData",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldNullable: false);

            // Existing foreign key changes
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireItems_QuestionnaireData_DataId",
                table: "QuestionnaireItems");

            migrationBuilder.DropIndex(
                name: "IX_QuestionnaireItems_DataId",
                table: "QuestionnaireItems");

            migrationBuilder.DropColumn(
                name: "DataId",
                table: "QuestionnaireItems");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireData_QuestionnaireId",
                table: "QuestionnaireData",
                column: "QuestionnaireId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionnaireData_QuestionnaireItems_QuestionnaireId",
                table: "QuestionnaireData",
                column: "QuestionnaireId",
                principalTable: "QuestionnaireItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireData_QuestionnaireItems_QuestionnaireId",
                table: "QuestionnaireData");

            migrationBuilder.DropIndex(
                name: "IX_QuestionnaireData_QuestionnaireId",
                table: "QuestionnaireData");

            migrationBuilder.AddColumn<Guid>(
                name: "DataId",
                table: "QuestionnaireItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireItems_DataId",
                table: "QuestionnaireItems",
                column: "DataId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionnaireItems_QuestionnaireData_DataId",
                table: "QuestionnaireItems",
                column: "DataId",
                principalTable: "QuestionnaireData",
                principalColumn: "Id");

            migrationBuilder.AlterColumn<string[]>(
                name: "CleaningMethods",
                table: "QuestionnaireData",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldNullable: true);

        }
    }
}
