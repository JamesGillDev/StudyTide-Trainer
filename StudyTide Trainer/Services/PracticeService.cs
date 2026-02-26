using Microsoft.EntityFrameworkCore;
using StudyTideTrainer.Data;
using StudyTideTrainer.Models;

namespace StudyTideTrainer.Services;

public sealed class PracticeService(IDbContextFactory<TrainingDbContext> dbFactory)
{
    public async Task<Snippet?> GetNextSnippetAsync(int? topicId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var now = DateTime.UtcNow;

        var query = db.Snippets
            .AsNoTracking()
            .Include(x => x.Topic)
            .AsQueryable();

        if (topicId.HasValue)
        {
            query = query.Where(x => x.TopicId == topicId.Value);
        }

        var dueSnippet = await query
            .Where(x => !x.NextDueAt.HasValue || x.NextDueAt <= now)
            .OrderBy(x => x.NextDueAt ?? DateTime.MinValue)
            .ThenBy(x => x.LastPracticedAt ?? DateTime.MinValue)
            .ThenBy(x => x.Id)
            .FirstOrDefaultAsync();

        if (dueSnippet is not null)
        {
            return dueSnippet;
        }

        return await query
            .OrderBy(x => x.LastPracticedAt ?? DateTime.MinValue)
            .ThenBy(x => x.Id)
            .FirstOrDefaultAsync();
    }

    public PracticeEvaluation Evaluate(string sourceText, string typedText)
    {
        // Browser text areas and seeded snippets can use different newline styles.
        // Normalize both sides so scoring focuses on visible content differences.
        var normalizedSource = NormalizeLineEndings(sourceText);
        var normalizedTyped = NormalizeLineEndings(typedText);

        var minLength = Math.Min(normalizedSource.Length, normalizedTyped.Length);
        var mismatchCount = 0;
        int? firstMismatchIndex = null;

        for (var i = 0; i < minLength; i++)
        {
            if (normalizedSource[i] != normalizedTyped[i])
            {
                mismatchCount++;
                firstMismatchIndex ??= i;
            }
        }

        var missingChars = Math.Max(0, normalizedSource.Length - normalizedTyped.Length);
        var extraChars = Math.Max(0, normalizedTyped.Length - normalizedSource.Length);

        if (!firstMismatchIndex.HasValue && (missingChars > 0 || extraChars > 0))
        {
            firstMismatchIndex = minLength;
        }

        var errorCount = mismatchCount + missingChars + extraChars;

        double accuracyPercent;
        if (normalizedSource.Length == 0)
        {
            accuracyPercent = normalizedTyped.Length == 0 ? 100 : 0;
        }
        else
        {
            // Accuracy is based on source length because the goal is verbatim retyping.
            var rawAccuracy = ((double)(normalizedSource.Length - errorCount) / normalizedSource.Length) * 100;
            accuracyPercent = Math.Clamp(rawAccuracy, 0, 100);
        }

        var mismatchedLines = BuildLineMismatches(normalizedSource, normalizedTyped);

        return new PracticeEvaluation
        {
            AccuracyPercent = Math.Round(accuracyPercent, 2),
            ErrorCount = errorCount,
            MissingCharacters = missingChars,
            ExtraCharacters = extraChars,
            FirstMismatchIndex = firstMismatchIndex,
            MismatchedLines = mismatchedLines
        };
    }

    public async Task SaveAttemptAsync(int snippetId, string typedText, PracticeEvaluation evaluation)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var snippet = await db.Snippets.FirstOrDefaultAsync(x => x.Id == snippetId);

        if (snippet is null)
        {
            return;
        }

        var now = DateTime.UtcNow;

        snippet.TimesPracticed++;
        snippet.LastPracticedAt = now;

        // Simple spaced-repetition style scheduling rules requested for MVP.
        if (evaluation.AccuracyPercent == 100)
        {
            snippet.NextDueAt = now.AddDays(3);
            snippet.TimesPerfect++;
        }
        else if (evaluation.AccuracyPercent >= 95)
        {
            snippet.NextDueAt = now.AddDays(1);
        }
        else
        {
            snippet.NextDueAt = now.AddHours(1);
        }

        db.PracticeAttempts.Add(new PracticeAttempt
        {
            SnippetId = snippetId,
            AttemptedAt = now,
            TypedText = typedText,
            AccuracyPercent = evaluation.AccuracyPercent,
            ErrorCount = evaluation.ErrorCount,
            MissingChars = evaluation.MissingCharacters,
            ExtraChars = evaluation.ExtraCharacters,
            FirstMismatchIndex = evaluation.FirstMismatchIndex ?? -1
        });

        await db.SaveChangesAsync();
    }

    private static string NormalizeLineEndings(string text)
    {
        return text.Replace("\r\n", "\n").Replace("\r", "\n");
    }

    private static List<LineMismatch> BuildLineMismatches(string sourceText, string typedText)
    {
        var sourceLines = sourceText.Split('\n');
        var typedLines = typedText.Split('\n');
        var maxLineCount = Math.Max(sourceLines.Length, typedLines.Length);

        var mismatches = new List<LineMismatch>();

        for (var i = 0; i < maxLineCount; i++)
        {
            var sourceLine = i < sourceLines.Length ? sourceLines[i] : string.Empty;
            var typedLine = i < typedLines.Length ? typedLines[i] : string.Empty;

            if (!string.Equals(sourceLine, typedLine, StringComparison.Ordinal))
            {
                mismatches.Add(new LineMismatch
                {
                    LineNumber = i + 1,
                    SourceLine = sourceLine,
                    TypedLine = typedLine
                });
            }
        }

        return mismatches;
    }
}

public sealed class PracticeEvaluation
{
    public double AccuracyPercent { get; init; }

    public int ErrorCount { get; init; }

    public int MissingCharacters { get; init; }

    public int ExtraCharacters { get; init; }

    public int? FirstMismatchIndex { get; init; }

    public List<LineMismatch> MismatchedLines { get; init; } = new();
}

public sealed class LineMismatch
{
    public int LineNumber { get; init; }

    public string SourceLine { get; init; } = string.Empty;

    public string TypedLine { get; init; } = string.Empty;
}
