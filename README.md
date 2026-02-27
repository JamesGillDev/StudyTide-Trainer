# StudyTide Forge

StudyTide Forge is a .NET 8 Blazor Server training platform for structured cognitive reinforcement.  
It is built to help learners encode knowledge by retyping structured material verbatim and drilling flashcards over time.

## Purpose

- Train deep recall through exact retyping of structured content.
- Reinforce knowledge with module-based lessons and flashcards.
- Track weak areas with practice and flashcard performance metrics.

## Architecture

- .NET 8
- Blazor Server
- EF Core 8
- SQLite
- Single DbContext: `ForgeDbContext`
- EF Core migrations for schema lifecycle

## Core Domain

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

## Feature Set

- Structured curriculum at `/modules`
  - Module list
  - Module detail -> lessons
  - Lesson detail -> blocks + flashcards
- Verbatim practice at `/practice`
  - Scope by module or lesson (including lesson-only global scope)
  - Load next due block
  - Full retype submission
  - Diff summary with mismatch stats
- Flashcards at `/flashcards`
  - Random due card with module or lesson filtering
  - Reveal answer
  - Mark correct/incorrect with tracked stats
- Dashboard at `/`
  - Total modules
  - Blocks due
  - Flashcards due
  - Accuracy over last 7 days
  - Weakest blocks
  - Weakest flashcards

## Spaced Repetition Rules

Training block due scheduling on each submission:

- `100%` accuracy -> next due in `3 days`
- `>= 95%` accuracy -> next due in `1 day`
- `< 95%` accuracy -> next due in `1 hour`

## Flashcard Scoring

- `Mark Correct` increments `TimesCorrect`
- `Mark Incorrect` increments `TimesIncorrect`
- Due-card loading prioritizes cards with no attempts or weaker performance trends

## Training Data Seeding

- Static seed source is stored in `StudyTide Forge/Data/TrainingSeedCatalog.cs`.
- Only question and answer text are used.
- Each pair creates:
  - one `Flashcard`
  - one `TrainingBlock` with clean structured content:
    - `Question: ...`
    - `Answer: ...`
- The seed contains more than 50 flashcards.

## Project Structure

```text
StudyTide Forge/
  Components/
    App.razor
    Routes.razor
    Layout/
    Pages/
  Data/
    ForgeDbContext.cs
    ForgeDbContextFactory.cs
    DatabaseInitializer.cs
    TrainingSeedCatalog.cs
    Migrations/
  Models/
  Services/
  wwwroot/css/app.css
  Program.cs
  StudyTideForge.csproj
  appsettings.json
  appsettings.Development.json
```

## Run Locally

From repository root:

```powershell
dotnet restore
dotnet tool restore
dotnet dotnet-ef database update --project ".\StudyTide Forge\StudyTideForge.csproj" --startup-project ".\StudyTide Forge\StudyTideForge.csproj"
dotnet run --project ".\StudyTide Forge\StudyTideForge.csproj"
```

## EF Core Migration Commands

Create a new migration:

```powershell
dotnet dotnet-ef migrations add <MigrationName> --project ".\StudyTide Forge\StudyTideForge.csproj" --startup-project ".\StudyTide Forge\StudyTideForge.csproj" --output-dir Data\Migrations
```

Apply migrations:

```powershell
dotnet dotnet-ef database update --project ".\StudyTide Forge\StudyTideForge.csproj" --startup-project ".\StudyTide Forge\StudyTideForge.csproj"
```

## Git Workflow

If starting from a fresh local repository:

```powershell
git init
git add .
git commit -m "Initial release - StudyTide Forge v1.0"
git branch -M main
```

If repository already exists, skip `git init`.

## Create GitHub Repository

1. Create an empty repository on GitHub.
2. Copy the repository URL.
3. Run:

```powershell
git remote add origin https://github.com/<your-user>/<your-repo>.git
git push -u origin main
git tag v1.0
git push origin v1.0
```

## Publish

```powershell
dotnet publish ".\StudyTide Forge\StudyTideForge.csproj" -c Release -o ".\artifacts\publish"
```

## Azure App Service Deployment

1. Create Azure resources (resource group + App Service Plan + Web App).
2. Deploy published output.

Example CLI flow:

```powershell
az login
az account set --subscription "<subscription-id>"
az group create --name "<rg-name>" --location "eastus"
az appservice plan create --name "<plan-name>" --resource-group "<rg-name>" --sku B1 --is-linux
az webapp create --name "<unique-app-name>" --resource-group "<rg-name>" --plan "<plan-name>" --runtime "DOTNETCORE:8.0"
az webapp deploy --resource-group "<rg-name>" --name "<unique-app-name>" --src-path ".\artifacts\publish.zip" --type zip
```

## Environment Variables

Typical App Service settings:

- `ASPNETCORE_ENVIRONMENT=Production`
- `ConnectionStrings__ForgeDb=<optional override connection string>`

Local defaults use:

- `ConnectionStrings:ForgeDb=Data Source=studytide-forge.db`

## SQLite Deployment Considerations

- SQLite is file-based and best for low-concurrency or single-instance scenarios.
- For App Service scale-out, shared write access to one local SQLite file is not suitable.
- For production scale or multi-instance hosting, move to a managed relational database.
- If using SQLite in App Service anyway, pin to a single instance and persist the DB file to mounted storage.

## Screenshots

- Dashboard: _placeholder_
- Modules: _placeholder_
- Practice: _placeholder_
- Flashcards: _placeholder_
