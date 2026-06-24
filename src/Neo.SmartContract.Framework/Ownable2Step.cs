// Copyright (C) 2015-2026 The Neo Project.
//
// Ownable2Step.cs file belongs to the neo project and is free
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
using System.Diagnostics.CodeAnalysis;

namespace Neo.SmartContract.Framework
{
    /// <summary>
    /// A base contract providing two-step ownership transfer, the OpenZeppelin
    /// <c>Ownable2Step</c> analog for Neo.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="Ownable"/>, ownership is never handed over in a single call.
    /// <see cref="TransferOwnership"/> only records a <em>pending</em> owner; the transfer
    /// completes only when that account itself calls <see cref="AcceptOwnership"/> and proves
    /// control via <c>Runtime.CheckWitness</c>. This prevents the classic footgun where a
    /// mistyped or otherwise unusable address is set as owner in one step and control is lost
    /// forever. There is intentionally no single-step setter on this class.
    /// <para>
    /// <see cref="RenounceOwnership"/> is the one deliberate, irreversible action: it removes the
    /// owner entirely (and any in-flight pending transfer). After renouncing, every owner-gated
    /// method is permanently uncallable.
    /// </para>
    /// <para>
    /// Composition note: do not gate <see cref="CancelOwnershipTransfer"/> or
    /// <see cref="RenounceOwnership"/> behind a pause guard. Both must remain callable so that an
    /// in-flight transfer to a compromised account can always be neutralized.
    /// </para>
    /// </remarks>
    [ExcludeFromCodeCoverage]
    public abstract class Ownable2Step : SmartContract
    {
        // The current confirmed owner. Absent (null) means renounced or not yet initialized.
        private const byte Prefix_Owner = 0xFD;

        // The proposed owner awaiting acceptance. Absent (null) means no transfer is in flight.
        private const byte Prefix_PendingOwner = 0xFC;

        private static LocalStorageMap OwnerStorage => new(Prefix_Owner);
        private static LocalStorageMap PendingOwnerStorage => new(Prefix_PendingOwner);

        /// <summary>
        /// Returns the current owner, or <see langword="null"/> if ownership has been renounced
        /// or the contract has not been initialized.
        /// </summary>
        [Safe]
        public static UInt160? GetOwner()
        {
            return (UInt160)OwnerStorage.Get(ByteString.Empty)!;
        }

        /// <summary>
        /// Returns the account that has been proposed as the new owner and is awaiting its own
        /// acceptance, or <see langword="null"/> when no transfer is in flight.
        /// </summary>
        [Safe]
        public static UInt160? GetPendingOwner()
        {
            return (UInt160)PendingOwnerStorage.Get(ByteString.Empty)!;
        }

        protected static bool IsOwner()
        {
            UInt160? owner = GetOwner();
            return owner is not null && Runtime.CheckWitness(owner);
        }

        public delegate void OnOwnershipTransferStartedDelegate(UInt160? previousOwner, UInt160? newOwner);

        /// <summary>
        /// Raised when an owner proposes a new owner. <c>newOwner</c> is the pending candidate and
        /// is not yet the owner; the change is not effective until <see cref="AcceptOwnership"/>.
        /// </summary>
        [DisplayName("OwnershipTransferStarted")]
        public static event OnOwnershipTransferStartedDelegate OnOwnershipTransferStarted = null!;

        public delegate void OnOwnershipTransferredDelegate(UInt160? previousOwner, UInt160? newOwner);

        /// <summary>
        /// Raised whenever the owner actually changes: on initialization (<c>previousOwner</c>
        /// null), on a completed acceptance, and on renounce (<c>newOwner</c> null).
        /// </summary>
        [DisplayName("OwnershipTransferred")]
        public static event OnOwnershipTransferredDelegate OnOwnershipTransferred = null!;

        public delegate void OnOwnershipTransferCanceledDelegate(UInt160? currentOwner, UInt160? canceledPendingOwner);

        /// <summary>
        /// Raised when a pending transfer is dropped without completing: explicitly via
        /// <see cref="CancelOwnershipTransfer"/>, or implicitly when a new proposal supersedes it.
        /// </summary>
        [DisplayName("OwnershipTransferCanceled")]
        public static event OnOwnershipTransferCanceledDelegate OnOwnershipTransferCanceled = null!;

