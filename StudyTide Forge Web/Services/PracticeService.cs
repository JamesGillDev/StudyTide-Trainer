using Microsoft.EntityFrameworkCore;
using StudyTideForge.Data;
using StudyTideForge.Models;

namespace StudyTideForge.Services;

public sealed class PracticeService(IDbContextFactory<ForgeDbContext> dbFactory)
{
    public async Task<TrainingBlock?> GetNextDueBlockAsync(int? moduleId, int? lessonId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var now = DateTime.UtcNow;

        var query = db.TrainingBlocks
            .AsNoTracking()
            .Include(x => x.Lesson)
            .ThenInclude(x => x!.Module)
            .AsQueryable();

        if (moduleId.HasValue)
        {
            query = query.Where(x => x.Lesson!.ModuleId == moduleId.Value);
        }

        if (lessonId.HasValue)
        {
            query = query.Where(x => x.LessonId == lessonId.Value);
        }

        var dueBlock = await query
            .Where(x => !x.NextDueAt.HasValue || x.NextDueAt <= now)
            .OrderBy(x => x.NextDueAt ?? DateTime.MinValue)
            .ThenBy(x => x.LastPracticedAt ?? DateTime.MinValue)
            .ThenBy(x => x.Id)
            .FirstOrDefaultAsync();

        if (dueBlock is not null)
        {
            return dueBlock;
        }

        return await query
            .OrderBy(x => x.LastPracticedAt ?? DateTime.MinValue)
            .ThenBy(x => x.Id)
            .FirstOrDefaultAsync();
    }

    public PracticeEvaluation Evaluate(string source, string typed)
    {
        var normalizedSource = NormalizeLineEndings(source);
        var normalizedTyped = NormalizeLineEndings(typed);

        var minLength = Math.Min(normalizedSource.Length, normalizedTyped.Length);
        var mismatchCount = 0;
        int? firstMismatch = null;

        for (var i = 0; i < minLength; i++)
        {
            if (normalizedSource[i] == normalizedTyped[i])
            {
                continue;
            }

            mismatchCount++;
            firstMismatch ??= i;
        }

        var missingChars = Math.Max(0, normalizedSource.Length - normalizedTyped.Length);
        var extraChars = Math.Max(0, normalizedTyped.Length - normalizedSource.Length);

        if (!firstMismatch.HasValue && (missingChars > 0 || extraChars > 0))
        {
            firstMismatch = minLength;
        }

        var errorCount = mismatchCount + missingChars + extraChars;
        var accuracy = normalizedSource.Length == 0
            ? normalizedTyped.Length == 0 ? 100 : 0
            : Math.Clamp(((double)(normalizedSource.Length - errorCount) / normalizedSource.Length) * 100, 0, 100);

        return new PracticeEvaluation
        {
            AccuracyPercent = Math.Round(accuracy, 2),
            ErrorCount = errorCount,
            FirstMismatchIndex = firstMismatch,
            MissingCharacters = missingChars,
            ExtraCharacters = extraChars,
            MismatchedLines = BuildLineDiff(normalizedSource, normalizedTyped)
        };
    }

    public async Task SaveAttemptAsync(int trainingBlockId, string typedText, PracticeEvaluation evaluation)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var block = await db.TrainingBlocks.FirstOrDefaultAsync(x => x.Id == trainingBlockId);

        if (block is null)
        {
            return;
        }

        var now = DateTime.UtcNow;

        block.TimesPracticed++;
        block.LastPracticedAt = now;

        if (evaluation.AccuracyPercent == 100)
        {
            block.TimesPerfect++;
            block.NextDueAt = now.AddDays(3);
        }
        else if (evaluation.AccuracyPercent >= 95)
        {
            block.NextDueAt = now.AddDays(1);
        }
        else
        {
            block.NextDueAt = now.AddHours(1);
        }

        db.PracticeAttempts.Add(new PracticeAttempt
        {
            TrainingBlockId = trainingBlockId,
            AttemptedAt = now,
            TypedText = typedText,
            AccuracyPercent = evaluation.AccuracyPercent,
            ErrorCount = evaluation.ErrorCount,
            FirstMismatchIndex = evaluation.FirstMismatchIndex ?? -1
        });

        await db.SaveChangesAsync();
    }

    private static string NormalizeLineEndings(string text)
    {
        return text.Replace("\r\n", "\n").Replace("\r", "\n");
    }

    private static IReadOnlyList<LineMismatch> BuildLineDiff(string source, string typed)
    {
        var sourceLines = source.Split('\n');
        var typedLines = typed.Split('\n');
        var maxLines = Math.Max(sourceLines.Length, typedLines.Length);

        var mismatches = new List<LineMismatch>();

        for (var index = 0; index < maxLines; index++)
        {
            var sourceLine = index < sourceLines.Length ? sourceLines[index] : string.Empty;
            var typedLine = index < typedLines.Length ? typedLines[index] : string.Empty;

            if (string.Equals(sourceLine, typedLine, StringComparison.Ordinal))
            {
                continue;
            }

            mismatches.Add(new LineMismatch
            {
                LineNumber = index + 1,
                SourceLine = sourceLine,
                TypedLine = typedLine
            });
        }

        return mismatches;
    }
}

public sealed class PracticeEvaluation
{
    public double AccuracyPercent { get; init; }

    public int ErrorCount { get; init; }

    public int? FirstMismatchIndex { get; init; }

    public int MissingCharacters { get; init; }

    public int ExtraCharacters { get; init; }

    public IReadOnlyList<LineMismatch> MismatchedLines { get; init; } = [];
}

public sealed class LineMismatch
{
    public int LineNumber { get; init; }

    public string SourceLine { get; init; } = string.Empty;

    public string TypedLine { get; init; } = string.Empty;
}
