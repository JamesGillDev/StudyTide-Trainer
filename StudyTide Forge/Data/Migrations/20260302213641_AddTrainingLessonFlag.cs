using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyTideForge.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainingLessonFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFlagged",
                table: "TrainingLessons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingLessons_IsFlagged",
                table: "TrainingLessons",
                column: "IsFlagged");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrainingLessons_IsFlagged",
                table: "TrainingLessons");

            migrationBuilder.DropColumn(
                name: "IsFlagged",
                table: "TrainingLessons");
        }
    }
}
