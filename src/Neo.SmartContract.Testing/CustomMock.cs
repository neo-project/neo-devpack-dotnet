// Copyright (C) 2015-2024 The Neo Project.
//
// CustomMock.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Reflection;

namespace Neo.SmartContract.Testing
{
    internal class CustomMock
    {
        /// <summary>
        /// Mocked contract
        /// </summary>
        public SmartContract Contract { get; }

        /// <summary>
        /// Mocked method
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contract">Contract</param>
        /// <param name="method">Method</param>
        public CustomMock(SmartContract contract, MethodInfo method)
        {
            Contract = contract;
            Method = method;
        }
    }
}
