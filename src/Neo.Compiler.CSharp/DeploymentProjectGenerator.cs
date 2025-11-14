using System;
using System.Collections.Generic;
using System.IO;

namespace Neo.Compiler
{
    internal static class DeploymentProjectGenerator
    {
        private const string DeployPackageVersion = "3.8.1-*";

        public static int Generate(string projectName, string outputDirectory, bool force)
        {
            string projectPath = Path.Combine(outputDirectory, projectName);
            if (Directory.Exists(projectPath))
            {
                if (!force)
                {
                    Console.Error.WriteLine($"Error: Directory '{projectPath}' already exists. Use --force to overwrite.");
                    return 1;
                }
            }
            else
            {
                Directory.CreateDirectory(projectPath);
            }

            File.WriteAllText(Path.Combine(projectPath, $"{projectName}.csproj"), BuildProjectFile());
            File.WriteAllText(Path.Combine(projectPath, "Program.cs"), BuildProgramFile());
            File.WriteAllText(Path.Combine(projectPath, "deploysettings.json"), BuildConfigFile());

            Console.WriteLine($"Created deployment project '{projectName}' in {projectPath}");
            Console.WriteLine();
            Console.WriteLine("Next steps:");
            Console.WriteLine($"  1. Update {Path.Combine(projectName, "Program.cs")} to point at your NEF/manifest paths.");
            Console.WriteLine($"  2. Update {Path.Combine(projectName, "deploysettings.json")} with RPC endpoints and private keys.");
            Console.WriteLine($"  3. dotnet run --project {Path.Combine(projectPath, $"{projectName}.csproj")} -- --network testnet --nef <path> --manifest <path>");
            return 0;
        }

        private static string BuildProjectFile()
        {
            return $@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Neo.SmartContract.Deploy"" Version=""{DeployPackageVersion}"" />
  </ItemGroup>

</Project>
";
        }

