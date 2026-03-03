# Release Notes

## v2.9.3 (2026-03-03)

This release is **ready for public release**.

### Highlights

- Dashboard now auto-refreshes metrics every 15 seconds while the page is open, so totals and completion bars stay in sync after content add/delete/update operations.
- Expanded non-training content sweep in startup cleanup to remove heading/schedule artifacts (for example `MSSA...`, `Week ...`, `Morning/Afternoon`, `Appendix`, and CAD manual title noise) from both Study Library items and Flashcards.
- Cleanup now also prunes empty lessons/modules produced by the non-training removal pass.

### Local Publish Outputs

- Desktop publish target:
  - `StudyTide Forge\bin\Release\net10.0-windows10.0.19041.0\win-x64\StudyTideForge.exe`
- Web publish target:
  - `artifacts\publish\studytide-forge-web-win-x64\StudyTideForgeWeb.exe`

## v2.9.2 (2026-03-03)

This release is **ready for public release**.

### Highlights

- Updated Study Cue generation to remove boilerplate phrasing that was mismatched with training material tone:
  - removed `In a real project, apply ...`
  - removed injected `when ...` / `when you need to ...`
- Study cues now generate as direct cue-style phrasing and trim duplicated term prefixes.
- Existing training items are refreshed by startup migration (`ApplyConcreteExampleMigrationAsync`) so stored Study Cue text is rewritten automatically.

### Local Publish Outputs

- Desktop publish target:
  - `StudyTide Forge\bin\Release\net10.0-windows10.0.19041.0\win-x64\StudyTideForge.exe`
- Web publish target:
  - `artifacts\publish\studytide-forge-web-win-x64\StudyTideForgeWeb.exe`

## v2.9.1 (2026-03-03)

This release is **ready for public release**.
Public release verification refreshed on `2026-03-03`.

### Highlights

- Reduced Lesson Study header height so `Imported Set...` and module metadata consume less vertical space.
- Removed generated `Refined - ...` / `Focused - ...` prefixes from seeded Study Library content through startup migration cleanup.
- Renamed `Example` display labels to `Study Cue` for grammar accuracy where content is reinforcement text rather than true scenario examples.
- Renamed Expanded Context `Application` bullet text to `Study cue`.

### Local Publish Outputs

- Desktop publish target:
  - `StudyTide Forge\bin\Release\net10.0-windows10.0.19041.0\win-x64\StudyTideForge.exe`
- Web publish target:
  - `artifacts\publish\studytide-forge-web-win-x64\StudyTideForgeWeb.exe`

## v2.9.0 (2026-03-03)

This release is **ready for public release**.

### Highlights

- Added `Hide Outline` / `Show Outline` in Lesson Study so users can expand Prompt/Response/Example content to full width while studying.
- Simplified Dashboard by removing redundant overlap/mirroring metric blocks and keeping core readiness metrics.
- Added Dashboard completion progress bars for:
  - `Study Library` (completed lessons)
  - `Flashcards` (reviewed cards)
  - `Practice` (practiced training items)
- Verified combined seeded inventory remains `6972` entries:
  - `3488` training items
  - `3484` flashcards

### Local Publish Outputs

- Desktop publish target:
  - `StudyTide Forge\bin\Release\net10.0-windows10.0.19041.0\win-x64\StudyTideForge.exe`
- Web publish target:
  - `artifacts\publish\studytide-forge-web-win-x64\StudyTideForgeWeb.exe`
