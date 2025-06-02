using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftNet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLiftNetTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppointmentId",
                table: "LiftNetTransactions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LiftNetTransactions_AppointmentId",
                table: "LiftNetTransactions",
                column: "AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_LiftNetTransactions_Appointments_AppointmentId",
                table: "LiftNetTransactions",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiftNetTransactions_Appointments_AppointmentId",
                table: "LiftNetTransactions");

            migrationBuilder.DropIndex(
                name: "IX_LiftNetTransactions_AppointmentId",
                table: "LiftNetTransactions");

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "LiftNetTransactions");
        }
    }
}
