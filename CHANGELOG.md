# Changelog

All notable changes to this project are documented in this file.

The format is based on semantic versioning (`MAJOR.MINOR.PATCH`).

## [2.9.0] - 2026-03-03

### Added
- Lesson Study Outline visibility toggle:
  - `Hide Outline` / `Show Outline` control in lesson view.
  - full-width slide layout when outline is hidden.
- New dashboard completion progress bars for:
  - `Study Library`
  - `Flashcards`
  - `Practice`

### Changed
- Dashboard now removes redundant overlap/mirroring summary metrics and keeps the readiness view focused on actionable totals, due counts, and completion status.
- Study layout styles updated so outline-hidden mode expands prompt/response/example content horizontally.
- App version bumped to `2.9.0` (`ApplicationVersion` `2900`).

### Release
- Marked `v2.9.0` as **ready for public release** on `2026-03-03`.

## [2.8.0] - 2026-03-03

### Added
- Study Library lesson reset workflow:
  - added `Reset` button next to `Start`/`Resume` actions.
  - added persisted reset service path (`ResetLessonProgressAsync`) to clear lesson checkpoints back to `Not Started`.

### Changed
- Navigation active-state logic for Training Materials now correctly isolates:
  - `Study Library`
  - `Flashcards`
  - `Flagged Material`
  so `Study Library` is no longer highlighted while viewing `Flashcards` or `Flagged Material`.
- Prompt normalization now removes malformed `Identify the term: This ...` lead artifacts and rephrases list/instruction stems for cleaner grammar.
- Concrete example regeneration now uses improved term/action extraction to remove malformed forms like `use This Use...` and `when where...`.
- Lesson study layout now constrains scrolling to study panels (slide + outline) instead of scrolling the entire page.
- App version bumped to `2.8.0` (`ApplicationVersion` `2800`).

### Release
- Marked `v2.8.0` as **ready for public release** on `2026-03-03`.

## [2.7.0] - 2026-03-03

### Added
- Concrete example regeneration pipeline for training items:
  - replaced generic `When practicing...` example text with contextual, usage-oriented examples derived from prompt/response content.
  - applied to existing datasets through startup migration logic (`ApplyConcreteExampleMigrationAsync`).

### Changed
- Updated training-content example generation (`BuildExample`) to always create concrete usage examples for newly seeded entries.
- Lesson-study action controls (`Previous`, `Next`, `Practice`, `Flashcards`) now share a fixed minimum button height for consistent UI alignment.
- App version bumped to `2.7.0` (`ApplicationVersion` `2700`).

### Release
- Marked `v2.7.0` as **ready for public release** on `2026-03-03`.

## [2.6.0] - 2026-03-03

### Added
- New `Study Library` experience with persistent progress states across modules/lessons:
  - `Not Started`
  - `In Progress`
  - `Completed`
- New slide-based `Lesson Study` view with:
  - One training item at a time (`Prompt`, `Response`, `Example`)
  - `Previous` / `Next` navigation
  - Left/right keyboard arrow navigation
  - Outline panel for instant jump to any item
- Persistent lesson checkpoint storage via `StudyLessonProgress` model and `StudyLibraryService`.
- New nested training navigation structure:
  - `Training Materials > Study Library`
  - `Training Materials > Flashcards`
  - `Training Materials > Flagged Material`

### Changed
- Practice view now includes an `Expanded Context` section derived from Prompt/Response/Example while keeping verbatim scoring behavior unchanged.
- `Flashcards` and `Flagged Material` routes now support `/modules/flashcards` and `/modules/flagged` in addition to legacy paths.
- Desktop and web data initialization now ensure extended schema compatibility for persistent study progress and lesson-flag support.
- App version bumped to `2.6.0` (`ApplicationVersion` `2600`).

### Release
- Marked `v2.6.0` as **ready for public release** on `2026-03-03`.

## [2.5.0] - 2026-03-03

### Added
- New extraction pipeline script: `tools/extract_training_material.py` to ingest the provided MSSA training PDFs/Docx/Xlsx, apply OCR fallback when needed, and produce deduplicated Q/A seed material.
- Generated training data artifacts under `App_Data/training-material`:
  - `extracted-training-pairs.json`
  - `extraction-report.md`
- Bundled import source for desktop app at `StudyTide Forge/seed-source/legacy-source.cs` (in addition to web seed source).

### Changed
- Refreshed importer seed source (`legacy-source.cs`) with `3484` unique training pairs extracted from the supplied MSSA training document set.
- Desktop startup now auto-sets `FORGE_IMPORT_SOURCE_FILE` to bundled `seed-source/legacy-source.cs` when no override is defined.
- Desktop project now copies bundled seed source into build/publish output and excludes it from compile.
- App version bumped to `2.5.0` (`ApplicationVersion` `2500`).

### Release
- Marked `v2.5.0` as **ready for public release** on `2026-03-03`.

## [2.4.0] - 2026-03-02

### Added
- Lesson flagging support across Practice, Flashcards, Module Details, and Lesson Details.
- New `Flagged Material` navigation section and page for focused review of flagged lessons.
- Practice navigation controls: `Back`, `Skip`, and `Load Another Question`.
- Flashcard navigation controls: `Back`, `Skip`, and modal-based `Reveal Answer` dialog.
- Dashboard differentiator metrics for content math clarity:
  - Combined mode entries
  - Unique logical items
  - Items in both modes
  - Retype-only items
  - Flashcard-only items
  - Mirroring status percentage
- Database migration `AddTrainingLessonFlag` for persistent lesson flags.

