// Copyright (C) 2015-2025 The Neo Project.
//
// TemplateManager.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Neo.Compiler
{
    public enum ContractTemplate
    {
        Basic,
        NEP17,
        NEP11,
        Ownable,
        Oracle
    }

    public class TemplateManager
    {
        private const string NeoPackageVersion = "3.8.1-*";
        private readonly Dictionary<ContractTemplate, TemplateInfo> templates;

        public TemplateManager()
        {
            templates = new Dictionary<ContractTemplate, TemplateInfo>
            {
                [ContractTemplate.Basic] = new TemplateInfo
                {
                    Name = "Basic Contract",
                    Description = "A simple smart contract with basic functionality",
                    Files = new Dictionary<string, string>
                    {
                        ["{{ProjectName}}.cs"] = GetBasicContractTemplate(),
                        ["{{ProjectName}}.csproj"] = GetProjectFileTemplate()
                    }
                },
                [ContractTemplate.NEP17] = new TemplateInfo
                {
                    Name = "NEP-17 Token",
                    Description = "NEP-17 fungible token standard implementation",
                    Files = new Dictionary<string, string>
                    {
                        ["{{ProjectName}}.cs"] = GetNep17ContractTemplate(),
                        ["{{ProjectName}}.csproj"] = GetProjectFileTemplate()
                    }
                },
                [ContractTemplate.NEP11] = new TemplateInfo
                {
                    Name = "NEP-11 NFT",
                    Description = "NEP-11 non-fungible token standard implementation",
                    Files = new Dictionary<string, string>
                    {
                        ["{{ProjectName}}.cs"] = GetNep11ContractTemplate(),
                        ["{{ProjectName}}.csproj"] = GetProjectFileTemplate()
                    }
                },
                [ContractTemplate.Ownable] = new TemplateInfo
                {
                    Name = "Ownable Contract",
                    Description = "Contract with owner management functionality",
                    Files = new Dictionary<string, string>
                    {
                        ["{{ProjectName}}.cs"] = GetOwnableContractTemplate(),
                        ["{{ProjectName}}.csproj"] = GetProjectFileTemplate()
                    }
                },
                [ContractTemplate.Oracle] = new TemplateInfo
                {
                    Name = "Oracle Contract",
                    Description = "Contract that interacts with Oracle services",
                    Files = new Dictionary<string, string>
                    {
                        ["{{ProjectName}}.cs"] = GetOracleContractTemplate(),
                        ["{{ProjectName}}.csproj"] = GetProjectFileTemplate()
                    }
                }
            };
        }

        public class TemplateInfo
        {
            public string Name { get; set; } = "";
            public string Description { get; set; } = "";
            public Dictionary<string, string> Files { get; set; } = new Dictionary<string, string>();
        }

        public void GenerateContract(ContractTemplate template, string projectName, string outputPath, Dictionary<string, string>? additionalReplacements = null)
        {
            if (!templates.ContainsKey(template))
                throw new ArgumentException($"Unknown template: {template}");

            var templateInfo = templates[template];
            var replacements = new Dictionary<string, string>
            {
                { "{{ProjectName}}", projectName },
                { "{{Namespace}}", projectName },
                { "{{ClassName}}", projectName },
                { "{{Author}}", "Author" },
                { "{{Email}}", $"email@example.com" },
                { "{{Description}}", $"{projectName} Smart Contract" },
                { "{{Version}}", "1.0.0" },
                { "{{Year}}", DateTime.Now.Year.ToString() },
                { "TemplateNeoVersion", NeoPackageVersion }
            };

            if (additionalReplacements != null)
            {
                foreach (var kvp in additionalReplacements)
                {
                    replacements[kvp.Key] = kvp.Value;
                }
            }

            string projectPath = Path.Combine(outputPath, projectName);
            Directory.CreateDirectory(projectPath);

            foreach (var file in templateInfo.Files)
            {
                string fileName = ReplaceTokens(file.Key, replacements);
                string fileContent = ReplaceTokens(file.Value, replacements);
                string filePath = Path.Combine(projectPath, fileName);

                File.WriteAllText(filePath, fileContent);
                Console.WriteLine($"Created: {filePath}");
            }

            Console.WriteLine($"\nSuccessfully created {template} contract '{projectName}' in {projectPath}");
            Console.WriteLine("\nTo build your contract:");
            Console.WriteLine($"  dotnet build {Path.Combine(projectPath, projectName + ".csproj")}");
            Console.WriteLine("\nTo compile to NEF:");
            Console.WriteLine($"  dotnet run --project src/Neo.Compiler.CSharp/Neo.Compiler.CSharp.csproj -- {Path.Combine(projectPath, projectName + ".csproj")}");
        }

        public void GenerateDeploymentProject(string projectName, string outputPath, string contractProjectName, bool force)
        {
            if (string.IsNullOrWhiteSpace(projectName))
                throw new ArgumentException("Deployment project name cannot be empty.", nameof(projectName));

            if (!Regex.IsMatch(projectName, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
                throw new ArgumentException("Deployment project name must start with a letter and contain only letters, numbers, and underscores.", nameof(projectName));

            if (string.IsNullOrWhiteSpace(contractProjectName))
                throw new ArgumentException("Contract project name cannot be empty.", nameof(contractProjectName));

            if (!Regex.IsMatch(contractProjectName, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
                throw new ArgumentException("Contract project name must start with a letter and contain only letters, numbers, and underscores.", nameof(contractProjectName));

            string projectPath = Path.Combine(outputPath, projectName);
            if (Directory.Exists(projectPath) && !force)
                throw new InvalidOperationException($"Directory '{projectPath}' already exists. Use --force to overwrite.");

            Directory.CreateDirectory(projectPath);

            var replacements = new Dictionary<string, string>
            {
                { "{{DeployProjectName}}", projectName },
                { "{{Namespace}}", projectName },
                { "TemplateContractProject", contractProjectName },
                { "TemplateNeoVersion", NeoPackageVersion }
            };

            File.WriteAllText(Path.Combine(projectPath, $"{projectName}.csproj"), ReplaceTokens(GetDeployProjectFileTemplate(), replacements));
            File.WriteAllText(Path.Combine(projectPath, "Program.cs"), ReplaceTokens(GetDeployProgramTemplate(), replacements));
            File.WriteAllText(Path.Combine(projectPath, "deploysettings.json"), ReplaceTokens(GetDeploySettingsTemplate(), replacements));
            File.WriteAllText(Path.Combine(projectPath, "appsettings.json"), GetDeployAppSettingsTemplate());
            File.WriteAllText(Path.Combine(projectPath, "README.md"), ReplaceTokens(GetDeployReadmeTemplate(), replacements));

            Console.WriteLine($"\nSuccessfully created deployment project '{projectName}' in {projectPath}");
            Console.WriteLine("To run deployments:");
            Console.WriteLine($"  cd {projectPath}");
            Console.WriteLine("  dotnet run -- deploy --network express --wif <your-wif>");
        }

        private string ReplaceTokens(string content, Dictionary<string, string> replacements)
        {
            foreach (var replacement in replacements)
            {
                content = content.Replace(replacement.Key, replacement.Value);
            }
            return content;
        }

        public IEnumerable<(ContractTemplate template, string name, string description)> GetAvailableTemplates()
        {
            return templates.Select(t => (t.Key, t.Value.Name, t.Value.Description));
        }

        private static string GetProjectFileTemplate()
        {
            return @"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Neo.SmartContract.Framework"" Version=""TemplateNeoVersion"" />
  </ItemGroup>

  <Target Name=""PostBuild"" AfterTargets=""PostBuildEvent"">
    <Message Text=""Smart contract project {{ProjectName}} built successfully"" Importance=""high"" />
  </Target>

</Project>";
        }

        private static string GetDeployProjectFileTemplate() => @"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Neo.SmartContract.Deploy"" Version=""TemplateNeoVersion"" />
  </ItemGroup>

</Project>";

        private static string GetDeployProgramTemplate() => """""
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.Json;
using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Deploy;

namespace {{DeployProjectName}};

internal sealed class Program
{
    private const string DefaultSettingsFile = ""deploysettings.json"";

    private static async Task<int> Main(string[] args)
    {
        if (args.Length == 0 || IsHelp(args[0]))
        {
            PrintHelp();
            return 0;
        }

        var command = args[0].ToLowerInvariant();
        var options = ArgSet.Parse(args.Skip(1).ToArray());
        var settingsPath = options.Get(""config"") ?? DefaultSettingsFile;
        var settings = DeploySettings.Load(settingsPath);

        using var cancellation = new CancellationTokenSource();
        Console.CancelKeyPress += (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cancellation.Cancel();
        };

        try
        {
            switch (command)
            {
                case ""deploy"":
                    await RunDeployAsync(settings, options, cancellation.Token).ConfigureAwait(false);
                    break;
                case ""invoke"":
                    await RunInvokeAsync(settings, options, cancellation.Token).ConfigureAwait(false);
                    break;
                case ""call"":
                    await RunCallAsync(settings, options, cancellation.Token).ConfigureAwait(false);
                    break;
                default:
                    Console.Error.WriteLine($""Unknown command '{command}'."");
                    PrintHelp();
                    return 1;
            }

            return 0;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine(""Operation cancelled."");
            return 1;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($""Error: {ex.Message}"");
            return 1;
        }
    }

    private static bool IsHelp(string? argument)
        => string.IsNullOrWhiteSpace(argument)
           || argument.Equals(""-h"", StringComparison.OrdinalIgnoreCase)
           || argument.Equals(""--help"", StringComparison.OrdinalIgnoreCase);

    private static void PrintHelp()
    {
        const string help = \"\"\""
Deployment toolkit helper

Usage:
  dotnet run -- [command] [options]

Commands:
  deploy   Deploy compiled artifacts pointed to by deploysettings.json
  invoke   Invoke a state-changing method on a deployed contract
  call     Call a read-only method on a deployed contract

Common options:
  --config <path>        Path to deploysettings.json (defaults to ./deploysettings.json)
  --network <name|url>   Network alias (express, mainnet, testnet, custom name) or RPC URL
  --rpc <url>            Override RPC endpoint directly

Deploy options:
  --nef <path>           Override NEF artifact path
  --manifest <path>      Override manifest path
  --init-params <json>   JSON array of initialization parameters
  --wait                 Wait for confirmation (flag)
  --retries <number>     Confirmation retries
  --delay <seconds>      Delay between confirmation polls
  --wif <key>            WIF to use for signing (can also use NEO_WIF env var)

Invoke options:
  --contract <hash>      Target contract hash or address
  --method <name>        Method to invoke
  --args <json>          JSON array of arguments
  --wif <key>            WIF used for signing (can also use NEO_WIF env var)

Call options:
  --contract <hash>      Target contract hash or address
  --method <name>        Method to call
  --args <json>          JSON array of arguments
  --return <type>        Expected return type (string, bool, int, long, bigint, bytes) – defaults to string
\"\"\";

        Console.WriteLine(help);
    }

    private static async Task RunDeployAsync(DeploySettings settings, ArgSet options, CancellationToken cancellationToken)
    {
        using var toolkit = new DeploymentToolkit();
        var selection = ConfigureNetwork(toolkit, settings, options);
        EnsureWif(toolkit, settings, options, selection);

        var artifacts = settings.ResolveArtifacts();
        var nef = settings.ResolvePath(options.Get(""nef"")) ?? artifacts.Nef;
        var manifest = settings.ResolvePath(options.Get(""manifest"")) ?? artifacts.Manifest;

        if (string.IsNullOrWhiteSpace(nef))
            throw new InvalidOperationException(""Set the NEF path via deploysettings.json or the --nef option."");
        if (string.IsNullOrWhiteSpace(manifest))
            throw new InvalidOperationException(""Set the manifest path via deploysettings.json or the --manifest option."");

        if (!File.Exists(nef))
            throw new FileNotFoundException($""NEF file not found at {nef}. Build your contract before deploying."");
        if (!File.Exists(manifest))
            throw new FileNotFoundException($""Manifest file not found at {manifest}. Build your contract before deploying."");

        var initParams = ParameterParser.ResolveArray(options.Get(""init-params""), artifacts.InitParameters);
        var waitForConfirmation = options.GetBool(""wait"") ?? artifacts.WaitForConfirmation;
        var retries = options.GetInt(""retries"") ?? artifacts.ConfirmationRetries;
        var delaySeconds = options.GetInt(""delay"") ?? artifacts.ConfirmationDelaySeconds;

        var request = new DeploymentArtifactsRequest(nef, manifest, initParams)
            .WithConfirmationPolicy(waitForConfirmation, retries, delaySeconds);

        Console.WriteLine(""Deploying artifacts:"");
        Console.WriteLine($""  NEF:      {nef}"");
        Console.WriteLine($""  Manifest: {manifest}"");
        Console.WriteLine($""  Network:  {toolkit.CurrentNetwork.Identifier} ({toolkit.CurrentNetwork.RpcUrl})"");

        var result = await toolkit.DeployArtifactsAsync(request, cancellationToken).ConfigureAwait(false);

        Console.WriteLine(""Deployment submitted:"");
        Console.WriteLine($""  Transaction: {result.TransactionHash}"");
        if (result.ContractHash is not null)
        {
            Console.WriteLine($""  Expected contract hash: {result.ContractHash}"");
        }
    }

    private static async Task RunInvokeAsync(DeploySettings settings, ArgSet options, CancellationToken cancellationToken)
    {
        var contract = options.Require(""contract"");
        var method = options.Require(""method"");
        var arguments = ParameterParser.ResolveArray(options.Get(""args""), fallback: null);

        using var toolkit = new DeploymentToolkit();
        var selection = ConfigureNetwork(toolkit, settings, options);
        EnsureWif(toolkit, settings, options, selection);

        Console.WriteLine($""Invoking {method} on {contract}..."");
        var txHash = await toolkit.InvokeAsync(contract, method, cancellationToken, arguments ?? Array.Empty<object?>()).ConfigureAwait(false);
        Console.WriteLine($""Transaction submitted: {txHash}"");
    }

    private static async Task RunCallAsync(DeploySettings settings, ArgSet options, CancellationToken cancellationToken)
    {
        var contract = options.Require(""contract"");
        var method = options.Require(""method"");
        var returnType = options.Get(""return"") ?? ""string"";
        var arguments = ParameterParser.ResolveArray(options.Get(""args""), fallback: null) ?? Array.Empty<object?>();

        using var toolkit = new DeploymentToolkit();
        _ = ConfigureNetwork(toolkit, settings, options);

        Console.WriteLine($""Calling {method} on {contract}..."");
        var normalizedReturn = returnType.ToLowerInvariant();
        object? result = normalizedReturn switch
        {
            ""bool"" or ""boolean"" => await toolkit.CallAsync<bool>(contract, method, cancellationToken, arguments).ConfigureAwait(false),
            ""int"" => await toolkit.CallAsync<int>(contract, method, cancellationToken, arguments).ConfigureAwait(false),
            ""long"" => await toolkit.CallAsync<long>(contract, method, cancellationToken, arguments).ConfigureAwait(false),
            ""bigint"" => await toolkit.CallAsync<BigInteger>(contract, method, cancellationToken, arguments).ConfigureAwait(false),
            ""bytes"" => Convert.ToHexString(await toolkit.CallAsync<byte[]>(contract, method, cancellationToken, arguments).ConfigureAwait(false)),
            _ => await toolkit.CallAsync<string>(contract, method, cancellationToken, arguments).ConfigureAwait(false)
        };

        Console.WriteLine($""Result ({normalizedReturn}): {result}"");
    }

    private static NetworkSelection ConfigureNetwork(DeploymentToolkit toolkit, DeploySettings settings, ArgSet options)
    {
        var rpcOverride = options.Get(""rpc"");
        if (!string.IsNullOrWhiteSpace(rpcOverride))
        {
            toolkit.SetNetwork(rpcOverride);
            return new NetworkSelection(null, null);
        }

        var requested = options.Get(""network"") ?? settings.DefaultNetwork;
        if (string.IsNullOrWhiteSpace(requested))
        {
            toolkit.UseNetwork(NetworkProfile.Local);
            return new NetworkSelection(""local"", settings.ResolveNetworkWif(""local""));
        }

        if (settings.TryGetCustomNetwork(requested, out var profile, out var configuredWif))
        {
            toolkit.UseNetwork(profile);
            return new NetworkSelection(requested, configuredWif);
        }

        switch (requested.ToLowerInvariant())
        {
            case ""mainnet"":
                toolkit.UseNetwork(NetworkProfile.MainNet);
                return new NetworkSelection(""mainnet"", settings.ResolveNetworkWif(""mainnet""));
            case ""testnet"":
                toolkit.UseNetwork(NetworkProfile.TestNet);
                return new NetworkSelection(""testnet"", settings.ResolveNetworkWif(""testnet""));
            case ""express"":
            case ""local"":
            case ""private"":
                toolkit.UseNetwork(NetworkProfile.Local);
                return new NetworkSelection(""local"", settings.ResolveNetworkWif(requested));
        }

        if (Uri.TryCreate(requested, UriKind.Absolute, out _))
        {
            toolkit.SetNetwork(requested);
            return new NetworkSelection(null, null);
        }

        Console.WriteLine($""Unknown network '{requested}', falling back to express (local Neo-Express RPC)."");
        toolkit.UseNetwork(NetworkProfile.Local);
        return new NetworkSelection(""local"", settings.ResolveNetworkWif(""local""));
    }

    private static void EnsureWif(DeploymentToolkit toolkit, DeploySettings settings, ArgSet options, NetworkSelection selection)
    {
        var wif = options.Get(""wif"")
                  ?? Environment.GetEnvironmentVariable(""NEO_WIF"")
                  ?? selection.ConfiguredWif
                  ?? settings.DefaultWif;

        if (string.IsNullOrWhiteSpace(wif))
        {
            if (Console.IsInputRedirected)
            {
                throw new InvalidOperationException(""Provide a WIF via --wif, deploysettings.json, or the NEO_WIF environment variable."");
            }

            Console.Write(""Enter WIF (input hidden): "");
            wif = ReadSecret();
        }

        toolkit.SetWifKey(wif);
    }

    private static string ReadSecret()
    {
        var builder = new StringBuilder();
        while (true)
        {
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                if (builder.Length > 0)
                {
                    builder.Length--;
                }
                continue;
            }

            if (!char.IsControl(key.KeyChar))
            {
                builder.Append(key.KeyChar);
            }
        }

        return builder.ToString();
    }
}

internal sealed class DeploySettings
{
    [System.Text.Json.Serialization.JsonIgnore]
    private string _baseDirectory = Directory.GetCurrentDirectory();

    public ArtifactSettings Artifacts { get; set; } = new();
    public string DefaultNetwork { get; set; } = ""express"";
    public Dictionary<string, CustomNetwork> CustomNetworks { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public string? DefaultWif { get; set; }

    public static DeploySettings Load(string path)
    {
        var fullPath = Path.GetFullPath(path);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException($""Deployment settings not found at {fullPath}."");

        var json = File.ReadAllText(fullPath);
        var settings = JsonSerializer.Deserialize<DeploySettings>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        });

        if (settings is null)
            throw new InvalidOperationException(""Failed to parse deployment settings."");

        settings._baseDirectory = Path.GetDirectoryName(fullPath) ?? Directory.GetCurrentDirectory();
        settings.CustomNetworks = settings.CustomNetworks?.Count > 0
            ? new Dictionary<string, CustomNetwork>(settings.CustomNetworks, StringComparer.OrdinalIgnoreCase)
            : new Dictionary<string, CustomNetwork>(StringComparer.OrdinalIgnoreCase);
        return settings;
    }

    public ArtifactDescriptor ResolveArtifacts()
    {
        return new ArtifactDescriptor(
            ResolvePath(Artifacts.Nef),
            ResolvePath(Artifacts.Manifest),
            Artifacts.InitParameters,
            Artifacts.WaitForConfirmation,
            Artifacts.ConfirmationRetries,
            Artifacts.ConfirmationDelaySeconds);
    }

    public string? ResolvePath(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        return Path.IsPathRooted(value)
            ? Path.GetFullPath(value)
            : Path.GetFullPath(Path.Combine(_baseDirectory, value));
    }

    public bool TryGetCustomNetwork(string? name, out NetworkProfile profile, out string? wif)
    {
        profile = default!;
        wif = null;
        if (string.IsNullOrWhiteSpace(name))
            return false;

        if (CustomNetworks.TryGetValue(name, out var custom) &&
            !string.IsNullOrWhiteSpace(custom.RpcUrl))
        {
            profile = new NetworkProfile(name, custom.RpcUrl, custom.NetworkMagic, custom.AddressVersion);
            wif = string.IsNullOrWhiteSpace(custom.Wif) ? null : custom.Wif;
            return true;
        }

        return false;
    }

    public string? ResolveNetworkWif(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        return CustomNetworks.TryGetValue(name, out var custom) && !string.IsNullOrWhiteSpace(custom.Wif)
            ? custom.Wif
            : null;
    }
}

internal sealed record ArtifactDescriptor(
    string? Nef,
    string? Manifest,
    JsonElement? InitParameters,
    bool? WaitForConfirmation,
    int? ConfirmationRetries,
    int? ConfirmationDelaySeconds);

internal sealed class ArtifactSettings
{
    public string? Nef { get; set; }
    public string? Manifest { get; set; }
    public JsonElement? InitParameters { get; set; }
    public bool? WaitForConfirmation { get; set; }
    public int? ConfirmationRetries { get; set; }
    public int? ConfirmationDelaySeconds { get; set; }
}

internal sealed class CustomNetwork
{
    public string RpcUrl { get; set; } = string.Empty;
    public uint? NetworkMagic { get; set; }
    public byte? AddressVersion { get; set; }
    public string? Wif { get; set; }
}

internal sealed record NetworkSelection(string? Alias, string? ConfiguredWif);

internal sealed class ArgSet
{
    private readonly Dictionary<string, string?> _values;

    private ArgSet(Dictionary<string, string?> values) => _values = values;

    public static ArgSet Parse(string[] args)
    {
        var values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < args.Length; i++)
        {
            var token = args[i];
            if (!token.StartsWith(""--"", StringComparison.Ordinal))
            {
                continue;
            }

            var key = token[2..];
            if (i + 1 < args.Length && !args[i + 1].StartsWith(""--"", StringComparison.Ordinal))
            {
                values[key] = args[i + 1];
                i++;
            }
            else
            {
                values[key] = ""true"";
            }
        }

        return new ArgSet(values);
    }

    public string? Get(string key)
        => _values.TryGetValue(key, out var value) ? value : null;

    public string Require(string key)
    {
        var value = Get(key);
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($""Missing required option --{key}."");
        return value;
    }

    public bool? GetBool(string key)
    {
        if (!_values.TryGetValue(key, out var value))
            return null;

        return value switch
        {
            null => true,
            ""true"" or ""1"" or ""yes"" => true,
            ""false"" or ""0"" or ""no"" => false,
            _ => throw new InvalidOperationException($""Option --{key} expects a boolean value."")
        };
    }

    public int? GetInt(string key)
    {
        if (!_values.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
            return null;

        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
            return parsed;

        throw new InvalidOperationException($""Option --{key} expects an integer value."");
    }
}

internal static class ParameterParser
{
    public static object?[]? ResolveArray(string? json, JsonElement? fallback)
    {
        if (!string.IsNullOrWhiteSpace(json))
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                throw new InvalidOperationException(""Parameter payload must be a JSON array."");
            return ConvertJsonArray(doc.RootElement);
        }

        if (fallback.HasValue && fallback.Value.ValueKind == JsonValueKind.Array)
            return ConvertJsonArray(fallback.Value);

        return null;
    }

    private static object?[] ConvertJsonArray(JsonElement array)
    {
        var values = new object?[array.GetArrayLength()];
        var index = 0;
        foreach (var element in array.EnumerateArray())
        {
            values[index++] = ConvertJsonValue(element);
        }
        return values;
    }

    private static object? ConvertJsonValue(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.String => element.GetString(),
        JsonValueKind.Number => ConvertJsonNumberValue(element),
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Null => null,
        JsonValueKind.Array => ConvertJsonArrayParameter(element),
        JsonValueKind.Object => TryConvertContractParameter(element, out var parameter)
            ? parameter
            : ConvertJsonObjectParameter(element),
        _ => element.GetRawText()
    };

    private static object ConvertJsonNumberValue(JsonElement element)
    {
        var raw = element.GetRawText();
        if (raw.IndexOfAny(new[] { '.', 'e', 'E' }) >= 0)
            throw new InvalidOperationException($""Only integer values are supported in parameters. Value '{raw}' is not an integer."");

        if (long.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number))
            return number;

        if (BigInteger.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var bigInteger))
            return bigInteger;

        throw new InvalidOperationException($""Unable to parse numeric value '{raw}'."");
    }
}
""""";

        private static string GetDeploySettingsTemplate() => """
{
  "defaultNetwork": "express",
  "artifacts": {
    "nef": "../TemplateContractProject/bin/sc/TemplateContractProject.nef",
    "manifest": "../TemplateContractProject/bin/sc/TemplateContractProject.manifest.json",
    "initParameters": []
  },
  "customNetworks": {
    "mainnet": {
      "rpcUrl": "https://mainnet1.neo.coz.io:443",
      "networkMagic": 860833102,
      "wif": ""
    },
    "testnet": {
      "rpcUrl": "http://seed2t5.neo.org:20332",
      "networkMagic": 894710606,
      "wif": ""
    },
    "devnet": {
      "rpcUrl": "http://localhost:50012",
      "networkMagic": 123456789,
      "wif": ""
    }
  },
  "defaultWif": ""
}
""";

        private static string GetDeployAppSettingsTemplate() => """
{
  "Network": {
    "Networks": {
      "express": {
        "RpcUrl": "http://localhost:50012"
      }
    }
  }
}
""";

        private static string GetDeployReadmeTemplate() => """
# Deployment Project

This project wraps the `Neo.SmartContract.Deploy` toolkit so you can deploy and interact with compiled contracts alongside your solution.

## Getting Started

1. Build your contract project so the `.nef` and `.manifest.json` artifacts are generated (typically under `bin/sc`).
2. Update `deploysettings.json` to point to the artifact paths. The defaults assume your contract project lives next to this deployment project.
3. Declare the networks you plan to target under `customNetworks` inside `deploysettings.json` (mainnet/testnet/devnet are scaffolded for you). Each network can specify its RPC URL, network magic, and optional `wif`. If a network does not have a `wif`, you can still pass one via `--wif` or the `NEO_WIF` environment variable and the CLI will prompt as a fallback.

## Common Commands

```bash
# Deploy artifacts to a Neo-Express instance (default RPC http://localhost:50012)
dotnet run -- deploy --network express --wif <your-wif>

# Deploy to mainnet
dotnet run -- deploy --network mainnet --wif <your-wif>

# Call a read-only method
dotnet run -- call --contract 0xabc... --method symbol

# Invoke a state-changing method
dotnet run -- invoke --contract 0xabc... --method transfer --args "[\"sender\",\"receiver\",100]"
```

### Selecting a Network

- `--network express` (default) targets a local Neo-Express RPC node running on `http://localhost:50012`.
- `--network mainnet` / `testnet` use the public RPC endpoints defined by the toolkit.
- `--network devnet` (or any custom name) resolves to the matching `customNetworks` entry inside `deploysettings.json`. When a `wif` is present in that entry it is loaded automatically so you don't have to pass `--wif` for that network.
- `--rpc http://host:port` overrides the RPC URL directly.

### Network Configuration

`deploysettings.json` keeps network metadata together so you can switch environments with `--network`:

```json
{
  "customNetworks": {
    "mainnet": {
      "rpcUrl": "https://mainnet1.neo.coz.io:443",
      "networkMagic": 860833102,
      "wif": "<mainnet-deployment-key>"
    },
    "testnet": {
      "rpcUrl": "http://seed2t5.neo.org:20332",
      "networkMagic": 894710606,
      "wif": "<testnet-deployment-key>"
    },
    "devnet": {
      "rpcUrl": "http://localhost:50012",
      "networkMagic": 123456789,
      "wif": ""
    }
  }
}
```

Leave `wif` blank to be prompted at runtime or provide the key directly for automated scenarios (CI, scripts, etc.).

### Customising Artifacts

`deploysettings.json` stores the default NEF/manifest paths as relative paths. You can override them per run:

```bash
dotnet run -- deploy --nef ../MyContract/bin/sc/MyContract.nef --manifest ../MyContract/bin/sc/MyContract.manifest.json
```

Provide initialisation parameters with `--init-params`, for example:

```bash
dotnet run -- deploy --init-params "[\"owner\", 1, true]"
```

### Invoking / Calling Contracts

- `call` executes read-only methods. Use `--return` to hint the expected return type (`string`, `bool`, `int`, `long`, `bigint`, `bytes`).
- `invoke` sends a state-changing transaction and prints the transaction hash. Arguments are supplied as JSON via `--args`.

Both commands support the same network overrides as the `deploy` command.
""";

        private static string GetBasicContractTemplate()
        {
            return @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;

namespace {{Namespace}}
{
    [DisplayName(""{{ProjectName}}"")]
    [ContractAuthor(""{{Author}}"", ""{{Email}}"")]
    [ContractDescription(""{{Description}}"")]
    [ContractVersion(""{{Version}}"")]
    [ContractSourceCode(""https://github.com/{{Author}}/{{ProjectName}}"")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class {{ClassName}} : SmartContract
    {
        private const string HelloPrefix = ""Hello, "";

        [Safe]
        public static string GetMessage(string name)
        {
            return HelloPrefix + name + ""!"";
        }

        public static void SetMessage(string key, string value)
        {
            Storage.Put(Storage.CurrentContext, key, value);
        }

        [Safe]
        public static string GetStoredMessage(string key)
        {
            return Storage.Get(Storage.CurrentReadOnlyContext, key);
        }

        public static void _deploy(object data, bool update)
        {
            if (!update)
            {
                Storage.Put(Storage.CurrentContext, ""Deployed"", Runtime.Time);
            }
        }

        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {
            ContractManagement.Update(nefFile, manifest, data);
        }

        public static void Destroy()
        {
            ContractManagement.Destroy();
        }
    }
}";
        }

        private static string GetNep17ContractTemplate()
        {
            return @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace {{Namespace}}
{
    [DisplayName(""{{ProjectName}}"")]
    [ContractAuthor(""{{Author}}"", ""{{Email}}"")]
    [ContractDescription(""{{Description}}"")]
    [ContractVersion(""{{Version}}"")]
    [ContractSourceCode(""https://github.com/{{Author}}/{{ProjectName}}"")]
    [ContractPermission(Permission.Any, Method.Any)]
    [SupportedStandards(NepStandard.Nep17)]
    public class {{ClassName}} : Neo.SmartContract.Framework.Nep17Token
    {
        private const byte Prefix_Owner = 0xff;

        public override string Symbol { [Safe] get => ""{{ProjectName}}""; }
        public override byte Decimals { [Safe] get => 8; }

        [Safe]
        public static UInt160 GetOwner()
        {
            return (UInt160)Storage.Get(new[] { Prefix_Owner });
        }

        private static bool IsOwner() =>
            Runtime.CheckWitness(GetOwner());

        public delegate void OnSetOwnerDelegate(UInt160 previousOwner, UInt160 newOwner);

        [DisplayName(""SetOwner"")]
        public static event OnSetOwnerDelegate OnSetOwner;

        public static void SetOwner(UInt160 newOwner)
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No Authorization!"");

            ExecutionEngine.Assert(newOwner.IsValid && !newOwner.IsZero, ""owner must be valid"");

            UInt160 previous = GetOwner();
            Storage.Put(new[] { Prefix_Owner }, newOwner);
            OnSetOwner(previous, newOwner);
        }

        public static new void Burn(UInt160 account, BigInteger amount)
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No Authorization!"");
            Nep17Token.Burn(account, amount);
        }

        public static new void Mint(UInt160 to, BigInteger amount)
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No Authorization!"");
            Nep17Token.Mint(to, amount);
        }

        [Safe]
        public static bool Verify() => IsOwner();

        public static void _deploy(object data, bool update)
        {
            if (update) return;

            if (data is null) data = Runtime.Transaction.Sender;
            UInt160 initialOwner = (UInt160)data;

            ExecutionEngine.Assert(initialOwner.IsValid && !initialOwner.IsZero, ""owner must exists"");

            Storage.Put(new[] { Prefix_Owner }, initialOwner);
            OnSetOwner(null, initialOwner);

            // Mint initial supply to owner
            Mint(initialOwner, 1000000_00000000); // 1,000,000 tokens with 8 decimals
        }

        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No authorization."");
            ContractManagement.Update(nefFile, manifest, data);
        }
    }
}";
        }

        private static string GetNep11ContractTemplate()
        {
            return @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace {{Namespace}}
{
    [DisplayName(""{{ProjectName}}"")]
    [ContractAuthor(""{{Author}}"", ""{{Email}}"")]
    [ContractDescription(""{{Description}}"")]
    [ContractVersion(""{{Version}}"")]
    [ContractSourceCode(""https://github.com/{{Author}}/{{ProjectName}}"")]
    [ContractPermission(Permission.Any, Method.Any)]
    [SupportedStandards(NepStandard.Nep11)]
    public class {{ClassName}} : Nep11Token<TokenState>
    {
        private const byte Prefix_Owner = 0xff;
        private const byte Prefix_TokenId = 0xfe;

        public override string Symbol { [Safe] get => ""{{ProjectName}}""; }

        [Safe]
        public static UInt160 GetOwner()
        {
            return (UInt160)Storage.Get(new[] { Prefix_Owner });
        }

        private static bool IsOwner() =>
            Runtime.CheckWitness(GetOwner());

        public static void Mint(string tokenId, TokenState tokenState)
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No Authorization!"");

            Nep11Token<TokenState>.Mint(tokenId, tokenState);
        }

        public static void _deploy(object data, bool update)
        {
            if (update) return;

            if (data is null) data = Runtime.Transaction.Sender;
            UInt160 initialOwner = (UInt160)data;

            ExecutionEngine.Assert(initialOwner.IsValid && !initialOwner.IsZero, ""owner must exists"");

            Storage.Put(new[] { Prefix_Owner }, initialOwner);
            Storage.Put(new[] { Prefix_TokenId }, 0);
        }

        protected override byte[] GetKey(string tokenId) =>
            ConcatKey(Prefix_TokenId, tokenId);

        private static byte[] ConcatKey(byte prefix, string tokenId)
        {
            return Helper.Concat((byte[])new[] { prefix }, tokenId);
        }

        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No authorization."");
            ContractManagement.Update(nefFile, manifest, data);
        }
    }

    public class TokenState : Nep11TokenState
    {
        public string? Description;
        public string? Image;
        public Map<string, object>? Properties;
    }
}";
        }

        private static string GetOwnableContractTemplate()
        {
            return @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;

namespace {{Namespace}}
{
    [DisplayName(""{{ProjectName}}"")]
    [ContractAuthor(""{{Author}}"", ""{{Email}}"")]
    [ContractDescription(""{{Description}}"")]
    [ContractVersion(""{{Version}}"")]
    [ContractSourceCode(""https://github.com/{{Author}}/{{ProjectName}}"")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class {{ClassName}} : SmartContract
    {
        private const byte Prefix_Owner = 0xff;

        [Safe]
        public static UInt160 GetOwner()
        {
            return (UInt160)Storage.Get(new[] { Prefix_Owner });
        }

        private static bool IsOwner() =>
            Runtime.CheckWitness(GetOwner());

        public delegate void OnSetOwnerDelegate(UInt160 previousOwner, UInt160 newOwner);

        [DisplayName(""SetOwner"")]
        public static event OnSetOwnerDelegate OnSetOwner;

        public static void SetOwner(UInt160 newOwner)
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No Authorization!"");

            ExecutionEngine.Assert(newOwner.IsValid && !newOwner.IsZero, ""owner must be valid"");

            UInt160 previous = GetOwner();
            Storage.Put(new[] { Prefix_Owner }, newOwner);
            OnSetOwner(previous, newOwner);
        }

        public static void DoSomething()
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No Authorization!"");

            // Owner-only logic here
            Storage.Put(Storage.CurrentContext, ""LastAction"", Runtime.Time);
        }

        [Safe]
        public static object GetData(string key)
        {
            return Storage.Get(Storage.CurrentReadOnlyContext, key);
        }

        public static void SetData(string key, object value)
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No Authorization!"");

            Storage.Put(Storage.CurrentContext, key, value);
        }

        [Safe]
        public static bool Verify() => IsOwner();

        public static void _deploy(object data, bool update)
        {
            if (update) return;

            if (data is null) data = Runtime.Transaction.Sender;
            UInt160 initialOwner = (UInt160)data;

            ExecutionEngine.Assert(initialOwner.IsValid && !initialOwner.IsZero, ""owner must exists"");

            Storage.Put(new[] { Prefix_Owner }, initialOwner);
            OnSetOwner(null, initialOwner);
        }

        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No authorization."");
            ContractManagement.Update(nefFile, manifest, data);
        }

        public static void Destroy()
        {
            if (!IsOwner())
                throw new InvalidOperationException(""No authorization."");
            ContractManagement.Destroy();
        }
    }
}";
        }

        private static string GetOracleContractTemplate()
        {
            return @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Interfaces;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace {{Namespace}}
{
    [DisplayName(""{{ProjectName}}"")]
    [ContractAuthor(""{{Author}}"", ""{{Email}}"")]
    [ContractDescription(""{{Description}}"")]
    [ContractVersion(""{{Version}}"")]
    [ContractSourceCode(""https://github.com/{{Author}}/{{ProjectName}}"")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class {{ClassName}} : SmartContract, IOracle
    {
        private const byte Prefix_RequestId = 0xff;
        private const byte Prefix_Response = 0xfe;

        public static void RequestData(string url, string filter, string callback, object userData, long gasForResponse)
        {
            Oracle.Request(url, filter, callback, userData, gasForResponse);
        }

        // This method is called by the Oracle service when a response is received
        // Implements the IOracle interface
        public void OnOracleResponse(string requestedUrl, object userData, OracleResponseCode responseCode, string result)
        {
            // Do not remove this check; the oracle security model depends on restricting callers to the native oracle hash.
            if (Runtime.CallingScriptHash != Oracle.Hash)
            {
                throw new InvalidOperationException(""Unauthorized oracle callback."");
            }

            if (responseCode != OracleResponseCode.Success)
            {
                Runtime.Log(""Oracle response failed with code: "" + (byte)responseCode);
                return;
            }

            // Store the response
            StorageContext context = Storage.CurrentContext;
            byte[] key = Helper.Concat(new byte[] { Prefix_Response }, StdLib.Serialize(userData));
            Storage.Put(context, key, result);
            
            Runtime.Log(""Oracle response received: "" + result);
            
            // Trigger an event
            OnResponseReceived(requestedUrl, userData, result);
        }

        public delegate void OnResponseReceivedDelegate(string url, object userData, string response);
        
        [DisplayName(""ResponseReceived"")]
        public static event OnResponseReceivedDelegate OnResponseReceived = default!;

        [Safe]
        public static string GetLastResponse(object userData)
        {
            StorageContext context = Storage.CurrentReadOnlyContext;
            byte[] key = Helper.Concat(new byte[] { Prefix_Response }, StdLib.Serialize(userData));
            ByteString data = Storage.Get(context, key);
            return data;
        }

        public static void _deploy(object data, bool update)
        {
            if (!update)
            {
                Storage.Put(Storage.CurrentContext, ""Deployed"", Runtime.Time);
            }
        }

        public static void Update(ByteString nefFile, string manifest, object data)
        {
            ContractManagement.Update(nefFile, manifest, data);
        }

        public static void Destroy()
        {
            ContractManagement.Destroy();
        }
    }
}";
        }
    }
}
