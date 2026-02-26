namespace StudyTideTrainer.Data;

public sealed class SeedTopic
{
    public string Name { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public int Difficulty { get; init; }

    public List<SeedSnippet> Snippets { get; init; } = new();
}

public sealed class SeedSnippet
{
    public string Title { get; init; } = string.Empty;

    public string SourceText { get; init; } = string.Empty;

    public string Tags { get; init; } = string.Empty;
}