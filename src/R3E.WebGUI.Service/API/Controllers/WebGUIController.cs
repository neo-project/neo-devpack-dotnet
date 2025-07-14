using System.Security.Cryptography;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using R3E.WebGUI.Service.Core.Services;
using R3E.WebGUI.Service.Domain.Models;

namespace R3E.WebGUI.Service.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebGUIController : ControllerBase
{
    private readonly IWebGUIService _webGUIService;
    private readonly INeoRpcService _neoRpcService;
    private readonly IWebGUIGeneratorService _webGUIGeneratorService;
    private readonly IContractConfigService _contractConfigService;
    private readonly IStorageService _storageService;
    private readonly ILogger<WebGUIController> _logger;
    private readonly IValidator<DeployWebGUIRequest> _deployValidator;

    public WebGUIController(
        IWebGUIService webGUIService,
        INeoRpcService neoRpcService,
        IWebGUIGeneratorService webGUIGeneratorService,
        IContractConfigService contractConfigService,
        IStorageService storageService,
        ILogger<WebGUIController> logger,
        IValidator<DeployWebGUIRequest> deployValidator)
    {
        _webGUIService = webGUIService;
        _neoRpcService = neoRpcService;
        _webGUIGeneratorService = webGUIGeneratorService;
        _contractConfigService = contractConfigService;
        _storageService = storageService;
        _logger = logger;
        _deployValidator = deployValidator;
    }

