extern alias scfx;

using Microsoft.CodeAnalysis;
using Neo.Cryptography.ECC;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler
{
    static class Helper
    {
        public static byte[] HexToBytes(this string hex, bool removePrefix)
        {
            ReadOnlySpan<char> s = hex;
            if (removePrefix && hex.StartsWith("0x"))
                s = s[2..];
            return Convert.FromHexString(s);
        }

        public static ContractParameterType GetContractParameterType(this ITypeSymbol type)
        {
            if (type.TypeKind == TypeKind.Enum) return ContractParameterType.Integer;
            if (type.IsValueType) return ContractParameterType.Array;
            if (type is IArrayTypeSymbol array)
                if (array.ElementType.SpecialType == SpecialType.System_Byte)
                    return ContractParameterType.ByteArray;
                else
                    return ContractParameterType.Array;
            if (type.AllInterfaces.Any(p => p.Name == nameof(scfx::Neo.SmartContract.Framework.IApiInterface)))
                return ContractParameterType.InteropInterface;
            return type.SpecialType switch
            {
                SpecialType.System_Object => ContractParameterType.Any,
                SpecialType.System_Void => ContractParameterType.Void,
                SpecialType.System_Boolean => ContractParameterType.Boolean,
                SpecialType.System_Char => ContractParameterType.Integer,
                SpecialType.System_SByte => ContractParameterType.Integer,
                SpecialType.System_Byte => ContractParameterType.Integer,
                SpecialType.System_Int16 => ContractParameterType.Integer,
                SpecialType.System_UInt16 => ContractParameterType.Integer,
                SpecialType.System_Int32 => ContractParameterType.Integer,
                SpecialType.System_UInt32 => ContractParameterType.Integer,
                SpecialType.System_Int64 => ContractParameterType.Integer,
                SpecialType.System_UInt64 => ContractParameterType.Integer,
                SpecialType.System_String => ContractParameterType.String,
                _ => type.Name switch
                {
                    nameof(BigInteger) => ContractParameterType.Integer,
                    nameof(UInt160) => ContractParameterType.Hash160,
                    nameof(UInt256) => ContractParameterType.Hash256,
                    nameof(ECPoint) => ContractParameterType.PublicKey,
                    nameof(ByteString) => ContractParameterType.ByteArray,
                    _ => ContractParameterType.Any
                }
            };
        }

        public static StackItemType GetStackItemType(this ITypeSymbol type)
        {
            return type.SpecialType switch
            {
                SpecialType.System_Boolean => StackItemType.Boolean,
                SpecialType.System_Char => StackItemType.Integer,
                SpecialType.System_SByte => StackItemType.Integer,
                SpecialType.System_Byte => StackItemType.Integer,
                SpecialType.System_Int16 => StackItemType.Integer,
                SpecialType.System_UInt16 => StackItemType.Integer,
                SpecialType.System_Int32 => StackItemType.Integer,
                SpecialType.System_UInt32 => StackItemType.Integer,
                SpecialType.System_Int64 => StackItemType.Integer,
                SpecialType.System_UInt64 => StackItemType.Integer,
                _ => type.Name switch
                {
                    nameof(BigInteger) => StackItemType.Integer,
                    _ => StackItemType.Any
                }
            };
        }

        public static StackItemType GetPatternType(this ITypeSymbol type)
        {
            return type.ToString() switch
            {
                "bool" => StackItemType.Boolean,
                "byte[]" => StackItemType.Buffer,
                "string" => StackItemType.ByteString,
                "Neo.SmartContract.Framework.ByteString" => StackItemType.ByteString,
                "System.Numerics.BigInteger" => StackItemType.Integer,
                _ => throw new NotSupportedException($"Unsupported pattern type: {type}")
            };
        }

        public static IFieldSymbol[] GetFields(this ITypeSymbol type)
        {
            return type.GetMembers().OfType<IFieldSymbol>().Where(p => !p.IsStatic).ToArray();
        }

        public static string GetDisplayName(this ISymbol symbol, bool lowercase = false)
        {
            AttributeData? attribute = symbol.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(DisplayNameAttribute));
            if (attribute is not null) return (string)attribute.ConstructorArguments[0].Value!;
            if (symbol is IMethodSymbol method)
            {
                switch (method.MethodKind)
                {
                    case MethodKind.Constructor:
                        symbol = method.ContainingType;
                        break;
                    case MethodKind.PropertyGet:
                        ISymbol property = method.AssociatedSymbol!;
                        attribute = property.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(DisplayNameAttribute));
                        if (attribute is not null) return (string)attribute.ConstructorArguments[0].Value!;
                        symbol = property;
                        break;
                    case MethodKind.PropertySet:
                        return "set" + symbol.Name[4..];
                    case MethodKind.StaticConstructor:
                        return "_initialize";
                }
            }
            if (lowercase)
                return symbol.Name[..1].ToLowerInvariant() + symbol.Name[1..];
            else
                return symbol.Name;
        }

        public static ContractParameterDefinition ToAbiParameter(this IParameterSymbol symbol)
        {
            return new ContractParameterDefinition
            {
                Name = symbol.Name,
                Type = symbol.Type.GetContractParameterType()
            };
        }

        public static void RebuildOffset(this IEnumerable<Instruction> instructions)
        {
            int offset = 0;
            foreach (Instruction instruction in instructions)
            {
                instruction.Offset = offset;
                offset += instruction.Size;
            }
        }
    }
}
