using R3E.WebGUI.Service.Domain.Models;

namespace R3E.WebGUI.Service.Core.Services;

public interface IWebGUIService
{
    Task<ContractWebGUI> DeployWebGUIAsync(
        string contractAddress,
        string contractName,
        string network,
        IFormFileCollection files,
        string deployerAddress,
        string? description = null);

    Task<ContractWebGUI?> GetBySubdomainAsync(string subdomain);
    Task<IEnumerable<ContractWebGUI>> SearchByContractAddressAsync(string contractAddress, string? network = null);
    Task<PagedResult<ContractWebGUI>> ListWebGUIsAsync(int page, int pageSize, string? network = null);
    Task<ContractWebGUI> UpdateWebGUIAsync(string subdomain, IFormFileCollection? files, string? description);
    Task<bool> DeleteWebGUIAsync(string subdomain);
    Task<string> GenerateUniqueSubdomainAsync(string contractName);
    
    Task<ContractWebGUI> DeployWebGUIFromConfigAsync(
        ContractWebGUIConfig config,
        string deployerAddress);
}

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Array.Empty<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}