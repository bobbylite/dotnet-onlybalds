﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlyBalds.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddNameToAccountItemInDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Accounts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Accounts");
        }
    }
}
