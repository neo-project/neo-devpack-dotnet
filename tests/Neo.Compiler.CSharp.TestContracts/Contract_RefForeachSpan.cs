using Neo.SmartContract.Framework;
using System;

namespace Neo.Compiler.CSharp.TestContracts;

public class Contract_RefForeachSpan : SmartContract.Framework.SmartContract
{
    public static int IncrementAll(int first, int second, int third)
    {
        int[] values = new[] { first, second, third };
        Span<int> span = values;
        foreach (ref var value in span)
        {
            value += 2;
        }

        int total = 0;
        foreach (ref var value in span)
        {
            total += value;
        }

        return total;
    }

    public static int IncrementArrayValues(int first, int second, int third)
    {
        int[] values = new[] { first, second, third };
        Span<int> span = values;
        int index = 0;
        foreach (ref var value in span)
        {
            if (index++ == 0)
                continue;
            value++;
        }

        return values[0] + values[1] + values[2];
    }

    public static int SegmentDouble(int first, int second)
    {
        int[] values = new[] { first, second };
        Span<int> span = values;
        foreach (ref var value in span)
        {
            value *= 2;
        }

        int result = 0;
        foreach (ref var value in span)
        {
            result += value;
        }

        return result;
    }
}
