using R3E.WebGUI.Service.Domain.Models;

namespace R3E.WebGUI.Service.Core.Services;

public interface IWebGUIGeneratorService
{
    Task<Dictionary<string, string>> GenerateFromManifestAsync(ContractManifestInfo manifest, string templateType = "standard");
    Task<string> GenerateContractInfoPageAsync(ContractInfo contractInfo);
    Task<string> GenerateMethodInvocationFormsAsync(List<MethodInfo> methods);
    Task<string> GenerateTransactionHistoryPageAsync(string contractAddress);
    Task<string> GenerateAssetBalancePageAsync(string contractAddress);
    Task<List<WebGUITemplate>> GetAvailableTemplatesAsync();
    Task<string> GenerateWalletIntegrationScriptAsync(List<string> supportedWallets);
}

public class WebGUITemplate
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PreviewImage { get; set; } = string.Empty;
    public List<string> SupportedContractTypes { get; set; } = new();
    public Dictionary<string, string> DefaultSettings { get; set; } = new();
}

public class SimpleWebGUIGeneratorService : IWebGUIGeneratorService
{
    private readonly INeoRpcService _neoRpcService;
    private readonly ILogger<SimpleWebGUIGeneratorService> _logger;

    public SimpleWebGUIGeneratorService(INeoRpcService neoRpcService, ILogger<SimpleWebGUIGeneratorService> logger)
    {
        _neoRpcService = neoRpcService;
        _logger = logger;
    }

    public async Task<Dictionary<string, string>> GenerateFromManifestAsync(ContractManifestInfo manifest, string templateType = "standard")
    {
        var files = new Dictionary<string, string>();

        try
        {
            // Generate main index.html
            files["index.html"] = GenerateIndexHtml(manifest);
            
            // Generate CSS
            files["styles.css"] = GenerateBasicCSS();
            
            // Generate JavaScript
            files["contract.js"] = GenerateBasicJavaScript(manifest);
            
            return files;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate WebGUI from manifest for contract {ContractAddress}", manifest.ContractAddress);
            throw;
        }
    }

