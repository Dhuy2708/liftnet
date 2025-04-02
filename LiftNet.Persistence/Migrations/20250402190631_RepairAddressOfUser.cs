using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftNet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RepairAddressOfUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Users",
                newName: "Location");

            migrationBuilder.AddColumn<int>(
                name: "DistrictCode",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProvinceCode",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WardCode",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_DistrictCode",
                table: "Users",
                column: "DistrictCode");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ProvinceCode",
                table: "Users",
                column: "ProvinceCode");

            migrationBuilder.CreateIndex(
                name: "IX_Users_WardCode",
                table: "Users",
                column: "WardCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Districts_DistrictCode",
                table: "Users",
                column: "DistrictCode",
                principalTable: "Districts",
                principalColumn: "Code");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Provinces_ProvinceCode",
                table: "Users",
                column: "ProvinceCode",
                principalTable: "Provinces",
                principalColumn: "Code");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Wards_WardCode",
                table: "Users",
                column: "WardCode",
                principalTable: "Wards",
                principalColumn: "Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Districts_DistrictCode",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Provinces_ProvinceCode",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Wards_WardCode",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DistrictCode",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ProvinceCode",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_WardCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DistrictCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProvinceCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "WardCode",
                table: "Users");

            migrationBuilder.RenameColumn(
              name: "Location", 
              table: "Users",
              newName: "Address");
        }
    }
}
