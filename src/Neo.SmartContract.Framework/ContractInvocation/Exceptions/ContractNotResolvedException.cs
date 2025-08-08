// Copyright (C) 2015-2025 The Neo Project.
//
// ContractNotResolvedException.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;

namespace Neo.SmartContract.Framework.ContractInvocation.Exceptions
{
    /// <summary>
    /// Exception thrown when a contract reference cannot be resolved to an address.
    /// </summary>
    public class ContractNotResolvedException : ContractInvocationException
    {
        public ContractNotResolvedException(string message) : base(message) { }
        public ContractNotResolvedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
