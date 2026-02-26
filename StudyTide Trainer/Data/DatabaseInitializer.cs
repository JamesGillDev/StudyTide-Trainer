using Microsoft.EntityFrameworkCore;
using StudyTideTrainer.Models;

namespace StudyTideTrainer.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<TrainingDbContext>>();
        await using var db = await dbFactory.CreateDbContextAsync();

        // This app is migration-first. If no migrations exist yet, skip boot-time seeding.
        var hasMigrations = db.Database.GetMigrations().Any();
        if (!hasMigrations)
        {
            return;
        }

        await db.Database.MigrateAsync();

        if (await db.Topics.AnyAsync())
        {
            return;
        }

        var now = DateTime.UtcNow;

        foreach (var seedTopic in TrainingPackCatalog.GetAllPacks())
        {
            var topic = new Topic
            {
                Name = seedTopic.Name,
                Category = seedTopic.Category,
                Difficulty = seedTopic.Difficulty,
                CreatedAt = now
            };

            foreach (var seedSnippet in seedTopic.Snippets)
            {
                topic.Snippets.Add(new Snippet
                {
                    Title = seedSnippet.Title,
                    SourceText = seedSnippet.SourceText,
                    Tags = seedSnippet.Tags,
                    CreatedAt = now,
                    NextDueAt = now,
                    TimesPracticed = 0,
                    TimesPerfect = 0
                });
            }

            db.Topics.Add(topic);
        }

        await db.SaveChangesAsync();
    }
}
