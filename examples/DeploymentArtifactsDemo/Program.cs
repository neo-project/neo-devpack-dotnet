using Neo.SmartContract.Deploy;
using System.Text.Json;

static void PrintUsage()
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run -- --network <mainnet|testnet|http(s)://rpc> --wif <WIF> --nef <path> --manifest <path> [--wait]");
    Console.WriteLine("  dotnet run -- --network <...> --call --contract <hash|address> --method <name> [--args '[\"arg1\",123,true]']");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  dotnet run -- --network testnet --wif Kx... --nef My.nef --manifest My.manifest.json --wait");
    Console.WriteLine("  dotnet run -- --network testnet --call --contract 0x... --method symbol");
}

string? GetArg(string key)
{
    for (int i = 0; i < args.Length - 1; i++)
        if (string.Equals(args[i], key, StringComparison.OrdinalIgnoreCase))
            return args[i + 1];
    return null;
}

bool HasFlag(string key) => args.Any(a => string.Equals(a, key, StringComparison.OrdinalIgnoreCase));

if (args.Length == 0 || HasFlag("--help") || HasFlag("-h"))
{
    PrintUsage();
    return;
}

var network = GetArg("--network") ?? Environment.GetEnvironmentVariable("NEO_RPC_URL") ?? "testnet";
var wif = GetArg("--wif") ?? Environment.GetEnvironmentVariable("NEO_WIF");
var nef = GetArg("--nef");
var manifest = GetArg("--manifest");
var wait = HasFlag("--wait");

var doCall = HasFlag("--call");
var contract = GetArg("--contract");
var method = GetArg("--method");
var argsJson = GetArg("--args");

var toolkit = new DeploymentToolkit().SetNetwork(network);

try
{
    if (!doCall)
    {
        if (string.IsNullOrWhiteSpace(wif) || string.IsNullOrWhiteSpace(nef) || string.IsNullOrWhiteSpace(manifest))
        {
            Console.Error.WriteLine("Missing required parameters for deployment.\n");
            PrintUsage();
            return;
        }

        toolkit.SetWifKey(wif);
        var initParams = Array.Empty<object>();
        var result = await toolkit.DeployArtifactsAsync(nef, manifest, initParams, waitForConfirmation: wait);
        Console.WriteLine($"Transaction Hash: {result.TransactionHash}");
        Console.WriteLine($"Expected Contract Hash: {result.ContractHash}");
    }
    else
    {
        if (string.IsNullOrWhiteSpace(contract) || string.IsNullOrWhiteSpace(method))
        {
            Console.Error.WriteLine("Missing required parameters for call.\n");
            PrintUsage();
            return;
        }

        object[] callArgs = Array.Empty<object>();
        if (!string.IsNullOrWhiteSpace(argsJson))
        {
            try
            {
                var doc = JsonDocument.Parse(argsJson);
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    callArgs = doc.RootElement.EnumerateArray().Select(el => el.ValueKind switch
                    {
                        JsonValueKind.String => (object)el.GetString()!,
                        JsonValueKind.Number => el.TryGetInt64(out var l) ? (object)l : el.GetDouble(),
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        _ => el.ToString()
                    }).ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to parse --args JSON: {ex.Message}");
                return;
            }
        }

        var value = await toolkit.CallAsync<string>(contract, method, callArgs);
        Console.WriteLine($"Result: {value}");
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    Environment.ExitCode = 1;
}

