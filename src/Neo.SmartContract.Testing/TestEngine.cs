using Moq;
using Neo.Cryptography.ECC;
using Neo.IO;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Extensions;
using Neo.SmartContract.Testing.Storage;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Neo.SmartContract.Testing
{
    public class TestEngine
    {
        internal readonly Dictionary<UInt160, CoveredContract> Coverage = new();
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
        public EngineStorage Storage { get; set; } = new EngineStorage(new MemoryStore());

        /// <summary>
        /// Protocol Settings
        /// </summary>
        public ProtocolSettings ProtocolSettings { get; }

        /// <summary>
        /// Enable coverage capture
        /// </summary>
        public bool EnableCoverageCapture { get; set; } = true;

        /// <summary>
        /// Method detection
        /// </summary>
        public MethodDetectionMechanism MethodDetection { get; set; } = MethodDetectionMechanism.FindRET;

        /// <summary>
        /// Validators Address
        /// </summary>
        public UInt160 ValidatorsAddress
        {
            get
            {
                if (!Storage.IsInitialized)
                {
                    // If is not initialized, return the ProtocolSettings

                    var validatorsScript = Contract.CreateMultiSigRedeemScript(ProtocolSettings.StandbyValidators.Count - (ProtocolSettings.StandbyValidators.Count - 1) / 3, ProtocolSettings.StandbyValidators);
                    return validatorsScript.ToScriptHash();
                }

                var validators = Neo.SmartContract.Native.NativeContract.NEO.ComputeNextBlockValidators(Storage.Snapshot, ProtocolSettings);
                return Contract.GetBFTAddress(validators);
            }
        }

        /// <summary>
        /// Committee Address
        /// </summary>
        public UInt160 CommitteeAddress
        {
            get
            {
                if (!Storage.IsInitialized)
                {
                    // If is not initialized, return the ProtocolSettings

                    var committeeScript = Contract.CreateMultiSigRedeemScript(ProtocolSettings.StandbyCommittee.Count - (ProtocolSettings.StandbyCommittee.Count - 1) / 2, ProtocolSettings.StandbyCommittee);
                    return committeeScript.ToScriptHash();
                }

                return Neo.SmartContract.Native.NativeContract.NEO.GetCommitteeAddress(Storage.Snapshot);
            }
        }

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
        public long Gas
        {
            get => Transaction.NetworkFee;
            set { Transaction.NetworkFee = value; }
        }

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
                NetworkFee = ApplicationEngine.TestModeGas,
                Signers = new Signer[]
                {
                    new()
                    {
                        // ValidatorsAddress
                        Account = validatorsScript.ToScriptHash(),
                        Scopes = WitnessScope.Global
                    },
                    new()
                    {
                        // CommitteeAddress
                        Account = committeeScript.ToScriptHash(),
                        Scopes = WitnessScope.Global
                    }
                },
                Witnesses = System.Array.Empty<Witness>() // Not required
            };

            if (initializeNativeContracts)
            {
                Native.Initialize(false);
            }
        }

        #region Invoke events

        internal void ApplicationEngineNotify(object? sender, NotifyEventArgs e)
        {
            if (_contracts.TryGetValue(e.ScriptHash, out var contracts))
            {
                foreach (var contract in contracts)
                {
                    contract.InvokeOnNotify(e.EventName, e.State);
                }
            }
        }

        internal void ApplicationEngineLog(object? sender, LogEventArgs e)
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

        #region Checkpoints

        /// <summary>
        /// Get storage checkpoint
        /// </summary>
        /// <returns>EngineCheckpoint</returns>
        public EngineCheckpoint Checkpoint() => Storage.Checkpoint();

        /// <summary>
        /// Restore
        /// </summary>
        /// <param name="checkpoint">Checkpoint</param>
        public void Restore(EngineCheckpoint checkpoint) => Storage.Restore(checkpoint);

        #endregion

        /// <summary>
        /// Get deploy hash
        /// </summary>
        /// <param name="nef">Nef</param>
        /// <param name="manifest">Manifest</param>
        /// <returns>Contract hash</returns>
        public UInt160 GetDeployHash(byte[] nef, string manifest)
        {
            return GetDeployHash(nef.AsSerializable<NefFile>(), ContractManifest.Parse(manifest));
        }

        /// <summary>
        /// Get deploy hash
        /// </summary>
        /// <param name="nef">Nef</param>
        /// <param name="manifest">Manifest</param>
        /// <returns>Contract hash</returns>
        public UInt160 GetDeployHash(NefFile nef, ContractManifest manifest)
        {
            return Helper.GetContractHash(Sender, nef.CheckSum, manifest.Name);
        }

        /// <summary>
        /// Deploy Smart contract
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="nef">Nef file</param>
        /// <param name="manifest">Contract manifest</param>
        /// <param name="data">Construction data</param>
        /// <param name="customMock">Custom Mock</param>
        /// <returns>Mocked Smart Contract</returns>
        public T Deploy<T>(byte[] nef, string manifest, object? data = null, Action<Mock<T>>? customMock = null) where T : SmartContract
        {
            return Deploy(nef.AsSerializable<NefFile>(), ContractManifest.Parse(manifest), data, customMock);
        }

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

            var state = Native.ContractManagement.Deploy(nef.ToArray(), Encoding.UTF8.GetBytes(manifest.ToJson().ToString(false)), data);

            if (state is null)
            {
                throw new Exception("Can't get the ContractState");
            }

            // Mock contract

            //UInt160 hash = Helper.GetContractHash(Sender, nef.CheckSum, manifest.Name);
            var ret = MockContract(state.Hash, state.Id, customMock);

            // We cache the coverage contract during `_deploy`
            // at this moment we don't have the abi stored
            // so we need to regenerate the coverage methods

            if (EnableCoverageCapture)
            {
                var coverage = GetCoverage(ret);
                coverage?.GenerateMethods(MethodDetection, state);
            }

            return ret;
        }

        /// <summary>
        /// Smart contract from Hash
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
        /// Smart contract from Hash
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

        /// <summary>
        /// Used for native artifacts only
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hash">Contract hash</param>
        /// <param name="contractId">Contract Id</param>
        /// <returns>Mocked Smart Contract</returns>
        internal T FromHash<T>(UInt160 hash, int? contractId = null) where T : SmartContract
        {
            return MockContract<T>(hash, contractId, null);
        }

        private T MockContract<T>(UInt160 hash, int? contractId = null, Action<Mock<T>>? customMock = null) where T : SmartContract
        {
            var mock = new Mock<T>(new SmartContractInitialize(this, hash, contractId))
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
                    var cm = new CustomMock(mock.Object, method);

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
                    mock.MockFunction(method.Name, args, method.ReturnType);
                }
                else
                {
                    mock.MockAction(method.Name, args);
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

            using var engine = new TestingApplicationEngine(this, TriggerType.Application, Transaction, snapshot, CurrentBlock);

            engine.LoadScript(script);

            // Clean events, if we Execute inside and execute
            // becaus it's a mock, we can register twice

            ApplicationEngine.Log -= ApplicationEngineLog;
            ApplicationEngine.Notify -= ApplicationEngineNotify;

            // Attach to static event

            ApplicationEngine.Log += ApplicationEngineLog;
            ApplicationEngine.Notify += ApplicationEngineNotify;

            // Execute

            var executionResult = engine.Execute();

            // Detach to static event

            ApplicationEngine.Log -= ApplicationEngineLog;
            ApplicationEngine.Notify -= ApplicationEngineNotify;

            // Process result

            if (executionResult != VMState.HALT)
            {
                throw engine.FaultException ?? new Exception($"Error while executing the script");
            }

            snapshot.Commit();

            if (engine.ResultStack.Count == 0) return StackItem.Null;
            return engine.ResultStack.Pop();
        }

        /// <summary>
        /// Get contract coverage
        /// </summary>
        /// <typeparam name="T">Contract</typeparam>
        /// <param name="contract">Contract</param>
        /// <returns>CoveredContract</returns>
        public CoveredContract? GetCoverage<T>(T contract) where T : SmartContract
        {
            if (!Coverage.TryGetValue(contract.Hash, out var coveredContract))
            {
                var state = Neo.SmartContract.Native.NativeContract.ContractManagement.GetContract(Storage.Snapshot, contract.Hash);
                if (state == null) return null;

                Coverage[coveredContract.Hash] = coveredContract = new(MethodDetection, contract.Hash, state);
            }

            return coveredContract;
        }

        /// <summary>
        /// Get method coverage by contract
        /// </summary>
        /// <typeparam name="T">Contract</typeparam>
        /// <param name="contract">Contract</param>
        /// <returns>CoveredContract</returns>
        public CoverageBase? GetCoverage<T>(T contract, string methodName, int pcount) where T : SmartContract
        {
            return GetCoverage(contract)?.GetCoverage(methodName, pcount);
        }

        /// <summary>
        /// Get method coverage
        /// </summary>
        /// <typeparam name="T">Contract</typeparam>
        /// <param name="contract">Contract</param>
        /// <param name="method">Method</param>
        /// <returns>CoveredContract</returns>
        public CoverageBase? GetCoverage<T>(T contract, Expression<Action<T>> method) where T : SmartContract
        {
            var coveredContract = GetCoverage(contract);
            if (coveredContract == null) return null;

            var abiMethods = AbiMethod.CreateFromExpression(method.Body)
                .Select(coveredContract.GetCoverage)
                .Where(u => u != null)
                .Cast<CoveredMethod>()
                .ToArray();

            return abiMethods.Length switch
            {
                0 => null,
                1 => abiMethods[0],
                _ => new CoveredCollection(abiMethods),
            };
        }

        /// <summary>
        /// Get method coverage
        /// </summary>
        /// <typeparam name="T">Contract</typeparam>
        /// <typeparam name="TResult">Result</typeparam>
        /// <param name="contract">Contract</param>
        /// <param name="method">Method</param>
        /// <returns>CoveredContract</returns>
        public CoverageBase? GetCoverage<T, TResult>(T contract, Expression<Func<T, TResult>> method) where T : SmartContract
        {
            var coveredContract = GetCoverage(contract);
            if (coveredContract == null) return null;

            var abiMethods = AbiMethod.CreateFromExpression(method.Body)
                .Select(coveredContract.GetCoverage)
                .Where(u => u != null)
                .Cast<CoveredMethod>()
                .ToArray();

            return abiMethods.Length switch
            {
                0 => null,
                1 => abiMethods[0],
                _ => new CoveredCollection(abiMethods),
            };
        }

        /// <summary>
        /// Set Transaction signers
        /// </summary>
        /// <param name="signers">Signers</param>
        public void SetTransactionSigners(params Signer[] signers)
        {
            Transaction.Signers = signers;
        }

        /// <summary>
        /// Set Transaction Signers using CalledByEntry
        /// </summary>
        /// <param name="signers">Signers</param>
        public void SetTransactionSigners(params UInt160[] signers)
        {
            Transaction.Signers = signers.Select(u => new Signer() { Account = u, Scopes = WitnessScope.CalledByEntry }).ToArray();
        }

        /// <summary>
        /// Set Transaction Signers
        /// </summary>
        /// <param name="scope">Scope</param>
        /// <param name="signers">Signers</param>
        public void SetTransactionSigners(WitnessScope scope, params UInt160[] signers)
        {
            Transaction.Signers = signers.Select(u => new Signer() { Account = u, Scopes = scope }).ToArray();
        }

        /// <summary>
        /// Generate a random new Signers with CalledByEntry scope by default
        /// </summary>
        /// <param name="scope">Witness scope</param>
        /// <returns>Signer</returns>
        public static Signer GetNewSigner(WitnessScope scope = WitnessScope.CalledByEntry)
        {
            var rand = new Random();
            var data = new byte[UInt160.Length];
            rand.NextBytes(data);

            // Ensure that if we convert to BigInteger this value will work

            if (data[0] == 0) data[0] = 1;

            return new Signer()
            {
                Account = new UInt160(data),
                Scopes = scope,
            };
        }
    }
}
