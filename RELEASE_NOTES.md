# Release Notes

## v2.10.8 (2026-03-04)

This release is **in Public Release**.
Public release verification refreshed on `2026-03-04` after local desktop/web publish.

### Highlights

- Added bundled Alpha Waves audio asset at:
  - `StudyTide Forge\wwwroot\audio\8_Hour_Alpha_Waves.wav`
  - `StudyTide Forge Web\wwwroot\audio\8_Hour_Alpha_Waves.wav`
- Updated default bundled Alpha Waves source path to `/audio/8_Hour_Alpha_Waves.wav`.
- Hardened media panel startup and shell-state update flows with defensive JS interop error handling to prevent first-open crashes.
- Removed fragile bundled-track source preflight checks and switched to direct playback attempts with clearer error/fallback behavior.
- Reworked sidebar media-player sizing and control layout:
  - wider left navigation rail
  - fixed one-line now-playing title
  - grid-based control groups tuned for sidebar width
  - improved playlist row sizing to avoid collapsed/overflowed controls and title rendering artifacts.

### Local Publish Outputs

- Desktop publish target:
  - `StudyTide Forge\bin\Release\net10.0-windows10.0.19041.0\win-x64\StudyTideForge.exe`
- Web publish target:
  - `artifacts\publish\studytide-forge-web-win-x64\StudyTideForgeWeb.exe`

## v2.10.7 (2026-03-04)

This release is **in Public Release**.
Public release verification refreshed on `2026-03-04` after local desktop/web publish.

### Highlights

- Fixed media source setup for local playlist files so `.mp3` and other audio selections are passed with explicit MIME hints during playback startup.
- Added guardrails for missing bundled Alpha Waves source:
  - remove unavailable bundled track from active playback path
  - disable Alpha Waves toggle state when source is unavailable
  - show guidance to add local `.mp3` files via `Browse`
- Reworked playlist row layout in the sidebar media panel so controls wrap cleanly and long track titles no longer collapse vertically or overflow horizontally.
- Updated audio mini-screen hiding to collapse the visual frame while keeping media playback element behavior stable for audio-only tracks.

### Local Publish Outputs

- Desktop publish target:
  - `StudyTide Forge\bin\Release\net10.0-windows10.0.19041.0\win-x64\StudyTideForge.exe`
- Web publish target:
  - `artifacts\publish\studytide-forge-web-win-x64\StudyTideForgeWeb.exe`

## v2.10.6 (2026-03-04)

This release is **in Public Release**.
Public release verification refreshed on `2026-03-04` after local republish in updated VS Code.

### Highlights

- Added persistent app settings via local JSON storage so shell preferences survive app restarts:
  - launch tip banner visibility
  - sidebar media player visibility
  - Alpha Waves on/off
  - media volume
- Added a centralized `ModalShell` component and migrated popup dialogs to it for consistent spacing, blur backdrop behavior, and shared action layout.
- Added explicit media capability note in settings and media player:
  - file selection supports any audio/video type
  - playback support depends on codecs available on the OS/browser engine.

### Local Publish Outputs

- Desktop publish target:
  - `StudyTide Forge\bin\Release\net10.0-windows10.0.19041.0\win-x64\StudyTideForge.exe`
- Web publish target:
  - `artifacts\publish\studytide-forge-web-win-x64\StudyTideForgeWeb.exe`

## v2.10.5 (2026-03-04)

This release is **in Public Release**.

### Highlights

- Added a Practice session progress bar below the `Verbatim Practice` header.
- Added a `Reset` control next to `Scope` with a warning bubble (`!` indicator), blurred backdrop, and explicit `Reset Now` confirmation action.
- Moved `Random` into the main Practice action row with `Submit Attempt`, `Back`, `Skip`, and `Next`.
- Added bottom-left settings gear and moved prior quick-start/help guidance into the Settings modal.
- Added Settings controls for launch tip visibility, media-player visibility, Alpha Waves on/off, and volume.
- Added sidebar media player (toggle-enabled) with:
  - `Prev`, `Play/Pause`, `Next`
  - scrolling now-playing title
  - local multi-file browse for audio/video
  - playlist controls (select/deselect, move up/down, play next, queue)
  - queue management
  - mini-screen hidden for audio-only and shown for video playback.
- Standardized new modal/pop-up experiences to use blurred backdrop styling for UI consistency.

### Local Publish Outputs

- Desktop publish target:
  - `StudyTide Forge\bin\Release\net10.0-windows10.0.19041.0\win-x64\StudyTideForge.exe`
