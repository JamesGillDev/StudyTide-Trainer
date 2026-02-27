# Changelog

All notable changes to this project are documented in this file.

The format is based on semantic versioning (`MAJOR.MINOR.PATCH`).

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
