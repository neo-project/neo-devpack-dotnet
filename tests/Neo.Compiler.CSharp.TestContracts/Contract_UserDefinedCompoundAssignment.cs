// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_UserDefinedCompoundAssignment.cs file belongs to the neo project and is free
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
/// Exercises C# 14 user-defined compound assignment operators, declared as
/// void instance operators such as `public void operator +=(int amount)`.
/// The compiler lowers `x op= y` to a single invocation of the operator on
/// the current value of `x`, which is evaluated exactly once and mutated in
/// place; no result is written back.
/// </summary>
public class Contract_UserDefinedCompoundAssignment : SmartContract.Framework.SmartContract
{
    public struct Counter
    {
        public int Value;

        public Counter(int value)
        {
            Value = value;
        }

        public void operator +=(int amount)
        {
            Value += amount;
        }

        public void operator -=(int amount)
        {
            Value -= amount;
        }

        public void operator *=(int factor) => Value *= factor;
    }

    public class Wallet
    {
        private int _balance;

        public Wallet(int balance)
        {
            _balance = balance;
        }

        public int Balance => _balance;

        public void operator +=(int amount)
        {
            _balance += amount;
        }

        public void operator -=(int amount)
        {
            _balance -= amount;
        }
    }

    public class Holder
    {
        public Counter Slot;

        public Holder(Counter slot)
        {
            Slot = slot;
        }
    }

    private static Counter _total;
    private static Wallet _bank = new Wallet(40);

    /// <summary>
    /// Local variable target. 2 + 5 - 1 = 6, scaled by 2 = 12. The `*=`
    /// operator is expression-bodied, so its int result must be dropped.
    /// Expected: 12.
    /// </summary>
    public static int StructLocalTarget()
    {
        Counter counter = new Counter(2);
        counter += 5;
        counter -= 1;
        counter *= 2;
        return counter.Value;
    }

    /// <summary>Static field target. 3 + 4 = 7. Expected: 7.</summary>
    public static int StructStaticFieldTarget()
    {
        _total = new Counter(3);
        _total += 4;
        return _total.Value;
    }

    /// <summary>
    /// Array element target. arr[0]: 10 + 5 = 15, arr[1] keeps 1.
    /// Expected: 1501.
    /// </summary>
    public static int StructArrayElementTarget()
    {
        Counter[] counters = new Counter[] { new Counter(10), new Counter(1) };
        counters[0] += 5;
        return counters[0].Value * 100 + counters[1].Value;
    }

    /// <summary>Field member access target. holder.Slot: 20 + 2 = 22. Expected: 22.</summary>
    public static int StructMemberAccessTarget()
    {
        Holder holder = new Holder(new Counter(20));
        holder.Slot += 2;
        return holder.Slot.Value;
    }

    /// <summary>
    /// Reference-type (class) target. 30 + 8 - 3 = 35. Expected: 35.
    /// </summary>
    public static int ClassLocalTarget()
    {
        Wallet wallet = new Wallet(30);
        wallet += 8;
        wallet -= 3;
        return wallet.Balance;
    }

    /// <summary>
    /// The callee mutates the wallet it receives. 50 + 2 = 52. Expected: 52.
    /// The helper uses the compound operator in an expression body, which must not
    /// leave a value on the stack.
    /// </summary>
    public static int ClassParameterTarget()
    {
        Wallet wallet = new Wallet(50);
        TopUp(wallet, 2);
        return wallet.Balance;
    }

    private static void TopUp(Wallet wallet, int amount) => wallet += amount;

    /// <summary>
    /// The receiver is evaluated before the right operand. ReplaceBank points _bank at
    /// a fresh Wallet(0) and returns 5; the compound operator mutates the wallet
    /// captured before the call, so the fresh wallet keeps 0. If the receiver were
    /// evaluated after the argument, the fresh wallet would hold 5 instead.
    /// Expected: 0.
    /// </summary>
    public static int ReceiverEvaluatedBeforeArgument()
    {
        _bank = new Wallet(40);
        _bank += ReplaceBank();
        return _bank.Balance;
    }

    private static int ReplaceBank()
    {
        _bank = new Wallet(0);
        return 5;
    }
}
