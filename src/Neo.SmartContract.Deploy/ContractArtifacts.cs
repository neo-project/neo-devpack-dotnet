using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Neo.SmartContract.Deploy;

/// <summary>
/// Represents the compiled NEF/manifest pair that will be deployed to a target network.
/// </summary>
public sealed class ContractArtifacts
{
    private ContractArtifacts(NefFile nef, ContractManifest manifest)
    {
        Nef = nef ?? throw new ArgumentNullException(nameof(nef));
        Manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
    }

    /// <summary>
    /// Gets the compiled NEF file.
    /// </summary>
    public NefFile Nef { get; }

    /// <summary>
    /// Gets the manifest that describes the contract's ABI and permissions.
    /// </summary>
    public ContractManifest Manifest { get; }

    /// <summary>
    /// Loads artifacts from NEF/manifest files on disk.
    /// </summary>
    public static async Task<ContractArtifacts> FromFilesAsync(string nefPath, string manifestPath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(nefPath))
            throw new ArgumentException("NEF path must be provided.", nameof(nefPath));
        if (string.IsNullOrWhiteSpace(manifestPath))
            throw new ArgumentException("Manifest path must be provided.", nameof(manifestPath));

        if (!File.Exists(nefPath))
            throw new FileNotFoundException($"NEF file '{nefPath}' was not found.", nefPath);
        if (!File.Exists(manifestPath))
            throw new FileNotFoundException($"Manifest file '{manifestPath}' was not found.", manifestPath);

        var nefBytes = await File.ReadAllBytesAsync(nefPath, cancellationToken).ConfigureAwait(false);
        var nef = NefFile.Parse(nefBytes, verify: true);

        var manifestJson = await File.ReadAllTextAsync(manifestPath, cancellationToken).ConfigureAwait(false);
        var manifest = ContractManifest.Parse(manifestJson);

        return new ContractArtifacts(nef, manifest);
    }
}
