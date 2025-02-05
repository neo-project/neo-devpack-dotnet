// Copyright (C) 2015-2024 The Neo Project.
//
// Contract_Reentrancy.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
#pragma warning disable CS8625
    public class Contract_Reentrancy : SmartContract.Framework.SmartContract
    {
        public static void HasReentrancy()
        {
            try
            {
                Contract.Call(NEO.Hash, "transfer", CallFlags.All, [UInt160.Zero, UInt160.Zero, 0, null]);
            }
            catch
            {
                Storage.Put(Storage.CurrentContext, new byte[] { 0x01 }, 1);
            }
        }
        public static void HasReentrancyFromSingleBasicBlock()
        {
            Contract.Call(NEO.Hash, "transfer", CallFlags.All, [UInt160.Zero, UInt160.Zero, 0, null]);
            Storage.Put(Storage.CurrentContext, new byte[] { 0x01 }, 1);
        }
        public static void HasReentrancyFromCall()
        {
            Contract.Call(GAS.Hash, "transfer", CallFlags.All, [UInt160.Zero, UInt160.Zero, 0, null]);
            NoReentrancy();
        }
        public static void NoReentrancy()
        {
            Storage.Put(Storage.CurrentContext, new byte[] { 0x01 }, 1);
            Contract.Call(NEO.Hash, "transfer", CallFlags.All, [UInt160.Zero, UInt160.Zero, 0, null]);
        }
        public static void NoReentrancyFromCall()
        {
            Storage.Put(Storage.CurrentContext, new byte[] { 0x01 }, 1);
            NoReentrancy();
        }
        public static void NoReentrancyFromJump(bool input)
        {
            if (input)
                Contract.Call(GAS.Hash, "transfer", CallFlags.All, [UInt160.Zero, UInt160.Zero, 0, null]);
            else
                Storage.Put(Storage.CurrentContext, new byte[] { 0x01 }, 1);
        }
        [NoReentrant]
        public static void NoReentrancyByAttribute()
        {
            HasReentrancyFromSingleBasicBlock();
        }
#pragma warning restore CS8625
    }
}
