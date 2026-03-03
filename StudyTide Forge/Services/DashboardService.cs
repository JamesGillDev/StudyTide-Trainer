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
        var lessonRows = await db.TrainingLessons
            .AsNoTracking()
            .Select(x => new
            {
                BlockCount = x.TrainingBlocks.Count,
                IsCompleted = x.StudyProgress != null &&
                              (x.StudyProgress.IsCompleted ||
                               (x.TrainingBlocks.Count > 0 && x.StudyProgress.HighestBlockIndex >= x.TrainingBlocks.Count - 1))
            })
            .ToListAsync();
        var totalLessons = lessonRows.Count;
        var completedStudyLessons = lessonRows.Count(x => x.IsCompleted);
        var totalTrainingItems = await db.TrainingBlocks.CountAsync();
        var totalFlashcards = await db.Flashcards.CountAsync();
        var completedPracticeItems = await db.TrainingBlocks
            .CountAsync(x => x.TimesPracticed > 0);
        var completedFlashcards = await db.Flashcards
            .CountAsync(x => (x.TimesCorrect + x.TimesIncorrect) > 0);
        var blocksDue = await db.TrainingBlocks.CountAsync(x => !x.NextDueAt.HasValue || x.NextDueAt <= now);
        var flashcardsDue = await db.Flashcards.CountAsync(x => (x.TimesCorrect + x.TimesIncorrect) == 0 || x.TimesIncorrect >= x.TimesCorrect);
        var categoryRows = await db.TrainingModules
            .AsNoTracking()
            .Select(x => new
            {
                x.Category,
                LessonCount = x.Lessons.Count,
                BlockCount = x.Lessons.SelectMany(lesson => lesson.TrainingBlocks).Count(),
                FlashcardCount = x.Lessons.SelectMany(lesson => lesson.Flashcards).Count()
            })
            .ToListAsync();
        var categoryCoverage = categoryRows
            .GroupBy(x => x.Category, StringComparer.OrdinalIgnoreCase)
            .Select(group =>
            {
                var trainingItemCount = group.Sum(x => x.BlockCount);
                var recommendationTarget = GetRecommendedTrainingItemTarget(group.Key);
                var gap = Math.Max(0, recommendationTarget - trainingItemCount);
                var coveragePercent = recommendationTarget == 0
                    ? 100
                    : Math.Min(100, (double)trainingItemCount / recommendationTarget * 100);

                return new CategoryCoverage
                {
                    Category = group.Key,
                    Modules = group.Count(),
                    Lessons = group.Sum(x => x.LessonCount),
                    TrainingItems = trainingItemCount,
                    Flashcards = group.Sum(x => x.FlashcardCount),
                    RecommendedMinimumTrainingItems = recommendationTarget,
                    AdditionalTrainingItemsNeeded = gap,
                    CoveragePercent = Math.Round(coveragePercent, 2)
                };
            })
            .OrderBy(x => x.Category)
            .ToList();

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
            TotalLessons = totalLessons,
            TotalTrainingItems = totalTrainingItems,
            TotalFlashcards = totalFlashcards,
            CompletedStudyLessons = completedStudyLessons,
            CompletedPracticeItems = completedPracticeItems,
            CompletedFlashcards = completedFlashcards,
            BlocksDue = blocksDue,
            FlashcardsDue = flashcardsDue,
            AccuracyLast7Days = Math.Round(accuracyLast7Days, 2),
            CategoryCoverage = categoryCoverage,
            WeakestBlocks = weakestBlocks,
            WeakestFlashcards = weakestFlashcards
        };
    }

    private static int GetRecommendedTrainingItemTarget(string category)
    {
        return category switch
        {
            "Azure" => 500,
            "C#" => 450,
            "SQL" => 350,
            "DevOps" => 300,
            "System Design" => 300,
            "Behavioral" => 150,
            _ => 250
        };
    }
}

public sealed class DashboardSnapshot
{
    public int TotalModules { get; init; }

    public int TotalLessons { get; init; }

    public int TotalTrainingItems { get; init; }

    public int TotalFlashcards { get; init; }

    public int CompletedStudyLessons { get; init; }

    public int CompletedPracticeItems { get; init; }

    public int CompletedFlashcards { get; init; }

    public double StudyLibraryCompletionPercent => ComputePercent(CompletedStudyLessons, TotalLessons);

    public double PracticeCompletionPercent => ComputePercent(CompletedPracticeItems, TotalTrainingItems);

    public double FlashcardsCompletionPercent => ComputePercent(CompletedFlashcards, TotalFlashcards);

    public int BlocksDue { get; init; }

    public int FlashcardsDue { get; init; }

    public double AccuracyLast7Days { get; init; }

    public IReadOnlyList<CategoryCoverage> CategoryCoverage { get; init; } = [];

    public IReadOnlyList<WeakBlock> WeakestBlocks { get; init; } = [];

    public IReadOnlyList<WeakFlashcard> WeakestFlashcards { get; init; } = [];

    private static double ComputePercent(int completed, int total)
    {
        if (total <= 0)
        {
            return 100;
        }

        return Math.Round((double)completed / total * 100, 2);
    }
}

public sealed class CategoryCoverage
{
    public string Category { get; init; } = string.Empty;

    public int Modules { get; init; }

    public int Lessons { get; init; }

    public int TrainingItems { get; init; }

    public int Flashcards { get; init; }

    public int RecommendedMinimumTrainingItems { get; init; }

    public int AdditionalTrainingItemsNeeded { get; init; }

    public double CoveragePercent { get; init; }
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
