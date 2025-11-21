using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Neo.Json;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;

namespace Neo.SmartContract.Deploy;

public partial class DeploymentToolkit
{
    /// <summary>
    /// Deploy multiple contracts from a manifest file.
    /// </summary>
    /// <param name="manifestPath">Path to the deployment manifest JSON file</param>
    /// <returns>Dictionary of contract names to deployment information</returns>
    /// <example>
    /// <code>
    /// var deployments = await toolkit.DeployFromManifestAsync("deployment.json");
    /// foreach (var (name, info) in deployments)
    /// {
    ///     Console.WriteLine($"{name}: {info.ContractHash}");
    /// }
    /// </code>
    /// </example>
    public async Task<Dictionary<string, ContractDeploymentInfo>> DeployFromManifestAsync(string manifestPath, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(manifestPath))
            throw new ArgumentException("Manifest path is required.", nameof(manifestPath));

        if (!File.Exists(manifestPath))
            throw new FileNotFoundException("Deployment manifest file not found.", manifestPath);

        var manifestJson = await File.ReadAllTextAsync(manifestPath, cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(manifestJson))
            throw new InvalidOperationException("Deployment manifest file is empty.");

        var manifest = System.Text.Json.JsonSerializer.Deserialize<DeploymentManifestDocument>(manifestJson, JsonOptions)
            ?? throw new InvalidOperationException("Deployment manifest is invalid or could not be parsed.");

        if (manifest.Contracts is null || manifest.Contracts.Count == 0)
            throw new InvalidOperationException("Deployment manifest must contain at least one contract entry.");

        var manifestDirectory = Path.GetDirectoryName(Path.GetFullPath(manifestPath)) ?? Directory.GetCurrentDirectory();
        PreValidateManifest(manifest, manifestDirectory);

        var originalState = CaptureState();
        var results = new Dictionary<string, ContractDeploymentInfo>(StringComparer.OrdinalIgnoreCase);