    [Obsolete("Use /api/webgui/deploy-from-manifest for JSON-based deployment with signature authentication")]
    [HttpPost("deploy")]
    public async Task<IActionResult> DeployWebGUI([FromForm] DeployWebGUIRequest request)
    {
        return BadRequest(new 
        { 
            error = "This endpoint is deprecated", 
            message = "Please use POST /api/webgui/deploy-from-manifest for JSON-based deployment with signature authentication",
            newEndpoint = "/api/webgui/deploy-from-manifest",
            documentation = "/api/docs#deploy-from-manifest"
        });
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchByContract([FromQuery] string contractAddress, [FromQuery] string? network = "testnet")
    {
        try
        {
            var results = await _webGUIService.SearchByContractAddressAsync(contractAddress, network);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for contract");
            return StatusCode(500, new { error = "Search failed", message = ex.Message });
        }
    }

    [HttpGet("{subdomain}")]
    public async Task<IActionResult> GetWebGUIInfo(string subdomain)
    {
        try
        {
            var webGUI = await _webGUIService.GetBySubdomainAsync(subdomain);
            if (webGUI == null)
            {
                return NotFound(new { error = "WebGUI not found" });
            }

            return Ok(webGUI);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving WebGUI info");
            return StatusCode(500, new { error = "Failed to retrieve WebGUI", message = ex.Message });
        }
    }

    [HttpGet("list")]
    public async Task<IActionResult> ListWebGUIs([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? network = null)
    {
        try
        {
            var results = await _webGUIService.ListWebGUIsAsync(page, pageSize, network);
            return Ok(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing WebGUIs");
            return StatusCode(500, new { error = "Failed to list WebGUIs", message = ex.Message });
        }
    }

    [Obsolete("WebGUI updates should be done through deploy-from-manifest with new configuration")]
    [HttpPut("{subdomain}/update")]
    public async Task<IActionResult> UpdateWebGUI(string subdomain, [FromForm] UpdateWebGUIRequest request)
    {
        return BadRequest(new 
        { 
            error = "This endpoint is deprecated", 
            message = "To update a WebGUI, redeploy using POST /api/webgui/deploy-from-manifest with updated configuration",
            newEndpoint = "/api/webgui/deploy-from-manifest",
            documentation = "/api/docs#deploy-from-manifest"
        });
    }

    [Obsolete("Use /api/webgui/deploy-from-manifest which includes signature authentication")]
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateWebGUI([FromBody] AutoGenerateWebGUIRequest request)
    {
        return BadRequest(new 
        { 
            error = "This endpoint is deprecated", 
            message = "Please use POST /api/webgui/deploy-from-manifest which automatically generates WebGUI from manifest with signature authentication",
            newEndpoint = "/api/webgui/deploy-from-manifest",
            documentation = "/api/docs#deploy-from-manifest"
        });
    }

    // Preview endpoint - keeping for backward compatibility but simplified
    [HttpGet("{subdomain}/preview")]
    public async Task<IActionResult> PreviewWebGUI(string subdomain)
    {
        try
        {
            var webGUI = await _webGUIService.GetBySubdomainAsync(subdomain);
            if (webGUI == null)
            {
                return NotFound(new { error = "WebGUI not found" });
            }

            return Ok(new
            {
                subdomain,
                contractAddress = webGUI.ContractAddress,
                contractName = webGUI.ContractName,
                network = webGUI.Network,
                url = $"http://{subdomain}.localhost:8888"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error previewing WebGUI for subdomain {Subdomain}", subdomain);
            return StatusCode(500, new { error = "Failed to preview WebGUI", message = ex.Message });
        }
    }

    [HttpPost("deploy-from-manifest")]
    public async Task<IActionResult> DeployFromManifest(
        [FromBody] DeployFromManifestRequest request,
        [FromServices] ISignatureValidationService signatureService)
    {
        try
        {
            // Validate timestamp
            if (!signatureService.IsTimestampValid(request.Timestamp))
            {
                return BadRequest(new { error = "Invalid or expired timestamp" });
            }

            // Create and validate signature
            var message = signatureService.CreateDeploymentMessage(
                request.ContractAddress, 
                request.DeployerAddress, 
                request.Timestamp);
                
            var isSignatureValid = await signatureService.ValidateSignatureAsync(
                message, 
                request.Signature, 
                request.PublicKey, 
                request.DeployerAddress);
                
            if (!isSignatureValid)
            {
                return Unauthorized(new { error = "Invalid signature or unauthorized deployer" });
            }

            // Validate contract
            var isDeployed = await _neoRpcService.IsContractDeployedAsync(request.ContractAddress, request.Network);
            if (!isDeployed)
            {
                return BadRequest(new { error = "Contract not found on the specified network" });
            }

            // Get contract manifest
            var manifest = await _neoRpcService.GetContractManifestAsync(request.ContractAddress, request.Network);
            if (manifest == null)
            {
                return BadRequest(new { error = "Could not retrieve contract manifest" });
            }

            // Generate contract configuration
            var config = await _contractConfigService.GenerateConfigFromManifestAsync(
                request.ContractAddress,
                request.ContractName ?? manifest.Name,
                request.Network,
                request.DeployerAddress,
                System.Text.Json.JsonSerializer.Serialize(manifest),
                request.Description);

            // Deploy WebGUI using JSON configuration
            var webGuiEntry = await _webGUIService.DeployWebGUIFromConfigAsync(
                config,
                request.DeployerAddress);

            return Ok(new
            {
                success = true,
                subdomain = webGuiEntry.Subdomain,
                url = $"http://{webGuiEntry.Subdomain}.localhost:8888",
                contractAddress = webGuiEntry.ContractAddress,
                contractName = webGuiEntry.ContractName,
                configGenerated = true,
                methodsFound = config.Methods.Count,
                eventsFound = config.Events.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deploying WebGUI from manifest");
            return StatusCode(500, new { error = "Failed to deploy WebGUI", message = ex.Message });
        }
    }

    [HttpGet("{contractAddress}/config")]
    public async Task<IActionResult> GetContractConfig(string contractAddress)
    {
        try
        {
            var config = await _contractConfigService.LoadConfigAsync(contractAddress);
            if (config == null)
            {
                return NotFound(new { error = "Contract configuration not found" });
            }

            return Ok(config);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contract config");
            return StatusCode(500, new { error = "Failed to retrieve config", message = ex.Message });
        }
    }

    [HttpPost("{contractAddress}/plugin")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10MB limit
    public async Task<IActionResult> UploadPlugin(
        string contractAddress, 
        IFormFile pluginFile,
        [FromHeader(Name = "X-Timestamp")] long timestamp,
        [FromHeader(Name = "X-Signature")] string signature,
        [FromHeader(Name = "X-Public-Key")] string publicKey,
        [FromServices] ISignatureValidationService signatureService)
    {
        try
        {
            if (pluginFile == null || pluginFile.Length == 0)
            {
                return BadRequest(new { error = "Plugin file is required" });
            }

            // Validate file size (max 10MB)
            if (pluginFile.Length > 10 * 1024 * 1024)
            {
                return BadRequest(new { error = "Plugin file size exceeds 10MB limit" });
            }

            // Validate file type (should be .zip)
            var extension = Path.GetExtension(pluginFile.FileName).ToLowerInvariant();
            if (extension != ".zip")
            {
                return BadRequest(new { error = "Plugin must be a ZIP file" });
            }

            // Validate contract address format
            if (!IsValidContractAddress(contractAddress))
            {
                return BadRequest(new { error = "Invalid contract address format" });
            }

            // Check if contract exists
            var webGUIs = await _webGUIService.SearchByContractAddressAsync(contractAddress);
            var webGUI = webGUIs.FirstOrDefault();
            if (webGUI == null)
            {
                return NotFound(new { error = "Contract WebGUI not found" });
            }

            // Validate timestamp
            if (!signatureService.IsTimestampValid(timestamp))
            {
                return BadRequest(new { error = "Invalid or expired timestamp" });
            }

            // Calculate plugin hash
            var pluginBytes = await GetFileBytes(pluginFile);
            var pluginHash = Convert.ToHexString(SHA256.HashData(pluginBytes)).ToLowerInvariant();

            // Create and validate signature
            var message = signatureService.CreatePluginUploadMessage(contractAddress, pluginHash, timestamp);
            var isSignatureValid = await signatureService.ValidateSignatureAsync(
                message, 
                signature, 
                publicKey, 
                webGUI.DeployerAddress);
                
            if (!isSignatureValid)
            {
                return Unauthorized(new { error = "Invalid signature or unauthorized uploader" });
            }

            // Save plugin file
            var pluginPath = Path.Combine("plugins", $"{contractAddress}-plugin.zip");
            await _storageService.SaveFileAsync("plugins", $"{contractAddress}-plugin.zip", 
                Convert.ToBase64String(await GetFileBytes(pluginFile)));

            // Update config with plugin URL
            var config = await _contractConfigService.LoadConfigAsync(contractAddress);
            if (config != null)
            {
                config.PluginDownloadUrl = $"/api/webgui/{contractAddress}/plugin/download";
                await _contractConfigService.SaveConfigAsync(contractAddress, config);
            }

            return Ok(new 
            { 
                success = true, 
                message = "Plugin uploaded successfully",
                pluginUrl = $"/api/webgui/{contractAddress}/plugin/download"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading plugin for contract {ContractAddress}", contractAddress);
            return StatusCode(500, new { error = "Failed to upload plugin", message = ex.Message });
        }
    }

    [HttpGet("{contractAddress}/plugin/download")]
    public async Task<IActionResult> DownloadPlugin(string contractAddress)
    {
        try
        {
            var pluginContent = await _storageService.GetFileContentAsync("plugins", $"{contractAddress}-plugin.zip");
            if (string.IsNullOrEmpty(pluginContent))
            {
                return NotFound(new { error = "Plugin not found for this contract" });
            }

            var bytes = Convert.FromBase64String(pluginContent);
            return File(bytes, "application/zip", $"{contractAddress}-plugin.zip");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading plugin for contract {ContractAddress}", contractAddress);
            return StatusCode(500, new { error = "Failed to download plugin", message = ex.Message });
        }
    }

    private async Task<byte[]> GetFileBytes(IFormFile file)
    {
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        return stream.ToArray();
    }

    private string GenerateSubdomain(string contractName)
    {
        return contractName.ToLowerInvariant()
            .Replace(" ", "")
            .Replace("-", "")
            .Replace("_", "");
    }

    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".html" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".json" => "application/json",
            _ => "text/plain"
        };
    }

    private bool IsValidContractAddress(string address)
    {
        // Neo contract address validation (0x prefix + 40 hex chars)
        return System.Text.RegularExpressions.Regex.IsMatch(address, @"^0x[a-fA-F0-9]{40}$");
    }
}

[Obsolete("Use DeployFromManifestRequest for JSON-based deployment")]
public class DeployWebGUIRequest
{
    public string ContractAddress { get; set; } = string.Empty;
    public string ContractName { get; set; } = string.Empty;
    public string Network { get; set; } = "testnet";
    public string DeployerAddress { get; set; } = string.Empty;
    public string? Description { get; set; }
    public IFormFileCollection? WebGUIFiles { get; set; }
}

[Obsolete("Updates should be done through redeployment")]
public class UpdateWebGUIRequest
{
    public string? Description { get; set; }
    public IFormFileCollection? WebGUIFiles { get; set; }
}

[Obsolete("Use DeployFromManifestRequest")]
public class AutoGenerateWebGUIRequest
{
    public string ContractAddress { get; set; } = string.Empty;
    public string Network { get; set; } = "testnet";
    public string DeployerAddress { get; set; } = string.Empty;
    public string TemplateType { get; set; } = "standard";
    public Dictionary<string, string> CustomSettings { get; set; } = new();
}

public class DeployFromManifestRequest
{
    public string ContractAddress { get; set; } = string.Empty;
    public string? ContractName { get; set; }
    public string Network { get; set; } = "testnet";
    public string DeployerAddress { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long Timestamp { get; set; }
    public string Signature { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
    public Dictionary<string, string> CustomSettings { get; set; } = new();
}