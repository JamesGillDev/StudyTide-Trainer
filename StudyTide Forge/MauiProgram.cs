using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudyTideForge.Data;
using StudyTideForge.Services;

namespace StudyTideForge;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "studytide-forge.db");

        builder.Services.AddDbContextFactory<ForgeDbContext>(options =>
            options.UseSqlite($"Data Source={databasePath}"));

        builder.Services.AddScoped<PracticeService>();
        builder.Services.AddScoped<FlashcardService>();
        builder.Services.AddScoped<DashboardService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();
        DatabaseInitializer.InitializeAsync(app.Services).GetAwaiter().GetResult();

        return app;
    }
}
