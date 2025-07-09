# Production Deployment Security Guide

This guide provides comprehensive security best practices for deploying Neo smart contracts in production environments using the Neo Contract Deployment Toolkit.

## Table of Contents

1. [Security Overview](#security-overview)
2. [Credential Management](#credential-management)
3. [Secure Configuration](#secure-configuration)
4. [Health Monitoring](#health-monitoring)
5. [Performance and Metrics](#performance-and-metrics)
6. [Deployment Best Practices](#deployment-best-practices)
7. [Security Checklist](#security-checklist)

## Security Overview

The Neo Contract Deployment Toolkit provides enterprise-grade security features to protect your smart contract deployments:

- **Encrypted credential storage**
- **Environment-based configuration**
- **Health monitoring and diagnostics**
- **Deployment audit trails**
- **Performance metrics collection**
- **Multi-signature support**

## Credential Management

### 1. Secure Credential Storage

Never store private keys or WIF keys in plain text. Use the toolkit's credential providers:

#### Using SecureCredentialProvider

```csharp
// Create an encryption key (store this securely!)
var encryptionKey = new byte[32];
using (var rng = RandomNumberGenerator.Create())
{
    rng.GetBytes(encryptionKey);
}

// Create secure credential provider
var credentialProvider = new SecureCredentialProvider(
    encryptionKey,
    "/secure/path/credentials.enc",
    logger);

// Store credentials securely
await credentialProvider.SetCredentialAsync("deployer_wif", wifKey);

// Configure deployment to use credential provider
services.AddSingleton<ICredentialProvider>(credentialProvider);
```

#### Using EnvironmentCredentialProvider

```csharp
// Set environment variables (e.g., in your CI/CD pipeline)
// NEO_DEPLOY_DEPLOYER_WIF=your_wif_key
// NEO_DEPLOY_RPC_URL=https://your-node.com

// Configure environment-based credentials
var envProvider = new EnvironmentCredentialProvider("NEO_DEPLOY_");
services.AddSingleton<ICredentialProvider>(envProvider);
```

### 2. Key Management Best Practices

- **Use Hardware Security Modules (HSMs)** for production keys when possible
- **Implement key rotation** policies
- **Use separate keys** for different environments (dev, staging, production)
- **Enable multi-signature deployments** for critical contracts
- **Audit key usage** through deployment records

### 3. Password-Derived Encryption

For applications requiring password-based security:

```csharp
// Generate a unique salt for your application
var salt = new byte[32];
using (var rng = RandomNumberGenerator.Create())
{
    rng.GetBytes(salt);
}

// Create provider with password-derived key
var provider = SecureCredentialProvider.CreateWithPassword(
    password: securePassword,
    salt: salt,
    persistencePath: "/secure/path/credentials.enc"
);
```

## Secure Configuration

### 1. Enhanced Deployment Options

Configure deployments with security in mind:

```csharp
var deploymentOptions = new DeploymentOptions
{
    // Security settings
    RequireSecureCredentials = true,
    ValidateRpcCertificate = true,
    EnableMultiSig = true,
    
    // Use credential provider instead of plain text
    CredentialKey = "deployer_wif",
    
    // Performance and monitoring
    EnablePerformanceMonitoring = true,
    EnableMetrics = true,
    RecordDeploymentHistory = true,
    
    // Health checks
    PerformHealthChecks = true,
    AbortOnHealthCheckFailure = true,
    
    // Environment tagging
    Environment = "production",
    DeploymentTags = new Dictionary<string, string>
    {
        ["version"] = "1.0.0",
        ["team"] = "blockchain-team",
        ["purpose"] = "token-contract"
    }
};

// Validate options before deployment
deploymentOptions.Validate();
```

### 2. Multi-Signature Deployments

For critical contracts requiring multiple approvals:

```csharp
deploymentOptions.EnableMultiSig = true;
deploymentOptions.AdditionalSigners = new List<SignerConfiguration>
{
    new SignerConfiguration
    {
        CredentialKey = "signer1_wif",
        Scopes = WitnessScope.CalledByEntry
    },
    new SignerConfiguration
    {
        CredentialKey = "signer2_wif",
        Scopes = WitnessScope.CalledByEntry
    }
};
```

### 3. RPC Security

Ensure secure communication with Neo nodes:

```csharp
// Configure HTTPS with certificate validation
deploymentOptions.ValidateRpcCertificate = true;
deploymentOptions.RpcUrl = "https://secure-node.example.com";

// Custom certificate validation (if needed)
deploymentOptions.RpcCertificateValidationCallback = (sender, certificate, chain, errors) =>
{
    // Implement custom validation logic
    return errors == SslPolicyErrors.None;
};

// Add authentication headers if required
deploymentOptions.CustomRpcHeaders = new Dictionary<string, string>
{
    ["Authorization"] = "Bearer your-api-token"
};
```

## Health Monitoring

### 1. Configure Health Checks

Set up comprehensive health monitoring:

```csharp
// Create health check service
var healthService = new HealthCheckService(serviceProvider, logger);

// Configure default health checks
healthService.ConfigureDefaultHealthChecks(rpcUrl, httpClient);

// Add custom health checks
healthService.AddHealthCheck(new CustomHealthCheck());

// Run health checks before deployment
if (deploymentOptions.PerformHealthChecks)
{
    var healthReport = await healthService.CheckHealthAsync();
    
    if (healthReport.Status == HealthStatus.Unhealthy && 
        deploymentOptions.AbortOnHealthCheckFailure)
    {
        throw new InvalidOperationException(
            $"Health check failed: {healthService.GetHealthSummary(healthReport)}");
    }
}
```

### 2. Continuous Health Monitoring

Implement ongoing health monitoring:

```csharp
// Schedule periodic health checks
var timer = new Timer(async _ =>
{
    var report = await healthService.CheckHealthAsync();
    logger.LogInformation("Health Status: {Status}", report.Status);
    
    if (report.Status != HealthStatus.Healthy)
    {
        // Alert operations team
        await alertService.SendAlert("Deployment service unhealthy", report);
    }
}, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
```

## Performance and Metrics

### 1. Enable Performance Monitoring

Track deployment performance:

```csharp
// Create performance monitor
var perfMonitor = new PerformanceMonitor(logger);

// Track deployment operations
using (var tracker = perfMonitor.StartTracking("deployment", "contract_deploy"))
{
    // Deployment code here
    await deployer.DeployAsync(contract, options);
}

// Get performance report
var perfReport = perfMonitor.GetReport();
logger.LogInformation("Performance Report:\n{Report}", perfReport);
```

### 2. Deployment Metrics Collection

Collect and analyze deployment metrics:

```csharp
// Create metrics collector
var metrics = new DeploymentMetrics();

// Record deployment
var deploymentId = metrics.RecordDeployment(
    contractName: contract.Name,
    contractHash: deploymentInfo.ContractHash.ToString(),
    success: true,
    duration: deploymentDuration
);

// Get statistics
var stats = metrics.GetStatistics();
logger.LogInformation(
    "Deployment Statistics - Total: {Total}, Success Rate: {Rate:P}",
    stats.TotalDeployments,
    stats.DeploymentSuccessRate
);
```

### 3. Deployment History Tracking

Maintain audit trails:

```csharp
// Create deployment record service
var recordService = new DeploymentRecordService(
    "/secure/path/deployment-history.json",
    logger
);

// Record deployment
var recordId = await recordService.RecordDeploymentAsync(
    deploymentInfo,
    deploymentOptions,
    additionalData: new Dictionary<string, object>
    {
        ["approved_by"] = approversList,
        ["deployment_reason"] = "Feature release v1.2.0",
        ["risk_assessment"] = "Low"
    }
);

// Query deployment history
var recentDeployments = recordService.GetRecordsByTimeRange(
    DateTime.UtcNow.AddDays(-7),
    DateTime.UtcNow
);
```

## Deployment Best Practices

### 1. Pre-Deployment Validation

Always validate before deploying:

```csharp
// Validate deployment options
deploymentOptions.Validate();

// Verify contract compatibility
var isCompatible = await deployer.ContractExistsAsync(
    existingContractHash,
    deploymentOptions.RpcUrl
);

// Check account balance
var balance = await walletManager.GetGasBalanceAsync(
    deployerAccount,
    deploymentOptions.RpcUrl
);

if (balance < requiredGas)
{
    throw new InvalidOperationException("Insufficient GAS balance");
}
```

### 2. Deployment Hooks

Implement custom deployment logic:

```csharp
deploymentOptions.Hooks = new DeploymentHooks
{
    PreDeployment = async (options) =>
    {
        // Pre-deployment checks
        await ValidateDeploymentWindow();
        await NotifyStakeholders();
    },
    
    PostDeployment = async (info) =>
    {
        // Post-deployment tasks
        await UpdateInventory(info);
        await TriggerMonitoring(info.ContractHash);
    },
    
    OnFailure = async (exception) =>
    {
        // Failure handling
        await AlertOpsTeam(exception);
        await CreateIncident(exception);
    },
    
    PreTransaction = async (transaction) =>
    {
        // Final transaction validation
        return await ValidateTransaction(transaction);
    }
};
```

### 3. Gradual Rollout

Implement safe deployment strategies:

```csharp
// Deploy to testnet first
var testnetOptions = new DeploymentOptions
{
    RpcUrl = "https://testnet.neo.org",
    Environment = "testnet",
    // ... other options
};

var testnetResult = await toolkit.DeployAsync(contract, testnetOptions);

// Verify on testnet
await VerifyContractFunctionality(testnetResult.ContractHash, testnetOptions.RpcUrl);

// Deploy to mainnet after verification
var mainnetOptions = new DeploymentOptions
{
    RpcUrl = "https://mainnet.neo.org",
    Environment = "mainnet",
    RequireSecureCredentials = true,
    EnableMultiSig = true,
    // ... other options
};

var mainnetResult = await toolkit.DeployAsync(contract, mainnetOptions);
```

## Security Checklist

Before deploying to production, ensure:

### Credential Security
- [ ] Private keys are encrypted and stored securely
- [ ] Using credential providers instead of plain text WIFs
- [ ] Separate keys for different environments
- [ ] Key rotation policy in place
- [ ] Multi-signature enabled for critical contracts

### Network Security
- [ ] Using HTTPS for RPC connections
- [ ] RPC certificate validation enabled
- [ ] Authentication headers configured (if applicable)
- [ ] Firewall rules restrict RPC access

### Monitoring and Auditing
- [ ] Health checks configured and tested
- [ ] Performance monitoring enabled
- [ ] Deployment history tracking enabled
- [ ] Alerts configured for failures
- [ ] Regular security audits scheduled

### Deployment Process
- [ ] Pre-deployment validation implemented
- [ ] Deployment hooks configured
- [ ] Rollback procedure documented
- [ ] Incident response plan in place
- [ ] Change management process followed

### Code Security
- [ ] Smart contract audited
- [ ] Unit tests pass
- [ ] Integration tests pass
- [ ] Security vulnerabilities addressed
- [ ] Gas optimization completed

### Operational Security
- [ ] Access controls implemented
- [ ] Deployment approvals required
- [ ] Audit logs retained
- [ ] Backup and recovery tested
- [ ] Disaster recovery plan documented

## Example: Complete Secure Deployment

Here's a complete example of a production-ready deployment:

```csharp
public async Task<ContractDeploymentInfo> DeployToProduction(
    CompiledContract contract,
    IServiceProvider services)
{
    // 1. Set up secure credential provider
    var credentialProvider = services.GetRequiredService<ICredentialProvider>();
    
    // 2. Configure deployment options
    var options = new DeploymentOptions
    {
        // Security
        RequireSecureCredentials = true,
        CredentialKey = "mainnet_deployer",
        EnableMultiSig = true,
        AdditionalSigners = GetRequiredSigners(),
        ValidateRpcCertificate = true,
        
        // Network
        RpcUrl = configuration["Neo:MainnetRpcUrl"],
        NetworkMagic = 860833102, // Mainnet
        
        // Monitoring
        EnablePerformanceMonitoring = true,
        EnableMetrics = true,
        RecordDeploymentHistory = true,
        
        // Safety
        PerformHealthChecks = true,
        AbortOnHealthCheckFailure = true,
        EnableAutoRetry = true,
        MaxAutoRetries = 3,
        
        // Metadata
        Environment = "production",
        DeploymentTags = new Dictionary<string, string>
        {
            ["version"] = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
            ["git_commit"] = GetGitCommitHash(),
            ["deployed_by"] = Environment.UserName,
            ["deployment_id"] = Guid.NewGuid().ToString()
        },
        
        // Hooks
        Hooks = new DeploymentHooks
        {
            PreDeployment = PreDeploymentChecks,
            PostDeployment = PostDeploymentTasks,
            OnFailure = HandleDeploymentFailure
        }
    };
    
    // 3. Validate options
    options.Validate();
    
    // 4. Run health checks
    var healthService = services.GetRequiredService<HealthCheckService>();
    var healthReport = await healthService.CheckHealthAsync();
    
    if (healthReport.Status != HealthStatus.Healthy)
    {
        throw new InvalidOperationException(
            $"System health check failed: {healthService.GetHealthSummary(healthReport)}");
    }
    
    // 5. Deploy with monitoring
    var toolkit = services.GetRequiredService<NeoContractToolkit>();
    var perfMonitor = services.GetRequiredService<PerformanceMonitor>();
    
    using (var tracker = perfMonitor.StartTracking("deployment", "production_deploy"))
    {
        try
        {
            var result = await toolkit.DeployAsync(contract, options);
            
            // 6. Verify deployment
            if (options.VerifyAfterDeploy)
            {
                await Task.Delay(options.VerificationDelayMs);
                var exists = await toolkit.ContractExistsAsync(
                    result.ContractHash,
                    options.RpcUrl);
                
                if (!exists)
                {
                    throw new InvalidOperationException(
                        "Contract verification failed after deployment");
                }
            }
            
            // 7. Record success
            var metrics = services.GetRequiredService<DeploymentMetrics>();
            metrics.RecordDeployment(
                contract.Name,
                result.ContractHash.ToString(),
                true,
                tracker.Elapsed);
            
            return result;
        }
        catch (Exception ex)
        {
            // Record failure
            var metrics = services.GetRequiredService<DeploymentMetrics>();
            metrics.RecordDeployment(
                contract.Name,
                string.Empty,
                false,
                tracker.Elapsed,
                ex.Message);
            
            throw;
        }
    }
}
```

## Conclusion

By following these security best practices and utilizing the toolkit's security features, you can ensure safe and reliable smart contract deployments in production environments. Remember that security is an ongoing process - regularly review and update your security measures as threats evolve.

For additional support or security concerns, please consult the Neo documentation or contact the Neo development community.