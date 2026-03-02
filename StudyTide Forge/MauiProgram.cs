using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Storage;
using StudyTideForge.Data;
using StudyTideForge.Services;

namespace StudyTideForge;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        ConfigureProcessEnvironment();

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "studytide-forge.db");
        var connectionString = $"Data Source={databasePath}";

        builder.Services.AddDbContextFactory<ForgeDbContext>(options =>
            options.UseSqlite(connectionString));

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

    private static void ConfigureProcessEnvironment()
    {
        // Ensure relative asset loading is stable when launched from shortcuts with custom "Start in" directories.
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);

        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var webViewUserDataFolder = Path.Combine(localAppData, "StudyTideForge", "webview2");
        Directory.CreateDirectory(webViewUserDataFolder);
        Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", webViewUserDataFolder);

        // Force software rendering to avoid black/blank WebView windows on affected GPU/driver combos.
        const string disableGpuArgument = "--disable-gpu";
        var existingArgs = Environment.GetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS");

        if (string.IsNullOrWhiteSpace(existingArgs))
        {
            Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", disableGpuArgument);
            return;
        }

        if (!existingArgs.Contains(disableGpuArgument, StringComparison.OrdinalIgnoreCase))
        {
            Environment.SetEnvironmentVariable("WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS", $"{existingArgs} {disableGpuArgument}");
        }
    }
}
