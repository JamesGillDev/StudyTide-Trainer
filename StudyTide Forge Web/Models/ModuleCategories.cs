namespace StudyTideForge.Models;

public static class ModuleCategories
{
    public const string CSharp = "C#";
    public const string Azure = "Azure";
    public const string Sql = "SQL";
    public const string DevOps = "DevOps";
    public const string SystemDesign = "System Design";
    public const string Behavioral = "Behavioral";

    public static readonly IReadOnlyList<string> Allowed =
    [
        CSharp,
        Azure,
        Sql,
        DevOps,
        SystemDesign,
        Behavioral
    ];
}
