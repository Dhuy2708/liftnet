using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftNet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AlterColInLiftNetTrans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiftNetTransactions_Users_FromUserId",
                table: "LiftNetTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_LiftNetTransactions_Users_ToUserId",
                table: "LiftNetTransactions");

            migrationBuilder.AlterColumn<string>(
                name: "ToUserId",
                table: "LiftNetTransactions",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "FromUserId",
                table: "LiftNetTransactions",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_LiftNetTransactions_Users_FromUserId",
                table: "LiftNetTransactions",
                column: "FromUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LiftNetTransactions_Users_ToUserId",
                table: "LiftNetTransactions",
                column: "ToUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiftNetTransactions_Users_FromUserId",
                table: "LiftNetTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_LiftNetTransactions_Users_ToUserId",
                table: "LiftNetTransactions");

            migrationBuilder.AlterColumn<string>(
                name: "ToUserId",
                table: "LiftNetTransactions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FromUserId",
                table: "LiftNetTransactions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LiftNetTransactions_Users_FromUserId",
                table: "LiftNetTransactions",
                column: "FromUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LiftNetTransactions_Users_ToUserId",
                table: "LiftNetTransactions",
                column: "ToUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
