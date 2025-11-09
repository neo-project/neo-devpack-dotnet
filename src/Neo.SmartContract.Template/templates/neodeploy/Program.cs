using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.Json;
using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Deploy;

namespace DeployProject;

internal sealed class Program
{
    private const string DefaultSettingsFile = "deploysettings.json";

    private static async Task<int> Main(string[] args)
    {
        if (args.Length == 0 || IsHelp(args[0]))
        {
            PrintHelp();
            return 0;
        }

        var command = args[0].ToLowerInvariant();
        var options = ArgSet.Parse(args.Skip(1).ToArray());
        var settingsPath = options.Get("config") ?? DefaultSettingsFile;
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
                case "deploy":
                    await RunDeployAsync(settings, options, cancellation.Token).ConfigureAwait(false);
                    break;
                case "invoke":
                    await RunInvokeAsync(settings, options, cancellation.Token).ConfigureAwait(false);
                    break;
                case "call":
                    await RunCallAsync(settings, options, cancellation.Token).ConfigureAwait(false);
                    break;
                default:
                    Console.Error.WriteLine($"Unknown command '{command}'.");
                    PrintHelp();
                    return 1;
            }

            return 0;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation cancelled.");
            return 1;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    private static bool IsHelp(string? argument)
        => string.IsNullOrWhiteSpace(argument)
           || argument.Equals("-h", StringComparison.OrdinalIgnoreCase)
           || argument.Equals("--help", StringComparison.OrdinalIgnoreCase);

    private static void PrintHelp()
    {
        const string help = """
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
        """;

        Console.WriteLine(help);
    }

    private static async Task RunDeployAsync(DeploySettings settings, ArgSet options, CancellationToken cancellationToken)
    {
        using var toolkit = new DeploymentToolkit();
        ConfigureNetwork(toolkit, settings, options);
        EnsureWif(toolkit, settings, options);

        var artifacts = settings.ResolveArtifacts();
        var nef = settings.ResolvePath(options.Get("nef")) ?? artifacts.Nef;
        var manifest = settings.ResolvePath(options.Get("manifest")) ?? artifacts.Manifest;

        if (string.IsNullOrWhiteSpace(nef))
            throw new InvalidOperationException("Set the NEF path via deploysettings.json or the --nef option.");
        if (string.IsNullOrWhiteSpace(manifest))
            throw new InvalidOperationException("Set the manifest path via deploysettings.json or the --manifest option.");

        if (!File.Exists(nef))
            throw new FileNotFoundException($"NEF file not found at {nef}. Build your contract before deploying.");
        if (!File.Exists(manifest))
            throw new FileNotFoundException($"Manifest file not found at {manifest}. Build your contract before deploying.");

        var initParams = ParameterParser.ResolveArray(options.Get("init-params"), artifacts.InitParameters);
        var waitForConfirmation = options.GetBool("wait") ?? artifacts.WaitForConfirmation;
        var retries = options.GetInt("retries") ?? artifacts.ConfirmationRetries;
        var delaySeconds = options.GetInt("delay") ?? artifacts.ConfirmationDelaySeconds;

        var request = new DeploymentArtifactsRequest(nef, manifest, initParams)
            .WithConfirmationPolicy(waitForConfirmation, retries, delaySeconds);

        Console.WriteLine($"Deploying artifacts:");
        Console.WriteLine($"  NEF:      {nef}");
        Console.WriteLine($"  Manifest: {manifest}");
        Console.WriteLine($"  Network:  {toolkit.CurrentNetwork.Identifier} ({toolkit.CurrentNetwork.RpcUrl})");

        var result = await toolkit.DeployArtifactsAsync(request, cancellationToken).ConfigureAwait(false);

        Console.WriteLine("Deployment submitted:");
        Console.WriteLine($"  Transaction: {result.TransactionHash}");
        if (result.ContractHash is not null)
        {
            Console.WriteLine($"  Expected contract hash: {result.ContractHash}");
        }
    }

    private static async Task RunInvokeAsync(DeploySettings settings, ArgSet options, CancellationToken cancellationToken)
    {
        var contract = options.Require("contract");
        var method = options.Require("method");
        var arguments = ParameterParser.ResolveArray(options.Get("args"), fallback: null);

        using var toolkit = new DeploymentToolkit();
        ConfigureNetwork(toolkit, settings, options);
        EnsureWif(toolkit, settings, options);

        Console.WriteLine($"Invoking {method} on {contract}...");
        var txHash = await toolkit.InvokeAsync(contract, method, cancellationToken, arguments ?? Array.Empty<object?>()).ConfigureAwait(false);
        Console.WriteLine($"Transaction submitted: {txHash}");
    }

