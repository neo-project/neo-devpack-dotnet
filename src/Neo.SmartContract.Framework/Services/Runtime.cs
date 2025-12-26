// Copyright (C) 2015-2026 The Neo Project.
//
// Runtime.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
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
        /// <summary>
        /// Gets the trigger of the execution. see <see cref="TriggerType"/> for more details.
        /// </summary>
        public static extern TriggerType Trigger
        {
            [Syscall("System.Runtime.GetTrigger")]
            get;
        }

        /// <summary>
        /// Gets the platform of the execution. Always returns "NEO" for now.
        /// </summary>
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

        /// <summary>
        /// Gets the transaction of the execution.
        /// </summary>
        public static extern Transaction Transaction
        {
            [Syscall("System.Runtime.GetScriptContainer")]
            get;
        }

        /// <summary>
        /// Gets the script hash of the executing context.
        /// </summary>
        public static extern UInt160 ExecutingScriptHash
        {
            [Syscall("System.Runtime.GetExecutingScriptHash")]
            get;
        }

        /// <summary>
        /// The script hash of the calling contract.
        /// It returns null if the current context is the entry context.
        /// </summary>
        public static extern UInt160 CallingScriptHash
        {
            [Syscall("System.Runtime.GetCallingScriptHash")]
            get;
        }

        /// <summary>
        /// The script hash of the entry context.
        /// </summary>
        public static extern UInt160 EntryScriptHash
        {
            [Syscall("System.Runtime.GetEntryScriptHash")]
            get;
        }

        /// <summary>
        /// Gets the unixtimestamp in milliseconds of the current block.
        /// </summary>
        public static extern ulong Time
        {
            [Syscall("System.Runtime.GetTime")]
            get;
        }

        /// <summary>
        /// Gets the number of times the current contract has been called during the execution.
        /// </summary>
        public static extern uint InvocationCounter
        {
            [Syscall("System.Runtime.GetInvocationCounter")]
            get;
        }

        /// <summary>
        /// The remaining GAS that can be spent in order to complete the execution.
        /// In the unit of datoshi, 1 datoshi = 1e-8 GAS, 1 GAS = 1e8 datoshi
        /// </summary>
        public static extern long GasLeft
        {
            [Syscall("System.Runtime.GasLeft")]
            get;
        }

        /// <summary>
        /// Gets the address version of the current network.
        /// </summary>
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
        public static extern Notification[] GetNotifications(UInt160? hash = null);

        /// <summary>
        /// Determines whether the specified account has witnessed the current transaction.
        /// <para>
        /// The execution will fail if the hash is null.
        /// </para>
        /// </summary>
        [Syscall("System.Runtime.CheckWitness")]
        public static extern bool CheckWitness(UInt160 hash);

        /// <summary>
        /// Determines whether the specified public key has witnessed the current transaction.
        /// <para>
        /// The execution will fail if the pubkey is null or the format of the pubkey is invalid.
        /// </para>
        /// </summary>
        [Syscall("System.Runtime.CheckWitness")]
        public static extern bool CheckWitness(ECPoint pubkey);

        /// <summary>
        /// Writes a log.
        /// <para>
        /// The execution will fail if:
        ///  1. The message is null;
        ///  2. The length of the message is greater than MaxNotificationSize(the default value is 1024).
        ///  3. The message is not a valid UTF-8 string.
        /// </para>
        /// </summary>
        [Syscall("System.Runtime.Log")]
        public static extern void Log(string message);

        // Events not present in the ABI were disabled in HF_Basilisk
        // [Syscall("System.Runtime.Notify")]
        // public static extern void Notify(string eventName, object[] state);

        /// <summary>
        /// Sends a message as a notification with the name "Debug".
        /// <para>
        /// The execution will fail if message exceeds limits.
        /// </para>
        /// </summary>
        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.PACK)]
        [OpCode(OpCode.PUSHDATA1, "054465627567")] // 0x5 - Debug
        //[OpCode(OpCode.SYSCALL, "95016f61")] // SHA256(System.Runtime.Notify)[0..4]
        [Syscall("System.Runtime.Notify")]
        public static extern void Debug(string message);

        /// <summary>
        /// Burns the specified amount of GAS.
        /// <para>
        /// The execution will fail if the amount is less than or equal to 0.
        /// </para>
        /// </summary>
        [Syscall("System.Runtime.BurnGas")]
        public static extern void BurnGas(long gas);

        /// <summary>
        /// Mints GAS to the executing contract by increasing the transaction's system fee.
        /// The minted amount is credited to Runtime.ExecutingScriptHash.
        /// </summary>
        /// <param name="amount">The amount of GAS to mint (in datoshi, 1 GAS = 100000000 datoshi)</param>
        /// <remarks>
        /// - Amount must be non-negative.
        /// - If SystemFee + amount exceeds MaxSystemFee, the VM will FAULT.
        /// - Can only be used in Application trigger.
        /// </remarks>
        [Syscall("System.Runtime.MintGas")]
        public static extern void MintGas(BigInteger amount);

        /// <summary>
        /// Gets the next random number. The random number is deterministic.
        /// </summary>
        [Syscall("System.Runtime.GetRandom")]
        public static extern BigInteger GetRandom();

        /// <summary>
        /// Gets the magic number of the current network.
        /// </summary>
        [Syscall("System.Runtime.GetNetwork")]
        public static extern uint GetNetwork();

        /// <summary>
        /// Loads a script at rumtime.
        /// </summary>
        /// <para>
        /// The execution will fail if:
        /// 1. The flags is not a valid <see cref="CallFlags"/>.
        /// 2. The arguments is null.
        /// </para>
        [Syscall("System.Runtime.LoadScript")]
        public static extern object LoadScript(ByteString script, CallFlags flags, params object[] args);

        /// <summary>
        /// Gets the signers of the current transaction.
        /// </summary>
        [Syscall("System.Runtime.CurrentSigners")]
        public static extern Signer[] CurrentSigners();
    }
}
