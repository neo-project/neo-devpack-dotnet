# Shared helpers for GAS baseline tooling (merge-baseline.ps1, sync-baseline.ps1).
# Dot-source this file. Requires PowerShell 7+ (System.Text.Json).
# Layout rules (must match the committed baselines): one JSON object per file, entries sorted ordinally,
# two-space indent, LF, no trailing newline, UTF-8 without BOM, classes split into <Class>.partNN.json
# when they exceed MaxEntries executions.

function Read-GasSnapshotText {
	param([Parameter(Mandatory)][string]$Text, [Parameter(Mandatory)][string]$Source)
	$result = [System.Collections.Generic.SortedDictionary[string, long]]::new([System.StringComparer]::Ordinal)
	try { $document = [System.Text.Json.JsonDocument]::Parse($Text) }
	catch { throw "Invalid JSON in '$Source': $($_.Exception.Message)" }
	try {
		if ($document.RootElement.ValueKind -ne [System.Text.Json.JsonValueKind]::Object) { throw "'$Source' must contain a JSON object." }
		foreach ($property in $document.RootElement.EnumerateObject()) {
			$name = $property.Name
			if ([string]::IsNullOrWhiteSpace($name)) { throw "Empty measurement name in '$Source'." }
			if ($property.Value.ValueKind -ne [System.Text.Json.JsonValueKind]::Number) { throw "Measurement '$name' in '$Source' is not a number." }
			$value = [long]0
			if (-not $property.Value.TryGetInt64([ref]$value)) { throw "Measurement '$name' in '$Source' is not a 64-bit integer." }
			if ($value -lt 0) { throw "Measurement '$name' in '$Source' is negative." }
			if ($result.ContainsKey($name)) { throw "Duplicate measurement '$name' in '$Source'." }
			$result[$name] = $value
		}

	}
	finally { $document.Dispose() }
	return , $result
}

function Get-GasDirectoryHash([string]$Directory) {
	$rows = [System.Collections.Generic.List[string]]::new()
	$names = [string[]]@(Get-ChildItem -LiteralPath $Directory -Filter *.json -File -ErrorAction SilentlyContinue | ForEach-Object { $_.Name })
	[Array]::Sort($names, [StringComparer]::Ordinal)
	foreach ($name in $names) {
		$rows.Add($name + ':' + (Get-FileHash -LiteralPath (Join-Path $Directory $name) -Algorithm SHA256).Hash)
	}
	return [Convert]::ToHexString([System.Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes(($rows -join "`n"))))
}

function Enter-GasLock([string]$Root) {
	$identity = [IO.Path]::GetFullPath($Root).TrimEnd([IO.Path]::DirectorySeparatorChar)
	if ($IsWindows) { $identity = $identity.ToUpperInvariant() }
	$hash = [Convert]::ToHexString([System.Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($identity)))
	# Do not delete the lock file: unlinking it permits two different inodes to be locked on Unix.
	return [IO.File]::Open((Join-Path ([IO.Path]::GetTempPath()) "neo-gas-$hash.lock"), [IO.FileMode]::OpenOrCreate, [IO.FileAccess]::ReadWrite, [IO.FileShare]::None)
}

function Publish-GasTransaction($Plans) {
	$backup = Join-Path ([IO.Path]::GetTempPath()) ('gas-transaction-' + [guid]::NewGuid().ToString('N'))
	New-Item -ItemType Directory -Path $backup | Out-Null
	$saved = @()
	try {
		foreach ($plan in $Plans) {
			$directory = $plan[0]
			$copy = Join-Path $backup ([string]$saved.Count)
			New-Item -ItemType Directory -Path $copy | Out-Null
			$existed = Test-Path -LiteralPath $directory -PathType Container
			if ($existed) { Get-ChildItem -LiteralPath $directory -Filter *.json -File | Copy-Item -Destination $copy }
			$saved += , @($directory, $copy, $existed)
		}
		foreach ($plan in $Plans) { Publish-GasFiles -Directory $plan[0] -Files $plan[1] -Delete $plan[2] }
	}
	catch {
		foreach ($entry in $saved) {
			Get-ChildItem -LiteralPath $entry[0] -Filter *.json -File -ErrorAction SilentlyContinue | Remove-Item -Force
			Get-ChildItem -LiteralPath $entry[1] -Filter *.json -File | Copy-Item -Destination $entry[0] -Force
			if (-not $entry[2] -and (Test-Path -LiteralPath $entry[0])) { Remove-Item -LiteralPath $entry[0] }
		}
		throw
	}
	finally { Remove-Item -LiteralPath $backup -Recurse -Force }
}

