# Changelog

All notable changes to this project are documented in this file.

The format is based on semantic versioning (`MAJOR.MINOR.PATCH`).

## [2.10.13] - 2026-03-05

### Fixed
- Replaced the broken bundled Alpha Waves WAV asset with the validated 8-hour MP3 provided for the app.
- Updated bundled-track metadata and source path to use `audio/mpeg` from `/audio/8_Hour_Alpha_Waves.mp3`.

### Changed
- App version bumped to `2.10.13` (`ApplicationVersion` `21013`).

### Release
- Marked `v2.10.13` as **in Public Release** on `2026-03-05`.

## [2.10.12] - 2026-03-05

### Fixed
- Resolved startup Alpha Waves race where media `onplay` could fire before current-track assignment and falsely trigger bundled-track failure flow.
- Updated playback initialization order so current track is assigned before JS `play()` call.
- Adjusted media play-state handler so UI play/pause state only reflects active-track playback identity.

### Changed
- App version bumped to `2.10.12` (`ApplicationVersion` `21012`).

### Release
- Marked `v2.10.12` as **in Public Release** on `2026-03-05`.

## [2.10.11] - 2026-03-05

### Fixed
- Added startup media-element cleanup to stop stale hidden playback before media player initialization.
- Added guard handling for `onplay` events without an active current track so playback is force-stopped and state is recovered.
- Updated media-player JS module loading to use a versioned import path, forcing fresh script resolution after app updates.
- Corrected async-disposal cleanup path to unsubscribe shell-state events and dispose component references.

### Changed
- Refreshed media-player playlist control layout and styling for a cleaner professional presentation with larger action controls.
- App version bumped to `2.10.11` (`ApplicationVersion` `21011`).

### Release
- Marked `v2.10.11` as **in Public Release** on `2026-03-05`.

## [2.10.10] - 2026-03-05

### Fixed
- Resolved Alpha Waves state drift where playback could continue while UI track state showed no active media.
- Added explicit stop/reset handling for bundled-track errors, Alpha Waves disable actions, and current-track removals.
- Synced media `play` and `pause` runtime events back into Blazor state to keep button label/state aligned with actual playback.
- Removed bundled-track auto-loop behavior to prevent repeated restart artifacts from short/invalid bundled audio.

### Changed
- Alpha Waves now auto-disables when bundled-track runtime errors occur, preventing repeated startup retry loops.
- App version bumped to `2.10.10` (`ApplicationVersion` `21010`).

### Release
- Marked `v2.10.10` as **in Public Release** on `2026-03-05`.

## [2.10.9] - 2026-03-05

### Fixed
- Updated automatic track-end handling so playlist playback no longer force-wraps to the first track on end events, preventing repeated restart artifacts.
- Added bundled Alpha Waves loop behavior in media runtime wiring to reduce restart churn while focus audio is active.
- Improved media source reset flow in `mediaPlayer.js` before loading a new source.

### Changed
- Refined sidebar media panel visual design (status treatment, now-playing hierarchy, control sizing, and focus visibility) for a cleaner production UI.
- App version bumped to `2.10.9` (`ApplicationVersion` `21009`).

### Release
- Marked `v2.10.9` as **in Public Release** on `2026-03-05`.

## [2.10.8] - 2026-03-04

### Added
- Bundled Alpha Waves audio asset in both apps at `wwwroot/audio/8_Hour_Alpha_Waves.wav` so built-in playback has a local packaged source.

### Fixed
- Hardened media player initialization and shell-settings event handling to reduce first-open startup crash risk caused by JS interop failures.
- Removed fragile bundled-track source preflight checks that could misreport availability in host-specific runtime schemes.
- Updated sidebar media-player sizing/layout to prevent compressed controls, overflow artifacts, and inconsistent title rendering in narrow side rails.

### Changed
- Updated bundled Alpha Waves default source path to `/audio/8_Hour_Alpha_Waves.wav`.
- App version bumped to `2.10.8` (`ApplicationVersion` `21008`).

### Release
- Marked `v2.10.8` as **in Public Release** on `2026-03-04`.

## [2.10.7] - 2026-03-04

### Fixed
- Updated media playback source initialization so local playlist audio files (including `.mp3`) are started with explicit MIME hints.
- Added fail-safe handling for unavailable bundled Alpha Waves source to avoid repeated dead-track playback attempts.
- Updated sidebar playlist item layout to prevent narrow-width title collapse/overflow by separating controls from track title rendering.

### Changed
- Audio mini-screen hidden-state styling now collapses the frame while preserving media element playback behavior for audio-only tracks.
- `AlphaWavesDefaultPath` now targets web-root path `/audio/8_Hour_Alpha_Waves.mp3` instead of an environment-specific absolute development path.
- App version bumped to `2.10.7` (`ApplicationVersion` `21007`).

### Release
- Marked `v2.10.7` as **in Public Release** on `2026-03-04`.

## [2.10.6] - 2026-03-04

### Added
- Persistent app-shell settings backed by local JSON storage so settings survive restart.
- Shared `ModalShell` component used across popup dialogs for consistent blur overlay, panel spacing, and action footer structure.
- Media capability note clarifying codec-dependent playback behavior even when file selection accepts broad audio/video types.

