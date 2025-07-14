using R3E.WebGUI.Service.Domain.Models;

namespace R3E.WebGUI.Service.Infrastructure.Repositories;

public interface IWebGUIRepository
{
    Task<ContractWebGUI> CreateAsync(ContractWebGUI webGUI);
    Task<ContractWebGUI?> GetByIdAsync(Guid id);
    Task<ContractWebGUI?> GetBySubdomainAsync(string subdomain);
    Task<IEnumerable<ContractWebGUI>> SearchByContractAddressAsync(string contractAddress, string? network = null);
    Task<(IEnumerable<ContractWebGUI> items, int totalCount)> GetPagedAsync(int page, int pageSize, string? network = null);
    Task<ContractWebGUI> UpdateAsync(ContractWebGUI webGUI);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> SubdomainExistsAsync(string subdomain);
}