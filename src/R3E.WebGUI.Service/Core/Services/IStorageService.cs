using System.Text.Json;
using R3E.WebGUI.Service.Domain.Models;

namespace R3E.WebGUI.Service.Core.Services;

public interface IStorageService
{
    Task<string> UploadWebGUIFilesAsync(string subdomain, IFormFileCollection files);
    Task<Stream> GetFileAsync(string subdomain, string fileName);
    Task<bool> DeleteWebGUIFilesAsync(string subdomain);
    Task<IEnumerable<string>> ListFilesAsync(string subdomain);
    Task SaveFileAsync(string directory, string fileName, string content);
    Task<string> GetFileContentAsync(string directory, string fileName);
    Task SaveConfigAsync(string contractAddress, ContractWebGUIConfig config);
}

public class LocalStorageService : IStorageService
{
    private readonly string _basePath;
    private readonly ILogger<LocalStorageService> _logger;

    public LocalStorageService(IConfiguration configuration, ILogger<LocalStorageService> logger)
    {
        _basePath = configuration.GetValue<string>("Storage:LocalPath") ?? "./webgui-storage";
        _logger = logger;
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> UploadWebGUIFilesAsync(string subdomain, IFormFileCollection files)
    {
        var subdomainPath = Path.Combine(_basePath, subdomain);
        
        // Clean up existing files
        if (Directory.Exists(subdomainPath))
        {
            Directory.Delete(subdomainPath, true);
        }
        
        Directory.CreateDirectory(subdomainPath);

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(subdomainPath, fileName);
                
                // Create subdirectories if needed
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
            }
        }

        _logger.LogInformation($"Uploaded {files.Count} files for subdomain {subdomain}");
        return subdomainPath;
    }

    public async Task<Stream> GetFileAsync(string subdomain, string fileName)
    {
        var filePath = Path.Combine(_basePath, subdomain, fileName);
        
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {fileName}");
        }

        return new FileStream(filePath, FileMode.Open, FileAccess.Read);
    }

    public async Task<bool> DeleteWebGUIFilesAsync(string subdomain)
    {
        var subdomainPath = Path.Combine(_basePath, subdomain);
        
        if (Directory.Exists(subdomainPath))
        {
            Directory.Delete(subdomainPath, true);
            _logger.LogInformation($"Deleted files for subdomain {subdomain}");
            return true;
        }

        return false;
    }

    public async Task<IEnumerable<string>> ListFilesAsync(string subdomain)
    {
        var subdomainPath = Path.Combine(_basePath, subdomain);
        
        if (!Directory.Exists(subdomainPath))
        {
            return Array.Empty<string>();
        }

        return Directory.GetFiles(subdomainPath, "*", SearchOption.AllDirectories)
            .Select(f => Path.GetRelativePath(subdomainPath, f));
    }

    public async Task SaveFileAsync(string directory, string fileName, string content)
    {
        var directoryPath = Path.Combine(_basePath, directory);
        Directory.CreateDirectory(directoryPath);
        
        var filePath = Path.Combine(directoryPath, fileName);
        await File.WriteAllTextAsync(filePath, content);
        
        _logger.LogInformation($"Saved file {fileName} in directory {directory}");
    }

    public async Task<string> GetFileContentAsync(string directory, string fileName)
    {
        var filePath = Path.Combine(_basePath, directory, fileName);
        
        if (!File.Exists(filePath))
        {
            return string.Empty;
        }

        return await File.ReadAllTextAsync(filePath);
    }

    public async Task SaveConfigAsync(string contractAddress, ContractWebGUIConfig config)
    {
        var configDirectory = Path.Combine(_basePath, "configs");
        Directory.CreateDirectory(configDirectory);
        
        var fileName = $"{contractAddress}.webgui.json";
        var filePath = Path.Combine(configDirectory, fileName);
        
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        await File.WriteAllTextAsync(filePath, json);
        _logger.LogInformation($"Saved contract configuration for {contractAddress}");
    }
}