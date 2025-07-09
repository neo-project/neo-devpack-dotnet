# Neo Contract Deployment Security Features

This document describes the security features available in the Neo Contract Deployment Toolkit.

## Overview

The toolkit provides enterprise-grade security features for managing credentials and securing deployments:

- **Encrypted Credential Storage**: Store private keys and sensitive data with AES-256 encryption
- **Environment-based Credentials**: Use environment variables for CI/CD pipelines
- **Multi-signature Support**: Require multiple approvals for critical deployments
- **Health Monitoring**: Ensure system health before deployments
- **Audit Trails**: Track all deployment activities
- **Performance Monitoring**: Monitor deployment performance and detect anomalies

## Credential Providers

### SecureCredentialProvider

Provides encrypted storage for credentials using AES-256 encryption.

```csharp
// Create with a specific encryption key
var encryptionKey = new byte[32]; // Must be 32 bytes for AES-256
using (var rng = RandomNumberGenerator.Create())
{
    rng.GetBytes(encryptionKey);
}

var provider = new SecureCredentialProvider(
    encryptionKey,
    "./credentials.enc", // Optional persistence path
    logger);

// Store credentials
await provider.SetCredentialAsync("deployer_wif", wifKey);

// Retrieve credentials
var wif = await provider.GetCredentialAsync("deployer_wif");
```

#### Password-Derived Encryption

For applications that need password-based security:

```csharp
// Generate a unique salt for your application
var salt = new byte[32];
using (var rng = RandomNumberGenerator.Create())
{
    rng.GetBytes(salt);
}

// Create provider with password
var provider = SecureCredentialProvider.CreateWithPassword(
    password: "strong_password_from_secure_source",
    salt: salt,
    persistencePath: "./credentials.enc",
    logger);
```

### EnvironmentCredentialProvider

Reads credentials from environment variables, ideal for CI/CD pipelines.

```csharp
// Set environment variables
// NEO_DEPLOY_MAINNET_WIF=your_wif_key
// NEO_DEPLOY_RPC_URL=https://your-node.com

var provider = new EnvironmentCredentialProvider(
    prefix: "NEO_DEPLOY_",
    allowSet: false, // Read-only by default
    logger);

// Retrieve credentials
var wif = await provider.GetCredentialAsync("MAINNET_WIF");
```

### Credential Provider Chain

Use multiple providers with fallback:

```csharp
services.AddCredentialProviderChain(
    primary: s => s.AddSecureCredentialProvider(encryptionKey, "./credentials.enc"),
    fallback: s => s.AddEnvironmentCredentialProvider("NEO_DEPLOY_")
);
```

## Using Credentials in Deployments

Configure deployments to use credential providers:

```csharp
var deploymentOptions = new DeploymentOptions
{
    // Use credential provider instead of plain text WIF
    RequireSecureCredentials = true,
    CredentialKey = "mainnet_deployer",
    
    // Other security settings
    ValidateRpcCertificate = true,
    EnableMultiSig = true,
    
    // ... other options
};
```

## Security Best Practices

### 1. Key Storage

- **Never** commit private keys to source control
- Use Hardware Security Modules (HSMs) for production keys
- Implement key rotation policies
- Use separate keys for different environments

### 2. Encryption Key Management

- Store encryption keys in secure key management services:
  - Azure Key Vault
  - AWS KMS
  - HashiCorp Vault
  - Hardware Security Modules

### 3. Environment Variables

When using environment variables:
- Set them at runtime, not in scripts
- Use CI/CD secret management features
- Rotate credentials regularly
- Audit access to environment variables

### 4. Multi-Signature Deployments

For critical contracts:

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

### 5. Secure RPC Communication

Always use HTTPS for RPC endpoints:

```csharp
deploymentOptions.RpcUrl = "https://secure-node.example.com";
deploymentOptions.ValidateRpcCertificate = true;

// Custom certificate validation if needed
deploymentOptions.RpcCertificateValidationCallback = (sender, cert, chain, errors) =>
{
    // Implement custom validation
    return errors == SslPolicyErrors.None;
};
```

