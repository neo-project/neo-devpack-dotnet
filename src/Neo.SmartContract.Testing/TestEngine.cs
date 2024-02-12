using Moq;
using Neo.Cryptography.ECC;
using Neo.IO;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.Extensions;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Neo.SmartContract.Testing
{
    public class TestEngine
    {
        private readonly Dictionary<UInt160, List<SmartContract>> _contracts = new();
        private readonly Dictionary<UInt160, Dictionary<string, CustomMock>> _customMocks = new();
        private NativeArtifacts? _native;

        /// <summary>
        /// Default Protocol Settings
        /// </summary>
        public static readonly ProtocolSettings Default = new()
        {
            Network = 0x334F454Eu,
            AddressVersion = ProtocolSettings.Default.AddressVersion,
            StandbyCommittee = new[]
            {
                //Validators
                ECPoint.Parse("03b209fd4f53a7170ea4444e0cb0a6bb6a53c2bd016926989cf85f9b0fba17a70c", ECCurve.Secp256r1),
                ECPoint.Parse("02df48f60e8f3e01c48ff40b9b7f1310d7a8b2a193188befe1c2e3df740e895093", ECCurve.Secp256r1),
                ECPoint.Parse("03b8d9d5771d8f513aa0869b9cc8d50986403b78c6da36890638c3d46a5adce04a", ECCurve.Secp256r1),
                ECPoint.Parse("02ca0e27697b9c248f6f16e085fd0061e26f44da85b58ee835c110caa5ec3ba554", ECCurve.Secp256r1),
                ECPoint.Parse("024c7b7fb6c310fccf1ba33b082519d82964ea93868d676662d4a59ad548df0e7d", ECCurve.Secp256r1),
                ECPoint.Parse("02aaec38470f6aad0042c6e877cfd8087d2676b0f516fddd362801b9bd3936399e", ECCurve.Secp256r1),
                ECPoint.Parse("02486fd15702c4490a26703112a5cc1d0923fd697a33406bd5a1c00e0013b09a70", ECCurve.Secp256r1),
                //Other Members
                ECPoint.Parse("023a36c72844610b4d34d1968662424011bf783ca9d984efa19a20babf5582f3fe", ECCurve.Secp256r1),
                ECPoint.Parse("03708b860c1de5d87f5b151a12c2a99feebd2e8b315ee8e7cf8aa19692a9e18379", ECCurve.Secp256r1),
                ECPoint.Parse("03c6aa6e12638b36e88adc1ccdceac4db9929575c3e03576c617c49cce7114a050", ECCurve.Secp256r1),
                ECPoint.Parse("03204223f8c86b8cd5c89ef12e4f0dbb314172e9241e30c9ef2293790793537cf0", ECCurve.Secp256r1),
                ECPoint.Parse("02a62c915cf19c7f19a50ec217e79fac2439bbaad658493de0c7d8ffa92ab0aa62", ECCurve.Secp256r1),
                ECPoint.Parse("03409f31f0d66bdc2f70a9730b66fe186658f84a8018204db01c106edc36553cd0", ECCurve.Secp256r1),
                ECPoint.Parse("0288342b141c30dc8ffcde0204929bb46aed5756b41ef4a56778d15ada8f0c6654", ECCurve.Secp256r1),
                ECPoint.Parse("020f2887f41474cfeb11fd262e982051c1541418137c02a0f4961af911045de639", ECCurve.Secp256r1),
                ECPoint.Parse("0222038884bbd1d8ff109ed3bdef3542e768eef76c1247aea8bc8171f532928c30", ECCurve.Secp256r1),
                ECPoint.Parse("03d281b42002647f0113f36c7b8efb30db66078dfaaa9ab3ff76d043a98d512fde", ECCurve.Secp256r1),
                ECPoint.Parse("02504acbc1f4b3bdad1d86d6e1a08603771db135a73e61c9d565ae06a1938cd2ad", ECCurve.Secp256r1),
                ECPoint.Parse("0226933336f1b75baa42d42b71d9091508b638046d19abd67f4e119bf64a7cfb4d", ECCurve.Secp256r1),
                ECPoint.Parse("03cdcea66032b82f5c30450e381e5295cae85c5e6943af716cc6b646352a6067dc", ECCurve.Secp256r1),
                ECPoint.Parse("02cd5a5547119e24feaa7c2a0f37b8c9366216bab7054de0065c9be42084003c8a", ECCurve.Secp256r1)
            },
            ValidatorsCount = 7,
            SeedList = System.Array.Empty<string>(),
            MillisecondsPerBlock = ProtocolSettings.Default.MillisecondsPerBlock,
            MaxTransactionsPerBlock = ProtocolSettings.Default.MaxTransactionsPerBlock,
            MemoryPoolMaxTransactions = ProtocolSettings.Default.MemoryPoolMaxTransactions,
            MaxTraceableBlocks = ProtocolSettings.Default.MaxTraceableBlocks,
            InitialGasDistribution = ProtocolSettings.Default.InitialGasDistribution,
            Hardforks = ProtocolSettings.Default.Hardforks
        };

        /// <summary>
        /// Storage
        /// </summary>
        public TestStorage Storage { get; init; } = new TestStorage(new MemoryStore());

        /// <summary>
        /// Protocol Settings
        /// </summary>
        public ProtocolSettings ProtocolSettings { get; }

        /// <summary>
        /// BFT Address
        /// </summary>
        public UInt160 BFTAddress
        {
            get
            {
                var validators = Neo.SmartContract.Native.NativeContract.NEO.ComputeNextBlockValidators(Storage.Snapshot, ProtocolSettings);
                return Contract.GetBFTAddress(validators);
            }
        }

        /// <summary>
        /// Committee Address
        /// </summary>
        public UInt160 CommitteeAddress =>
            Neo.SmartContract.Native.NativeContract.NEO.GetCommitteeAddress(Storage.Snapshot);

        /// <summary>
        /// BFTAddress
        /// </summary>
        public Block CurrentBlock { get; }

        /// <summary>
        /// Transaction
        /// </summary>
        public Transaction Transaction { get; }

        /// <summary>
        /// Gas
        /// </summary>
        public long Gas { get; set; } = ApplicationEngine.TestModeGas;

        /// <summary>
        /// Sender
        /// </summary>
        public UInt160 Sender => Transaction.Sender;

        /// <summary>
        /// Native artifacts
        /// </summary>
        public NativeArtifacts Native
        {
            get
            {
                _native ??= new NativeArtifacts(this);
                return _native;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="initializeNativeContracts">Initialize native contracts</param>
        public TestEngine(bool initializeNativeContracts = true) : this(Default, initializeNativeContracts) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings">Settings</param>
        /// <param name="initializeNativeContracts">Initialize native contracts</param>
        public TestEngine(ProtocolSettings settings, bool initializeNativeContracts = true)
        {
            ProtocolSettings = settings;
            CurrentBlock = NeoSystem.CreateGenesisBlock(ProtocolSettings);
            CurrentBlock.Header.Index++;

            var validatorsScript = Contract.CreateMultiSigRedeemScript(settings.StandbyValidators.Count - (settings.StandbyValidators.Count - 1) / 3, settings.StandbyValidators);
            var committeeScript = Contract.CreateMultiSigRedeemScript(settings.StandbyCommittee.Count - (settings.StandbyCommittee.Count - 1) / 2, settings.StandbyCommittee);

            Transaction = new Transaction()
            {
                Attributes = System.Array.Empty<TransactionAttribute>(),
                Script = System.Array.Empty<byte>(),
                Signers = new Signer[]
                {
                    new Signer()
                    {
                        // BFTAddress
                        Account = validatorsScript.ToScriptHash(),
                        Scopes = WitnessScope.Global
                    },
                    new Signer()
                    {
                        // CommitteeAddress
                        Account = committeeScript.ToScriptHash(),
                        Scopes = WitnessScope.Global
                    }
                },
                Witnesses = new Witness[]
                {
                    new Witness()
                    {
                         InvocationScript = validatorsScript,
                         VerificationScript = System.Array.Empty<byte>()
                    },
                    new Witness()
                    {
                         InvocationScript = committeeScript,
                         VerificationScript = System.Array.Empty<byte>()
                    }
                }
            };

            ApplicationEngine.Log += ApplicationEngine_Log;
            ApplicationEngine.Notify += ApplicationEngine_Notify;

            if (initializeNativeContracts)
            {
                Native.Initialize(false);
            }
        }

        #region Invoke events

        private void ApplicationEngine_Notify(object? sender, NotifyEventArgs e)
        {
            if (_contracts.TryGetValue(e.ScriptHash, out var contracts))
            {
                foreach (var contract in contracts)
                {
                    contract.InvokeOnNotify(e.EventName, e.State);
                }
            }
        }

        private void ApplicationEngine_Log(object? sender, LogEventArgs e)
        {
            if (_contracts.TryGetValue(e.ScriptHash, out var contracts))
            {
                foreach (var contract in contracts)
                {
                    contract.InvokeOnRuntimeLog(e.Message);
                }
            }
        }

        #endregion

        /// <summary>
        /// Deploy Smart contract
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="nef">Nef file</param>
        /// <param name="manifest">Contract manifest</param>
        /// <param name="data">Construction data</param>
        /// <param name="customMock">Custom Mock</param>
        /// <returns>Mocked Smart Contract</returns>
        public T Deploy<T>(NefFile nef, ContractManifest manifest, object? data = null, Action<Mock<T>>? customMock = null) where T : SmartContract
        {
            // Deploy

            var state = Native.ContractManagement.Deploy(nef.ToArray(), Encoding.UTF8.GetBytes(manifest.ToJson().ToString(false)), data.ConvertToStackItem());

            // Mock contract

            //UInt160 hash = Helper.GetContractHash(Sender, nef.CheckSum, manifest.Name);
            return MockContract(state.Hash, state.Id, customMock);
        }

        /// <summary>
        /// Deploy Smart contract
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="hash">Contract hash</param>
        /// <param name="checkExistence">Check existence (default: true)</param>
        /// <returns>Mocked Smart Contract</returns>
        public T FromHash<T>(UInt160 hash, bool checkExistence = true) where T : SmartContract
        {
            return FromHash<T>(hash, null, checkExistence);
        }

        /// <summary>
        /// Deploy Smart contract
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="hash">Contract hash</param>
        /// <param name="customMock">Custom Mock</param>
        /// <param name="checkExistence">Check existence (default: true)</param>
        /// <returns>Mocked Smart Contract</returns>
        public T FromHash<T>(UInt160 hash, Action<Mock<T>>? customMock = null, bool checkExistence = true) where T : SmartContract
        {
            if (!checkExistence)
            {
                return MockContract(hash, null, customMock);
            }

            var state = Native.ContractManagement.GetContract(hash);

            return MockContract(state.Hash, state.Id, customMock);
        }

        private T MockContract<T>(UInt160 hash, int? contractId = null, Action<Mock<T>>? customMock = null) where T : SmartContract
        {
            var mock = new Mock<T>(new SmartContractInitialize() { Engine = this, Hash = hash, ContractId = contractId })
            {
                CallBase = true
            };

            // User can mock specific calls

            customMock?.Invoke(mock);

            // Mock SmartContract

            foreach (var method in typeof(T).GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!method.IsAbstract) continue;

                // Avoid to mock already mocked by custom mocks

                if (mock.IsMocked(method))
                {
                    var mockName = method.Name + ";" + method.GetParameters().Length;
                    var cm = new CustomMock() { Contract = mock.Object, Method = method };

                    if (_customMocks.TryGetValue(hash, out var mocks))
                    {
                        if (!mocks.TryAdd(mockName, cm))
                        {
                            throw new Exception("The same method can't be mocked twice");
                        }
                    }
                    else
                    {
                        _customMocks.Add(hash, new Dictionary<string, CustomMock>() { { mockName, cm } });
                    }

                    continue;
                }

                // Get args

                Type[] args = method.GetParameters().Select(u => u.ParameterType).ToArray();

                // Mock by ReturnType

                if (method.ReturnType != typeof(void))
                {
                    mock.MockMethodWithReturn(method.Name, args, method.ReturnType);
                }
                else
                {
                    mock.MockMethod(method.Name, args);
                }
            }

            mock.Verify();

            // Cache sc

            if (_contracts.TryGetValue(hash, out var result))
            {
                result.Add(mock.Object);
            }
            else
            {
                _contracts[hash] = new List<SmartContract>(new SmartContract[] { mock.Object });
            }

            // return mocked SmartContract

            return mock.Object;
        }

        internal bool TryGetCustomMock(UInt160 hash, string method, int rc, [NotNullWhen(true)] out CustomMock? mi)
        {
            if (_customMocks.TryGetValue(hash, out var mocks))
            {
                var mockName = method + ";" + rc;

                if (mocks.TryGetValue(mockName, out mi))
                {
                    return true;
                }
            }

            mi = null;
            return false;
        }

        /// <summary>
        /// Execute raw script
        /// </summary>
        /// <param name="script">Script</param>
        /// <returns>StackItem</returns>
        public StackItem Execute(Script script)
        {
            // Store the script in current transaction

            Transaction.Script = script;

            // Execute in neo VM

            var snapshot = Storage.Snapshot.CreateSnapshot();

            using var engine = new TestingApplicationEngine(this, TriggerType.Application,
                Transaction, snapshot, CurrentBlock, ProtocolSettings, Gas);

            engine.LoadScript(script);

            if (engine.Execute() != VMState.HALT)
            {
                throw engine.FaultException ?? new Exception($"Error while executing the script");
            }

            snapshot.Commit();

            // Return

            if (engine.ResultStack.Count == 0) return StackItem.Null;
            return engine.ResultStack.Pop();
        }
    }
}
