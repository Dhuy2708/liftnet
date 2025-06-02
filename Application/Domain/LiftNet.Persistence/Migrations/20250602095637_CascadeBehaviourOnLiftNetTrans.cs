using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftNet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CascadeBehaviourOnLiftNetTrans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiftNetTransactions_Appointments_AppointmentId",
                table: "LiftNetTransactions");

            migrationBuilder.AddForeignKey(
                name: "FK_LiftNetTransactions_Appointments_AppointmentId",
                table: "LiftNetTransactions",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiftNetTransactions_Appointments_AppointmentId",
                table: "LiftNetTransactions");

            migrationBuilder.AddForeignKey(
                name: "FK_LiftNetTransactions_Appointments_AppointmentId",
                table: "LiftNetTransactions",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
