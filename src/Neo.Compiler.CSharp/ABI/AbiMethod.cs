// Copyright (C) 2015-2026 The Neo Project.
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
            Safe = GetSafeAttribute(symbol) != null;
            if (Safe && symbol.MethodKind == MethodKind.PropertySet)
                throw new CompilationException(symbol, DiagnosticId.SafeSetter, "Safe setters are not allowed.");
            ReturnType = symbol.ReturnType.GetContractParameterType();
        }

        private static AttributeData? GetSafeAttribute(IMethodSymbol symbol)
        {
            AttributeData? attribute = symbol.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(scfx::Neo.SmartContract.Framework.Attributes.SafeAttribute));
            if (attribute != null) return attribute;
            if (symbol.AssociatedSymbol is IPropertySymbol property)
                return property.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(scfx::Neo.SmartContract.Framework.Attributes.SafeAttribute));
            return null;
        }
    }
}
