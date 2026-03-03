# Release Notes

## v2.8.0 (2026-03-03)

This release is **ready for public release**.

### Highlights

- Fixed Training Materials navigation active state so `Study Library` no longer appears selected on `Flashcards` or `Flagged Material`.
- Added `Reset` actions in Study Library next to `Start` and `Resume` to clear checkpoint progress for a lesson.
- Prompt normalization now removes malformed lead phrasing (`Identify the term: This Use...`, `Identify the term: This 1...`) and rewrites existing records at startup.
- Example regeneration now removes malformed patterns (`use This ...`, `when where ...`) while preserving concrete contextual examples.
- Updated lesson-study layout so scrolling stays inside the study panels (slide content + outline), instead of scrolling the whole page.
- Verified combined seeded inventory remains `6972` entries:
  - `3488` training items
  - `3484` flashcards

### Local Publish Outputs

- Desktop publish target:
  - `StudyTide Forge\bin\Release\net10.0-windows10.0.19041.0\win-x64\StudyTideForge.exe`
- Web publish target:
  - `artifacts\publish\studytide-forge-web-win-x64\StudyTideForgeWeb.exe`
