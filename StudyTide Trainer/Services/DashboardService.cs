using Microsoft.EntityFrameworkCore;
using StudyTideTrainer.Data;

namespace StudyTideTrainer.Services;

public sealed class DashboardService(IDbContextFactory<TrainingDbContext> dbFactory)
{
    public async Task<DashboardSnapshot> GetSnapshotAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var now = DateTime.UtcNow;
        var todayStart = now.Date;
        var sevenDaysAgo = now.AddDays(-7);

        var totalSnippets = await db.Snippets.CountAsync();
        var snippetsDueNow = await db.Snippets.CountAsync(x => !x.NextDueAt.HasValue || x.NextDueAt <= now);
        var attemptsToday = await db.PracticeAttempts.CountAsync(x => x.AttemptedAt >= todayStart);

        var avgAccuracyLast7Days = await db.PracticeAttempts
            .Where(x => x.AttemptedAt >= sevenDaysAgo)
            .Select(x => (double?)x.AccuracyPercent)
            .AverageAsync() ?? 0;

        var weakestAggregate = await db.PracticeAttempts
            .GroupBy(x => x.SnippetId)
            .Select(group => new
            {
                SnippetId = group.Key,
                AverageAccuracy = group.Average(x => x.AccuracyPercent),
                AttemptCount = group.Count()
            })
            .OrderBy(x => x.AverageAccuracy)
            .ThenByDescending(x => x.AttemptCount)
            .Take(5)
            .ToListAsync();

        var weakestSnippetIds = weakestAggregate.Select(x => x.SnippetId).ToList();

        var snippetLookup = await db.Snippets
            .AsNoTracking()
            .Include(x => x.Topic)
            .Where(x => weakestSnippetIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id);

        var weakestSnippets = weakestAggregate
            .Select(x =>
            {
                snippetLookup.TryGetValue(x.SnippetId, out var snippet);
                return new WeakSnippet
                {
                    SnippetId = x.SnippetId,
                    SnippetTitle = snippet?.Title ?? "Unknown",
                    TopicName = snippet?.Topic?.Name ?? "Unknown",
                    AverageAccuracy = Math.Round(x.AverageAccuracy, 2),
                    AttemptCount = x.AttemptCount
                };
            })
            .ToList();

        return new DashboardSnapshot
        {
            TotalSnippets = totalSnippets,
            SnippetsDueNow = snippetsDueNow,
            AttemptsToday = attemptsToday,
            AvgAccuracyLast7Days = Math.Round(avgAccuracyLast7Days, 2),
            WeakestSnippets = weakestSnippets
        };
    }
}

public sealed class DashboardSnapshot
{
    public int TotalSnippets { get; init; }

    public int SnippetsDueNow { get; init; }

    public int AttemptsToday { get; init; }

    public double AvgAccuracyLast7Days { get; init; }

    public List<WeakSnippet> WeakestSnippets { get; init; } = new();
}

public sealed class WeakSnippet
{
    public int SnippetId { get; init; }

    public string SnippetTitle { get; init; } = string.Empty;

    public string TopicName { get; init; } = string.Empty;

    public double AverageAccuracy { get; init; }

    public int AttemptCount { get; init; }
}