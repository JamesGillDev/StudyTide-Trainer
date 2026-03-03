using System.Text.RegularExpressions;

namespace StudyTideForge.Services;

public static class TrainingContentFormatter
{
    private static readonly string[] SubjectVerbLeadWords =
    [
        "is ",
        "are ",
        "was ",
        "were ",
        "can ",
        "could ",
        "should ",
        "would ",
        "will ",
        "may ",
        "might ",
        "must ",
        "has ",
        "have ",
        "had "
    ];

    private static readonly Regex LabeledContentPattern = new(
        @"^\s*Prompt:\s*\n(?<prompt>.*?)\n\s*\nResponse:\s*\n(?<response>.*?)\n\s*\nExample:\s*\n(?<example>.*)\s*$",
        RegexOptions.Singleline | RegexOptions.CultureInvariant);

    private static readonly Regex WhatQuestionPattern = new(
        @"^\s*what\s+(is|are)\s+(?<answer>.+?)\??\s*$",
        RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    public static string NormalizeLineEndings(string value)
    {
        return value.Replace("\r\n", "\n").Replace("\r", "\n");
    }

    public static bool TryParseLabeledSections(string content, out ParsedTrainingContent contentSections)
    {
        var normalized = NormalizeLineEndings(content).Trim();
        var match = LabeledContentPattern.Match(normalized);

        if (!match.Success)
        {
            contentSections = ParsedTrainingContent.Empty;
            return false;
        }

        contentSections = new ParsedTrainingContent(
            Prompt: match.Groups["prompt"].Value.Trim(),
            Response: match.Groups["response"].Value.Trim(),
            Example: match.Groups["example"].Value.Trim());

        return true;
    }

    public static string BuildLabeledContent(string prompt, string response, string example)
    {
        return $"Prompt:\n{prompt}\n\nResponse:\n{response}\n\nExample:\n{example}";
    }

    public static bool NeedsPromptResponseReversal(string prompt, string response)
    {
        var normalizedPrompt = NormalizeInline(prompt);
        var normalizedResponse = NormalizeInline(response);

        if (string.IsNullOrWhiteSpace(normalizedPrompt) || string.IsNullOrWhiteSpace(normalizedResponse))
        {
            return false;
        }

        var promptWordCount = CountWords(normalizedPrompt);
        var responseWordCount = CountWords(normalizedResponse);
        var promptLooksQuestion = IsWhatQuestion(normalizedPrompt);
        var responseLooksTermLike = LooksTermLike(normalizedResponse, responseWordCount);

        if (promptLooksQuestion && !responseLooksTermLike)
        {
            return true;
        }

        if (responseWordCount >= 9 && promptWordCount <= 8)
        {
            return true;
        }

        return normalizedResponse.Length >= normalizedPrompt.Length + 16;
    }

    public static ParsedTrainingContent ReversePromptResponse(ParsedTrainingContent original)
    {
        var normalized = original;

        if (NeedsPromptResponseReversal(original.Prompt, original.Response))
        {
            var reversedPrompt = BuildPromptFromResponse(original.Response);
            var reversedResponse = BuildResponseFromPrompt(original.Prompt);

            normalized = original with
            {
                Prompt = reversedPrompt,
                Response = reversedResponse
            };
        }

        var polishedPrompt = NormalizePromptForTermResponse(normalized.Prompt, normalized.Response);
        if (!string.Equals(polishedPrompt, normalized.Prompt, StringComparison.Ordinal))
        {
            normalized = normalized with { Prompt = polishedPrompt };
        }

        return normalized;
    }

    public static string BuildPromptFromResponse(string response)
    {
        var normalized = NormalizeInline(response);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return string.Empty;
        }

        var definition = BuildDefinitionSentence(TrimSentenceEnding(normalized));
        return $"Identify the term: {definition}";
    }

    public static string BuildResponseFromPrompt(string prompt)
    {
        var normalized = NormalizeInline(prompt);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return string.Empty;
        }

        var match = WhatQuestionPattern.Match(normalized);
        if (match.Success)
        {
            return TrimSentenceEnding(match.Groups["answer"].Value);
        }

