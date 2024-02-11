// Copyright (C) 2015-2023 The Neo Project.
//
// The Neo.SmartContract.Framework is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using System;
using System.Numerics;

namespace Neo.SmartContract.Framework.Services
{
    public static class Runtime
    {
        public static extern TriggerType Trigger
        {
            [Syscall("System.Runtime.GetTrigger")]
            get;
        }

        public static extern string Platform
        {
            [Syscall("System.Runtime.Platform")]
            get;
        }

        [Obsolete("Use System.Runtime.Transaction instead")]
        public static extern object ScriptContainer
        {
            [Syscall("System.Runtime.GetScriptContainer")]
            get;
        }

        public static extern Transaction Transaction
        {
            [Syscall("System.Runtime.GetScriptContainer")]
            get;
        }

        public static extern UInt160 ExecutingScriptHash
        {
            [Syscall("System.Runtime.GetExecutingScriptHash")]
            get;
        }

        public static extern UInt160 CallingScriptHash
        {
            [Syscall("System.Runtime.GetCallingScriptHash")]
            get;
        }

        public static extern UInt160 EntryScriptHash
        {
            [Syscall("System.Runtime.GetEntryScriptHash")]
            get;
        }

        public static extern ulong Time
        {
            [Syscall("System.Runtime.GetTime")]
            get;
        }

        public static extern uint InvocationCounter
        {
            [Syscall("System.Runtime.GetInvocationCounter")]
            get;
        }

        public static extern long GasLeft
        {
            [Syscall("System.Runtime.GasLeft")]
            get;
        }

        public static extern byte AddressVersion
        {
            [Syscall("System.Runtime.GetAddressVersion")]
            get;
        }

        /// <summary>
        /// This method gets current invocation notifications from specific 'scriptHash'
        /// 'scriptHash' must have 20 bytes, but if it's all zero 0000...0000 it refers to all existing notifications (like a * wildcard)
        /// It will return an array of all matched notifications
        /// Each notification has two elements: a ScriptHash and the stackitem content of notification itself (called a 'State')
        /// The stackitem 'State' can be of any kind (a number, a string, an array, ...), so it's up to the developer perform the expected cast here
        /// </summary>
        [Syscall("System.Runtime.GetNotifications")]
        public static extern Notification[] GetNotifications(UInt160 hash = null);

        [Syscall("System.Runtime.CheckWitness")]
        public static extern bool CheckWitness(UInt160 hash);

        [Syscall("System.Runtime.CheckWitness")]
        public static extern bool CheckWitness(Cryptography.ECC.ECPoint pubkey);

        [Syscall("System.Runtime.Log")]
        public static extern void Log(string message);

        // Events not present in the ABI were disabled in HF_Basilisk
        // [Syscall("System.Runtime.Notify")]
        // public static extern void Notify(string eventName, object[] state);

        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.PACK)]
        [OpCode(OpCode.PUSHDATA1, "054465627567")] // 0x5 - Debug
        //[OpCode(OpCode.SYSCALL, "95016f61")] // SHA256(System.Runtime.Notify)[0..4]
        [Syscall("System.Runtime.Notify")]
        public static extern void Debug(string message);

        [Syscall("System.Runtime.BurnGas")]
        public static extern void BurnGas(long gas);

        [Syscall("System.Runtime.GetRandom")]
        public static extern BigInteger GetRandom();

        [Syscall("System.Runtime.GetNetwork")]
        public static extern uint GetNetwork();

        [Syscall("System.Runtime.LoadScript")]
        public static extern object LoadScript(ByteString script, CallFlags flags, params object[] args);

        [Syscall("System.Runtime.CurrentSigners")]
        public static extern Signer[] CurrentSigners();
    }
}
