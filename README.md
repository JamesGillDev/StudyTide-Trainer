# StudyTide Forge

StudyTide Forge is a structured cognitive reinforcement app for verbatim retyping and flashcard recall.

## Purpose

- Encode technical knowledge through exact typing practice.
- Reinforce recall with reversed flashcards and contextual examples.
- Track weak areas with measurable training stats.

## Architecture

- .NET 10
- MAUI Blazor Hybrid (local desktop runtime)
- EF Core 8 + SQLite
- Single DbContext: `ForgeDbContext`

## Importer Workflow

Importer file: `Data/LegacyQaSourceImporter.cs`

Runtime behavior:

1. Reads a legacy shared source file from disk at app startup.
2. Extracts only question and answer string literals.
3. Drops blank entries and deduplicates exact duplicates.
4. Reverses imported direction for flashcards:
   - `Flashcard.Question` <- legacy answer text
   - `Flashcard.Answer` <- legacy question text
5. Creates training items with this content format:
   - `Prompt:`
   - `Response:`
   - `Example:`
6. Distributes content across multiple modules and multiple lessons per module.
7. Repairs blank-module states by reseeding when required.

## Source File Placement

Preferred:

```powershell
$env:FORGE_IMPORT_SOURCE_FILE = "C:\path\to\legacy\Shared\<your-source-file>.cs"
```

Fallback search also scans parent folders for files ending with:

```text
GameService.Shared.cs
```

## Imported Modules

Seeding targets these modules:

- `Legacy Import - C# Skill Forge`
- `Legacy Import - Azure Skill Forge`
- `Legacy Import - SQL Skill Forge`
- `Legacy Import - DevOps Skill Forge`
- `Legacy Import - System Design Skill Forge`
- `Legacy Import - Behavioral Skill Forge`

Dashboard shows:

- `Imported flashcards: X`
- `Imported training items: Y`

## Spaced Repetition Rules

Practice scheduling:

- `100%` -> due in `3 days`
- `>=95%` -> due in `1 day`
- `<95%` -> due in `1 hour`

## Run Locally

```powershell
dotnet restore
dotnet tool restore
dotnet dotnet-ef database update --project ".\StudyTide Forge\StudyTideForge.csproj" --startup-project ".\StudyTide Forge\StudyTideForge.csproj" --framework net10.0-windows10.0.19041.0
dotnet run --project ".\StudyTide Forge\StudyTideForge.csproj" --framework net10.0-windows10.0.19041.0
```

## EF Core Commands

Add migration:

```powershell
dotnet dotnet-ef migrations add <MigrationName> --project ".\StudyTide Forge\StudyTideForge.csproj" --startup-project ".\StudyTide Forge\StudyTideForge.csproj" --framework net10.0-windows10.0.19041.0 --output-dir Data\Migrations
```

Apply migration:

```powershell
dotnet dotnet-ef database update --project ".\StudyTide Forge\StudyTideForge.csproj" --startup-project ".\StudyTide Forge\StudyTideForge.csproj" --framework net10.0-windows10.0.19041.0
```

## Git Commands

```powershell
git init
git add .
git commit -m "Initial release - StudyTide Forge v1.0"
```

Push:

```powershell
git branch -M main
git remote add origin https://github.com/<your-account>/<your-repo>.git
git push -u origin main
```

Create release tag:

```powershell
git tag -a v1.0 -m "StudyTide Forge v1.0"
git push origin v1.0
```

## Azure App Service Steps

For a web-host deployment variant:

1. Publish:
   ```powershell
   dotnet publish ".\StudyTide Forge\StudyTideForge.csproj" -c Release -o ".\artifacts\publish"
   Compress-Archive -Path ".\artifacts\publish\*" -DestinationPath ".\artifacts\publish.zip" -Force
   ```
2. Provision:
   ```powershell
   az group create --name <rg> --location <region>
   az appservice plan create --name <plan> --resource-group <rg> --sku B1 --is-linux
   az webapp create --name <app-name> --resource-group <rg> --plan <plan> --runtime "DOTNETCORE|8.0"
   ```
3. Deploy package:
   ```powershell
   az webapp deploy --resource-group <rg> --name <app-name> --src-path ".\artifacts\publish.zip" --type zip
   ```
4. Configure settings:
   - `ASPNETCORE_ENVIRONMENT=Production`
   - `FORGE_IMPORT_SOURCE_FILE=<absolute-path-to-legacy-source-file>`

### SQLite Notes (App Service)

- Local filesystem may be ephemeral.
- Use mounted persistent storage for database durability.
- For multi-instance production, move to a server database.