- Web publish target:
  - `artifacts\publish\studytide-forge-web-win-x64\StudyTideForgeWeb.exe`

## v2.10.4 (2026-03-04)

This release is **ready for public release**.

### Highlights

- Replaced Practice `Load Another Question` with `Next`.
- Added a `Random` Practice mode with locked non-repeating question selection on `Skip` and `Next` until the lock pool is exhausted.
- Renamed Practice `Diff Summary` to `Summary`.
- `Submit Attempt` now opens a compact summary bubble with blurred background and key accuracy/difference metrics.
- Added `Next` inside the summary bubble to close the modal and advance to the next question in the active mode.

### Local Publish Outputs

- Desktop publish target:
  - `StudyTide Forge\bin\Release\net10.0-windows10.0.19041.0\win-x64\StudyTideForge.exe`
- Web publish target:
  - `artifacts\publish\studytide-forge-web-win-x64\StudyTideForgeWeb.exe`

## v2.10.3 (2026-03-04)

This release is **ready for public release**.

### Highlights

- Updated Study Library lesson view to hide the Outline panel automatically when a lesson is opened.
- Added lesson-load reset behavior so Outline always starts hidden, even after previously toggling it visible.
- Preserved manual control with the existing `Show Outline` / `Hide Outline` toggle.

### Local Publish Outputs

- Desktop publish target:
  - `StudyTide Forge\bin\Release\net10.0-windows10.0.19041.0\win-x64\StudyTideForge.exe`
- Web publish target:
  - `artifacts\publish\studytide-forge-web-win-x64\StudyTideForgeWeb.exe`

## v2.10.2 (2026-03-04)

This release is **ready for public release**.

### Highlights

- Added `38` requested Azure supplemental seed blocks under the existing `Azure` category, spanning Azure Container Apps, Container Registry, DevOps/Pipelines, Identity, Networking, and Security topics.
- Updated supplemental seeding so each added training block also backfills a matching flashcard when missing.
- Kept desktop and web seeders aligned by applying the same data-initialization changes in both projects.

### Local Publish Outputs

- Desktop publish target:
  - `StudyTide Forge\bin\Release\net10.0-windows10.0.19041.0\win-x64\StudyTideForge.exe`
- Web publish target:
  - `artifacts\publish\studytide-forge-web-win-x64\StudyTideForgeWeb.exe`

## v2.10.1 (2026-03-03)

This release is **ready for public release**.

### Highlights

- Removed non-training item `"A practical arc for CAD Month 1-2"` from Study Library and Flashcards with explicit startup cleanup.
- Added safeguard cleanup pass so the same artifact is removed automatically if it appears in future reseeds/imports.
- Updated Lesson Study layout to pin `Previous`, `Next`, `Practice`, and `Flashcards` actions in a fixed footer area while training content scrolls.
- Verified local inventory after cleanup:
  - `3702` training items
  - `3698` flashcards
  - `7400` combined entries

### Local Publish Outputs

- Desktop publish target:
  - `StudyTide Forge\bin\Release\net10.0-windows10.0.19041.0\win-x64\StudyTideForge.exe`
- Web publish target:
  - `artifacts\publish\studytide-forge-web-win-x64\StudyTideForgeWeb.exe`

## v2.10.0 (2026-03-03)

This release is **ready for public release**.

### Highlights

- Added archive-aware training extraction to ingest `.pdf`, `.docx`, and `.xlsx` files from `Training Notes.zip` in addition to direct source files.
- Regenerated bundled import source with `3699` deduplicated Q/A pairs and refreshed extraction artifacts:
  - `StudyTide Forge\seed-source\legacy-source.cs`
  - `StudyTide Forge Web\seed-source\legacy-source.cs`
  - `App_Data\training-material\extracted-training-pairs.json`
  - `App_Data\training-material\extraction-report.md`
- Added Azure category coverage target support (`500`) with Azure-specific generated content templates for targeted gap fill behavior.
- Verified reseed inventory with current source:
  - `3703` training items
  - `3699` flashcards
  - `7402` combined entries
  - `574` Azure and `437` System Design items (no gap for requested categories).

### Local Publish Outputs

- Desktop publish target:
  - `StudyTide Forge\bin\Release\net10.0-windows10.0.19041.0\win-x64\StudyTideForge.exe`
- Web publish target:
  - `artifacts\publish\studytide-forge-web-win-x64\StudyTideForgeWeb.exe`

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
