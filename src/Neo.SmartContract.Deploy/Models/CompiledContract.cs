using System;
using Neo.SmartContract.Manifest;

namespace Neo.SmartContract.Deploy.Models;

/// <summary>
/// Represents a compiled smart contract
/// </summary>
public class CompiledContract
{
    /// <summary>
    /// Contract name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Path to NEF file
    /// </summary>
    public string NefFilePath { get; set; } = string.Empty;

    /// <summary>
    /// Path to manifest file
    /// </summary>
    public string ManifestFilePath { get; set; } = string.Empty;

    /// <summary>
    /// NEF file bytes
    /// </summary>
    public byte[] NefBytes { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Contract manifest
    /// </summary>
    public ContractManifest Manifest { get; set; } = new();

    /// <summary>
    /// Manifest bytes (JSON)
    /// </summary>
    public byte[] ManifestBytes => System.Text.Encoding.UTF8.GetBytes(Manifest.ToJson().ToString());
}
