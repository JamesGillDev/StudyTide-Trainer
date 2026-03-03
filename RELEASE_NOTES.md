# Release Notes

## v2.6.0 (2026-03-03)

This release is **ready for public release**.

### Highlights

- Added `Study Library` with persistent lesson/module status (`Not Started`, `In Progress`, `Completed`).
- Added module-level and global `Resume` actions to continue exactly where the user left off.
- Added slide-based lesson study experience with:
  - one item at a time (`Prompt`, `Response`, `Example`)
  - `Previous` / `Next` controls
  - left/right keyboard navigation
  - outline jump panel for instant navigation
- Moved `Flashcards` and `Flagged Material` into nested `Training Materials` navigation.
- Updated Practice view with `Expanded Context` while preserving strict verbatim scoring behavior.
- Added persistent checkpoint model/service support (`StudyLessonProgress`, `StudyLibraryService`) and startup schema extension checks for desktop/web compatibility.

### Local Publish Outputs

- Desktop publish target:
  - `StudyTide Forge\bin\Release\net10.0-windows10.0.19041.0\win-x64\StudyTideForge.exe`
- Web publish target:
  - `artifacts\publish\studytide-forge-web-win-x64\StudyTideForgeWeb.exe`