        /// <summary>
        /// Step 1 of an ownership transfer. The current owner proposes <paramref name="newOwner"/>;
        /// the owner does not change until that account calls <see cref="AcceptOwnership"/>.
        /// </summary>
        public static void TransferOwnership(UInt160 newOwner)
        {
            if (!IsOwner())
                throw new System.InvalidOperationException("No Authorization!");

            ExecutionEngine.Assert(newOwner.IsValidAndNotZero, "owner must be valid");

            UInt160? previousOwner = GetOwner();
            ExecutionEngine.Assert(previousOwner != newOwner, "owner must change");

            // A new proposal supersedes any earlier one; emit a terminal event for the dropped
            // candidate so off-chain indexers never see a pending offer left dangling.
            UInt160? oldPending = (UInt160)PendingOwnerStorage.Get(ByteString.Empty)!;
            if (oldPending is not null)
                OnOwnershipTransferCanceled(previousOwner, oldPending);

            PendingOwnerStorage.Put(ByteString.Empty, newOwner);
            OnOwnershipTransferStarted(previousOwner, newOwner);
        }

        /// <summary>
        /// Step 2 of an ownership transfer. The pending owner accepts and becomes the owner.
        /// Must be called by the pending account itself (verified with <c>Runtime.CheckWitness</c>).
        /// </summary>
        public static void AcceptOwnership()
        {
            UInt160? pendingOwner = (UInt160)PendingOwnerStorage.Get(ByteString.Empty)!;
            ExecutionEngine.Assert(pendingOwner is not null, "no pending owner");
            ExecutionEngine.Assert(pendingOwner!.IsValidAndNotZero, "no pending owner");
            ExecutionEngine.Assert(Runtime.CheckWitness(pendingOwner), "only pending owner");

            UInt160? previousOwner = GetOwner();
            OwnerStorage.Put(ByteString.Empty, pendingOwner);
            PendingOwnerStorage.Delete(ByteString.Empty);
            OnOwnershipTransferred(previousOwner, pendingOwner);
        }

        /// <summary>
        /// Cancels an in-flight ownership transfer, leaving the current owner in place. Only the
        /// owner may call this, and only while a transfer is pending.
        /// </summary>
        public static void CancelOwnershipTransfer()
        {
            if (!IsOwner())
                throw new System.InvalidOperationException("No Authorization!");

            UInt160? pendingOwner = (UInt160)PendingOwnerStorage.Get(ByteString.Empty)!;
            ExecutionEngine.Assert(pendingOwner is not null, "no pending owner");

            PendingOwnerStorage.Delete(ByteString.Empty);
            OnOwnershipTransferCanceled(GetOwner(), pendingOwner);
        }

        /// <summary>
        /// Permanently removes the owner. This is irreversible: after renouncing, every
        /// owner-gated method becomes uncallable. Any in-flight pending transfer is also cleared
        /// so a previously-proposed account cannot seize the abandoned contract.
        /// </summary>
        public static void RenounceOwnership()
        {
            if (!IsOwner())
                throw new System.InvalidOperationException("No Authorization!");

            UInt160? previousOwner = GetOwner();
            OwnerStorage.Delete(ByteString.Empty);
            PendingOwnerStorage.Delete(ByteString.Empty);
            OnOwnershipTransferred(previousOwner, null);
        }

        /// <summary>
        /// Initializes the owner. Call this from the contract's <c>_deploy</c> method.
        /// </summary>
        /// <remarks>
        /// On a contract update this is a no-op and never resets the owner. Note that an in-flight
        /// pending transfer is intentionally left untouched across an update; operators should
        /// cancel any in-flight transfer before upgrading if the upgrade changes trust assumptions.
        /// </remarks>
        protected static void InitializeOwner(object? data, bool update)
        {
            if (update)
                return;

            data ??= Runtime.Transaction.Sender;

            UInt160 initialOwner = (UInt160)data;
            ExecutionEngine.Assert(initialOwner.IsValidAndNotZero, "owner must be valid");

            OwnerStorage.Put(ByteString.Empty, initialOwner);
            OnOwnershipTransferred(null, initialOwner);
        }
    }
}