### Changed
- Prompt/response normalization now removes forced `What is ...` phrasing and repairs malformed question stems.
- Existing training items and flashcards are auto-normalized on startup for orientation and prompt grammar quality.
- Dashboard wording now explicitly distinguishes Practice retype inventory vs Flashcard inventory.
- App version bumped to `2.4.0` (`ApplicationVersion` `2400`).

## [2.3.1] - 2026-03-02

### Added
- Automated duplicate-sweep pass that repairs repeated training items by replacing duplicates with unique, category-aligned material.
- Targeted curriculum expansion for `C#` and `System Design` categories to satisfy dashboard readiness targets.
- Coverage-boost lesson tracks:
  - `Coverage Boost - C#`
  - `Coverage Boost - System Design`

### Changed
- Seeding pipeline now runs in this order: supplemental seeds, duplicate repair, then targeted coverage expansion.
- Training inventory now reaches target depth for both focus categories:
  - `C#`: `450` training items
  - `System Design`: `300` training items
- App version bumped to `2.3.1` (`ApplicationVersion` `2301`).

## [2.3.0] - 2026-03-02

### Added
- First-launch in-app instruction bubble with a floating help button to reopen guidance.
- Practice workflow panel that explains the required retype sequence step-by-step.
- Dashboard curriculum coverage table with category targets, coverage percentages, and additional-item gap counts.
- Dashboard inventory metrics for total lessons, total training items, and total flashcards.

### Changed
- Refreshed desktop and web UI styling for a clearer, more professional training workflow.
- Practice page now shows explicit line-order guidance and live expected/typed character counters.
- Dashboard focus updated to software-engineering and cloud-application readiness framing.
- Module naming migrated from `Legacy Import - ...` to `Training Track - ...` with startup auto-migration for existing databases.
- Importer-facing error wording updated to remove user-facing `Legacy` terminology.
- App version bumped to `2.3.0` (`ApplicationVersion` `2300`).

## [2.2.2] - 2026-02-27

### Changed
- Updated Practice page so `Prompt`, `Response`, and `Example` labels are display-only and no longer part of verbatim retype scoring.
- Added structured section rendering for training content in Practice UI to keep context visible without label retyping overhead.
- App version bumped to `2.2.2` (`ApplicationVersion` `2202`).

## [2.2.1] - 2026-02-27

### Added
- Release publish commands and output paths in README for local Windows desktop/web executables.
- Windows icon cache reset steps in README for local verification after icon updates.

### Changed
- Refreshed StudyTide Forge application icon assets with a new tide/spark design.
- App version bumped to `2.2.1` (`ApplicationVersion` `2201`).

## [2.2.0] - 2026-02-27

### Added
- Generated reinforcement examples for each imported training item.
- Multi-module import seeding with multiple lessons per module.

### Changed
- Reversed imported flashcard direction (`Question` now seeded from legacy answer text; `Answer` seeded from legacy question text).
- Training item content now uses `Prompt`, `Response`, and `Example` structure.
- Automatic reseed logic now repairs blank modules by rebuilding imported content across all target modules.
- User-facing text now uses `training item` in place of `block`.
- App version bumped to `2.2.0`.

## [2.1.0] - 2026-02-27

### Added
- Runtime legacy source importer (`LegacyQaSourceImporter`) that reads training Q/A from an external shared source file during seeding.
- Imported-content dashboard metrics: imported flashcard count and imported training block count.
- Import constants for stable module/lesson identity (`Legacy Q&A Pack`, `Imported Q&A`).

### Changed
- Database seeding now builds content from runtime import instead of hardcoded in-app seed arrays.
- Seeding now ensures one imported module/lesson with synchronized flashcard and training block counts.
- Navigation label updated to `Training Materials`.
- README updated with importer workflow, source placement guidance, EF commands, git/release commands, and App Service deployment steps.

### Removed
- Static hardcoded seed catalog (`TrainingSeedCatalog.cs`).

## [2.0.0] - 2026-02-27

### Added
- Windows MAUI Blazor Hybrid host (`App.xaml`, `MainPage.xaml`, `MauiProgram.cs`) for local desktop execution.
- Desktop bootstrap page (`wwwroot/index.html`) for BlazorWebView runtime.
- Windows platform files required for MAUI packaging/runtime.

### Changed
- Migrated hosting model from Blazor Server (Kestrel) to local-only desktop MAUI Blazor Hybrid.
- Updated `StudyTideForge.csproj` to MAUI single-project configuration targeting `net10.0-windows10.0.19041.0`.
- Updated startup to initialize EF Core/SQLite via MAUI DI and local app-data database path.
- Updated README with desktop run, migration, and publish instructions.
- Bumped application version to `2.0.0` / `2000`.

### Removed
- Web-server entrypoint (`Program.cs`) and browser-host-only startup flow.

## [1.1.0] - 2026-02-26

### Added
- Explicit assembly/application version metadata in `StudyTide Trainer.csproj`.
- Documented release-history workflow for future public releases.

### Changed
- Updated release process to support visible iteration milestones in GitHub Releases.

## [1.0.1] - 2026-02-26

### Added
- README deployment instructions for both local (non-Azure) and Azure App Service paths.

## [1.0.0] - 2026-02-26

### Added
- Initial public MVP of StudyTide Trainer.
- Blazor Server app with EF Core + SQLite persistence.
- Topic and snippet CRUD workflows.
- Verbatim retype practice mode with scoring and mismatch feedback.
- Scheduling rules for spaced review.
- Dashboard metrics and weakness analysis.
- Curated built-in training packs.
