namespace StudyTideForge.Models;

public sealed class PracticeAttempt
{
    public int Id { get; set; }

    public int TrainingBlockId { get; set; }

    public DateTime AttemptedAt { get; set; }

    public string TypedText { get; set; } = string.Empty;

    public double AccuracyPercent { get; set; }

    public int ErrorCount { get; set; }

    public int FirstMismatchIndex { get; set; }

    public TrainingBlock? TrainingBlock { get; set; }
}
