<#
.SYNOPSIS
    Copies Nano.Library's AGENTS.md, .claude folder (Claude Code skills), .github/prompts folder
    (Copilot prompt files), .github/copilot-instructions.md (Copilot always-on context), and
    .vscode/settings.json (enables prompt file discovery in VS Code) into the relevant subfolders of
    the sibling Nano.Templates and Nano.Lessons repos, overwriting.

.DESCRIPTION
    Run this from within Nano.Library itself. It expects Nano.Templates and Nano.Lessons to be sibling
    directories one level up (e.g. Nano.Library, Nano.Templates, and Nano.Lessons all under
    C:\Development\Nano-Core). Re-run any time AGENTS.md, .claude/, .github/prompts/,
    .github/copilot-instructions.md, or .vscode/settings.json changes in Nano.Library to propagate the
    update.

    - Nano.Templates: copied into every top-level folder that is an actual Nano application (contains a
      Program.cs anywhere under it, excluding bin/obj) - this excludes shared library folders like
      Lib.Emailing/Lib.Images.
    - Nano.Lessons: copied into every top-level folder that is not completely empty - this excludes
      reserved/placeholder lesson folders that don't have any content yet.

.EXAMPLE
    cd C:\Development\Nano-Core\Nano.Library
    .\sync-agents-md.ps1
#>

$ErrorActionPreference = "Stop"

$libraryRoot = $PSScriptRoot
$root = Split-Path $libraryRoot -Parent
$sourcePath = Join-Path $libraryRoot "AGENTS.md"
$claudeSourcePath = Join-Path $libraryRoot ".claude"
$promptsSourcePath = Join-Path $libraryRoot ".github\prompts"
$copilotInstructionsSourcePath = Join-Path $libraryRoot ".github\copilot-instructions.md"
$vscodeSettingsSourcePath = Join-Path $libraryRoot ".vscode\settings.json"

if (-not (Test-Path $sourcePath)) {
    Write-Error "Source file not found: $sourcePath. Run this script from within the Nano.Library folder."
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

            if (Test-Path $claudeSourcePath) {
                $claudeDestinationPath = Join-Path $folder.FullName ".claude"
                New-Item -Path $claudeDestinationPath -ItemType Directory -Force | Out-Null
                Copy-Item -Path (Join-Path $claudeSourcePath "*") -Destination $claudeDestinationPath -Recurse -Force
                Write-Output ("Copied .claude to " + $claudeDestinationPath)
            }

            if (Test-Path $promptsSourcePath) {
                $promptsDestinationPath = Join-Path $folder.FullName ".github\prompts"
                New-Item -Path $promptsDestinationPath -ItemType Directory -Force | Out-Null
                Copy-Item -Path (Join-Path $promptsSourcePath "*") -Destination $promptsDestinationPath -Recurse -Force
                Write-Output ("Copied .github/prompts to " + $promptsDestinationPath)
            }

            if (Test-Path $copilotInstructionsSourcePath) {
                $copilotInstructionsDestinationPath = Join-Path $folder.FullName ".github\copilot-instructions.md"
                New-Item -Path (Join-Path $folder.FullName ".github") -ItemType Directory -Force | Out-Null
                Copy-Item -Path $copilotInstructionsSourcePath -Destination $copilotInstructionsDestinationPath -Force
                Write-Output ("Copied .github/copilot-instructions.md to " + $copilotInstructionsDestinationPath)
            }

            if (Test-Path $vscodeSettingsSourcePath) {
                $vscodeSettingsDestinationPath = Join-Path $folder.FullName ".vscode\settings.json"
                New-Item -Path (Join-Path $folder.FullName ".vscode") -ItemType Directory -Force | Out-Null
                Copy-Item -Path $vscodeSettingsSourcePath -Destination $vscodeSettingsDestinationPath -Force
                Write-Output ("Copied .vscode/settings.json to " + $vscodeSettingsDestinationPath)
            }
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
