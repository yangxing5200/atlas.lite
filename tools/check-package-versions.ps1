$ErrorActionPreference = 'Stop'

$root = Resolve-Path (Join-Path $PSScriptRoot '..')
$rootPath = $root.Path
$projects = Get-ChildItem -Path $rootPath -Recurse -Filter '*.csproj' -File |
    Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }

$violations = New-Object System.Collections.Generic.List[string]

foreach ($project in $projects) {
    $content = Get-Content -Raw -LiteralPath $project.FullName
    if ($content -match '<PackageReference[^>]*\sVersion\s*=') {
        $violations.Add($project.FullName)
        continue
    }

    if ($content -match '<PackageReference[\s\S]*?<Version>') {
        $violations.Add($project.FullName)
    }
}

if ($violations.Count -gt 0) {
    Write-Error "Inline PackageReference versions found:`n$($violations -join [Environment]::NewLine)"
    exit 1
}

Write-Host "Package version check passed."
