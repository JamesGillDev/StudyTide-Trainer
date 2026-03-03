using Microsoft.EntityFrameworkCore;
using StudyTideForge.Components;
using StudyTideForge.Data;
using StudyTideForge.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var databasePath = builder.Configuration["FORGE_WEB_DB_PATH"];

if (string.IsNullOrWhiteSpace(databasePath))
{
    databasePath = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "studytide-forge-web.db");
}

var databaseDirectory = Path.GetDirectoryName(databasePath);

if (!string.IsNullOrWhiteSpace(databaseDirectory))
{
    Directory.CreateDirectory(databaseDirectory);
}

var bundledSourcePath = Path.Combine(builder.Environment.ContentRootPath, "seed-source", "legacy-source.cs");

if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("FORGE_IMPORT_SOURCE_FILE")) && File.Exists(bundledSourcePath))
{
    Environment.SetEnvironmentVariable("FORGE_IMPORT_SOURCE_FILE", bundledSourcePath);
}

builder.Services.AddDbContextFactory<ForgeDbContext>(options =>
    options.UseSqlite($"Data Source={databasePath}"));

builder.Services.AddScoped<PracticeService>();
builder.Services.AddScoped<FlashcardService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<StudyLibraryService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

await DatabaseInitializer.InitializeAsync(app.Services);

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
