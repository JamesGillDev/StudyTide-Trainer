using System.ComponentModel.DataAnnotations;

namespace StudyTideForge.Models;

public sealed class Flashcard
{
    public int Id { get; set; }

    public int LessonId { get; set; }

    [Required]
    [StringLength(500)]
    public string Question { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Answer { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Difficulty { get; set; }

    public int TimesCorrect { get; set; }

    public int TimesIncorrect { get; set; }

    public TrainingLesson? Lesson { get; set; }
}
