using R3E.WebGUI.Service.Domain.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace R3E.WebGUI.Service.Core.Services;

public interface INeoRpcService
{
    Task<ContractManifestInfo?> GetContractManifestAsync(string contractAddress, string network);
    Task<JObject?> GetContractStateAsync(string contractAddress, string network);
    Task<List<ContractAssetInfo>> GetContractAssetsAsync(string contractAddress, string network);
    Task<List<ContractBalance>> GetContractBalancesAsync(string contractAddress, string network);
    Task<List<ContractTransaction>> GetContractTransactionsAsync(string contractAddress, string network, int page = 1, int pageSize = 20);
    Task<string> InvokeContractMethodAsync(string contractAddress, string method, object[] parameters, string network, string? signerAddress = null);
    Task<string> SendTransactionAsync(string signedTransaction, string network);
    Task<decimal> EstimateGasAsync(string contractAddress, string method, object[] parameters, string network);
    Task<bool> IsContractDeployedAsync(string contractAddress, string network);
    Task<uint> GetLatestBlockIndexAsync(string network);
    Task<JObject?> GetApplicationLogAsync(string transactionHash, string network);
}

public class SimpleNeoRpcService : INeoRpcService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SimpleNeoRpcService> _logger;
    private readonly Dictionary<string, string> _rpcEndpoints;

    public SimpleNeoRpcService(HttpClient httpClient, ILogger<SimpleNeoRpcService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _rpcEndpoints = new Dictionary<string, string>
        {
            ["testnet"] = configuration.GetConnectionString("NeoTestnetRpc") ?? "https://testnet1.neo.coz.io:443",
            ["mainnet"] = configuration.GetConnectionString("NeoMainnetRpc") ?? "https://mainnet1.neo.coz.io:443"
        };
    }

    public async Task<ContractManifestInfo?> GetContractManifestAsync(string contractAddress, string network)
    {
        try
        {
            var contractState = await GetContractStateAsync(contractAddress, network);
            if (contractState == null || !contractState.ContainsKey("manifest"))
                return null;

            var manifest = contractState["manifest"] as JObject;
            if (manifest == null) return null;

            var manifestInfo = new ContractManifestInfo
            {
                Id = Guid.NewGuid(),
                ContractAddress = contractAddress,
                Network = network,
                ManifestJson = manifest.ToString(),
                Name = manifest["name"]?.ToString() ?? "Unknown",
                Description = manifest["extra"]?["Description"]?.ToString() ?? "",
                Author = manifest["extra"]?["Author"]?.ToString() ?? "",
                Email = manifest["extra"]?["Email"]?.ToString() ?? "",
                Version = manifest["extra"]?["Version"]?.ToString() ?? "1.0",
                FetchedAt = DateTime.UtcNow,
                IsValid = true
            };

            // Parse methods from ABI
            var abi = manifest["abi"] as JObject;
            if (abi != null && abi["methods"] is JArray methods)
            {
                foreach (var method in methods)
                {
                    var methodInfo = new ContractMethod
                    {
                        Id = Guid.NewGuid(),
                        ContractManifestId = manifestInfo.Id,
                        Name = method["name"]?.ToString() ?? "",
                        ReturnType = method["returntype"]?.ToString() ?? "Void",
                        Safe = method["safe"]?.ToObject<bool>() ?? false,
                        Description = ""
                    };

                    // Parse parameters
                    if (method["parameters"] is JArray parameters)
                    {
                        for (int i = 0; i < parameters.Count; i++)
                        {
                            var param = parameters[i];
                            methodInfo.Parameters.Add(new ContractParameter
                            {
                                Id = Guid.NewGuid(),
                                ContractMethodId = methodInfo.Id,
                                Name = param["name"]?.ToString() ?? $"param{i}",
                                Type = param["type"]?.ToString() ?? "Any",
                                Order = i
                            });
                        }
                    }

                    manifestInfo.Methods.Add(methodInfo);
                }
            }

            // Parse events from ABI
            if (abi != null && abi["events"] is JArray events)
            {
                foreach (var eventDef in events)
                {
                    var eventInfo = new ContractEvent
                    {
                        Id = Guid.NewGuid(),
                        ContractManifestId = manifestInfo.Id,
                        Name = eventDef["name"]?.ToString() ?? "",
                        Description = ""
                    };

                    // Parse event parameters
                    if (eventDef["parameters"] is JArray eventParams)
                    {
                        for (int i = 0; i < eventParams.Count; i++)
                        {
                            var param = eventParams[i];
                            eventInfo.Parameters.Add(new ContractEventParameter
                            {
                                Id = Guid.NewGuid(),
                                ContractEventId = eventInfo.Id,
                                Name = param["name"]?.ToString() ?? $"param{i}",
                                Type = param["type"]?.ToString() ?? "Any",
                                Order = i
                            });
                        }
                    }

                    manifestInfo.Events.Add(eventInfo);
                }
            }

            return manifestInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get contract manifest for {ContractAddress} on {Network}", contractAddress, network);
            return null;
        }
    }

    public async Task<JObject?> GetContractStateAsync(string contractAddress, string network)
    {
        try
        {
            var result = await CallRpcMethodAsync(network, "getcontractstate", contractAddress);
            return result?["result"] as JObject;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get contract state for {ContractAddress} on {Network}", contractAddress, network);
            return null;
        }
    }

    public async Task<List<ContractAssetInfo>> GetContractAssetsAsync(string contractAddress, string network)
    {
        // This would require additional API calls or indexer integration
        return new List<ContractAssetInfo>();
    }

    public async Task<List<ContractBalance>> GetContractBalancesAsync(string contractAddress, string network)
    {
        try
        {
            var balances = new List<ContractBalance>();
            
            // Get NEP-17 balances
            var result = await CallRpcMethodAsync(network, "getnep17balances", contractAddress);
            var balanceData = result?["result"]?["balance"] as JArray;
            
            if (balanceData != null)
            {
                foreach (var balance in balanceData)
                {
                    balances.Add(new ContractBalance
                    {
                        Id = Guid.NewGuid(),
                        ContractAddress = contractAddress,
                        Network = network,
                        AssetHash = balance["assethash"]?.ToString() ?? "",
                        Balance = balance["amount"]?.ToString() ?? "0",
                        LastUpdated = DateTime.UtcNow
                    });
                }
            }

            return balances;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get contract balances for {ContractAddress} on {Network}", contractAddress, network);
            return new List<ContractBalance>();
        }
    }

    public async Task<List<ContractTransaction>> GetContractTransactionsAsync(string contractAddress, string network, int page = 1, int pageSize = 20)
    {
        // This would require blockchain indexer integration
        return new List<ContractTransaction>();
    }

    public async Task<string> InvokeContractMethodAsync(string contractAddress, string method, object[] parameters, string network, string? signerAddress = null)
    {
        try
        {
            // For now, just return a mock response
            var mockResult = new
            {
                state = "HALT",
                gasconsumed = "0.0234",
                stack = new[]
                {
                    new { type = "ByteString", value = "SGVsbG8gV29ybGQ=" }
                }
            };

            return JsonConvert.SerializeObject(mockResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to invoke contract method {Method} for {ContractAddress} on {Network}", method, contractAddress, network);
            throw;
        }
    }

    public async Task<string> SendTransactionAsync(string signedTransaction, string network)
    {
        try
        {
            var result = await CallRpcMethodAsync(network, "sendrawtransaction", signedTransaction);
            return result?["result"]?["hash"]?.ToString() ?? "";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send transaction on {Network}", network);
            throw;
        }
    }

    public async Task<decimal> EstimateGasAsync(string contractAddress, string method, object[] parameters, string network)
    {
        try
        {
            // Mock gas estimation for now
            return 0.03m;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to estimate gas for {Method} on {ContractAddress} on {Network}", method, contractAddress, network);
            return 0;
        }
    }

    public async Task<bool> IsContractDeployedAsync(string contractAddress, string network)
    {
        var contractState = await GetContractStateAsync(contractAddress, network);
        return contractState != null;
    }

    public async Task<uint> GetLatestBlockIndexAsync(string network)
    {
        try
        {
            var result = await CallRpcMethodAsync(network, "getblockcount");
            var blockCount = result?["result"]?.ToObject<uint>() ?? 0;
            return blockCount > 0 ? blockCount - 1 : 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get latest block index on {Network}", network);
            return 0;
        }
    }

    public async Task<JObject?> GetApplicationLogAsync(string transactionHash, string network)
    {
        try
        {
            var result = await CallRpcMethodAsync(network, "getapplicationlog", transactionHash);
            return result?["result"] as JObject;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get application log for {TransactionHash} on {Network}", transactionHash, network);
            return null;
        }
    }

    private async Task<JObject?> CallRpcMethodAsync(string network, string method, params object[] parameters)
    {
        var rpcEndpoint = GetRpcEndpoint(network);
        
        var request = new
        {
            jsonrpc = "2.0",
            method = method,
            @params = parameters,
            id = 1
        };

        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(rpcEndpoint, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("RPC call failed: {StatusCode} {ReasonPhrase} {Content}", 
                response.StatusCode, response.ReasonPhrase, responseContent);
            return null;
        }

        return JsonConvert.DeserializeObject<JObject>(responseContent);
    }

    private string GetRpcEndpoint(string network)
    {
        if (!_rpcEndpoints.TryGetValue(network.ToLowerInvariant(), out var endpoint))
        {
            throw new ArgumentException($"Unsupported network: {network}");
        }
        return endpoint;
    }
}