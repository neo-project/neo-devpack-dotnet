using Microsoft.AspNetCore.Mvc;
using R3E.WebGUI.Service.Core.Services;

namespace R3E.WebGUI.Service.API.Controllers;

[ApiController]
public class SubdomainController : ControllerBase
{
    private readonly IWebGUIService _webGUIService;
    private readonly IStorageService _storageService;
    private readonly IContractConfigService _contractConfigService;
    private readonly ILogger<SubdomainController> _logger;

    public SubdomainController(
        IWebGUIService webGUIService,
        IStorageService storageService,
        IContractConfigService contractConfigService,
        ILogger<SubdomainController> logger)
    {
        _webGUIService = webGUIService;
        _storageService = storageService;
        _contractConfigService = contractConfigService;
        _logger = logger;
    }

    [HttpGet("/subdomain")]
    public async Task<IActionResult> ServeSubdomain()
    {
        var host = Request.Host.Host;
        
        // Extract subdomain from host
        var subdomain = ExtractSubdomain(host);
        if (string.IsNullOrEmpty(subdomain))
        {
            // Redirect to home controller for main domain
            return RedirectToAction("Index", "Home");
        }

        try
        {
            var webGUI = await _webGUIService.GetBySubdomainAsync(subdomain);
            if (webGUI == null)
            {
                return NotFound($"WebGUI not found for subdomain: {subdomain}");
            }

            // Check if contract has JSON configuration
            var config = await _contractConfigService.LoadConfigAsync(webGUI.ContractAddress);
            if (config != null)
            {
                // Serve modern WebGUI template
                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "modern-webgui.html");
                if (System.IO.File.Exists(templatePath))
                {
                    var template = await System.IO.File.ReadAllTextAsync(templatePath);
                    
                    // Inject contract address for JavaScript to pick up
                    template = template.Replace("<head>", 
                        $"<head>\n    <meta name=\"contract-address\" content=\"{webGUI.ContractAddress}\">");
                    
                    return Content(template, "text/html");
                }
            }

            // Fallback: try to serve custom index.html file
            try
            {
                var indexStream = await _storageService.GetFileAsync(subdomain, "index.html");
                return new FileStreamResult(indexStream, "text/html");
            }
            catch (FileNotFoundException)
            {
                // No custom files, return default template
                return Content(GenerateDefaultWebGUI(webGUI), "text/html");
            }
        }
        catch (FileNotFoundException)
        {
            return NotFound("WebGUI files not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error serving subdomain {subdomain}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("/subdomain/{**path}")]
    public async Task<IActionResult> ServeSubdomainFile(string path)
    {
        var host = Request.Host.Host;
        var subdomain = ExtractSubdomain(host);
        
        if (string.IsNullOrEmpty(subdomain))
        {
            return NotFound();
        }

        try
        {
            // Check if it's a template file request
            if (path == "modern-webgui.js" || path == "neo-wallet-adapter.js")
            {
                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", path);
                if (System.IO.File.Exists(templatePath))
                {
                    var content = await System.IO.File.ReadAllTextAsync(templatePath);
                    return Content(content, "application/javascript");
                }
            }

            var webGUI = await _webGUIService.GetBySubdomainAsync(subdomain);
            if (webGUI == null)
            {
                return NotFound();
            }

            var fileStream = await _storageService.GetFileAsync(subdomain, path);
            var contentType = GetContentType(path);
            
            return new FileStreamResult(fileStream, contentType);
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error serving file {path} for subdomain {subdomain}");
            return StatusCode(500);
        }
    }

    private string ExtractSubdomain(string host)
    {
        // Extract subdomain from host like "mycontract.localhost:8888"
        // First remove port if present
        var hostWithoutPort = host.Split(':')[0];
        var parts = hostWithoutPort.Split('.');
        
        // Check if it's a subdomain of localhost
        if (parts.Length >= 2 && parts[^1] == "localhost")
        {
            return parts[0];
        }
        return string.Empty;
    }

    private string GenerateDefaultWebGUI(Domain.Models.ContractWebGUI webGUI)
    {
        return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <meta name=""contract-address"" content=""{webGUI.ContractAddress}"">
    <title>{webGUI.ContractName} - Neo Smart Contract</title>
    <style>
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
            margin: 0;
            padding: 2rem;
            background: #f8fafc;
            color: #1a1a1a;
        }}
        .container {{
            max-width: 800px;
            margin: 0 auto;
            background: white;
            padding: 2rem;
            border-radius: 12px;
            box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
        }}
        h1 {{
            color: #667eea;
            margin-bottom: 1rem;
        }}
        .meta {{
            background: #f8fafc;
            padding: 1rem;
            border-radius: 8px;
            margin: 1rem 0;
        }}
        .meta strong {{
            color: #374151;
        }}
        .upgrade-notice {{
            background: linear-gradient(135deg, #667eea, #764ba2);
            color: white;
            padding: 1.5rem;
            border-radius: 8px;
            margin-top: 2rem;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <h1>{webGUI.ContractName}</h1>
        <p>{webGUI.Description ?? "Smart contract interface"}</p>
        
        <div class=""meta"">
            <strong>Contract Address:</strong> {webGUI.ContractAddress}<br>
            <strong>Network:</strong> {webGUI.Network}<br>
            <strong>Deployed:</strong> {webGUI.DeployedAt:yyyy-MM-dd}
        </div>
        
        <div class=""upgrade-notice"">
            <h3>🚀 Enhanced Interface Available</h3>
            <p>This contract can be upgraded to use our modern WebGUI interface with wallet integration, method invocation, and real-time data.</p>
            <p>Contact the contract deployer to enable the enhanced interface.</p>
        </div>
    </div>
</body>
</html>";
    }

    private string GetContentType(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();
        return extension switch
        {
            ".html" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".json" => "application/json",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            ".ico" => "image/x-icon",
            ".woff" => "font/woff",
            ".woff2" => "font/woff2",
            ".ttf" => "font/ttf",
            ".eot" => "application/vnd.ms-fontobject",
            _ => "application/octet-stream"
        };
    }
}