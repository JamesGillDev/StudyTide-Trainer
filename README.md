# StudyTide Trainer (.NET 8, Local-First)

StudyTide Trainer is a local-first MSSA training app built with Blazor Server, EF Core, and SQLite. It is designed for verbatim retype practice: you retype source material exactly, get instant mismatch feedback, and the app schedules your next review.

## Tech Stack

- .NET 8
- ASP.NET Core Blazor Server
- EF Core + SQLite
- EF Core migrations

## Project Structure

- `StudyTide Trainer/Program.cs`: app startup, service registration, and database initialization
- `StudyTide Trainer/Data/TrainingDbContext.cs`: EF Core DbContext
- `StudyTide Trainer/Models/*`: `Topic`, `Snippet`, `PracticeAttempt`
- `StudyTide Trainer/Services/PracticeService.cs`: scoring + scheduling logic
- `StudyTide Trainer/Services/DashboardService.cs`: dashboard metrics
- `StudyTide Trainer/Data/TrainingPackCatalog.cs`: built-in curated packs and seed snippets
- `StudyTide Trainer/Pages/*`: dashboard, topic CRUD, snippet CRUD, practice UI, snippet details/history

## Data Model

- `Topic`
  - `Id, Name, Category, Difficulty, CreatedAt`
- `Snippet`
  - `Id, TopicId, Title, SourceText, Tags, CreatedAt`
  - `TimesPracticed, TimesPerfect, LastPracticedAt, NextDueAt`
- `PracticeAttempt`
  - `Id, SnippetId, AttemptedAt, TypedText`
  - `AccuracyPercent, ErrorCount, MissingChars, ExtraChars, FirstMismatchIndex`

## Verbatim Practice Scoring

On submit, the app compares `TypedText` to `SourceText` using:

1. Character-level comparison
- Counts character mismatches
- Counts missing and extra characters
- Captures first mismatch index

2. Line-level comparison
- Builds a diff list of lines that do not match
- Shows source line and typed line side-by-side

### Metrics

- `AccuracyPercent`
- `ErrorCount`
- `MissingCharacters`
- `ExtraCharacters`
- `FirstMismatchIndex`

Accuracy formula:

- If source length is `0`, accuracy is `100` only when typed length is also `0`
- Otherwise: `((sourceLength - errorCount) / sourceLength) * 100`, clamped to `0..100`

## Scheduling Rules

After each attempt:

- Always increment `TimesPracticed`
- Always set `LastPracticedAt = now`
- If `AccuracyPercent == 100`
  - `NextDueAt = now + 3 days`
  - increment `TimesPerfect`
- Else if `AccuracyPercent >= 95`
  - `NextDueAt = now + 1 day`
- Else
  - `NextDueAt = now + 1 hour`

Practice selection:

- Prefer snippets where `NextDueAt <= now` (or no due date yet)
- If none are due, pick least-recently-practiced snippet

## Built-In Curated Packs

The seed catalog includes these packs, each with 20+ interview-depth multi-line snippets:

- C# Deep Dive
- ASP.NET Core Internals
- Azure for Cloud Developers
- SQL for Backend Engineers
- Git & DevOps Essentials
- Interview Behavioral Mastery
- System Design for Junior Engineers

It also includes the requested examples for:

- C# loops
- String interpolation
- Parsing with `TryParse`

## Deployment

### Non-Azure Deployment (Local)

From the repo root:

```bash
dotnet restore
dotnet tool restore
```

From the project folder (`StudyTide Trainer/StudyTide Trainer`):

```bash
dotnet dotnet-ef database update
dotnet run
```

The app runs locally and uses SQLite (`studytide-trainer.db`) in the project directory.

Optional (only for first-time schema creation in a new clone if no migration exists yet):

```bash
dotnet dotnet-ef migrations add InitialCreate
dotnet dotnet-ef database update
```

### Azure Deployment (App Service)

Use this when your Azure subscription allows write operations.

1. Login and select subscription:

```bash
az login
az account set --subscription "<your-subscription-id-or-name>"
```

2. Deploy from the project folder (`StudyTide Trainer/StudyTide Trainer`):

```bash
az webapp up \
  --name "<globally-unique-app-name>" \
  --resource-group "<resource-group-name>" \
  --plan "<app-service-plan-name>" \
  --location "eastus" \
  --runtime "DOTNETCORE:8.0" \
  --os-type Linux \
  --sku F1
```

3. Set app settings if needed:

```bash
az webapp config appsettings set \
  --name "<globally-unique-app-name>" \
  --resource-group "<resource-group-name>" \
  --settings ASPNETCORE_ENVIRONMENT=Production
```

4. Browse deployed app:

```bash
az webapp browse --name "<globally-unique-app-name>" --resource-group "<resource-group-name>"
```

Notes:
- The app uses SQLite by default (`Data Source=studytide-trainer.db`).
- For production-grade Azure hosting, move from SQLite to Azure SQL and update `DefaultConnection`.
- If Azure returns `ReadOnlyDisabledSubscription`, the subscription must be re-enabled first.

## Notes

- Database file: `studytide-trainer.db`
- Local-first by default (no auth, no external APIs)
- Source text is stored as-is so newlines and indentation are preserved for verbatim practice
