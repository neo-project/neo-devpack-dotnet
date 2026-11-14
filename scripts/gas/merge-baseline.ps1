# Maintainer-only: merges per-test GAS files generated with NEO_GAS_BASELINE_GENERATE_DIR
# into deterministic baseline files (one per test class, sorted ordinally, LF, no timestamps).
# Classes with more than -MaxEntries executions are split into <Class>.partNN.json (the loader merges all files).
# By default the existing baseline is preserved and only the measurements present in the input are added or
# overwritten (safe for filtered runs). Use -Replace after a complete run to drop measurements that no longer exist.
# Files are staged and validated before anything is replaced; a failure leaves the baseline untouched.
# Usage: ./merge-baseline.ps1 -InputDirectory <dir>/<Assembly> -Output tests/<Project>/GasBaselines/<Assembly> [-Replace]
param(
	[Parameter(Mandatory)][string]$InputDirectory,
	[Parameter(Mandatory)][string]$Output,
	[int]$MaxEntries = 2000,
	[switch]$Replace
)

$ErrorActionPreference = 'Stop'
if ($env:CI -eq 'true' -or $env:GITHUB_ACTIONS) { throw 'GAS baselines must not be generated in CI.' }
. (Join-Path $PSScriptRoot 'gas-baseline-lib.ps1')
$lock = Enter-GasLock ([IO.Path]::GetFullPath((Join-Path $Output '../../..')))
try {

if (-not (Test-Path -LiteralPath $InputDirectory -PathType Container)) { throw "Input directory '$InputDirectory' does not exist." }
$observed = Read-GasBaselineDirectory -Directory $InputDirectory
if ($observed.Count -eq 0) { throw "No measurements found in '$InputDirectory'." }

$existing = Read-GasBaselineDirectory -Directory $Output
$merged = [System.Collections.Generic.SortedDictionary[string, long]]::new([System.StringComparer]::Ordinal)
if (-not $Replace) { foreach ($kv in $existing.GetEnumerator()) { $merged[$kv.Key] = $kv.Value } }
foreach ($kv in $observed.GetEnumerator()) { $merged[$kv.Key] = $kv.Value }

$all = Get-GasClassFiles -Measurements $merged -MaxEntries $MaxEntries
$files = [ordered]@{}
foreach ($class in $all.Keys) {
	foreach ($name in $all[$class].Keys) {
		$path = Join-Path $Output $name
		if (-not (Test-Path -LiteralPath $path) -or [System.IO.File]::ReadAllText($path) -cne $all[$class][$name]) { $files[$name] = $all[$class][$name] }
	}
}
$keep = @($all.Keys | ForEach-Object { $all[$_].Keys })
$delete = @()
if (Test-Path -LiteralPath $Output -PathType Container) {
	$delete = @(Get-ChildItem -LiteralPath $Output -Filter *.json -File | ForEach-Object { $_.Name } | Where-Object { $keep -cnotcontains $_ })
}

$overlay = Get-GasOverlay -BaselineDirectory $Output -Files $files -Delete $delete
if (-not (Test-GasMeasurementsEqual $overlay $merged)) { throw 'Validation of the generated baseline failed; nothing was changed.' }

Publish-GasFiles -Directory $Output -Files $files -Delete $delete
Write-Host "Wrote $($merged.Count) executions ($($files.Count) files updated, $($delete.Count) removed) to $Output"
}
finally { $lock.Dispose() }
