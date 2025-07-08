using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neo;
using Neo.Network.RPC;
using Neo.Network.RPC.Models;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.Cryptography.ECC;

namespace Neo.SmartContract.Deploy.UnitTests.Mocks;

/// <summary>
/// Mock RPC client for unit testing without network dependency
/// </summary>
public class MockRpcClient : RpcClient
{
    private readonly RpcInvokeResult _invokeResult;
    private readonly uint _blockCount;
    private readonly bool _contractExists;
    private readonly UInt256? _sentTxHash;
    private readonly RpcTransaction? _transaction;

    public MockRpcClient(
        RpcInvokeResult? invokeResult = null,
        uint blockCount = 1000,
        bool contractExists = false,
        UInt256? sentTxHash = null,
        RpcTransaction? transaction = null) : base(new Uri("http://mock"))
    {
        _invokeResult = invokeResult ?? new RpcInvokeResult
        {
            State = Neo.VM.VMState.HALT,
            GasConsumed = 1000000,
            Stack = new Neo.VM.Types.StackItem[] { Neo.VM.Types.StackItem.Null }
        };
        _blockCount = blockCount;
        _contractExists = contractExists;
        _sentTxHash = sentTxHash;
        _transaction = transaction;
    }

    public new Task<uint> GetBlockCountAsync()
    {
        return Task.FromResult(_blockCount);
    }

    public new Task<RpcInvokeResult> InvokeScriptAsync(byte[] script, params Signer[] signers)
    {
        return Task.FromResult(_invokeResult);
    }

    public new Task<ContractState?> GetContractStateAsync(string hash)
    {
        if (!_contractExists) return Task.FromResult<ContractState?>(null);

        // Return a mock contract state
        var manifest = new ContractManifest
        {
            Name = "MockContract",
            Groups = System.Array.Empty<ContractGroup>(),
            SupportedStandards = System.Array.Empty<string>(),
            Abi = new ContractAbi
            {
                Methods = System.Array.Empty<ContractMethodDescriptor>(),
                Events = System.Array.Empty<ContractEventDescriptor>()
            },
            Permissions = new[] { ContractPermission.DefaultPermission },
            Trusts = WildcardContainer<ContractPermissionDescriptor>.Create(),
            Extra = null
        };

        var nef = new NefFile
        {
            Compiler = "neo-core-v3.0",
            Source = "",
            Tokens = System.Array.Empty<MethodToken>(),
            Script = new byte[] { 0x01, 0x02, 0x03 },
            CheckSum = 12345
        };

        var state = new ContractState
        {
            Id = 1,
            UpdateCounter = 0,
            Hash = UInt160.Parse(hash),
            Nef = nef,
            Manifest = manifest
        };

        return Task.FromResult<ContractState?>(state);
    }

    public new Task<UInt256> SendRawTransactionAsync(Transaction transaction)
    {
        return Task.FromResult(_sentTxHash ?? transaction.Hash);
    }

    public new Task<RpcTransaction?> GetRawTransactionAsync(string txHash)
    {
        return Task.FromResult(_transaction);
    }

    public new Task<RpcVersion> GetVersionAsync()
    {
        return Task.FromResult(new RpcVersion
        {
            TcpPort = 10333,
            Nonce = 1234567890,
            UserAgent = "/Neo:3.0.0/",
            Protocol = new RpcVersion.RpcProtocol
            {
                AddressVersion = 53,
                Network = 860833102, // TestNet
                MillisecondsPerBlock = 15000,
                MaxTraceableBlocks = 2102400,
                MaxValidUntilBlockIncrement = 5760,
                MaxTransactionsPerBlock = 512,
                MemoryPoolMaxTransactions = 50000,
                ValidatorsCount = 7,
                InitialGasDistribution = 5200000000000000,
                Hardforks = new Dictionary<Hardfork, uint>(),
                SeedList = new List<string>(),
                StandbyCommittee = new List<ECPoint>()
            }
        });
    }
}
