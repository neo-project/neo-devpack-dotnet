using System.Security.Cryptography;
using System.Text;
using Neo;
using Neo.Cryptography;
using Neo.SmartContract;
using Neo.Wallets;

namespace R3E.WebGUI.Service.Core.Services;

public class SignatureValidationService : ISignatureValidationService
{
    private readonly ILogger<SignatureValidationService> _logger;
    private readonly IConfiguration _configuration;
    private const int TIMESTAMP_VALIDITY_MINUTES = 5;

    public SignatureValidationService(ILogger<SignatureValidationService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<bool> ValidateSignatureAsync(string message, string signature, string publicKey, string expectedAddress)
    {
        try
        {
            // Convert hex strings to byte arrays
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var signatureBytes = Convert.FromHexString(signature);
            var publicKeyBytes = Convert.FromHexString(publicKey);

            // Create ECPoint from public key
            var pubKey = Neo.Cryptography.ECC.ECPoint.DecodePoint(publicKeyBytes, Neo.Cryptography.ECC.ECCurve.Secp256r1);
            
            // Verify signature
            var isValid = Crypto.VerifySignature(messageBytes, signatureBytes, pubKey);
            
            if (!isValid)
            {
                _logger.LogWarning("Invalid signature for message: {Message}", message);
                return false;
            }

            // Verify that public key corresponds to expected address
            var scriptHash = Contract.CreateSignatureContract(pubKey).ScriptHash;
            var address = scriptHash.ToAddress(0x35); // 0x35 is the version byte for Neo N3 mainnet/testnet
            
            if (!string.Equals(address, expectedAddress, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Public key does not match expected address. Expected: {Expected}, Got: {Actual}", 
                    expectedAddress, address);
                return false;
            }

            _logger.LogInformation("Signature validated successfully for address: {Address}", expectedAddress);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating signature");
            return false;
        }
    }

    public string CreateDeploymentMessage(string contractAddress, string deployerAddress, long timestamp)
    {
        // Create a deterministic message for deployment
        return $"Deploy WebGUI for contract {contractAddress} by {deployerAddress} at {timestamp}";
    }

    public string CreatePluginUploadMessage(string contractAddress, string pluginHash, long timestamp)
    {
        // Create a deterministic message for plugin upload
        return $"Upload plugin for contract {contractAddress} with hash {pluginHash} at {timestamp}";
    }

    public bool IsTimestampValid(long timestamp)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var difference = Math.Abs(now - timestamp);
        
        // Check if timestamp is within acceptable range (default 5 minutes)
        var validityMinutes = _configuration.GetValue<int>("R3EWebGUI:Security:TimestampValidityMinutes", TIMESTAMP_VALIDITY_MINUTES);
        return difference <= (validityMinutes * 60);
    }
}