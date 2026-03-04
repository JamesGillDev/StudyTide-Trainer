# StudyTide Forge

StudyTide Forge is structured cognitive reinforcement training for software engineers, software developers, and cloud application developers. It combines verbatim retyping, flashcards, spaced repetition, and importer-based curriculum seeding.

## Release Status

StudyTide Forge desktop (`v2.10.5`) is marked **in Public Release** as of `2026-03-04`.
Public release verification refreshed on `2026-03-04` after local desktop/web publish validation.

## Solution Layout

This repository now contains two **parallel** apps:

1. `StudyTide Forge`  
   Primary desktop app (`.NET MAUI Blazor Hybrid`, local runtime).
2. `StudyTide Forge Web`  
   Completely separate web app (`.NET 8 Blazor Server`) intended for container hosting.

Both use EF Core + SQLite and the same domain model terminology (`TrainingModule`, `TrainingLesson`, `TrainingItem`, `Flashcard`, `PracticeAttempt`).

## Source Import Workflow

Importer file: `Data/LegacyQaSourceImporter.cs`

Runtime behavior:

1. Reads a shared C# source file at startup.
2. Extracts only question/answer string literals.
3. Drops blanks and deduplicates exact duplicates.
4. Reverses direction for reinforcement:
   - `Flashcard.Question` <- source answer text
   - `Flashcard.Answer` <- source question text
5. Builds `TrainingItem.Content` as:
   - `Prompt:`
   - `Response:`
   - `Study Cue:` (stored in the `Example` field for compatibility)
6. Distributes entries across multiple modules/lessons and repairs blank-module states during reseed.

## Training Material Extraction (MSSA Source Files)

Extraction command:

```powershell
py .\tools\extract_training_material.py
```

Current extraction output (`2026-03-03`):

- `3699` deduplicated training pairs generated from the provided MSSA PDFs/Docx/Xlsx files plus `Training Notes.zip` archive documents.
- OCR fallback applied to image-only pages (notably `az2006_portrait_dark_teal.pdf`).
- Generated artifacts:
  - `StudyTide Forge\seed-source\legacy-source.cs`
  - `StudyTide Forge Web\seed-source\legacy-source.cs`
  - `App_Data\training-material\extracted-training-pairs.json`
  - `App_Data\training-material\extraction-report.md`

## Study Library Workflow

- `Training Materials` navigation now uses nested sections:
  - `Study Library`
  - `Flashcards`
  - `Flagged Material`
- Study Library shows lesson/module progress state:
  - `Not Started`
  - `In Progress`
  - `Completed`
- Resume support is persistent:
  - Module-level and global `Resume` buttons jump to the exact lesson slide last viewed.
  - Checkpoints auto-save on every `Previous`, `Next`, and outline jump.
- Lesson study view is slide-first with an outline panel for instant navigation.
- Keyboard navigation is enabled in study view:
  - `ArrowLeft` -> previous slide
  - `ArrowRight` -> next slide
- Practice now includes an `Expanded Context` section derived from Prompt/Response/Study Cue while preserving strict verbatim scoring against section values.

## Concrete Example Refresh

- Training item examples were regenerated to remove the old generic `When practicing...` template.
- Current seeded inventory now contains concrete examples across all combined entries:
  - `3702` training items
  - `3698` flashcards
  - `7400` combined entries
- Example migration runs at startup and rewrites existing training item `Example` sections from Prompt/Response context when needed.
- Study-view action buttons use a fixed minimum height for consistent visual alignment (`Previous`, `Next`, `Practice`, `Flashcards`).

## v2.10.2 Azure Supplemental Pack + Flashcard Sync

- Added `38` Azure supplemental seed blocks from the requested Azure Container Apps, Azure Container Registry, Azure DevOps, Azure Identity, Azure Networking, and Azure Security topics.
- Mapped all new blocks into the existing `Azure` module category while preserving lesson titles and concept prompts.
- Updated seeding to add matching flashcards for supplemental/generated seed blocks when missing, so training-item and flashcard coverage stay aligned.
- Deduplicated overlapping ingress prompt input during import-to-seed translation to avoid redundant same-lesson/same-title collisions.

