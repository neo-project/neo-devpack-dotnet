// Copyright (C) 2015-2025 The Neo Project.
//
// AbiEvent.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Neo.SmartContract.Manifest;
using System.Linq;

namespace Neo.Compiler.ABI
{
    class AbiEvent
    {
        public readonly string Name;
        public readonly ContractParameterDefinition[] Parameters;

        public virtual ISymbol Symbol { get; }

        public AbiEvent(ISymbol symbol, string name, params ContractParameterDefinition[] parameters)
        {
            Symbol = symbol;
            Name = name;
            Parameters = parameters;
        }

        public AbiEvent(IEventSymbol symbol)
            : this(symbol, symbol.GetDisplayName(), ((INamedTypeSymbol)symbol.Type).DelegateInvokeMethod!.Parameters.Select(p => p.ToAbiParameter()).ToArray())
        {
        }
    }
}
