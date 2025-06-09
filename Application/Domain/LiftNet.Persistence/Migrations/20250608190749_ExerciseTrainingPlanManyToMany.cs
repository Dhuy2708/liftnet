using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftNet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ExerciseTrainingPlanManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercises_TrainingPlans_TrainingPlanId",
                table: "Exercises");

            migrationBuilder.DropIndex(
                name: "IX_Exercises_TrainingPlanId",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "TrainingPlanId",
                table: "Exercises");

            migrationBuilder.CreateTable(
                name: "ExerciseTrainingPlan",
                columns: table => new
                {
                    ExercisesSelfId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TrainingPlansId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseTrainingPlan", x => new { x.ExercisesSelfId, x.TrainingPlansId });
                    table.ForeignKey(
                        name: "FK_ExerciseTrainingPlan_Exercises_ExercisesSelfId",
                        column: x => x.ExercisesSelfId,
                        principalTable: "Exercises",
                        principalColumn: "SelfId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExerciseTrainingPlan_TrainingPlans_TrainingPlansId",
                        column: x => x.TrainingPlansId,
                        principalTable: "TrainingPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseTrainingPlan_TrainingPlansId",
                table: "ExerciseTrainingPlan",
                column: "TrainingPlansId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExerciseTrainingPlan");

            migrationBuilder.AddColumn<int>(
                name: "TrainingPlanId",
                table: "Exercises",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_TrainingPlanId",
                table: "Exercises",
                column: "TrainingPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercises_TrainingPlans_TrainingPlanId",
                table: "Exercises",
                column: "TrainingPlanId",
                principalTable: "TrainingPlans",
                principalColumn: "Id");
        }
    }
}
