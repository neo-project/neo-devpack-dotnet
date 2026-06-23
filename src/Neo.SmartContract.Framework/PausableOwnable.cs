// Copyright (C) 2015-2026 The Neo Project.
//
// PausableOwnable.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;

namespace Neo.SmartContract.Framework
{
    /// <summary>
    /// An emergency-stop ("circuit breaker") base contract whose pause switch is owner-gated by
    /// default. It combines <see cref="Ownable"/> with a paused flag so that only the owner can
    /// pause or unpause the contract.
    /// </summary>
    /// <remarks>
    /// This is the safe-by-default alternative to <see cref="Pausable"/>, whose
    /// <c>Pause</c>/<c>Unpause</c> are unguarded and rely on the author to add an access check.
    /// Here <see cref="Pause"/> and <see cref="Unpause"/> are public but require the owner's
    /// witness, so a contract is never left with an anyone-can-pause denial-of-service.
    /// <para>
    /// Initialize the owner from your <c>_deploy</c> method with <see cref="Ownable.InitializeOwner"/>,
    /// then guard your state-changing methods with <see cref="WhenNotPaused"/>:
    /// <code>
    /// public static void Transfer(/* ... */)
    /// {
    ///     WhenNotPaused();
    ///     // ...
    /// }
    /// </code>
    /// </para>
    /// </remarks>
    public abstract class PausableOwnable : Ownable
    {
        private const byte Prefix_Paused = 0xFE;

        /// <summary>
        /// Whether the contract is currently paused.
        /// </summary>
        [Safe]
        public static bool Paused => Storage.Get(new[] { Prefix_Paused }) is not null;

        public delegate void OnPausedDelegate(UInt160? account);

        /// <summary>
        /// Raised when the contract is paused. <c>account</c> is the owner that paused it.
        /// </summary>
        [DisplayName("Paused")]
        public static event OnPausedDelegate OnPaused = null!;

        public delegate void OnUnpausedDelegate(UInt160? account);

        /// <summary>
        /// Raised when the contract is unpaused. <c>account</c> is the owner that unpaused it.
        /// </summary>
        [DisplayName("Unpaused")]
        public static event OnUnpausedDelegate OnUnpaused = null!;

        /// <summary>
        /// Pauses the contract. Only the owner may call this, and only while not already paused.
        /// </summary>
        public static void Pause()
        {
            if (!IsOwner())
                throw new System.InvalidOperationException("No Authorization!");

            ExecutionEngine.Assert(!Paused, "contract is paused");
            Storage.Put(new[] { Prefix_Paused }, 1);
            OnPaused(GetOwner());
        }

        /// <summary>
        /// Unpauses the contract. Only the owner may call this, and only while paused.
        /// </summary>
        public static void Unpause()
        {
            if (!IsOwner())
                throw new System.InvalidOperationException("No Authorization!");

            ExecutionEngine.Assert(Paused, "contract is not paused");
            Storage.Delete(new[] { Prefix_Paused });
            OnUnpaused(GetOwner());
        }

        /// <summary>
        /// Guard for an author's method that must only run while the contract is not paused.
        /// </summary>
        protected static void WhenNotPaused() => ExecutionEngine.Assert(!Paused, "contract is paused");

        /// <summary>
        /// Guard for an author's method that must only run while the contract is paused.
        /// </summary>
        protected static void WhenPaused() => ExecutionEngine.Assert(Paused, "contract is not paused");
    }
}
