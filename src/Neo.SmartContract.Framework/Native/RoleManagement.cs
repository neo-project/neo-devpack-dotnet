// Copyright (C) 2015-2026 The Neo Project.
//
// RoleManagement.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

#pragma warning disable CS0626

using Neo.SmartContract.Framework.Attributes;

namespace Neo.SmartContract.Framework.Native
{
    [Contract("0x49cf4e5378ffcd4dec034fd98a174c5491e395e2")]
    public class RoleManagement
    {
        [ContractHash]
        public static extern UInt160 Hash { get; }

        /// <summary>
        /// Gets the list of nodes(public keys) for the specified role.
        /// If no such role is designated, an empty array will be returned.
        /// <para>
        /// The execution will fail if:
        ///  1. The 'role' is not a valid Role value.
        ///  2. The 'index' greater than the current_index + 1.
        /// </para>
        /// </summary>
        public static extern ECPoint[] GetDesignatedByRole(Role role, uint index);
    }
}
