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
using System.Text.RegularExpressions;

namespace Neo.Compiler.ABI
{
    class AbiMethod : AbiEvent
    {
        public readonly bool Safe;
        public readonly ContractParameterType ReturnType;
        public readonly AbiFee? Fee;

        public override IMethodSymbol Symbol { get; }

        public AbiMethod(IMethodSymbol symbol)
            : base(symbol, symbol.GetDisplayName(true), symbol.Parameters.Select(p => p.ToAbiParameter()).ToArray())
        {
            Symbol = symbol;
            Safe = GetSafeAttribute(symbol) != null;
            if (Safe && symbol.MethodKind == MethodKind.PropertySet)
                throw new CompilationException(symbol, DiagnosticId.SafeSetter, "Safe setters are not allowed.");
            ReturnType = symbol.ReturnType.GetContractParameterType();
            Fee = GetFeeFromAttribute(symbol);
        }

        private static AttributeData? GetSafeAttribute(IMethodSymbol symbol)
        {
            AttributeData? attribute = symbol.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(scfx::Neo.SmartContract.Framework.Attributes.SafeAttribute));
            if (attribute != null) return attribute;
            if (symbol.AssociatedSymbol is IPropertySymbol property)
                return property.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(scfx::Neo.SmartContract.Framework.Attributes.SafeAttribute));
            return null;
        }

        private static AbiFee? GetFeeFromAttribute(IMethodSymbol symbol)
        {
            AttributeData? attribute = symbol.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == "FeeAttribute");
            if (attribute == null && symbol.AssociatedSymbol is IPropertySymbol property)
                attribute = property.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == "FeeAttribute");

            if (attribute == null) return null;

            long amount = 0;
            string beneficiary = string.Empty;
            string mode = "fixed";
            string? calculator = null;

            foreach (var namedArg in attribute.NamedArguments)
            {
                switch (namedArg.Key)
                {
                    case "Amount":
                        amount = (long)namedArg.Value.Value!;
                        break;
                    case "Beneficiary":
                        beneficiary = (string)namedArg.Value.Value!;
                        break;
                    case "Mode":
                        mode = (int)namedArg.Value.Value! == 0 ? "fixed" : "dynamic";
                        break;
                    case "Calculator":
                        calculator = (string)namedArg.Value.Value!;
                        break;
                }
            }

            // Validation: Beneficiary is required
            if (string.IsNullOrEmpty(beneficiary))
                throw new CompilationException(symbol, DiagnosticId.FeeMissingBeneficiary, "Fee attribute requires a Beneficiary address.");

            // Validation: Beneficiary format (Neo address or script hash)
            if (!IsValidNeoAddress(beneficiary) && !IsValidScriptHash(beneficiary))
                throw new CompilationException(symbol, DiagnosticId.FeeInvalidBeneficiary, $"Invalid Beneficiary format: '{beneficiary}'. Must be a valid Neo address or script hash.");

            // Validation: Dynamic mode requires Calculator
            if (mode == "dynamic" && string.IsNullOrEmpty(calculator))
                throw new CompilationException(symbol, DiagnosticId.FeeMissingCalculator, "Dynamic fee mode requires a Calculator script hash.");

            // Debug trace for high fee amounts (> 10 GAS)
            const long highFeeThreshold = 1000000000;
            if (mode == "fixed" && amount > highFeeThreshold)
            {
                System.Diagnostics.Debug.WriteLine($"High fee: {amount / 100000000.0} GAS on {symbol.Name}");
            }

            return new AbiFee(amount, beneficiary, mode, calculator);
        }

        private static bool IsValidNeoAddress(string address)
        {
            // Neo N3 addresses start with 'N' and are 34 characters long (Base58Check)
            return !string.IsNullOrEmpty(address) &&
                   address.Length == 34 &&
                   address.StartsWith("N") &&
                   Regex.IsMatch(address, @"^N[1-9A-HJ-NP-Za-km-z]{33}$");
        }

        private static bool IsValidScriptHash(string hash)
        {
            // Script hash format: 0x followed by 40 hex characters
            return !string.IsNullOrEmpty(hash) &&
                   hash.Length == 42 &&
                   hash.StartsWith("0x") &&
                   Regex.IsMatch(hash[2..], @"^[0-9a-fA-F]{40}$");
        }
    }
}
