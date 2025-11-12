using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts;

public class Contract_RefSupport : SmartContract.Framework.SmartContract
{
    private static int _counter;

    public static int IncrementRefLocal(int value)
    {
        Increment(ref value);
        return value;
    }

    public static int IncrementRefStaticField(int value)
    {
        _counter = value;
        Increment(ref _counter);
        return _counter;
    }

    public static int IncrementRefInstanceField(int value)
    {
        var holder = new Holder { Value = value };
        Increment(ref holder.Value);
        return holder.Value;
    }

    public static int ProduceOutValue()
    {
        GetValue(out int value);
        return value;
    }

    public static int ProduceOutStaticField()
    {
        _counter = 0;
        GetValue(out _counter);
        return _counter;
    }

    public static int ProduceOutInstanceField()
    {
        var holder = new Holder();
        GetValue(out holder.Value);
        return holder.Value;
    }

    public static int SwapDigits(int first, int second)
    {
        Swap(ref first, ref second);
        return first * 10 + second;
    }

    private static void Increment(ref int value) => value++;
    private static void GetValue(out int value) => value = 123;

    private static void Swap(ref int left, ref int right)
    {
        int temp = left;
        left = right;
        right = temp;
    }

    private sealed class Holder
    {
        public int Value;
    }
}
