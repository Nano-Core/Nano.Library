<#
.SYNOPSIS
    Copies Nano.Library's AGENTS.md into the relevant subfolders of the sibling Nano.Templates and
    Nano.Lessons repos, overwriting.

.DESCRIPTION
    Run this from the parent folder that contains Nano.Library, Nano.Templates, and Nano.Lessons as
    sibling directories (e.g. C:\Development\Nano-Core). Re-run any time AGENTS.md changes in Nano.Library
    to propagate the update.

    - Nano.Templates: copied into every top-level folder that is an actual Nano application (contains a
      Program.cs anywhere under it, excluding bin/obj) - this excludes shared library folders like
      Lib.Emailing/Lib.Images.
    - Nano.Lessons: copied into every top-level folder that is not completely empty - this excludes
      reserved/placeholder lesson folders that don't have any content yet.

.EXAMPLE
    cd C:\Development\Nano-Core
    .\Nano.Library\sync-agents-md.ps1
#>

$ErrorActionPreference = "Stop"

$root = Get-Location
$sourcePath = Join-Path $root "Nano.Library\AGENTS.md"

if (-not (Test-Path $sourcePath)) {
    Write-Error "Source file not found: $sourcePath. Run this script from the parent folder containing Nano.Library, Nano.Templates, and Nano.Lessons."
    exit 1
}

function Copy-ToQualifyingFolders {
    param(
        [string]$RepoName,
        [scriptblock]$Qualifies
    )

    $repoPath = Join-Path $root $RepoName

    if (-not (Test-Path $repoPath)) {
        Write-Warning "Skipping '$RepoName' - folder not found at $repoPath"
        return
    }

    $subfolders = Get-ChildItem -Path $repoPath -Directory | Where-Object { $_.Name -notmatch '^\.' }

    foreach ($folder in $subfolders) {
        if (& $Qualifies $folder.FullName) {
            $destinationPath = Join-Path $folder.FullName "AGENTS.md"
            Copy-Item -Path $sourcePath -Destination $destinationPath -Force
            Write-Output ("Copied AGENTS.md to " + $destinationPath)
        }
    }
}

# Nano.Templates: copy into every application folder (contains a Program.cs somewhere, excluding bin/obj)
Copy-ToQualifyingFolders -RepoName "Nano.Templates" -Qualifies {
    param($folderPath)
    $hasProgram = Get-ChildItem -Path $folderPath -Filter "Program.cs" -Recurse -File -ErrorAction SilentlyContinue |
        Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' } |
        Select-Object -First 1
    return $null -ne $hasProgram
}

# Nano.Lessons: copy into every folder that is not completely empty
Copy-ToQualifyingFolders -RepoName "Nano.Lessons" -Qualifies {
    param($folderPath)
    $anyFile = Get-ChildItem -Path $folderPath -Recurse -File -ErrorAction SilentlyContinue | Select-Object -First 1
    return $null -ne $anyFile
}
