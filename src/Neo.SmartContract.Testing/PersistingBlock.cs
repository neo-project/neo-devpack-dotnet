using Neo.Cryptography;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract.Native;
using Neo.VM;
using System;
using System.Linq;

namespace Neo.SmartContract.Testing
{
    public class PersistingBlock
    {
        private readonly static Script onPersistScript, postPersistScript;
        private readonly TestEngine _engine;

        static PersistingBlock()
        {
            using (ScriptBuilder sb = new())
            {
                sb.EmitSysCall(ApplicationEngine.System_Contract_NativeOnPersist);
                onPersistScript = sb.ToArray();
            }
            using (ScriptBuilder sb = new())
            {
                sb.EmitSysCall(ApplicationEngine.System_Contract_NativePostPersist);
                postPersistScript = sb.ToArray();
            }
        }

        /// <summary>
        /// Underlying block
        /// </summary>
        internal readonly Block UnderlyingBlock;

        /// <summary>
        /// Index
        /// </summary>
        public uint Index => UnderlyingBlock.Header.Index;

        /// <summary>
        /// Nonce
        /// </summary>
        public ulong Nonce
        {
            get => UnderlyingBlock.Header.Nonce;
            set => UnderlyingBlock.Header.Nonce = value;
        }

        /// <summary>
        /// Time
        /// </summary>
        public TimeSpan Timestamp => TimeSpan.FromMilliseconds(UnderlyingBlock.Header.Timestamp);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="currentBlock">Current block</param>
        public PersistingBlock(TestEngine engine, Block currentBlock)
        {
            _engine = engine;
            UnderlyingBlock = new Block()
            {
                Header = CreateNextHeader(currentBlock.Header, TimeSpan.FromSeconds(15), currentBlock.Nonce),
                Transactions = [],
            };
        }

        /// <summary>
        /// Advance
        /// </summary>
        /// <param name="elapsed">Elapsed time</param>
        public void Advance(TimeSpan elapsed)
        {
            UnderlyingBlock.Header.Timestamp += (ulong)elapsed.TotalMilliseconds;
        }

        /// <summary>
        /// Skip blocks
        /// </summary>
        /// <param name="count">Count</param>
        /// <param name="elapsed">Elapsed</param>
        public void Skip(uint count, TimeSpan elapsed)
        {
            UnderlyingBlock.Header.Index += count;
            UnderlyingBlock.Header.Timestamp += (ulong)elapsed.TotalMilliseconds;
        }

        /// <summary>
        /// Persist block
        /// </summary>
        /// <returns>Persisted block</returns>
        public Block Persist()
        {
            return Persist([], []);
        }

        /// <summary>
        /// Persist block
        /// </summary>
        /// <param name="tx">Transaction</param>
        /// <param name="state">State</param>
        /// <returns>Persisted block</returns>
        public Block Persist(Transaction tx, VMState state = VMState.HALT)
        {
            return Persist([tx], [state]);
        }

        /// <summary>
        /// Persist block
        /// </summary>
        /// <param name="txs">Transactions</param>
        /// <param name="states">States</param>
        /// <returns>Persisted block</returns>
        public Block Persist(Transaction[] txs, VMState[] states)
        {
            if (txs.Length != states.Length)
            {
                throw new ArgumentException("Transactions count and states count are different");
            }

            // Build block

            Block persist = new()
            {
                Header = UnderlyingBlock.Header,
                Transactions = txs,
            };
            persist.Header.MerkleRoot = MerkleTree.ComputeRoot(txs.Select(p => p.Hash).ToArray());

            // Invoke OnPersist

            DataCache clonedSnapshot = _engine.Storage.Snapshot.CloneCache();

            using (var engine = new TestingApplicationEngine(_engine, TriggerType.OnPersist, persist, clonedSnapshot, persist))
            {
                engine.LoadScript(onPersistScript);
                if (engine.Execute() != VMState.HALT)
                    throw new Exception($"Error executing OnPersist");
            }

            // Invoke PostPersist

            using (var engine = new TestingApplicationEngine(_engine, TriggerType.PostPersist, persist, clonedSnapshot, persist))
            {
                engine.LoadScript(postPersistScript);
                if (engine.Execute() != VMState.HALT)
                    throw new Exception($"Error executing PostPersist");
            }

            // Update states

            const byte prefix_Transaction = 11;

            for (int x = 0; x < txs.Length; x++)
            {
                var transactionState = clonedSnapshot.TryGet(new KeyBuilder(_engine.Native.Ledger.Storage.Id, prefix_Transaction).Add(txs[x].Hash));
                transactionState.GetInteroperable<TransactionState>().State = states[x];
            }

            // Commit changes and return block

            UnderlyingBlock.Header = CreateNextHeader(persist.Header, TimeSpan.FromSeconds(15), persist.Nonce);
            clonedSnapshot.Commit();

            return persist;
        }

        /// <summary>
        /// Create next header
        /// </summary>
        /// <param name="previous">Previous</param>
        /// <param name="elapsed">Elapsed</param>
        /// <param name="nonce">Nonce</param>
        /// <returns>Header</returns>
        private static Header CreateNextHeader(Header previous, TimeSpan elapsed, ulong nonce = 0)
        {
            return new Header()
            {
                Version = previous.Version,
                Index = previous.Index + 1,
                MerkleRoot = UInt256.Zero,
                NextConsensus = previous.NextConsensus,
                Nonce = nonce,
                PrevHash = previous.Hash,
                PrimaryIndex = previous.PrimaryIndex,
                Timestamp = previous.Timestamp + (ulong)elapsed.TotalMilliseconds,
                Witness = new Witness()
                {
                    InvocationScript = Array.Empty<byte>(),
                    VerificationScript = Array.Empty<byte>(),
                }
            };
        }
    }
}
