using Microsoft.EntityFrameworkCore;
using StudyTideForge.Components;
using StudyTideForge.Data;
using StudyTideForge.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration.GetConnectionString("ForgeDb") ?? "Data Source=studytide-forge.db";

builder.Services.AddDbContextFactory<ForgeDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<PracticeService>();
builder.Services.AddScoped<FlashcardService>();
builder.Services.AddScoped<DashboardService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await DatabaseInitializer.InitializeAsync(app.Services);

app.Run();
