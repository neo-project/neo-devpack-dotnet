// Copyright (C) 2015-2026 The Neo Project.
//
// Pausable.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework
{
    /// <summary>
    /// A minimal pausable ("circuit breaker") base contract.
    /// </summary>
    /// <remarks>
    /// <see cref="Pause"/> and <see cref="Unpause"/> are <c>protected</c> and perform <b>no</b>
    /// access control: if you expose them through a public method you <b>must</b> add your own
    /// authorization check, otherwise any caller can pause the contract and deny service to every
    /// guarded method. For an owner-gated pause switch out of the box, derive from
    /// <see cref="PausableOwnable"/> instead.
    /// </remarks>
    public abstract class Pausable : SmartContract
    {
        private const byte Prefix_Paused = 0xFE;

        /// <summary>
        /// Whether the contract is currently paused.
        /// </summary>
        [Safe]
        public static bool Paused => Storage.Get(new[] { Prefix_Paused }) is not null;

        /// <summary>
        /// Pauses the contract. Has no access control; wrap it behind an authorization check.
        /// </summary>
        protected static void Pause()
        {
            ExecutionEngine.Assert(!Paused, "contract is paused");
            Storage.Put(new[] { Prefix_Paused }, 1);
        }

        /// <summary>
        /// Unpauses the contract. Has no access control; wrap it behind an authorization check.
        /// </summary>
        protected static void Unpause()
        {
            ExecutionEngine.Assert(Paused, "contract is not paused");
            Storage.Delete(new[] { Prefix_Paused });
        }
    }
}
