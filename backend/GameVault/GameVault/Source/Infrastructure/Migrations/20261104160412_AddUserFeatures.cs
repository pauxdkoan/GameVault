using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameVault.Source.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameListItems_GameLists_GameListId",
                table: "GameListItems");

            migrationBuilder.DropIndex(
                name: "IX_UserGames_ApplicationUserId",
                table: "UserGames");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ApplicationUserId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_GameListItems_GameListId",
                table: "GameListItems");

            migrationBuilder.AlterColumn<string>(
                name: "PrivateNotes",
                table: "UserGames",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Reviews",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GameLists",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "GameLists",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "GameListItems",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ReviewComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewComments_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReviewComments_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReviewLikes",
                columns: table => new
                {
                    ReviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewLikes", x => new { x.ReviewId, x.ApplicationUserId });
                    table.ForeignKey(
                        name: "FK_ReviewLikes_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReviewLikes_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserGames_ApplicationUserId_GameId",
                table: "UserGames",
                columns: new[] { "ApplicationUserId", "GameId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ApplicationUserId_GameId",
                table: "Reviews",
                columns: new[] { "ApplicationUserId", "GameId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Platforms_ExternalId",
                table: "Platforms",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Genres_ExternalId",
                table: "Genres",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameListItems_GameListId_GameId",
                table: "GameListItems",
                columns: new[] { "GameListId", "GameId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Game_ExternalId",
                table: "Game",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewComments_ApplicationUserId",
                table: "ReviewComments",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewComments_ReviewId",
                table: "ReviewComments",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewLikes_ApplicationUserId",
                table: "ReviewLikes",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameListItems_GameLists_GameListId",
                table: "GameListItems",
                column: "GameListId",
                principalTable: "GameLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameListItems_GameLists_GameListId",
                table: "GameListItems");

            migrationBuilder.DropTable(
                name: "ReviewComments");

            migrationBuilder.DropTable(
                name: "ReviewLikes");

            migrationBuilder.DropIndex(
                name: "IX_UserGames_ApplicationUserId_GameId",
                table: "UserGames");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ApplicationUserId_GameId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Platforms_ExternalId",
                table: "Platforms");

            migrationBuilder.DropIndex(
                name: "IX_Genres_ExternalId",
                table: "Genres");

            migrationBuilder.DropIndex(
                name: "IX_GameListItems_GameListId_GameId",
                table: "GameListItems");

            migrationBuilder.DropIndex(
                name: "IX_Game_ExternalId",
                table: "Game");

            migrationBuilder.AlterColumn<string>(
                name: "PrivateNotes",
                table: "UserGames",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GameLists",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "GameLists",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "GameListItems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserGames_ApplicationUserId",
                table: "UserGames",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ApplicationUserId",
                table: "Reviews",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GameListItems_GameListId",
                table: "GameListItems",
                column: "GameListId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameListItems_GameLists_GameListId",
                table: "GameListItems",
                column: "GameListId",
                principalTable: "GameLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
