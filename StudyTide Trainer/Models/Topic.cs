using System.ComponentModel.DataAnnotations;

namespace StudyTideTrainer.Models;

public class Topic
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(60)]
    public string Category { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Difficulty { get; set; } = 1;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Snippet> Snippets { get; set; } = new List<Snippet>();
}