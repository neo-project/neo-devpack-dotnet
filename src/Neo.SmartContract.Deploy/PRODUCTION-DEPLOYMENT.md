# Production Deployment Guide for Neo Smart Contracts

This guide provides best practices and recommendations for deploying Neo smart contracts to production (MainNet) using the deployment toolkit.

## 🔒 Security Configuration

### 1. Secure Credential Management

**Never store passwords in configuration files or source code!**

#### Environment Variables (Recommended)
```bash
# Set wallet password securely
export NEO_WALLET_PASSWORD="your-secure-password"
export NEO_WALLET_PATH="/secure/path/to/wallet.json"

# For specific wallets
export NEO_WALLET_PASSWORD_MAINNET="mainnet-password"
export NEO_WALLET_PASSWORD_TESTNET="testnet-password"
```

#### Using the Secure Credential Provider
```csharp
// Register secure credential provider in your services
services.AddSingleton<ICredentialProvider, SecureCredentialProvider>();

// Or use environment-only provider
services.AddSingleton<ICredentialProvider, EnvironmentCredentialProvider>();
```

#### Azure Key Vault Integration (Production Recommended)
```csharp
// In Program.cs
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());

// Configure credential provider to use Key Vault
services.AddSingleton<ICredentialProvider>(sp => 
    new KeyVaultCredentialProvider(
        sp.GetRequiredService<ILogger<KeyVaultCredentialProvider>>(),
        sp.GetRequiredService<IConfiguration>()
    ));
```

### 2. Wallet Security

- **Use separate wallets** for each environment (dev, test, staging, production)
- **Hardware wallets** recommended for MainNet deployments
- **Multi-signature wallets** for critical contracts
- **Minimal permissions** - deployment wallet should only have deployment rights

## 🚀 Production Deployment Process

### 1. Pre-Deployment Checklist

```csharp
// Always run dry-run first
var deploymentOptions = new DeploymentOptions
{
    DryRun = true,
    VerifyAfterDeploy = true,
    VerificationDelayMs = 10000, // 10 seconds for MainNet
    GasLimit = 100_000_000,
    WaitForConfirmation = true
};

var dryRunResult = await toolkit.DeployAsync("MyContract.csproj", deploymentOptions);
Console.WriteLine($"Dry run successful. Estimated GAS: {dryRunResult.GasConsumed}");
```

### 2. Production Deployment Script

```csharp
using R3E.SmartContract.Deploy;
using R3E.SmartContract.Deploy.Security;

// Production deployment with all safety checks
public static async Task DeployToMainNet()
{
    // 1. Create toolkit with secure configuration
    var toolkit = new DeploymentToolkit()
        .SetNetwork("mainnet");
    
    // 2. Verify network connectivity
    try
    {
        var gasBalance = await toolkit.GetGasBalanceAsync();
        Console.WriteLine($"Deployer GAS balance: {gasBalance}");
        
        if (gasBalance < 500) // Ensure sufficient GAS
        {
            throw new InvalidOperationException("Insufficient GAS for MainNet deployment");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Network connectivity check failed: {ex.Message}");
        return;
    }
    
    // 3. Perform dry-run
    var dryRunOptions = new DeploymentOptions
    {
        DryRun = true,
        GasLimit = 200_000_000 // 2 GAS max for safety
    };
    
    var dryRun = await toolkit.DeployAsync("MyContract.csproj", dryRunOptions);
    Console.WriteLine($"Dry-run successful. Contract hash will be: {dryRun.ContractHash}");
    Console.WriteLine($"Estimated GAS consumption: {dryRun.GasConsumed / 100_000_000m} GAS");
    
    // 4. Confirm deployment
    Console.Write("Proceed with actual deployment? (yes/no): ");
    if (Console.ReadLine()?.ToLower() != "yes")
    {
        Console.WriteLine("Deployment cancelled.");
        return;
    }
    
    // 5. Actual deployment with verification
    var deployOptions = new DeploymentOptions
    {
        DryRun = false,
        VerifyAfterDeploy = true,
        VerificationDelayMs = 15000, // 15 seconds for MainNet
        GasLimit = (long)(dryRun.GasConsumed * 1.2), // 20% buffer
        WaitForConfirmation = true,
        ConfirmationRetries = 60, // 5 minutes max wait
        ConfirmationDelaySeconds = 5
    };
    
    var result = await toolkit.DeployAsync("MyContract.csproj", deployOptions);
    
    if (result.Success && !result.VerificationFailed)
    {
        Console.WriteLine($"✅ Contract deployed successfully!");
        Console.WriteLine($"Contract Hash: {result.ContractHash}");
        Console.WriteLine($"Transaction: {result.TransactionHash}");
        Console.WriteLine($"Block: {result.BlockIndex}");
        Console.WriteLine($"GAS Used: {result.GasConsumed / 100_000_000m} GAS");
        
        // Save deployment record
        await SaveDeploymentRecord(result);
    }
    else
    {
        Console.WriteLine($"❌ Deployment failed or verification failed");
        if (result.VerificationFailed)
        {
            Console.WriteLine("Contract was deployed but verification failed. Manual verification required.");
        }
    }
}
```

### 3. Post-Deployment Verification

