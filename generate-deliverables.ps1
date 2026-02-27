$root = Get-Location
$output = Join-Path $root "DELIVERABLES_FULL.md"
$changed = git diff --name-only HEAD~2..HEAD | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne "" }
$textExt = @(".cs", ".csproj", ".slnx", ".md", ".json", ".razor", ".xaml", ".css", ".html", ".xml", ".manifest", ".yml", ".yaml", ".txt", ".appxmanifest")
$files = $changed | Where-Object {
    $ext = [IO.Path]::GetExtension($_).ToLowerInvariant()
    $textExt -contains $ext -or $_ -eq ".gitignore"
} | Sort-Object -Unique

"# StudyTide Forge Deliverables" | Set-Content $output
"" | Add-Content $output
"## Complete File Structure" | Add-Content $output
"```text" | Add-Content $output
(Get-ChildItem -Recurse -Path "StudyTide Forge" | ForEach-Object { $_.FullName.Replace((Join-Path $root ""), "") }) | Add-Content $output
"```" | Add-Content $output
"" | Add-Content $output
"## Full Contents Of Modified Files" | Add-Content $output

foreach ($f in $files) {
    if (-not (Test-Path $f)) { continue }
    $ext = [IO.Path]::GetExtension($f).ToLowerInvariant()
    $lang = switch ($ext) {
        ".cs" { "csharp" }
        ".csproj" { "xml" }
        ".slnx" { "xml" }
        ".json" { "json" }
        ".razor" { "razor" }
        ".xaml" { "xml" }
        ".css" { "css" }
        ".html" { "html" }
        ".xml" { "xml" }
        ".manifest" { "xml" }
        ".appxmanifest" { "xml" }
        ".yml" { "yaml" }
        ".yaml" { "yaml" }
        ".md" { "markdown" }
        default { "" }
    }

    "### $f" | Add-Content $output
    "```$lang" | Add-Content $output
    Get-Content -Path $f | Add-Content $output
    "```" | Add-Content $output
    "" | Add-Content $output
}

Write-Host "Created $output with $($files.Count) files."
