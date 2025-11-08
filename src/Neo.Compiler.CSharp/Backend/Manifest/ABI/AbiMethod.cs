// Copyright (C) 2015-2025 The Neo Project.
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
using System;
using Neo.Compiler.HIR;
using Neo.SmartContract;
using System.Linq;

namespace Neo.Compiler.ABI
{
    class AbiMethod : AbiEvent
    {
        public readonly bool Safe;
        public readonly bool AggressiveInline;
        public readonly ContractParameterType ReturnType;
        public HirNoReentrantAttribute? NoReentrant { get; }

        public override IMethodSymbol Symbol { get; }

        public AbiMethod(IMethodSymbol symbol, HirSignature? signature = null)
            : base(symbol, DetermineName(symbol, signature), symbol.Parameters.Select(p => p.ToAbiParameter()).ToArray())
        {
            Symbol = symbol;
            Safe = DetermineSafe(symbol, signature);
            AggressiveInline = DetermineAggressiveInline(symbol, signature);
            NoReentrant = DetermineNoReentrant(symbol, signature);
            if (Safe && symbol.MethodKind == MethodKind.PropertySet)
                throw new CompilationException(symbol, DiagnosticId.SafeSetter, "Safe setters are not allowed.");
            ReturnType = symbol.ReturnType.GetContractParameterType();
        }

        private static string DetermineName(IMethodSymbol symbol, HirSignature? signature)
        {
            if (signature?.Attributes is { Count: > 0 })
            {
                var alias = signature.Attributes.OfType<HirMethodNameAttribute>().FirstOrDefault();
                if (alias is not null && !string.IsNullOrWhiteSpace(alias.Name))
                    return alias.Name;
            }

            return symbol.GetDisplayName(true);
        }

        private static bool DetermineSafe(IMethodSymbol symbol, HirSignature? signature)
        {
            if (signature?.Attributes is { } attrs && attrs.OfType<HirSafeAttribute>().Any())
                return true;

            return GetSafeAttribute(symbol) != null;
        }

        private static HirNoReentrantAttribute? DetermineNoReentrant(IMethodSymbol symbol, HirSignature? signature)
        {
            if (signature?.Attributes is { } attrs)
            {
                var guard = attrs.OfType<HirNoReentrantAttribute>().FirstOrDefault();
                if (guard is not null)
                    return guard;
            }

            var attribute = GetNoReentrantAttribute(symbol);
            if (attribute is null)
                return null;

            byte prefix = 0xFF;
            string key = "noReentrant";

            if (attribute.ConstructorArguments.Length > 0 &&
                attribute.ConstructorArguments[0].Value is byte prefixValue)
            {
                prefix = prefixValue;
            }

            if (attribute.ConstructorArguments.Length > 1 &&
                attribute.ConstructorArguments[1].Value is string keyValue &&
                !string.IsNullOrWhiteSpace(keyValue))
            {
                key = keyValue;
            }
            else if (string.Equals(attribute.AttributeClass?.Name, nameof(scfx::Neo.SmartContract.Framework.Attributes.NoReentrantMethodAttribute), StringComparison.Ordinal))
            {
                key = symbol.Name;
            }

            return new HirNoReentrantAttribute(prefix, key);
        }

        private static bool DetermineAggressiveInline(IMethodSymbol symbol, HirSignature? signature)
        {
            if (signature?.Attributes is { } attrs && attrs.OfType<HirInlineAttribute>().Any(attr => attr.Aggressive))
                return true;

            return HasAggressiveInlineAttribute(symbol);
        }

        private static AttributeData? GetSafeAttribute(IMethodSymbol symbol)
        {
            AttributeData? attribute = symbol.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(scfx::Neo.SmartContract.Framework.Attributes.SafeAttribute));
            if (attribute != null) return attribute;
            if (symbol.AssociatedSymbol is IPropertySymbol property)
                return property.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(scfx::Neo.SmartContract.Framework.Attributes.SafeAttribute));
            return null;
        }

        private static AttributeData? GetNoReentrantAttribute(IMethodSymbol symbol)
        {
            foreach (var attribute in symbol.GetAttributes())
            {
                var name = attribute.AttributeClass?.Name;
                if (name is nameof(scfx::Neo.SmartContract.Framework.Attributes.NoReentrantAttribute) or nameof(scfx::Neo.SmartContract.Framework.Attributes.NoReentrantMethodAttribute))
                    return attribute;
            }

            if (symbol.AssociatedSymbol is IPropertySymbol property)
            {
                foreach (var attribute in property.GetAttributes())
                {
                    var name = attribute.AttributeClass?.Name;
                    if (name is nameof(scfx::Neo.SmartContract.Framework.Attributes.NoReentrantAttribute) or nameof(scfx::Neo.SmartContract.Framework.Attributes.NoReentrantMethodAttribute))
                        return attribute;
                }
            }

            return null;
        }

        private static bool HasAggressiveInlineAttribute(IMethodSymbol symbol)
        {
            if (symbol.GetAttributes().Any(IsAggressiveInlineAttribute))
                return true;

            if (symbol.AssociatedSymbol is IPropertySymbol property && property.GetAttributes().Any(IsAggressiveInlineAttribute))
                return true;

            return false;
        }

        private static bool IsAggressiveInlineAttribute(AttributeData attribute)
        {
            var attributeClass = attribute.AttributeClass;
            if (attributeClass is null)
                return false;

            if (!string.Equals(attributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), "System.Runtime.CompilerServices.MethodImplAttribute", StringComparison.Ordinal))
                return false;

            if (attribute.ConstructorArguments.Length > 0 && attribute.ConstructorArguments[0].Value is int ctorValue)
            {
                if ((((System.Runtime.CompilerServices.MethodImplOptions)ctorValue) & System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining) != 0)
                    return true;
            }

            foreach (var (name, typedValue) in attribute.NamedArguments)
            {
                if (string.Equals(name, "Value", StringComparison.Ordinal) && typedValue.Value is int namedValue)
                {
                    if ((((System.Runtime.CompilerServices.MethodImplOptions)namedValue) & System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining) != 0)
                        return true;
                }
            }

            return false;
        }
    }
}
