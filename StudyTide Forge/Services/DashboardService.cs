using Microsoft.EntityFrameworkCore;
using StudyTideForge.Data;

namespace StudyTideForge.Services;

public sealed class DashboardService(IDbContextFactory<ForgeDbContext> dbFactory)
{
    public async Task<DashboardSnapshot> GetSnapshotAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var now = DateTime.UtcNow;
        var sevenDaysAgo = now.AddDays(-7);

        var totalModules = await db.TrainingModules.CountAsync();
        var blocksDue = await db.TrainingBlocks.CountAsync(x => !x.NextDueAt.HasValue || x.NextDueAt <= now);
        var flashcardsDue = await db.Flashcards.CountAsync(x => (x.TimesCorrect + x.TimesIncorrect) == 0 || x.TimesIncorrect >= x.TimesCorrect);
        var importedModuleNames = LegacyImportConstants.ModuleDefinitions
            .Select(x => $"{LegacyImportConstants.ModuleNamePrefix}{x.Name}")
            .ToList();
        var importedFlashcards = await db.Flashcards
            .CountAsync(x =>
                importedModuleNames.Contains(x.Lesson!.Module!.Name));
        var importedTrainingItems = await db.TrainingBlocks
            .CountAsync(x =>
                importedModuleNames.Contains(x.Lesson!.Module!.Name));

        var accuracyLast7Days = await db.PracticeAttempts
            .Where(x => x.AttemptedAt >= sevenDaysAgo)
            .Select(x => (double?)x.AccuracyPercent)
            .AverageAsync() ?? 0;

        var weakestBlocks = await db.PracticeAttempts
            .GroupBy(x => x.TrainingBlockId)
            .Select(group => new
            {
                TrainingBlockId = group.Key,
                AvgAccuracy = group.Average(x => x.AccuracyPercent),
                Attempts = group.Count()
            })
            .OrderBy(x => x.AvgAccuracy)
            .ThenByDescending(x => x.Attempts)
            .Take(5)
            .Join(
                db.TrainingBlocks.Include(x => x.Lesson),
                metric => metric.TrainingBlockId,
                block => block.Id,
                (metric, block) => new WeakBlock
                {
                    TrainingBlockId = block.Id,
                    Title = block.Title,
                    LessonTitle = block.Lesson!.Title,
                    AverageAccuracy = Math.Round(metric.AvgAccuracy, 2),
                    Attempts = metric.Attempts
                })
            .ToListAsync();

        var weakestFlashcards = await db.Flashcards
            .AsNoTracking()
            .Include(x => x.Lesson)
            .Where(x => (x.TimesCorrect + x.TimesIncorrect) > 0)
            .Select(x => new WeakFlashcard
            {
                FlashcardId = x.Id,
                Question = x.Question,
                LessonTitle = x.Lesson!.Title,
                Attempts = x.TimesCorrect + x.TimesIncorrect,
                SuccessRate = Math.Round((double)x.TimesCorrect / (x.TimesCorrect + x.TimesIncorrect) * 100, 2)
            })
            .OrderBy(x => x.SuccessRate)
            .ThenByDescending(x => x.Attempts)
            .Take(5)
            .ToListAsync();

        return new DashboardSnapshot
        {
            TotalModules = totalModules,
            BlocksDue = blocksDue,
            FlashcardsDue = flashcardsDue,
            ImportedFlashcards = importedFlashcards,
            ImportedTrainingItems = importedTrainingItems,
            AccuracyLast7Days = Math.Round(accuracyLast7Days, 2),
            WeakestBlocks = weakestBlocks,
            WeakestFlashcards = weakestFlashcards
        };
    }
}

public sealed class DashboardSnapshot
{
    public int TotalModules { get; init; }

    public int BlocksDue { get; init; }

    public int FlashcardsDue { get; init; }

    public int ImportedFlashcards { get; init; }

    public int ImportedTrainingItems { get; init; }

    public double AccuracyLast7Days { get; init; }

    public IReadOnlyList<WeakBlock> WeakestBlocks { get; init; } = [];

    public IReadOnlyList<WeakFlashcard> WeakestFlashcards { get; init; } = [];
}

public sealed class WeakBlock
{
    public int TrainingBlockId { get; init; }

    public string Title { get; init; } = string.Empty;

    public string LessonTitle { get; init; } = string.Empty;

    public double AverageAccuracy { get; init; }

    public int Attempts { get; init; }
}

public sealed class WeakFlashcard
{
    public int FlashcardId { get; init; }

    public string Question { get; init; } = string.Empty;

    public string LessonTitle { get; init; } = string.Empty;

    public double SuccessRate { get; init; }

    public int Attempts { get; init; }
}
