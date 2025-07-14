using Microsoft.EntityFrameworkCore;
using R3E.WebGUI.Service.Domain.Models;
using R3E.WebGUI.Service.Infrastructure.Data;

namespace R3E.WebGUI.Service.Infrastructure.Repositories;

public class WebGUIRepository : IWebGUIRepository
{
    private readonly WebGUIDbContext _context;

    public WebGUIRepository(WebGUIDbContext context)
    {
        _context = context;
    }

    public async Task<ContractWebGUI> CreateAsync(ContractWebGUI webGUI)
    {
        _context.WebGUIs.Add(webGUI);
        await _context.SaveChangesAsync();
        return webGUI;
    }

    public async Task<ContractWebGUI?> GetByIdAsync(Guid id)
    {
        return await _context.WebGUIs.FindAsync(id);
    }

    public async Task<ContractWebGUI?> GetBySubdomainAsync(string subdomain)
    {
        return await _context.WebGUIs
            .FirstOrDefaultAsync(w => w.Subdomain == subdomain && w.IsActive);
    }

    public async Task<IEnumerable<ContractWebGUI>> SearchByContractAddressAsync(string contractAddress, string? network = null)
    {
        var query = _context.WebGUIs
            .Where(w => w.ContractAddress == contractAddress.ToLowerInvariant() && w.IsActive);

        if (!string.IsNullOrEmpty(network))
        {
            query = query.Where(w => w.Network == network.ToLowerInvariant());
        }

        return await query
            .OrderByDescending(w => w.DeployedAt)
            .ToListAsync();
    }

    public async Task<(IEnumerable<ContractWebGUI> items, int totalCount)> GetPagedAsync(int page, int pageSize, string? network = null)
    {
        var query = _context.WebGUIs.Where(w => w.IsActive);

        if (!string.IsNullOrEmpty(network))
        {
            query = query.Where(w => w.Network == network.ToLowerInvariant());
        }

        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderByDescending(w => w.DeployedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<ContractWebGUI> UpdateAsync(ContractWebGUI webGUI)
    {
        _context.WebGUIs.Update(webGUI);
        await _context.SaveChangesAsync();
        return webGUI;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var webGUI = await _context.WebGUIs.FindAsync(id);
        if (webGUI == null)
        {
            return false;
        }

        _context.WebGUIs.Remove(webGUI);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SubdomainExistsAsync(string subdomain)
    {
        return await _context.WebGUIs
            .AnyAsync(w => w.Subdomain == subdomain && w.IsActive);
    }
}