using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyTideForge.Data.Migrations;

[DbContext(typeof(ForgeDbContext))]
[Migration("20260226220000_InitialForge")]
public sealed class InitialForge : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "TrainingModules",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                Category = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TrainingModules", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "TrainingLessons",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                ModuleId = table.Column<int>(type: "INTEGER", nullable: false),
                Title = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                OrderIndex = table.Column<int>(type: "INTEGER", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TrainingLessons", x => x.Id);
                table.ForeignKey(
                    name: "FK_TrainingLessons_TrainingModules_ModuleId",
                    column: x => x.ModuleId,
                    principalTable: "TrainingModules",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Flashcards",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                LessonId = table.Column<int>(type: "INTEGER", nullable: false),
                Question = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                Answer = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                Difficulty = table.Column<int>(type: "INTEGER", nullable: false),
                TimesCorrect = table.Column<int>(type: "INTEGER", nullable: false),
                TimesIncorrect = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Flashcards", x => x.Id);
                table.ForeignKey(
                    name: "FK_Flashcards_TrainingLessons_LessonId",
                    column: x => x.LessonId,
                    principalTable: "TrainingLessons",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "TrainingBlocks",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                LessonId = table.Column<int>(type: "INTEGER", nullable: false),
                Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                Content = table.Column<string>(type: "TEXT", nullable: false),
                Difficulty = table.Column<int>(type: "INTEGER", nullable: false),
                TimesPracticed = table.Column<int>(type: "INTEGER", nullable: false),
                TimesPerfect = table.Column<int>(type: "INTEGER", nullable: false),
                LastPracticedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                NextDueAt = table.Column<DateTime>(type: "TEXT", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TrainingBlocks", x => x.Id);
                table.ForeignKey(
                    name: "FK_TrainingBlocks_TrainingLessons_LessonId",
                    column: x => x.LessonId,
                    principalTable: "TrainingLessons",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "PracticeAttempts",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                TrainingBlockId = table.Column<int>(type: "INTEGER", nullable: false),
                AttemptedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                TypedText = table.Column<string>(type: "TEXT", nullable: false),
                AccuracyPercent = table.Column<double>(type: "REAL", nullable: false),
                ErrorCount = table.Column<int>(type: "INTEGER", nullable: false),
                FirstMismatchIndex = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PracticeAttempts", x => x.Id);
                table.ForeignKey(
                    name: "FK_PracticeAttempts_TrainingBlocks_TrainingBlockId",
                    column: x => x.TrainingBlockId,
                    principalTable: "TrainingBlocks",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Flashcards_LessonId",
            table: "Flashcards",
            column: "LessonId");

        migrationBuilder.CreateIndex(
            name: "IX_PracticeAttempts_AttemptedAt",
            table: "PracticeAttempts",
            column: "AttemptedAt");

        migrationBuilder.CreateIndex(
            name: "IX_PracticeAttempts_TrainingBlockId",
            table: "PracticeAttempts",
            column: "TrainingBlockId");

        migrationBuilder.CreateIndex(
            name: "IX_TrainingBlocks_LessonId",
            table: "TrainingBlocks",
            column: "LessonId");

        migrationBuilder.CreateIndex(
            name: "IX_TrainingBlocks_NextDueAt",
            table: "TrainingBlocks",
            column: "NextDueAt");

        migrationBuilder.CreateIndex(
            name: "IX_TrainingLessons_ModuleId_OrderIndex",
            table: "TrainingLessons",
            columns: new[] { "ModuleId", "OrderIndex" });

        migrationBuilder.CreateIndex(
            name: "IX_TrainingModules_Category_Name",
            table: "TrainingModules",
            columns: new[] { "Category", "Name" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Flashcards");

        migrationBuilder.DropTable(
            name: "PracticeAttempts");

        migrationBuilder.DropTable(
            name: "TrainingBlocks");

        migrationBuilder.DropTable(
            name: "TrainingLessons");

        migrationBuilder.DropTable(
            name: "TrainingModules");
    }
}
