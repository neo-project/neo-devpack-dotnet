// Copyright (C) 2015-2024 The Neo Project.
//
// AbiMethod.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using Microsoft.CodeAnalysis;
using Neo.SmartContract;
using System.Linq;

namespace Neo.Compiler.ABI
{
    class AbiMethod : AbiEvent
    {
        public readonly bool Safe;
        public readonly ContractParameterType ReturnType;

        public override IMethodSymbol Symbol { get; }

        public AbiMethod(IMethodSymbol symbol)
            : base(symbol, symbol.GetDisplayName(true), symbol.Parameters.Select(p => p.ToAbiParameter()).ToArray())
        {
            Symbol = symbol;
            Safe = symbol.GetAttributes().Any(p => p.AttributeClass!.Name == nameof(scfx::Neo.SmartContract.Framework.Attributes.SafeAttribute));
            ReturnType = symbol.ReturnType.GetContractParameterType();
        }
    }
}
