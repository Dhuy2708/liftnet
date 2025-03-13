using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftNet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RepairAppointment2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Appointments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldDefaultValue: "e1380ca8-8bfd-4dc7-9ca7-b249ae0b081f");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Appointments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "e1380ca8-8bfd-4dc7-9ca7-b249ae0b081f",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
