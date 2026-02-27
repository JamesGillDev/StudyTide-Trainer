using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StudyTideForge.Data;

public sealed class ForgeDbContextFactory : IDesignTimeDbContextFactory<ForgeDbContext>
{
    public ForgeDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ForgeDbContext>();
        optionsBuilder.UseSqlite("Data Source=studytide-forge.db");

        return new ForgeDbContext(optionsBuilder.Options);
    }
}