        try
        {
            if (!string.IsNullOrWhiteSpace(manifest.Network))
            {
                UseNetwork(ResolveNetworkProfile(manifest.Network));
            }

            var manifestPolicy = ResolveConfirmationPolicy(
                manifest.WaitForConfirmation,
                manifest.ConfirmationRetries,
                manifest.ConfirmationDelaySeconds);

            using var manifestWifScope = UseTemporaryWif(manifest.Wif);

            foreach (var contract in manifest.Contracts)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (string.IsNullOrWhiteSpace(contract.Nef) || string.IsNullOrWhiteSpace(contract.Manifest))
                    throw new InvalidOperationException($"Contract entry '{contract.Name ?? contract.Nef}' is missing required artifact paths.");

                var nefPath = ResolveArtifactPath(manifestDirectory, contract.Nef);
                var contractManifestPath = ResolveArtifactPath(manifestDirectory, contract.Manifest);

                var key = string.IsNullOrWhiteSpace(contract.Name)
                    ? Path.GetFileNameWithoutExtension(nefPath)
                    : contract.Name;

                object?[]? initParams = null;
                if (contract.InitParams.ValueKind is JsonValueKind.Array)
                {
                    initParams = ConvertJsonArray(contract.InitParams);
                }
                else if (contract.InitParams.ValueKind is not JsonValueKind.Undefined and not JsonValueKind.Null)
                {
                    throw new InvalidOperationException($"Contract entry '{contract.Name ?? contract.Nef}' initialization parameters must be an array when provided.");
                }

                var contractPolicy = ResolveConfirmationPolicy(
                    contract.WaitForConfirmation,
                    contract.ConfirmationRetries,
                    contract.ConfirmationDelaySeconds,
                    manifestPolicy);

                using var contractWifScope = UseTemporaryWif(contract.Wif);

                var deploymentInfo = await DeployArtifactsAsync(
                    nefPath,
                    contractManifestPath,
                    initParams,
                    contractPolicy.WaitForConfirmation,
                    contractPolicy.ConfirmationRetries,
                    contractPolicy.ConfirmationDelaySeconds,
                    cancellationToken,
                    signers: null,
                    transactionSignerAsync: null).ConfigureAwait(false);

                results[key] = deploymentInfo;
            }
        }
        finally
        {
            RestoreState(originalState);
        }

        return results;
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
        _ => throw new InvalidOperationException($"Unsupported JSON value '{element.GetRawText()}' in deployment manifest.")
    };

    private static object ConvertJsonNumberValue(JsonElement element)
    {
        var raw = element.GetRawText();
        if (raw.IndexOfAny(new[] { '.', 'e', 'E' }) >= 0)
            throw new InvalidOperationException($"Only integer values are supported in deployment parameters. Value '{raw}' is not an integer.");

        if (!BigInteger.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
            throw new InvalidOperationException($"Unable to parse numeric value '{raw}' in deployment parameters.");

        if (value >= long.MinValue && value <= long.MaxValue)
            return (long)value;

        return value;
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
            // Ignore parse errors so fallback handlers can process the payload.
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
        _ => throw new InvalidOperationException($"Unsupported JSON element '{element.GetRawText()}' in deployment manifest.")
    };

    private static ContractParameter ConvertJsonNumberParameter(JsonElement element)
    {
        var value = ConvertJsonNumberValue(element);
        return value switch
        {
            long l => new ContractParameter(ContractParameterType.Integer) { Value = new BigInteger(l) },
            BigInteger bigInteger => new ContractParameter(ContractParameterType.Integer) { Value = bigInteger },
            _ => throw new InvalidOperationException($"Unsupported numeric value '{value}' in deployment parameters.")
        };
    }

    private static string ResolveArtifactPath(string baseDirectory, string path)
    {
        if (Path.IsPathRooted(path))
            return Path.GetFullPath(path);

        return Path.GetFullPath(Path.Combine(baseDirectory, path));
    }

    private sealed record DeploymentManifestDocument
    {
        public string? Network { get; init; }
        public string? Wif { get; init; }
        public bool? WaitForConfirmation { get; init; }
        public int? ConfirmationRetries { get; init; }
        public int? ConfirmationDelaySeconds { get; init; }
        public List<DeploymentManifestContract> Contracts { get; init; } = new();
    }

    private sealed record DeploymentManifestContract
    {
        public string? Name { get; init; }
        public string? Nef { get; init; }
        public string? Manifest { get; init; }
        public JsonElement InitParams { get; init; }
        public string? Wif { get; init; }
        public bool? WaitForConfirmation { get; init; }
        public int? ConfirmationRetries { get; init; }
        public int? ConfirmationDelaySeconds { get; init; }
    }

    private static void PreValidateManifest(DeploymentManifestDocument manifest, string manifestDirectory)
    {
        var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var artifacts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var contract in manifest.Contracts)
        {
            if (string.IsNullOrWhiteSpace(contract.Nef) || string.IsNullOrWhiteSpace(contract.Manifest))
                throw new InvalidOperationException($"Contract entry '{contract.Name ?? contract.Nef}' is missing required artifact paths.");

            var nefPath = ResolveArtifactPath(manifestDirectory, contract.Nef);
            var manifestPath = ResolveArtifactPath(manifestDirectory, contract.Manifest);

            var key = string.IsNullOrWhiteSpace(contract.Name)
                ? Path.GetFileNameWithoutExtension(nefPath)
                : contract.Name;

            if (!keys.Add(key))
                throw new InvalidOperationException($"Duplicate deployment key '{key}' detected in manifest. Provide unique names or NEF paths.");

            var artifactKey = $"{nefPath}|{manifestPath}";
            if (!artifacts.Add(artifactKey))
                throw new InvalidOperationException($"Contract entry '{key}' reuses the same NEF/manifest artifacts as another entry. Provide unique artifact pairs.");
        }
    }
}