    private static async Task RunCallAsync(DeploySettings settings, ArgSet options, CancellationToken cancellationToken)
    {
        var contract = options.Require("contract");
        var method = options.Require("method");
        var returnType = options.Get("return") ?? "string";
        var arguments = ParameterParser.ResolveArray(options.Get("args"), fallback: null) ?? Array.Empty<object?>();

        using var toolkit = new DeploymentToolkit();
        ConfigureNetwork(toolkit, settings, options);

        Console.WriteLine($"Calling {method} on {contract}...");
        var normalizedReturn = returnType.ToLowerInvariant();
        object? result = normalizedReturn switch
        {
            "bool" or "boolean" => await toolkit.CallAsync<bool>(contract, method, cancellationToken, arguments).ConfigureAwait(false),
            "int" => await toolkit.CallAsync<int>(contract, method, cancellationToken, arguments).ConfigureAwait(false),
            "long" => await toolkit.CallAsync<long>(contract, method, cancellationToken, arguments).ConfigureAwait(false),
            "bigint" => await toolkit.CallAsync<BigInteger>(contract, method, cancellationToken, arguments).ConfigureAwait(false),
            "bytes" => Convert.ToHexString(await toolkit.CallAsync<byte[]>(contract, method, cancellationToken, arguments).ConfigureAwait(false)),
            _ => await toolkit.CallAsync<string>(contract, method, cancellationToken, arguments).ConfigureAwait(false)
        };

        Console.WriteLine($"Result ({normalizedReturn}): {result}");
    }

    private static void ConfigureNetwork(DeploymentToolkit toolkit, DeploySettings settings, ArgSet options)
    {
        var rpcOverride = options.Get("rpc");
        if (!string.IsNullOrWhiteSpace(rpcOverride))
        {
            toolkit.SetNetwork(rpcOverride);
            return;
        }

        var requested = options.Get("network") ?? settings.DefaultNetwork;
        if (string.IsNullOrWhiteSpace(requested))
        {
            toolkit.UseNetwork(NetworkProfile.Local);
            return;
        }

        if (settings.TryGetCustomNetwork(requested, out var profile))
        {
            toolkit.UseNetwork(profile);
            return;
        }

        switch (requested.ToLowerInvariant())
        {
            case "mainnet":
                toolkit.UseNetwork(NetworkProfile.MainNet);
                return;
            case "testnet":
                toolkit.UseNetwork(NetworkProfile.TestNet);
                return;
            case "express":
            case "local":
            case "private":
                toolkit.UseNetwork(NetworkProfile.Local);
                return;
        }

        if (Uri.TryCreate(requested, UriKind.Absolute, out _))
        {
            toolkit.SetNetwork(requested);
            return;
        }

        Console.WriteLine($"Unknown network '{requested}', falling back to express (local Neo-Express RPC).");
        toolkit.UseNetwork(NetworkProfile.Local);
    }

