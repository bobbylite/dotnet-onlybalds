using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlyBalds.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddFirstLastDisplayAddressToAccountItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Accounts",
                newName: "LastName");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Accounts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Accounts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Accounts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Accounts",
                newName: "Name");
        }
    }
}
