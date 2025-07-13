param(
    [Parameter(Mandatory=$true)]
    [string]$ContractName,
    
    [string]$Author = "Your Name",
    
    [string]$Email = "your.email@example.com",
    
    [string]$Description = "A NEO N3 smart contract"
)

$ErrorActionPreference = "Stop"

# Validate contract name
if ($ContractName -notmatch '^[a-zA-Z][a-zA-Z0-9]*$') {
    Write-Error "Contract name must start with a letter and contain only letters and numbers"
    exit 1
}

# Check if we're in a solution directory
if (-not (Test-Path "*.sln")) {
    Write-Error "No solution file found. Please run this script from the solution root directory"
    exit 1
}

$solutionFile = Get-ChildItem "*.sln" | Select-Object -First 1
Write-Host "Found solution: $($solutionFile.Name)" -ForegroundColor Green

# Check if contract already exists
if (Test-Path "src/$ContractName") {
    Write-Error "Contract '$ContractName' already exists in src/"
    exit 1
}

Write-Host "Creating new contract: $ContractName" -ForegroundColor Yellow

# Create contract project
Write-Host "Creating contract project..." -ForegroundColor Cyan
New-Item -ItemType Directory -Path "src/$ContractName" -Force | Out-Null

# Create contract class
$contractContent = @"
using System;
using System.ComponentModel;
using System.Numerics;
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace NeoContractSolution.Contracts
{
    [DisplayName("$ContractName")]
    [ManifestExtra("Author", "$Author")]
    [ManifestExtra("Email", "$Email")]
    [ManifestExtra("Description", "$Description")]
    [ContractPermission("*", "*")]
    public class $ContractName : SmartContract
    {
        private const byte Prefix_Owner = 0x01;
        
        [InitialValue("NhNg7GgRV1VUGNqjGxqgJfbgRoPyJgJVPq", ContractParameterType.Hash160)]
        private static readonly UInt160 InitialOwner = default!;

        [Safe]
        public static UInt160 GetOwner()
        {
            return (UInt160)Storage.Get(Storage.CurrentContext, new byte[] { Prefix_Owner });
        }

        public static bool Initialize()
        {
            if (GetOwner() != UInt160.Zero)
                throw new Exception("Contract already initialized");

            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_Owner }, InitialOwner);
            return true;
        }

        public static bool Update(ByteString nefFile, string manifest, object? data = null)
        {
            var owner = GetOwner();
            ExecutionEngine.Assert(Runtime.CheckWitness(owner), "Only owner can update");
            
            ContractManagement.Update(nefFile, manifest, data);
            return true;
        }

        public static bool Destroy()
        {
            var owner = GetOwner();
            ExecutionEngine.Assert(Runtime.CheckWitness(owner), "Only owner can destroy");
            
            ContractManagement.Destroy();
            return true;
        }
    }
}
"@

Set-Content -Path "src/$ContractName/$ContractName.cs" -Value $contractContent

# Create project file
$projectContent = @"
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <LangVersion>latest</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Neo.SmartContract.Framework" Version="3.8.1-*" />
  </ItemGroup>

  <Target Name="PostBuild" AfterTargets="PostBuildEvent">
    <Message Text="Compiling smart contract..." Importance="high" />
    <Exec Command="dotnet rncc &quot;`$(MSBuildProjectFullPath)&quot; -o &quot;`$(SolutionDir)deploy\contracts&quot;" />
  </Target>

</Project>
"@

Set-Content -Path "src/$ContractName/$ContractName.csproj" -Value $projectContent

# Create test project
Write-Host "Creating test project..." -ForegroundColor Cyan
New-Item -ItemType Directory -Path "tests/$ContractName.Tests" -Force | Out-Null

# Create test class
$testContent = @"
using System.Numerics;
using Neo;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Xunit;

namespace NeoContractSolution.Tests
{
    public class ${ContractName}Tests : TestBase<$ContractName>
    {
        private readonly TestEngine _engine;
        private readonly UInt160 _defaultOwner;

        public ${ContractName}Tests()
        {
            _engine = new TestEngine();
            _engine.OnRuntimeLogMessage += OnRuntimeLog;
            
            _defaultOwner = _engine.GetDefaultAccount("owner").ScriptHash;
            _engine.SetTransactionSigners(_defaultOwner);
        }

