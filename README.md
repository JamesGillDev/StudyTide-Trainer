# StudyTide Forge

StudyTide Forge is structured cognitive reinforcement training with verbatim retyping, flashcards, spaced repetition, and importer-based curriculum seeding.

## Solution Layout

This repository now contains two **parallel** apps:

1. `StudyTide Forge`  
   Primary desktop app (`.NET MAUI Blazor Hybrid`, local runtime).
2. `StudyTide Forge Web`  
   Completely separate web app (`.NET 8 Blazor Server`) intended for container hosting.

Both use EF Core + SQLite and the same domain model terminology (`TrainingModule`, `TrainingLesson`, `TrainingItem`, `Flashcard`, `PracticeAttempt`).

## Importer Workflow

Importer file: `Data/LegacyQaSourceImporter.cs`

Runtime behavior:

1. Reads a legacy shared C# source file at startup.
2. Extracts only question/answer string literals.
3. Drops blanks and deduplicates exact duplicates.
4. Reverses direction for reinforcement:
   - `Flashcard.Question` <- legacy answer text
   - `Flashcard.Answer` <- legacy question text
5. Builds `TrainingItem.Content` as:
   - `Prompt:`
   - `Response:`
   - `Example:`
6. Distributes entries across multiple modules/lessons and repairs blank-module states during reseed.

## Source File Placement

Set environment variable:

```powershell
$env:FORGE_IMPORT_SOURCE_FILE = "C:\path\to\legacy\Shared\legacy-source.cs"
```

For the web container build, a bundled copy is included at:

`StudyTide Forge Web/seed-source/legacy-source.cs`

## Dashboard Metrics

Dashboard includes:

- `Imported flashcards: X`
- `Imported training items: Y`

## Spaced Repetition Rules

- `100%` accuracy -> next due in `3 days`
- `>=95%` accuracy -> next due in `1 day`
- `<95%` accuracy -> next due in `1 hour`

## Run Local Desktop App

```powershell
dotnet restore
dotnet tool restore
dotnet dotnet-ef database update --project ".\StudyTide Forge\StudyTideForge.csproj" --startup-project ".\StudyTide Forge\StudyTideForge.csproj" --framework net10.0-windows10.0.19041.0
dotnet run --project ".\StudyTide Forge\StudyTideForge.csproj" --framework net10.0-windows10.0.19041.0
```

## Build Local Desktop `.exe` (Release)

```powershell
dotnet publish ".\StudyTide Forge\StudyTideForge.csproj" -c Release -f net10.0-windows10.0.19041.0 -r win-x64
```

Primary desktop executable:

`StudyTide Forge\bin\Release\net10.0-windows10.0.19041.0\win-x64\StudyTideForge.exe`

Published executable:

`StudyTide Forge\bin\Release\net10.0-windows10.0.19041.0\win-x64\publish\StudyTideForge.exe`

### Windows Icon Cache Note

If File Explorer still shows the old/default icon after rebuilding, clear icon cache:

```powershell
Stop-Process -Name explorer -Force
Remove-Item "$env:LOCALAPPDATA\Microsoft\Windows\Explorer\iconcache*" -Force -ErrorAction SilentlyContinue
Start-Process explorer.exe
```

## Run Local Web App (Separate Project)

```powershell
dotnet restore ".\StudyTide Forge Web\StudyTide Forge Web.csproj"
dotnet run --project ".\StudyTide Forge Web\StudyTide Forge Web.csproj"
```

Default local URL from launch settings: `http://localhost:5173`

## Build Local Web `.exe` (Release)

```powershell
dotnet publish ".\StudyTide Forge Web\StudyTide Forge Web.csproj" -c Release -r win-x64 --self-contained false -o ".\artifacts\publish\studytide-forge-web-win-x64"
```

Web executable:

`artifacts\publish\studytide-forge-web-win-x64\StudyTideForgeWeb.exe`

## Container Image (Separate and Parallel)

The web app is published as a standalone container image:

- Image: `ghcr.io/jamesgilldev/studytide-forge-web:latest`
- Package page (share this in Teams):  
  `https://github.com/JamesGillDev/StudyTide-Trainer/pkgs/container/studytide-forge-web`

Pull and run:

```powershell
docker pull ghcr.io/jamesgilldev/studytide-forge-web:latest
docker run -d --name studytide-forge-web -p 8080:8080 -v studytide-forge-web-data:/data ghcr.io/jamesgilldev/studytide-forge-web:latest
```

Or with compose:

```powershell
docker compose -f .\docker-compose.web.yml up -d
```

Then browse: `http://localhost:8080`

## GHCR Publishing Automation

Workflow: `.github/workflows/publish-web-container.yml`

- Triggers on push to `main` for web-container paths.
- Builds from `StudyTide Forge Web/Dockerfile`.
- Pushes:
  - `ghcr.io/<owner>/studytide-forge-web:latest`
  - `ghcr.io/<owner>/studytide-forge-web:sha-<short>`

## EF Core Commands

Desktop project:

```powershell
dotnet dotnet-ef migrations add <MigrationName> --project ".\StudyTide Forge\StudyTideForge.csproj" --startup-project ".\StudyTide Forge\StudyTideForge.csproj" --framework net10.0-windows10.0.19041.0 --output-dir Data\Migrations
dotnet dotnet-ef database update --project ".\StudyTide Forge\StudyTideForge.csproj" --startup-project ".\StudyTide Forge\StudyTideForge.csproj" --framework net10.0-windows10.0.19041.0
```

Web project:

```powershell
dotnet ef migrations add <MigrationName> --project ".\StudyTide Forge Web\StudyTide Forge Web.csproj" --startup-project ".\StudyTide Forge Web\StudyTide Forge Web.csproj" --output-dir Data\Migrations
dotnet ef database update --project ".\StudyTide Forge Web\StudyTide Forge Web.csproj" --startup-project ".\StudyTide Forge Web\StudyTide Forge Web.csproj"
```

## Git and Release Commands

```powershell
git add .
git commit -m "Add separate StudyTide Forge Web container app and GHCR pipeline"
git push origin main
git tag -a v1.0 -m "StudyTide Forge v1.0"
git push origin v1.0
```

## Azure App Service (Container) Steps

Deploy the separate web container image:

1. Create resource group and App Service plan:
   ```powershell
   az group create --name <rg> --location <region>
   az appservice plan create --name <plan> --resource-group <rg> --sku B1 --is-linux
   ```
2. Create web app from container:
   ```powershell
   az webapp create --resource-group <rg> --plan <plan> --name <app-name> --deployment-container-image-name ghcr.io/jamesgilldev/studytide-forge-web:latest
   ```
3. Configure app settings:
   ```powershell
   az webapp config appsettings set --resource-group <rg> --name <app-name> --settings ASPNETCORE_ENVIRONMENT=Production FORGE_WEB_DB_PATH=/home/site/data/studytide-forge-web.db
   ```
4. If registry auth is required, set container registry credentials for GHCR (or use public package visibility).

### SQLite Notes

- Persist DB using a mounted volume (`/data`) in Docker.
- App Service local filesystem can be ephemeral.
- For scaled production, prefer a server database instead of SQLite.
