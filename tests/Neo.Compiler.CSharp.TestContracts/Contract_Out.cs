// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_Out.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts;

public class Contract_Out : SmartContract.Framework.SmartContract
{
    private sealed class Holder
    {
        public int Value;
        public string Message = string.Empty;
        public bool Flag;
    }

    private static int _cachedResult;

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
        BasicOut(out int y);
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

    public static int TestOutStaticField()
    {
        BasicOut(out _cachedResult);
        return _cachedResult;
    }

    public static (int, string, bool) TestOutNamedArguments()
    {
        MultipleOut(b: out string message, c: out bool flag, a: out int value);
        return (value, message, flag);
    }

    public static (int, string, bool) TestOutInstanceField()
    {
        var holder = new Holder();
        MultipleOut(out holder.Value, out holder.Message, out holder.Flag);
        return (holder.Value, holder.Message, holder.Flag);
    }

    private static int NextValue(ref int state)
    {
        state++;
        return state;
    }

    private static string NextText(ref int state)
    {
        state++;
        return "X";
    }

    private static void IncrementTwice(ref int first, ref int second)
    {
        first++;
        second++;
    }

    private static void SetTwice(out int first, out int second)
    {
        first = 1;
        second = first + 1;
    }

    public static int TestRefLocalAfterCoalesce(bool evaluateRight)
    {
        int calls = 0;
        string? value = evaluateRight ? null : "Child";
        int length = (value ?? NextText(ref calls)).Length;
        return length * 100 + calls;
    }

    private static int ParameterAfterCoalesce(string value, int calls)
    {
        int length = (value ?? NextText(ref calls)).Length;
        return length * 100 + calls;
    }

    public static int TestRefParameterAfterCoalesce()
    {
        return ParameterAfterCoalesce("Child", 7);
    }

    public static int TestRefAfterConditionalBranch(bool execute)
    {
        int calls = 3;
        if (execute)
            calls = NextValue(ref calls);
        return calls;
    }

    public static int TestRefAliases()
    {
        int value = 0;
        IncrementTwice(ref value, ref value);
        return value;
    }

    public static int TestOutAliases()
    {
        int value = 0;
        SetTwice(out value, out value);
        return value;
    }

    public static int TestDistinctRefArguments()
    {
        int first = 0;
        int second = 10;
        IncrementTwice(ref first, ref second);
        return first * 100 + second;
    }

    public static int TestMixedRefAliases()
    {
        int alias = 0;
        int first = 0;
        int second = 10;
        IncrementTwice(ref alias, ref alias);
        IncrementTwice(ref first, ref second);
        return alias * 10000 + first * 100 + second;
    }

    public static int TestStaticFieldRefAliases()
    {
        _cachedResult = 0;
        IncrementTwice(ref _cachedResult, ref _cachedResult);
        return _cachedResult;
    }

    public static int TestInstanceFieldRefAliases()
    {
        var holder = new Holder();
        IncrementTwice(ref holder.Value, ref holder.Value);
        return holder.Value;
    }

    public static int TestDistinctInstanceFieldRefs()
    {
        var first = new Holder();
        var second = new Holder { Value = 10 };
        IncrementTwice(ref first.Value, ref second.Value);
        return first.Value * 100 + second.Value;
    }

    private static int RecurseRefAliases(ref int first, ref int second, int depth)
    {
        if (depth == 0)
            return first * 10 + second;

        first += 2;
        second += 3;
        return RecurseRefAliases(ref first, ref first, depth - 1) + 1;
    }

    public static int TestRecursiveRefAliases()
    {
        int state = 1;
        int result = RecurseRefAliases(ref state, ref state, 2);
        return result * 100 + state;
    }

    private class VirtualRefBase
    {
        public virtual int BumpTwice(ref int first, ref int second)
        {
            first += 2;
            second += 3;
            return first * 10 + second;
        }
    }

    private class VirtualRefDerived : VirtualRefBase
    {
        public override int BumpTwice(ref int first, ref int second)
        {
            first += 5;
            second += 7;
            return first * 10 + second;
        }

        public virtual int Extra() => 1;
    }

    public static int TestVirtualRefAliases()
    {
        VirtualRefBase value = new VirtualRefDerived();
        int state = 1;
        int result = value.BumpTwice(ref state, ref state);
        return result * 100 + state;
    }

