using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using StudyTideForge.Models;
using StudyTideForge.Services;

namespace StudyTideForge.Data;

public static class DatabaseInitializer
{
    private static readonly IReadOnlyList<SeedBlock> SupplementalSeedBlocks =
    [
        new(
            ModuleCategory: "C#",
            LessonTitle: "Imported Set 99",
            Title: "private",
            Response: "Use private when a member should be accessible only inside the class where it is declared. It keeps implementation details hidden from outside callers.",
            Example: "Think of private as a secret room key: only the class holding the key can open that room.",
            Difficulty: 1),
        new(
            ModuleCategory: "C#",
            LessonTitle: "Imported Set 99",
            Title: "static",
            Response: "Use static for members that belong to the type itself instead of a specific object instance.",
            Example: "A static member is like a shared remote control: you can use it without first creating a toy instance.",
            Difficulty: 1),
        new(
            ModuleCategory: "C#",
            LessonTitle: "Imported Set 99",
            Title: "void",
            Response: "Use void as the return type when a method performs an action but does not return a value.",
            Example: "A void method is like completing a chore: work gets done, but no value is handed back.",
            Difficulty: 1),
        new(
            ModuleCategory: "C#",
            LessonTitle: "Imported Set 99",
            Title: "C# numeric types by bit size",
            Response: "byte/8 unsigned, sbyte/8 signed, short/16 signed, ushort/16 unsigned, int/32 signed, uint/32 unsigned, long/64 signed, ulong/64 unsigned, float/32 signed, double/64 signed, decimal/128 signed.",
            Example: "Pick numeric types by size and sign needs; use decimal for high-precision base-10 values such as currency.",
            Difficulty: 2)
    ];

