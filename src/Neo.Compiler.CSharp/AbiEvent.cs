// Copyright (C) 2015-2021 The Neo Project.
// 
// The Neo.Compiler.CSharp is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Neo.SmartContract.Manifest;
using System.Linq;

namespace Neo.Compiler
{
    class AbiEvent
    {
        public readonly string Name;
        public readonly ContractParameterDefinition[] Parameters;

        public virtual ISymbol Symbol { get; }

        protected AbiEvent(ISymbol symbol, string name, ContractParameterDefinition[] parameters)
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
