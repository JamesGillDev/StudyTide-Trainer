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
        var appLogPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "StudyTideForge",
            "logs",
            "app.log");

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
        builder.Logging.SetMinimumLevel(LogLevel.Information);
        builder.Logging.AddProvider(new FileLoggerProvider(appLogPath, LogLevel.Information));

        var app = builder.Build();

        DatabaseInitializer.InitializeAsync(app.Services).GetAwaiter().GetResult();

        return app;
    }
}

internal sealed class FileLoggerProvider(string logPath, LogLevel minLevel) : ILoggerProvider
{
    private readonly object _sync = new();

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(logPath, minLevel, categoryName, _sync);
    }

    public void Dispose()
    {
    }
}

internal sealed class FileLogger(
    string logPath,
    LogLevel minLevel,
    string categoryName,
    object sync) : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= minLevel;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var directory = Path.GetDirectoryName(logPath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var message = formatter(state, exception);
        var line = $"[{DateTime.UtcNow:O}] {logLevel} {categoryName} {eventId.Id}: {message}";

        if (exception is not null)
        {
            line = $"{line}{Environment.NewLine}{exception}";
        }

        lock (sync)
        {
            File.AppendAllText(logPath, $"{line}{Environment.NewLine}");
        }
    }
}
