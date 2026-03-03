# Release Notes

## v2.7.0 (2026-03-03)

This release is **ready for public release**.

### Highlights

- Replaced generic training-item example text (`When practicing...`) with concrete, context-aware examples generated from each item's prompt/response content.
- Startup migration now upgrades existing training data in-place so older databases are brought forward automatically.
- Verified combined seeded inventory at `6972` entries:
  - `3488` training items
  - `3484` flashcards
- Standardized lesson-study action button heights for visual consistency across:
  - `Previous`
  - `Next`
  - `Practice`
  - `Flashcards`

### Local Publish Outputs

- Desktop publish target:
  - `StudyTide Forge\bin\Release\net10.0-windows10.0.19041.0\win-x64\StudyTideForge.exe`
- Web publish target:
  - `artifacts\publish\studytide-forge-web-win-x64\StudyTideForgeWeb.exe`
