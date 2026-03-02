using System.ComponentModel.DataAnnotations;

namespace StudyTideForge.Models;

public sealed class TrainingLesson
{
    public int Id { get; set; }

    public int ModuleId { get; set; }

    [Required]
    [StringLength(160)]
    public string Title { get; set; } = string.Empty;

    public bool IsFlagged { get; set; }

    public int OrderIndex { get; set; }

    public DateTime CreatedAt { get; set; }

    public TrainingModule? Module { get; set; }

    public ICollection<TrainingBlock> TrainingBlocks { get; set; } = new List<TrainingBlock>();

    public ICollection<Flashcard> Flashcards { get; set; } = new List<Flashcard>();
}
