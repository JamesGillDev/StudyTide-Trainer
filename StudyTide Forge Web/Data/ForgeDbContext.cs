using Microsoft.EntityFrameworkCore;
using StudyTideForge.Models;

namespace StudyTideForge.Data;

public sealed class ForgeDbContext : DbContext
{
    public ForgeDbContext(DbContextOptions<ForgeDbContext> options)
        : base(options)
    {
    }

    public DbSet<TrainingModule> TrainingModules => Set<TrainingModule>();

    public DbSet<TrainingLesson> TrainingLessons => Set<TrainingLesson>();

    public DbSet<TrainingBlock> TrainingBlocks => Set<TrainingBlock>();

    public DbSet<Flashcard> Flashcards => Set<Flashcard>();

    public DbSet<PracticeAttempt> PracticeAttempts => Set<PracticeAttempt>();

    public DbSet<StudyLessonProgress> StudyLessonProgresses => Set<StudyLessonProgress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TrainingModule>(entity =>
        {
            entity.Property(x => x.Name).IsRequired().HasMaxLength(120);
            entity.Property(x => x.Category).IsRequired().HasMaxLength(40);
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.HasIndex(x => new { x.Category, x.Name }).IsUnique();
        });

        modelBuilder.Entity<TrainingLesson>(entity =>
        {
            entity.Property(x => x.Title).IsRequired().HasMaxLength(160);
            entity.Property(x => x.IsFlagged).IsRequired().HasDefaultValue(false);
            entity.Property(x => x.OrderIndex).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.HasIndex(x => new { x.ModuleId, x.OrderIndex });
            entity.HasIndex(x => x.IsFlagged);

            entity.HasOne(x => x.Module)
                .WithMany(x => x.Lessons)
                .HasForeignKey(x => x.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TrainingBlock>(entity =>
        {
            entity.Property(x => x.Title).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Content).IsRequired();
            entity.Property(x => x.Difficulty).IsRequired();
            entity.HasIndex(x => x.NextDueAt);

            entity.HasOne(x => x.Lesson)
                .WithMany(x => x.TrainingBlocks)
                .HasForeignKey(x => x.LessonId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Flashcard>(entity =>
        {
            entity.Property(x => x.Question).IsRequired().HasMaxLength(500);
            entity.Property(x => x.Answer).IsRequired().HasMaxLength(500);
            entity.Property(x => x.Difficulty).IsRequired();
            entity.HasIndex(x => x.LessonId);

            entity.HasOne(x => x.Lesson)
                .WithMany(x => x.Flashcards)
                .HasForeignKey(x => x.LessonId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StudyLessonProgress>(entity =>
        {
            entity.Property(x => x.CurrentBlockIndex).IsRequired().HasDefaultValue(0);
            entity.Property(x => x.HighestBlockIndex).IsRequired().HasDefaultValue(-1);
            entity.Property(x => x.IsCompleted).IsRequired().HasDefaultValue(false);
            entity.Property(x => x.LastViewedAt).IsRequired();
            entity.HasIndex(x => x.LessonId).IsUnique();
            entity.HasIndex(x => x.LastViewedAt);

            entity.HasOne(x => x.Lesson)
                .WithOne(x => x.StudyProgress)
                .HasForeignKey<StudyLessonProgress>(x => x.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.CurrentTrainingBlock)
                .WithMany()
                .HasForeignKey(x => x.CurrentTrainingBlockId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<PracticeAttempt>(entity =>
        {
            entity.Property(x => x.AttemptedAt).IsRequired();
            entity.Property(x => x.TypedText).IsRequired();
            entity.Property(x => x.AccuracyPercent).IsRequired();
            entity.Property(x => x.ErrorCount).IsRequired();
            entity.Property(x => x.FirstMismatchIndex).IsRequired();
            entity.HasIndex(x => x.AttemptedAt);

            entity.HasOne(x => x.TrainingBlock)
                .WithMany(x => x.PracticeAttempts)
                .HasForeignKey(x => x.TrainingBlockId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
