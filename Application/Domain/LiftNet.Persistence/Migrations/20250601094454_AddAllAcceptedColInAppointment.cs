using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftNet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAllAcceptedColInAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllAccepted",
                table: "Appointments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllAccepted",
                table: "Appointments");
        }
    }
}