function Add-GasMeasurements {
	param($Target, $Source, [string]$SourceName)
	foreach ($kv in $Source.GetEnumerator()) {
		if ($Target.ContainsKey($kv.Key)) { throw "Duplicate measurement '$($kv.Key)' in $SourceName." }
		$Target[$kv.Key] = $kv.Value
	}
}

# Reads every *.json file of a directory (sorted ordinally) into one dictionary; a missing directory is empty.
function Read-GasBaselineDirectory {
	param([Parameter(Mandatory)][string]$Directory)
	$merged = [System.Collections.Generic.SortedDictionary[string, long]]::new([System.StringComparer]::Ordinal)
	if (Test-Path -LiteralPath $Directory -PathType Container) {
		$names = [string[]]@(Get-ChildItem -LiteralPath $Directory -Filter *.json -File | ForEach-Object { $_.Name })
		[System.Array]::Sort($names, [System.StringComparer]::Ordinal)
		foreach ($name in $names) {
			$path = Join-Path $Directory $name
			$snapshot = Read-GasSnapshotText -Text ([System.IO.File]::ReadAllText($path)) -Source $path
			Add-GasMeasurements -Target $merged -Source $snapshot -SourceName $path
		}
	}
	return , $merged
}

function Get-GasTestId([string]$ExecutionId) {
	$depth = 0
	for ($i = 0; $i -lt $ExecutionId.Length; $i++) {
		if ($ExecutionId[$i] -eq '[') { $depth++ }
		elseif ($ExecutionId[$i] -eq ']') { $depth-- }
		elseif ($depth -eq 0 -and $ExecutionId[$i] -in '#', ':') { return $ExecutionId.Substring(0, $i) }
		if ($depth -lt 0) { throw "Invalid execution identifier '$ExecutionId'." }
	}
	throw "Execution identifier '$ExecutionId' has no operation suffix."
}

function Get-GasClassName([string]$ExecutionId) {
	$test = Get-GasTestId $ExecutionId
	return ($test -replace '\[.*$', '') -replace '\.[^.]+$', ''
}

function Format-GasJson {
	param($Entries)
	$lines = foreach ($kv in $Entries) {
		'  ' + (ConvertTo-Json $kv.Key -Compress) + ': ' + $kv.Value.ToString([System.Globalization.CultureInfo]::InvariantCulture)
	}
	return "{`n" + ($lines -join ",`n") + "`n}"
}

# Returns [ordered]@{ <class> = [ordered]@{ '<file name>' = '<json text>' } } for all measurements.
function Get-GasClassFiles {
	param($Measurements, [int]$MaxEntries = 2000)
	$classes = [System.Collections.Generic.SortedDictionary[string, object]]::new([System.StringComparer]::Ordinal)
	foreach ($kv in $Measurements.GetEnumerator()) {
		$test = Get-GasTestId $kv.Key
		$class = Get-GasClassName $kv.Key
		if (-not $classes.ContainsKey($class)) { $classes[$class] = [System.Collections.Generic.SortedDictionary[string, object]]::new([System.StringComparer]::Ordinal) }
		if (-not $classes[$class].ContainsKey($test)) { $classes[$class][$test] = [System.Collections.Generic.List[object]]::new() }
		$classes[$class][$test].Add($kv)
	}

	$result = [ordered]@{}
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

		$files = [ordered]@{}
		for ($i = 0; $i -lt $chunks.Count; $i++) {
			$name = if ($chunks.Count -eq 1) { "$class.json" } else { '{0}.part{1:D2}.json' -f $class, ($i + 1) }
			$files[$name] = Format-GasJson $chunks[$i]
		}
		$result[$class] = $files
	}
	return $result
}

function Test-GasClassFileName {
	param([string]$FileName, [string]$Class)
	return $FileName -ceq "$Class.json" -or $FileName -cmatch ('^' + [regex]::Escape($Class) + '\.part\d{2,}\.json$')
}

function Assert-GasSafeFileName {
	param([string]$Name)
	if ([string]::IsNullOrWhiteSpace($Name) -or [System.IO.Path]::GetFileName($Name) -cne $Name -or
		-not $Name.EndsWith('.json', [System.StringComparison]::Ordinal) -or $Name.StartsWith('.', [System.StringComparison]::Ordinal) -or
		$Name.IndexOfAny([System.IO.Path]::GetInvalidFileNameChars()) -ge 0) {
		throw "Unsafe baseline file name '$Name'."
	}
}

