using Microsoft.EntityFrameworkCore;
using StudyTideForge.Data;
using StudyTideForge.Models;

namespace StudyTideForge.Services;

public sealed class StudyLibraryService(IDbContextFactory<ForgeDbContext> dbFactory)
{
    public async Task<StudyLibrarySnapshot> GetLibrarySnapshotAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var lessonRows = await db.TrainingLessons
            .AsNoTracking()
            .Select(x => new LessonRow
            {
                LessonId = x.Id,
                LessonTitle = x.Title,
                ModuleId = x.ModuleId,
                ModuleName = x.Module!.Name,
                ModuleCategory = x.Module!.Category,
                OrderIndex = x.OrderIndex,
                IsFlagged = x.IsFlagged,
                BlockCount = x.TrainingBlocks.Count,
                FlashcardCount = x.Flashcards.Count
            })
            .OrderBy(x => x.ModuleCategory)
            .ThenBy(x => x.ModuleName)
            .ThenBy(x => x.OrderIndex)
            .ToListAsync();

        var progressRows = await db.StudyLessonProgresses
            .AsNoTracking()
            .ToDictionaryAsync(x => x.LessonId);

        var modules = lessonRows
            .GroupBy(x => new { x.ModuleId, x.ModuleName, x.ModuleCategory })
            .Select(group =>
            {
                var lessonCards = group
                    .Select(lesson => BuildLessonCard(lesson, progressRows.GetValueOrDefault(lesson.LessonId)))
                    .ToList();

                var status = CalculateModuleStatus(lessonCards);
                var resumeTarget = BuildModuleResumeTarget(
                    group.Key.ModuleId,
                    group.Key.ModuleName,
                    lessonCards);

                return new StudyModuleCard
                {
                    ModuleId = group.Key.ModuleId,
                    Name = group.Key.ModuleName,
                    Category = group.Key.ModuleCategory,
                    LessonCount = lessonCards.Count,
                    TrainingItemCount = lessonCards.Sum(x => x.BlockCount),
                    FlashcardCount = lessonCards.Sum(x => x.FlashcardCount),
                    Status = status,
                    ResumeTarget = resumeTarget,
                    Lessons = lessonCards
                };
            })
            .OrderBy(x => x.Category)
            .ThenBy(x => x.Name)
            .ToList();

        var lastResumeTarget = modules
            .Select(x => x.ResumeTarget)
            .Where(x => x is not null)
            .Cast<StudyResumeTarget>()
            .OrderByDescending(x => x.LastViewedAt)
            .FirstOrDefault();