        private static string BuildProgramFile()
        {
            return @"using Neo.SmartContract.Deploy;
using System;
using System.Collections.Generic;
using System.IO;

const string ContractProjectName = ""MyContract"";
var defaultArtifacts = Path.Combine("".."", ContractProjectName, ""bin"", ""Debug"", ""net10.0"");
var defaultNef = Path.GetFullPath(Path.Combine(defaultArtifacts, $""{ContractProjectName}.nef""));
var defaultManifest = Path.GetFullPath(Path.Combine(defaultArtifacts, $""{ContractProjectName}.manifest.json""));

var options = DeploymentOptions.Parse(args, ""deploysettings.json"", defaultNef, defaultManifest);

var toolkit = File.Exists(options.ConfigPath)
    ? DeploymentToolkit.FromConfigFile(options.ConfigPath)
    : new DeploymentToolkit();

toolkit.UseConsolePrivateKeyPrompt(""Enter the deployment private key (hex or WIF): "");

if (!string.IsNullOrWhiteSpace(options.Network))
{
    toolkit.UseNetwork(options.Network);
}

if (!string.IsNullOrWhiteSpace(options.PrivateKey))
{
    toolkit.UsePrivateKey(options.PrivateKey);
}

if (!string.IsNullOrWhiteSpace(options.RpcEndpoint))
{
    toolkit.UseRpcEndpoint(options.RpcEndpoint);
}

var artifacts = await ContractArtifacts.FromFilesAsync(options.NefPath, options.ManifestPath);
var deployment = await toolkit.DeployAsync(artifacts);

Console.WriteLine($""Contract hash: {deployment.ContractHash}"");
Console.WriteLine($""Transaction: {deployment.TransactionHash}"");

var invokeResult = await toolkit.TestInvokeAsync(deployment.ContractHash, options.InvokeMethod, options.InvokeArguments.ToArray());
Console.WriteLine(""Read-only invocation result:"");
Console.WriteLine(invokeResult.ToJson());

internal sealed class DeploymentOptions
{
    private readonly List<object> _invokeArgs = new();

    private DeploymentOptions(string configPath, string nefPath, string manifestPath)
    {
        ConfigPath = configPath;
        NefPath = nefPath;
        ManifestPath = manifestPath;
    }

    public string ConfigPath { get; private set; }
    public string NefPath { get; private set; }
    public string ManifestPath { get; private set; }
    public string? Network { get; private set; }
    public string? PrivateKey { get; private set; }
    public string? RpcEndpoint { get; private set; }
    public string InvokeMethod { get; private set; } = ""symbol"";
    public IReadOnlyList<object> InvokeArguments => _invokeArgs;

    public static DeploymentOptions Parse(string[] args, string defaultConfig, string defaultNef, string defaultManifest)
    {
        var options = new DeploymentOptions(defaultConfig, defaultNef, defaultManifest);
        for (int i = 0; i < args.Length; i++)
        {
            var token = args[i];
            switch (token)
            {
                case ""--config"":
                    options.ConfigPath = RequireValue(args, ref i, token);
                    break;
                case ""--network"":
                    options.Network = RequireValue(args, ref i, token);
                    break;
                case ""--nef"":
                    options.NefPath = RequireValue(args, ref i, token);
                    break;
                case ""--manifest"":
                    options.ManifestPath = RequireValue(args, ref i, token);
                    break;
                case ""--private-key"":
                    options.PrivateKey = RequireValue(args, ref i, token);
                    break;
                case ""--rpc"":
                    options.RpcEndpoint = RequireValue(args, ref i, token);
                    break;
                case ""--invoke-method"":
                    options.InvokeMethod = RequireValue(args, ref i, token);
                    break;
                case ""--invoke-arg"":
                    options._invokeArgs.Add(RequireValue(args, ref i, token));
                    break;
                case ""-h"":
                case ""--help"":
                    PrintUsage();
                    Environment.Exit(0);
                    break;
                default:
                    Console.Error.WriteLine($""Unrecognized argument '{{token}}'."");
                    PrintUsage();
                    Environment.Exit(1);
                    break;
            }
        }

        return options;
    }

    private static string RequireValue(string[] args, ref int index, string option)
    {
        if (index + 1 >= args.Length)
        {
            throw new ArgumentException($""Missing value for {option}."");
        }

        index++;
        return args[index];
    }

    private static void PrintUsage()
    {
        Console.WriteLine(""Deployment project options:"");
        Console.WriteLine(""  --config <path>        Path to deploysettings.json (default deploysettings.json)"");
        Console.WriteLine(""  --network <name>       Network name defined in config file"");
        Console.WriteLine(""  --nef <path>           Path to compiled .nef file"");
        Console.WriteLine(""  --manifest <path>      Path to compiled manifest file"");
        Console.WriteLine(""  --private-key <value>  Hex or WIF private key (overrides config)"");
        Console.WriteLine(""  --rpc <url>            Override RPC endpoint"");
        Console.WriteLine(""  --invoke-method <name> Method to test invoke after deployment"");
        Console.WriteLine(""  --invoke-arg <value>   Adds an argument for the invoke method (repeatable)"");
    }
}
";
        }

        private static string BuildConfigFile()
        {
            return @"{
  ""deployment"": {
    ""defaultNetwork"": ""testnet"",
    ""networks"": {
      ""mainnet"": {
        ""rpcUrls"": [ ""https://mainnet1.neo.org:443"" ],
        ""networkMagic"": 860833102
      },
      ""testnet"": {
        ""rpcUrls"": [ ""https://testnet1.neo.org:443"" ],
        ""networkMagic"": 894710606,
        ""privateKey"": ""<insert testnet WIF or hex private key>""
      },
      ""devnet"": {
        ""rpcUrls"": [ ""http://localhost:50012"" ],
        ""networkMagic"": 0
      }
    }
  }
}
";
        }
    }
}
