using System.Text;
using System.Text.Json;

namespace R3E.WebGUI.Deploy.Services;

public class WebGUIDeployer
{
    private readonly HttpClient _httpClient;
    private readonly string _serviceUrl;

    public WebGUIDeployer(string serviceUrl)
    {
        _serviceUrl = serviceUrl;
        _httpClient = new HttpClient();
    }

    public async Task DeployAsync(
        string contractAddress,
        string contractName,
        string network,
        string deployerAddress,
        string webguiPath,
        string? description = null)
    {
        try
        {
            Console.WriteLine($"🚀 Deploying WebGUI for contract {contractName} ({contractAddress})...");
            
            if (!Directory.Exists(webguiPath))
            {
                throw new DirectoryNotFoundException($"WebGUI directory not found: {webguiPath}");
            }

            // Prepare multipart form data
            using var form = new MultipartFormDataContent();
            
            // Add metadata
            form.Add(new StringContent(contractAddress), "contractAddress");
            form.Add(new StringContent(contractName), "contractName");
            form.Add(new StringContent(network), "network");
            form.Add(new StringContent(deployerAddress), "deployerAddress");
            
            if (!string.IsNullOrEmpty(description))
            {
                form.Add(new StringContent(description), "description");
            }

            // Add WebGUI files
            await AddFilesToForm(form, webguiPath);

            // Deploy to service
            var response = await _httpClient.PostAsync($"{_serviceUrl}/api/webgui/deploy", form);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<DeploymentResult>(responseContent, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                Console.WriteLine("✅ WebGUI deployed successfully!");
                Console.WriteLine($"🌐 Subdomain: {result?.Subdomain}");
                Console.WriteLine($"🔗 URL: {result?.Url}");
                Console.WriteLine($"📍 Contract: {result?.ContractAddress}");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Deployment failed: {response.StatusCode}");
                Console.WriteLine($"Error: {errorContent}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Deployment failed: {ex.Message}");
            throw;
        }
    }

    private async Task AddFilesToForm(MultipartFormDataContent form, string webguiPath)
    {
        var files = Directory.GetFiles(webguiPath, "*", SearchOption.AllDirectories);
        
        Console.WriteLine($"📁 Adding {files.Length} files...");

        foreach (var filePath in files)
        {
            var relativePath = Path.GetRelativePath(webguiPath, filePath);
            var fileContent = await File.ReadAllBytesAsync(filePath);
            var byteContent = new ByteArrayContent(fileContent);
            
            // Set proper content type
            var contentType = GetContentType(filePath);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            
            form.Add(byteContent, "webGUIFiles", relativePath);
            Console.WriteLine($"  📄 {relativePath}");
        }
    }

    private string GetContentType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
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
            _ => "application/octet-stream"
        };
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

public class DeploymentResult
{
    public bool Success { get; set; }
    public string? Subdomain { get; set; }
    public string? Url { get; set; }
    public string? ContractAddress { get; set; }
}