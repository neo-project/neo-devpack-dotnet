# Maintainer-only: merges per-test GAS files generated with NEO_GAS_BASELINE_GENERATE_DIR
# into deterministic baseline files (one per test class, sorted ordinally, LF, no timestamps).
# Classes with more than -MaxEntries executions are split into <Class>.partNN.json (the loader merges all files).
# Usage: ./merge-baseline.ps1 -InputDirectory <dir>/<Assembly> -Output tests/<Project>/GasBaselines/<Assembly>
param(
	[Parameter(Mandatory)][string]$InputDirectory,
	[Parameter(Mandatory)][string]$Output,
	[int]$MaxEntries = 2000
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

$classes = [System.Collections.Generic.SortedDictionary[string, object]]::new([System.StringComparer]::Ordinal)
foreach ($kv in $merged.GetEnumerator()) {
	$test = $kv.Key -replace '[#:].*$', ''
	$class = ($test -replace '\[.*$', '') -replace '\.[^.]+$', ''
	if (-not $classes.ContainsKey($class)) { $classes[$class] = [System.Collections.Generic.SortedDictionary[string, object]]::new([System.StringComparer]::Ordinal) }
	if (-not $classes[$class].ContainsKey($test)) { $classes[$class][$test] = [System.Collections.Generic.List[object]]::new() }
	$classes[$class][$test].Add($kv)
}

New-Item -ItemType Directory -Force -Path $Output | Out-Null
Get-ChildItem -Path $Output -Filter *.json | Remove-Item -Force

function Write-Chunk($path, $entries) {
	$lines = foreach ($kv in $entries) {
		'  ' + (ConvertTo-Json $kv.Key -Compress) + ': ' + $kv.Value.ToString([System.Globalization.CultureInfo]::InvariantCulture)
	}
	[System.IO.File]::WriteAllText($path, "{`n" + ($lines -join ",`n") + "`n}", [System.Text.UTF8Encoding]::new($false))
}

$files = 0
foreach ($class in $classes.Keys) {
	$chunks = [System.Collections.Generic.List[object]]::new()
	$current = [System.Collections.Generic.List[object]]::new()
	foreach ($test in $classes[$class].Keys) {
		foreach ($entry in $classes[$class][$test]) {
			if ($current.Count -ge $MaxEntries) {
				$chunks.Add($current)
				$current = [System.Collections.Generic.List[object]]::new()
			}
			$current.Add($entry)
		}
	}
	if ($current.Count -gt 0) { $chunks.Add($current) }

	for ($i = 0; $i -lt $chunks.Count; $i++) {
		$name = if ($chunks.Count -eq 1) { "$class.json" } else { '{0}.part{1:D2}.json' -f $class, ($i + 1) }
		Write-Chunk (Join-Path $Output $name) $chunks[$i]
		$files++
	}
}
Write-Host "Wrote $($merged.Count) executions in $files files to $Output"
