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
using Neo.VM.Types;
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
            Reset();
        }

        public uint Height => engine.Snapshot.Height;

        public StoreView Snaptshot => engine.Snapshot;

        public void Reset()
        {
            engine = SetupNativeContracts();
            IncreaseBlockCount(0);
        }

        public void SetTestEngine(string path)
        {
            engine.AddEntryScript(path);
            var manifest = ContractManifest.FromJson(JObject.Parse(engine.ScriptEntry.finalManifest));
            var hash = engine.ScriptEntry.finalNEFScript.ToScriptHash();

            engine.Snapshot.ContractAdd(new ContractState()
            {
                Hash = hash,
                Script = engine.ScriptEntry.finalNEFScript,
                Manifest = manifest,
            });
        }

        public void AddSmartContract(string path)
        {
            var builtScript = engine.Build(path);
            var hash = builtScript.finalNEFScript.ToScriptHash();

            var snapshot = engine.Snapshot;

            if (!snapshot.ContainsContract(hash))
            {
                snapshot.ContractAdd(new ContractState()
                {
                    Hash = hash,
                    Script = builtScript.finalNEFScript,
                    Manifest = ContractManifest.FromJson(JObject.Parse(builtScript.finalManifest)),
                });
            }
        }

        public void IncreaseBlockCount(uint newHeight)
        {
            var snapshot = (TestSnapshot)engine.Snapshot;
            var blocks = (TestDataCache<UInt256, TrimmedBlock>)snapshot.Blocks;
            Block newBlock;
            Block lastBlock = null;
            if (blocks.Count() == 0)
            {
                newBlock = Blockchain.GenesisBlock;
                snapshot.AddTransactions(newBlock.Transactions, newBlock.Index);
            }
            else
            {
                newBlock = CreateBlock();
            }

            while (blocks.Count() <= newHeight)
            {
                var hash = newBlock.Hash;
                var trim = newBlock.Trim();
                blocks.AddForTest(hash, trim);
                lastBlock = newBlock;
                newBlock = CreateBlock();
            }

            var index = (uint)(blocks.Count() - 1);
            snapshot.SetCurrentBlockHash(index, lastBlock?.Hash ?? newBlock.Hash);
        }

        public void SetStorage(Dictionary<PrimitiveType, StackItem> storage)
        {
            foreach (var data in storage)
            {
                var key = new StorageKey()
                {
                    Key = data.Key.GetSpan().ToArray()
                };
                var value = new StorageItem()
                {
                    Value = data.Value.GetSpan().ToArray()
                };
                ((TestDataCache<StorageKey, StorageItem>)engine.Snapshot.Storages).AddForTest(key, value);
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
            if (engine.Snapshot is TestSnapshot snapshot)
            {
                if (snapshot.Height < block.Index)
                {
                    IncreaseBlockCount(block.Index);
                }

                var currentBlock = snapshot.TryGetBlock(block.Index);

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

                    if (snapshot.Blocks is TestDataCache<UInt256, TrimmedBlock> blocks)
                    {
                        blocks.UpdateChangingKey(hash, currentBlock.Hash, currentBlock.Trim());
                    }
                    snapshot.AddTransactions(block.Transactions);
                }
            }
        }

        public JObject Run(string method, ContractParameter[] args)
        {
            if (engine.Snapshot is TestSnapshot snapshot)
            {
                var persistingBlock = snapshot.TryGetBlock(snapshot.Height);
                snapshot.SetPersistingBlock(persistingBlock ?? Blockchain.GenesisBlock);
                currentTx.ValidUntilBlock = snapshot.Height;
            }

            using (ScriptBuilder scriptBuilder = new ScriptBuilder())
            {
                scriptBuilder.EmitAppCall(engine.EntryScriptHash, method, args);
                currentTx.Script = scriptBuilder.ToArray();
            }

            var stackItemsArgs = args.Select(a => a.ToStackItem()).ToArray();
            engine.GetMethod(method).RunEx(stackItemsArgs);

            currentTx.ValidUntilBlock = engine.Snapshot.Height + Transaction.MaxValidUntilBlockIncrement;
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
            TestEngine engine = new TestEngine(TriggerType.Application, currentTx);

            SetNative(engine);
            engine.ClearNotifications();
            ((TestSnapshot)engine.Snapshot).ClearStorage();

            return engine;
        }

        private void SetNative(TestEngine engine)
        {
            engine.Snapshot.SetPersistingBlock(new Block() { Index = 0 });
            engine.Snapshot.DeployNativeContracts();
        }

        private Block CreateBlock()
        {
            var (blockHash, trimmedBlock) = engine.Snapshot.Blocks.Seek().Last();
            if (blockHash == null)
            {
                (blockHash, trimmedBlock) = (Blockchain.GenesisBlock.Hash, Blockchain.GenesisBlock.Trim());
            }

            return new Block()
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
                PrevHash = blockHash
            };
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
