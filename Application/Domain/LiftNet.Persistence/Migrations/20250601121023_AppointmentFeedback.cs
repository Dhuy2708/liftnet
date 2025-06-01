using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftNet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AppointmentFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppointmentFeedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApppointmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReviewerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Img = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Star = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentFeedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentFeedbacks_Appointments_ApppointmentId",
                        column: x => x.ApppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentFeedbacks_Users_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentFeedbacks_ApppointmentId",
                table: "AppointmentFeedbacks",
                column: "ApppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentFeedbacks_ReviewerId",
                table: "AppointmentFeedbacks",
                column: "ReviewerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentFeedbacks");
        }
    }
}
