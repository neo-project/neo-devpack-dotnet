using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    struct StructA
    {
        public byte[] from;
        public byte[] to;
        public BigInteger amount;
        public BigInteger index;
        public StructB data;
    }

    struct StructB
    {
        public byte[] hash;
        public BigInteger height;
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
            state.index = index;
            return state;
        }
    }
}
