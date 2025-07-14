using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using R3E.WebGUI.Service.Core.Services;
using R3E.WebGUI.Service.Infrastructure.Data;
using R3E.WebGUI.Service.Domain.Models;
using System.Text;

namespace R3E.WebGUI.Service.API.Controllers;

[ApiController]
public class HomeController : ControllerBase
{
    private readonly WebGUIDbContext _context;
    private readonly IWebGUIService _webGUIService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        WebGUIDbContext context,
        IWebGUIService webGUIService,
        IConfiguration configuration,
        ILogger<HomeController> logger)
    {
        _context = context;
        _webGUIService = webGUIService;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet("/")]
    [HttpGet("/home")]
    public async Task<IActionResult> Index()
    {
        try
        {
            var stats = await GetStatisticsAsync();
            var featuredContracts = await GetFeaturedContractsAsync();
            var recentContracts = await GetRecentContractsAsync();
            
            var html = GenerateHomePage(stats, featuredContracts, recentContracts);
            return Content(html, "text/html", Encoding.UTF8);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not connect to database, showing default page");
            
            // Return a default page when database is not available
            var defaultStats = new Dictionary<string, object>
            {
                ["TotalContracts"] = 0,
                ["TestnetContracts"] = 0,
                ["MainnetContracts"] = 0,
                ["TotalTransactions"] = 0
            };
            
            var html = GenerateHomePage(defaultStats, new List<ContractWebGUI>(), new List<ContractWebGUI>());
            return Content(html, "text/html", Encoding.UTF8);
        }
    }

    [HttpGet("/search")]
    public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] string network = "all")
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return RedirectToAction(nameof(Index));
        }

        var results = await SearchContractsAsync(q, network);
        var html = GenerateSearchResultsPage(q, network, results);
        return Content(html, "text/html", Encoding.UTF8);
    }

    [HttpGet("/api/search")]
    public async Task<IActionResult> ApiSearch([FromQuery] string q, [FromQuery] string network = "all")
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest(new { error = "Search query is required" });
        }

        var results = await SearchContractsAsync(q, network);
        return Ok(new
        {
            query = q,
            network = network,
            count = results.Count,
            results = results.Select(r => new
            {
                r.Id,
                r.ContractAddress,
                r.ContractName,
                r.Network,
                r.Description,
                r.Subdomain,
                r.DeployedAt,
                url = $"http://{r.Subdomain}.{_configuration["R3EWebGUI:BaseDomain"] ?? "localhost"}:8888"
            })
        });
    }

    [HttpGet("/contract/{id}")]
    public async Task<IActionResult> ViewContract(string id)
    {
        try
        {
            var contract = await _context.WebGUIs
                .FirstOrDefaultAsync(c => c.Id.ToString() == id);

            if (contract == null)
            {
                return NotFound("Contract not found");
            }

            var html = GenerateContractViewerPage(contract);
            return Content(html, "text/html", Encoding.UTF8);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error viewing contract {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("/contracts")]
    public async Task<IActionResult> AllContracts([FromQuery] int page = 1, [FromQuery] string network = "all")
    {
        const int pageSize = 12;
        var query = _context.WebGUIs.Where(w => w.IsActive);
        
        if (network != "all")
        {
            query = query.Where(w => w.Network.ToLower() == network.ToLower());
        }

        var totalCount = await query.CountAsync();
        var contracts = await query
            .OrderByDescending(w => w.DeployedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var html = GenerateContractsListPage(contracts, page, pageSize, totalCount, network);
        return Content(html, "text/html", Encoding.UTF8);
    }

    private async Task<List<ContractWebGUI>> SearchContractsAsync(string query, string network)
    {
        var q = _context.WebGUIs.Where(w => w.IsActive);
        
        if (network != "all")
        {
            q = q.Where(w => w.Network.ToLower() == network.ToLower());
        }

        // Search in contract name, address, description, and subdomain
        q = q.Where(w => 
            w.ContractName.Contains(query) ||
            w.ContractAddress.Contains(query) ||
            w.Description.Contains(query) ||
            w.Subdomain.Contains(query));

        return await q.OrderByDescending(w => w.DeployedAt).Take(20).ToListAsync();
    }

    private async Task<Dictionary<string, object>> GetStatisticsAsync()
    {
        var totalContracts = await _context.WebGUIs.CountAsync(w => w.IsActive);
        var testnetContracts = await _context.WebGUIs.CountAsync(w => w.IsActive && w.Network == "testnet");
        var mainnetContracts = await _context.WebGUIs.CountAsync(w => w.IsActive && w.Network == "mainnet");
        var totalTransactions = await _context.ContractTransactions.CountAsync();

        return new Dictionary<string, object>
        {
            ["TotalContracts"] = totalContracts,
            ["TestnetContracts"] = testnetContracts,
            ["MainnetContracts"] = mainnetContracts,
            ["TotalTransactions"] = totalTransactions
        };
    }

    private async Task<List<ContractWebGUI>> GetFeaturedContractsAsync()
    {
        // For now, return the most recent contracts with descriptions
        return await _context.WebGUIs
            .Where(w => w.IsActive && !string.IsNullOrEmpty(w.Description))
            .OrderByDescending(w => w.DeployedAt)
            .Take(3)
            .ToListAsync();
    }

    private async Task<List<ContractWebGUI>> GetRecentContractsAsync()
    {
        return await _context.WebGUIs
            .Where(w => w.IsActive)
            .OrderByDescending(w => w.DeployedAt)
            .Take(6)
            .ToListAsync();
    }

    private string GenerateHomePage(Dictionary<string, object> stats, List<ContractWebGUI> featured, List<ContractWebGUI> recent)
    {
        var baseDomain = _configuration["R3EWebGUI:BaseDomain"] ?? "localhost";
        return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>R3E WebGUI Service - Neo N3 Contract Interface Platform</title>
    <link href=""https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap"" rel=""stylesheet"">
    <link href=""https://cdn.jsdelivr.net/npm/remixicon@3.5.0/fonts/remixicon.css"" rel=""stylesheet"">
    <style>
        :root {{
            --neo-green: #00d4aa;
            --neo-blue: #667eea;
            --neo-purple: #764ba2;
            --dark: #1a1a1a;
            --light: #f8f9fa;
            --gray: #6c757d;
            --white: #ffffff;
            --shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            --shadow-lg: 0 10px 25px rgba(0, 0, 0, 0.15);
        }}

        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}

        body {{
            font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            background: var(--light);
            color: var(--dark);
            line-height: 1.6;
        }}

        /* Header */
        header {{
            background: var(--white);
            box-shadow: var(--shadow);
            position: sticky;
            top: 0;
            z-index: 1000;
        }}

        nav {{
            max-width: 1200px;
            margin: 0 auto;
            padding: 1rem 2rem;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }}

        .logo {{
            font-size: 1.5rem;
            font-weight: 700;
            background: linear-gradient(135deg, var(--neo-blue), var(--neo-purple));
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            text-decoration: none;
        }}

        .nav-links {{
            display: flex;
            gap: 2rem;
            align-items: center;
        }}

        .nav-links a {{
            color: var(--dark);
            text-decoration: none;
            font-weight: 500;
            transition: color 0.3s;
        }}

        .nav-links a:hover {{
            color: var(--neo-blue);
        }}

        /* Hero Section */
        .hero {{
            background: linear-gradient(135deg, var(--neo-blue) 0%, var(--neo-purple) 100%);
            color: var(--white);
            padding: 5rem 2rem;
            text-align: center;
        }}

        .hero h1 {{
            font-size: 3rem;
            font-weight: 700;
            margin-bottom: 1rem;
        }}

        .hero p {{
            font-size: 1.25rem;
            margin-bottom: 2rem;
            opacity: 0.9;
        }}

        /* Search Bar */
        .search-container {{
            max-width: 600px;
            margin: 0 auto;
            position: relative;
        }}

        .search-form {{
            display: flex;
            gap: 1rem;
            background: var(--white);
            padding: 0.5rem;
            border-radius: 50px;
            box-shadow: var(--shadow-lg);
        }}

        .search-input {{
            flex: 1;
            padding: 1rem 1.5rem;
            border: none;
            outline: none;
            font-size: 1rem;
            background: transparent;
        }}

        .search-select {{
            padding: 0.5rem 1rem;
            border: none;
            outline: none;
            background: transparent;
            font-size: 1rem;
            cursor: pointer;
        }}

        .search-button {{
            padding: 1rem 2rem;
            background: var(--neo-green);
            color: var(--white);
            border: none;
            border-radius: 50px;
            font-weight: 600;
            cursor: pointer;
            transition: transform 0.2s, box-shadow 0.2s;
        }}

        .search-button:hover {{
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(0, 212, 170, 0.3);
        }}

        /* Statistics */
        .stats {{
            padding: 4rem 2rem;
            background: var(--white);
        }}

        .stats-grid {{
            max-width: 1200px;
            margin: 0 auto;
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 2rem;
        }}

        .stat-card {{
            text-align: center;
            padding: 2rem;
            border-radius: 12px;
            background: var(--light);
            transition: transform 0.3s;
        }}

        .stat-card:hover {{
            transform: translateY(-5px);
        }}

        .stat-number {{
            font-size: 2.5rem;
            font-weight: 700;
            color: var(--neo-blue);
        }}

        .stat-label {{
            color: var(--gray);
            font-size: 1rem;
            margin-top: 0.5rem;
        }}

        /* Contracts Section */
        .section {{
            padding: 4rem 2rem;
        }}

        .container {{
            max-width: 1200px;
            margin: 0 auto;
        }}

        .section-header {{
            text-align: center;
            margin-bottom: 3rem;
        }}

        .section-title {{
            font-size: 2.5rem;
            font-weight: 700;
            margin-bottom: 1rem;
        }}

        .section-subtitle {{
            color: var(--gray);
            font-size: 1.1rem;
        }}

        /* Contract Cards */
        .contracts-grid {{
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
            gap: 2rem;
        }}

        .contract-card {{
            background: var(--white);
            border-radius: 12px;
            padding: 2rem;
            box-shadow: var(--shadow);
            transition: transform 0.3s, box-shadow 0.3s;
            position: relative;
            overflow: hidden;
        }}

        .contract-card::before {{
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 4px;
            background: linear-gradient(90deg, var(--neo-green), var(--neo-blue));
        }}

        .contract-card:hover {{
            transform: translateY(-5px);
            box-shadow: var(--shadow-lg);
        }}

        .contract-header {{
            display: flex;
            justify-content: space-between;
            align-items: start;
            margin-bottom: 1rem;
        }}

        .contract-name {{
            font-size: 1.25rem;
            font-weight: 600;
            color: var(--dark);
            margin-bottom: 0.5rem;
        }}

        .contract-network {{
            display: inline-block;
            padding: 0.25rem 0.75rem;
            background: var(--neo-green);
            color: var(--white);
            border-radius: 20px;
            font-size: 0.8rem;
            font-weight: 600;
            text-transform: uppercase;
        }}

        .contract-network.mainnet {{
            background: var(--neo-blue);
        }}

        .contract-address {{
            font-family: monospace;
            font-size: 0.9rem;
            color: var(--gray);
            margin-bottom: 1rem;
            word-break: break-all;
        }}

        .contract-description {{
            color: var(--dark);
            margin-bottom: 1.5rem;
            line-height: 1.6;
        }}

        .contract-footer {{
            display: flex;
            justify-content: space-between;
            align-items: center;
        }}

        .contract-date {{
            color: var(--gray);
            font-size: 0.9rem;
        }}

        .contract-link {{
            display: inline-flex;
            align-items: center;
            gap: 0.5rem;
            color: var(--neo-blue);
            text-decoration: none;
            font-weight: 500;
            transition: color 0.3s;
        }}

        .contract-link:hover {{
            color: var(--neo-purple);
        }}

        /* CTA Section */
        .cta {{
            background: linear-gradient(135deg, var(--neo-purple), var(--neo-blue));
            color: var(--white);
            padding: 4rem 2rem;
            text-align: center;
        }}

        .cta h2 {{
            font-size: 2.5rem;
            margin-bottom: 1rem;
        }}

        .cta p {{
            font-size: 1.2rem;
            margin-bottom: 2rem;
            opacity: 0.9;
        }}

        .cta-buttons {{
            display: flex;
            gap: 1rem;
            justify-content: center;
            flex-wrap: wrap;
        }}

        .btn {{
            padding: 1rem 2rem;
            border-radius: 50px;
            text-decoration: none;
            font-weight: 600;
            transition: transform 0.2s, box-shadow 0.2s;
            display: inline-block;
        }}

        .btn-primary {{
            background: var(--neo-green);
            color: var(--white);
        }}

        .btn-secondary {{
            background: var(--white);
            color: var(--neo-blue);
        }}

        .btn:hover {{
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.2);
        }}

        /* Footer */
        footer {{
            background: var(--dark);
            color: var(--white);
            padding: 3rem 2rem;
            text-align: center;
        }}

        .footer-content {{
            max-width: 1200px;
            margin: 0 auto;
        }}

        .footer-links {{
            display: flex;
            gap: 2rem;
            justify-content: center;
            margin-bottom: 2rem;
        }}

        .footer-links a {{
            color: var(--white);
            text-decoration: none;
            opacity: 0.8;
            transition: opacity 0.3s;
        }}

        .footer-links a:hover {{
            opacity: 1;
        }}

        /* Responsive */
        @media (max-width: 768px) {{
            .hero h1 {{
                font-size: 2rem;
            }}
            
            .search-form {{
                flex-direction: column;
                border-radius: 12px;
            }}
            
            .search-button {{
                width: 100%;
            }}
            
            .contracts-grid {{
                grid-template-columns: 1fr;
            }}
            
            nav {{
                flex-direction: column;
                gap: 1rem;
            }}
            
            .nav-links {{
                flex-wrap: wrap;
                justify-content: center;
            }}
        }}

        /* Animations */
        @keyframes fadeIn {{
            from {{
                opacity: 0;
                transform: translateY(20px);
            }}
            to {{
                opacity: 1;
                transform: translateY(0);
            }}
        }}

        .fade-in {{
            animation: fadeIn 0.6s ease-out;
        }}

        /* Empty State */
        .empty-state {{
            text-align: center;
            padding: 4rem 2rem;
            color: var(--gray);
        }}

        .empty-state i {{
            font-size: 4rem;
            margin-bottom: 1rem;
            opacity: 0.5;
        }}
    </style>
</head>
<body>
    <header>
        <nav>
            <a href=""/"" class=""logo"">R3E WebGUI</a>
            <div class=""nav-links"">
                <a href=""/"">Home</a>
                <a href=""/contracts"">All Contracts</a>
                <a href=""/api/docs"">API Docs</a>
                <a href=""https://github.com/r3e-network"" target=""_blank"">GitHub</a>
            </div>
        </nav>
    </header>

    <section class=""hero"">
        <h1>Neo N3 Contract WebGUI Platform</h1>
        <p>Discover and interact with smart contracts on Neo blockchain</p>
        
        <div class=""search-container"">
            <form action=""/search"" method=""get"" class=""search-form"">
                <input type=""text"" name=""q"" placeholder=""Search by contract name, address, or description..."" class=""search-input"" required>
                <select name=""network"" class=""search-select"">
                    <option value=""all"">All Networks</option>
                    <option value=""testnet"">Testnet</option>
                    <option value=""mainnet"">Mainnet</option>
                </select>
                <button type=""submit"" class=""search-button"">
                    <i class=""ri-search-line""></i> Search
                </button>
            </form>
        </div>
    </section>

    <section class=""stats"">
        <div class=""stats-grid"">
            <div class=""stat-card fade-in"">
                <div class=""stat-number"">{stats["TotalContracts"]}</div>
                <div class=""stat-label"">Total Contracts</div>
            </div>
            <div class=""stat-card fade-in"">
                <div class=""stat-number"">{stats["TestnetContracts"]}</div>
                <div class=""stat-label"">Testnet Contracts</div>
            </div>
            <div class=""stat-card fade-in"">
                <div class=""stat-number"">{stats["MainnetContracts"]}</div>
                <div class=""stat-label"">Mainnet Contracts</div>
            </div>
            <div class=""stat-card fade-in"">
                <div class=""stat-number"">{stats["TotalTransactions"]}</div>
                <div class=""stat-label"">Tracked Transactions</div>
            </div>
        </div>
    </section>

    {(featured.Any() ? $@"
    <section class=""section"">
        <div class=""container"">
            <div class=""section-header"">
                <h2 class=""section-title"">Featured Contracts</h2>
                <p class=""section-subtitle"">Popular and well-documented smart contracts</p>
            </div>
            <div class=""contracts-grid"">
                {string.Join("", featured.Select(c => GenerateContractCard(c, baseDomain)))}
            </div>
        </div>
    </section>
    " : "")}

    <section class=""section"" style=""background: var(--white);"">
        <div class=""container"">
            <div class=""section-header"">
                <h2 class=""section-title"">Recent Contracts</h2>
                <p class=""section-subtitle"">Newly deployed contract interfaces</p>
            </div>
            {(recent.Any() ? $@"
            <div class=""contracts-grid"">
                {string.Join("", recent.Select(c => GenerateContractCard(c, baseDomain)))}
            </div>
            <div style=""text-align: center; margin-top: 3rem;"">
                <a href=""/contracts"" class=""btn btn-primary"">View All Contracts</a>
            </div>
            " : @"
            <div class=""empty-state"">
                <i class=""ri-folder-open-line""></i>
                <h3>No contracts deployed yet</h3>
                <p>Be the first to deploy a contract WebGUI!</p>
            </div>
            ")}
        </div>
    </section>

    <section class=""cta"">
        <h2>Ready to Deploy Your Contract Interface?</h2>
        <p>Join the Neo ecosystem and make your smart contracts accessible to everyone</p>
        <div class=""cta-buttons"">
            <a href=""/api/docs"" class=""btn btn-primary"">View API Documentation</a>
            <a href=""https://github.com/r3e-network"" class=""btn btn-secondary"" target=""_blank"">
                <i class=""ri-github-fill""></i> View on GitHub
            </a>
        </div>
    </section>

    <footer>
        <div class=""footer-content"">
            <div class=""footer-links"">
                <a href=""/"">Home</a>
                <a href=""/contracts"">Contracts</a>
                <a href=""/api/docs"">API</a>
                <a href=""https://neo.org"" target=""_blank"">Neo.org</a>
            </div>
            <p>&copy; 2024 R3E WebGUI Service. Built with ❤️ for the Neo community.</p>
        </div>
    </footer>

    <script>
        // Add fade-in animation on scroll
        const observerOptions = {{
            threshold: 0.1,
            rootMargin: '0px 0px -100px 0px'
        }};

        const observer = new IntersectionObserver((entries) => {{
            entries.forEach(entry => {{
                if (entry.isIntersecting) {{
                    entry.target.style.opacity = '1';
                    entry.target.style.transform = 'translateY(0)';
                }}
            }});
        }}, observerOptions);

        document.querySelectorAll('.contract-card, .stat-card').forEach(el => {{
            el.style.opacity = '0';
            el.style.transform = 'translateY(20px)';
            el.style.transition = 'opacity 0.6s ease-out, transform 0.6s ease-out';
            observer.observe(el);
        }});
    </script>
</body>
</html>";
    }

    private string GenerateContractCard(ContractWebGUI contract, string baseDomain)
    {
        // Use a direct link to view the contract through a viewer page
        var viewUrl = $"/contract/{contract.Id}";
        var subdomainUrl = $"http://{contract.Subdomain}.{baseDomain}:8888";
        var truncatedAddress = $"{contract.ContractAddress.Substring(0, 10)}...{contract.ContractAddress.Substring(contract.ContractAddress.Length - 8)}";
        var description = string.IsNullOrWhiteSpace(contract.Description) 
            ? "No description available" 
            : (contract.Description.Length > 100 ? contract.Description.Substring(0, 100) + "..." : contract.Description);
        
        // Check if this is a JSON-based contract
        var isJsonBased = contract.Metadata != null && 
                         contract.Metadata.ContainsKey("configBased") && 
                         contract.Metadata["configBased"]?.ToString()?.ToLower() == "true";
        
        return $@"
        <div class=""contract-card"" onclick=""window.location.href='{viewUrl}'"" style=""cursor: pointer;"">
            <div class=""contract-header"">
                <div>
                    <h3 class=""contract-name"">
                        {contract.ContractName}
                        {(isJsonBased ? @" <span style=""color: var(--neo-green); font-size: 0.8rem; margin-left: 0.5rem;"" title=""Modern JSON-based WebGUI"">✨</span>" : "")}
                    </h3>
                    <div class=""contract-address"" title=""{contract.ContractAddress}"">{truncatedAddress}</div>
                </div>
                <span class=""contract-network {contract.Network.ToLower()}"">{contract.Network}</span>
            </div>
            <p class=""contract-description"">{description}</p>
            <div class=""contract-footer"">
                <span class=""contract-date"" title=""{contract.DeployedAt:yyyy-MM-dd HH:mm:ss}"">{GetRelativeTime(contract.DeployedAt)}</span>
                <a href=""{viewUrl}"" class=""contract-link"" onclick=""event.stopPropagation();"">
                    View Interface <i class=""ri-external-link-line""></i>
                </a>
            </div>
        </div>";
    }

    private string GenerateSearchResultsPage(string query, string network, List<ContractWebGUI> results)
    {
        var baseDomain = _configuration["R3EWebGUI:BaseDomain"] ?? "localhost";
        
        return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Search Results - R3E WebGUI Service</title>
    <link href=""https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap"" rel=""stylesheet"">
    <link href=""https://cdn.jsdelivr.net/npm/remixicon@3.5.0/fonts/remixicon.css"" rel=""stylesheet"">
    <style>
        /* Include all the styles from the homepage */
        {GetCommonStyles()}
        
        /* Search Results Specific */
        .search-header {{
            background: linear-gradient(135deg, var(--neo-blue) 0%, var(--neo-purple) 100%);
            color: var(--white);
            padding: 3rem 2rem;
            text-align: center;
        }}
        
        .search-info {{
            max-width: 800px;
            margin: 0 auto;
        }}
        
        .search-info h1 {{
            font-size: 2rem;
            margin-bottom: 1rem;
        }}
        
        .search-query {{
            font-size: 1.2rem;
            opacity: 0.9;
        }}
        
        .results-count {{
            background: rgba(255, 255, 255, 0.2);
            padding: 0.5rem 1rem;
            border-radius: 20px;
            display: inline-block;
            margin-top: 1rem;
        }}
    </style>
</head>
<body>
    <header>
        <nav>
            <a href=""/"" class=""logo"">R3E WebGUI</a>
            <div class=""nav-links"">
                <a href=""/"">Home</a>
                <a href=""/contracts"">All Contracts</a>
                <a href=""/api/docs"">API Docs</a>
                <a href=""https://github.com/r3e-network"" target=""_blank"">GitHub</a>
            </div>
        </nav>
    </header>

    <section class=""search-header"">
        <div class=""search-info"">
            <h1>Search Results</h1>
            <p class=""search-query"">Searching for: <strong>""{query}""</strong></p>
            {(network != "all" ? $@"<p>Network: <strong>{network}</strong></p>" : "")}
            <div class=""results-count"">{results.Count} {(results.Count == 1 ? "contract" : "contracts")} found</div>
        </div>
        
        <div class=""search-container"" style=""margin-top: 2rem;"">
            <form action=""/search"" method=""get"" class=""search-form"">
                <input type=""text"" name=""q"" value=""{query}"" placeholder=""Search contracts..."" class=""search-input"" required>
                <select name=""network"" class=""search-select"">
                    <option value=""all"" {(network == "all" ? "selected" : "")}>All Networks</option>
                    <option value=""testnet"" {(network == "testnet" ? "selected" : "")}>Testnet</option>
                    <option value=""mainnet"" {(network == "mainnet" ? "selected" : "")}>Mainnet</option>
                </select>
                <button type=""submit"" class=""search-button"">
                    <i class=""ri-search-line""></i> Search
                </button>
            </form>
        </div>
    </section>

    <section class=""section"">
        <div class=""container"">
            {(results.Any() ? $@"
            <div class=""contracts-grid"">
                {string.Join("", results.Select(c => GenerateContractCard(c, baseDomain)))}
            </div>
            " : $@"
            <div class=""empty-state"">
                <i class=""ri-search-eye-line""></i>
                <h3>No contracts found</h3>
                <p>Try searching with different keywords or check your spelling</p>
                <a href=""/"" class=""btn btn-primary"" style=""margin-top: 2rem;"">Back to Home</a>
            </div>
            ")}
        </div>
    </section>

    <footer>
        <div class=""footer-content"">
            <div class=""footer-links"">
                <a href=""/"">Home</a>
                <a href=""/contracts"">Contracts</a>
                <a href=""/api/docs"">API</a>
                <a href=""https://neo.org"" target=""_blank"">Neo.org</a>
            </div>
            <p>&copy; 2024 R3E WebGUI Service. Built with ❤️ for the Neo community.</p>
        </div>
    </footer>
</body>
</html>";
    }

    private string GenerateContractsListPage(List<ContractWebGUI> contracts, int page, int pageSize, int totalCount, string network)
    {
        var baseDomain = _configuration["R3EWebGUI:BaseDomain"] ?? "localhost";
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        
        return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>All Contracts - R3E WebGUI Service</title>
    <link href=""https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap"" rel=""stylesheet"">
    <link href=""https://cdn.jsdelivr.net/npm/remixicon@3.5.0/fonts/remixicon.css"" rel=""stylesheet"">
    <style>
        {GetCommonStyles()}
        
        /* Pagination */
        .pagination {{
            display: flex;
            justify-content: center;
            align-items: center;
            gap: 0.5rem;
            margin-top: 3rem;
        }}
        
        .pagination a,
        .pagination span {{
            padding: 0.5rem 1rem;
            border-radius: 8px;
            text-decoration: none;
            color: var(--dark);
            background: var(--white);
            border: 1px solid #ddd;
            transition: all 0.3s;
        }}
        
        .pagination a:hover {{
            background: var(--neo-blue);
            color: var(--white);
            border-color: var(--neo-blue);
        }}
        
        .pagination .current {{
            background: var(--neo-blue);
            color: var(--white);
            border-color: var(--neo-blue);
        }}
        
        .pagination .disabled {{
            opacity: 0.5;
            cursor: not-allowed;
        }}
        
        /* Filter Tabs */
        .filter-tabs {{
            display: flex;
            justify-content: center;
            gap: 1rem;
            margin-bottom: 3rem;
        }}
        
        .filter-tab {{
            padding: 0.75rem 1.5rem;
            background: var(--white);
            border: 2px solid transparent;
            border-radius: 50px;
            text-decoration: none;
            color: var(--dark);
            font-weight: 500;
            transition: all 0.3s;
        }}
        
        .filter-tab:hover {{
            border-color: var(--neo-blue);
            color: var(--neo-blue);
        }}
        
        .filter-tab.active {{
            background: var(--neo-blue);
            color: var(--white);
        }}
    </style>
</head>
<body>
    <header>
        <nav>
            <a href=""/"" class=""logo"">R3E WebGUI</a>
            <div class=""nav-links"">
                <a href=""/"">Home</a>
                <a href=""/contracts"" class=""active"">All Contracts</a>
                <a href=""/api/docs"">API Docs</a>
                <a href=""https://github.com/r3e-network"" target=""_blank"">GitHub</a>
            </div>
        </nav>
    </header>

    <section class=""hero"" style=""padding: 3rem 2rem;"">
        <h1>All Contracts</h1>
        <p>Browse all deployed contract interfaces</p>
    </section>

    <section class=""section"">
        <div class=""container"">
            <div class=""filter-tabs"">
                <a href=""/contracts?network=all"" class=""filter-tab {(network == "all" ? "active" : "")}"">All Networks ({totalCount})</a>
                <a href=""/contracts?network=testnet"" class=""filter-tab {(network == "testnet" ? "active" : "")}"">Testnet</a>
                <a href=""/contracts?network=mainnet"" class=""filter-tab {(network == "mainnet" ? "active" : "")}"">Mainnet</a>
            </div>
            
            {(contracts.Any() ? $@"
            <div class=""contracts-grid"">
                {string.Join("", contracts.Select(c => GenerateContractCard(c, baseDomain)))}
            </div>
            
            {(totalPages > 1 ? $@"
            <div class=""pagination"">
                {(page > 1 ? $@"<a href=""/contracts?page={page - 1}&network={network}""><i class=""ri-arrow-left-line""></i> Previous</a>" : @"<span class=""disabled""><i class=""ri-arrow-left-line""></i> Previous</span>")}
                
                {string.Join("", Enumerable.Range(1, totalPages).Select(p => 
                    p == page ? $@"<span class=""current"">{p}</span>" : $@"<a href=""/contracts?page={p}&network={network}"">{p}</a>"
                ))}
                
                {(page < totalPages ? $@"<a href=""/contracts?page={page + 1}&network={network}"">Next <i class=""ri-arrow-right-line""></i></a>" : @"<span class=""disabled"">Next <i class=""ri-arrow-right-line""></i></span>")}
            </div>
            " : "")}
            " : @"
            <div class=""empty-state"">
                <i class=""ri-folder-open-line""></i>
                <h3>No contracts found</h3>
                <p>No contracts have been deployed yet in this network</p>
            </div>
            ")}
        </div>
    </section>

    <footer>
        <div class=""footer-content"">
            <div class=""footer-links"">
                <a href=""/"">Home</a>
                <a href=""/contracts"">Contracts</a>
                <a href=""/api/docs"">API</a>
                <a href=""https://neo.org"" target=""_blank"">Neo.org</a>
            </div>
            <p>&copy; 2024 R3E WebGUI Service. Built with ❤️ for the Neo community.</p>
        </div>
    </footer>
</body>
</html>";
    }

    private string GetCommonStyles()
    {
        return @"
        :root {
            --neo-green: #00d4aa;
            --neo-blue: #667eea;
            --neo-purple: #764ba2;
            --dark: #1a1a1a;
            --light: #f8f9fa;
            --gray: #6c757d;
            --white: #ffffff;
            --shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            --shadow-lg: 0 10px 25px rgba(0, 0, 0, 0.15);
        }

        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
            background: var(--light);
            color: var(--dark);
            line-height: 1.6;
        }

        /* Copy all styles from GenerateHomePage */
        header {
            background: var(--white);
            box-shadow: var(--shadow);
            position: sticky;
            top: 0;
            z-index: 1000;
        }

        nav {
            max-width: 1200px;
            margin: 0 auto;
            padding: 1rem 2rem;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .logo {
            font-size: 1.5rem;
            font-weight: 700;
            background: linear-gradient(135deg, var(--neo-blue), var(--neo-purple));
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            text-decoration: none;
        }

        .nav-links {
            display: flex;
            gap: 2rem;
            align-items: center;
        }

        .nav-links a {
            color: var(--dark);
            text-decoration: none;
            font-weight: 500;
            transition: color 0.3s;
        }

        .nav-links a:hover,
        .nav-links a.active {
            color: var(--neo-blue);
        }

        .hero {
            background: linear-gradient(135deg, var(--neo-blue) 0%, var(--neo-purple) 100%);
            color: var(--white);
            padding: 5rem 2rem;
            text-align: center;
        }

        .hero h1 {
            font-size: 3rem;
            font-weight: 700;
            margin-bottom: 1rem;
        }

        .hero p {
            font-size: 1.25rem;
            margin-bottom: 2rem;
            opacity: 0.9;
        }

        .search-container {
            max-width: 600px;
            margin: 0 auto;
            position: relative;
        }

        .search-form {
            display: flex;
            gap: 1rem;
            background: var(--white);
            padding: 0.5rem;
            border-radius: 50px;
            box-shadow: var(--shadow-lg);
        }

        .search-input {
            flex: 1;
            padding: 1rem 1.5rem;
            border: none;
            outline: none;
            font-size: 1rem;
            background: transparent;
        }

        .search-select {
            padding: 0.5rem 1rem;
            border: none;
            outline: none;
            background: transparent;
            font-size: 1rem;
            cursor: pointer;
        }

        .search-button {
            padding: 1rem 2rem;
            background: var(--neo-green);
            color: var(--white);
            border: none;
            border-radius: 50px;
            font-weight: 600;
            cursor: pointer;
            transition: transform 0.2s, box-shadow 0.2s;
        }

        .search-button:hover {
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(0, 212, 170, 0.3);
        }

        .section {
            padding: 4rem 2rem;
        }

        .container {
            max-width: 1200px;
            margin: 0 auto;
        }

        .contracts-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
            gap: 2rem;
        }

        .contract-card {
            background: var(--white);
            border-radius: 12px;
            padding: 2rem;
            box-shadow: var(--shadow);
            transition: transform 0.3s, box-shadow 0.3s;
            position: relative;
            overflow: hidden;
        }

        .contract-card::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 4px;
            background: linear-gradient(90deg, var(--neo-green), var(--neo-blue));
        }

        .contract-card:hover {
            transform: translateY(-5px);
            box-shadow: var(--shadow-lg);
        }

        .contract-header {
            display: flex;
            justify-content: space-between;
            align-items: start;
            margin-bottom: 1rem;
        }

        .contract-name {
            font-size: 1.25rem;
            font-weight: 600;
            color: var(--dark);
            margin-bottom: 0.5rem;
        }

        .contract-network {
            display: inline-block;
            padding: 0.25rem 0.75rem;
            background: var(--neo-green);
            color: var(--white);
            border-radius: 20px;
            font-size: 0.8rem;
            font-weight: 600;
            text-transform: uppercase;
        }

        .contract-network.mainnet {
            background: var(--neo-blue);
        }

        .contract-address {
            font-family: monospace;
            font-size: 0.9rem;
            color: var(--gray);
            margin-bottom: 1rem;
            word-break: break-all;
        }

        .contract-description {
            color: var(--dark);
            margin-bottom: 1.5rem;
            line-height: 1.6;
        }

        .contract-footer {
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .contract-date {
            color: var(--gray);
            font-size: 0.9rem;
        }

        .contract-link {
            display: inline-flex;
            align-items: center;
            gap: 0.5rem;
            color: var(--neo-blue);
            text-decoration: none;
            font-weight: 500;
            transition: color 0.3s;
        }

        .contract-link:hover {
            color: var(--neo-purple);
        }

        footer {
            background: var(--dark);
            color: var(--white);
            padding: 3rem 2rem;
            text-align: center;
        }

        .footer-content {
            max-width: 1200px;
            margin: 0 auto;
        }

        .footer-links {
            display: flex;
            gap: 2rem;
            justify-content: center;
            margin-bottom: 2rem;
        }

        .footer-links a {
            color: var(--white);
            text-decoration: none;
            opacity: 0.8;
            transition: opacity 0.3s;
        }

        .footer-links a:hover {
            opacity: 1;
        }

        .btn {
            padding: 1rem 2rem;
            border-radius: 50px;
            text-decoration: none;
            font-weight: 600;
            transition: transform 0.2s, box-shadow 0.2s;
            display: inline-block;
        }

        .btn-primary {
            background: var(--neo-green);
            color: var(--white);
        }

        .btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.2);
        }

        .empty-state {
            text-align: center;
            padding: 4rem 2rem;
            color: var(--gray);
        }

        .empty-state i {
            font-size: 4rem;
            margin-bottom: 1rem;
            opacity: 0.5;
        }

        @media (max-width: 768px) {
            .hero h1 {
                font-size: 2rem;
            }
            
            .search-form {
                flex-direction: column;
                border-radius: 12px;
            }
            
            .search-button {
                width: 100%;
            }
            
            .contracts-grid {
                grid-template-columns: 1fr;
            }
            
            nav {
                flex-direction: column;
                gap: 1rem;
            }
            
            .nav-links {
                flex-wrap: wrap;
                justify-content: center;
            }
        }";
    }

    private string GenerateContractViewerPage(ContractWebGUI contract)
    {
        var baseDomain = _configuration["R3EWebGUI:BaseDomain"] ?? "localhost";
        var subdomainUrl = $"http://{contract.Subdomain}.{baseDomain}:8888";
        
        return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{contract.ContractName} - R3E WebGUI Service</title>
    <link href=""https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap"" rel=""stylesheet"">
    <link href=""https://cdn.jsdelivr.net/npm/remixicon@3.5.0/fonts/remixicon.css"" rel=""stylesheet"">
    <style>
        {GetCommonStyles()}
        
        .contract-viewer {{
            padding: 2rem;
            min-height: 100vh;
            background: var(--light);
        }}
        
        .viewer-header {{
            max-width: 1200px;
            margin: 0 auto 2rem;
            background: var(--white);
            border-radius: 12px;
            padding: 2rem;
            box-shadow: var(--shadow);
        }}
        
        .viewer-content {{
            max-width: 1200px;
            margin: 0 auto;
            background: var(--white);
            border-radius: 12px;
            box-shadow: var(--shadow);
            overflow: hidden;
        }}
        
        .iframe-container {{
            width: 100%;
            height: 80vh;
            border: none;
            position: relative;
        }}
        
        .iframe-container iframe {{
            width: 100%;
            height: 100%;
            border: none;
        }}
        
        .contract-meta {{
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 1rem;
            margin-bottom: 2rem;
        }}
        
        .meta-item {{
            background: var(--light);
            padding: 1rem;
            border-radius: 8px;
        }}
        
        .meta-label {{
            font-size: 0.9rem;
            color: var(--gray);
            margin-bottom: 0.5rem;
            font-weight: 500;
        }}
        
        .meta-value {{
            font-weight: 600;
            color: var(--dark);
        }}
        
        .address-value {{
            font-family: monospace;
            font-size: 0.9rem;
            word-break: break-all;
        }}
        
        .copy-button {{
            background: var(--neo-blue);
            color: var(--white);
            border: none;
            padding: 0.25rem 0.5rem;
            border-radius: 4px;
            cursor: pointer;
            font-size: 0.8rem;
            margin-left: 0.5rem;
            transition: background 0.3s;
        }}
        
        .copy-button:hover {{
            background: var(--neo-purple);
        }}
        
        .action-buttons {{
            display: flex;
            gap: 1rem;
            flex-wrap: wrap;
        }}
        
        .btn-link {{
            display: inline-flex;
            align-items: center;
            gap: 0.5rem;
            padding: 0.75rem 1.5rem;
            border-radius: 8px;
            text-decoration: none;
            font-weight: 500;
            transition: all 0.3s;
        }}
        
        .btn-primary {{
            background: var(--neo-blue);
            color: var(--white);
        }}
        
        .btn-primary:hover {{
            background: var(--neo-purple);
            transform: translateY(-2px);
        }}
        
        .btn-secondary {{
            background: var(--light);
            color: var(--dark);
            border: 1px solid #ddd;
        }}
        
        .btn-secondary:hover {{
            background: var(--neo-green);
            color: var(--white);
            border-color: var(--neo-green);
        }}
        
        .loading-overlay {{
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(255, 255, 255, 0.9);
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.2rem;
            color: var(--gray);
        }}
        
        .spinner {{
            display: inline-block;
            width: 20px;
            height: 20px;
            border: 2px solid var(--light);
            border-radius: 50%;
            border-top-color: var(--neo-blue);
            animation: spin 1s ease-in-out infinite;
            margin-right: 0.5rem;
        }}
        
        @keyframes spin {{
            to {{ transform: rotate(360deg); }}
        }}
    </style>
</head>
<body>
    <header>
        <nav>
            <a href=""/"" class=""logo"">R3E WebGUI</a>
            <div class=""nav-links"">
                <a href=""/"">Home</a>
                <a href=""/contracts"">All Contracts</a>
                <a href=""/api/docs"">API Docs</a>
                <a href=""https://github.com/r3e-network"" target=""_blank"">GitHub</a>
            </div>
        </nav>
    </header>

    <div class=""contract-viewer"">
        <div class=""viewer-header"">
            <h1>{contract.ContractName}</h1>
            <p style=""color: var(--gray); margin-bottom: 2rem;"">{(string.IsNullOrWhiteSpace(contract.Description) ? "No description available" : contract.Description)}</p>
            
            <div class=""contract-meta"">
                <div class=""meta-item"">
                    <div class=""meta-label"">Contract Address</div>
                    <div class=""meta-value address-value"">
                        {contract.ContractAddress}
                        <button class=""copy-button"" onclick=""copyToClipboard('{contract.ContractAddress}')"">
                            <i class=""ri-file-copy-line""></i>
                        </button>
                    </div>
                </div>
                <div class=""meta-item"">
                    <div class=""meta-label"">Network</div>
                    <div class=""meta-value"">
                        <span class=""contract-network {contract.Network.ToLower()}"">{contract.Network}</span>
                    </div>
                </div>
                <div class=""meta-item"">
                    <div class=""meta-label"">Subdomain</div>
                    <div class=""meta-value"">{contract.Subdomain}</div>
                </div>
                <div class=""meta-item"">
                    <div class=""meta-label"">Deployed</div>
                    <div class=""meta-value"">{GetRelativeTime(contract.DeployedAt)}</div>
                </div>
            </div>
            
            <div class=""action-buttons"">
                <a href=""{subdomainUrl}"" target=""_blank"" class=""btn-link btn-primary"">
                    <i class=""ri-external-link-line""></i>
                    Open in New Tab
                </a>
                <a href=""/"" class=""btn-link btn-secondary"">
                    <i class=""ri-arrow-left-line""></i>
                    Back to Home
                </a>
                <a href=""/search?q={Uri.EscapeDataString(contract.ContractName)}"" class=""btn-link btn-secondary"">
                    <i class=""ri-search-line""></i>
                    Find Similar
                </a>
            </div>
        </div>
        
        <div class=""viewer-content"">
            <div class=""iframe-container"">
                <div class=""loading-overlay"" id=""loading"">
                    <div class=""spinner""></div>
                    Loading contract interface...
                </div>
                <iframe src=""{subdomainUrl}"" onload=""hideLoading()"" onerror=""showError()""></iframe>
            </div>
        </div>
    </div>

    <script>
        function hideLoading() {{
            const loading = document.getElementById('loading');
            if (loading) {{
                loading.style.display = 'none';
            }}
        }}
        
        function showError() {{
            const loading = document.getElementById('loading');
            if (loading) {{
                loading.innerHTML = '<i class=""ri-error-warning-line""></i> Failed to load contract interface';
                loading.style.background = 'rgba(255, 0, 0, 0.1)';
                loading.style.color = '#d32f2f';
            }}
        }}
        
        function copyToClipboard(text) {{
            navigator.clipboard.writeText(text).then(() => {{
                // Show success feedback
                const button = event.target.closest('.copy-button');
                const originalHTML = button.innerHTML;
                button.innerHTML = '<i class=""ri-check-line""></i>';
                button.style.background = 'var(--neo-green)';
                setTimeout(() => {{
                    button.innerHTML = originalHTML;
                    button.style.background = 'var(--neo-blue)';
                }}, 1000);
            }}).catch(() => {{
                alert('Failed to copy to clipboard');
            }});
        }}
        
        // Handle iframe loading timeout
        setTimeout(() => {{
            const loading = document.getElementById('loading');
            if (loading && loading.style.display !== 'none') {{
                showError();
            }}
        }}, 10000); // 10 second timeout
    </script>
</body>
</html>";
    }

    private string GetRelativeTime(DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;

        if (timeSpan.TotalMinutes < 1) return "just now";
        if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes} minute{((int)timeSpan.TotalMinutes != 1 ? "s" : "")} ago";
        if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours != 1 ? "s" : "")} ago";
        if (timeSpan.TotalDays < 7) return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays != 1 ? "s" : "")} ago";
        if (timeSpan.TotalDays < 30) return $"{(int)(timeSpan.TotalDays / 7)} week{((int)(timeSpan.TotalDays / 7) != 1 ? "s" : "")} ago";
        if (timeSpan.TotalDays < 365) return $"{(int)(timeSpan.TotalDays / 30)} month{((int)(timeSpan.TotalDays / 30) != 1 ? "s" : "")} ago";
        
        return dateTime.ToString("MMM dd, yyyy");
    }
}