        return TrimSentenceEnding(normalized);
    }

    public static string BuildRetypeTarget(ParsedTrainingContent sections)
    {
        return string.Join(
            "\n",
            new[] { sections.Prompt, sections.Response, sections.Example }
                .Where(value => !string.IsNullOrWhiteSpace(value)));
    }

    public static string NormalizePromptForTermResponse(string prompt, string response)
    {
        var normalizedPrompt = NormalizeInline(prompt);
        var normalizedResponse = NormalizeInline(response);

        if (string.IsNullOrWhiteSpace(normalizedPrompt))
        {
            return normalizedPrompt;
        }

        normalizedPrompt = NormalizeIdentifyPrompt(normalizedPrompt);

        if (string.IsNullOrWhiteSpace(normalizedResponse))
        {
            return normalizedPrompt;
        }

        var responseWordCount = CountWords(normalizedResponse);
        if (!LooksTermLike(normalizedResponse, responseWordCount))
        {
            return normalizedPrompt;
        }

        var match = WhatQuestionPattern.Match(normalizedPrompt);
        if (!match.Success)
        {
            return normalizedPrompt;
        }

        var definition = BuildDefinitionSentence(TrimSentenceEnding(match.Groups["answer"].Value));
        return $"Identify the term: {definition}";
    }

    private static string NormalizeIdentifyPrompt(string prompt)
    {
        if (!prompt.StartsWith("Identify the term:", StringComparison.OrdinalIgnoreCase))
        {
            return prompt;
        }

        var definition = prompt["Identify the term:".Length..].Trim();
        if (string.IsNullOrWhiteSpace(definition))
        {
            return "Identify the term:";
        }

        return $"Identify the term: {BuildDefinitionSentence(definition)}";
    }

    private static string NormalizeInline(string value)
    {
        return Regex.Replace(NormalizeLineEndings(value), @"\s+", " ").Trim();
    }

    private static string TrimSentenceEnding(string value)
    {
        return value.Trim().TrimEnd('.', '!', '?', ';', ':');
    }

    private static bool IsWhatQuestion(string value)
    {
        return WhatQuestionPattern.IsMatch(value);
    }

    private static bool LooksTermLike(string value, int wordCount)
    {
        if (wordCount <= 0)
        {
            return false;
        }

        if (wordCount <= 6 && value.Length <= 80)
        {
            return true;
        }

        if (wordCount <= 8 && value.Length <= 120 && !value.Contains(". ", StringComparison.Ordinal))
        {
            return true;
        }

        return false;
    }

    private static int CountWords(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return 0;
        }

        return value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
    }

    private static string BuildDefinitionSentence(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var trimmed = NormalizeLeadPhrase(TrimSentenceEnding(NormalizeInline(value)));
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return string.Empty;
        }

        if (trimmed.StartsWith("is ", StringComparison.OrdinalIgnoreCase))
        {
            trimmed = $"It {trimmed}";
        }
        else if (trimmed.StartsWith("are ", StringComparison.OrdinalIgnoreCase))
        {
            trimmed = $"They {trimmed}";
        }
        else if (LooksLikeNumberedList(trimmed))
        {
            trimmed = $"The following sequence applies: {trimmed}";
        }

        trimmed = CapitalizeFirst(trimmed);
        return $"{trimmed}.";
    }

    private static bool LooksLikeNumberedList(string value)
    {
        return Regex.IsMatch(value, @"^\d+(\.|\))\s*", RegexOptions.CultureInvariant);
    }

    private static string NormalizeLeadPhrase(string value)
    {
        var normalized = value.Trim();

        while (normalized.StartsWith("this ", StringComparison.OrdinalIgnoreCase) ||
               normalized.StartsWith("these ", StringComparison.OrdinalIgnoreCase))
        {
            var candidate = normalized.StartsWith("this ", StringComparison.OrdinalIgnoreCase)
                ? normalized[5..].TrimStart()
                : normalized[6..].TrimStart();

            if (string.IsNullOrWhiteSpace(candidate))
            {
                break;
            }

            if (NeedsSubjectPronoun(candidate))
            {
                normalized = normalized.StartsWith("this ", StringComparison.OrdinalIgnoreCase)
                    ? $"It {candidate}"
                    : $"They {candidate}";
                break;
            }

            normalized = candidate;
        }

        return normalized;
    }

    private static bool NeedsSubjectPronoun(string value)
    {
        return SubjectVerbLeadWords.Any(lead => value.StartsWith(lead, StringComparison.OrdinalIgnoreCase));
    }

    private static string CapitalizeFirst(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        if (char.IsUpper(value[0]))
        {
            return value;
        }

        return char.ToUpperInvariant(value[0]) + value[1..];
    }
}

public readonly record struct ParsedTrainingContent(string Prompt, string Response, string Example)
{
    public static ParsedTrainingContent Empty { get; } = new(string.Empty, string.Empty, string.Empty);
}
