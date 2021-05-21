extern alias scfx;

using Microsoft.CodeAnalysis;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using Neo.VM.Types;
using System;
using System.Buffers.Binary;
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

        public static bool IsSubclassOf(this ITypeSymbol type, string baseTypeName)
        {
            INamedTypeSymbol? baseType = type.BaseType;
            while (baseType is not null)
            {
                if (baseType.Name == baseTypeName)
                    return true;
                baseType = baseType.BaseType;
            }
            return false;
        }

        public static bool IsVirtualMethod(this IMethodSymbol method)
        {
            return method.IsAbstract || method.IsVirtual || method.IsOverride;
        }

        public static bool IsInternalCoreMethod(this IMethodSymbol method)
        {
            return method.Name switch
            {
                "_initialize" => true,
                "_deploy" => true,
                _ => false,
            };
        }

        public static ContractParameterType GetContractParameterType(this ITypeSymbol type)
        {
            switch (type.ToString())
            {
                case "void": return ContractParameterType.Void;
                case "bool": return ContractParameterType.Boolean;
                case "char": return ContractParameterType.Integer;
                case "sbyte": return ContractParameterType.Integer;
                case "byte": return ContractParameterType.Integer;
                case "short": return ContractParameterType.Integer;
                case "ushort": return ContractParameterType.Integer;
                case "int": return ContractParameterType.Integer;
                case "uint": return ContractParameterType.Integer;
                case "long": return ContractParameterType.Integer;
                case "ulong": return ContractParameterType.Integer;
                case "string": return ContractParameterType.String;
                case "byte[]": return ContractParameterType.ByteArray;
                case "object": return ContractParameterType.Any;
                case "Neo.Cryptography.ECC.ECPoint": return ContractParameterType.PublicKey;
                case "Neo.SmartContract.Framework.ByteString": return ContractParameterType.ByteArray;
                case "Neo.UInt160": return ContractParameterType.Hash160;
                case "Neo.UInt256": return ContractParameterType.Hash256;
                case "System.Numerics.BigInteger": return ContractParameterType.Integer;
            }
            if (type.Name == "Map") return ContractParameterType.Map;
            if (type.Name == "List") return ContractParameterType.Array;
            if (type.TypeKind == TypeKind.Enum) return ContractParameterType.Integer;
            if (type is IArrayTypeSymbol) return ContractParameterType.Array;
            if (type.AllInterfaces.Any(p => p.Name == nameof(scfx::Neo.SmartContract.Framework.IApiInterface)))
                return ContractParameterType.InteropInterface;
            if (type.IsValueType) return ContractParameterType.Array;
            return ContractParameterType.Any;
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
                _ => throw new CompilationException(type, DiagnosticId.SyntaxNotSupported, $"Unsupported pattern type: {type}")
            };
        }

        public static ISymbol[] GetAllMembers(this ITypeSymbol type)
        {
            return GetAllMembersInternal(type).ToArray();
        }

        private static IEnumerable<ISymbol> GetAllMembersInternal(ITypeSymbol type)
        {
            if (type.SpecialType == SpecialType.System_Object) yield break;
            List<ISymbol> myMembers = type.GetMembers().ToList();
            if (type.IsReferenceType)
                foreach (ISymbol member in GetAllMembersInternal(type.BaseType!))
                {
                    if (member is IMethodSymbol method && (method.MethodKind == MethodKind.Constructor || method.MethodKind == MethodKind.StaticConstructor))
                    {
                        continue;
                    }
                    else if (member.IsAbstract || member.IsVirtual || member.IsOverride)
                    {
                        int index = myMembers.FindIndex(p => p is IMethodSymbol method && SymbolEqualityComparer.Default.Equals(method.OverriddenMethod, member));
                        if (index >= 0)
                        {
                            yield return myMembers[index];
                            myMembers.RemoveAt(index);
                        }
                        else
                        {
                            yield return member;
                        }
                    }
                    else
                    {
                        yield return member;
                    }
                }
            foreach (ISymbol member in myMembers)
            {
                yield return member;
            }
        }

        public static IFieldSymbol[] GetFields(this ITypeSymbol type)
        {
            return type.GetAllMembers().OfType<IFieldSymbol>().Where(p => !p.IsStatic).ToArray();
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

        public static void RebuildOffsets(this IReadOnlyList<Instruction> instructions)
        {
            int offset = 0;
            foreach (Instruction instruction in instructions)
            {
                instruction.Offset = offset;
                offset += instruction.Size;
            }
        }

        public static void RebuildOperands(this IReadOnlyList<Instruction> instructions)
        {
            foreach (Instruction instruction in instructions)
            {
                if (instruction.Target is null) continue;
                bool isLong;
                if (instruction.OpCode >= OpCode.JMP && instruction.OpCode <= OpCode.CALL_L)
                    isLong = (instruction.OpCode - OpCode.JMP) % 2 != 0;
                else
                    isLong = instruction.OpCode == OpCode.PUSHA || instruction.OpCode == OpCode.CALLA || instruction.OpCode == OpCode.TRY_L || instruction.OpCode == OpCode.ENDTRY_L;
                if (instruction.OpCode == OpCode.TRY || instruction.OpCode == OpCode.TRY_L)
                {
                    int offset1 = (instruction.Target.Instruction?.Offset - instruction.Offset) ?? 0;
                    int offset2 = (instruction.Target2!.Instruction?.Offset - instruction.Offset) ?? 0;
                    if (isLong)
                    {
                        instruction.Operand = new byte[sizeof(int) + sizeof(int)];
                        BinaryPrimitives.WriteInt32LittleEndian(instruction.Operand, offset1);
                        BinaryPrimitives.WriteInt32LittleEndian(instruction.Operand.AsSpan(sizeof(int)), offset2);
                    }
                    else
                    {
                        instruction.Operand = new byte[sizeof(sbyte) + sizeof(sbyte)];
                        sbyte sbyte1 = checked((sbyte)offset1);
                        sbyte sbyte2 = checked((sbyte)offset2);
                        instruction.Operand[0] = unchecked((byte)sbyte1);
                        instruction.Operand[1] = unchecked((byte)sbyte2);
                    }
                }
                else
                {
                    int offset = instruction.Target.Instruction!.Offset - instruction.Offset;
                    if (isLong)
                    {
                        instruction.Operand = BitConverter.GetBytes(offset);
                    }
                    else
                    {
                        sbyte sbyte1 = checked((sbyte)offset);
                        instruction.Operand = new[] { unchecked((byte)sbyte1) };
                    }
                }
            }
        }
    }
}
