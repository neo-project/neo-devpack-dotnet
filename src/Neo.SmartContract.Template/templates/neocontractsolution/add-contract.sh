#!/bin/bash

# Default values
AUTHOR="Your Name"
EMAIL="your.email@example.com"
DESCRIPTION="A NEO N3 smart contract"

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -n|--name)
            CONTRACT_NAME="$2"
            shift 2
            ;;
        -a|--author)
            AUTHOR="$2"
            shift 2
            ;;
        -e|--email)
            EMAIL="$2"
            shift 2
            ;;
        -d|--description)
            DESCRIPTION="$2"
            shift 2
            ;;
        -h|--help)
            echo "Usage: $0 -n CONTRACT_NAME [-a AUTHOR] [-e EMAIL] [-d DESCRIPTION]"
            echo ""
            echo "Options:"
            echo "  -n, --name         Contract name (required)"
            echo "  -a, --author       Author name (default: Your Name)"
            echo "  -e, --email        Author email (default: your.email@example.com)"
            echo "  -d, --description  Contract description (default: A NEO N3 smart contract)"
            echo "  -h, --help         Show this help message"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

# Validate contract name
if [ -z "$CONTRACT_NAME" ]; then
    echo "Error: Contract name is required. Use -n or --name option."
    exit 1
fi

if ! [[ "$CONTRACT_NAME" =~ ^[a-zA-Z][a-zA-Z0-9]*$ ]]; then
    echo "Error: Contract name must start with a letter and contain only letters and numbers"
    exit 1
fi

# Check if we're in a solution directory
if ! ls *.sln >/dev/null 2>&1; then
    echo "Error: No solution file found. Please run this script from the solution root directory"
    exit 1
fi

SOLUTION_FILE=$(ls *.sln | head -n 1)
echo "Found solution: $SOLUTION_FILE"

# Check if contract already exists
if [ -d "src/$CONTRACT_NAME" ]; then
    echo "Error: Contract '$CONTRACT_NAME' already exists in src/"
    exit 1
fi

echo "Creating new contract: $CONTRACT_NAME"

# Create contract project
echo "Creating contract project..."
mkdir -p "src/$CONTRACT_NAME"

# Create contract class
cat > "src/$CONTRACT_NAME/$CONTRACT_NAME.cs" << EOF
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
    [DisplayName("$CONTRACT_NAME")]
    [ManifestExtra("Author", "$AUTHOR")]
    [ManifestExtra("Email", "$EMAIL")]
    [ManifestExtra("Description", "$DESCRIPTION")]
    [ContractPermission("*", "*")]
    public class $CONTRACT_NAME : SmartContract
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
EOF

# Create project file
cat > "src/$CONTRACT_NAME/$CONTRACT_NAME.csproj" << 'EOF'
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
    <Exec Command="dotnet rncc &quot;$(MSBuildProjectFullPath)&quot; -o &quot;$(SolutionDir)deploy/contracts&quot;" />
  </Target>

</Project>
EOF

# Create test project
echo "Creating test project..."
mkdir -p "tests/$CONTRACT_NAME.Tests"

# Create test class
cat > "tests/$CONTRACT_NAME.Tests/${CONTRACT_NAME}Tests.cs" << EOF
using System.Numerics;
using Neo;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Xunit;

namespace NeoContractSolution.Tests
{
    public class ${CONTRACT_NAME}Tests : TestBase<$CONTRACT_NAME>
    {
        private readonly TestEngine _engine;
        private readonly UInt160 _defaultOwner;

        public ${CONTRACT_NAME}Tests()
        {
            _engine = new TestEngine();
            _engine.OnRuntimeLogMessage += OnRuntimeLog;
            
            _defaultOwner = _engine.GetDefaultAccount("owner").ScriptHash;
            _engine.SetTransactionSigners(_defaultOwner);
        }

        [Fact]
        public void Test_Deploy()
        {
            var contract = _engine.Deploy<$CONTRACT_NAME>(new Neo.SmartContract.Testing.Storage.NonPersistentStorageProvider());
            Assert.NotNull(contract);
        }

        [Fact]
        public void Test_Initialize()
        {
            var contract = _engine.Deploy<$CONTRACT_NAME>(new Neo.SmartContract.Testing.Storage.NonPersistentStorageProvider());
            
            var result = contract.Initialize();
            Assert.True(result);
            
            Assert.Equal(_defaultOwner, contract.GetOwner());
        }

        private void OnRuntimeLog(TestEngine engine, string message)
        {
            Console.WriteLine($"[Runtime] {message}");
        }
    }

    public abstract class $CONTRACT_NAME : SmartContract.Testing.SmartContract
    {
        public abstract UInt160 GetOwner();
        public abstract bool Initialize();
        public abstract bool Update(byte[] nefFile, string manifest, object? data = null);
        public abstract bool Destroy();
    }
}
EOF

# Create test project file
cat > "tests/$CONTRACT_NAME.Tests/$CONTRACT_NAME.Tests.csproj" << 'EOF'
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
EOF

# Add projects to solution
echo "Adding projects to solution..."
dotnet sln add "src/$CONTRACT_NAME/$CONTRACT_NAME.csproj"
dotnet sln add "tests/$CONTRACT_NAME.Tests/$CONTRACT_NAME.Tests.csproj"

# Create deployment step
echo "Creating deployment step..."
cat > "deploy/Deploy/Steps/Deploy${CONTRACT_NAME}Step.cs" << EOF
using Microsoft.Extensions.Logging;
using Neo;
using NeoContractSolution.Deploy.Services;

namespace NeoContractSolution.Deploy.Steps
{
    public class Deploy${CONTRACT_NAME}Step : IDeploymentStep
    {
        private readonly ILogger<Deploy${CONTRACT_NAME}Step> _logger;

        public Deploy${CONTRACT_NAME}Step(ILogger<Deploy${CONTRACT_NAME}Step> logger)
        {
            _logger = logger;
        }

        public string Name => "Deploy $CONTRACT_NAME";
        public int Order => 20; // Adjust order as needed

        public async Task<bool> ExecuteAsync(DeploymentContext context)
        {
            try
            {
                var contract = await context.ContractLoader.LoadContractAsync("$CONTRACT_NAME");
                
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
                _logger.LogError(ex, "Failed to deploy $CONTRACT_NAME");
                return false;
            }
        }
    }
}
EOF

echo ""
echo "Contract '$CONTRACT_NAME' added successfully!"
echo ""
echo "Next steps:"
echo "1. Update deploy/Deploy/Program.cs to register the new deployment step:"
echo "   services.AddTransient<IDeploymentStep, Deploy${CONTRACT_NAME}Step>();"
echo "2. Implement your contract logic in src/$CONTRACT_NAME/$CONTRACT_NAME.cs"
echo "3. Add tests in tests/$CONTRACT_NAME.Tests/${CONTRACT_NAME}Tests.cs"
echo "4. Build the solution: dotnet build"
echo "5. Run tests: dotnet test"
echo "6. Deploy: cd deploy/Deploy && dotnet run"