using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Neo.Compiler;
using Neo.Extensions;
using Neo.IO;
using Neo.Network.P2P.Payloads;
using Neo.Network.RPC;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.Wallets;
using CompilationOptions = Neo.Compiler.CompilationOptions;

namespace Neo.SmartContract.Deploy;

public partial class DeploymentToolkit
{
    /// <summary>
    /// Compile and deploy a smart contract from source (csproj or single C# file).
    /// </summary>
    /// <param name="path">Path to the project or source file.</param>
    /// <param name="initParams">Optional initialization parameters supplied to the deploy script.</param>
    /// <param name="targetContract">Optional contract name when the project builds multiple contracts.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Deployment information.</returns>
    /// <example>
    /// <code>
    /// using var toolkit = new DeploymentToolkit()
    ///     .SetNetwork("testnet")
    ///     .SetWifKey("<wif>");
    /// var deployment = await toolkit.DeployAsync("Contracts/MyContract.csproj", new object?[] { "owner", 1000 });
    /// </code>
    /// </example>
    public virtual async Task<ContractDeploymentInfo> DeployAsync(
        string path,
        object?[]? initParams = null,
        string? targetContract = null,
        CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));

        var fullPath = Path.GetFullPath(path);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException("Contract project or source file not found.", fullPath);

        var compilationOptions = await CreateCompilationOptionsAsync(
            Path.GetFileNameWithoutExtension(fullPath),
            cancellationToken).ConfigureAwait(false);

        var artifacts = await CompileContractsAsync(
            fullPath,
            compilationOptions,
            targetContract,
            cancellationToken).ConfigureAwait(false);

        var artifact = artifacts[0];
        return await DeployCompiledArtifactAsync(artifact, initParams, cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<IReadOnlyList<CompiledContractArtifact>> CompileAsync(
        string path,
        string? targetContract = null,
        CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be null or empty.", nameof(path));

        var fullPath = Path.GetFullPath(path);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException("Contract project or source file not found.", fullPath);

        var options = await CreateCompilationOptionsAsync(
            Path.GetFileNameWithoutExtension(fullPath),
            cancellationToken).ConfigureAwait(false);

        return await CompileContractsAsync(fullPath, options, targetContract, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deploy a pre-compiled contract from NEF and manifest files.
    /// </summary>
    /// <param name="nefPath">Path to NEF file</param>
    /// <param name="manifestPath">Path to manifest file</param>
    /// <param name="initParams">Optional initialization parameters</param>
    /// <returns>Deployment information</returns>
    /// <example>
    /// <code>
    /// var request = new DeploymentArtifactsRequest("Token.nef", "Token.manifest.json")
    ///     .WithInitParams("admin", 10_000);
    /// using var toolkit = new DeploymentToolkit().SetNetwork("local").SetWifKey("<wif>");
    /// var result = await toolkit.DeployArtifactsAsync(request);
    /// </code>
    /// </example>
    public virtual Task<ContractDeploymentInfo> DeployArtifactsAsync(
        DeploymentArtifactsRequest request,
        CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        ArgumentNullException.ThrowIfNull(request);
        return DeployArtifactsInternalAsync(
            request.NefPath,
            request.ManifestPath,
            request.InitParams ?? Array.Empty<object?>(),
            request.WaitForConfirmation,
            request.ConfirmationRetries,
            request.ConfirmationDelaySeconds,
            cancellationToken,
            request.Signers,
            request.TransactionSignerAsync);
    }

    /// <summary>
    /// Deploy a pre-compiled contract from NEF and manifest files with customization options.
    /// </summary>
    /// <param name="nefPath">Path to NEF file.</param>
    /// <param name="manifestPath">Path to manifest file.</param>
    /// <param name="initParams">Optional initialization parameters.</param>
    /// <param name="waitForConfirmation">Whether to poll for confirmation after submission.</param>
    /// <param name="confirmationRetries">Number of confirmation polls.</param>
    /// <param name="confirmationDelaySeconds">Delay between confirmation polls (seconds).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="signers">Optional signers.</param>
    /// <param name="transactionSignerAsync">Custom transaction signer.</param>
    /// <example>
    /// <code>
    /// using var toolkit = new DeploymentToolkit().SetNetwork("http://localhost:50012").SetWifKey("<wif>");
    /// var info = await toolkit.DeployArtifactsAsync("My.nef", "My.manifest.json", new object?[] { "owner" }, waitForConfirmation: true);
    /// </code>
    /// </example>
    public virtual Task<ContractDeploymentInfo> DeployArtifactsAsync(
        string nefPath,
        string manifestPath,
        object?[]? initParams = null,
        bool? waitForConfirmation = null,
        int? confirmationRetries = null,
        int? confirmationDelaySeconds = null,
        CancellationToken cancellationToken = default,
        IReadOnlyList<Signer>? signers = null,
        Func<TransactionManager, CancellationToken, Task<Transaction>>? transactionSignerAsync = null)
    {
        EnsureNotDisposed();
        return DeployArtifactsInternalAsync(
            nefPath,
            manifestPath,
            initParams ?? Array.Empty<object?>(),
            waitForConfirmation,
            confirmationRetries,
            confirmationDelaySeconds,
            cancellationToken,
            signers,
            transactionSignerAsync);
    }

    private async Task<ContractDeploymentInfo> DeployArtifactsInternalAsync(
        string nefPath,
        string manifestPath,
        object?[] initParams,
        bool? waitForConfirmation,
        int? confirmationRetries,
        int? confirmationDelaySeconds,
        CancellationToken cancellationToken,
        IReadOnlyList<Signer>? signers,
        Func<TransactionManager, CancellationToken, Task<Transaction>>? transactionSignerAsync)
    {
        EnsureNotDisposed();
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(nefPath))
            throw new ArgumentException("NEF path is required.", nameof(nefPath));

        if (string.IsNullOrWhiteSpace(manifestPath))
            throw new ArgumentException("Manifest path is required.", nameof(manifestPath));

        if (!File.Exists(nefPath))
            throw new FileNotFoundException("NEF file not found.", nefPath);

        if (!File.Exists(manifestPath))
            throw new FileNotFoundException("Manifest file not found.", manifestPath);

        var nefBytes = await File.ReadAllBytesAsync(nefPath, cancellationToken).ConfigureAwait(false);
        var manifestJson = await File.ReadAllTextAsync(manifestPath, cancellationToken).ConfigureAwait(false);

        // Compute expected contract hash
        var nef = NefFile.Parse(nefBytes, verify: true);
        var manifest = ContractManifest.FromJson((Neo.Json.JObject)Neo.Json.JToken.Parse(manifestJson)!);

        var protocolSettings = await GetProtocolSettingsAsync().ConfigureAwait(false);
        var resolvedSigners = ResolveSigners(signers, protocolSettings);

        KeyPair? defaultKeyPair = null;
        Neo.UInt160? defaultSignerAccount = null;

        if (!string.IsNullOrWhiteSpace(_wifKey))
        {
            defaultKeyPair = Neo.Network.RPC.Utility.GetKeyPair(_wifKey);
            defaultSignerAccount = Contract.CreateSignatureContract(defaultKeyPair.PublicKey).ScriptHash;
        }

        KeyPair EnsureDefaultKeyPair()
        {
            if (defaultKeyPair is not null)
                return defaultKeyPair;

            var wif = EnsureWif();
            defaultKeyPair = Neo.Network.RPC.Utility.GetKeyPair(wif);
            defaultSignerAccount ??= Contract.CreateSignatureContract(defaultKeyPair.PublicKey).ScriptHash;
            return defaultKeyPair;
        }

        Neo.UInt160 EnsureDefaultSignerAccount()
        {
            if (defaultSignerAccount is not null)
                return defaultSignerAccount;

            var key = EnsureDefaultKeyPair();
            defaultSignerAccount = Contract.CreateSignatureContract(key.PublicKey).ScriptHash;
            return defaultSignerAccount!;
        }

        if (resolvedSigners.Count == 0)
        {
            resolvedSigners = new[]
            {
                new Signer { Account = EnsureDefaultSignerAccount(), Scopes = WitnessScope.CalledByEntry }
            };
        }

        var signersArray = resolvedSigners is Signer[] direct
            ? (Signer[])direct.Clone()
            : resolvedSigners.ToArray();

        Neo.UInt160? expectedSender = null;

        // Build deploy script
        var script = BuildDeployScript(nefBytes, manifestJson, initParams);

        var policy = ResolveConfirmationPolicy(waitForConfirmation, confirmationRetries, confirmationDelaySeconds);
        var signerDelegate = transactionSignerAsync
            ?? _options.TransactionSignerAsync;

        if (signerDelegate is null)
        {
            if (string.IsNullOrWhiteSpace(_wifKey))
            {
                throw new InvalidOperationException("No signing credentials available. Provide a WIF via SetWifKey(), configure DeploymentOptions.TransactionSignerAsync, or supply a transaction signer when calling DeployArtifactsAsync.");
            }

            var key = EnsureDefaultKeyPair();
            var wifAccount = EnsureDefaultSignerAccount();

            var wifIndex = Array.FindIndex(signersArray, s => s.Account == wifAccount);
            if (wifIndex < 0)
            {
                throw new InvalidOperationException("Resolved signers do not contain the account derived from the configured WIF. Supply a TransactionSignerAsync or include a signer entry for that account.");
            }

            if (wifIndex != 0)
            {
                (signersArray[0], signersArray[wifIndex]) = (signersArray[wifIndex], signersArray[0]);
            }

            expectedSender = signersArray[0].Account;

            signerDelegate = (tm, ct) =>
            {
                tm.AddSignature(key);
                return tm.SignAsync();
            };
        }
        else
        {
            expectedSender = signersArray[0].Account;
        }

        var sender = expectedSender ?? signersArray[0].Account;
        var expectedHash = Helper.GetContractHash(sender, nef.CheckSum, manifest.Name);

        return await WithRpcClientAsync(async (rpc, _, ct) =>
        {
            ct.ThrowIfCancellationRequested();
            // Build transaction
            var tm = await CreateTransactionManagerAsync(rpc, script.AsMemory(), signersArray, ct).ConfigureAwait(false);

            // Sign and send
            var tx = await signerDelegate(tm, ct).ConfigureAwait(false);
            var txHash = await rpc.SendRawTransactionAsync(tx).ConfigureAwait(false);

            if (policy.WaitForConfirmation)
            {
                var confirmed = await WaitForConfirmationAsync(
                    rpc,
                    txHash,
                    policy.ConfirmationRetries,
                    TimeSpan.FromSeconds(policy.ConfirmationDelaySeconds),
                    ct).ConfigureAwait(false);

                if (!confirmed)
                {
                    throw new TimeoutException($"Transaction {txHash} was not confirmed within the allotted retries.");
                }
            }

            return new ContractDeploymentInfo
            {
                TransactionHash = txHash,
                ContractHash = expectedHash
            };
        }, cancellationToken).ConfigureAwait(false);
    }

    private Task<CompilationOptions> CreateCompilationOptionsAsync(string baseName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var addressVersion = ResolveAddressVersionForCompilation();

        var options = new CompilationOptions
        {
            AddressVersion = addressVersion,
            BaseName = baseName,
            Nullable = NullableContextOptions.Enable,
            Optimize = CompilationOptions.OptimizationType.Basic
        };

        return Task.FromResult(options);
    }

    protected virtual CompilationEngine CreateCompilationEngine(CompilationOptions options) => new(options);

    protected virtual Task<TransactionManager> CreateTransactionManagerAsync(
        RpcClient rpcClient,
        ReadOnlyMemory<byte> script,
        Signer[] signers,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return TransactionManager.MakeTransactionAsync(rpcClient, script, signers);
    }

    protected virtual Task<IReadOnlyList<CompiledContractArtifact>> CompileContractsAsync(
        string path,
        CompilationOptions compilationOptions,
        string? targetContractName,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var extension = Path.GetExtension(path).ToLowerInvariant();
        var engine = CreateCompilationEngine(compilationOptions);

        List<CompilationContext> contexts = extension switch
        {
            ".csproj" => engine.CompileProject(path),
            ".cs" => engine.CompileSources(path),
            _ => throw new NotSupportedException($"Unsupported contract source type '{extension}'. Provide a .csproj or .cs file.")
        } ?? [];

        if (contexts.Count == 0)
            throw new InvalidOperationException("Compilation did not produce any smart contract classes.");

        var errors = contexts
            .SelectMany(ctx => ctx.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
            .ToList();

        if (errors.Count > 0)
            throw new InvalidOperationException(BuildCompilationErrorMessage(errors));

        if (!string.IsNullOrWhiteSpace(targetContractName))
        {
            var context = contexts.FirstOrDefault(c => string.Equals(c.ContractName, targetContractName, StringComparison.OrdinalIgnoreCase));
            if (context is null)
                throw new ArgumentException($"Contract '{targetContractName}' was not found in the compilation output.", nameof(targetContractName));
            contexts = new List<CompilationContext> { context };
        }
        else if (contexts.Count > 1)
        {
            var names = string.Join(", ", contexts.Select(c => c.ContractName ?? "<unnamed>"));
            throw new InvalidOperationException($"Multiple contracts were produced ({names}). Provide a target contract name.");
        }

        var baseFolder = Path.GetDirectoryName(path) ?? Directory.GetCurrentDirectory();
        var artifacts = contexts.Select(context =>
        {
            var (nef, manifest, _) = context.CreateResults(baseFolder);
            var name = context.ContractName ?? Path.GetFileNameWithoutExtension(path);
            return new CompiledContractArtifact(name, nef, manifest);
        }).ToList();

        return Task.FromResult<IReadOnlyList<CompiledContractArtifact>>(artifacts);
    }

    private async Task<ContractDeploymentInfo> DeployCompiledArtifactAsync(
        CompiledContractArtifact artifact,
        object?[]? initParams,
        CancellationToken cancellationToken)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);

        try
        {
            var contractName = string.IsNullOrWhiteSpace(artifact.ContractName)
                ? "contract"
                : artifact.ContractName;

            var nefPath = Path.Combine(tempDir, contractName + ".nef");
            var manifestPath = Path.Combine(tempDir, contractName + ".manifest.json");

            await File.WriteAllBytesAsync(nefPath, artifact.Nef.ToArray(), cancellationToken).ConfigureAwait(false);
            await File.WriteAllTextAsync(manifestPath, artifact.Manifest.ToJson().ToString(), cancellationToken).ConfigureAwait(false);

            return await DeployArtifactsAsync(
                nefPath,
                manifestPath,
                initParams,
                waitForConfirmation: null,
                confirmationRetries: null,
                confirmationDelaySeconds: null,
                cancellationToken: cancellationToken,
                signers: null,
                transactionSignerAsync: null).ConfigureAwait(false);
        }
        finally
        {
            try
            {
                if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
            }
            catch
            {
                // ignore cleanup exceptions
            }
        }
    }

    private static async Task<bool> WaitForConfirmationAsync(RpcClient rpc, UInt256 txHash, int retries, TimeSpan delay, CancellationToken cancellationToken)
    {
        for (int i = 0; i < retries; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var height = await rpc.GetTransactionHeightAsync(txHash.ToString());
                if (height > 0)
                    return true;
            }
            catch
            {
                // Not yet confirmed or node not returning height
            }
            await Task.Delay(delay, cancellationToken);
        }
        return false;
    }

    private static byte[] BuildDeployScript(byte[] nefBytes, string manifestJson, object?[]? initParams)
    {
        using var sb = new ScriptBuilder();
        // Build args in reverse order and PACK
        if (initParams is { Length: > 0 })
        {
            // data (pack array if multiple)
            for (int i = initParams.Length - 1; i >= 0; i--)
            {
                var value = initParams[i];
                if (value is null)
                {
                    sb.Emit(OpCode.PUSHNULL);
                }
                else
                {
                    sb.EmitPush(value);
                }
            }
            sb.EmitPush(initParams.Length);
            sb.Emit(OpCode.PACK);
            // manifest
            sb.EmitPush(manifestJson);
            // nef bytes
            sb.EmitPush(nefBytes);
            // pack [nef, manifest, data]
            sb.EmitPush(3);
            sb.Emit(OpCode.PACK);
        }
        else
        {
            sb.Emit(OpCode.PUSHNULL);
            // manifest
            sb.EmitPush(manifestJson);
            // nef bytes
            sb.EmitPush(nefBytes);
            // pack [nef, manifest, data=null]
            sb.EmitPush(3);
            sb.Emit(OpCode.PACK);
        }

        // call ContractManagement.deploy
        sb.EmitPush(CallFlags.All);
        sb.EmitPush("deploy");
        sb.EmitPush(NativeContract.ContractManagement.Hash);
        sb.EmitSysCall(ApplicationEngine.System_Contract_Call);

        return sb.ToArray();
    }

    private static string BuildCompilationErrorMessage(IEnumerable<Diagnostic> diagnostics)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Compilation failed with the following diagnostics:");
        foreach (var diagnostic in diagnostics)
        {
            sb.AppendLine(diagnostic.ToString());
        }
        return sb.ToString();
    }

    public sealed record CompiledContractArtifact(string ContractName, NefFile Nef, ContractManifest Manifest);
}