# Computes the measurements that result from applying staged files and deletions to a baseline directory.
function Get-GasOverlay {
	param([Parameter(Mandatory)][string]$BaselineDirectory, $Files, [string[]]$Delete = @())
	$merged = [System.Collections.Generic.SortedDictionary[string, long]]::new([System.StringComparer]::Ordinal)
	$replaced = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
	foreach ($name in $Delete) { [void]$replaced.Add($name) }
	foreach ($name in $Files.Keys) { [void]$replaced.Add($name) }
	if (Test-Path -LiteralPath $BaselineDirectory -PathType Container) {
		foreach ($file in (Get-ChildItem -LiteralPath $BaselineDirectory -Filter *.json -File | Sort-Object { $_.Name } -CaseSensitive)) {
			if ($replaced.Contains($file.Name)) { continue }
			$snapshot = Read-GasSnapshotText -Text ([System.IO.File]::ReadAllText($file.FullName)) -Source $file.FullName
			Add-GasMeasurements -Target $merged -Source $snapshot -SourceName $file.FullName
		}
	}
	foreach ($name in @($Files.Keys | Sort-Object -CaseSensitive)) {
		$snapshot = Read-GasSnapshotText -Text $Files[$name] -Source "staged/$name"
		Add-GasMeasurements -Target $merged -Source $snapshot -SourceName "staged/$name"
	}
	return , $merged
}

function Test-GasMeasurementsEqual {
	param($Left, $Right)
	if ($Left.Count -ne $Right.Count) { return $false }
	foreach ($kv in $Left.GetEnumerator()) {
		if (-not $Right.ContainsKey($kv.Key) -or $Right[$kv.Key] -ne $kv.Value) { return $false }
	}
	return $true
}

# Replaces files in a directory with rollback: new content is written to temporary files first, and
# the original files are restored if any step fails, so a failure never leaves partial updates.
function Publish-GasFiles {
	param([Parameter(Mandatory)][string]$Directory, $Files, [string[]]$Delete = @())
	$utf8 = [System.Text.UTF8Encoding]::new($false)
	foreach ($name in @($Files.Keys) + $Delete) { Assert-GasSafeFileName $name }
	New-Item -ItemType Directory -Force -Path $Directory | Out-Null
	$backup = Join-Path ([System.IO.Path]::GetTempPath()) ('gas-backup-' + [guid]::NewGuid().ToString('N'))
	New-Item -ItemType Directory -Force -Path $backup | Out-Null
	$touched = @(@($Files.Keys) + $Delete | Select-Object -Unique)
	$existed = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
	$temporary = [System.Collections.Generic.List[string]]::new()
	try {
		foreach ($name in $touched) {
			$path = Join-Path $Directory $name
			if (Test-Path -LiteralPath $path) { Copy-Item -LiteralPath $path -Destination (Join-Path $backup $name); [void]$existed.Add($name) }
		}
		foreach ($name in $Files.Keys) {
			$tmp = (Join-Path $Directory $name) + '.' + [guid]::NewGuid().ToString('N') + '.tmp'
			[System.IO.File]::WriteAllText($tmp, $Files[$name], $utf8)
			$temporary.Add($tmp)
		}
		$index = 0
		foreach ($name in $Files.Keys) {
			Move-Item -LiteralPath $temporary[$index] -Destination (Join-Path $Directory $name) -Force
			$index++
		}
		foreach ($name in $Delete) {
			$path = Join-Path $Directory $name
			if (Test-Path -LiteralPath $path) { Remove-Item -LiteralPath $path -Force }
		}
	}
	catch {
		$failure = $_
		foreach ($tmp in $temporary) { if (Test-Path -LiteralPath $tmp) { Remove-Item -LiteralPath $tmp -Force -ErrorAction SilentlyContinue } }
		foreach ($name in $touched) {
			$path = Join-Path $Directory $name
			if ($existed.Contains($name)) { Copy-Item -LiteralPath (Join-Path $backup $name) -Destination $path -Force }
			elseif (Test-Path -LiteralPath $path) { Remove-Item -LiteralPath $path -Force -ErrorAction SilentlyContinue }
		}
		throw $failure
	}
	finally {
		Remove-Item -LiteralPath $backup -Recurse -Force -ErrorAction SilentlyContinue
	}
}