    private static readonly IReadOnlyDictionary<string, int> CategoryCoverageTargets = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        ["C#"] = 450,
        ["System Design"] = 300
    };

    private static readonly IReadOnlyList<string> CSharpConcepts =
    [
        "Type system and value vs reference behavior",
        "Nullability and null-safety",
        "Access modifiers and encapsulation",
        "Class design and cohesion",
        "Struct usage and memory considerations",
        "Record types and immutability",
        "Interfaces and abstraction boundaries",
        "Inheritance and composition trade-offs",
        "Pattern matching with switch expressions",
        "Generic types and constraints",
        "Collections and collection interfaces",
        "LINQ query and method syntax",
        "Deferred execution in LINQ",
        "Async and await control flow",
        "Task coordination and cancellation",
        "IAsyncEnumerable streaming",
        "Exception handling strategy",
        "Custom exception design",
        "IDisposable and resource cleanup",
        "Dependency injection lifetimes",
        "Configuration and options pattern",
        "Logging strategy and structured logs",
        "Middleware pipeline behavior",
        "Minimal API endpoint design",
        "Blazor component state management",
        "HTTP client lifetime management",
        "Serialization with System.Text.Json",
        "Date and time correctness",
        "String handling and performance",
        "Span and memory slicing",
        "File and stream I/O safety",
        "Thread-safety primitives",
        "Parallelism and throughput tuning",
        "Events and delegates",
        "Lambda expressions and closures",
        "Extension methods",
        "Attributes and metadata",
        "Reflection trade-offs",
        "Source generators and analyzers",
        "Unit testing patterns",
        "Integration testing patterns",
        "Mocking and fakes strategy",
        "SOLID design principles",
        "Domain modeling and invariants",
        "Validation strategy",
        "EF Core tracking behavior",
        "EF Core query optimization",
        "EF Core migrations workflow",
        "Transactions and consistency",
        "Caching patterns in .NET",
        "API versioning strategy",
        "Authentication and authorization",
        "Secure secret handling",
        "Resilience with retries/timeouts",
        "Observability instrumentation",
        "Background jobs and hosted services",
        "Performance profiling and benchmarks",
        "Code review and maintainability",
        "Build and release automation",
        "Package/version management"
    ];

    private static readonly IReadOnlyList<string> SystemDesignConcepts =
    [
        "Service boundaries and bounded contexts",
        "Load balancing strategies",
        "Horizontal vs vertical scaling",
        "Caching layers and invalidation",
        "Content delivery network usage",
        "Database indexing strategy",
        "Read replicas and read/write split",
        "Partitioning and sharding",
        "Message queue decoupling",
        "Event-driven architecture",
        "Idempotency in distributed systems",
        "Retry and backoff policies",
        "Circuit breaker behavior",
        "Bulkhead isolation",
        "Rate limiting and throttling",
        "API gateway responsibilities",
        "Service discovery patterns",
        "Distributed tracing design",
        "Metrics and alerting strategy",
        "Logging architecture",
        "SLO/SLA planning",
        "Data consistency models",
        "CAP theorem trade-offs",
        "Consensus and leader election",
        "CQRS command/query separation",
        "Event sourcing trade-offs",
        "Blue-green deployment",
        "Canary rollout strategy",
        "Feature flag governance",
        "Disaster recovery planning",
        "Multi-region failover",
        "Data retention and lifecycle",
        "Security boundaries and zero trust",
        "Tenant isolation in multi-tenant apps",
        "API contract evolution",
        "Throughput and latency budgeting",
        "Capacity planning",
        "Backpressure management",
        "Workflow orchestration",
        "Observability-driven incident response"
    ];

    public static async Task InitializeAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ForgeDbContext>>();
        await using var db = await dbFactory.CreateDbContextAsync();

        await db.Database.MigrateAsync();

        var importer = new LegacyQaSourceImporter();
        ImportedQaResult imported;

        try
        {
            imported = importer.Import();
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"[StudyTide] Source import failed. Startup will continue with existing database content. {exception.Message}");
            imported = new ImportedQaResult("Unavailable", []);
        }

        await MigrateModuleNamePrefixAsync(db);

        if (imported.Pairs.Count < 200)
        {
            Console.Error.WriteLine($"[StudyTide] Source import returned {imported.Pairs.Count} Q/A pairs. Skipping base reseed and continuing with supplemental content.");
        }
        else if (await RequiresReseedAsync(db, imported.Pairs.Count))
        {
            await ReseedAsync(db, imported.Pairs);
        }

        await ApplySupplementalSeedBlocksAsync(db);
        await ReplaceDuplicateTrainingBlocksAsync(db);
        await ApplyTargetedCoverageBoostAsync(db);
        await ApplyPromptResponseOrientationMigrationAsync(db);
        await ReplaceDuplicateTrainingBlocksAsync(db);
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
        var sections = new ParsedTrainingContent(prompt, response, example);
        var normalized = TrainingContentFormatter.ReversePromptResponse(sections);
        return TrainingContentFormatter.BuildLabeledContent(normalized.Prompt, normalized.Response, normalized.Example);
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

    private static async Task MigrateModuleNamePrefixAsync(ForgeDbContext db)
    {
        var oldPrefix = LegacyImportConstants.PreviousModuleNamePrefix;
        var newPrefix = LegacyImportConstants.ModuleNamePrefix;

        if (string.Equals(oldPrefix, newPrefix, StringComparison.Ordinal))
        {
            return;
        }

        var modulesToRename = await db.TrainingModules
            .Where(x => x.Name.StartsWith(oldPrefix))
            .ToListAsync();

        if (modulesToRename.Count == 0)
        {
            return;
        }

        foreach (var module in modulesToRename)
        {
            module.Name = $"{newPrefix}{module.Name[oldPrefix.Length..]}";
        }

        await db.SaveChangesAsync();
    }

    private static async Task ApplySupplementalSeedBlocksAsync(ForgeDbContext db)
    {
        await ApplySeedBlocksAsync(db, SupplementalSeedBlocks);
    }

    private static async Task ApplyTargetedCoverageBoostAsync(ForgeDbContext db)
    {
        foreach (var target in CategoryCoverageTargets)
        {
            var category = target.Key;
            var currentCount = await db.TrainingBlocks
                .CountAsync(x => x.Lesson!.Module!.Category == category);
            var missingCount = target.Value - currentCount;

            if (missingCount <= 0)
            {
                continue;
            }

            var lessonTitle = $"Coverage Boost - {category}";
            var lesson = await EnsureLessonAsync(db, category, lessonTitle);
            var existingTitles = (await db.TrainingBlocks
                .Where(x => x.LessonId == lesson.Id)
                .Select(x => x.Title)
                .ToListAsync())
                .ToHashSet(StringComparer.Ordinal);
            var usedContentKeys = (await db.TrainingBlocks
                .Select(x => x.Content)
                .ToListAsync())
                .Select(NormalizeDuplicateKey)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToHashSet(StringComparer.Ordinal);
            var generated = GenerateUniqueSeedBlocks(
                category,
                lessonTitle,
                missingCount,
                existingTitles,
                usedContentKeys,
                "Coverage Boost");

            await ApplySeedBlocksAsync(db, generated);
        }
    }

    private static async Task ReplaceDuplicateTrainingBlocksAsync(ForgeDbContext db)
    {
        var blocks = await db.TrainingBlocks
            .Include(x => x.Lesson)
            .ThenInclude(x => x!.Module)
            .OrderBy(x => x.Id)
            .ToListAsync();

        if (blocks.Count == 0)
        {
            return;
        }

        var usedContentKeys = blocks
            .Select(x => NormalizeDuplicateKey(x.Content))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToHashSet(StringComparer.Ordinal);
        var updated = false;

        // First pass: duplicate titles within the same lesson.
        var duplicateTitleGroups = blocks
            .GroupBy(x => BuildLessonTitleGroupKey(x.LessonId, x.Title), StringComparer.Ordinal)
            .Where(x => x.Count() > 1)
            .ToList();

        foreach (var group in duplicateTitleGroups)
        {
            var duplicates = group
                .OrderBy(x => x.Id)
                .Skip(1)
                .ToList();

            if (duplicates.Count == 0)
            {
                continue;
            }

            var reference = group.First();
            var category = reference.Lesson?.Module?.Category ?? "C#";
            var lessonTitle = reference.Lesson?.Title ?? $"Coverage Boost - {category}";
            var lessonTitles = blocks
                .Where(x => x.LessonId == reference.LessonId)
                .Select(x => x.Title)
                .ToHashSet(StringComparer.Ordinal);
            var replacements = GenerateUniqueSeedBlocks(
                category,
                lessonTitle,
                duplicates.Count,
                lessonTitles,
                usedContentKeys,
                "Refined");

            for (var index = 0; index < duplicates.Count; index++)
            {
                ApplyReplacement(duplicates[index], replacements[index]);
                updated = true;
            }
        }

        // Second pass: duplicate content regardless of lesson.
        var duplicateContentGroups = blocks
            .GroupBy(x => NormalizeDuplicateKey(x.Content), StringComparer.Ordinal)
            .Where(x => !string.IsNullOrWhiteSpace(x.Key) && x.Count() > 1)
            .ToList();

        foreach (var group in duplicateContentGroups)
        {
            var duplicates = group
                .OrderBy(x => x.Id)
                .Skip(1)
                .ToList();

            foreach (var duplicate in duplicates)
            {
                var category = duplicate.Lesson?.Module?.Category ?? "C#";
                var lessonTitle = duplicate.Lesson?.Title ?? $"Coverage Boost - {category}";
                var lessonTitles = blocks
                    .Where(x => x.LessonId == duplicate.LessonId)
                    .Select(x => x.Title)
                    .ToHashSet(StringComparer.Ordinal);
                var replacement = GenerateUniqueSeedBlocks(
                    category,
                    lessonTitle,
                    1,
                    lessonTitles,
                    usedContentKeys,
                    "Refined")[0];

                ApplyReplacement(duplicate, replacement);
                updated = true;
            }
        }

        if (updated)
        {
            await db.SaveChangesAsync();
        }
    }

    private static void ApplyReplacement(TrainingBlock targetBlock, SeedBlock replacement)
    {
        targetBlock.Title = replacement.Title;
        targetBlock.Content = BuildSeedBlockContent(replacement);
        targetBlock.Difficulty = Math.Clamp(replacement.Difficulty, 1, 5);
    }

    private static List<SeedBlock> GenerateUniqueSeedBlocks(
        string category,
        string lessonTitle,
        int requiredCount,
        ISet<string> existingLessonTitles,
        ISet<string> usedContentKeys,
        string titlePrefix)
    {
        var generated = new List<SeedBlock>(requiredCount);
        var concepts = GetConceptsForCategory(category);
        var variantCount = GetVariantCountForCategory(category);
        var sequence = 1;

        while (generated.Count < requiredCount)
        {
            var conceptIndex = (sequence - 1) % concepts.Count;
            var variantIndex = ((sequence - 1) / concepts.Count) % variantCount;
            var concept = concepts[conceptIndex];
            var candidate = BuildGeneratedSeedBlock(category, lessonTitle, concept, sequence, variantIndex, titlePrefix);
            sequence++;

            if (existingLessonTitles.Contains(candidate.Title))
            {
                continue;
            }

            var contentKey = NormalizeDuplicateKey(BuildSeedBlockContent(candidate));
            if (usedContentKeys.Contains(contentKey))
            {
                continue;
            }

            generated.Add(candidate);
            existingLessonTitles.Add(candidate.Title);
            usedContentKeys.Add(contentKey);
        }

        return generated;
    }

    private static SeedBlock BuildGeneratedSeedBlock(
        string category,
        string lessonTitle,
        string concept,
        int sequence,
        int variantIndex,
        string titlePrefix)
    {
        if (string.Equals(category, "System Design", StringComparison.OrdinalIgnoreCase))
        {
            return BuildSystemDesignSeedBlock(lessonTitle, concept, sequence, variantIndex, titlePrefix);
        }

        return BuildCSharpSeedBlock(lessonTitle, concept, sequence, variantIndex, titlePrefix);
    }

    private static SeedBlock BuildCSharpSeedBlock(
        string lessonTitle,
        string concept,
        int sequence,
        int variantIndex,
        string titlePrefix)
    {
        return variantIndex switch
        {
            0 => new SeedBlock(
                ModuleCategory: "C#",
                LessonTitle: lessonTitle,
                Title: $"{titlePrefix} - C# #{sequence:000}: {concept} (Core)",
                Response: $"{concept} is a core C# skill used to keep .NET code readable, reliable, and maintainable in production systems.",
                Example: $"In an ASP.NET Core service, apply {concept.ToLowerInvariant()} so features remain testable and easy to evolve.",
                Difficulty: 2),
            1 => new SeedBlock(
                ModuleCategory: "C#",
                LessonTitle: lessonTitle,
                Title: $"{titlePrefix} - C# #{sequence:000}: {concept} (Application)",
                Response: $"Use {concept.ToLowerInvariant()} when implementing application code that needs clear behavior, strong testability, and stable runtime outcomes.",
                Example: $"During feature development, evaluate where {concept.ToLowerInvariant()} reduces complexity before adding new code.",
                Difficulty: 3),
            _ => new SeedBlock(
                ModuleCategory: "C#",
                LessonTitle: lessonTitle,
                Title: $"{titlePrefix} - C# #{sequence:000}: {concept} (Pitfall)",
                Response: $"A common mistake with {concept.ToLowerInvariant()} is inconsistent use across the codebase, which increases defects and long-term maintenance cost.",
                Example: $"During code review, standardize {concept.ToLowerInvariant()} usage to prevent regressions and improve team velocity.",
                Difficulty: 3)
        };
    }

    private static SeedBlock BuildSystemDesignSeedBlock(
        string lessonTitle,
        string concept,
        int sequence,
        int variantIndex,
        string titlePrefix)
    {
        return variantIndex switch
        {
            0 => new SeedBlock(
                ModuleCategory: "System Design",
                LessonTitle: lessonTitle,
                Title: $"{titlePrefix} - System Design #{sequence:000}: {concept} (Principle)",
                Response: $"{concept} is a system-design principle that improves scalability, resiliency, and operational clarity in distributed systems.",
                Example: $"When designing a cloud platform, explicitly document {concept.ToLowerInvariant()} to align architecture decisions with reliability goals.",
                Difficulty: 3),
            _ => new SeedBlock(
                ModuleCategory: "System Design",
                LessonTitle: lessonTitle,
                Title: $"{titlePrefix} - System Design #{sequence:000}: {concept} (Trade-off)",
                Response: $"{concept} requires evaluating performance, cost, and complexity trade-offs before selecting a final architecture.",
                Example: $"In architecture review, compare options for {concept.ToLowerInvariant()} and justify the selected trade-off with measurable criteria.",
                Difficulty: 4)
        };
    }

    private static IReadOnlyList<string> GetConceptsForCategory(string category)
    {
        return string.Equals(category, "System Design", StringComparison.OrdinalIgnoreCase)
            ? SystemDesignConcepts
            : CSharpConcepts;
    }

    private static int GetVariantCountForCategory(string category)
    {
        return string.Equals(category, "System Design", StringComparison.OrdinalIgnoreCase)
            ? 2
            : 3;
    }

    private static string BuildLessonTitleGroupKey(int lessonId, string title)
    {
        return $"{lessonId}\u001f{NormalizeDuplicateKey(title)}";
    }

    private static string NormalizeDuplicateKey(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return Regex.Replace(value.Trim().ToLowerInvariant(), @"\s+", " ");
    }

    private static async Task ApplySeedBlocksAsync(ForgeDbContext db, IReadOnlyList<SeedBlock> seedBlocks)
    {
        if (seedBlocks.Count == 0)
        {
            return;
        }

        var now = DateTime.UtcNow;

        foreach (var seedBlock in seedBlocks)
        {
            var lesson = await EnsureLessonAsync(db, seedBlock.ModuleCategory, seedBlock.LessonTitle);
            var blockExists = await db.TrainingBlocks
                .AnyAsync(x => x.LessonId == lesson.Id && x.Title == seedBlock.Title);

            if (blockExists)
            {
                continue;
            }

            db.TrainingBlocks.Add(new TrainingBlock
            {
                LessonId = lesson.Id,
                Title = seedBlock.Title,
                Content = BuildSeedBlockContent(seedBlock),
                Difficulty = Math.Clamp(seedBlock.Difficulty, 1, 5),
                TimesPracticed = 0,
                TimesPerfect = 0,
                LastPracticedAt = null,
                NextDueAt = now
            });
        }

        await db.SaveChangesAsync();
    }

    private static async Task<TrainingLesson> EnsureLessonAsync(ForgeDbContext db, string category, string lessonTitle)
    {
        var now = DateTime.UtcNow;
        var module = await db.TrainingModules
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync(x => x.Category == category);

        if (module is null)
        {
            var moduleName = $"{LegacyImportConstants.ModuleNamePrefix}{category} Supplemental";

            module = new TrainingModule
            {
                Name = moduleName,
                Category = category,
                CreatedAt = now
            };

            db.TrainingModules.Add(module);
            await db.SaveChangesAsync();
        }

        var lesson = await db.TrainingLessons
            .FirstOrDefaultAsync(x => x.ModuleId == module.Id && x.Title == lessonTitle);

        if (lesson is not null)
        {
            return lesson;
        }

        var currentMaxOrderIndex = await db.TrainingLessons
            .Where(x => x.ModuleId == module.Id)
            .Select(x => (int?)x.OrderIndex)
            .MaxAsync() ?? -1;

        lesson = new TrainingLesson
        {
            ModuleId = module.Id,
            Title = lessonTitle,
            OrderIndex = currentMaxOrderIndex + 1,
            CreatedAt = now
        };

        db.TrainingLessons.Add(lesson);
        await db.SaveChangesAsync();
        return lesson;
    }

    private static string BuildSeedBlockContent(SeedBlock seedBlock)
    {
        var sections = new ParsedTrainingContent(seedBlock.Title, seedBlock.Response, seedBlock.Example);
        var normalized = TrainingContentFormatter.ReversePromptResponse(sections);
        return TrainingContentFormatter.BuildLabeledContent(normalized.Prompt, normalized.Response, normalized.Example);
    }

    private static async Task ApplyPromptResponseOrientationMigrationAsync(ForgeDbContext db)
    {
        var updated = false;

        var trainingBlocks = await db.TrainingBlocks.ToListAsync();
        foreach (var block in trainingBlocks)
        {
            if (!TrainingContentFormatter.TryParseLabeledSections(block.Content, out var sections))
            {
                continue;
            }

            var normalized = TrainingContentFormatter.ReversePromptResponse(sections);
            if (normalized == sections)
            {
                continue;
            }

            block.Content = TrainingContentFormatter.BuildLabeledContent(
                normalized.Prompt,
                normalized.Response,
                normalized.Example);

            updated = true;
        }

        var flashcards = await db.Flashcards.ToListAsync();
        foreach (var flashcard in flashcards)
        {
            var question = flashcard.Question;
            var answer = flashcard.Answer;

            if (TrainingContentFormatter.NeedsPromptResponseReversal(question, answer))
            {
                question = TrainingContentFormatter.BuildPromptFromResponse(answer);
                answer = TrainingContentFormatter.BuildResponseFromPrompt(flashcard.Question);
            }

            question = TrainingContentFormatter.NormalizePromptForTermResponse(question, answer);

            var normalizedQuestion = TruncateValue(question, 500);
            var normalizedAnswer = TruncateValue(answer, 500);

            if (!string.Equals(flashcard.Question, normalizedQuestion, StringComparison.Ordinal) ||
                !string.Equals(flashcard.Answer, normalizedAnswer, StringComparison.Ordinal))
            {
                flashcard.Question = normalizedQuestion;
                flashcard.Answer = normalizedAnswer;
                updated = true;
            }
        }

        if (updated)
        {
            await db.SaveChangesAsync();
        }
    }

    private static string TruncateValue(string value, int maxLength)
    {
        if (value.Length <= maxLength)
        {
            return value;
        }

        return value[..maxLength].TrimEnd();
    }

    private sealed record SeedBlock(
        string ModuleCategory,
        string LessonTitle,
        string Title,
        string Response,
        string Example,
        int Difficulty);
}
