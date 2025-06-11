using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiftNet.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class IntermediateTableExerciseAndPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ExerciseTrainingPlan",
                table: "ExerciseTrainingPlan");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ExerciseTrainingPlan",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<float>(
                name: "Order",
                table: "ExerciseTrainingPlan",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExerciseTrainingPlan",
                table: "ExerciseTrainingPlan",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseTrainingPlan_ExercisesSelfId",
                table: "ExerciseTrainingPlan",
                column: "ExercisesSelfId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ExerciseTrainingPlan",
                table: "ExerciseTrainingPlan");

            migrationBuilder.DropIndex(
                name: "IX_ExerciseTrainingPlan_ExercisesSelfId",
                table: "ExerciseTrainingPlan");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ExerciseTrainingPlan");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "ExerciseTrainingPlan");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExerciseTrainingPlan",
                table: "ExerciseTrainingPlan",
                columns: new[] { "ExercisesSelfId", "TrainingPlansId" });
        }
    }
}
