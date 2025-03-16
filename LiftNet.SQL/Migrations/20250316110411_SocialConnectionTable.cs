using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftNet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SocialConnectionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SocialConnections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TargetId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialConnections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialConnections_Users_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SocialConnections_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SocialConnections_TargetId",
                table: "SocialConnections",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialConnections_UserId",
                table: "SocialConnections",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SocialConnections");
        }
    }
}
