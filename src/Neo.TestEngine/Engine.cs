using Neo.Cryptography.ECC;
using Neo.IO;
using Neo.IO.Json;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Native;
using Neo.VM;
using System.Collections.Generic;
using System.Linq;

namespace Neo.TestingEngine
{
    public class Engine
    {
        private static Engine instance = null;
        public static Engine Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Engine();
                }
                return instance;
            }
        }

        private TestEngine engine = null;
        private Transaction currentTx = null;
        private byte[] PubKey => HexString2Bytes("03ea01cb94bdaf0cd1c01b159d474f9604f4af35a3e2196f6bdfdb33b2aa4961fa");

        private Engine()
        {
            var _ = TestBlockchain.TheNeoSystem;
            Reset();
        }

        public uint Height => NativeContract.Ledger.CurrentIndex(engine.Snapshot);

        public DataCache Snaptshot => engine.Snapshot;

        public void Reset()
        {
            engine = SetupNativeContracts();
        }

        public void SetEntryScript(string path)
        {
            engine.AddEntryScript(path);
            var manifest = ContractManifest.FromJson(JObject.Parse(engine.ScriptEntry.finalManifest));
            var hash = engine.ScriptEntry.finalNEFScript.ToScriptHash();

            if (engine.Snapshot.ContainsContract(hash))
            {
                engine.Snapshot.DeleteContract(hash);
            }

            engine.Snapshot.ContractAdd(new ContractState()
            {
                Hash = hash,
                Nef = engine.ScriptEntry.nefFile,
                Manifest = manifest,
            });
        }

        public void SetEntryScript(UInt160 contractHash)
        {
            engine.AddEntryScript(contractHash);
        }

        public void AddSmartContract(TestContract contract)
        {
            var script = AddSmartContract(contract.nefPath);
            contract.nefFile = script.nefFile;
        }

        private BuildScript AddSmartContract(string path)
        {
            var builtScript = engine.Build(path);
            var hash = builtScript.finalNEFScript.ToScriptHash();

            var snapshot = engine.Snapshot;

            if (!snapshot.ContainsContract(hash))
            {
                snapshot.TryContractAdd(new ContractState()
                {
                    Hash = hash,
                    Nef = builtScript.nefFile,
                    Manifest = ContractManifest.FromJson(JObject.Parse(builtScript.finalManifest)),
                });
            }

            return builtScript;
        }

        public void IncreaseBlockCount(uint newHeight)
        {
            var snapshot = (TestDataCache)engine.Snapshot;
            if (snapshot.Blocks().Count <= newHeight)
            {
                Block newBlock;
                Block lastBlock = null;
                if (snapshot.Blocks().Count == 0)
                {
                    newBlock = Blockchain.GenesisBlock;
                    snapshot.AddOrUpdateTransactions(newBlock.Transactions, newBlock.Index);
                }
                else
                {
                    newBlock = CreateBlock();
                }

                while (snapshot.Blocks().Count <= newHeight)
                {
                    var hash = newBlock.Hash;
                    var trim = newBlock.Trim();
                    snapshot.BlocksAddOrUpdate(hash, trim);
                    lastBlock = newBlock;
                    newBlock = CreateBlock();
                }

                snapshot.SetCurrentBlockHash(lastBlock.Index, lastBlock.Hash);
            }
        }

        public void SetStorage(Dictionary<StorageKey, StorageItem> storage)
        {
            if (engine.Snapshot is TestDataCache snapshot)
            {
                foreach (var (key, value) in storage)
                {
                    snapshot.AddForTest(key, value);
                }
            }
        }

        public void SetSigners(UInt160[] signerAccounts)
        {
            if (signerAccounts.Length > 0)
            {
                currentTx.Signers = signerAccounts.Select(p => new Signer() { Account = p, Scopes = WitnessScope.CalledByEntry }).ToArray();
            }
        }

        public void AddBlock(Block block)
        {
            if (engine.Snapshot is TestDataCache snapshot)
            {
                Block currentBlock = null;
                if (Height < block.Index || snapshot.Blocks().Count == 0)
                {
                    IncreaseBlockCount(block.Index);
                    currentBlock = engine.Snapshot.GetLastBlock();
                }
                else
                {
                    currentBlock = NativeContract.Ledger.GetBlock(snapshot, block.Index);
                }

                if (currentBlock != null)
                {
                    var hash = currentBlock.Hash;
                    currentBlock.Timestamp = block.Timestamp;

                    if (currentBlock.Transactions.Length > 0)
                    {
                        var tx = currentBlock.Transactions.ToList();
                        tx.AddRange(block.Transactions);
                        currentBlock.Transactions = tx.ToArray();
                    }
                    else
                    {
                        currentBlock.Transactions = block.Transactions;
                    }

                    foreach (var tx in block.Transactions)
                    {
                        tx.ValidUntilBlock = block.Index + Transaction.MaxValidUntilBlockIncrement;
                    }

                    var trimmed = currentBlock.Trim();
                    snapshot.UpdateChangedBlocks(hash, trimmed.Hash, trimmed);
                }

                snapshot.AddOrUpdateTransactions(block.Transactions);
            }
        }

        public JObject Run(string method, ContractParameter[] args)
        {
            if (engine.Snapshot is TestDataCache snapshot)
            {
                if (snapshot.Blocks().Count == 0)
                {
                    IncreaseBlockCount(0);
                }
                var lastBlock = snapshot.GetLastBlock();

                engine.PersistingBlock.Index = lastBlock.Index;
                engine.PersistingBlock.Timestamp = lastBlock.Timestamp;
                engine.PersistingBlock.PrevHash = lastBlock.PrevHash;
                engine.PersistingBlock.ConsensusData = lastBlock.ConsensusData;
                engine.PersistingBlock.Transactions = lastBlock.Transactions;
                engine.PersistingBlock.Witness = lastBlock.Witness;
                engine.PersistingBlock.NextConsensus = lastBlock.NextConsensus;
                engine.PersistingBlock.MerkleRoot = lastBlock.MerkleRoot;

                currentTx.ValidUntilBlock = lastBlock.Index;
                snapshot.SetCurrentBlockHash(lastBlock.Index, lastBlock.Hash);
            }

            using (ScriptBuilder scriptBuilder = new ScriptBuilder())
            {
                scriptBuilder.EmitAppCall(engine.EntryScriptHash, method, args);
                currentTx.Script = scriptBuilder.ToArray();
            }

            var stackItemsArgs = args.Select(a => a.ToStackItem()).ToArray();
            if (engine.ScriptEntry is BuildNative)
            {
                engine.RunNativeContract(method, stackItemsArgs);
            }
            else
            {
                engine.GetMethod(method).RunEx(stackItemsArgs);
            }

            //currentTx.ValidUntilBlock = engine.Snapshot.Height + Transaction.MaxValidUntilBlockIncrement;
            currentTx.SystemFee = engine.GasConsumed;
            UInt160[] hashes = currentTx.GetScriptHashesForVerifying(engine.Snapshot);

            // base size for transaction: includes const_header + signers + attributes + script + hashes
            int size = Transaction.HeaderSize + currentTx.Signers.GetVarSize() + currentTx.Attributes.GetVarSize() + currentTx.Script.GetVarSize() + IO.Helper.GetVarSize(hashes.Length);
            currentTx.NetworkFee += size * NativeContract.Policy.GetFeePerByte(engine.Snapshot);

            return engine.ToJson();
        }

        private TestEngine SetupNativeContracts()
        {
            currentTx = new Transaction()
            {
                Attributes = new TransactionAttribute[0],
                Script = new byte[0],
                Signers = new Signer[] { new Signer() { Account = UInt160.Zero } },
                Witnesses = new Witness[0],
                NetworkFee = 1,
                Nonce = 2,
                SystemFee = 3,
                Version = 4
            };
            TestEngine engine = new TestEngine(TriggerType.Application, currentTx, new TestDataCache(), persistingBlock: new Block() { Index = 0 });

            engine.ClearNotifications();
            return engine;
        }

        private Block CreateBlock(Block originBlock = null)
        {
            TrimmedBlock trimmedBlock = null;
            var blocks = engine.Snapshot.Blocks();
            if (blocks.Count > 0)
            {
                trimmedBlock = blocks.Last();
            }

            if (trimmedBlock == null)
            {
                trimmedBlock = Blockchain.GenesisBlock.Trim();
            }

            var newBlock = new Block()
            {
                Index = trimmedBlock.Index + 1,
                Timestamp = trimmedBlock.Timestamp + Blockchain.MillisecondsPerBlock,
                ConsensusData = new ConsensusData(),
                Transactions = new Transaction[0],
                Witness = new Witness()
                {
                    InvocationScript = new byte[0],
                    VerificationScript = Contract.CreateSignatureRedeemScript(ECPoint.FromBytes(PubKey, ECCurve.Secp256k1))
                },
                NextConsensus = trimmedBlock.NextConsensus,
                MerkleRoot = trimmedBlock.MerkleRoot,
                PrevHash = trimmedBlock.Hash
            };

            if (originBlock != null)
            {
                newBlock.Timestamp = originBlock.Timestamp;
            }

            return newBlock;
        }

        private static byte[] HexString2Bytes(string str)
        {
            if (str.IndexOf("0x") == 0)
                str = str.Substring(2);
            byte[] outd = new byte[str.Length / 2];
            for (var i = 0; i < str.Length / 2; i++)
            {
                outd[i] = byte.Parse(str.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return outd;
        }
    }
}
