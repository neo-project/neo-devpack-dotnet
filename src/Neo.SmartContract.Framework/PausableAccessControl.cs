// Copyright (C) 2015-2026 The Neo Project.
//
// PausableAccessControl.cs file belongs to the neo project and is free
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
using System.Numerics;

namespace Neo.SmartContract.Framework
{
    /// <summary>
    /// An emergency-stop base contract whose pause switch is controlled by a role
    /// (<see cref="PAUSER_ROLE"/>) of <see cref="AccessControl"/>, so several accounts can pause and
    /// unpause the contract.
    /// </summary>
    /// <remarks>
    /// <see cref="PausableOwnable"/> remains the simpler single-owner model; this class targets
    /// contracts that need multiple operators. Both share the same pause storage flag (prefix 0xFE),
    /// so <see cref="WhenPausedAttribute"/> and <see cref="WhenNotPausedAttribute"/> work with either.
    /// <para>
    /// Initialize from <c>_deploy</c> with <see cref="InitializePausableAccessControl"/>. The initial
    /// administrator receives <see cref="AccessControl.DEFAULT_ADMIN_ROLE"/> and <see cref="PAUSER_ROLE"/>.
    /// </para>
    /// <para>
    /// Assign or revoke pausers with the inherited <c>GrantRole</c>/<c>RevokeRole</c> using
    /// <see cref="PAUSER_ROLE"/>. With the default role-admin policy, a default administrator can
    /// recover after every pauser has been revoked by granting itself the role and unpausing.
    /// Derived contracts that change the pauser role's admin must preserve an authorized recovery path.
    /// </para>
    /// <para>
    /// <see cref="Pause"/> and <see cref="Unpause"/> take the acting account and call
    /// <see cref="AccessControl.OnlyRole"/>: the account must hold the role AND witness the
    /// transaction. Role membership alone is insufficient, otherwise anyone could pass in a pauser's address.
    /// </para>
    /// <para>
    /// On contract update call <c>InitializePausableAccessControl(null, true)</c>: it changes nothing, so
    /// roles, administrator and pause state are preserved.
    /// </para>
    /// <para>
    /// Role identifiers share the inherited access-control namespace: 0 is the default admin role
    /// and 1 is the pauser role. Use distinct identifiers for other roles; reusing 1 also grants pause authority.
    /// </para>
    /// <code>
    /// public static void _deploy(object data, bool update) => InitializePausableAccessControl((UInt160?)data, update);
    ///
    /// [WhenNotPaused]
    /// public static void Transfer(/* ... */) { }
    ///
    /// [WhenPaused]
    /// public static void Repair(/* ... */) { }
    /// </code>
    /// </remarks>
    public abstract class PausableAccessControl : AccessControl
    {
        private const string Prefix_Paused = "\0xFE";

        /// <summary>
        /// The role (1) allowed to pause and unpause the contract.
        /// </summary>
        /// <remarks>Do not reuse this identifier for unrelated roles in a derived contract.</remarks>
        [Safe]
        public static BigInteger PAUSER_ROLE() => BigInteger.One;

        /// <summary>
        /// Whether the contract is currently paused.
        /// </summary>
        [Safe]
        public static bool Paused => Storage.Get(new[] { Prefix_Paused }) is not null;

        /// <summary>
        /// Describes a pause notification identifying the authorized acting account.
        /// </summary>
        /// <param name="account">The account that paused the contract.</param>
        public delegate void OnPausedDelegate(UInt160? account);

        /// <summary>
        /// Raised when the contract is paused. <c>account</c> is the pauser that paused it.
        /// </summary>
        [DisplayName("Paused")]
        public static event OnPausedDelegate OnPaused = null!;

        /// <summary>
        /// Describes an unpause notification identifying the authorized acting account.
        /// </summary>
        /// <param name="account">The account that unpaused the contract.</param>
        public delegate void OnUnpausedDelegate(UInt160? account);

        /// <summary>
        /// Raised when the contract is unpaused. <c>account</c> is the pauser that unpaused it.
        /// </summary>
        [DisplayName("Unpaused")]
        public static event OnUnpausedDelegate OnUnpaused = null!;

        /// <summary>
        /// Pauses the contract. <paramref name="account"/> must hold <see cref="PAUSER_ROLE"/> and
        /// witness the call; aborts if already paused.
        /// </summary>
        public static void Pause(UInt160 account)
        {
            OnlyRole(PAUSER_ROLE(), account);
            ExecutionEngine.Assert(!Paused, "contract is paused");
            Storage.Put(new[] { Prefix_Paused }, 1);
            OnPaused(account);
        }

        /// <summary>
        /// Unpauses the contract. <paramref name="account"/> must hold <see cref="PAUSER_ROLE"/> and
        /// witness the call; aborts if not paused.
        /// </summary>
        public static void Unpause(UInt160 account)
        {
            OnlyRole(PAUSER_ROLE(), account);
            ExecutionEngine.Assert(Paused, "contract is not paused");
            Storage.Delete(new[] { Prefix_Paused });
            OnUnpaused(account);
        }

        /// <summary>
        /// Call from <c>_deploy</c>. On first deployment initializes the default administrator (the
        /// transaction sender when <paramref name="admin"/> is null) and also grants it
        /// <see cref="PAUSER_ROLE"/>. A no-op when <paramref name="update"/> is true.
        /// </summary>
        /// <param name="admin">The initial administrator, or null to use the deployment transaction sender.</param>
        /// <param name="update">Whether this deployment callback is a contract update.</param>
        /// <remarks>
        /// This trusted initializer does not require the initial administrator's witness. Call it only
        /// from the deployment path; it rejects repeated initial deployment and never regrants roles on update.
        /// </remarks>
        protected static void InitializePausableAccessControl(UInt160? admin, bool update)
        {
            if (update) return;

            InitializeAccessControl(admin, false);
            UInt160 initialAdmin = admin ?? Runtime.Transaction.Sender;
            GrantRoleInternal(PAUSER_ROLE(), initialAdmin, initialAdmin);
        }
    }
}
