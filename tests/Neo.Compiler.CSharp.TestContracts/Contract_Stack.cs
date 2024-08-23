using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts;

public class Contract_Stack : SmartContract.Framework.SmartContract
{
    public static BigInteger Test_Push_Integer(BigInteger value)
    {
        return value;
    }

    public static (byte, byte, sbyte, sbyte, short, short, ushort, uint, int, int, ulong, long, long) Test_Push_Integer_Internal()
    {
        return (byte.MinValue, byte.MaxValue, sbyte.MinValue, sbyte.MaxValue, short.MinValue, short.MaxValue, ushort.MaxValue, uint.MaxValue, int.MinValue, int.MaxValue, ulong.MaxValue, long.MinValue, long.MaxValue);
    }

    public static (int, int, long) Test_External()
    {
        const int minusOne = -1;
        const int intValue = -1000000;
        const long longValue = -1000000000000;

        return (minusOne, intValue, longValue);
    }
}