```csharp
// Verify contract is operational
public static async Task VerifyDeployment(string contractHash)
{
    var toolkit = new DeploymentToolkit().SetNetwork("mainnet");
    
    // 1. Check contract exists
    if (!await toolkit.ContractExistsAsync(contractHash))
    {
        throw new Exception("Contract not found on blockchain!");
    }
    
    // 2. Test basic functionality (read-only)
    try
    {
        var name = await toolkit.CallAsync<string>(contractHash, "name");
        Console.WriteLine($"Contract name: {name}");
        
        var symbol = await toolkit.CallAsync<string>(contractHash, "symbol");
        Console.WriteLine($"Contract symbol: {symbol}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Contract verification call failed: {ex.Message}");
    }
}
```

## 📊 Monitoring and Logging

### Configure Production Logging

```json
// appsettings.Production.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "R3E.SmartContract.Deploy": "Debug",
      "Microsoft": "Warning"
    }
  },
  "ApplicationInsights": {
    "InstrumentationKey": "your-key"
  }
}
```

### Add Application Insights

```csharp
// In Program.cs
builder.Services.AddApplicationInsightsTelemetry();

// Custom telemetry for deployments
public class DeploymentTelemetry
{
    private readonly TelemetryClient _telemetryClient;
    
    public void TrackDeployment(ContractDeploymentInfo result)
    {
        var telemetry = new EventTelemetry("ContractDeployment");
        telemetry.Properties["ContractName"] = result.ContractName;
        telemetry.Properties["ContractHash"] = result.ContractHash.ToString();
        telemetry.Properties["Network"] = "mainnet";
        telemetry.Properties["Success"] = result.Success.ToString();
        telemetry.Metrics["GasConsumed"] = result.GasConsumed;
        
        _telemetryClient.TrackEvent(telemetry);
    }
}
```

## 🛡️ Security Best Practices

### 1. Environment Isolation

```bash
# Production environment setup
export ASPNETCORE_ENVIRONMENT=Production
export NEO_NETWORK=mainnet
export NEO_WALLET_PATH=/secure/wallets/mainnet.json
export NEO_WALLET_PASSWORD="${MAINNET_WALLET_PASSWORD}"
```

### 2. Network Security

- Use VPN or private endpoints for RPC nodes
- Whitelist deployment machine IPs
- Use HTTPS for all RPC communications
- Consider running your own RPC node

### 3. Deployment Approval Process

```csharp
// Multi-signature deployment approval
public class MultiSigDeploymentService
{
    public async Task<bool> RequestDeploymentApproval(
        string contractName,
        string contractHash,
        long estimatedGas)
    {
        // Send approval request to stakeholders
        var approvalRequest = new DeploymentApprovalRequest
        {
            ContractName = contractName,
            ExpectedHash = contractHash,
            EstimatedGas = estimatedGas,
            RequestedBy = Environment.UserName,
            RequestedAt = DateTime.UtcNow
        };
        
        // Wait for required approvals
        var approvals = await WaitForApprovals(approvalRequest, requiredApprovals: 2);
        
        return approvals.Count >= 2;
    }
}
```

## 🔄 Rollback Strategy

### Enable Rollback Support

```csharp
var deployOptions = new DeploymentOptions
{
    EnableRollback = true,
    // Other options...
};

// Track deployment for potential rollback
var deploymentTracker = new DeploymentTracker();
deploymentTracker.RecordDeployment(result);

// If issues detected, rollback
if (await DetectDeploymentIssues(result.ContractHash))
{
    await deploymentTracker.Rollback(result);
}
```

## 📝 Deployment Record Management

### Save Deployment Records

```csharp
public static async Task SaveDeploymentRecord(ContractDeploymentInfo result)
{
    var record = new
    {
        Network = "mainnet",
        ContractName = result.ContractName,
        ContractHash = result.ContractHash.ToString(),
        TransactionHash = result.TransactionHash.ToString(),
        BlockIndex = result.BlockIndex,
        DeployedAt = result.DeployedAt,
        GasConsumed = result.GasConsumed,
        DeployedBy = Environment.UserName,
        MachineId = Environment.MachineName
    };
    
    // Save to secure storage (not in source control!)
    var recordPath = $"/secure/deployments/mainnet/{result.ContractName}-{result.DeployedAt:yyyyMMdd-HHmmss}.json";
    await File.WriteAllTextAsync(recordPath, JsonSerializer.Serialize(record));
    
    // Also save to database or secure cloud storage
    await SaveToSecureStorage(record);
}
```

## ⚠️ Common Pitfalls to Avoid

1. **Never commit deployment records** to source control
2. **Always verify contract** after deployment
3. **Test on TestNet first** with identical parameters
4. **Monitor gas prices** before deployment
5. **Have rollback plan** ready
6. **Document deployment process** and decisions
7. **Use separate deployment machine** not development machine
8. **Verify contract source** matches audited version

## 🚨 Emergency Procedures

### If Deployment Fails

1. **Don't panic** - Check transaction status
2. **Verify wallet balance** - Ensure sufficient GAS
3. **Check network status** - RPC node might be down
4. **Review error logs** - Check detailed error messages
5. **Contact support** - If using third-party RPC

### If Verification Fails

1. **Wait longer** - MainNet can take time to propagate
2. **Try different RPC nodes** - Node might not be synced
3. **Check block explorer** - Verify independently
4. **Manual verification** - Use neo-cli if needed

## 📞 Support Contacts

- Neo Developer Support: https://neo.org/dev
- Community Discord: https://discord.gg/neo
- Emergency Hotline: [Your organization's contact]

Remember: **Take your time, double-check everything, and never rush a MainNet deployment!**