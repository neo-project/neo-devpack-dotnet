// Copyright (C) 2015-2024 The Neo Project.
//
// Contract_Attribute.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Native;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class OwnerOnlyAttribute : ModifierAttribute
    {
        UInt160 owner;

        public OwnerOnlyAttribute(string hex)
        {
            owner = (UInt160)(byte[])StdLib.Base64Decode(hex);
        }

        public override void Enter()
        {
            if (!Runtime.CheckWitness(owner)) throw new System.Exception();
        }

        public override void Exit() { }
    }

    public class Contract_Attribute : SmartContract
    {
        [OwnerOnly("AAAAAAAAAAAAAAAAAAAAAAAAAAA=")]
        public static bool test()
        {
            return true;
        }

        [NoReentrant]
        public void reentrantB()
        {
            // do nothing
        }

        [NoReentrant]
        public void reentrantA()
        {
            reentrantB();
        }

        [NoReentrantMethod]
        public void reentrantTest(int value)
        {
            if (value == 0) return;
            if (value == 123)
            {
                reentrantTest(0);
            }
        }
    }
}
