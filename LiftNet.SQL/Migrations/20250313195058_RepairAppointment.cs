using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftNet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RepairAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Appointments",
                newName: "Address");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Appointments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "e1380ca8-8bfd-4dc7-9ca7-b249ae0b081f",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Appointments",
                newName: "Location");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Appointments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldDefaultValue: "e1380ca8-8bfd-4dc7-9ca7-b249ae0b081f");
        }
    }
}
