using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftNet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UserFKInTrainingPlanTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "TrainingPlans",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlans_UserId",
                table: "TrainingPlans",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingPlans_Users_UserId",
                table: "TrainingPlans",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingPlans_Users_UserId",
                table: "TrainingPlans");

            migrationBuilder.DropIndex(
                name: "IX_TrainingPlans_UserId",
                table: "TrainingPlans");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TrainingPlans");
        }
    }
}
