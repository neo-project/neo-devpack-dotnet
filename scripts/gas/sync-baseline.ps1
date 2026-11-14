# CI policy engine for automatic GAS baseline synchronization.
#
# Plan  : validates candidate measurements (NEO_GAS_BASELINE_CANDIDATE_DIR output of a CI test run) against the
#         committed baselines and stages replacement files. Never modifies the committed baselines.
# Apply : re-validates a staged directory against the committed baselines and replaces the changed files
#         atomically (with rollback). Staged data is treated as untrusted input.
#
# Policy (full run): unchanged -> no change; lower -> update; higher -> reject (unless -AllowIncrease, used only by
# the authorized approval workflow); measurement disappeared -> reject; new execution inside an existing test ->
# reject; whole new test -> onboarded (reported, never auto-merged); failed/unstable/incomplete data -> reject.
# -Partial (filtered runs, local use only): missing measurements are preserved instead of rejected.
#
# Exit code 0: NoChange/Update. Exit code 1: Rejected. result.json and summary.md are written to the staging directory.
[CmdletBinding()]
param(
	[ValidateSet('Plan', 'Apply')][string]$Mode = 'Plan',
	[Parameter(Mandatory)][string]$TestsRoot,
	[Parameter(Mandatory)][string]$StagingDirectory,
	[Parameter(Mandatory)][string[]]$RequiredAssemblies,
	[string]$CandidateRoot,
	[string]$ManifestPath,
	[string]$ExpectedSha,
	[string]$ExpectedRunId,
	[string]$ExpectedRunAttempt,
	[switch]$Partial,
	[switch]$AllowIncrease,
	[int]$MaxEntries = 2000
)

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'gas-baseline-lib.ps1')

$assemblies = @($RequiredAssemblies | ForEach-Object { $_ -split ',' } | ForEach-Object { $_.Trim() } | Where-Object { $_ })
foreach ($assembly in $assemblies) {
	if ($assembly -cnotmatch '^[A-Za-z0-9_.]+$' -or $assembly.Contains('..')) { throw "Invalid assembly name '$assembly'." }
}
if ($assemblies.Count -eq 0) { throw 'At least one required assembly must be specified.' }

$reasons = [System.Collections.Generic.List[string]]::new()
$improvements = [System.Collections.Generic.List[object]]::new()
$increases = [System.Collections.Generic.List[object]]::new()
$newTests = [System.Collections.Generic.SortedSet[string]]::new([System.StringComparer]::Ordinal)
$newExecutions = 0
$stagedFiles = [System.Collections.Generic.List[string]]::new()
$deletedFiles = [System.Collections.Generic.List[string]]::new()
$culture = [System.Globalization.CultureInfo]::InvariantCulture
$utf8 = [System.Text.UTF8Encoding]::new($false)
$baseHashes = [ordered]@{}

function Add-Reason([string]$Text) { $script:reasons.Add($Text) }

function Test-Manifest {
	if ($Partial) { return }
	if (-not $ManifestPath -or -not (Test-Path -LiteralPath $ManifestPath -PathType Leaf)) { Add-Reason 'The run manifest is missing; completeness of the run cannot be verified.'; return }
	try { $manifest = [System.IO.File]::ReadAllText($ManifestPath) | ConvertFrom-Json }
	catch { Add-Reason "The run manifest is not valid JSON: $($_.Exception.Message)"; return }
	if ($manifest.schema -ne 1) { Add-Reason 'Unsupported run manifest schema.' }
	if (-not $ExpectedSha -or $manifest.sha -cne $ExpectedSha) { Add-Reason "The manifest commit '$($manifest.sha)' does not match the expected commit '$ExpectedSha'." }
	if ($ExpectedRunId -and [string]$manifest.runId -cne $ExpectedRunId) { Add-Reason 'Run ID mismatch.' }
	if ($ExpectedRunAttempt -and [string]$manifest.runAttempt -cne $ExpectedRunAttempt) { Add-Reason 'Run attempt mismatch.' }
	if ($manifest.filtered -ne $false) { Add-Reason 'The run was filtered; a partial run cannot synchronize baselines.' }
	foreach ($assembly in $assemblies) {
		$outcome = if ($manifest.suites) { $manifest.suites.PSObject.Properties[$assembly] } else { $null }
		if (-not $outcome -or $outcome.Value -cne 'success') { Add-Reason "Required test suite '$assembly' did not complete successfully ($(if ($outcome) { $outcome.Value } else { 'not executed' }))." }
	}
}

function Limit-List($Items, [int]$Max = 10) {
	$list = @($Items)
	$text = ($list | Select-Object -First $Max) -join '; '
	if ($list.Count -gt $Max) { $text += "; ... and $($list.Count - $Max) more" }
	return $text
}

