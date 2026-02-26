using System.ComponentModel.DataAnnotations;

namespace StudyTideTrainer.Models;

public class PracticeAttempt
{
    public int Id { get; set; }

    [Required]
    public int SnippetId { get; set; }

    public Snippet? Snippet { get; set; }

    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string TypedText { get; set; } = string.Empty;

    [Range(0, 100)]
    public double AccuracyPercent { get; set; }

    public int ErrorCount { get; set; }

    public int MissingChars { get; set; }

    public int ExtraChars { get; set; }

    // -1 means no mismatch was found.
    public int FirstMismatchIndex { get; set; } = -1;
}