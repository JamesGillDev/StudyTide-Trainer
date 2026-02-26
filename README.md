# StudyTide Forge

StudyTide Forge is a local desktop training app for structured cognitive reinforcement. Learners retype training blocks verbatim and practice flashcards to strengthen recall over time.

## Purpose

- Build durable technical memory through structured repetition.
- Practice exact retyping against source content.
- Reinforce question-and-answer recall with flashcards.

## Architecture

- .NET 8
- .NET MAUI Blazor Hybrid (desktop app)
- EF Core 8
- SQLite
- Single `DbContext`: `ForgeDbContext`
- Migration-first schema management

## Domain Model

- `TrainingModule`
  - `Id, Name, Category, CreatedAt`
- `TrainingLesson`
  - `Id, ModuleId, Title, OrderIndex, CreatedAt`
- `TrainingBlock`
  - `Id, LessonId, Title, Content, Difficulty, TimesPracticed, TimesPerfect, LastPracticedAt, NextDueAt`
- `Flashcard`
  - `Id, LessonId, Question, Answer, Difficulty, TimesCorrect, TimesIncorrect`
- `PracticeAttempt`
  - `Id, TrainingBlockId, AttemptedAt, TypedText, AccuracyPercent, ErrorCount, FirstMismatchIndex`

## Features

- Structured curriculum browser (`/modules`)
  - Modules -> lessons -> blocks and flashcards
- Verbatim practice mode (`/practice`)
  - Scope by module/lesson
  - Character-level scoring and line diff summary
  - Scheduling rules:
    - `100%` accuracy -> due in `3 days`
    - `>=95%` accuracy -> due in `1 day`
    - Otherwise -> due in `1 hour`
- Flashcard mode (`/flashcards`)
  - Load random due card
  - Reveal answer
  - Mark correct/incorrect
- Dashboard (`/`)
  - Total modules
  - Blocks due
  - Flashcards due
  - Accuracy last 7 days
  - Weakest blocks
  - Weakest flashcards

## Training Data Import Rules Implemented

- Source records are treated as generic Q/A training data.
- Imported fields: `Question`, `Answer`.
- Ignored fields and mechanics: points, timers, score logic, and game-specific behavior.
- Each Q/A pair creates:
  - one `Flashcard`
  - one `TrainingBlock` with clean content:
    - `Question:`
    - `Answer:`

## Project Structure

- `StudyTide Forge/StudyTideForge.csproj` - MAUI app project
- `StudyTide Forge/MauiProgram.cs` - startup and service registration
- `StudyTide Forge/Data/ForgeDbContext.cs` - EF Core context
- `StudyTide Forge/Data/DatabaseInitializer.cs` - migration + seeding bootstrap
- `StudyTide Forge/Data/TrainingSeedCatalog.cs` - static Q/A seed records
- `StudyTide Forge/Models/*` - domain entities
- `StudyTide Forge/Services/*` - practice, flashcards, dashboard logic
- `StudyTide Forge/Components/Pages/*` - UI screens
- `StudyTide Forge/wwwroot/css/app.css` - dark navy + teal theme

## Local Run

From repo root:

```powershell
dotnet restore
```

Run desktop app on Windows:

```powershell
dotnet run --project ".\StudyTide Forge\StudyTideForge.csproj" -f net8.0-windows10.0.19041.0
```

## EF Core Migration Commands

From repo root:

```powershell
dotnet tool restore

dotnet dotnet-ef migrations add InitialForge \
  --project ".\StudyTide Forge\StudyTideForge.csproj" \
  --framework net8.0-windows10.0.19041.0 \
  --output-dir Data\Migrations

dotnet dotnet-ef database update \
  --project ".\StudyTide Forge\StudyTideForge.csproj" \
  --framework net8.0-windows10.0.19041.0
```

## Release Commands

Create a publish package for Windows:

```powershell
dotnet publish ".\StudyTide Forge\StudyTideForge.csproj" \
  -c Release \
  -f net8.0-windows10.0.19041.0 \
  -o .\artifacts\publish
```

## Git Workflow

If this is a new repository:

```powershell
git init
git add .
git commit -m "Initial release - StudyTide Forge v1.0"
git branch -M main
git remote add origin https://github.com/<your-user>/<your-repo>.git
git push -u origin main
git tag v1.0
git push origin v1.0
```

If git is already initialized, skip `git init`.

## GitHub Repository Creation

1. Create an empty repository on GitHub.
2. Copy the HTTPS URL.
3. Run `git remote add origin <url>`.
4. Push `main` and `v1.0` tag.

## Azure App Service Guidance

A MAUI desktop app is not hosted directly in App Service. Use these options:

1. Deploy a companion ASP.NET Core web/API service to App Service.
2. Keep desktop app local and call hosted APIs if needed.

Example App Service deploy flow for a companion web app:

```powershell
az login
az account set --subscription "<subscription>"
az webapp up \
  --name "<unique-app-name>" \
  --resource-group "<resource-group>" \
  --plan "<app-service-plan>" \
  --runtime "DOTNETCORE:8.0" \
  --os-type Linux
```

## Environment Variables

Desktop app:

- `FORGE_DB_PATH` (optional future override for database location)
- `ASPNETCORE_ENVIRONMENT` (if reused by shared libraries)

Companion web app (if deployed):

- `ConnectionStrings__DefaultConnection`
- `ASPNETCORE_ENVIRONMENT`

## SQLite Deployment Considerations

- SQLite is ideal for local-first desktop use.
- Keep database file in app data folder for user-scoped storage.
- Back up the `.db` file during upgrades.
- For multi-user cloud workloads, migrate to a server database (for example Azure SQL).

## Screenshots

- Dashboard: _placeholder_
- Modules: _placeholder_
- Practice: _placeholder_
- Flashcards: _placeholder_
