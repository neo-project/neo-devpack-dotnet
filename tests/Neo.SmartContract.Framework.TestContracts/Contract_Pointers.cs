// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_Pointers.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Numerics;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_Pointers : SmartContract
    {
        public static object CreateFuncPointer()
        {
            return new Func<int>(MyMethod);
        }

        public static int MyMethod()
        {
            return 123;
        }

        public static int CallFuncPointer()
        {
            var pointer = new Func<int>(MyMethod);
            return pointer.Invoke();
        }

        public static object CreateFuncPointerWithArg()
        {
            return new Func<byte[], BigInteger>(MyMethodWithArg);
        }

        public static BigInteger MyMethodWithArg(byte[] num)
        {
            return new BigInteger(num);
        }

        public static BigInteger CallFuncPointerWithArg()
        {
            var pointer = new Func<byte[], BigInteger>(MyMethodWithArg);

            return pointer.Invoke(new byte[] { 11, 22, 33 });
        }
    }
}
