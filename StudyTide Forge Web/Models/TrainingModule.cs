using System.ComponentModel.DataAnnotations;

namespace StudyTideForge.Models;

public sealed class TrainingModule
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(40)]
    public string Category { get; set; } = ModuleCategories.Behavioral;

    public DateTime CreatedAt { get; set; }

    public ICollection<TrainingLesson> Lessons { get; set; } = new List<TrainingLesson>();
}
