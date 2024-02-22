// Copyright (C) 2015-2023 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
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

partial class MethodConvert
{
    private void ConvertNoBody(AccessorDeclarationSyntax syntax)
    {
        _callingConvention = CallingConvention.Cdecl;
        IPropertySymbol property = (IPropertySymbol)Symbol.AssociatedSymbol!;
        AttributeData? attribute = property.GetAttributes().FirstOrDefault(p => p.AttributeClass!.Name == nameof(StoredAttribute));
        using (InsertSequencePoint(syntax))
        {
            _inline = attribute is null;
            ConvertFieldBackedProperty(property);
            if (attribute is not null)
                ConvertStorageBackedProperty(property, attribute);
        }
    }

    private void ConvertFieldBackedProperty(IPropertySymbol property)
    {
        IFieldSymbol[] fields = property.ContainingType.GetAllMembers().OfType<IFieldSymbol>().ToArray();
        if (Symbol.IsStatic)
        {
            IFieldSymbol backingField = Array.Find(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property))!;
            byte backingFieldIndex = context.AddStaticField(backingField);
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
                    throw new CompilationException(Symbol, DiagnosticId.SyntaxNotSupported, $"Unsupported accessor: {Symbol}");
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
                    throw new CompilationException(Symbol, DiagnosticId.SyntaxNotSupported, $"Unsupported accessor: {Symbol}");
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
                key = new byte[] { b };
            }
            else if (attribute.ConstructorArguments[0].Value is string s)
            {
                key = Utility.StrictUTF8.GetBytes(s);
            }
            else
            {
                throw new CompilationException(Symbol, DiagnosticId.SyntaxNotSupported, $"Unknown StorageBacked constructor: {Symbol}");
            }
        }
        return key;
    }

    private void ConvertStorageBackedProperty(IPropertySymbol property, AttributeData attribute)
    {
        IFieldSymbol[] fields = property.ContainingType.GetAllMembers().OfType<IFieldSymbol>().ToArray();
        byte[] key = GetStorageBackedKey(property, attribute);
        if (Symbol.MethodKind == MethodKind.PropertyGet)
        {
            JumpTarget endTarget = new();
            if (Symbol.IsStatic)
            {
                // AddInstruction(OpCode.DUP);
                AddInstruction(OpCode.ISNULL);
                // Ensure that no object was sent
                Jump(OpCode.JMPIFNOT_L, endTarget);
            }
            else
            {
                // Check class
                Jump(OpCode.JMPIF_L, endTarget);
            }
            Push(key);
            Call(ApplicationEngine.System_Storage_GetReadOnlyContext);
            Call(ApplicationEngine.System_Storage_Get);
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
                    // Replace NULL with 0
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ISNULL);
                    JumpTarget ifFalse = new();
                    Jump(OpCode.JMPIFNOT_L, ifFalse);
                    {
                        AddInstruction(OpCode.DROP);
                        AddInstruction(OpCode.PUSH0);
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
                    Call(NativeContract.StdLib.Hash, "deserialize", 1, true);
                    break;
            }
            AddInstruction(OpCode.DUP);
            if (Symbol.IsStatic)
            {
                IFieldSymbol backingField = Array.Find(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property))!;
                byte backingFieldIndex = context.AddStaticField(backingField);
                AccessSlot(OpCode.STSFLD, backingFieldIndex);
            }
            else
            {
                fields = fields.Where(p => !p.IsStatic).ToArray();
                int backingFieldIndex = Array.FindIndex(fields, p => SymbolEqualityComparer.Default.Equals(p.AssociatedSymbol, property));
                AccessSlot(OpCode.LDARG, 0);
                Push(backingFieldIndex);
                AddInstruction(OpCode.ROT);
                AddInstruction(OpCode.SETITEM);
            }
            endTarget.Instruction = AddInstruction(OpCode.NOP);
        }
        else
        {
            if (Symbol.IsStatic)
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
                    Call(NativeContract.StdLib.Hash, "serialize", 1, true);
                    break;
            }
            Push(key);
            Call(ApplicationEngine.System_Storage_GetContext);
            Call(ApplicationEngine.System_Storage_Put);
        }
    }
}
