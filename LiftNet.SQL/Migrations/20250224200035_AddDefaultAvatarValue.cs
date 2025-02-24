using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftNet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultAvatarValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Avatar",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "https://res.cloudinary.com/dvwgt4tm1/image/upload/v1730031850/360_F_549983970_bRCkYfk0P6PP5fKbMhZMIb07mCJ6esXL_t9czwt.jpg",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Avatar",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "https://res.cloudinary.com/dvwgt4tm1/image/upload/v1730031850/360_F_549983970_bRCkYfk0P6PP5fKbMhZMIb07mCJ6esXL_t9czwt.jpg");
        }
    }
}
