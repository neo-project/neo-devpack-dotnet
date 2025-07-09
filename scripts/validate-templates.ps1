# Validate Neo Smart Contract Templates
# This script validates all templates and ensures they work correctly

param(
    [switch]$Verbose = $false
)

# Script configuration
$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectRoot = Split-Path -Parent $ScriptDir
$TempDir = Join-Path $env:TEMP "neo-template-validation-$(Get-Random)"

# Create temp directory
New-Item -ItemType Directory -Path $TempDir -Force | Out-Null

# Cleanup on exit
trap {
    if (Test-Path $TempDir) {
        Remove-Item -Path $TempDir -Recurse -Force -ErrorAction SilentlyContinue
    }
}

Write-Host "Neo Smart Contract Template Validation" -ForegroundColor Blue
Write-Host "======================================" -ForegroundColor Blue
Write-Host ""

# Function to validate a template
function Test-Template {
    param(
        [string]$TemplateName,
        [string]$TemplatePath,
        [string]$TestName,
        [hashtable]$Parameters = @{}
    )
    
    Write-Host "Testing template: $TemplateName" -ForegroundColor Yellow
    
    # Create test project directory
    $TestDir = Join-Path $TempDir $TestName
    New-Item -ItemType Directory -Path $TestDir -Force | Out-Null
    
    # Copy template files
    Copy-Item -Path "$TemplatePath\*" -Destination $TestDir -Recurse -Force
    
    # Apply transformations
    Get-ChildItem -Path $TestDir -Recurse -Include "*.cs","*.csproj","*.json","*.sln" | ForEach-Object {
        if (Test-Path $_.FullName -PathType Leaf) {
            $content = Get-Content $_.FullName -Raw -ErrorAction SilentlyContinue
            if ($content) {
                $content = $content -replace "MyContract", $TestName
                $content = $content -replace "NeoContractSolution", $TestName
                
                # Apply parameter replacements
                foreach ($key in $Parameters.Keys) {
                    $content = $content -replace "\`$$key\`$", $Parameters[$key]
                }
                
                Set-Content -Path $_.FullName -Value $content -Force
            }
        }
    }
    
    # Rename directories and files
    Get-ChildItem -Path $TestDir -Recurse | Sort-Object -Property FullName -Descending | ForEach-Object {
        if ($_.Name -like "*MyContract*") {
            $newName = $_.Name -replace "MyContract", $TestName
            $newPath = Join-Path $_.Directory.FullName $newName
            if ($_.FullName -ne $newPath) {
                Move-Item -Path $_.FullName -Destination $newPath -Force -ErrorAction SilentlyContinue
            }
        }
    }
    
    # Try to build
    Write-Host "  Building... " -NoNewline
    Push-Location $TestDir
    try {
        $output = & dotnet build --nologo --verbosity quiet 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓" -ForegroundColor Green
        } else {
            Write-Host "✗" -ForegroundColor Red
            Write-Host "  Build failed for $TemplateName" -ForegroundColor Red
            if ($Verbose) {
                Write-Host $output
            }
            return $false
        }
        
        # Try to run tests if they exist
        if (Test-Path "tests") {
            Write-Host "  Running tests... " -NoNewline
            $output = & dotnet test --no-build --nologo --verbosity quiet 2>&1
            if ($LASTEXITCODE -eq 0) {
                Write-Host "✓" -ForegroundColor Green
            } else {
                Write-Host "✗" -ForegroundColor Red
                Write-Host "  Tests failed for $TemplateName" -ForegroundColor Red
                if ($Verbose) {
                    Write-Host $output
                }
                return $false
            }
        }
    } finally {
        Pop-Location
    }
    
    Write-Host "  Template $TemplateName validated successfully!" -ForegroundColor Green
    return $true
}

# Function to check template structure
function Test-TemplateStructure {
    param(
        [string]$TemplatePath
    )
    
    $TemplateName = Split-Path -Leaf $TemplatePath
    Write-Host "Checking structure: $TemplateName" -ForegroundColor Yellow
    
    # Check for required files
    $requiredFiles = @(
        ".template.config\template.json"
    )
    
    foreach ($file in $requiredFiles) {
        $filePath = Join-Path $TemplatePath $file
        if (!(Test-Path $filePath)) {
            Write-Host "  Missing required file: $file" -ForegroundColor Red
            return $false
        }
    }
    
    # Validate template.json
    $templateJsonPath = Join-Path $TemplatePath ".template.config\template.json"
    try {
        $templateJson = Get-Content $templateJsonPath -Raw | ConvertFrom-Json
        
        # Check required fields
        $requiredFields = @("identity", "name", "shortName")
        foreach ($field in $requiredFields) {
            if (!$templateJson.$field) {
                Write-Host "  Missing required field in template.json: $field" -ForegroundColor Red
                return $false
            }
        }
    } catch {
        Write-Host "  Invalid JSON in template.json" -ForegroundColor Red
        return $false
    }
    
    Write-Host "  Structure OK" -ForegroundColor Green
    return $true
}

