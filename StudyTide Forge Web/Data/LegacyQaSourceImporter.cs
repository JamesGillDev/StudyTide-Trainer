using System.Text.RegularExpressions;

namespace StudyTideForge.Data;

public sealed class LegacyQaSourceImporter
{
    private const string SourceFileSuffix = "GameService.Shared.cs";
    private const string SourcePathOverrideVariable = "FORGE_IMPORT_SOURCE_FILE";

    private static readonly Regex PairRegex = new(
        @"Question\s*=\s*(?<question>@""(?:""""|[^""])*""|""(?:\\.|[^""\\])*"")\s*,\s*Answer\s*=\s*(?<answer>@""(?:""""|[^""])*""|""(?:\\.|[^""\\])*"")",
        RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);

    public ImportedQaResult Import()
    {
        var sourcePath = ResolveSourcePath();
        var sourceText = File.ReadAllText(sourcePath);

        var seen = new HashSet<string>(StringComparer.Ordinal);
        var pairs = new List<ImportedQaPair>();

        foreach (Match match in PairRegex.Matches(sourceText))
        {
            var question = DecodeCSharpStringLiteral(match.Groups["question"].Value).Trim();
            var answer = DecodeCSharpStringLiteral(match.Groups["answer"].Value).Trim();

            if (string.IsNullOrWhiteSpace(question) || string.IsNullOrWhiteSpace(answer))
            {
                continue;
            }

            var dedupeKey = $"{question}\u001f{answer}";

            if (!seen.Add(dedupeKey))
            {
                continue;
            }

            pairs.Add(new ImportedQaPair(question, answer));
        }

        return new ImportedQaResult(sourcePath, pairs);
    }

    private static string ResolveSourcePath()
    {
        var overridePath = Environment.GetEnvironmentVariable(SourcePathOverrideVariable);

        if (!string.IsNullOrWhiteSpace(overridePath) && File.Exists(overridePath))
        {
            return Path.GetFullPath(overridePath);
        }

        var checkedPaths = new List<string>();

        foreach (var root in GetSearchRoots())
        {
            foreach (var candidate in GetCandidates(root))
            {
                checkedPaths.Add(candidate);

                if (File.Exists(candidate))
                {
                    return Path.GetFullPath(candidate);
                }
            }
        }

        throw new FileNotFoundException(
            $"Legacy source file was not found. Set {SourcePathOverrideVariable} or place the source file in a searched location.{Environment.NewLine}Checked paths:{Environment.NewLine}{string.Join(Environment.NewLine, checkedPaths.Distinct(StringComparer.OrdinalIgnoreCase))}");
    }

    private static IEnumerable<string> GetSearchRoots()
    {
        var roots = new List<string>();

        AddAncestors(roots, Directory.GetCurrentDirectory());
        AddAncestors(roots, AppContext.BaseDirectory);

        return roots
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Where(Directory.Exists);
    }

    private static void AddAncestors(ICollection<string> target, string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        var current = new DirectoryInfo(path);

        while (current is not null)
        {
            target.Add(current.FullName);
            current = current.Parent;
        }
    }

    private static IEnumerable<string> GetCandidates(string root)
    {
        foreach (var candidate in EnumerateFilesSafe(root, $"*{SourceFileSuffix}"))
        {
            yield return candidate;
        }

        var sharedPath = Path.Combine(root, "Shared");

        foreach (var candidate in EnumerateFilesSafe(sharedPath, $"*{SourceFileSuffix}"))
        {
            yield return candidate;
        }

        foreach (var directory in EnumerateDirectoriesSafe(root))
        {
            var nestedShared = Path.Combine(directory, "Shared");

            foreach (var candidate in EnumerateFilesSafe(nestedShared, $"*{SourceFileSuffix}"))
            {
                yield return candidate;
            }
        }
    }

    private static IEnumerable<string> EnumerateFilesSafe(string directoryPath, string pattern)
    {
        if (!Directory.Exists(directoryPath))
        {
            yield break;
        }

        IReadOnlyList<string> files;

        try
        {
            files = Directory.EnumerateFiles(directoryPath, pattern, SearchOption.TopDirectoryOnly).ToList();
        }
        catch
        {
            yield break;
        }

        foreach (var file in files)
        {
            yield return file;
        }
    }

    private static IEnumerable<string> EnumerateDirectoriesSafe(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            yield break;
        }

        IReadOnlyList<string> directories;

        try
        {
            directories = Directory.EnumerateDirectories(directoryPath).ToList();
        }
        catch
        {
            yield break;
        }

        foreach (var directory in directories)
        {
            yield return directory;
        }
    }

    private static string DecodeCSharpStringLiteral(string literal)
    {
        if (literal.StartsWith("@\"", StringComparison.Ordinal) && literal.EndsWith('"'))
        {
            var inner = literal[2..^1];
            return inner.Replace("\"\"", "\"", StringComparison.Ordinal);
        }

        if (literal.StartsWith('"') && literal.EndsWith('"'))
        {
            var inner = literal[1..^1];
            return Regex.Unescape(inner);
        }

        throw new InvalidOperationException($"Unsupported string literal format: {literal}");
    }
}

public sealed record ImportedQaResult(string SourcePath, IReadOnlyList<ImportedQaPair> Pairs);

public sealed record ImportedQaPair(string Question, string Answer);
