// Copyright (C) 2015-2024 The Neo Project.
//
// Method.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;

namespace Neo.SmartContract.Framework;

public static class Method
{
    /// <summary>
    /// Indicates that the contract is allowed to call any method of allowed contract.
    /// </summary>
    [Obsolete("Use Any instead.")]
    public const string WildCard = "*";

    /// <summary>
    /// Indicates that the contract is allowed to call any method of allowed contract.
    /// </summary>
    public const string Any = "*";

    /// <summary>
    /// The name of the method that is called when a contract receives NEP-17 tokens.
    /// </summary>
    public const string OnNEP17Payment = "onNEP17Payment";

    /// <summary>
    /// The name of the method that is called when a contract receives NEP-11 tokens.
    /// </summary>
    public const string OnNEP11Payment = "onNEP11Payment";

    /// <summary>
    /// The name of the method that is called when a contract receives Oracle response.
    /// </summary>
    public const string OnOracleResponse = "onOracleResponse";
}