# Main validation
$TemplatesDir = Join-Path $ProjectRoot "src\Neo.SmartContract.Template\templates"
$FailedChecks = 0

Write-Host "1. Checking template structures..." -ForegroundColor White
Write-Host ""

Get-ChildItem -Path $TemplatesDir -Directory | ForEach-Object {
    if (!(Test-TemplateStructure -TemplatePath $_.FullName)) {
        $FailedChecks++
    }
}

Write-Host ""
Write-Host "2. Validating template functionality..." -ForegroundColor White
Write-Host ""

# Test neocontractsolution template with different contract types
$neoContractSolutionPath = Join-Path $TemplatesDir "neocontractsolution"
if (Test-Path $neoContractSolutionPath) {
    # Basic contract
    if (!(Test-Template -TemplateName "neocontractsolution-basic" `
                       -TemplatePath $neoContractSolutionPath `
                       -TestName "TestBasic" `
                       -Parameters @{contractType = "Basic"})) {
        $FailedChecks++
    }
    
    # NEP-17 Token
    if (!(Test-Template -TemplateName "neocontractsolution-token" `
                       -TemplatePath $neoContractSolutionPath `
                       -TestName "TestToken" `
                       -Parameters @{contractType = "NEP17"})) {
        $FailedChecks++
    }
    
    # NEP-11 NFT
    if (!(Test-Template -TemplateName "neocontractsolution-nft" `
                       -TemplatePath $neoContractSolutionPath `
                       -TestName "TestNFT" `
                       -Parameters @{contractType = "NEP11"})) {
        $FailedChecks++
    }
    
    # Governance
    if (!(Test-Template -TemplateName "neocontractsolution-governance" `
                       -TemplatePath $neoContractSolutionPath `
                       -TestName "TestGov" `
                       -Parameters @{contractType = "Governance"})) {
        $FailedChecks++
    }
}

# Test other templates
Get-ChildItem -Path $TemplatesDir -Directory | Where-Object { $_.Name -ne "neocontractsolution" } | ForEach-Object {
    $templateName = $_.Name
    $testName = "Test" + (Get-Culture).TextInfo.ToTitleCase($templateName)
    
    if (!(Test-Template -TemplateName $templateName `
                       -TemplatePath $_.FullName `
                       -TestName $testName)) {
        $FailedChecks++
    }
}

Write-Host ""
Write-Host "3. Security checks..." -ForegroundColor White
Write-Host ""

# Check for hardcoded private keys
Write-Host "  Checking for hardcoded private keys... " -NoNewline
$privateKeyPattern = "L[1-9A-HJ-NP-Za-km-z]{51}"
$foundKeys = Get-ChildItem -Path $TemplatesDir -Recurse -Include "*.cs","*.json" | 
    Select-String -Pattern $privateKeyPattern -Quiet

if ($foundKeys) {
    Write-Host "✗ Found potential private keys!" -ForegroundColor Red
    $FailedChecks++
} else {
    Write-Host "✓" -ForegroundColor Green
}

# Check for placeholder values
Write-Host "  Checking for placeholder values... " -NoNewline
$placeholders = @("YOUR_WIF_KEY", "YOUR_CONTRACT_HASH", "YOUR_ADDRESS", "TODO", "FIXME")
$foundPlaceholders = 0

foreach ($placeholder in $placeholders) {
    $found = Get-ChildItem -Path $TemplatesDir -Recurse -Include "*.cs","*.json" | 
        Select-String -Pattern $placeholder -Quiet
    
    if ($found) {
        Write-Host "Warning: Found placeholder '$placeholder'" -ForegroundColor Yellow
        $foundPlaceholders++
    }
}

if ($foundPlaceholders -eq 0) {
    Write-Host "✓" -ForegroundColor Green
}

Write-Host ""
Write-Host "4. Template packaging test..." -ForegroundColor White
Write-Host ""

# Try to pack the template project
Write-Host "  Packing template project... " -NoNewline
Push-Location "$ProjectRoot\src\Neo.SmartContract.Template"
try {
    $output = & dotnet pack --nologo --verbosity quiet 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓" -ForegroundColor Green
        
        # Check if package was created
        if (Get-ChildItem -Path "bin\Debug" -Filter "*.nupkg" -ErrorAction SilentlyContinue) {
            Write-Host "  Package created successfully" -ForegroundColor Green
        } else {
            Write-Host "  Package file not found" -ForegroundColor Red
            $FailedChecks++
        }
    } else {
        Write-Host "✗ Packing failed" -ForegroundColor Red
        if ($Verbose) {
            Write-Host $output
        }
        $FailedChecks++
    }
} finally {
    Pop-Location
}

# Summary
Write-Host ""
Write-Host "Validation Summary" -ForegroundColor Blue
Write-Host "==================" -ForegroundColor Blue

if ($FailedChecks -eq 0) {
    Write-Host "All template validations passed!" -ForegroundColor Green
    exit 0
} else {
    Write-Host "$FailedChecks validation(s) failed" -ForegroundColor Red
    exit 1
}