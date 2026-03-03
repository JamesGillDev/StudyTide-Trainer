# Release Notes

## v2.5.0 (2026-03-03)

This release is **ready for public release**.

### Highlights

- Extracted training material from the provided MSSA document set (PDF, DOCX, XLSX), including OCR fallback for image-only content.
- Produced and integrated `3484` deduplicated training pairs into bundled seed sources for both desktop and web apps.
- Added reproducible extraction workflow via `tools/extract_training_material.py`.
- Added extraction audit artifacts in `App_Data/training-material` for traceability.
- Desktop app now automatically uses bundled seed source when `FORGE_IMPORT_SOURCE_FILE` is not provided.

### Local Publish Outputs

- Desktop publish target:
  - `StudyTide Forge\bin\Release\net10.0-windows10.0.19041.0\win-x64\StudyTideForge.exe`
- Web publish target:
  - `artifacts\publish\studytide-forge-web-win-x64\StudyTideForgeWeb.exe`
