using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StudyTideTrainer.Data;

public class TrainingDbContextFactory : IDesignTimeDbContextFactory<TrainingDbContext>
{
    public TrainingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TrainingDbContext>();
        optionsBuilder.UseSqlite("Data Source=studytide-trainer.db");

        return new TrainingDbContext(optionsBuilder.Options);
    }
}