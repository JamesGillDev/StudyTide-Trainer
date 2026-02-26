using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyTideTrainer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Difficulty = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Snippets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TopicId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    SourceText = table.Column<string>(type: "TEXT", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TimesPracticed = table.Column<int>(type: "INTEGER", nullable: false),
                    TimesPerfect = table.Column<int>(type: "INTEGER", nullable: false),
                    LastPracticedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NextDueAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Snippets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Snippets_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PracticeAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SnippetId = table.Column<int>(type: "INTEGER", nullable: false),
                    AttemptedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TypedText = table.Column<string>(type: "TEXT", nullable: false),
                    AccuracyPercent = table.Column<double>(type: "REAL", nullable: false),
                    ErrorCount = table.Column<int>(type: "INTEGER", nullable: false),
                    MissingChars = table.Column<int>(type: "INTEGER", nullable: false),
                    ExtraChars = table.Column<int>(type: "INTEGER", nullable: false),
                    FirstMismatchIndex = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticeAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PracticeAttempts_Snippets_SnippetId",
                        column: x => x.SnippetId,
                        principalTable: "Snippets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PracticeAttempts_AttemptedAt",
                table: "PracticeAttempts",
                column: "AttemptedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PracticeAttempts_SnippetId",
                table: "PracticeAttempts",
                column: "SnippetId");

            migrationBuilder.CreateIndex(
                name: "IX_Snippets_NextDueAt",
                table: "Snippets",
                column: "NextDueAt");

            migrationBuilder.CreateIndex(
                name: "IX_Snippets_TopicId",
                table: "Snippets",
                column: "TopicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PracticeAttempts");

            migrationBuilder.DropTable(
                name: "Snippets");

            migrationBuilder.DropTable(
                name: "Topics");
        }
    }
}
