using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftNet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedModifiedField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Appointments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "Appointments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_BookerId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "BookerId",
                table: "Appointments",
                newName: "BookerId1");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_BookerId",
                table: "Appointments",
                newName: "IX_Appointments_BookerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_BookerId1",
                table: "Appointments",
                column: "BookerId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