## Monitoring and Auditing

### Health Checks

Ensure system health before deployments:

```csharp
var healthService = serviceProvider.GetRequiredService<HealthCheckService>();
var report = await healthService.CheckHealthAsync();

if (report.Status != HealthStatus.Healthy)
{
    // Abort deployment
    throw new InvalidOperationException("System health check failed");
}
```

### Deployment Records

Track all deployments:

```csharp
var recordService = serviceProvider.GetRequiredService<DeploymentRecordService>();

// Record deployment
await recordService.RecordDeploymentAsync(
    deploymentInfo,
    deploymentOptions,
    additionalData: new Dictionary<string, object>
    {
        ["approved_by"] = approvers,
        ["reason"] = "Feature release v1.2.0"
    });

// Query history
var recentDeployments = recordService.GetRecordsByTimeRange(
    DateTime.UtcNow.AddDays(-30),
    DateTime.UtcNow);
```

### Performance Monitoring

Monitor deployment performance:

```csharp
var perfMonitor = serviceProvider.GetRequiredService<PerformanceMonitor>();

using (var tracker = perfMonitor.StartTracking("deployment", "mainnet_deploy"))
{
    // Deployment code
    await toolkit.DeployAsync(contract, options);
}

// View performance report
var report = perfMonitor.GetReport();
```

## Security Checklist

Before deploying to production:

- [ ] Credentials are encrypted, not in plain text
- [ ] Using credential providers instead of direct WIF keys
- [ ] Encryption keys are stored securely
- [ ] Environment-specific keys are segregated
- [ ] Multi-signature is enabled for critical contracts
- [ ] RPC communication uses HTTPS
- [ ] Certificate validation is enabled
- [ ] Health checks are configured
- [ ] Deployment tracking is enabled
- [ ] Performance monitoring is active
- [ ] Audit logs are being collected
- [ ] Key rotation schedule is defined
- [ ] Incident response plan is documented

## Example: Complete Secure Setup

```csharp
// Configure services
services.AddNeoContractDeploy(configuration)
    .AddSecureCredentialProvider(encryptionKey, "./credentials.enc")
    .AddNeoHealthChecks("https://seed1.neo.org:443")
    .AddDeploymentMetrics(TimeSpan.FromDays(30))
    .AddPerformanceMonitoring()
    .AddDeploymentRecordService("./deployment-history.json");

// Configure deployment
var options = new DeploymentOptions
{
    // Security
    RequireSecureCredentials = true,
    CredentialKey = "mainnet_deployer",
    EnableMultiSig = true,
    ValidateRpcCertificate = true,
    
    // Monitoring
    EnablePerformanceMonitoring = true,
    EnableMetrics = true,
    RecordDeploymentHistory = true,
    
    // Safety
    PerformHealthChecks = true,
    AbortOnHealthCheckFailure = true,
    
    // Environment
    Environment = "production",
    DeploymentTags = new Dictionary<string, string>
    {
        ["version"] = "1.0.0",
        ["approved_by"] = "security_team"
    }
};

// Deploy with full security
var deploymentInfo = await toolkit.DeployAsync(contract, options);
```

## Troubleshooting

### Common Issues

1. **Credential Not Found**
   - Verify the credential key name
   - Check if the credential provider is configured
   - Ensure credentials are set before retrieval

2. **Decryption Failed**
   - Verify the encryption key is correct
   - Check if the persistence file is corrupted
   - Ensure the same encryption key is used

3. **Environment Variable Not Found**
   - Check the environment variable name and prefix
   - Verify the variable is set in the current environment
   - Use correct key normalization (dots/dashes become underscores)

4. **Health Check Failures**
   - Review individual health check results
   - Verify RPC endpoint connectivity
   - Check service dependencies

## Support

For security-related issues or questions:
- Review the [production deployment guide](../../../docs/production-deployment-security.md)
- Check the Neo documentation
- Contact the Neo development community

Remember: Security is not a one-time setup but an ongoing process. Regularly review and update your security measures.