## v2.10.3 Study Library Outline Auto-Hide

- Lesson Study now opens with the Outline hidden by default.
- Outline visibility is reset to hidden each time a lesson page loads so focus stays on Prompt/Response/Study Cue content.
- `Show Outline` remains available for manual expansion at any time.

## v2.10.5 Practice Session Controls + Settings + Media Sidebar

- Added a session progress bar directly below `Verbatim Practice`.
- Added `Reset` next to `Scope` with a warning bubble (`!` icon + `Reset Now`) and blurred backdrop.
- Moved `Random` into the main Practice action row (`Submit Attempt`, `Back`, `Skip`, `Random`, `Next`).
- Added bottom-left settings gear and moved Quick Start guidance from the old `i` helper into Settings.
- Added Settings controls for:
  - show/hide launch tip banner
  - enable/disable sidebar media player
  - enable/disable bundled Alpha Waves concentration track
  - live volume slider
- Added sidebar media player (enabled via Settings) with:
  - `Play`, `Pause`, `Prev`, `Next`
  - scrolling now-playing title
  - multi-file `Browse` picker for local audio/video files
  - playlist controls (`Select All`, `Deselect All`, remove selected, move up/down, `Play Next`, `+` queue)
  - queued-items list management
  - auto-hidden mini-screen for audio-only tracks (visible only for video tracks)
- Standardized new overlays/pop-ups to use blurred backdrop styling for visual consistency.

## v2.10.4 Practice Navigation + Random Lock + Summary Modal

- Replaced `Load Another Question` with `Next` in Practice workflow actions.
- Added `Random` mode in Practice scope controls with locked non-repeating question rotation on `Skip` and `Next` until the random lock set is exhausted.
- Renamed `Diff Summary` to `Summary`.
- Updated `Submit Attempt` behavior to open a compact summary bubble with blurred background and key accuracy/mismatch metrics.
- Added `Next` action inside the summary bubble to close the modal and immediately load the next question using the active mode (`Due` or `Random Lock`).

## v2.10.1 Study Library Cleanup + Footer Pinning

- Removed explicit non-training artifact `"A practical arc for CAD Month 1-2"` from both Study Library and Flashcards through startup cleanup.
- Added persistent targeted removal logic so this artifact does not reappear after reseed/import.
- Updated lesson study slide layout so `Previous`, `Next`, `Practice`, and `Flashcards` stay pinned in a fixed footer position while slide content scrolls independently.

## v2.10.0 Archive Ingestion + Coverage Completion

- Added archive-aware extraction to ingest supported training files from `Training Notes.zip` (`.pdf`, `.docx`, `.xlsx`) in addition to direct file paths.
- Extraction remains deduplicated and now outputs `3699` unique pairs for import seeding.
- Added Azure coverage target support (`500`) with Azure-specific generated reinforcement content for any future category gap backfill.
- Verified fresh reseed snapshot from the updated import source:
  - `3699` flashcards
  - `3703` training items
  - `574` Azure items and `437` System Design items (both above minimum target).

## v2.9.1 Content and Layout Polish

- Compacted Study Library lesson header spacing (`Imported Set...` + module/status row) to show more training content above the fold.
- Added generated-title cleanup migration to remove `Refined - ...` / `Focused - ...` prefixes from seeded lesson titles/content.
- Renamed UI labels from `Example` to `Study Cue` where examples are cue-style restatements.
- Renamed `Expanded Context` bullet label from `Application` to `Study cue` for wording accuracy.

## v2.9.2 Study Cue Phrasing Cleanup

- Removed Study Cue boilerplate phrasing that did not match training voice:
  - `In a real project, apply ...`
  - auto-injected `when ...` / `when you need to ...`
- Study cues now render with direct cue phrasing and strip duplicated term prefixes for cleaner grammar.

## v2.9.3 Dashboard Auto-Sync and Content Sweep

