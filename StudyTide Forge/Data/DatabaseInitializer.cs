using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using StudyTideForge.Models;

namespace StudyTideForge.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ForgeDbContext>>();
        await using var db = await dbFactory.CreateDbContextAsync();

        await db.Database.MigrateAsync();

        var importer = new LegacyQaSourceImporter();
        var imported = importer.Import();

        if (imported.Pairs.Count < 200)
        {
            throw new InvalidOperationException($"Legacy source import returned {imported.Pairs.Count} Q/A pairs, which is below the expected minimum.");
        }

        if (!await RequiresReseedAsync(db, imported.Pairs.Count))
        {
            return;
        }

        await ReseedAsync(db, imported.Pairs);
    }

    private static async Task<bool> RequiresReseedAsync(ForgeDbContext db, int importedPairCount)
    {
        var minimumExpected = Math.Min(importedPairCount, 200);
        var moduleNames = LegacyImportConstants.ModuleDefinitions
            .Select(x => $"{LegacyImportConstants.ModuleNamePrefix}{x.Name}")
            .ToList();

        var modules = await db.TrainingModules
            .AsNoTracking()
            .Include(x => x.Lessons)
            .ThenInclude(x => x.TrainingBlocks)
            .ToListAsync();

        if (modules.Count == 0)
        {
            return true;
        }

        var flashcardCount = await db.Flashcards.CountAsync();
        var itemCount = await db.TrainingBlocks.CountAsync();

        if (flashcardCount < minimumExpected || itemCount < minimumExpected)
        {
            return true;
        }

        if (flashcardCount != itemCount)
        {
            return true;
        }

        if (modules.Any(x => x.Lessons.Count == 0 || x.Lessons.SelectMany(lesson => lesson.TrainingBlocks).Count() == 0))
        {
            return true;
        }

        var moduleNameSet = modules
            .Select(x => x.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return moduleNames.Any(name => !moduleNameSet.Contains(name));
    }

    private static async Task ReseedAsync(ForgeDbContext db, IReadOnlyList<ImportedQaPair> pairs)
    {
        var now = DateTime.UtcNow;
        var existingModules = await db.TrainingModules.ToListAsync();

        if (existingModules.Count > 0)
        {
            db.TrainingModules.RemoveRange(existingModules);
            await db.SaveChangesAsync();
        }

        var groupedPairs = BuildCategoryGroups(pairs);
        EnsureNoEmptyGroups(groupedPairs);

        var modulesByCategory = new Dictionary<string, TrainingModule>(StringComparer.OrdinalIgnoreCase);

        foreach (var definition in LegacyImportConstants.ModuleDefinitions)
        {
            var module = new TrainingModule
            {
                Name = $"{LegacyImportConstants.ModuleNamePrefix}{definition.Name}",
                Category = definition.Category,
                CreatedAt = now
            };

            db.TrainingModules.Add(module);
            modulesByCategory[definition.Category] = module;
        }

        await db.SaveChangesAsync();

        foreach (var definition in LegacyImportConstants.ModuleDefinitions)
        {
            var module = modulesByCategory[definition.Category];
            var modulePairs = groupedPairs[definition.Category];

            var lessonNumber = 1;
            foreach (var chunk in Chunk(modulePairs, LegacyImportConstants.LessonChunkSize))
            {
                var lesson = new TrainingLesson
                {
                    ModuleId = module.Id,
                    Title = $"Imported Set {lessonNumber}",
                    OrderIndex = lessonNumber,
                    CreatedAt = now
                };

                foreach (var pair in chunk)
                {
                    var prompt = pair.Answer.Trim();
                    var response = pair.Question.Trim();
                    var difficulty = ComputeDifficulty(prompt, response);

                    lesson.Flashcards.Add(new Flashcard
                    {
                        Question = prompt,
                        Answer = response,
                        Difficulty = difficulty,
                        TimesCorrect = 0,
                        TimesIncorrect = 0
                    });

                    lesson.TrainingBlocks.Add(new TrainingBlock
                    {
                        Title = BuildTrainingItemTitle(prompt),
                        Content = BuildTrainingContent(prompt, response),
                        Difficulty = difficulty,
                        TimesPracticed = 0,
                        TimesPerfect = 0,
                        LastPracticedAt = null,
                        NextDueAt = now
                    });
                }

                db.TrainingLessons.Add(lesson);
                lessonNumber++;
            }
        }

        await db.SaveChangesAsync();
    }

    private static Dictionary<string, List<ImportedQaPair>> BuildCategoryGroups(IReadOnlyList<ImportedQaPair> pairs)
    {
        var groups = LegacyImportConstants.ModuleDefinitions
            .ToDictionary(
                x => x.Category,
                _ => new List<ImportedQaPair>(),
                StringComparer.OrdinalIgnoreCase);

        foreach (var pair in pairs)
        {
            var category = InferCategory(pair);
            groups[category].Add(pair);
        }

        return groups;
    }

    private static string InferCategory(ImportedQaPair pair)
    {
        var sample = $"{pair.Question} {pair.Answer}".ToLowerInvariant();

        if (ContainsAny(sample, "azure", "entra", "key vault", "app service", "resource group", "subscription", "vnet"))
        {
            return "Azure";
        }

        if (ContainsAny(sample, "sql", "database", "query", "table", "index", "join", "ef core", "entity framework", "cosmos"))
        {
            return "SQL";
        }

        if (ContainsAny(sample, "devops", "ci/cd", "pipeline", "docker", "kubernetes", "git", "deployment", "automation"))
        {
            return "DevOps";
        }

        if (ContainsAny(sample, "architecture", "system design", "distributed", "scalability", "load balancer", "microservices", "resilience"))
        {
            return "System Design";
        }

        if (ContainsAny(sample, "scrum", "agile", "retrospective", "team", "stakeholder", "communication", "behavior", "leadership"))
        {
            return "Behavioral";
        }

        if (ContainsAny(sample, "c#", ".net", "asp.net", "blazor", "linq", "class", "interface", "delegate", "async"))
        {
            return "C#";
        }

        var hash = Math.Abs(pair.Question.GetHashCode(StringComparison.Ordinal));
        var fallback = LegacyImportConstants.ModuleDefinitions[hash % LegacyImportConstants.ModuleDefinitions.Count];
        return fallback.Category;
    }

    private static bool ContainsAny(string value, params string[] needles)
    {
        return needles.Any(value.Contains);
    }

    private static void EnsureNoEmptyGroups(Dictionary<string, List<ImportedQaPair>> groups)
    {
        foreach (var emptyKey in groups.Where(x => x.Value.Count == 0).Select(x => x.Key).ToList())
        {
            var donor = groups
                .Where(x => x.Value.Count > 1)
                .OrderByDescending(x => x.Value.Count)
                .FirstOrDefault();

            if (donor.Value is null || donor.Value.Count == 0)
            {
                break;
            }

            var movedPair = donor.Value[^1];
            donor.Value.RemoveAt(donor.Value.Count - 1);
            groups[emptyKey].Add(movedPair);
        }
    }

    private static IEnumerable<IReadOnlyList<ImportedQaPair>> Chunk(IReadOnlyList<ImportedQaPair> pairs, int chunkSize)
    {
        for (var offset = 0; offset < pairs.Count; offset += chunkSize)
        {
            var size = Math.Min(chunkSize, pairs.Count - offset);
            var chunk = new List<ImportedQaPair>(size);

            for (var index = 0; index < size; index++)
            {
                chunk.Add(pairs[offset + index]);
            }

            yield return chunk;
        }
    }

    private static int ComputeDifficulty(string prompt, string response)
    {
        var length = prompt.Length + response.Length;

        if (length <= 60)
        {
            return 1;
        }

        if (length <= 120)
        {
            return 2;
        }

        if (length <= 200)
        {
            return 3;
        }

        if (length <= 320)
        {
            return 4;
        }

        return 5;
    }

    private static string BuildTrainingItemTitle(string prompt)
    {
        var normalized = Regex.Replace(prompt, @"\s+", " ").Trim();

        if (normalized.Length <= 60)
        {
            return normalized;
        }

        return $"{normalized[..57].TrimEnd()}...";
    }

    private static string BuildTrainingContent(string prompt, string response)
    {
        var example = BuildExample(prompt, response);
        return $"Prompt:\n{prompt}\n\nResponse:\n{response}\n\nExample:\n{example}";
    }

    private static string BuildExample(string prompt, string response)
    {
        var normalizedPrompt = NormalizeForSentence(prompt, 180);
        var normalizedResponse = NormalizeForSentence(response, 180);
        return $"When practicing, map \"{normalizedPrompt}\" to \"{normalizedResponse}\" and restate it from memory.";
    }

    private static string NormalizeForSentence(string value, int maxLength)
    {
        var normalized = Regex.Replace(value, @"\s+", " ").Trim();

        if (normalized.Length <= maxLength)
        {
            return normalized;
        }

        return $"{normalized[..(maxLength - 3)].TrimEnd()}...";
    }
}
