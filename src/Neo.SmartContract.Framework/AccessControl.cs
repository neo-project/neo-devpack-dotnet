// Copyright (C) 2015-2026 The Neo Project.
//
// AccessControl.cs file belongs to the neo project and is free
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
    /// A role-based access-control base contract, the OpenZeppelin <c>AccessControl</c> analog
    /// for Neo. Unlike <see cref="Ownable"/> (a single owner), it lets a contract grant distinct
    /// roles (for example a minter role, a pauser role) to multiple accounts, each role
    /// administered by another role.
    /// </summary>
    /// <remarks>
    /// A role is identified by a non-negative integer. Define role enums in your contract and pass
    /// their numeric values directly, e.g. <c>OnlyRole((int)Roles.Minter, minter)</c>.
    /// <see cref="DEFAULT_ADMIN_ROLE"/> (0) is the root role and is its own admin;
    /// the account it is granted to at deployment can administer every other role.
    /// <para>
    /// Authorization is by explicit actor: callers pass the acting account and the contract
    /// verifies it both holds the required role and witnessed the call
    /// (<c>Runtime.CheckWitness</c>). This honors the signer's witness scope and lets a contract
    /// (multisig, DAO) hold a role.
    /// </para>
    /// <para>
    /// The last holder of <see cref="DEFAULT_ADMIN_ROLE"/> can never be removed, so the contract
    /// can never become un-administrable from a single call. To hand over root control, grant the
    /// role to a successor first, then renounce your own.
    /// </para>
    /// <para>
    /// To gate your own methods on a role, call <see cref="OnlyRole"/> as the first statement:
    /// <code>
    /// public static void Mint(UInt160 minter, UInt160 to, BigInteger amount)
    /// {
    ///     OnlyRole(MINTER_ROLE, minter);
    ///     // ...
    /// }
    /// </code>
    /// </para>
    /// </remarks>
    public abstract class AccessControl : SmartContract
    {
        private const byte Prefix = 0xFB;

        // Sub-namespace tags. 0x00 is reserved for the one-time init flag only; the role keys use
        // non-zero tags so they can never collide with it.
        private const byte TAG_MEMBER = 0x01;  // [role][account:20] -> granted
        private const byte TAG_ADMIN = 0x02;   // [role]             -> admin role override
        private const byte TAG_COUNT = 0x03;   // [role]             -> member count

        // The init flag lives under tag 0x00 (a fixed single-byte key) so it can never collide with
        // the non-zero tagged variable-length role keys.
        private const byte TAG_INIT = 0x00;

        private static StorageMap Map => new(Storage.CurrentContext, Prefix);

        private static byte[] MemberKey(BigInteger role, UInt160 account)
            => new byte[] { TAG_MEMBER }.Concat(RoleBytes(role)).Concat((byte[])account);

        private static byte[] AdminKey(BigInteger role)
            => new byte[] { TAG_ADMIN }.Concat(RoleBytes(role));

        private static byte[] CountKey(BigInteger role)
            => new byte[] { TAG_COUNT }.Concat(RoleBytes(role));

        private static byte[] RoleBytes(BigInteger role)
            => (byte[])(ByteString)role;

        private static void ValidateRole(BigInteger role)
            => ExecutionEngine.Assert(role >= 0, "AccessControl: role must be non-negative");

        /// <summary>
        /// The root admin role (0). It is its own admin, and the account granted this
        /// role at deployment can administer every other role.
        /// </summary>
        [Safe]
        public static BigInteger DEFAULT_ADMIN_ROLE() => BigInteger.Zero;

        /// <summary>
        /// Returns whether <paramref name="account"/> has been granted <paramref name="role"/>.
        /// </summary>
        [Safe]
        public static bool HasRole(BigInteger role, UInt160 account)
        {
            ValidateRole(role);
            return Map.Get(MemberKey(role, account)) is not null;
        }

        /// <summary>
        /// Returns the admin role that controls granting and revoking <paramref name="role"/>.
        /// Defaults to <see cref="DEFAULT_ADMIN_ROLE"/> when no override has been set. This is
        /// the admin role id, not a single admin account; every account holding the admin role can
        /// administer <paramref name="role"/>.
        /// </summary>
        [Safe]
        public static BigInteger GetRoleAdmin(BigInteger role)
        {
            ValidateRole(role);
            return Map.GetIntegerOrZero(AdminKey(role));
        }

        /// <summary>
        /// Returns the number of accounts that currently hold <paramref name="role"/>.
        /// </summary>
        [Safe]
        public static BigInteger GetRoleMemberCount(BigInteger role)
        {
            ValidateRole(role);
            return Map.GetIntegerOrZero(CountKey(role));
        }

        public delegate void OnRoleGrantedDelegate(BigInteger role, UInt160 account, UInt160 sender);

        [DisplayName("RoleGranted")]
        public static event OnRoleGrantedDelegate OnRoleGranted = null!;

        public delegate void OnRoleRevokedDelegate(BigInteger role, UInt160 account, UInt160 sender);

        [DisplayName("RoleRevoked")]
        public static event OnRoleRevokedDelegate OnRoleRevoked = null!;

        public delegate void OnRoleAdminChangedDelegate(BigInteger role, BigInteger previousAdminRole, BigInteger newAdminRole);

        [DisplayName("RoleAdminChanged")]
        public static event OnRoleAdminChangedDelegate OnRoleAdminChanged = null!;

        /// <summary>
        /// Asserts that <paramref name="account"/> holds <paramref name="role"/> and witnessed the
        /// current invocation. This is the authorization core; it aborts otherwise.
        /// </summary>
        protected static void CheckRole(BigInteger role, UInt160 account)
        {
            ValidateRole(role);
            ExecutionEngine.Assert(account.IsValidAndNotZero, "AccessControl: invalid account");
            ExecutionEngine.Assert(HasRole(role, account), "AccessControl: account is missing role");
            ExecutionEngine.Assert(Runtime.CheckWitness(account), "AccessControl: missing witness for account");
        }

        /// <summary>
        /// The first-line guard for an author's role-gated method: asserts that
        /// <paramref name="account"/> holds <paramref name="role"/> and signed the call.
        /// </summary>
        protected static void OnlyRole(BigInteger role, UInt160 account) => CheckRole(role, account);

        /// <summary>
        /// Grants <paramref name="role"/> to <paramref name="account"/>. <paramref name="admin"/>
        /// must hold the role's admin role and witness the call. A no-op if already granted.
        /// </summary>
        public static void GrantRole(BigInteger role, UInt160 admin, UInt160 account)
        {
            ValidateRole(role);
            ExecutionEngine.Assert(account.IsValidAndNotZero, "AccessControl: invalid account");
            CheckRole(GetRoleAdmin(role), admin);
            GrantRoleInternal(role, account, admin);
        }

        /// <summary>
        /// Revokes <paramref name="role"/> from <paramref name="account"/>. <paramref name="admin"/>
        /// must hold the role's admin role and witness the call. A no-op if not granted.
        /// </summary>
        public static void RevokeRole(BigInteger role, UInt160 admin, UInt160 account)
        {
            ValidateRole(role);
            ExecutionEngine.Assert(account.IsValidAndNotZero, "AccessControl: invalid account");
            CheckRole(GetRoleAdmin(role), admin);
            if (!HasRole(role, account))
                return;
            GuardLastAdmin(role);
            RevokeRoleInternal(role, account, admin);
        }

        /// <summary>
        /// Renounces <paramref name="role"/> for the caller. Only <paramref name="account"/> itself
        /// (verified by witness) may renounce its own role. Aborts if the role is not held.
        /// </summary>
        public static void RenounceRole(BigInteger role, UInt160 account)
        {
            ValidateRole(role);
            ExecutionEngine.Assert(account.IsValidAndNotZero, "AccessControl: invalid account");
            ExecutionEngine.Assert(Runtime.CheckWitness(account), "AccessControl: can only renounce roles for self");
            ExecutionEngine.Assert(HasRole(role, account), "AccessControl: account is missing role");
            GuardLastAdmin(role);
            RevokeRoleInternal(role, account, account);
        }

        // The final DEFAULT_ADMIN_ROLE holder can never be removed, so a single call can never
        // leave the contract un-administrable. Hand over root control by granting the role to a
        // successor first (count >= 2), then dropping your own.
        private static void GuardLastAdmin(BigInteger role)
        {
            if (role == DEFAULT_ADMIN_ROLE())
                ExecutionEngine.Assert(GetRoleMemberCount(role) > 1, "AccessControl: cannot remove last default admin");
        }

        /// <summary>
        /// Unconditionally grants <paramref name="role"/> to <paramref name="account"/> (no admin
        /// check). Idempotent. Not exported; wrap it behind your own authorization.
        /// </summary>
        protected static void GrantRoleInternal(BigInteger role, UInt160 account, UInt160 sender)
        {
            ValidateRole(role);
            ExecutionEngine.Assert(account.IsValidAndNotZero, "AccessControl: invalid account");
            byte[] key = MemberKey(role, account);
            if (Map.Get(key) is not null)
                return;
            Map.Put(key, 1);
            Map.Increase(CountKey(role));
            OnRoleGranted(role, account, sender);
        }

        /// <summary>
        /// Unconditionally revokes <paramref name="role"/> from <paramref name="account"/> (no admin
        /// or last-admin check). Idempotent. Not exported; the public callers apply the guards.
        /// </summary>
        protected static void RevokeRoleInternal(BigInteger role, UInt160 account, UInt160 sender)
        {
            ValidateRole(role);
            byte[] key = MemberKey(role, account);
            if (Map.Get(key) is null)
                return;
            Map.Delete(key);
            Map.Decrease(CountKey(role));
            OnRoleRevoked(role, account, sender);
        }

        /// <summary>
        /// Sets the admin role for <paramref name="role"/>. Not exported (no admin check); expose it
        /// behind a gated wrapper, for example:
        /// <code>
        /// public static void SetRoleAdmin(BigInteger role, BigInteger adminRole, UInt160 admin)
        /// {
        ///     OnlyRole(GetRoleAdmin(role), admin);
        ///     AccessControl.SetRoleAdminInternal(role, adminRole);
        /// }
        /// </code>
        /// </summary>
        protected static void SetRoleAdminInternal(BigInteger role, BigInteger adminRole)
        {
            ValidateRole(role);
            ValidateRole(adminRole);
            BigInteger previous = GetRoleAdmin(role);
            if (previous == adminRole)
                return;
            if (adminRole == DEFAULT_ADMIN_ROLE())
                Map.Delete(AdminKey(role));
            else
                Map.Put(AdminKey(role), adminRole);
            OnRoleAdminChanged(role, previous, adminRole);
        }

        /// <summary>
        /// Bootstraps access control by granting <see cref="DEFAULT_ADMIN_ROLE"/> to the initial
        /// admin. Call this from the contract's <c>_deploy</c> method. Defaults the admin to the
        /// transaction sender when <paramref name="data"/> is null; a no-op on update. Can only run
        /// once.
        /// </summary>
        protected static void InitializeAccessControl(object? data, bool update)
        {
            if (update)
                return;

            ExecutionEngine.Assert(Map.Get(new byte[] { TAG_INIT }) is null, "AccessControl: already initialized");
            Map.Put(new byte[] { TAG_INIT }, 1);

            data ??= Runtime.Transaction.Sender;

            UInt160 admin = (UInt160)data;
            ExecutionEngine.Assert(admin.IsValidAndNotZero, "AccessControl: invalid initial admin");

            GrantRoleInternal(DEFAULT_ADMIN_ROLE(), admin, admin);
        }
    }
}