### Changed
- App version bumped to `2.10.6` (`ApplicationVersion` `21006`).

### Release
- Marked `v2.10.6` as **in Public Release** on `2026-03-04`.
- Refreshed local publish verification on `2026-03-04` after VS Code update.

## [2.10.5] - 2026-03-04

### Added
- Practice session progress bar below the `Verbatim Practice` heading.
- Practice scope `Reset` action with warning modal, `!` alert marker, and `Reset Now` confirmation flow.
- Bottom-left settings gear with consolidated help/training guidance in the Settings modal.
- New settings toggles for launch-tip banner, sidebar media player visibility, and Alpha Waves concentration music.
- Sidebar media player with local multi-file browse, scrolling now-playing title, queue/playlist controls, and video-only mini-screen behavior.

### Changed
- Moved `Random` into the main Practice action row beside `Submit Attempt`, `Back`, `Skip`, and `Next`.
- Standardized modal overlays to blurred backdrop styling for consistency.
- App version bumped to `2.10.5` (`ApplicationVersion` `21005`).

### Release
- Marked `v2.10.5` as **in Public Release** on `2026-03-04`.

## [2.10.4] - 2026-03-04

### Added
- Added Practice `Random` mode with locked non-repeating question selection for `Skip` and `Next` actions until the randomized scope pool is exhausted.
- Added a Practice summary modal bubble (blurred backdrop) after `Submit Attempt` with key metrics and a modal `Next` action.

### Changed
- Replaced Practice `Load Another Question` button with `Next`.
- Renamed Practice `Diff Summary` label to `Summary`.
- App version bumped to `2.10.4` (`ApplicationVersion` `21004`).

### Release
- Marked `v2.10.4` as **ready for public release** on `2026-03-04`.

## [2.10.3] - 2026-03-04

### Changed
- Lesson Study now defaults to hidden Outline on page load for Study Library lessons.
- Outline visibility is reset to hidden during lesson parameter initialization so each lesson opens focused on core study content.
- App version bumped to `2.10.3` (`ApplicationVersion` `21003`).

### Release
- Marked `v2.10.3` as **ready for public release** on `2026-03-04`.

## [2.10.2] - 2026-03-04

### Added
- Added `38` Azure supplemental seed blocks based on requested Azure Container Apps, Azure Container Registry, Azure DevOps, Azure Identity, Azure Networking, and Azure Security topics.
- Supplemental/generated seeding now backfills matching flashcards when missing so added training material appears in both Study Library and Flashcards.

### Changed
- App version bumped to `2.10.2` (`ApplicationVersion` `21002`).

### Release
- Marked `v2.10.2` as **ready for public release** on `2026-03-04`.

## [2.10.1] - 2026-03-03

### Fixed
- Removed explicit non-training artifact `A practical arc for CAD Month 1-2` from Study Library and Flashcards during startup cleanup.
- Ensured the same artifact is removed after reseed/import so it does not reappear.
- Pinned Lesson Study action buttons (`Previous`, `Next`, `Practice`, `Flashcards`) to a fixed footer position while slide content scrolls.

### Changed
- App version bumped to `2.10.1` (`ApplicationVersion` `21001`).

### Release
- Marked `v2.10.1` as **ready for public release** on `2026-03-03`.

## [2.10.0] - 2026-03-03

### Added
- Archive-aware training extraction support for `Training Notes.zip`, including `.pdf`, `.docx`, and `.xlsx` entries.
- Azure coverage target support (`500`) with Azure-specific generated seed content for targeted coverage backfill.

### Changed
- Refreshed bundled `legacy-source.cs` import material to `3699` deduplicated training pairs from the expanded training corpus.
- Updated extraction artifacts under `App_Data/training-material` with archive-source reporting.
- App version bumped to `2.10.0` (`ApplicationVersion` `21000`).

### Release
- Marked `v2.10.0` as **ready for public release** on `2026-03-03`.

## [2.9.2] - 2026-03-03

### Changed
- Updated Study Cue generation templates to remove phrasing that did not match training material voice:
  - removed `In a real project, apply ...`
  - removed injected `when ...` / `when you need to ...` prefixes
- Simplified generated Study Cue sentences to direct cue-style phrasing (`Term: cue detail`) and added duplicate-term stripping for cleaner output.
- App version bumped to `2.9.2` (`ApplicationVersion` `2902`).

### Release
- Marked `v2.9.2` as **ready for public release** on `2026-03-03`.

## [2.9.1] - 2026-03-03

### Added
- Startup cleanup migration to remove generated title prefixes (`Refined - ...`, `Focused - ...`) from existing seeded study content.

### Changed
- Compacted Lesson Study header/meta spacing so `Imported Set...` and `Module / Slide / Status` consume less height and leave more room for training content.
- Renamed user-facing `Example` labels to `Study Cue` across Study Library and Practice surfaces where the third field is a cue-style reinforcement line.
- Renamed Expanded Context bullet wording from `Application` to `Study cue` for clearer grammar alignment with cue text.
- App version bumped to `2.9.1` (`ApplicationVersion` `2901`).

### Release
- Marked `v2.9.1` as **ready for public release** on `2026-03-03`.

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
