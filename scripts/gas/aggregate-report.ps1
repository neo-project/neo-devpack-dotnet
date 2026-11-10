# Aggregates per-test GAS tracking files (tracked-<assembly>/tests/*.tsv) into a deterministic
# Markdown and JSON report. Safe to run after failures; never touches committed baselines.
# Usage: ./aggregate-report.ps1 -ReportDirectory gas-report
param(
    [Parameter(Mandatory)][string]$ReportDirectory,
    [string]$NeoCoreVersion = $env:NEO_CORE_VERSION,
    [string]$Optimization = 'All',
    [string]$Hardforks = 'ProtocolSettings.Default'
)

$culture = [System.Globalization.CultureInfo]::InvariantCulture
$sb = [System.Text.StringBuilder]::new()
$json = [System.Collections.Generic.SortedDictionary[string, object]]::new([System.StringComparer]::Ordinal)
[void]$sb.Append("# GAS tracking report`n")
if (-not $NeoCoreVersion) { $NeoCoreVersion = 'unknown' }
$json['_metadata'] = [ordered]@{ neoCore = $NeoCoreVersion; optimization = $Optimization; hardforks = $Hardforks }
[void]$sb.Append("`n- Neo Core: $NeoCoreVersion`n- Optimization: $Optimization`n- Hardforks: $Hardforks`n")

foreach ($dir in (Get-ChildItem -Path $ReportDirectory -Directory -Filter 'tracked-*' | Sort-Object Name -CaseSensitive)) {
	$rows = foreach ($file in (Get-ChildItem -Path (Join-Path $dir.FullName 'tests') -Filter *.tsv -ErrorAction SilentlyContinue)) {
		foreach ($line in (Get-Content -Path $file.FullName)) {
			if ($line) { $f = $line -split "`t"; [pscustomobject]@{ Name = $f[0]; Status = $f[1]; Baseline = $f[2]; Current = $f[3] } }
		}
	}
	$rows = @($rows | Sort-Object { $_.Name } -CaseSensitive)
	$count = { param($s) @($rows | Where-Object { $_.Status -eq $s }).Count }
	$failures = @($rows | Where-Object { $_.Status -in 'Regression', 'Missing', 'Unexpected' })

	[void]$sb.Append("`n## $($dir.Name.Substring(8))`n`n")
	[void]$sb.Append("- Executions: $($rows.Count)`n")
	foreach ($s in 'Unchanged', 'WithinTolerance', 'Improvement', 'Regression', 'Missing', 'Unexpected') {
		[void]$sb.Append("- ${s}: $(& $count $s)`n")
	}
	if ($failures.Count -gt 0) {
		[void]$sb.Append("`n| Execution | Status | Baseline | Current |`n|---|---|---:|---:|`n")
		foreach ($r in $failures) { [void]$sb.Append("| $($r.Name -replace '\|','\|') | $($r.Status) | $($r.Baseline) | $($r.Current) |`n") }
	}
	$json[$dir.Name.Substring(8)] = [ordered]@{
		executions = $rows.Count
		regressions = (& $count 'Regression'); improvements = (& $count 'Improvement')
		missing = (& $count 'Missing'); unexpected = (& $count 'Unexpected')
	}
}

[System.IO.File]::WriteAllText((Join-Path $ReportDirectory 'gas-tracking-report.md'), $sb.ToString(), [System.Text.UTF8Encoding]::new($false))
[System.IO.File]::WriteAllText((Join-Path $ReportDirectory 'gas-tracking-report.json'), (($json | ConvertTo-Json -Depth 5) -replace "`r`n", "`n"), [System.Text.UTF8Encoding]::new($false))
