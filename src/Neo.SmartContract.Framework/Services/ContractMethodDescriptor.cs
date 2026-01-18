// Copyright (C) 2015-2026 The Neo Project.
//
// ContractMethodDescriptor.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.SmartContract.Framework.Services
{
    /// <summary>
    /// Represents a method in the ABI of a contract.
    /// Including name, parameters, return type, the offset in the contract NEF file and  it's safe or not.
    /// </summary>
    public struct ContractMethodDescriptor
    {
        public string Name;
        public ContractParameterDefinition[] Parameters;
        public ContractParameterType ReturnType;
        public int Offset;
        public bool Safe;
    }
}