- Dashboard metrics now auto-refresh every 15 seconds while the dashboard is open, so totals/progress stay current after training content is added, deleted, or altered.
- Added broader non-training material filtering and cleanup for imported content (for example document headings/schedule labels such as `MSSA ...`, `Week ...`, `Morning/Afternoon`, `Appendix`, and CAD manual title artifacts).
- Startup cleanup now removes those non-training items from both Study Library and Flashcards, then removes empty lessons/modules caused by cleanup.

## Study UX and Grammar Cleanup

- Fixed Training Materials nav state so `Study Library` is no longer highlighted while viewing `Flashcards` or `Flagged Material`.
- Added `Reset` controls in Study Library next to `Start`/`Resume` to clear per-lesson checkpoint progress back to `Not Started`.
- Updated prompt normalization so forced stems like `Identify the term: This Use...` and `Identify the term: This 1...` are rewritten into clean phrasing.
- Regenerated examples with stricter term/action extraction so malformed patterns like `In code, use This Use...` are removed.
- Updated lesson study layout to keep scrolling inside the slide/outline panels instead of scrolling the full page.

## Dashboard and Study View Refinements

- Lesson Study now supports hiding the `Outline` panel with a toggle to expand `Prompt`, `Response`, and `Study Cue` content to full width.
- Dashboard now removes redundant overlap/mirroring inventory math and focuses on core readiness metrics.
- Added completion progress bars on Dashboard for:
  - `Study Library` (completed lessons)
  - `Flashcards` (reviewed cards)
  - `Practice` (practiced training items)

## Verbatim Practice Behavior

- Practice displays `Prompt`, `Response`, and `Study Cue` as section labels in the UI.
- Retyping and accuracy scoring are performed against the section text only (labels are excluded).
- Users retype section values in order: `Prompt` text, then `Response` text, then `Study Cue` text.
- Practice actions include `Submit Attempt`, `Back`, `Skip`, `Random`, and `Next`.
- On first app launch, a quick-start instruction bubble explains the workflow and can be reopened from the bottom-left settings gear.
- Character-level scoring uses edit-distance logic, so one missing character/newline no longer collapses the entire score.
- Prompt/response normalization removes malformed forced question stems (for example `What is is ...`) and uses clean term-identification phrasing.

## Curriculum Coverage Dashboard

Dashboard now includes:

- Total modules, lessons, retype items, and flashcards.
- Due-item metrics and recent practice accuracy.
- Category coverage table with recommended minimum training-item targets for software/cloud roles.
- Additional-item gap counts to show where more content is needed.
- Duplicate-sweep repair behavior so repeated training items are automatically replaced with unique content.
- Explicit relationship metrics:
  - Combined mode entries (`retype + flashcards`)
  - Unique logical items
  - Items in both modes
  - Retype-only items
  - Flashcard-only items
  - Mirroring status percentage (fully or partially mirrored)

### Current Local Inventory Snapshot (Desktop DB)

As measured on `2026-03-03` from a fresh reseed validation run using the current bundled source:

- `6` modules
- `41` lessons
- `3703` training (retype) items
- `3699` flashcards
- `7402` combined mode entries (`3703 + 3699`)

## Source File Placement

Set environment variable (optional override):

```powershell
$env:FORGE_IMPORT_SOURCE_FILE = "C:\path\to\legacy\Shared\legacy-source.cs"
```

Bundled defaults (used automatically when override is not set):

- Desktop: `StudyTide Forge/seed-source/legacy-source.cs`
- Web: `StudyTide Forge Web/seed-source/legacy-source.cs`

## Dashboard Metrics

Dashboard includes:

- `Imported flashcards: X`
- `Imported training items: Y`
- `Category coverage and target gap` by curriculum area
- `How totals relate` panel with explicit formulas and mirroring status

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

`StudyTide Forge\bin\Release\net10.0-windows10.0.19041.0\win-x64\StudyTideForge.exe`

Preferred fixed local output path:

```powershell
dotnet publish ".\StudyTide Forge\StudyTideForge.csproj" -c Release -f net10.0-windows10.0.19041.0 -r win-x64 -o "C:\MSSA Code-github\StudyTide Trainer\StudyTide Forge\bin\Release\net10.0-windows10.0.19041.0\win-x64"
```

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
