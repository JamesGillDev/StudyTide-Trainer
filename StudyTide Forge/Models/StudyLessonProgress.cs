namespace StudyTideForge.Models;

public sealed class StudyLessonProgress
{
    public int Id { get; set; }

    public int LessonId { get; set; }

    public int? CurrentTrainingBlockId { get; set; }

    public int CurrentBlockIndex { get; set; }

    public int HighestBlockIndex { get; set; } = -1;

    public bool IsCompleted { get; set; }

    public DateTime LastViewedAt { get; set; }

    public TrainingLesson? Lesson { get; set; }

    public TrainingBlock? CurrentTrainingBlock { get; set; }
}
