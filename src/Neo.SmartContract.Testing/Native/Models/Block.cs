using Neo.Network.P2P.Payloads;
using Neo.SmartContract.Testing.Attributes;

namespace Neo.SmartContract.Testing.Native.Models
{
    public class Block
    {
        [FieldOrder(0)]
        public UInt256 Hash { get; set; }

        [FieldOrder(1)]
        public uint Version { get; set; }

        [FieldOrder(2)]
        public UInt256 PrevHash { get; set; }

        [FieldOrder(3)]
        public UInt256 MerkleRoot { get; set; }

        [FieldOrder(4)]
        public ulong Timestamp { get; set; }

        [FieldOrder(5)]
        public ulong Nonce { get; set; }

        [FieldOrder(6)]
        public uint Index { get; set; }

        [FieldOrder(7)]
        public byte PrimaryIndex { get; set; }

        [FieldOrder(8)]
        public UInt160 NextConsensus { get; set; }

        [FieldOrder(9)]
        public int TransactionsCount { get; set; }

        /// <summary>
        /// Convert to Header
        /// </summary>
        /// <returns>Header</returns>
        internal Header AsHeader()
        {
            return new Header()
            {
                Version = Version,
                PrevHash = PrevHash,
                MerkleRoot = MerkleRoot,
                Timestamp = Timestamp,
                Index = Index,
                NextConsensus = NextConsensus,
                Nonce = Nonce,
                PrimaryIndex = PrimaryIndex,
                Witness = null
            };
        }
    }
}
