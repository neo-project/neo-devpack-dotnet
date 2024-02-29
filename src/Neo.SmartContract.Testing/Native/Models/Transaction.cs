using Neo.SmartContract.Testing.Attributes;

namespace Neo.SmartContract.Testing.Native.Models
{
    public class Transaction
    {
        [FieldOrder(0)]
        public UInt256? Hash { get; set; }

        [FieldOrder(1)]
        public byte Version { get; set; }

        [FieldOrder(2)]
        public uint Nonce { get; set; }

        [FieldOrder(3)]
        public UInt160? Sender { get; set; }

        [FieldOrder(4)]
        public long SystemFee { get; set; }

        [FieldOrder(5)]
        public long NetworkFee { get; set; }

        [FieldOrder(6)]
        public uint ValidUntilBlock { get; set; }

        [FieldOrder(7)]
        public byte[]? Script { get; set; }
    }
}