        return new StudyLibrarySnapshot
        {
            Modules = modules,
            LastResumeTarget = lastResumeTarget
        };
    }

    public async Task<LessonStudySession?> GetLessonStudySessionAsync(int moduleId, int lessonId, bool resume)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var lesson = await db.TrainingLessons
            .AsNoTracking()
            .Include(x => x.Module)
            .FirstOrDefaultAsync(x => x.Id == lessonId && x.ModuleId == moduleId);

        if (lesson is null)
        {
            return null;
        }

        var blocks = await db.TrainingBlocks
            .AsNoTracking()
            .Where(x => x.LessonId == lessonId)
            .OrderBy(x => x.Id)
            .ToListAsync();

        var progress = await db.StudyLessonProgresses
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.LessonId == lessonId);

        var blockViews = blocks
            .Select((block, index) => BuildBlockView(block, index))
            .ToList();

        var startIndex = 0;
        if (resume && progress is not null && blockViews.Count > 0)
        {
            startIndex = Math.Clamp(progress.CurrentBlockIndex, 0, blockViews.Count - 1);
        }

        var highestVisitedIndex = progress is null
            ? -1
            : Math.Clamp(progress.HighestBlockIndex, -1, Math.Max(-1, blockViews.Count - 1));

        return new LessonStudySession
        {
            ModuleId = moduleId,
            ModuleName = lesson.Module?.Name ?? string.Empty,
            LessonId = lesson.Id,
            LessonTitle = lesson.Title,
            IsFlagged = lesson.IsFlagged,
            Blocks = blockViews,
            StartIndex = startIndex,
            HighestVisitedIndex = highestVisitedIndex,
            IsCompleted = progress?.IsCompleted ?? false,
            LastViewedAt = progress?.LastViewedAt
        };
    }

    public async Task SaveCheckpointAsync(int lessonId, int? trainingBlockId, int blockIndex, int totalBlocks)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var progress = await db.StudyLessonProgresses
            .FirstOrDefaultAsync(x => x.LessonId == lessonId);

        if (progress is null)
        {
            progress = new StudyLessonProgress
            {
                LessonId = lessonId,
                HighestBlockIndex = -1
            };

            db.StudyLessonProgresses.Add(progress);
        }

        var normalizedIndex = Math.Max(0, blockIndex);
        progress.CurrentTrainingBlockId = trainingBlockId;
        progress.CurrentBlockIndex = normalizedIndex;
        progress.HighestBlockIndex = Math.Max(progress.HighestBlockIndex, normalizedIndex);
        progress.IsCompleted = totalBlocks == 0 || progress.HighestBlockIndex >= totalBlocks - 1;
        progress.LastViewedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
    }

    public async Task<bool> ResetLessonProgressAsync(int lessonId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var progress = await db.StudyLessonProgresses
            .FirstOrDefaultAsync(x => x.LessonId == lessonId);

        if (progress is null)
        {
            return false;
        }

        db.StudyLessonProgresses.Remove(progress);
        await db.SaveChangesAsync();
        return true;
    }

    private static StudyLessonCard BuildLessonCard(LessonRow row, StudyLessonProgress? progress)
    {
        var status = progress switch
        {
            null => StudyProgressStatus.NotStarted,
            _ when progress.IsCompleted => StudyProgressStatus.Completed,
            _ when row.BlockCount > 0 && progress.HighestBlockIndex >= row.BlockCount - 1 => StudyProgressStatus.Completed,
            _ => StudyProgressStatus.InProgress
        };

        var highestVisited = progress?.HighestBlockIndex ?? -1;
        var viewedItems = row.BlockCount == 0
            ? 0
            : Math.Clamp(highestVisited + 1, 0, row.BlockCount);
        var completionPercent = row.BlockCount == 0
            ? 100
            : Math.Round((double)viewedItems / row.BlockCount * 100, 2);

        return new StudyLessonCard
        {
            LessonId = row.LessonId,
            LessonTitle = row.LessonTitle,
            OrderIndex = row.OrderIndex,
            BlockCount = row.BlockCount,
            FlashcardCount = row.FlashcardCount,
            IsFlagged = row.IsFlagged,
            Status = status,
            CompletionPercent = completionPercent,
            ResumeBlockIndex = progress?.CurrentBlockIndex,
            HighestVisitedIndex = progress?.HighestBlockIndex ?? -1,
            LastViewedAt = progress?.LastViewedAt
        };
    }

    private static StudyProgressStatus CalculateModuleStatus(IReadOnlyList<StudyLessonCard> lessons)
    {
        if (lessons.Count == 0)
        {
            return StudyProgressStatus.NotStarted;
        }

        if (lessons.All(x => x.Status == StudyProgressStatus.Completed))
        {
            return StudyProgressStatus.Completed;
        }

        if (lessons.Any(x => x.Status != StudyProgressStatus.NotStarted))
        {
            return StudyProgressStatus.InProgress;
        }

        return StudyProgressStatus.NotStarted;
    }

    private static StudyResumeTarget? BuildModuleResumeTarget(
        int moduleId,
        string moduleName,
        IReadOnlyList<StudyLessonCard> lessons)
    {
        var progressedLesson = lessons
            .Where(x => x.LastViewedAt.HasValue)
            .OrderByDescending(x => x.LastViewedAt)
            .FirstOrDefault();

        if (progressedLesson is not null)
        {
            return new StudyResumeTarget
            {
                ModuleId = moduleId,
                ModuleName = moduleName,
                LessonId = progressedLesson.LessonId,
                LessonTitle = progressedLesson.LessonTitle,
                BlockIndex = progressedLesson.ResumeBlockIndex ?? 0,
                LastViewedAt = progressedLesson.LastViewedAt!.Value
            };
        }

        var firstLesson = lessons
            .OrderBy(x => x.OrderIndex)
            .FirstOrDefault();

        if (firstLesson is null)
        {
            return null;
        }

        return new StudyResumeTarget
        {
            ModuleId = moduleId,
            ModuleName = moduleName,
            LessonId = firstLesson.LessonId,
            LessonTitle = firstLesson.LessonTitle,
            BlockIndex = 0,
            LastViewedAt = DateTime.MinValue
        };
    }

    private static StudyBlockView BuildBlockView(TrainingBlock block, int index)
    {
        var normalizedContent = TrainingContentFormatter.NormalizeLineEndings(block.Content).Trim();
        string prompt;
        string response;
        string example;

        if (TrainingContentFormatter.TryParseLabeledSections(normalizedContent, out var sections))
        {
            var oriented = TrainingContentFormatter.ReversePromptResponse(sections);
            prompt = oriented.Prompt;
            response = oriented.Response;
            example = oriented.Example;
        }
        else
        {
            prompt = block.Title;
            response = normalizedContent;
            example = string.Empty;
        }

        return new StudyBlockView
        {
            BlockId = block.Id,
            OrderIndex = index,
            Title = block.Title,
            Prompt = prompt,
            Response = response,
            Example = example,
            ExpandedExample = BuildExpandedExample(prompt, response, example),
            Difficulty = block.Difficulty
        };
    }

    private static IReadOnlyList<string> BuildExpandedExample(string prompt, string response, string example)
    {
        var normalizedPrompt = prompt.Trim();
        var normalizedResponse = response.Trim();
        var normalizedExample = example.Trim();

        var points = new List<string>();

        if (!string.IsNullOrWhiteSpace(normalizedResponse))
        {
            points.Add($"Core concept: {normalizedResponse}");
        }

        if (!string.IsNullOrWhiteSpace(normalizedPrompt))
        {
            points.Add($"Recognition cue: {normalizedPrompt}");
        }

        if (!string.IsNullOrWhiteSpace(normalizedExample))
        {
            points.Add($"Study cue: {normalizedExample}");
        }
        else if (!string.IsNullOrWhiteSpace(normalizedResponse))
        {
            points.Add($"Study cue: explain {normalizedResponse} in your own words and connect it to a real implementation.");
        }

        return points;
    }

    private sealed class LessonRow
    {
        public int LessonId { get; init; }

        public string LessonTitle { get; init; } = string.Empty;

        public int ModuleId { get; init; }

        public string ModuleName { get; init; } = string.Empty;

        public string ModuleCategory { get; init; } = string.Empty;

        public int OrderIndex { get; init; }

        public bool IsFlagged { get; init; }

        public int BlockCount { get; init; }

        public int FlashcardCount { get; init; }
    }
}

