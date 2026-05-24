$ErrorActionPreference = 'Stop'

$root = Resolve-Path (Join-Path $PSScriptRoot '..')
$rootPath = $root.Path
$binaryExtensions = @(
    '.png', '.jpg', '.jpeg', '.gif', '.ico', '.pdf', '.zip',
    '.dll', '.exe', '.pdb', '.db', '.db-shm', '.db-wal'
)

$files = git -C $rootPath ls-files --cached --others --exclude-standard
$violations = New-Object System.Collections.Generic.List[string]

foreach ($file in $files) {
    $path = Join-Path $rootPath $file
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
        continue
    }

    $extension = [System.IO.Path]::GetExtension($path).ToLowerInvariant()
    if ($binaryExtensions -contains $extension) {
        continue
    }

    $bytes = [System.IO.File]::ReadAllBytes($path)
    if ($bytes.Length -eq 0) {
        continue
    }

    $isYaml = $extension -eq '.yml' -or $extension -eq '.yaml'
    for ($i = 0; $i -lt $bytes.Length; $i++) {
        if ($bytes[$i] -ne 10) {
            continue
        }

        $hasCarriageReturn = $i -gt 0 -and $bytes[$i - 1] -eq 13
        if ($isYaml -and $hasCarriageReturn) {
            $violations.Add("$file must use LF.")
            break
        }

        if (-not $isYaml -and -not $hasCarriageReturn) {
            $violations.Add("$file must use CRLF.")
            break
        }
    }
}

if ($violations.Count -gt 0) {
    Write-Error "Line ending check failed:`n$($violations -join [Environment]::NewLine)"
    exit 1
}

Write-Host "Line ending check passed."
