using Microsoft.EntityFrameworkCore;
using StudyTideForge.Data;
using StudyTideForge.Models;

namespace StudyTideForge.Services;

public sealed class FlashcardService(IDbContextFactory<ForgeDbContext> dbFactory)
{
    public async Task<Flashcard?> GetRandomCardAsync(
        int? moduleId,
        int? lessonId,
        bool dueOnly,
        IReadOnlyCollection<int>? excludedCardIds = null)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var query = db.Flashcards
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

        if (excludedCardIds is { Count: > 0 })
        {
            query = query.Where(x => !excludedCardIds.Contains(x.Id));
        }

        if (dueOnly)
        {
            query = query.Where(x => (x.TimesCorrect + x.TimesIncorrect) == 0 || x.TimesIncorrect >= x.TimesCorrect);
        }

        var candidateIds = await query.Select(x => x.Id).ToListAsync();

        if (candidateIds.Count == 0 && dueOnly)
        {
            return await GetRandomCardAsync(moduleId, lessonId, dueOnly: false, excludedCardIds);
        }

        if (candidateIds.Count == 0)
        {
            return null;
        }

        var chosenId = candidateIds[Random.Shared.Next(candidateIds.Count)];

        return await db.Flashcards
            .AsNoTracking()
            .Include(x => x.Lesson)
            .ThenInclude(x => x!.Module)
            .FirstOrDefaultAsync(x => x.Id == chosenId);
    }

    public async Task SaveResultAsync(int flashcardId, bool isCorrect)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var flashcard = await db.Flashcards.FirstOrDefaultAsync(x => x.Id == flashcardId);

        if (flashcard is null)
        {
            return;
        }

        if (isCorrect)
        {
            flashcard.TimesCorrect++;
        }
        else
        {
            flashcard.TimesIncorrect++;
        }

        await db.SaveChangesAsync();
    }
}
