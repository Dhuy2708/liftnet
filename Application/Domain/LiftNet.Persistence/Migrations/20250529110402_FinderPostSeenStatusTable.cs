using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftNet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FinderPostSeenStatusTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinderPostSeenStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FinderPostId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NotiCount = table.Column<int>(type: "int", nullable: false),
                    LastSeen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinderPostSeenStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinderPostSeenStatuses_FinderPosts_FinderPostId",
                        column: x => x.FinderPostId,
                        principalTable: "FinderPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_FinderPostSeenStatuses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinderPostSeenStatuses_FinderPostId",
                table: "FinderPostSeenStatuses",
                column: "FinderPostId");

            migrationBuilder.CreateIndex(
                name: "IX_FinderPostSeenStatuses_UserId",
                table: "FinderPostSeenStatuses",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinderPostSeenStatuses");
        }
    }
}
