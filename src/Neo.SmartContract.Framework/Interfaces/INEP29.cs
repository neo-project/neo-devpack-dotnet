// Copyright (C) 2015-2024 The Neo Project.
//
// INEP29.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.SmartContract.Framework.Interfaces;

/// <summary>
/// NEP-29: Contract deployment/update callback function
/// This interface standardizes the callback function that can be implemented by contracts
/// to have some code executed right after initial deployment or update.
/// </summary>
public interface INEP29
{
    /// <summary>
    /// This method will be automatically executed by ContractManagement contract when a contract is first deployed or updated.
    /// </summary>
    /// <param name="data">Contract-specific data, can be any valid NEP-14 parameter type</param>
    /// <param name="update">True when contract is updated, false on initial deployment</param>
#pragma warning disable IDE1006 // Naming Styles
    void _deploy(object data, bool update);
#pragma warning restore IDE1006 // Naming Styles
}
