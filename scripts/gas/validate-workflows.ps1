# Parse PowerShell run blocks without executing workflow commands or requiring a YAML module.
param([string]$WorkflowDirectory = (Join-Path $PSScriptRoot '../../.github/workflows'))
$ErrorActionPreference = 'Stop'
$failures = [System.Collections.Generic.List[string]]::new()
foreach ($file in Get-ChildItem -LiteralPath $WorkflowDirectory -Filter 'gas-baseline-*.yml') {
	$text = [IO.File]::ReadAllText($file.FullName)
	foreach ($match in [regex]::Matches($text, '(?m)^      shell: pwsh\r?\n(?:(?!      run:).|\r?\n)*?      run: \|\r?\n((?:        .*\r?\n|\r?\n)+)')) {
		$script = [regex]::Replace($match.Groups[1].Value, '(?m)^        ', '')
		$script = [regex]::Replace($script, '\$\{\{.*?\}\}', 'placeholder')
		$tokens = $null; $errors = $null
		[void][System.Management.Automation.Language.Parser]::ParseInput($script, [ref]$tokens, [ref]$errors)
		foreach ($error in $errors) { $failures.Add("$($file.Name): $($error.Message)") }
	}
}
if ($failures.Count) { throw ($failures -join "`n") }
Write-Host 'Workflow PowerShell blocks parsed successfully.'
