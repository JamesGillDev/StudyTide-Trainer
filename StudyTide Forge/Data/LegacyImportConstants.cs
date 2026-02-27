namespace StudyTideForge.Data;

public static class LegacyImportConstants
{
    public const string ModuleNamePrefix = "Legacy Import - ";

    public const int LessonChunkSize = 100;

    public static readonly IReadOnlyList<ModuleSeedDefinition> ModuleDefinitions =
    [
        new("C# Skill Forge", "C#"),
        new("Azure Skill Forge", "Azure"),
        new("SQL Skill Forge", "SQL"),
        new("DevOps Skill Forge", "DevOps"),
        new("System Design Skill Forge", "System Design"),
        new("Behavioral Skill Forge", "Behavioral")
    ];
}

public sealed record ModuleSeedDefinition(string Name, string Category);
