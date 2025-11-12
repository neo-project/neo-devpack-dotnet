using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts;

public class Contract_RefLocals : SmartContract.Framework.SmartContract
{
    private static int _counter;

    public static int IncrementViaRefLocal(int value)
    {
        ref int alias = ref value;
        alias += 5;
        return value;
    }

    public static int RebindRefLocal(int first, int second)
    {
        ref int alias = ref first;
        alias = ref second;
        alias += 2;
        return second;
    }

    public static int RewriteArrayElement(int index)
    {
        int[] values = new[] { 3, 5, 7 };
        ref int alias = ref values[index];
        alias = 99;
        return values[index];
    }

    public static int UpdateInstanceField(int start)
    {
        var holder = new Holder { Value = start };
        ref int alias = ref holder.Value;
        alias += 4;
        return holder.Value;
    }

    public static int UpdateStaticField(int start)
    {
        _counter = start;
        ref int alias = ref _counter;
        alias++;
        return _counter;
    }

    public static int UpdateNestedHolder(int start)
    {
        var wrapper = new Wrapper { Inner = new Holder { Value = start } };
        ref int alias = ref wrapper.Inner.Value;
        alias += 3;
        return wrapper.Inner.Value;
    }

    private sealed class Holder
    {
        public int Value;
    }

    private sealed class Wrapper
    {
        public Holder Inner = new();
    }
}
