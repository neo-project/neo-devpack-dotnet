using Neo.Persistence;
using System;
using System.Reflection;
using NativeContract = Neo.SmartContract.Native.NativeContract;

namespace Neo.SmartContract.Testing.Native;

/// <summary>
/// NativeArtifacts makes it easier to access native contracts
/// </summary>
public class NativeArtifacts
{
    private readonly TestEngine _engine;

    /// <summary>
    /// ContractManagement
    /// </summary>
    public ContractManagement ContractManagement { get; }

    /// <summary>
    /// CryptoLib
    /// </summary>
    public CryptoLib CryptoLib { get; }

    /// <summary>
    /// GasToken
    /// </summary>
    public GasToken GAS { get; }

    /// <summary>
    /// NeoToken
    /// </summary>
    public NeoToken NEO { get; }

    /// <summary>
    /// LedgerContract
    /// </summary>
    public LedgerContract Ledger { get; }

    /// <summary>
    /// OracleContract
    /// </summary>
    public OracleContract Oracle { get; }

    /// <summary>
    /// PolicyContract
    /// </summary>
    public PolicyContract Policy { get; }

    /// <summary>
    /// RoleManagement
    /// </summary>
    public RoleManagement RoleManagement { get; }

    /// <summary>
    /// StdLib
    /// </StdLib>
    public StdLib StdLib { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="engine">Engine</param>
    public NativeArtifacts(TestEngine engine)
    {
        _engine = engine;

        ContractManagement = _engine.FromHash<ContractManagement>(NativeContract.ContractManagement.Hash, NativeContract.ContractManagement.Id);
        CryptoLib = _engine.FromHash<CryptoLib>(NativeContract.CryptoLib.Hash, NativeContract.CryptoLib.Id);
        GAS = _engine.FromHash<GasToken>(NativeContract.GAS.Hash, NativeContract.GAS.Id);
        NEO = _engine.FromHash<NeoToken>(NativeContract.NEO.Hash, NativeContract.NEO.Id);
        Ledger = _engine.FromHash<LedgerContract>(NativeContract.Ledger.Hash, NativeContract.Ledger.Id);
        Oracle = _engine.FromHash<OracleContract>(NativeContract.Oracle.Hash, NativeContract.Oracle.Id);
        Policy = _engine.FromHash<PolicyContract>(NativeContract.Policy.Hash, NativeContract.Policy.Id);
        RoleManagement = _engine.FromHash<RoleManagement>(NativeContract.RoleManagement.Hash, NativeContract.RoleManagement.Id);
        StdLib = _engine.FromHash<StdLib>(NativeContract.StdLib.Hash, NativeContract.StdLib.Id);
    }

    /// <summary>
    /// Initialize native contracts
    /// </summary>
    /// <param name="commit">Initialize native contracts</param>
    public void Initialize(bool commit = false)
    {
        _engine.Transaction.Script = Array.Empty<byte>(); // Store the script in the current transaction

        var genesis = NeoSystem.CreateGenesisBlock(_engine.ProtocolSettings);

        // Attach to static event

        ApplicationEngine.Log += _engine.ApplicationEngineLog;
        ApplicationEngine.Notify += _engine.ApplicationEngineNotify;

        // Process native contracts

        foreach (var native in new NativeContract[]
            {
                NativeContract.ContractManagement,
                NativeContract.Ledger,
                NativeContract.NEO,
                NativeContract.GAS
            }
        )
        {
            Type nativeType = native.GetType();

            // Mock Native.OnPersist

            var method = nativeType.GetMethod("OnPersist", BindingFlags.NonPublic | BindingFlags.Instance)!;

            DataCache clonedSnapshot = _engine.Storage.Snapshot.CreateSnapshot();
            using (var engine = new TestingApplicationEngine(_engine, TriggerType.OnPersist, genesis, clonedSnapshot, genesis))
            {
                engine.LoadScript(Array.Empty<byte>());

                Await(method, native, engine);

                if (engine.Execute() != VM.VMState.HALT)
                    throw new Exception($"Error executing {native.Name}.OnPersist");
            }

            // Mock Native.PostPersist

            method = nativeType.GetMethod("PostPersist", BindingFlags.NonPublic | BindingFlags.Instance)!;

            using (var engine = new TestingApplicationEngine(_engine, TriggerType.OnPersist, genesis, clonedSnapshot, genesis))
            {
                engine.LoadScript(Array.Empty<byte>());

                Await(method, native, engine);

                if (engine.Execute() != VM.VMState.HALT)
                    throw new Exception($"Error executing {native.Name}.PostPersist");
            }

            clonedSnapshot.Commit();
        }

        if (commit)
        {
            _engine.Storage.Commit();
        }

        // Detach to static event

        ApplicationEngine.Log -= _engine.ApplicationEngineLog;
        ApplicationEngine.Notify -= _engine.ApplicationEngineNotify;
    }

    private static void Await(MethodInfo method, NativeContract native, TestingApplicationEngine engine)
    {
        var result = method!.Invoke(native, new object[] { engine });

        /*
        if (result is not ContractTask task)
            throw new Exception($"Error casting {native.Name}.{method.Name} to ContractTask");

        task.GetAwaiter().GetResult();
        */

        if (result is null)
            throw new Exception($"{native.Name}.{method.Name} result can't be null");

        method = result.GetType().GetMethod("GetAwaiter", BindingFlags.Public | BindingFlags.Instance)!;
        result = method?.Invoke(result, Array.Empty<object>());

        if (result is null)
            throw new Exception($"{native.Name}.{method?.Name}.GetAwaiter() result can't be null");

        method = result.GetType().GetMethod("GetResult", BindingFlags.Public | BindingFlags.Instance)!;
        result = method?.Invoke(result, Array.Empty<object>());

        if (result is not null)
            throw new Exception($"{native.Name}.{method?.Name}.GetAwaiter().GetResult() can't be not null");
    }
}
