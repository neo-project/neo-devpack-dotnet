using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{
    struct StructA
    {
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public byte[] from;
        public byte[] to;
        public BigInteger amount;
        public BigInteger index;
        public StructB data;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
    }

    struct StructB
    {
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
        public byte[] hash;
        public BigInteger height;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value
    }

    public class Contract_Tuple : SmartContract.Framework.SmartContract
    {
        public static (BigInteger, BigInteger, BigInteger, BigInteger) GetResult()
        {
            return (1, 2, 3, 4);
        }

        public static object T1()
        {
            var state = new StructA();
            state.data = new StructB();
            BigInteger index = 0;
            (state.amount, state.data.height, _, index) = GetResult();
            _ = GetResult();
            state.index = index;
            return state;
        }
    }
}
