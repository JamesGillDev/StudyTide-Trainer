using System.ComponentModel.DataAnnotations;

namespace StudyTideForge.Models;

public sealed class TrainingBlock
{
    public int Id { get; set; }

    public int LessonId { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Difficulty { get; set; }

    public int TimesPracticed { get; set; }

    public int TimesPerfect { get; set; }

    public DateTime? LastPracticedAt { get; set; }

    public DateTime? NextDueAt { get; set; }

    public TrainingLesson? Lesson { get; set; }

    public ICollection<PracticeAttempt> PracticeAttempts { get; set; } = new List<PracticeAttempt>();
}
