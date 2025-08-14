using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlyBalds.Api.Migrations
{
    /// <inheritdoc />
    public partial class QuestionnaireNullability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<List<string>>(
                name: "Interests",
                table: "QuestionnaireData",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(List<string>),
                oldType: "text[]");

            migrationBuilder.AlterColumn<List<string>>(
                name: "Goals",
                table: "QuestionnaireData",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(List<string>),
                oldType: "text[]");

            migrationBuilder.AlterColumn<List<string>>(
                name: "CleaningMethods",
                table: "QuestionnaireData",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(List<string>),
                oldType: "text[]");

            migrationBuilder.AlterColumn<string>(
                name: "BaldType",
                table: "QuestionnaireData",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<List<string>>(
                name: "Interests",
                table: "QuestionnaireData",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<List<string>>(
                name: "Goals",
                table: "QuestionnaireData",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<List<string>>(
                name: "CleaningMethods",
                table: "QuestionnaireData",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BaldType",
                table: "QuestionnaireData",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

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
    }
}
