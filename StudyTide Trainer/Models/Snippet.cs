using System.ComponentModel.DataAnnotations;

namespace StudyTideTrainer.Models;

public class Snippet
{
    public int Id { get; set; }

    [Required]
    public int TopicId { get; set; }

    public Topic? Topic { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string SourceText { get; set; } = string.Empty;

    [StringLength(300)]
    public string Tags { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int TimesPracticed { get; set; }

    public int TimesPerfect { get; set; }

    public DateTime? LastPracticedAt { get; set; }

    public DateTime? NextDueAt { get; set; }

    public ICollection<PracticeAttempt> PracticeAttempts { get; set; } = new List<PracticeAttempt>();
}