using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlyBalds.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    IdentityProviderId = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    HasSubmittedQuestionnaire = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Author = table.Column<string>(type: "text", nullable: false),
                    PostedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ThreadId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThreadItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    PostsCount = table.Column<int>(type: "integer", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: false),
                    Creator = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreadItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionnaireItems_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommentItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Author = table.Column<string>(type: "text", nullable: false),
                    PostedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentItems_PostItems_PostId",
                        column: x => x.PostId,
                        principalTable: "PostItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    FavoritedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Favorites_PostItems_PostId",
                        column: x => x.PostId,
                        principalTable: "PostItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionnaireData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionnaireId = table.Column<Guid>(type: "uuid", nullable: false),
                    BaldType = table.Column<string>(type: "text", nullable: true),
                    CleaningMethods = table.Column<List<string>>(type: "text[]", nullable: true),
                    CleaningMethodsOther = table.Column<string>(type: "text", nullable: true),
                    BaldCareRoutine = table.Column<string>(type: "text", nullable: true),
                    ProductsUsed = table.Column<string>(type: "text", nullable: true),
                    MonthlySpend = table.Column<string>(type: "text", nullable: true),
                    ConfidenceLevel = table.Column<string>(type: "text", nullable: true),
                    Interests = table.Column<List<string>>(type: "text[]", nullable: true),
                    Goals = table.Column<List<string>>(type: "text[]", nullable: true),
                    GoalsOther = table.Column<string>(type: "text", nullable: true),
                    NewRoutine = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionnaireData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionnaireData_QuestionnaireItems_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalTable: "QuestionnaireItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentItems_PostId",
                table: "CommentItems",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_PostId",
                table: "Favorites",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireData_QuestionnaireId",
                table: "QuestionnaireData",
                column: "QuestionnaireId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionnaireItems_AccountId",
                table: "QuestionnaireItems",
                column: "AccountId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentItems");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "QuestionnaireData");

            migrationBuilder.DropTable(
                name: "ThreadItems");

            migrationBuilder.DropTable(
                name: "PostItems");

            migrationBuilder.DropTable(
                name: "QuestionnaireItems");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
