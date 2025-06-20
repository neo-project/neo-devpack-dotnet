// Copyright (C) 2015-2025 The Neo Project.
//
// PropertyConvert.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.IO;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;
using scfx::Neo.SmartContract.Framework.Attributes;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    private void ConvertNoBody(AccessorDeclarationSyntax syntax)
    {
        _callingConvention = CallingConvention.Cdecl;
        IPropertySymbol property = (IPropertySymbol)Symbol.AssociatedSymbol!;
        AttributeData? attribute = property.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(StoredAttribute));
        using (InsertSequencePoint(syntax))
        {
            _inline = attribute is null;

            if (attribute is not null)
                ConvertStorageBackedProperty(property, attribute);
            else
                ConvertFieldBackedProperty(property);
        }
    }

    private void ConvertFieldBackedProperty(IPropertySymbol property)
    {
        IFieldSymbol[] fields = property.ContainingType.GetAllMembers().OfType<IFieldSymbol>().ToArray();
        if (!NeedInstanceConstructor(Symbol))
        {
            IFieldSymbol backingField = Array.Find(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property))!;
            byte backingFieldIndex = _context.AddStaticField(backingField);
            switch (Symbol.MethodKind)
            {
                case MethodKind.PropertyGet:
                    AccessSlot(OpCode.LDSFLD, backingFieldIndex);
                    break;
                case MethodKind.PropertySet:
                    if (!_inline) AccessSlot(OpCode.LDARG, 0);
                    AccessSlot(OpCode.STSFLD, backingFieldIndex);
                    break;
                default:
                    throw new CompilationException(Symbol, DiagnosticId.SyntaxNotSupported, $"Unsupported property accessor '{Symbol.MethodKind}' for property '{Symbol.AssociatedSymbol?.Name}'. Only PropertyGet and PropertySet accessors are supported.");
            }
        }
        else
        {
            fields = fields.Where(p => !p.IsStatic).ToArray();
            int backingFieldIndex = Array.FindIndex(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property));
            switch (Symbol.MethodKind)
            {
                case MethodKind.PropertyGet:
                    if (!_inline) AccessSlot(OpCode.LDARG, 0);
                    Push(backingFieldIndex);
                    AddInstruction(OpCode.PICKITEM);
                    break;
                case MethodKind.PropertySet:
                    if (_inline)
                    {
                        Push(backingFieldIndex);
                        AddInstruction(OpCode.ROT);
                    }
                    else
                    {
                        AccessSlot(OpCode.LDARG, 0);
                        Push(backingFieldIndex);
                        AccessSlot(OpCode.LDARG, 1);
                    }
                    AddInstruction(OpCode.SETITEM);
                    break;
                default:
                    throw new CompilationException(Symbol, DiagnosticId.SyntaxNotSupported, $"Unsupported property accessor '{Symbol.MethodKind}' for property '{Symbol.AssociatedSymbol?.Name}'. Only PropertyGet and PropertySet accessors are supported.");
            }
        }
    }

    private byte[] GetStorageBackedKey(IPropertySymbol property, AttributeData attribute)
    {
        byte[] key;

        if (attribute.ConstructorArguments.Length == 0)
        {
            key = Utility.StrictUTF8.GetBytes(property.Name);
        }
        else
        {
            if (attribute.ConstructorArguments[0].Value is byte b)
            {
                key = [b];
            }
            else if (attribute.ConstructorArguments[0].Value is string s)
            {
                key = Utility.StrictUTF8.GetBytes(s);
            }
            else
            {
                throw new CompilationException(Symbol, DiagnosticId.SyntaxNotSupported, $"Invalid StorageBacked attribute constructor for property '{Symbol.AssociatedSymbol?.Name}'. Use StorageBacked() with no parameters, or provide a byte or string key.");
            }
        }
        return key;
    }

    private void ConvertStorageBackedProperty(IPropertySymbol property, AttributeData attribute)
    {
        IFieldSymbol[] fields = property.ContainingType.GetAllMembers().OfType<IFieldSymbol>().ToArray();
        byte[] key = GetStorageBackedKey(property, attribute);
        IFieldSymbol backingField = Array.Find(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property))!;
        // Load the store first, check if its initialized, if not, load the backing field value.
        if (Symbol.MethodKind == MethodKind.PropertyGet)
        {
            Push(key);
            CallInteropMethod(ApplicationEngine.System_Storage_GetReadOnlyContext);
            CallInteropMethod(ApplicationEngine.System_Storage_Get);
            switch (property.Type.Name)
            {
                case "byte":
                case "sbyte":
                case "Byte":
                case "SByte":

                case "short":
                case "ushort":
                case "Int16":
                case "UInt16":

                case "int":
                case "uint":
                case "Int32":
                case "UInt32":

                case "long":
                case "ulong":
                case "Int64":
                case "UInt64":
                case "BigInteger":
                case "bool":
                    // check if its initialized
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ISNULL);
                    JumpTarget ifFalse = new();

                    Jump(OpCode.JMPIFNOT_L, ifFalse);

                    AddInstruction(OpCode.DROP);
                    if (!NeedInstanceConstructor(Symbol))
                    {
                        AccessSlot(OpCode.LDSFLD, _context.AddStaticField(backingField));
                    }
                    else
                    {
                        // Check class
                        fields = fields.Where(p => !p.IsStatic).ToArray();
                        int backingFieldIndex = Array.FindIndex(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property));
                        AccessSlot(OpCode.LDARG, 0);
                        Push(backingFieldIndex);
                        AddInstruction(OpCode.PICKITEM);
                    }

                    ifFalse.Instruction = AddInstruction(OpCode.NOP);
                    break;
                case "String":
                case "ByteString":
                case "UInt160":
                case "UInt256":
                case "ECPoint":
                    break;
                default:
                    CallContractMethod(NativeContract.StdLib.Hash, "deserialize", 1, true);
                    break;
            }
        }
        else
        {
            if (Symbol.IsStatic || !NeedInstanceConstructor(Symbol))
                AccessSlot(OpCode.LDARG, 0);
            else
                AccessSlot(OpCode.LDARG, 1);
            switch (property.Type.Name)
            {
                case "byte":
                case "sbyte":
                case "Byte":
                case "SByte":

                case "short":
                case "ushort":
                case "Int16":
                case "UInt16":

                case "int":
                case "uint":
                case "Int32":
                case "UInt32":

                case "long":
                case "ulong":
                case "Int64":
                case "UInt64":
                case "BigInteger":
                case "String":
                case "ByteString":
                case "UInt160":
                case "UInt256":
                case "ECPoint":
                    break;
                default:
                    CallContractMethod(NativeContract.StdLib.Hash, "serialize", 1, true);
                    break;
            }
            Push(key);
            CallInteropMethod(ApplicationEngine.System_Storage_GetContext);
            CallInteropMethod(ApplicationEngine.System_Storage_Put);
        }
    }
}
