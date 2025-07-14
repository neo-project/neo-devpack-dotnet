using R3E.WebGUI.Service.Domain.Models;

namespace R3E.WebGUI.Service.Core.Services;

public interface ISignatureValidationService
{
    /// <summary>
    /// Validates a Neo signature for a given message and address
    /// </summary>
    /// <param name="message">The original message that was signed</param>
    /// <param name="signature">The signature in hex format</param>
    /// <param name="publicKey">The public key in hex format</param>
    /// <param name="expectedAddress">The expected Neo address</param>
    /// <returns>True if signature is valid and matches the expected address</returns>
    Task<bool> ValidateSignatureAsync(string message, string signature, string publicKey, string expectedAddress);

    /// <summary>
    /// Creates a message to be signed for WebGUI deployment
    /// </summary>
    /// <param name="contractAddress">The contract address</param>
    /// <param name="deployerAddress">The deployer address</param>
    /// <param name="timestamp">Unix timestamp</param>
    /// <returns>The message to be signed</returns>
    string CreateDeploymentMessage(string contractAddress, string deployerAddress, long timestamp);

    /// <summary>
    /// Creates a message to be signed for plugin upload
    /// </summary>
    /// <param name="contractAddress">The contract address</param>
    /// <param name="pluginHash">SHA256 hash of the plugin file</param>
    /// <param name="timestamp">Unix timestamp</param>
    /// <returns>The message to be signed</returns>
    string CreatePluginUploadMessage(string contractAddress, string pluginHash, long timestamp);

    /// <summary>
    /// Verifies that a timestamp is within acceptable range (e.g., within 5 minutes)
    /// </summary>
    /// <param name="timestamp">Unix timestamp to verify</param>
    /// <returns>True if timestamp is within acceptable range</returns>
    bool IsTimestampValid(long timestamp);
}