using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlyBalds.Api.Migrations
{
    /// <inheritdoc />
    public partial class NewQuestionnaireCascadingDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionnaireItems_QuestionnaireData_DataId",
                table: "QuestionnaireItems");

            migrationBuilder.DropIndex(
                name: "IX_QuestionnaireItems_DataId",
                table: "QuestionnaireItems");

            migrationBuilder.DropColumn(
                name: "DataId",
                table: "QuestionnaireItems");

            migrationBuilder.AddColumn<Guid>(
                name: "QuestionnaireId",
                table: "QuestionnaireData",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

            migrationBuilder.DropColumn(
                name: "QuestionnaireId",
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
        }
    }
}