    private static void EnsureWif(DeploymentToolkit toolkit, DeploySettings settings, ArgSet options)
    {
        var wif = options.Get("wif")
                  ?? Environment.GetEnvironmentVariable("NEO_WIF")
                  ?? settings.DefaultWif;

        if (string.IsNullOrWhiteSpace(wif))
        {
            if (Console.IsInputRedirected)
            {
                throw new InvalidOperationException("Provide a WIF via --wif, deploysettings.json, or the NEO_WIF environment variable.");
            }

            Console.Write("Enter WIF (input hidden): ");
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
    public string DefaultNetwork { get; set; } = "express";
    public Dictionary<string, CustomNetwork> CustomNetworks { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public string? DefaultWif { get; set; }

    public static DeploySettings Load(string path)
    {
        var fullPath = Path.GetFullPath(path);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Deployment settings not found at {fullPath}.");

        var json = File.ReadAllText(fullPath);
        var settings = JsonSerializer.Deserialize<DeploySettings>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        });

        if (settings is null)
            throw new InvalidOperationException("Failed to parse deployment settings.");

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

    public bool TryGetCustomNetwork(string? name, out NetworkProfile profile)
    {
        profile = default!;
        if (string.IsNullOrWhiteSpace(name))
            return false;

        if (CustomNetworks.TryGetValue(name, out var custom) &&
            !string.IsNullOrWhiteSpace(custom.RpcUrl))
        {
            profile = new NetworkProfile(name, custom.RpcUrl, custom.NetworkMagic, custom.AddressVersion);
            return true;
        }

        return false;
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
}

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
            if (!token.StartsWith("--", StringComparison.Ordinal))
            {
                continue;
            }

            var key = token[2..];
            if (i + 1 < args.Length && !args[i + 1].StartsWith("--", StringComparison.Ordinal))
            {
                values[key] = args[i + 1];
                i++;
            }
            else
            {
                values[key] = "true";
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
            throw new InvalidOperationException($"Missing required option --{key}.");
        return value;
    }

    public bool? GetBool(string key)
    {
        if (!_values.TryGetValue(key, out var value))
            return null;

        return value switch
        {
            null => true,
            "true" or "1" or "yes" => true,
            "false" or "0" or "no" => false,
            _ => throw new InvalidOperationException($"Option --{key} expects a boolean value.")
        };
    }

    public int? GetInt(string key)
    {
        if (!_values.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
            return null;

        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
            return parsed;

        throw new InvalidOperationException($"Option --{key} expects an integer value.");
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
                throw new InvalidOperationException("Parameter payload must be a JSON array.");
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
            throw new InvalidOperationException($"Only integer values are supported in parameters. Value '{raw}' is not an integer.");

        if (long.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number))
            return number;

        if (BigInteger.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var bigInteger))
            return bigInteger;

        throw new InvalidOperationException($"Unable to parse numeric value '{raw}'.");
    }

    private static bool TryConvertContractParameter(JsonElement element, out ContractParameter parameter)
    {
        parameter = default!;
        if (element.ValueKind != JsonValueKind.Object)
            return false;

        if (!element.TryGetProperty("type", out var typeProperty) || typeProperty.ValueKind != JsonValueKind.String)
            return false;

        try
        {
            var raw = element.GetRawText();
            if (JToken.Parse(raw) is JObject obj)
            {
                parameter = ContractParameter.FromJson(obj);
                return true;
            }
        }
        catch
        {
            // ignore malformed payloads so fallback handling can kick in
        }

        return false;
    }

    private static ContractParameter ConvertJsonArrayParameter(JsonElement array)
    {
        var parameter = new ContractParameter(ContractParameterType.Array);
        var items = new List<ContractParameter>(array.GetArrayLength());
        foreach (var child in array.EnumerateArray())
        {
            items.Add(ConvertJsonElementToContractParameter(child));
        }
        parameter.Value = items;
        return parameter;
    }

    private static ContractParameter ConvertJsonObjectParameter(JsonElement element)
    {
        var parameter = new ContractParameter(ContractParameterType.Map);
        var entries = new List<KeyValuePair<ContractParameter, ContractParameter>>();
        foreach (var property in element.EnumerateObject())
        {
            var key = new ContractParameter(ContractParameterType.String) { Value = property.Name };
            var value = ConvertJsonElementToContractParameter(property.Value);
            entries.Add(new KeyValuePair<ContractParameter, ContractParameter>(key, value));
        }
        parameter.Value = entries;
        return parameter;
    }

    private static ContractParameter ConvertJsonElementToContractParameter(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.Null => new ContractParameter(ContractParameterType.Any) { Value = null },
        JsonValueKind.True => new ContractParameter(ContractParameterType.Boolean) { Value = true },
        JsonValueKind.False => new ContractParameter(ContractParameterType.Boolean) { Value = false },
        JsonValueKind.String => new ContractParameter(ContractParameterType.String) { Value = element.GetString()! },
        JsonValueKind.Number => ConvertJsonNumberParameter(element),
        JsonValueKind.Array => ConvertJsonArrayParameter(element),
        JsonValueKind.Object => TryConvertContractParameter(element, out var parameter)
            ? parameter
            : ConvertJsonObjectParameter(element),
        _ => throw new InvalidOperationException($"Unsupported JSON element '{element.GetRawText()}' in parameters.")
    };

    private static ContractParameter ConvertJsonNumberParameter(JsonElement element)
    {
        var value = ConvertJsonNumberValue(element);
        return value switch
        {
            long l => new ContractParameter(ContractParameterType.Integer) { Value = new BigInteger(l) },
            BigInteger bigInteger => new ContractParameter(ContractParameterType.Integer) { Value = bigInteger },
            _ => throw new InvalidOperationException("Only integer values are supported when constructing contract parameters.")
        };
    }
}
