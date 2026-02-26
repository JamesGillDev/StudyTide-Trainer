using Microsoft.EntityFrameworkCore;
using StudyTideTrainer.Models;

namespace StudyTideTrainer.Data;

public class TrainingDbContext(DbContextOptions<TrainingDbContext> options) : DbContext(options)
{
    public DbSet<Topic> Topics => Set<Topic>();

    public DbSet<Snippet> Snippets => Set<Snippet>();

    public DbSet<PracticeAttempt> PracticeAttempts => Set<PracticeAttempt>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.Property(x => x.Name).IsRequired().HasMaxLength(120);
            entity.Property(x => x.Category).IsRequired().HasMaxLength(60);
            entity.Property(x => x.Difficulty).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<Snippet>(entity =>
        {
            entity.Property(x => x.Title).IsRequired().HasMaxLength(200);
            entity.Property(x => x.SourceText).IsRequired();
            entity.Property(x => x.Tags).HasMaxLength(300);
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.HasIndex(x => x.NextDueAt);

            entity.HasOne(x => x.Topic)
                .WithMany(x => x.Snippets)
                .HasForeignKey(x => x.TopicId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PracticeAttempt>(entity =>
        {
            entity.Property(x => x.TypedText).IsRequired();
            entity.Property(x => x.AccuracyPercent).IsRequired();
            entity.Property(x => x.AttemptedAt).IsRequired();
            entity.HasIndex(x => x.AttemptedAt);

            entity.HasOne(x => x.Snippet)
                .WithMany(x => x.PracticeAttempts)
                .HasForeignKey(x => x.SnippetId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}