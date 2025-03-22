using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftNet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeClientIdColumnToBookerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_ClientId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "Appointments",
                newName: "BookerId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_ClientId",
                table: "Appointments",
                newName: "IX_Appointments_BookerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_BookerId",
                table: "Appointments",
                column: "BookerId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_BookerId1",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "BookerId1",
                table: "Appointments",
                newName: "BookerId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_BookerId1",
                table: "Appointments",
                newName: "IX_Appointments_BookerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_BookerId",
                table: "Appointments",
                column: "BookerId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
