// Copyright (C) 2015-2025 The Neo Project.
//
// MethodNotFoundException.cs file belongs to the neo project and is free
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
    /// Exception thrown when a method is not found in the development contract.
    /// </summary>
    public class MethodNotFoundException : Exception
    {
        public MethodNotFoundException(string message) : base(message) { }
        public MethodNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
