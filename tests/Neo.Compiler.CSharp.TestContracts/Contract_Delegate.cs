// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_Delegate.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Delegate : SmartContract.Framework.SmartContract
    {
        public static int sumFunc(int a, int b)
        {
            return new Func<int, int, int>(privateSum).Invoke(a, b);
        }

        private static int privateSum(int a, int b)
        {
            return a + b;
        }

        public delegate int MyDelegate(int x, int y);

        static int CalculateSum(int x, int y)
        {
            return x + y;
        }

        public void TestDelegate()
        {
            MyDelegate myDelegate = CalculateSum;
            int result = myDelegate(5, 6);
            Runtime.Log($"Sum: {result}");
        }
    }
}
