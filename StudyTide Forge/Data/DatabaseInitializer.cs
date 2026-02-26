using Microsoft.EntityFrameworkCore;
using StudyTideForge.Models;

namespace StudyTideForge.Data;

public static class DatabaseInitializer
{
    private static readonly Dictionary<string, string> ModuleNameByCategory = new()
    {
        [ModuleCategories.CSharp] = "C# Mastery",
        [ModuleCategories.Azure] = "Azure Mastery",
        [ModuleCategories.Sql] = "SQL Mastery",
        [ModuleCategories.DevOps] = "DevOps Mastery",
        [ModuleCategories.SystemDesign] = "System Design Mastery",
        [ModuleCategories.Behavioral] = "Behavioral Mastery"
    };

    public static async Task InitializeAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ForgeDbContext>>();
        await using var db = await dbFactory.CreateDbContextAsync();

        await db.Database.MigrateAsync();

        if (await db.TrainingModules.AnyAsync())
        {
            return;
        }

        var now = DateTime.UtcNow;
        var modules = new Dictionary<string, TrainingModule>(StringComparer.OrdinalIgnoreCase);
        var lessons = new Dictionary<string, TrainingLesson>(StringComparer.OrdinalIgnoreCase);
        var orderByCategory = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var pair in TrainingSeedCatalog.GetPairs())
        {
            var category = MapCategory(pair.Topic);

            if (!modules.TryGetValue(category, out var module))
            {
                module = new TrainingModule
                {
                    Name = ModuleNameByCategory[category],
                    Category = category,
                    CreatedAt = now
                };

                modules[category] = module;
                orderByCategory[category] = 0;
            }

            var lessonKey = $"{category}:{pair.Topic}";

            if (!lessons.TryGetValue(lessonKey, out var lesson))
            {
                orderByCategory[category]++;

                lesson = new TrainingLesson
                {
                    Title = pair.Topic,
                    Module = module,
                    OrderIndex = orderByCategory[category],
                    CreatedAt = now
                };

                lessons[lessonKey] = lesson;
            }

            var difficulty = ComputeDifficulty(pair.Question, pair.Answer);

            lesson.Flashcards.Add(new Flashcard
            {
                Question = pair.Question.Trim(),
                Answer = pair.Answer.Trim(),
                Difficulty = difficulty,
                TimesCorrect = 0,
                TimesIncorrect = 0
            });

            lesson.TrainingBlocks.Add(new TrainingBlock
            {
                Title = pair.Topic,
                Content = $"Question:\n{pair.Question.Trim()}\n\nAnswer:\n{pair.Answer.Trim()}",
                Difficulty = difficulty,
                TimesPracticed = 0,
                TimesPerfect = 0,
                LastPracticedAt = null,
                NextDueAt = now
            });
        }

        db.TrainingModules.AddRange(modules.Values);
        await db.SaveChangesAsync();
    }

    private static string MapCategory(string topic)
    {
        if (topic.Contains("Azure", StringComparison.OrdinalIgnoreCase))
        {
            return ModuleCategories.Azure;
        }

        if (topic.Contains("C#", StringComparison.OrdinalIgnoreCase))
        {
            return ModuleCategories.CSharp;
        }

        if (topic.Contains("SQL", StringComparison.OrdinalIgnoreCase)
            || topic.Contains("Database", StringComparison.OrdinalIgnoreCase)
            || topic.Contains("Data", StringComparison.OrdinalIgnoreCase))
        {
            return ModuleCategories.Sql;
        }

        if (topic.Contains("DevOps", StringComparison.OrdinalIgnoreCase)
            || topic.Contains("Git", StringComparison.OrdinalIgnoreCase)
            || topic.Contains("CI/CD", StringComparison.OrdinalIgnoreCase))
        {
            return ModuleCategories.DevOps;
        }

        if (topic.Contains("System", StringComparison.OrdinalIgnoreCase)
            || topic.Contains("Architecture", StringComparison.OrdinalIgnoreCase))
        {
            return ModuleCategories.SystemDesign;
        }

        return ModuleCategories.Behavioral;
    }

    private static int ComputeDifficulty(string question, string answer)
    {
        var length = question.Length + answer.Length;

        if (length < 80)
        {
            return 2;
        }

        if (length < 120)
        {
            return 3;
        }

        if (length < 180)
        {
            return 4;
        }

        return 5;
    }
}