public enum StudyProgressStatus
{
    NotStarted,
    InProgress,
    Completed
}

public sealed class StudyLibrarySnapshot
{
    public IReadOnlyList<StudyModuleCard> Modules { get; init; } = [];

    public StudyResumeTarget? LastResumeTarget { get; init; }
}

public sealed class StudyModuleCard
{
    public int ModuleId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public int LessonCount { get; init; }

    public int TrainingItemCount { get; init; }

    public int FlashcardCount { get; init; }

    public StudyProgressStatus Status { get; init; }

    public StudyResumeTarget? ResumeTarget { get; init; }

    public IReadOnlyList<StudyLessonCard> Lessons { get; init; } = [];
}

public sealed class StudyLessonCard
{
    public int LessonId { get; init; }

    public string LessonTitle { get; init; } = string.Empty;

    public int OrderIndex { get; init; }

    public int BlockCount { get; init; }

    public int FlashcardCount { get; init; }

    public bool IsFlagged { get; init; }

    public StudyProgressStatus Status { get; init; }

    public double CompletionPercent { get; init; }

    public int? ResumeBlockIndex { get; init; }

    public int HighestVisitedIndex { get; init; }

    public DateTime? LastViewedAt { get; init; }
}

public sealed class StudyResumeTarget
{
    public int ModuleId { get; init; }

    public string ModuleName { get; init; } = string.Empty;

    public int LessonId { get; init; }

    public string LessonTitle { get; init; } = string.Empty;

    public int BlockIndex { get; init; }

    public DateTime LastViewedAt { get; init; }
}

public sealed class LessonStudySession
{
    public int ModuleId { get; init; }

    public string ModuleName { get; init; } = string.Empty;

    public int LessonId { get; init; }

    public string LessonTitle { get; init; } = string.Empty;

    public bool IsFlagged { get; init; }

    public IReadOnlyList<StudyBlockView> Blocks { get; init; } = [];

    public int StartIndex { get; init; }

    public int HighestVisitedIndex { get; init; }

    public bool IsCompleted { get; init; }

    public DateTime? LastViewedAt { get; init; }
}

public sealed class StudyBlockView
{
    public int BlockId { get; init; }

    public int OrderIndex { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Prompt { get; init; } = string.Empty;

    public string Response { get; init; } = string.Empty;

    public string Example { get; init; } = string.Empty;

    public IReadOnlyList<string> ExpandedExample { get; init; } = [];

    public int Difficulty { get; init; }
}
