# Changelog

All notable changes to this project are documented in this file.

The format is based on semantic versioning (`MAJOR.MINOR.PATCH`).

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