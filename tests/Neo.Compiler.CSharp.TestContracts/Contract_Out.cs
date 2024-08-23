using System;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts;

public class Contract_Out : SmartContract.Framework.SmartContract
{
    // Basic out parameter usage
    private static void BasicOut(out int result)
    {
        result = 42;
    }

    // Multiple out parameters
    private static void MultipleOut(out int a, out string b, out bool c)
    {
        a = 10;
        b = "Hello";
        c = true;
    }

    // Using out var declaration
    public static int TestOutVar()
    {
        BasicOut(out var x);
        return x;
    }

    // Using existing variable
    public static int TestExistingVar()
    {
        int y;
        BasicOut(out y);
        return y;
    }

    // Using multiple out parameters
    public static string TestMultipleOut()
    {
        MultipleOut(out int a, out string b, out bool c);
        return $"{a}, {b}, {c}";
    }

    // Using out parameters with discard
    public static void TestOutDiscard()
    {
        MultipleOut(out _, out var str, out _);
        // Use str here
    }

    // Using out parameters in loops
    public static int TestOutInLoop()
    {
        int sum = 0;
        for (int i = 0; i < 5; i++)
        {
            BasicOut(out int temp);
            sum += temp;
        }
        return sum;
    }

    // Using out parameters with conditional statements
    public static string TestOutConditional(bool flag)
    {
        if (flag)
        {
            BasicOut(out int result);
            return result.ToString();
        }
        else
        {
            MultipleOut(out _, out string str, out _);
            return str;
        }
    }

    // Using out parameters with switch statements
    public static int TestOutSwitch(int option)
    {
        switch (option)
        {
            case 1:
                BasicOut(out int result1);
                return result1;
            case 2:
                MultipleOut(out int result2, out _, out _);
                return result2;
            default:
                return -1;
        }
    }

    // Using out parameters in nested method calls
    public static (int, int) TestNestedOut()
    {
        var res = ProcessNestedOut(out int x);
        return (res, x);
    }

    private static int ProcessNestedOut(out int x)
    {
        BasicOut(out x);
        return x * 2;
    }
}
