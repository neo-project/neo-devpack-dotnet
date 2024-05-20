// Copyright (C) 2015-2024 The Neo Project.
//
// Permission.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;

namespace Neo.SmartContract.Framework;

/// <summary>
/// Sets the permission for the developing contract to call a method of another contract.
/// </summary>
public static class Permission
{
    /// <summary>
    /// Indicates that the contract is allowed to call <see cref="Method"/> of any contract.
    /// </summary>
    [Obsolete("Use Any instead.")]
    public const string WildCard = "*";

    /// <summary>
    /// Indicates that the contract is allowed to call <see cref="Method"/> of any contract.
    /// </summary>
    public const string Any = "*";
}