function Invoke-Plan {
	if (-not $CandidateRoot -or -not (Test-Path -LiteralPath $CandidateRoot -PathType Container)) { Add-Reason 'The candidate directory is missing.'; return }
	Test-Manifest
	foreach ($assembly in $assemblies) {
		$candidateDirectory = Join-Path $CandidateRoot $assembly
		$baselineDirectory = Join-Path $TestsRoot "$assembly/GasBaselines/$assembly"
		if (-not (Test-Path -LiteralPath $candidateDirectory -PathType Container)) { Add-Reason "[$assembly] no candidate measurements were produced."; continue }
		$baseHashes[$assembly] = Get-GasDirectoryHash $baselineDirectory

		$markers = @(Get-ChildItem -LiteralPath $candidateDirectory -File | Where-Object { $_.Extension -in '.failed', '.unstable' })
		if ($markers.Count -gt 0) { Add-Reason "[$assembly] $($markers.Count) test(s) failed or produced unstable measurements: $(Limit-List ($markers | ForEach-Object { $_.Name }))."; continue }

		try {
			$candidate = Read-GasBaselineDirectory -Directory $candidateDirectory
			$baseline = Read-GasBaselineDirectory -Directory $baselineDirectory
		}
		catch { Add-Reason "[$assembly] $($_.Exception.Message)"; continue }
		if ($candidate.Count -eq 0) { Add-Reason "[$assembly] candidate measurements are empty."; continue }

		$baselineTests = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
		foreach ($key in $baseline.Keys) { [void]$baselineTests.Add((Get-GasTestId $key)) }
		$merged = [System.Collections.Generic.SortedDictionary[string, long]]::new($baseline, [System.StringComparer]::Ordinal)
		$changedClasses = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
		$missing = [System.Collections.Generic.List[string]]::new()
		$unexpected = [System.Collections.Generic.List[string]]::new()
		$assemblyIncreases = [System.Collections.Generic.List[string]]::new()

		foreach ($id in @(@($baseline.Keys) + @($candidate.Keys) | Sort-Object -Unique -CaseSensitive)) {
			$inBaseline = $baseline.ContainsKey($id)
			$inCandidate = $candidate.ContainsKey($id)
			if ($inBaseline -and $inCandidate) {
				$old = $baseline[$id]; $new = $candidate[$id]
				if ($new -eq $old) { continue }
				if ($new -lt $old) {
					$improvements.Add([pscustomobject]@{ Assembly = $assembly; Id = $id; Baseline = $old; Current = $new })
				}
				else {
					$increases.Add([pscustomobject]@{ Assembly = $assembly; Id = $id; Baseline = $old; Current = $new })
					if (-not $AllowIncrease) { $assemblyIncreases.Add("$id ($old -> $new)"); continue }
				}
				$merged[$id] = $new
				[void]$changedClasses.Add((Get-GasClassName $id))
			}
			elseif ($inBaseline) {
				if (-not $Partial) { $missing.Add($id) }
			}
			else {
				$test = Get-GasTestId $id
				if ($baselineTests.Contains($test)) { $unexpected.Add($id); continue }
				[void]$newTests.Add("$assembly|$test")
				$script:newExecutions++
				$merged[$id] = $candidate[$id]
				[void]$changedClasses.Add((Get-GasClassName $id))
			}
		}
		if ($assemblyIncreases.Count -gt 0) { Add-Reason "[$assembly] GAS increased for $($assemblyIncreases.Count) execution(s); explicit approval is required: $(Limit-List $assemblyIncreases)." }
		if ($missing.Count -gt 0) { Add-Reason "[$assembly] $($missing.Count) expected measurement(s) disappeared; investigation required: $(Limit-List $missing)." }
		if ($unexpected.Count -gt 0) { Add-Reason "[$assembly] $($unexpected.Count) unexpected execution(s) in existing tests; investigation required: $(Limit-List $unexpected)." }
		if ($changedClasses.Count -eq 0 -or $assemblyIncreases.Count -gt 0 -or $missing.Count -gt 0 -or $unexpected.Count -gt 0) { continue }

		$all = Get-GasClassFiles -Measurements $merged -MaxEntries $MaxEntries
		$existingNames = @(Get-ChildItem -LiteralPath $baselineDirectory -Filter *.json -File | ForEach-Object { $_.Name })
		$files = [ordered]@{}
		$deletes = [System.Collections.Generic.List[string]]::new()
		foreach ($class in $changedClasses) {
			foreach ($name in $all[$class].Keys) {
				$path = Join-Path $baselineDirectory $name
				if (-not (Test-Path -LiteralPath $path) -or [System.IO.File]::ReadAllText($path) -cne $all[$class][$name]) { $files[$name] = $all[$class][$name] }
			}
			foreach ($name in $existingNames) {
				if ((Test-GasClassFileName -FileName $name -Class $class) -and -not $all[$class].Contains($name)) { $deletes.Add($name) }
			}
		}
		try {
			$overlay = Get-GasOverlay -BaselineDirectory $baselineDirectory -Files $files -Delete $deletes.ToArray()
			if (-not (Test-GasMeasurementsEqual $overlay $merged)) { throw 'staged files do not reproduce the expected measurements' }
		}
		catch { Add-Reason "[$assembly] validation of the generated files failed: $($_.Exception.Message)"; continue }

		$target = Join-Path $StagingDirectory $assembly
		New-Item -ItemType Directory -Force -Path $target | Out-Null
		foreach ($name in $files.Keys) {
			[System.IO.File]::WriteAllText((Join-Path $target $name), $files[$name], $utf8)
			$stagedFiles.Add("$assembly/GasBaselines/$assembly/$name")
		}
		foreach ($name in $deletes) { $deletedFiles.Add("$assembly/GasBaselines/$assembly/$name") }
		[System.IO.File]::WriteAllText((Join-Path $target '_delete.txt'), (($deletes | Sort-Object -CaseSensitive) -join "`n"), $utf8)
	}
}

