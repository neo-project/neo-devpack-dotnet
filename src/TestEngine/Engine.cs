using Neo.Cryptography.ECC;
using Neo.IO.Json;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Collections.Generic;

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
        private byte[] PubKey => HexString2Bytes("03ea01cb94bdaf0cd1c01b159d474f9604f4af35a3e2196f6bdfdb33b2aa4961fa");

        private Engine()
        {
            Reset();
        }

        public int BlockCount => ((TestDataCache<UInt256, TrimmedBlock>)engine.Snapshot.Blocks).Count();

        public void Reset()
        {
            engine = SetupNativeContracts();
        }

        public void SetTestEngine(string path)
        {
            engine.AddEntryScript(path);
            var manifest = ContractManifest.FromJson(JObject.Parse(engine.ScriptEntry.finalManifest));

            engine.Snapshot.Contracts.Add(manifest.Hash, new Neo.Ledger.ContractState()
            {
                Script = engine.ScriptEntry.finalNEF,
                Manifest = manifest,
            });
        }

        public void AddSmartContract(string path)
        {
            var builtScript = engine.Build(path);
            if (UInt160.TryParse(builtScript.finalABI["hash"].AsString(), out var hash))
            {
                engine.Snapshot.Contracts.Add(hash, new Ledger.ContractState()
                {
                    Script = builtScript.finalNEF,
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
            }
            else
            {
                newBlock = CreateBlock();
            }

            while (blocks.Count() < newHeight)
            {
                var hash = newBlock.Hash;
                var trim = newBlock.Trim();
                blocks.AddForTest(hash, trim);
                lastBlock = newBlock;
                newBlock = CreateBlock();
            }

            var index = (uint)(blocks.Count() - 1);
            snapshot.SetCurrentBlockHash(index, lastBlock.Hash);
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

        public JObject Run(string method, StackItem[] args)
        {
            engine.GetMethod(method).RunEx(args);
            return engine.ToJson();
        }

        private TestEngine SetupNativeContracts()
        {
            SetConsensus();
            var block = Blockchain.GenesisBlock;
            TestEngine engine = new TestEngine(TriggerType.Application, block);
            ((TestSnapshot)engine.Snapshot).SetPersistingBlock(block);

            using (var script = new ScriptBuilder())
            {
                script.EmitSysCall(TestEngine.Native_Deploy);
                engine.LoadScript(script.ToArray());
                engine.Execute();
            }
            engine.ClearNotifications();
            ((TestSnapshot)engine.Snapshot).ClearStorage();

            return engine;
        }

        private void SetConsensus()
        {
            var _ = TestBlockchain.TheNeoSystem;
            var store = Blockchain.Singleton.Store;
            var block = Blockchain.GenesisBlock;
        }

        private Block CreateBlock()
        {
            var blocks = engine.Snapshot.Blocks.Seek().GetEnumerator();
            while (blocks.MoveNext())
            { }

            var (blockHash, trimmedBlock) = blocks.Current;
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