    public static int TestVirtualDistinctRefArguments()
    {
        VirtualRefBase value = new VirtualRefDerived();
        int first = 1;
        int second = 10;
        int result = value.BumpTwice(ref first, ref second);
        return result * 1000 + first * 100 + second;
    }

    public static int TestVirtualRefAliasesThenDistinctCalls()
    {
        VirtualRefBase value = new VirtualRefDerived();
        int alias = 1;
        int first = 1;
        int second = 10;
        int aliasResult = value.BumpTwice(ref alias, ref alias);
        int distinctResult = value.BumpTwice(ref first, ref second);
        return aliasResult * 1000000 + alias * 10000 + distinctResult * 1000 + first * 100 + second;
    }

    public static int TestVirtualRefAliasWritebackIsolation()
    {
        VirtualRefBase value = new VirtualRefDerived();
        int alias = 1;
        int first = 1;
        int second = 10;
        value.BumpTwice(ref alias, ref alias);
        value.BumpTwice(ref first, ref second);
        return alias * 1000000 + first * 1000 + second;
    }

    private class VirtualMixBase
    {
        public virtual int Mix(ref int first, ref int second, ref int third)
        {
            first += 2;
            second += 3;
            third += 5;
            return first * 100 + second * 10 + third;
        }
    }

    private sealed class VirtualMixDerived : VirtualMixBase
    {
        public override int Mix(ref int first, ref int second, ref int third)
        {
            first += 5;
            second += 7;
            third += 11;
            return first * 100 + second * 10 + third;
        }
    }

    public static int TestVirtualRefAliasSignatures()
    {
        VirtualMixBase value = new VirtualMixDerived();
        int state = 1;
        int other = 20;
        int firstResult = value.Mix(ref state, ref state, ref other);
        int secondResult = value.Mix(ref state, ref other, ref other);
        return firstResult * 1000000 + secondResult * 1000 + state * 100 + other;
    }

    private class VirtualBaseCallBase
    {
        public virtual int BumpTwice(ref int first, ref int second)
        {
            first += 2;
            second += 3;
            return first * 10 + second;
        }

        public virtual int Recurse(ref int first, ref int second, int depth)
        {
            if (depth == 0)
                return first * 10 + second;
            first += 2;
            second += 3;
            return Recurse(ref first, ref first, depth - 1) + 1;
        }
    }

    private sealed class VirtualBaseCallDerived : VirtualBaseCallBase
    {
        public override int BumpTwice(ref int first, ref int second)
        {
            int result = base.BumpTwice(ref first, ref second);
            first += 5;
            second += 7;
            return result * 100 + first * 10 + second;
        }

        public override int Recurse(ref int first, ref int second, int depth)
        {
            if (depth == 0)
                return first * 100 + second * 10 + first;
            first += 5;
            second += 7;
            return base.Recurse(ref first, ref first, depth - 1) + first;
        }
    }

    public static int TestVirtualRefAliasesThroughBaseCall()
    {
        VirtualBaseCallBase value = new VirtualBaseCallDerived();
        int state = 1;
        int result = value.BumpTwice(ref state, ref state);
        return result * 100 + state;
    }

    public static int TestVirtualRecursiveRefAliasesThroughBaseCall()
    {
        VirtualBaseCallBase value = new VirtualBaseCallDerived();
        int state = 1;
        int result = value.Recurse(ref state, ref state, 2);
        return result * 100 + state;
    }

    private class VirtualOutBase
    {
        public virtual int SetValues(out int first, out int second)
        {
            first = 2;
            second = 3;
            return first * 10 + second;
        }
    }

    private class VirtualOutMiddle : VirtualOutBase
    {
        public override int SetValues(out int first, out int second)
        {
            int result = base.SetValues(out first, out second);
            first += 5;
            second += 7;
            return result * 100 + first * 10 + second;
        }
    }

    private sealed class VirtualOutDerived : VirtualOutMiddle
    {
        public override int SetValues(out int first, out int second)
        {
            int result = base.SetValues(out first, out second);
            first += 11;
            second += 13;
            return result * 100 + first * 10 + second;
        }
    }

    public static int TestVirtualOutArguments()
    {
        VirtualOutBase value = new VirtualOutDerived();
        int first = 0;
        int second = 0;
        int result = value.SetValues(out first, out second);
        return result * 1000 + first * 100 + second;
    }
}