function Invoke-Apply {
	if (-not (Test-Path -LiteralPath $StagingDirectory -PathType Container)) { Add-Reason 'The staging directory is missing.'; return }
	$original = Get-Content -LiteralPath (Join-Path $StagingDirectory 'result.json') -Raw | ConvertFrom-Json
	if ($original.status -cne 'Update') { throw 'Only a validated Update plan may be applied.' }
	$known = [System.Collections.Generic.HashSet[string]]::new([string[]]$assemblies, [System.StringComparer]::Ordinal)
	foreach ($entry in Get-ChildItem -LiteralPath $StagingDirectory) {
		if ($entry.PSIsContainer) { if (-not $known.Contains($entry.Name)) { Add-Reason "Unexpected staged directory '$($entry.Name)'." } }
		elseif ($entry.Name -notin 'result.json', 'summary.md', 'apply-result.json', 'apply-summary.md') { Add-Reason "Unexpected staged file '$($entry.Name)'." }
	}
	if ($reasons.Count -gt 0) { return }

	$plans = @()
	foreach ($assembly in $assemblies) {
		$stage = Join-Path $StagingDirectory $assembly
		if (-not (Test-Path -LiteralPath $stage -PathType Container)) { continue }
		$baselineDirectory = Join-Path $TestsRoot "$assembly/GasBaselines/$assembly"
		try {
			$hash = $original.baseHashes.PSObject.Properties[$assembly]
			if (-not $hash -or $hash.Value -cne (Get-GasDirectoryHash $baselineDirectory)) { throw 'Baseline changed since planning; regenerate candidates and staging.' }
			$files = [ordered]@{}
			$deletes = @()
			foreach ($file in Get-ChildItem -LiteralPath $stage -File) {
				if ($file.Name -ceq '_delete.txt') { $deletes = @([System.IO.File]::ReadAllText($file.FullName) -split "`n" | Where-Object { $_ }); continue }
				Assert-GasSafeFileName $file.Name
				$files[$file.Name] = [System.IO.File]::ReadAllText($file.FullName)
			}
			if (Get-ChildItem -LiteralPath $stage -Directory) { throw 'nested directories are not allowed' }
			foreach ($name in $deletes) {
				Assert-GasSafeFileName $name
				if (-not (Test-Path -LiteralPath (Join-Path $baselineDirectory $name) -PathType Leaf)) { throw "cannot delete non-existent file '$name'" }
			}
			$baseline = Read-GasBaselineDirectory -Directory $baselineDirectory
			$overlay = Get-GasOverlay -BaselineDirectory $baselineDirectory -Files $files -Delete $deletes
			$baselineTests = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
			foreach ($key in $baseline.Keys) { [void]$baselineTests.Add((Get-GasTestId $key)) }
			foreach ($kv in $baseline.GetEnumerator()) {
				if (-not $overlay.ContainsKey($kv.Key)) { throw "measurement '$($kv.Key)' would be removed" }
				if ($overlay[$kv.Key] -gt $kv.Value -and -not $AllowIncrease) { throw "measurement '$($kv.Key)' would increase ($($kv.Value) -> $($overlay[$kv.Key]))" }
			}
			foreach ($kv in $overlay.GetEnumerator()) {
				if (-not $baseline.ContainsKey($kv.Key) -and $baselineTests.Contains((Get-GasTestId $kv.Key))) { throw "unexpected new execution '$($kv.Key)' in an existing test" }
			}
			foreach ($name in $files.Keys) { $stagedFiles.Add("$assembly/GasBaselines/$assembly/$name") }
			foreach ($name in $deletes) { $deletedFiles.Add("$assembly/GasBaselines/$assembly/$name") }
			$plans += , @($baselineDirectory, $files, $deletes)
		}
		catch { Add-Reason "[$assembly] staged baseline rejected: $($_.Exception.Message)" }
	}
	if ($reasons.Count -gt 0) { return }
	Publish-GasTransaction $plans
}

