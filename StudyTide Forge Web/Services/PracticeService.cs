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

        var firstMismatch = FindFirstMismatchIndex(normalizedSource, normalizedTyped);
        var editMetrics = ComputeEditMetrics(normalizedSource, normalizedTyped);

        return new PracticeEvaluation
        {
            AccuracyPercent = Math.Round(editMetrics.AccuracyPercent, 2),
            ErrorCount = editMetrics.ErrorCount,
            FirstMismatchIndex = firstMismatch,
            MissingCharacters = editMetrics.MissingCharacters,
            ExtraCharacters = editMetrics.ExtraCharacters,
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

    private static int? FindFirstMismatchIndex(string source, string typed)
    {
        var minLength = Math.Min(source.Length, typed.Length);

        for (var index = 0; index < minLength; index++)
        {
            if (source[index] != typed[index])
            {
                return index;
            }
        }

        if (source.Length != typed.Length)
        {
            return minLength;
        }

        return null;
    }

    private static EditMetrics ComputeEditMetrics(string source, string typed)
    {
        var sourceLength = source.Length;
        var typedLength = typed.Length;

        if (sourceLength == 0)
        {
            var initialErrorCount = typedLength;
            return new EditMetrics(initialErrorCount, 0, typedLength, typedLength == 0 ? 100 : 0);
        }

        var distances = new int[sourceLength + 1, typedLength + 1];

        for (var i = 0; i <= sourceLength; i++)
        {
            distances[i, 0] = i;
        }

        for (var j = 0; j <= typedLength; j++)
        {
            distances[0, j] = j;
        }

        for (var i = 1; i <= sourceLength; i++)
        {
            for (var j = 1; j <= typedLength; j++)
            {
                if (source[i - 1] == typed[j - 1])
                {
                    distances[i, j] = distances[i - 1, j - 1];
                    continue;
                }

                var substitution = distances[i - 1, j - 1] + 1;
                var deletion = distances[i - 1, j] + 1;
                var insertion = distances[i, j - 1] + 1;

                distances[i, j] = Math.Min(substitution, Math.Min(deletion, insertion));
            }
        }

        var missingCharacters = 0;
        var extraCharacters = 0;
        var sourceIndex = sourceLength;
        var typedIndex = typedLength;

        while (sourceIndex > 0 || typedIndex > 0)
        {
            if (sourceIndex > 0 &&
                typedIndex > 0 &&
                source[sourceIndex - 1] == typed[typedIndex - 1] &&
                distances[sourceIndex, typedIndex] == distances[sourceIndex - 1, typedIndex - 1])
            {
                sourceIndex--;
                typedIndex--;
                continue;
            }

            // Prefer substitution when tied so a replaced character counts as one error.
            if (sourceIndex > 0 &&
                typedIndex > 0 &&
                distances[sourceIndex, typedIndex] == distances[sourceIndex - 1, typedIndex - 1] + 1)
            {
                sourceIndex--;
                typedIndex--;
                continue;
            }

            if (sourceIndex > 0 &&
                distances[sourceIndex, typedIndex] == distances[sourceIndex - 1, typedIndex] + 1)
            {
                missingCharacters++;
                sourceIndex--;
                continue;
            }

            if (typedIndex > 0 &&
                distances[sourceIndex, typedIndex] == distances[sourceIndex, typedIndex - 1] + 1)
            {
                extraCharacters++;
                typedIndex--;
                continue;
            }

            // Safety fallback for unexpected ties.
            if (sourceIndex > 0 && typedIndex > 0)
            {
                sourceIndex--;
                typedIndex--;
                continue;
            }

            if (sourceIndex > 0)
            {
                missingCharacters += sourceIndex;
                break;
            }

            if (typedIndex > 0)
            {
                extraCharacters += typedIndex;
                break;
            }
        }

        var errorCount = distances[sourceLength, typedLength];
        var rawAccuracy = ((double)(sourceLength - errorCount) / sourceLength) * 100;
        var accuracy = Math.Clamp(rawAccuracy, 0, 100);
        return new EditMetrics(errorCount, missingCharacters, extraCharacters, accuracy);
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

    private readonly record struct EditMetrics(
        int ErrorCount,
        int MissingCharacters,
        int ExtraCharacters,
        double AccuracyPercent);
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
