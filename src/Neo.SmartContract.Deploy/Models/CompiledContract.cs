using Neo.SmartContract.Manifest;

namespace Neo.SmartContract.Deploy.Models;

/// <summary>
/// Compiled contract result
/// </summary>
public class CompiledContract
{
    /// <summary>
    /// Contract name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Compiled NEF file path
    /// </summary>
    public string NefFilePath { get; set; } = string.Empty;

    /// <summary>
    /// Contract manifest file path
    /// </summary>
    public string ManifestFilePath { get; set; } = string.Empty;

    /// <summary>
    /// NEF file content
    /// </summary>
    public byte[] NefBytes { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Contract manifest
    /// </summary>
    public ContractManifest Manifest { get; set; } = new();

    /// <summary>
    /// Gets the manifest as bytes for deployment
    /// </summary>
    public byte[] ManifestBytes => System.Text.Encoding.UTF8.GetBytes(Manifest.ToJson().ToString());
}