function Write-Result([string]$Status) {
	New-Item -ItemType Directory -Force -Path $StagingDirectory | Out-Null
	$saved = [long]0
	foreach ($row in $improvements) { $saved += ($row.Baseline - $row.Current) }
	$result = [ordered]@{
		status            = $Status
		mode              = $Mode
		baseHashes        = $baseHashes
		improvements      = $improvements.Count
		totalGasSaved     = $saved
		increases         = $increases.Count
		newTests          = $newTests.Count
		newExecutions     = $newExecutions
		autoMergeEligible = ($Status -eq 'Update' -and $newExecutions -eq 0 -and $increases.Count -eq 0)
		changedFiles      = @($stagedFiles | Sort-Object -CaseSensitive)
		deletedFiles      = @($deletedFiles | Sort-Object -CaseSensitive)
		reasons           = @($reasons)
	}
	$sb = [System.Text.StringBuilder]::new()
	[void]$sb.Append("# GAS baseline synchronization`n`n- Result: $Status`n")
	[void]$sb.Append("- Improved executions: $($improvements.Count)`n- Total GAS saved (datoshi, sum over measured executions): $($saved.ToString($culture))`n")
	[void]$sb.Append("- New tests onboarded: $($newTests.Count) ($newExecutions executions)`n- Increases: $($increases.Count)`n")
	if ($reasons.Count -gt 0) { [void]$sb.Append("`n## Blocking reasons`n`n"); foreach ($r in $reasons) { [void]$sb.Append("- $r`n") } }
	if ($improvements.Count -gt 0) {
		[void]$sb.Append("`n## Improvements`n`n| Execution | Baseline | Current | Saved |`n|---|---:|---:|---:|`n")
		foreach ($row in ($improvements | Sort-Object @{ Expression = { $_.Baseline - $_.Current }; Descending = $true }, @{ Expression = { $_.Id } } | Select-Object -First 50)) {
			$cell = (($row.Id -replace '\\', '\\') -replace '\|', '\|')
			[void]$sb.Append("| $($row.Assembly): $cell | $($row.Baseline.ToString($culture)) | $($row.Current.ToString($culture)) | $(($row.Baseline - $row.Current).ToString($culture)) |`n")
		}
		if ($improvements.Count -gt 50) { [void]$sb.Append("`n... and $($improvements.Count - 50) more.`n") }
	}
	if ($newTests.Count -gt 0) {
		[void]$sb.Append("`n## New tests`n`n")
		foreach ($t in ($newTests | Select-Object -First 50)) { [void]$sb.Append("- $($t -replace '\|', ': ')`n") }
	}
	$prefix = if ($Mode -eq 'Apply') { 'apply-' } else { '' }
	[System.IO.File]::WriteAllText((Join-Path $StagingDirectory ($prefix + 'result.json')), (($result | ConvertTo-Json -Depth 4) -replace "`r`n", "`n"), $utf8)
	[System.IO.File]::WriteAllText((Join-Path $StagingDirectory ($prefix + 'summary.md')), $sb.ToString(), $utf8)
}

$lock = $null
try {
	$lock = Enter-GasLock $TestsRoot
	if ($Mode -eq 'Plan' -and (Test-Path -LiteralPath $StagingDirectory)) { Remove-Item -LiteralPath $StagingDirectory -Recurse -Force }
	if ($Mode -eq 'Plan') { Invoke-Plan } else { Invoke-Apply }
}
catch { Add-Reason $_.Exception.Message }
finally { if ($lock) { $lock.Dispose() } }

if ($reasons.Count -gt 0) {
	if ($Mode -eq 'Plan' -and (Test-Path -LiteralPath $StagingDirectory)) { Remove-Item -LiteralPath $StagingDirectory -Recurse -Force }
	Write-Result 'Rejected'
	Write-Host "GAS baseline synchronization rejected:`n- $($reasons -join "`n- ")"
	exit 1
}
$status = if ($stagedFiles.Count -gt 0 -or $deletedFiles.Count -gt 0) { 'Update' } else { 'NoChange' }
Write-Result $status
Write-Host "GAS baseline synchronization: $status ($($stagedFiles.Count) files)"
