// Copyright (C) 2015-2025 The Neo Project.
//
// SmartContractInitialize.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.SmartContract.Testing
{
    public class SmartContractInitialize
    {
        /// <summary>
        /// Engine
        /// </summary>
        public TestEngine Engine { get; }

        /// <summary>
        /// Hash
        /// </summary>
        public UInt160 Hash { get; }

        /// <summary>
        /// ContractId
        /// </summary>
        internal int? ContractId { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="hash">Hash</param>
        /// <param name="contractId">Contract Id</param>
        internal SmartContractInitialize(TestEngine engine, UInt160 hash, int? contractId = null)
        {
            Engine = engine;
            Hash = hash;
            ContractId = contractId;
        }
    }
}