        [Fact]
        public void Test_Deploy()
        {
            var contract = _engine.Deploy<$ContractName>(new Neo.SmartContract.Testing.Storage.NonPersistentStorageProvider());
            Assert.NotNull(contract);
        }

        [Fact]
        public void Test_Initialize()
        {
            var contract = _engine.Deploy<$ContractName>(new Neo.SmartContract.Testing.Storage.NonPersistentStorageProvider());
            
            var result = contract.Initialize();
            Assert.True(result);
            
            Assert.Equal(_defaultOwner, contract.GetOwner());
        }

        private void OnRuntimeLog(TestEngine engine, string message)
        {
            Console.WriteLine(`$"[Runtime] {message}");
        }
    }

    public abstract class $ContractName : SmartContract.Testing.SmartContract
    {
        public abstract UInt160 GetOwner();
        public abstract bool Initialize();
        public abstract bool Update(byte[] nefFile, string manifest, object? data = null);
        public abstract bool Destroy();
    }
}
"@

Set-Content -Path "tests/$ContractName.Tests/${ContractName}Tests.cs" -Value $testContent

# Create test project file
$testProjectContent = @"
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.1" />
    <PackageReference Include="xunit" Version="2.9.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="coverlet.collector" Version="6.0.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Neo.SmartContract.Testing" Version="3.8.1-*" />
  </ItemGroup>

  <ItemGroup>
    <Using Include="Xunit" />
  </ItemGroup>

</Project>
"@

Set-Content -Path "tests/$ContractName.Tests/$ContractName.Tests.csproj" -Value $testProjectContent

# Add projects to solution
Write-Host "Adding projects to solution..." -ForegroundColor Cyan

# Generate GUIDs for the projects
$contractGuid = [guid]::NewGuid().ToString().ToUpper()
$testGuid = [guid]::NewGuid().ToString().ToUpper()

# Add to solution
dotnet sln add "src/$ContractName/$ContractName.csproj"
dotnet sln add "tests/$ContractName.Tests/$ContractName.Tests.csproj"

# Create deployment step
Write-Host "Creating deployment step..." -ForegroundColor Cyan

$deployStepContent = @"
using Microsoft.Extensions.Logging;
using Neo;
using NeoContractSolution.Deploy.Services;

namespace NeoContractSolution.Deploy.Steps
{
    public class Deploy${ContractName}Step : IDeploymentStep
    {
        private readonly ILogger<Deploy${ContractName}Step> _logger;

        public Deploy${ContractName}Step(ILogger<Deploy${ContractName}Step> logger)
        {
            _logger = logger;
        }

        public string Name => "Deploy $ContractName";
        public int Order => 20; // Adjust order as needed

        public async Task<bool> ExecuteAsync(DeploymentContext context)
        {
            try
            {
                var contract = await context.ContractLoader.LoadContractAsync("$ContractName");
                
                var deployResult = await context.DeploymentService.DeployContractAsync(
                    contract.Name,
                    contract.NefBytes,
                    contract.Manifest
                );

                if (!deployResult.Success)
                {
                    _logger.LogError("Failed to deploy {Name}: {Error}", contract.Name, deployResult.ErrorMessage);
                    return false;
                }

                context.DeployedContracts[contract.Name] = deployResult.Hash!;
                _logger.LogInformation("Contract deployed: {Name} at {Hash}", contract.Name, deployResult.Hash);

                // Initialize the contract
                _logger.LogInformation("Initializing contract...");
                var initTx = await context.DeploymentService.InvokeContractAsync(
                    deployResult.Hash!,
                    "initialize"
                );

                _logger.LogInformation("Contract initialized. Transaction: {TxId}", initTx);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deploy $ContractName");
                return false;
            }
        }
    }
}
"@

Set-Content -Path "deploy/Deploy/Steps/Deploy${ContractName}Step.cs" -Value $deployStepContent

Write-Host "`nContract '$ContractName' added successfully!" -ForegroundColor Green
Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "1. Update deploy/Deploy/Program.cs to register the new deployment step:"
Write-Host "   services.AddTransient<IDeploymentStep, Deploy${ContractName}Step>();" -ForegroundColor Cyan
Write-Host "2. Implement your contract logic in src/$ContractName/$ContractName.cs"
Write-Host "3. Add tests in tests/$ContractName.Tests/${ContractName}Tests.cs"
Write-Host "4. Build the solution: dotnet build"
Write-Host "5. Run tests: dotnet test"
Write-Host "6. Deploy: cd deploy/Deploy && dotnet run"
"@