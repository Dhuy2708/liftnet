using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftNet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppointmentConfirmation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentConfirmations_Users_UserId",
                table: "AppointmentConfirmations");

            migrationBuilder.DropIndex(
                name: "IX_AppointmentConfirmations_UserId",
                table: "AppointmentConfirmations");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AppointmentConfirmations");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "AppointmentConfirmations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AppointmentConfirmations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "AppointmentConfirmations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AppointmentConfirmations");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AppointmentConfirmations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentConfirmations_UserId",
                table: "AppointmentConfirmations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentConfirmations_Users_UserId",
                table: "AppointmentConfirmations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
