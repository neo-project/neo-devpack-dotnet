using Neo.Cryptography.ECC;
using Neo.IO.Json;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using Neo.VM.Types;
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
            var block = new Block()
            {
                Index = 0,
                ConsensusData = new ConsensusData(),
                Transactions = new Transaction[0],
                Witness = new Witness()
                {
                    InvocationScript = new byte[0],
                    VerificationScript = Contract.CreateSignatureRedeemScript(ECPoint.FromBytes(PubKey, ECCurve.Secp256k1))
                },
                NextConsensus = UInt160.Zero,
                MerkleRoot = UInt256.Zero,
                PrevHash = UInt256.Zero
            };

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