    private string GenerateIndexHtml(ContractManifestInfo manifest)
    {
        var methodsHtml = string.Join("", manifest.Methods.Select(m => 
            $@"<div class=""method"">
                <h3>{m.Name} ({(m.Safe ? "Read" : "Write")})</h3>
                <form>
                    {string.Join("", m.Parameters.Select(p => 
                        $@"<input type=""text"" placeholder=""{p.Name} ({p.Type})"" name=""{p.Name}"" />"))}
                    <button type=""button"">{(m.Safe ? "Call" : "Invoke")}</button>
                </form>
            </div>"));

        return $@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{manifest.Name} - Neo Smart Contract</title>
    <link rel=""stylesheet"" href=""styles.css"">
</head>
<body>
    <div class=""container"">
        <header>
            <h1>{manifest.Name}</h1>
            <p>{manifest.Description}</p>
            <div class=""contract-info"">
                <strong>Address:</strong> {manifest.ContractAddress}<br>
                <strong>Network:</strong> {manifest.Network}<br>
                <strong>Author:</strong> {manifest.Author}<br>
                <strong>Version:</strong> {manifest.Version}
            </div>
        </header>

        <section class=""wallet-section"">
            <button id=""connect-wallet"">Connect Wallet</button>
            <div id=""wallet-status"">Not connected</div>
        </section>

        <section class=""balances"">
            <h2>Contract Balances</h2>
            <div id=""balances-list"">Loading...</div>
        </section>

        <section class=""methods"">
            <h2>Contract Methods</h2>
            {methodsHtml}
        </section>

        <section class=""transactions"">
            <h2>Recent Transactions</h2>
            <div id=""transactions-list"">Loading...</div>
        </section>
    </div>
    <script src=""contract.js""></script>
</body>
</html>";
    }

    private string GenerateBasicCSS()
    {
        return @"
body {
    font-family: Arial, sans-serif;
    margin: 0;
    padding: 20px;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    min-height: 100vh;
}

.container {
    max-width: 1200px;
    margin: 0 auto;
    background: white;
    padding: 30px;
    border-radius: 12px;
    box-shadow: 0 10px 30px rgba(0,0,0,0.1);
}

header {
    text-align: center;
    margin-bottom: 30px;
    padding-bottom: 20px;
    border-bottom: 2px solid #eee;
}

header h1 {
    color: #667eea;
    margin: 0 0 10px 0;
    font-size: 2.5em;
}

.contract-info {
    background: #f8f9fa;
    padding: 15px;
    border-radius: 8px;
    margin: 15px 0;
    text-align: left;
}

.wallet-section, .balances, .methods, .transactions {
    margin: 30px 0;
    padding: 20px;
    border: 1px solid #ddd;
    border-radius: 8px;
}

.method {
    background: #f8f9fa;
    padding: 20px;
    margin: 15px 0;
    border-radius: 8px;
    border-left: 4px solid #667eea;
}

.method h3 {
    margin: 0 0 15px 0;
    color: #333;
}

.method input {
    width: 200px;
    padding: 8px;
    margin: 5px;
    border: 1px solid #ddd;
    border-radius: 4px;
}

.method button, #connect-wallet {
    background: #667eea;
    color: white;
    border: none;
    padding: 10px 20px;
    border-radius: 4px;
    cursor: pointer;
    margin: 5px;
}

.method button:hover, #connect-wallet:hover {
    background: #5a6fd8;
}

#wallet-status {
    margin: 10px 0;
    padding: 10px;
    background: #e9ecef;
    border-radius: 4px;
}

h2 {
    color: #333;
    border-bottom: 2px solid #667eea;
    padding-bottom: 10px;
}";
    }

    private string GenerateBasicJavaScript(ContractManifestInfo manifest)
    {
        return $@"
// Contract Interface for {manifest.Name}
let wallet = null;
let contractAddress = '{manifest.ContractAddress}';
let network = '{manifest.Network}';

document.addEventListener('DOMContentLoaded', function() {{
    loadContractData();
    setupWalletConnection();
}});

function setupWalletConnection() {{
    const connectBtn = document.getElementById('connect-wallet');
    const walletStatus = document.getElementById('wallet-status');
    
    connectBtn.addEventListener('click', async function() {{
        try {{
            if (window.NEOLine) {{
                const account = await window.NEOLine.getAccount();
                wallet = account;
                walletStatus.textContent = 'Connected: ' + account.address.substring(0, 10) + '...';
                connectBtn.textContent = 'Disconnect';
            }} else {{
                alert('Please install NeoLine wallet extension');
            }}
        }} catch (error) {{
            console.error('Wallet connection failed:', error);
            alert('Failed to connect wallet');
        }}
    }});
}}

async function loadContractData() {{
    try {{
        await loadBalances();
        await loadTransactions();
    }} catch (error) {{
        console.error('Error loading contract data:', error);
    }}
}}

async function loadBalances() {{
    const balancesList = document.getElementById('balances-list');
    try {{
        const response = await fetch(`/api/contract/${{contractAddress}}/balances?network=${{network}}`);
        const balances = await response.json();
        
        if (balances.length === 0) {{
            balancesList.innerHTML = '<p>No balances found</p>';
            return;
        }}

        balancesList.innerHTML = balances.map(balance => 
            `<div class=""balance-item"">
                <strong>${{balance.symbol || 'Unknown'}}</strong>: ${{balance.balance}} 
            </div>`
        ).join('');
    }} catch (error) {{
        balancesList.innerHTML = '<p>Error loading balances</p>';
        console.error('Error loading balances:', error);
    }}
}}

async function loadTransactions() {{
    const transactionsList = document.getElementById('transactions-list');
    try {{
        const response = await fetch(`/api/contract/${{contractAddress}}/transactions?network=${{network}}`);
        const data = await response.json();
        
        if (!data.transactions || data.transactions.length === 0) {{
            transactionsList.innerHTML = '<p>No transactions found</p>';
            return;
        }}

        transactionsList.innerHTML = data.transactions.map(tx => 
            `<div class=""transaction-item"">
                <div><strong>Hash:</strong> ${{tx.hash.substring(0, 20)}}...</div>
                <div><strong>Method:</strong> ${{tx.method || 'Unknown'}}</div>
                <div><strong>Status:</strong> ${{tx.success ? 'Success' : 'Failed'}}</div>
            </div>`
        ).join('');
    }} catch (error) {{
        transactionsList.innerHTML = '<p>Error loading transactions</p>';
        console.error('Error loading transactions:', error);
    }}
}}

function copyToClipboard(text) {{
    navigator.clipboard.writeText(text).then(() => {{
        alert('Copied to clipboard!');
    }});
}}";
    }

    public async Task<string> GenerateContractInfoPageAsync(ContractInfo contractInfo)
    {
        return "Contract info page - Implementation pending";
    }

    public async Task<string> GenerateMethodInvocationFormsAsync(List<MethodInfo> methods)
    {
        return "Method invocation forms - Implementation pending";
    }

    public async Task<string> GenerateTransactionHistoryPageAsync(string contractAddress)
    {
        return "Transaction history page - Implementation pending";
    }

    public async Task<string> GenerateAssetBalancePageAsync(string contractAddress)
    {
        return "Asset balance page - Implementation pending";
    }

    public async Task<List<WebGUITemplate>> GetAvailableTemplatesAsync()
    {
        return new List<WebGUITemplate>
        {
            new WebGUITemplate
            {
                Id = "standard",
                Name = "Standard Contract Interface",
                Description = "Complete interface with all contract features",
                SupportedContractTypes = new List<string> { "general", "nep-17", "nep-11" }
            }
        };
    }

    public async Task<string> GenerateWalletIntegrationScriptAsync(List<string> supportedWallets)
    {
        return "// Wallet integration script - Implementation pending";
    }
}