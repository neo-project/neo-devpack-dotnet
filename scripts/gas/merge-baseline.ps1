# Maintainer-only: merges per-test GAS files generated with NEO_GAS_BASELINE_GENERATE_DIR
# into a deterministic baseline (sorted ordinally, LF, no timestamps).
# Usage: ./merge-baseline.ps1 -InputDirectory <dir>/<Assembly> -Output tests/<Project>/GasBaselines/<Assembly>.executions.json
param(
	[Parameter(Mandatory)][string]$InputDirectory,
	[Parameter(Mandatory)][string]$Output
)

if ($env:CI -eq 'true' -or $env:GITHUB_ACTIONS) { throw 'GAS baselines must not be generated in CI.' }

$merged = [System.Collections.Generic.SortedDictionary[string, long]]::new([System.StringComparer]::Ordinal)
foreach ($file in Get-ChildItem -Path $InputDirectory -Filter *.json) {
	$json = Get-Content -Raw -Path $file.FullName | ConvertFrom-Json
	foreach ($p in $json.PSObject.Properties) {
		if ($merged.ContainsKey($p.Name)) { throw "Duplicate measurement '$($p.Name)' in $($file.Name)" }
		$merged[$p.Name] = [long]$p.Value
	}
}

$lines = foreach ($kv in $merged.GetEnumerator()) {
	'  ' + (ConvertTo-Json $kv.Key -Compress) + ': ' + $kv.Value.ToString([System.Globalization.CultureInfo]::InvariantCulture)
}
$text = "{`n" + ($lines -join ",`n") + "`n}"
[System.IO.File]::WriteAllText($Output, $text, [System.Text.UTF8Encoding]::new($false))
Write-Host "Wrote $($merged.Count) executions to $Output"
