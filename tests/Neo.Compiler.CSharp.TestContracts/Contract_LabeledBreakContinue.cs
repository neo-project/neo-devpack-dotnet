// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_LabeledBreakContinue.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts;

/// <summary>
/// Exercises C# 15 labeled break/continue statements. A labeled jump exits the
/// loop (or switch) the label is attached to instead of the innermost one, and
/// every try block crossed on the way out runs its finally handler. The methods
/// accumulate into a single total so the tests can observe exactly which loop
/// levels and statements were executed.
/// </summary>
public class Contract_LabeledBreakContinue : SmartContract.Framework.SmartContract
{
    public int BreakOuterFromNestedForEach()
    {
        int total = 0;
        int[] values = new int[] { 1, 2, 3 };
        outer: foreach (int v in values)
        {
            foreach (int w in values)
            {
                if (v * w > 4)
                    break outer;
                total += v * w;
            }
            total += 100;
        }
        total += 1000;
        return total;
    }

    public int ContinueOuterFromNestedForEach()
    {
        int total = 0;
        int[] values = new int[] { 1, 2, 3 };
        outer: foreach (int v in values)
        {
            foreach (int w in values)
            {
                if (v * w > 4)
                    continue outer;
                total += v * w;
            }
            total += 100;
        }
        total += 1000;
        return total;
    }

    public int BreakLabeledWhile()
    {
        int total = 0;
        int i = 0;
        outer: while (i < 5)
        {
            i++;
            int j = 0;
            while (j < 5)
            {
                j++;
                if (i + j > 4)
                    break outer;
                total += i + j;
            }
            total += 10;
        }
        total += 100;
        return total;
    }

    public int ContinueLabeledFor()
    {
        int total = 0;
        outer: for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (i + j == 3)
                    continue outer;
                if (i + j > 4)
                    break outer;
                total += i + j;
            }
            total += 10;
        }
        total += 100;
        return total;
    }

    public int BreakLabeledSwitch(int a, int b)
    {
        int total = 0;
        outer: switch (a)
        {
            case 1:
                total += 1;
                switch (b)
                {
                    case 1:
                        total += 10;
                        break outer;
                    default:
                        total += 20;
                        break;
                }
                total += 100;
                break;
            default:
                total += 1000;
                break;
        }
        total += 10000;
        return total;
    }

    public int BreakOuterThroughTryFinally()
    {
        int total = 0;
        int[] values = new int[] { 1, 2, 3 };
        outer: foreach (int v in values)
        {
            try
            {
                if (v == 2)
                    break outer;
                total += v;
            }
            finally
            {
                total += 100;
            }
        }
        total += 1000;
        return total;
    }

    public int ContinueOuterThroughTryFinally()
    {
        int total = 0;
        int[] values = new int[] { 1, 2, 3 };
        outer: foreach (int v in values)
        {
            try
            {
                if (v == 2)
                    continue outer;
                total += v;
            }
            finally
            {
                total += 100;
            }
        }
        total += 1000;
        return total;
    }

    public int BreakOutermostFromThreeLevels()
    {
        int total = 0;
        int[] values = new int[] { 1, 2 };
        outer: foreach (int v in values)
        {
            foreach (int w in values)
            {
                foreach (int u in values)
                {
                    if (v + w + u > 3)
                        break outer;
                    total += v + w + u;
                }
                total += 10;
            }
            total += 100;
        }
        total += 1000;
        return total;
    }

    public int ContinueMiddleFromInnermost()
    {
        int total = 0;
        int[] values = new int[] { 1, 2 };
        foreach (int v in values)
        {
            middle: foreach (int w in values)
            {
                foreach (int u in values)
                {
                    if (u == 2)
                        continue middle;
                    total += v + w + u;
                }
                total += 10;
            }
            total += 100;
        }
        total += 1000;
        return total;
    }

    public int BreakLabeledDoWhile()
    {
        int total = 0;
        int i = 0;
        outer: do
        {
            i++;
            int j = 0;
            do
            {
                j++;
                if (i * j >= 4)
                    break outer;
                total += i * j;
            } while (j < 3);
            total += 10;
        } while (i < 3);
        total += 100;
        return total;
    }
}
