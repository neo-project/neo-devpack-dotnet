# Aggregates per-test GAS tracking files (tracked-<assembly>/tests/*.tsv) into a deterministic
# Markdown and JSON report. Safe to run after failures; never touches committed baselines.
# Usage: ./.github/scripts/gas/aggregate-report.ps1 -ReportDirectory gas-report
param(
    [Parameter(Mandatory)][string]$ReportDirectory,
    [string]$NeoCoreVersion = $env:NEO_CORE_VERSION,
    [string]$Optimization = 'All',
    [string]$Hardforks = 'ProtocolSettings.Default'
)

$culture = [System.Globalization.CultureInfo]::InvariantCulture
# Test ids are written escaped (\\, \t, \n, \r) so they cannot break the tab-separated rows.
function Unescape([string]$v) { if ($null -eq $v) { return $v }; [regex]::Replace($v, '\\(.)', { param($m) switch ($m.Groups[1].Value) { 't' { "`t" } 'n' { "`n" } 'r' { "`r" } default { $m.Groups[1].Value } } }) }
# Markdown table cell escaping for identifiers: backslash first, then pipe and control characters.
function EscapeCell([string]$v) { ($v -replace '\\', '\\') -replace '\|', '\|' -replace "`t", '\t' -replace "`r", '\r' -replace "`n", '\n' }
$sb = [System.Text.StringBuilder]::new()
$grandSaved = [decimal]0
$json = [System.Collections.Generic.SortedDictionary[string, object]]::new([System.StringComparer]::Ordinal)
[void]$sb.Append("# GAS tracking report`n")
if (-not $NeoCoreVersion) { $NeoCoreVersion = 'unknown' }
$json['_metadata'] = [ordered]@{ neoCore = $NeoCoreVersion; optimization = $Optimization; hardforks = $Hardforks }
[void]$sb.Append("`n- Neo Core: $NeoCoreVersion`n- Optimization: $Optimization`n- Hardforks: $Hardforks`n")
$headerLength = $sb.Length

foreach ($dir in (Get-ChildItem -Path $ReportDirectory -Directory -Filter 'tracked-*' | Sort-Object Name -CaseSensitive)) {
	$rows = foreach ($file in (Get-ChildItem -Path (Join-Path $dir.FullName 'tests') -Filter *.tsv -ErrorAction SilentlyContinue)) {
		foreach ($line in (Get-Content -Path $file.FullName)) {
			if ($line) { $f = $line -split "`t"; [pscustomobject]@{ Name = Unescape $f[0]; Status = $f[1]; Baseline = $f[2]; Current = $f[3] } }
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
		foreach ($r in $failures) { [void]$sb.Append("| $((($r.Name -replace '\|','\|') -replace "`t",'\t') -replace "`r",'\r' -replace "`n",'\n') | $($r.Status) | $($r.Baseline) | $($r.Current) |`n") }
	}
	$details = [System.Collections.Generic.List[object]]::new()
	foreach ($r in @($rows | Where-Object { $_.Status -eq 'Improvement' })) {
		$b = [decimal]0; $c = [decimal]0
		if (-not ([decimal]::TryParse([string]$r.Baseline, [System.Globalization.NumberStyles]::AllowLeadingSign, $culture, [ref]$b) -and [decimal]::TryParse([string]$r.Current, [System.Globalization.NumberStyles]::AllowLeadingSign, $culture, [ref]$c))) { continue }
		$saved = $b - $c
		$pct = if ($b -eq 0) { [decimal]0 } else { [math]::Round($saved / $b * 100, 2, [System.MidpointRounding]::AwayFromZero) }
		$details.Add([pscustomobject]@{ Name = $r.Name; Baseline = $b; Current = $c; Saved = $saved; Pct = $pct })
	}
	$details.Sort([System.Comparison[object]]{ param($x, $y) $d = [decimal]::Compare($y.Saved, $x.Saved); if ($d -ne 0) { $d } else { [string]::CompareOrdinal($x.Name, $y.Name) } })
	$totalSaved = [decimal]0; $largest = [decimal]0
	foreach ($d in $details) { $totalSaved += $d.Saved; if ($d.Saved -gt $largest) { $largest = $d.Saved } }
	$grandSaved += $totalSaved

	[void]$sb.Append("`n### GAS improvements`n`n")
	if ($details.Count -eq 0) {
		[void]$sb.Append("No GAS improvements detected.`n")
	}
	else {
		[void]$sb.Append("- Improved executions: $($details.Count)`n- Total measured GAS saved: $($totalSaved.ToString($culture)) datoshi`n- Largest GAS improvement: $($largest.ToString($culture)) datoshi`n")
		[void]$sb.Append("`n| Execution | Baseline | Current | GAS saved | Reduction |`n|---|---:|---:|---:|---:|`n")
		foreach ($d in $details) { [void]$sb.Append("| $(EscapeCell $d.Name) | $($d.Baseline.ToString($culture)) | $($d.Current.ToString($culture)) | $($d.Saved.ToString($culture)) | $($d.Pct.ToString('0.00', $culture))% |`n") }
	}
	$json[$dir.Name.Substring(8)] = [ordered]@{
		executions = $rows.Count
		regressions = (& $count 'Regression'); improvements = (& $count 'Improvement')
		missing = (& $count 'Missing'); unexpected = (& $count 'Unexpected')
		totalGasSaved = $totalSaved
		largestGasSaved = $largest
		improvementDetails = @($details | ForEach-Object { [ordered]@{ execution = $_.Name; baseline = $_.Baseline; current = $_.Current; gasSaved = $_.Saved; reductionPercent = $_.Pct } })
	}
}

[void]$sb.Insert($headerLength, "- Total measured GAS saved (sum over measured test executions, not a transaction fee estimate): $($grandSaved.ToString($culture)) datoshi`n")
$json['_totals'] = [ordered]@{ totalGasSaved = $grandSaved }

[System.IO.File]::WriteAllText((Join-Path $ReportDirectory 'gas-tracking-report.md'), $sb.ToString(), [System.Text.UTF8Encoding]::new($false))
[System.IO.File]::WriteAllText((Join-Path $ReportDirectory 'gas-tracking-report.json'), (($json | ConvertTo-Json -Depth 5) -replace "`r`n", "`n"), [System.Text.UTF8Encoding]::new($false))
