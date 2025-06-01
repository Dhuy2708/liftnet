using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftNet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeleteTypeAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                    name: "FK_AppointmentSeenStatuses_Appointments_AppointmentId",
                    table: "AppointmentSeenStatuses");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentSeenStatuses_Appointments_AppointmentId",
                table: "AppointmentSeenStatuses",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
