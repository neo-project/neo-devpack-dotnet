# Synthetic integration checks. No repository baselines are read or modified.
param([Parameter(Mandatory)][string]$Root)
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'gas-baseline-lib.ps1')
New-Item -ItemType Directory -Force -Path $Root | Out-Null
$first = Join-Path $Root 'first'
$second = Join-Path $Root 'second'
New-Item -ItemType Directory -Path $first, $second | Out-Null
[IO.File]::WriteAllText((Join-Path $first 'a.json'), '{"A.Test#1":100}')
[IO.File]::WriteAllText((Join-Path $second 'b.json'), '{"B.Test#1":200}')
$plans = @()
$plans += , @($first, ([ordered]@{ 'a.json' = '{"A.Test#1":90}' }), @())
# A missing source for deletion triggers rollback after the first directory was published.
$plans += , @($second, ([ordered]@{ '../invalid.json' = '{}' }), @())
$rejected = $false
try { Publish-GasTransaction $plans } catch { $rejected = $true }
if (-not $rejected) { throw 'Invalid second publication was accepted.' }
if ([IO.File]::ReadAllText((Join-Path $first 'a.json')) -cne '{"A.Test#1":100}') { throw 'First directory was not restored.' }
if ([IO.File]::ReadAllText((Join-Path $second 'b.json')) -cne '{"B.Test#1":200}') { throw 'Second directory was not restored.' }
$lock = Enter-GasLock $Root
try {
	$library = Join-Path $PSScriptRoot 'gas-baseline-lib.ps1'
	$command = ". '$($library.Replace("'", "''"))'; try { `$lock = Enter-GasLock '$($Root.Replace("'", "''"))'; `$lock.Dispose(); exit 1 } catch { exit 0 }"
	$encoded = [Convert]::ToBase64String([Text.Encoding]::Unicode.GetBytes($command))
	& (Join-Path $PSHOME 'pwsh') -NoProfile -EncodedCommand $encoded
	if ($LASTEXITCODE -ne 0) { throw 'Concurrent process acquired an already held lock.' }
}
finally { $lock.Dispose() }
$lock = Enter-GasLock $Root
$lock.Dispose()
Write-Host 'Transaction rollback and inter-process exclusion verified.'
