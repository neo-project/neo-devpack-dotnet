using Microsoft.AspNetCore.Mvc;
using R3E.WebGUI.Service.Core.Services;
using R3E.WebGUI.Service.Domain.Models;

namespace R3E.WebGUI.Service.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContractController : ControllerBase
{
    private readonly INeoRpcService _neoRpcService;
    private readonly IWebGUIGeneratorService _webGUIGeneratorService;
    private readonly ILogger<ContractController> _logger;

    public ContractController(
        INeoRpcService neoRpcService,
        IWebGUIGeneratorService webGUIGeneratorService,
        ILogger<ContractController> logger)
    {
        _neoRpcService = neoRpcService;
        _webGUIGeneratorService = webGUIGeneratorService;
        _logger = logger;
    }

    [HttpGet("{contractAddress}/info")]
    public async Task<IActionResult> GetContractInfo(string contractAddress, [FromQuery] string network = "testnet")
    {
        try
        {
            var manifest = await _neoRpcService.GetContractManifestAsync(contractAddress, network);
            if (manifest == null)
            {
                return NotFound(new { error = "Contract not found or invalid" });
            }

            var contractInfo = new ContractInfo
            {
                Address = contractAddress,
                Name = manifest.Name,
                Network = network,
                Author = manifest.Author,
                Description = manifest.Description,
                Version = manifest.Version,
                Methods = manifest.Methods.Select(m => new MethodInfo
                {
                    Name = m.Name,
                    ReturnType = m.ReturnType,
                    Safe = m.Safe,
                    Parameters = m.Parameters.Select(p => new ParameterInfo
                    {
                        Name = p.Name,
                        Type = p.Type,
                        Optional = p.Optional,
                        DefaultValue = p.DefaultValue,
                        Description = p.Description
                    }).ToList(),
                    Description = m.Description
                }).ToList(),
                Events = manifest.Events.Select(e => new EventInfo
                {
                    Name = e.Name,
                    Parameters = e.Parameters.Select(p => new EventParameterInfo
                    {
                        Name = p.Name,
                        Type = p.Type
                    }).ToList(),
                    Description = e.Description
                }).ToList(),
                LastUpdated = manifest.FetchedAt
            };

            return Ok(contractInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contract info for {ContractAddress} on {Network}", contractAddress, network);
            return StatusCode(500, new { error = "Failed to retrieve contract information", message = ex.Message });
        }
    }

    [HttpGet("{contractAddress}/balances")]
    public async Task<IActionResult> GetContractBalances(string contractAddress, [FromQuery] string network = "testnet")
    {
        try
        {
            var balances = await _neoRpcService.GetContractBalancesAsync(contractAddress, network);
            var balanceInfos = balances.Select(b => new BalanceInfo
            {
                AssetHash = b.AssetHash,
                Balance = b.Balance,
                Decimals = b.AssetInfo?.Decimals ?? 0,
                Symbol = b.AssetInfo?.Symbol ?? "Unknown",
                LastUpdated = b.LastUpdated
            }).ToList();

            return Ok(balanceInfos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contract balances for {ContractAddress} on {Network}", contractAddress, network);
            return StatusCode(500, new { error = "Failed to retrieve contract balances", message = ex.Message });
        }
    }

    [HttpGet("{contractAddress}/transactions")]
    public async Task<IActionResult> GetContractTransactions(
        string contractAddress, 
        [FromQuery] string network = "testnet",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var transactions = await _neoRpcService.GetContractTransactionsAsync(contractAddress, network, page, pageSize);
            var transactionInfos = transactions.Select(t => new TransactionInfo
            {
                Hash = t.TransactionHash,
                BlockHash = t.BlockHash,
                BlockIndex = t.BlockIndex,
                Timestamp = t.Timestamp,
                From = t.FromAddress,
                To = t.ToAddress,
                Method = t.Method,
                Parameters = System.Text.Json.JsonSerializer.Deserialize<object[]>(t.Parameters) ?? Array.Empty<object>(),
                Result = t.Result,
                Success = t.Success,
                GasConsumed = t.GasConsumed,
                Exception = t.Exception
            }).ToList();

            return Ok(new
            {
                transactions = transactionInfos,
                page,
                pageSize,
                totalCount = transactions.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contract transactions for {ContractAddress} on {Network}", contractAddress, network);
            return StatusCode(500, new { error = "Failed to retrieve contract transactions", message = ex.Message });
        }
    }

    [HttpPost("{contractAddress}/invoke")]
    public async Task<IActionResult> InvokeContractMethod(
        string contractAddress,
        [FromBody] InvokeMethodRequest request)
    {
        try
        {
            var result = await _neoRpcService.InvokeContractMethodAsync(
                contractAddress,
                request.Method,
                request.Parameters,
                request.Network,
                request.SignerAddress);

            return Ok(new
            {
                success = true,
                result = result,
                contractAddress,
                method = request.Method,
                network = request.Network
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invoking contract method {Method} for {ContractAddress} on {Network}", 
                request.Method, contractAddress, request.Network);
            return StatusCode(500, new { error = "Contract invocation failed", message = ex.Message });
        }
    }

    [HttpPost("{contractAddress}/estimate-gas")]
    public async Task<IActionResult> EstimateGas(
        string contractAddress,
        [FromBody] EstimateGasRequest request)
    {
        try
        {
            var gasEstimate = await _neoRpcService.EstimateGasAsync(
                contractAddress,
                request.Method,
                request.Parameters,
                request.Network);

            return Ok(new
            {
                gasEstimate,
                method = request.Method,
                contractAddress,
                network = request.Network
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error estimating gas for {Method} on {ContractAddress} on {Network}", 
                request.Method, contractAddress, request.Network);
            return StatusCode(500, new { error = "Gas estimation failed", message = ex.Message });
        }
    }

    [HttpPost("{contractAddress}/send-transaction")]
    public async Task<IActionResult> SendTransaction(
        string contractAddress,
        [FromBody] SendTransactionRequest request)
    {
        try
        {
            var result = await _neoRpcService.SendTransactionAsync(request.SignedTransaction, request.Network);
            
            return Ok(new
            {
                success = true,
                transactionHash = result,
                contractAddress,
                network = request.Network
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending transaction for {ContractAddress} on {Network}", contractAddress, request.Network);
            return StatusCode(500, new { error = "Transaction failed", message = ex.Message });
        }
    }

    [HttpGet("{contractAddress}/validate")]
    public async Task<IActionResult> ValidateContract(string contractAddress, [FromQuery] string network = "testnet")
    {
        try
        {
            var isDeployed = await _neoRpcService.IsContractDeployedAsync(contractAddress, network);
            var contractState = await _neoRpcService.GetContractStateAsync(contractAddress, network);
            
            return Ok(new
            {
                isDeployed,
                isValid = contractState != null,
                contractAddress,
                network,
                hasManifest = contractState?["manifest"] != null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating contract {ContractAddress} on {Network}", contractAddress, network);
            return StatusCode(500, new { error = "Contract validation failed", message = ex.Message });
        }
    }

    [HttpPost("{contractAddress}/generate-webgui")]
    public async Task<IActionResult> GenerateWebGUI(
        string contractAddress,
        [FromBody] GenerateWebGUIRequest request)
    {
        try
        {
            var manifest = await _neoRpcService.GetContractManifestAsync(contractAddress, request.Network);
            if (manifest == null)
            {
                return NotFound(new { error = "Contract not found or invalid" });
            }

            var generatedFiles = await _webGUIGeneratorService.GenerateFromManifestAsync(manifest, request.TemplateType);
            
            return Ok(new
            {
                success = true,
                contractAddress,
                network = request.Network,
                templateType = request.TemplateType,
                files = generatedFiles.Keys.ToList(),
                filesGenerated = generatedFiles.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating WebGUI for {ContractAddress} on {Network}", contractAddress, request.Network);
            return StatusCode(500, new { error = "WebGUI generation failed", message = ex.Message });
        }
    }

    [HttpGet("templates")]
    public async Task<IActionResult> GetAvailableTemplates()
    {
        try
        {
            var templates = await _webGUIGeneratorService.GetAvailableTemplatesAsync();
            return Ok(templates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available templates");
            return StatusCode(500, new { error = "Failed to retrieve templates", message = ex.Message });
        }
    }
}

// Request DTOs
public class InvokeMethodRequest
{
    public string Method { get; set; } = string.Empty;
    public object[] Parameters { get; set; } = Array.Empty<object>();
    public string Network { get; set; } = "testnet";
    public string? SignerAddress { get; set; }
}

public class EstimateGasRequest
{
    public string Method { get; set; } = string.Empty;
    public object[] Parameters { get; set; } = Array.Empty<object>();
    public string Network { get; set; } = "testnet";
}

public class SendTransactionRequest
{
    public string SignedTransaction { get; set; } = string.Empty;
    public string Network { get; set; } = "testnet";
}

public class GenerateWebGUIRequest
{
    public string Network { get; set; } = "testnet";
    public string TemplateType { get; set; } = "standard";
    public Dictionary<string, string> CustomSettings { get; set; } = new();
}